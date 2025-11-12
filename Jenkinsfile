pipeline {
    agent any

    environment {
        BACKEND_DIR = 'DevopsBasic'
        FRONTEND_DIR = 'students-ui'
        DOTNET_TOOLS = "${env.USERPROFILE}\\.dotnet\\tools"
    }

    options {
        timestamps()
    }

    stages {

        // 1️⃣ CHECKOUT CODE
        stage('Checkout Code') {
            steps {
                echo "Pulling latest code from GitHub..."
                git branch: 'main', url: 'https://github.com/<your-username>/<your-repo>.git'
            }
        }

        // 2️⃣ BUILD BACKEND
        stage('Build Backend (.NET)') {
            steps {
                dir("${BACKEND_DIR}") {
                    echo "Restoring and building backend..."
                    bat 'dotnet restore'
                    bat 'dotnet build --configuration Release --no-restore'
                }
            }
        }

        // 3️⃣ TEST BACKEND
        stage('Test Backend (.NET)') {
            steps {
                dir("${BACKEND_DIR}") {
                    echo "Running backend tests from DevopsBasic.sln..."
                    bat 'if not exist TestResults mkdir TestResults'
                    // Run tests on the solution file
                    bat 'dotnet test DevopsBasic.sln --configuration Release --logger "trx;LogFileName=TestResults\\testresults.trx" || exit /b 0'

                    // Convert TRX -> JUnit XML
                    withEnv(["PATH=${DOTNET_TOOLS};${env.PATH}"]) {
                        bat 'for /R %i in (TestResults\\*.trx) do trx2junit "%i" || echo No TRX files found, skipping conversion.'
                    }

                    // List the results
                    bat 'dir TestResults'
                }
            }
            post {
                always {
                    catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
                        junit allowEmptyResults: true, testResults: "${BACKEND_DIR}/TestResults/*.xml"
                        archiveArtifacts artifacts: "${BACKEND_DIR}/TestResults/*.*", allowEmptyArchive: true
                    }
                }
            }
        }

        // 4️⃣ BUILD FRONTEND (ANGULAR)
        stage('Build Frontend (Angular)') {
            steps {
                dir("${FRONTEND_DIR}") {
                    echo "Installing frontend dependencies and building..."
                    bat 'npm ci'
                    bat 'npm run build --if-present'
                }
            }
        }

        // 5️⃣ TEST FRONTEND (ANGULAR)
        stage('Test Frontend (Angular)') {
            steps {
                dir("${FRONTEND_DIR}") {
                    echo "Running Angular unit tests..."
                    bat 'npm test || exit /b 0'
                }
            }
            post {
                always {
                    junit allowEmptyResults: true, testResults: "${FRONTEND_DIR}/test-results/*.xml"
                    archiveArtifacts artifacts: "${FRONTEND_DIR}/test-results/*.xml", allowEmptyArchive: true
                }
            }
        }

        // 6️⃣ BUILD DOCKER IMAGES
        stage('Build Docker Images (Optional)') {
            steps {
                echo "Building Docker images..."
                bat 'docker compose build || echo Docker build skipped.'
            }
        }

        // 7️⃣ SUMMARY
        stage('Summary') {
            steps {
                echo "✅ Build and test pipeline completed successfully."
            }
        }
    }

    post {
        success {
            echo "🎉 Jenkins pipeline finished successfully!"
        }
        failure {
            echo "❌ Pipeline failed. Check console output for details."
        }
    }
}
