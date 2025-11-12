pipeline {
  agent any

  options {
    timestamps()
  }

  environment {
    BACKEND_DIR      = 'DevopsBasic'         // folder containing DevopsBasic.sln
    TEST_PROJECT_DIR = 'DevopsBasic.Tests'   // folder containing test project(s)
    FRONTEND_DIR     = 'students-ui'
    TEST_RESULTS_DIR = 'TestResults'         // repo-root test results folder
    DOTNET_TOOLS_UNIX = "${env.HOME}/.dotnet/tools"
    DOTNET_TOOLS_WIN  = "%USERPROFILE%\\.dotnet\\tools"
    CONFIGURATION = "Release"
  }

  stages {

    stage('Checkout Code') {
      steps {
        echo "Checking out repository..."
        checkout scm
        echo "Workspace: ${env.WORKSPACE}"
        // Show workspace contents to debug path issues
        script {
          if (isUnix()) {
            sh 'pwd; ls -la'
          } else {
            bat 'chdir & dir /a'
          }
        }
      }
    }

    stage('Restore & Build (.NET)') {
      steps {
        echo "Restoring and building .NET solution from ${BACKEND_DIR}..."
        dir("${BACKEND_DIR}") {
          script {
            if (isUnix()) {
              sh "dotnet restore DevopsBasic.sln"
              sh "dotnet build DevopsBasic.sln -c ${CONFIGURATION} --no-restore"
            } else {
              bat "dotnet restore DevopsBasic.sln"
              bat "dotnet build DevopsBasic.sln -c ${CONFIGURATION} --no-restore"
            }
          }
        }
      }
    }

    stage('Run Backend Tests (.NET)') {
      steps {
        echo "Running backend tests in ${TEST_PROJECT_DIR} and saving TRX to ${TEST_RESULTS_DIR}..."
        script {
          // ensure TestResults exists at repo root
          if (isUnix()) {
            sh "mkdir -p ${TEST_RESULTS_DIR}"
            // run tests from the test project folder (explicit)
            dir("${TEST_PROJECT_DIR}") {
              sh "dotnet test -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ../${TEST_RESULTS_DIR} || true"
            }
            // install/convert TRX if trx2junit available
            sh '''
              dotnet tool install -g trx2junit || true
              export PATH="$PATH:${DOTNET_TOOLS_UNIX}"
              for f in ${TEST_RESULTS_DIR}/*.trx; do
                [ -f "$f" ] && trx2junit "$f" -o "${f%.trx}.xml" || true
              done
            '''.stripIndent()
          } else {
            bat "if not exist ${TEST_RESULTS_DIR} mkdir ${TEST_RESULTS_DIR}"
            dir("${TEST_PROJECT_DIR}") {
              bat "dotnet test -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ..\\${TEST_RESULTS_DIR} || exit /b 0"
            }
            bat """
              dotnet tool install -g trx2junit || powershell -Command "Write-Host 'trx2junit maybe already installed'"
              set PATH=%PATH%;${DOTNET_TOOLS_WIN}
              for %%F in (${TEST_RESULTS_DIR}\\*.trx) do (
                if exist "%%F" (
                  ${DOTNET_TOOLS_WIN}\\trx2junit.exe "%%F" -o "%%~dpnF.xml" || echo convert failed for %%F
                )
              )
            """
          }
        }
      }
      post {
        always {
          junit allowEmptyResults: true, testResults: "${TEST_RESULTS_DIR}/*.xml"
          archiveArtifacts artifacts: "${TEST_RESULTS_DIR}/**/*", allowEmptyArchive: true, fingerprint: true
        }
      }
    }

    stage('Build Frontend (Angular)') {
      steps {
        echo "Building frontend in ${FRONTEND_DIR}..."
        dir("${FRONTEND_DIR}") {
          script {
            if (isUnix()) {
              sh 'npm ci'
              sh 'npm run build --if-present'
            } else {
              bat 'npm ci'
              bat 'npm run build --if-present'
            }
          }
        }
      }
    }

    stage('Run Frontend Tests (Angular)') {
      steps {
        echo "Running frontend tests in ${FRONTEND_DIR}..."
        dir("${FRONTEND_DIR}") {
          script {
            if (isUnix()) {
              sh 'mkdir -p test-results || true'
              sh 'npm test -- --watch=false || true'
            } else {
              bat 'if not exist test-results mkdir test-results'
              bat 'npm test -- --watch=false || exit /b 0'
            }
          }
        }
      }
      post {
        always {
          junit allowEmptyResults: true, testResults: "${FRONTEND_DIR}/test-results/*.xml"
          archiveArtifacts artifacts: "${FRONTEND_DIR}/test-results/**/*.*", allowEmptyArchive: true
        }
      }
    }

    stage('Build Docker Images (Optional)') {
      steps {
        echo "Optional: building docker images via docker compose (if present)"
        script {
          if (fileExists('docker-compose.yml')) {
            if (isUnix()) {
              sh 'docker compose build || true'
            } else {
              bat 'docker compose build || exit /b 0'
            }
          } else {
            echo "No docker-compose.yml found; skipping docker build"
          }
        }
      }
    }

    stage('Summary') {
      steps {
        echo "Pipeline finished. Check Tests and Artifacts tabs."
      }
    }
  }

  post {
    success { echo "Pipeline succeeded ✅" }
    unstable { echo "Pipeline unstable ⚠️" }
    failure { echo "Pipeline failed ❌" }
    always { echo "Done. Build status: ${currentBuild.currentResult}" }
  }
}
