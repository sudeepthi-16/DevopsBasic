pipeline {
    agent any

    environment {
        BACKEND_IMAGE = "devopsbasic-backendjenkins"
        FRONTEND_IMAGE = "devopsbasic-frontendjenkins"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Run Unit Tests') {
    steps {
        echo "Running .NET Unit Tests..."

        catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {

            bat """
                cd DevopsBasic.Tests

                dotnet restore
                dotnet build --configuration Debug

                REM Run tests and generate TRX report
                dotnet test --no-build --logger "trx;LogFileName=test-results.trx" --results-directory TestResults

                REM Convert TRX -> JUnit XML using trx2junit
                trx2junit TestResults\\test-results.trx TestResults\\
            """
        }

        // Publish JUnit XML results so Jenkins shows test reports
        junit allowEmptyResults: true, testResults: 'DevopsBasic.Tests/TestResults/*.xml'
    }
}
     

        stage('Build Backend Docker Image') {
            steps {
                bat """
                    docker build -t %BACKEND_IMAGE%:latest ./DevopsBasic
                """
            }
        }

        stage('Build Frontend Docker Image') {
            steps {
                bat """
                    docker build -t %FRONTEND_IMAGE%:latest ./students-ui
                """
            }
        }

        stage('Deploy using Docker Compose') {
            steps {
                bat """
                    docker-compose down --remove-orphans
                    docker-compose build
                    docker-compose up -d
                """
            }
        }
    }

    post {
        success {
            echo "Deployment SUCCESSFUL!"
            echo "Frontend: http://localhost:8080"
            echo "Backend:  http://localhost:5000"
        }
        failure {
            echo "Deployment FAILED. Check logs."
        }
    }
}
