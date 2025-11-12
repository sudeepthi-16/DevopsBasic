pipeline {
  agent any

  options { timestamps() }

  environment {
    BACKEND_DIR      = 'DevopsBasic'           // folder that contains DevopsBasic.sln
    TEST_PROJECT_DIR = 'DevopsBasic.Tests'     // folder that contains DevopsBasic.Tests.csproj (sibling of BACKEND_DIR)
    FRONTEND_DIR     = 'students-ui'
    TEST_RESULTS_DIR = 'TestResults'
    DOTNET_TOOLS_UNIX = "${env.HOME}/.dotnet/tools"
    DOTNET_TOOLS_WIN  = "%USERPROFILE%\\.dotnet\\tools"
    CONFIGURATION = "Release"
  }

  stages {

    stage('Checkout') {
      steps {
        checkout scm
        echo "Workspace: ${env.WORKSPACE}"
        // show workspace layout to help debug path mismatches
        script {
          if (isUnix()) {
            sh 'pwd; ls -la'
            sh 'ls -la ${BACKEND_DIR} || true'
            sh 'ls -la ${TEST_PROJECT_DIR} || true'
          } else {
            bat 'chdir & dir /a'
            bat "if exist ${BACKEND_DIR} (echo Found ${BACKEND_DIR} & dir ${BACKEND_DIR}) else echo ${BACKEND_DIR} not found"
            bat "if exist ${TEST_PROJECT_DIR} (echo Found ${TEST_PROJECT_DIR} & dir ${TEST_PROJECT_DIR}) else echo ${TEST_PROJECT_DIR} not found"
          }
        }
      }
    }

    stage('Restore & Build (.NET)') {
      steps {
        echo "Restoring & building solution from ${BACKEND_DIR}..."
        dir("${BACKEND_DIR}") {
          script {
            if (isUnix()) {
              sh "dotnet restore DevopsBasic.sln"
              sh "dotnet build DevopsBasic.sln -c ${CONFIGURATION} --no-restore"
            } else {
              // Windows agent
              bat "dotnet restore DevopsBasic.sln"
              bat "dotnet build DevopsBasic.sln -c ${CONFIGURATION} --no-restore"
            }
          }
        }
      }
    }

    stage('Run Backend Tests (.NET)') {
      steps {
        echo "Running backend tests (targeting actual test project file)..."
        script {
          // ensure a central TestResults folder at repo root
          if (isUnix()) {
            sh "mkdir -p ${TEST_RESULTS_DIR}"
            // run test by pointing directly at the test .csproj (the correct relative path from BACKEND_DIR)
            dir("${BACKEND_DIR}") {
              sh "dotnet test ../${TEST_PROJECT_DIR}/DevopsBasic.Tests.csproj -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ../${TEST_RESULTS_DIR} || true"
            }
            sh '''
              dotnet tool install -g trx2junit || true
              export PATH="$PATH:${DOTNET_TOOLS_UNIX}"
              for f in ${TEST_RESULTS_DIR}/*.trx; do
                [ -f "$f" ] && trx2junit "$f" -o "${f%.trx}.xml" || true
              done
            '''.stripIndent()
          } else {
            bat "if not exist ${TEST_RESULTS_DIR} mkdir ${TEST_RESULTS_DIR}"
            dir("${BACKEND_DIR}") {
              // From inside DevopsBasic, the test csproj is at ../DevopsBasic.Tests/DevopsBasic.Tests.csproj
              bat "dotnet test ..\\${TEST_PROJECT_DIR}\\DevopsBasic.Tests.csproj -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ..\\${TEST_RESULTS_DIR} || exit /b 0"
            }
            // convert TRX -> JUnit using trx2junit (install if needed)
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
          echo "Publishing backend test results and archiving TestResults/ ..."
          junit allowEmptyResults: true, testResults: "${TEST_RESULTS_DIR}/*.xml"
          archiveArtifacts artifacts: "${TEST_RESULTS_DIR}/**/*", allowEmptyArchive: true, fingerprint: true
        }
      }
    }

    stage('Build Frontend (Angular)') {
      steps {
        echo "Installing & building frontend in ${FRONTEND_DIR}..."
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
        echo "Running frontend tests (inside ${FRONTEND_DIR})..."
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
        script {
          if (fileExists('docker-compose.yml')) {
            echo "Running docker compose build..."
            if (isUnix()) { sh 'docker compose build || true' } else { bat 'docker compose build || exit /b 0' }
          } else {
            echo "No docker-compose.yml — skipping"
          }
        }
      }
    }

    stage('Summary') {
      steps { echo "Pipeline done — check Tests & Artifacts in Jenkins." }
    }
  }

  post {
    success { echo "SUCCESS ✅" }
    unstable { echo "UNSTABLE ⚠️" }
    failure { echo "FAILED ❌" }
    always { echo "Done. Status: ${currentBuild.currentResult}" }
  }
}
