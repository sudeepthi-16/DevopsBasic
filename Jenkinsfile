pipeline {
  agent any

  options {
    timestamps()
  }

  environment {
    // folders (relative to repo root)
    BACKEND_DIR     = 'DevopsBasic'            // contains DevopsBasic.sln (adjust if necessary)
    TEST_PROJECT_DIR= 'DevopsBasic.Tests'      // test project folder (adjust if necessary)
    FRONTEND_DIR    = 'students-ui'

    // test results directory at repo root (single place for all test outputs)
    TEST_RESULTS_DIR = 'TestResults'

    // dotnet global tool locations (will be used for trx2junit)
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
      }
    }

    stage('Restore & Build (.NET)') {
      steps {
        echo "Restoring and building .NET solution..."
        // restore/build from repo root to ensure solution references resolve
        script {
          if (isUnix()) {
            sh "dotnet restore"
            sh "dotnet build -c ${CONFIGURATION} --no-restore"
          } else {
            bat "dotnet restore"
            bat "dotnet build -c ${CONFIGURATION} --no-restore"
          }
        }
      }
    }

    stage('Run Backend Tests (.NET)') {
      steps {
        echo "Running backend tests and collecting results..."
        script {
          // ensure test results dir exists at repo root
          if (isUnix()) {
            sh "mkdir -p ${TEST_RESULTS_DIR}"
            // run the test project explicitly — safer than running entire solution
            sh "dotnet test ${TEST_PROJECT_DIR} -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ${TEST_RESULTS_DIR} || true"
            // convert any TRX -> JUnit (install tracer tool if necessary)
            sh '''
              dotnet tool install -g trx2junit || true
              export PATH="$PATH:${DOTNET_TOOLS_UNIX}"
              for f in ${TEST_RESULTS_DIR}/*.trx; do
                if [ -f "$f" ]; then
                  trx2junit "$f" -o "${TEST_RESULTS_DIR}/$(basename "$f" .trx).xml" || true
                fi
              done
            '''.stripIndent()
          } else {
            bat "if not exist ${TEST_RESULTS_DIR} mkdir ${TEST_RESULTS_DIR}"
            bat "dotnet test ${TEST_PROJECT_DIR} -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ${TEST_RESULTS_DIR} || exit /b 0"
            bat """
              dotnet tool install -g trx2junit || powershell -Command "Write-Host 'trx2junit may already be installed'"
              set PATH=%PATH%;${DOTNET_TOOLS_WIN}
              for %%F in (${TEST_RESULTS_DIR}\\*.trx) do (
                if exist "%%F" (
                  ${DOTNET_TOOLS_WIN}\\trx2junit.exe "%%F" -o "${TEST_RESULTS_DIR}\\%%~nF.xml" || echo convert failed for %%F
                )
              )
            """
          }
        }
      }
      post {
        always {
          echo "Publishing backend test results & archiving test artifacts..."
          // publish JUnit XMLs (converted) and archive anything in TestResults
          junit allowEmptyResults: true, testResults: "${TEST_RESULTS_DIR}/*.xml"
          archiveArtifacts artifacts: "${TEST_RESULTS_DIR}/**/*", allowEmptyArchive: true, fingerprint: true
        }
      }
    }

    stage('Build Frontend (Angular)') {
      steps {
        echo "Installing frontend dependencies and building frontend..."
        dir("${FRONTEND_DIR}") {
          script {
            if (isUnix()) {
              sh 'npm ci'
              // create dist output with production build if script exists
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
        echo "Running frontend tests (Angular)..."
        dir("${FRONTEND_DIR}") {
          script {
            // ensure frontend emits test results to students-ui/test-results/*.xml
            // Note: your Angular project must be configured to output JUnit XML (e.g. karma-junit-reporter)
            if (isUnix()) {
              sh 'mkdir -p test-results || true'
              // run tests in CI mode (no watch). If your package.json has a special CI test script use that.
              // The trailing args (--watch=false) may or may not be used depending on your test runner.
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
          echo "Publishing frontend test results (if any) and archiving..."
          // try to find JUnit xmls in students-ui/test-results
          junit allowEmptyResults: true, testResults: "${FRONTEND_DIR}/test-results/*.xml"
          archiveArtifacts artifacts: "${FRONTEND_DIR}/test-results/**/*.*", allowEmptyArchive: true
        }
      }
    }

    stage('Build Docker Images (Optional)') {
      steps {
        echo "Optional: building docker images via docker compose (if present)..."
        script {
          if (fileExists('docker-compose.yml')) {
            if (isUnix()) {
              sh 'docker compose build || true'
            } else {
              bat 'docker compose build || exit /b 0'
            }
          } else {
            echo "No docker-compose.yml found — skipping docker build"
          }
        }
      }
    }

    stage('Summary') {
      steps {
        echo "Pipeline finished. Check 'Tests' and 'Artifacts' tabs for results."
      }
    }
  }

  post {
    success {
      echo "Pipeline succeeded ✅"
    }
    unstable {
      echo "Pipeline unstable (some tests may have failed) ⚠️"
    }
    failure {
      echo "Pipeline failed ❌"
    }
    always {
      echo "Done. Build status: ${currentBuild.currentResult}"
    }
  }
}
