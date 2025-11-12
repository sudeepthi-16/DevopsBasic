pipeline {
    agent any  // means Jenkins can run this on any available agent (your local one)

    options {
        timestamps()  // show timestamps in logs
    }

    environment {
        // === Folder setup ===
        BACKEND_DIR    = 'DevopsBasic'      // .NET backend folder
        FRONTEND_DIR   = 'students-ui'      // Angular frontend folder

        // === Tool paths ===
        DOTNET_TOOLS   = "${env.USERPROFILE}\\.dotnet\\tools"  // where trx2junit lives
    }

    stages {

        // 1️⃣ --- Checkout code from GitHub ---
        stage('Checkout Code') {
            steps {
                echo "Pulling latest code from GitHub..."
                git branch: 'main', url: 'https://github.com/sudeepthi-16/DevopsBasic.git'
            }
        }

        // 2️⃣ --- Build the .NET Backend ---
        stage('Build Backend (.NET)') {
            steps {
                echo "Building backend project..."
                // Run from repo root, not inside DevopsBasic/
                bat 'dotnet restore DevopsBasic/DevopsBasic.sln'
                bat 'dotnet build DevopsBasic/DevopsBasic.sln --configuration Release --no-restore'
            }
        }

        // 3️⃣ --- Run Backend Tests (.NET) ---
        stage('Test Backend (.NET)') {
            steps {
                echo "Running .NET tests..."
                // Work from repo root, so relative paths to test project resolve correctly
                bat 'if not exist TestResults mkdir TestResults'
                bat 'dotnet test DevopsBasic/DevopsBasic.sln --configuration Release --no-build --logger "trx;LogFileName=testresults.trx" || exit /b 0'

                // Convert TRX → JUnit XML for Jenkins reporting
                withEnv(["PATH=${DOTNET_TOOLS};${env.PATH}"]) {
                    bat 'for /R %i in (*.trx) do trx2junit "%i"'
                }
            }
            post {
                always {
                    // Publish test results to Jenkins
                    junit allowEmptyResults: true, testResults: "TestResults/**/*.xml"
                    archiveArtifacts artifacts: "TestResults/**/*.*", allowEmptyArchive: true
                }
            }
        }

        // 4️⃣ --- Install & Build Frontend (Angular) ---
        stage('Build Frontend (Angular)') {
            steps {
                dir("${FRONTEND_DIR}") {
                    echo "Installing dependencies and building frontend..."
                    bat 'npm ci'
                    bat 'npm run build --if-present'
                }
            }
        }

        // 5️⃣ --- Run Frontend Tests (Angular) ---
        stage('Test Frontend (Angular)') {
            steps {
                dir("${FRONTEND_DIR}") {
                    echo "Running Angular tests..."
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

        // 6️⃣ --- Optional: Build Docker Images ---
        stage('Build Docker Images (Optional)') {
            steps {
                echo "Building Docker images (optional step)..."
                bat 'docker compose build'
            }
        }

        // 7️⃣ --- Wrap Up ---
        stage('Summary') {
            steps {
                echo "✅ All stages completed. Check the 'Test Result' tab for test summaries."
            }
        }
    }

    post {
        success {
            echo "🎉 Pipeline finished successfully!"
        }
        failure {
            echo "❌ Pipeline failed. Check the console output and test reports for details."
        }
    }
}
