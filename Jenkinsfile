pipeline {
    agent any

    environment {
        BACKEND_IMAGE = "devopsbasic-backend"
        FRONTEND_IMAGE = "devopsbasic-frontend"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
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
