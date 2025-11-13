// // pipeline {
// //   agent any

// //   options { timestamps() }

// //   environment {
// //     BACKEND_SLN      = 'DevopsBasic/DevopsBasic.sln'            // path to solution from repo root
// //     TEST_CSPROJ      = 'DevopsBasic.Tests/DevopsBasic.Tests.csproj' // test project path from repo root
// //     FRONTEND_DIR     = 'students-ui'
// //     TEST_RESULTS_DIR = 'TestResults'
// //     DOTNET_TOOLS_UNIX = "${env.HOME}/.dotnet/tools"
// //     DOTNET_TOOLS_WIN  = "%USERPROFILE%\\.dotnet\\tools"
// //     CONFIGURATION = "Release"
// //   }

// //   stages {
// //     stage('Checkout') {
// //       steps {
// //         checkout scm
// //         echo "Workspace: ${env.WORKSPACE}"
// //         script {
// //           if (isUnix()) {
// //             sh 'pwd; ls -la'
// //             sh "ls -la | true"
// //           } else {
// //             bat 'chdir & dir /a'
// //             bat "dir ${WORKSPACE}\\DevopsBasic || echo no DevopsBasic"
// //             bat "dir ${WORKSPACE}\\DevopsBasic.Tests || echo no DevopsBasic.Tests"
// //           }
// //         }
// //       }
// //     }

// //     stage('Restore & Build (.NET)') {
// //       steps {
// //         echo "dotnet restore/build using solution: ${env.BACKEND_SLN}"
// //         script {
// //           if (isUnix()) {
// //             // run restore & build from workspace root but point to the solution file path
// //             sh "dotnet restore \"${BACKEND_SLN}\""
// //             sh "dotnet build \"${BACKEND_SLN}\" -c ${CONFIGURATION} --no-restore"
// //           } else {
// //             bat "dotnet restore \"${BACKEND_SLN}\""
// //             bat "dotnet build \"${BACKEND_SLN}\" -c ${CONFIGURATION} --no-restore"
// //           }
// //         }
// //       }
// //     }

// //     stage('Run Backend Tests (.NET)') {
// //       steps {
// //         echo "Running tests from ${TEST_CSPROJ} and writing TRX to ${TEST_RESULTS_DIR}"
// //         script {
// //           if (isUnix()) {
// //             sh "mkdir -p ${TEST_RESULTS_DIR}"
// //             // run test directly against csproj (from workspace root)
// //             sh "dotnet test \"${TEST_CSPROJ}\" -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ${TEST_RESULTS_DIR} || true"

// //             sh '''
// //               dotnet tool install -g trx2junit || true
// //               export PATH="$PATH:${DOTNET_TOOLS_UNIX}"
// //               for f in ${TEST_RESULTS_DIR}/*.trx; do
// //                 [ -f "$f" ] && trx2junit "$f" -o "${f%.trx}.xml" || true
// //               done
// //             '''.stripIndent()
// //           } else {
// //             bat "if not exist ${TEST_RESULTS_DIR} mkdir ${TEST_RESULTS_DIR}"
// //             bat "dotnet test \"${TEST_CSPROJ}\" -c ${CONFIGURATION} --no-build --logger \"trx;LogFileName=backend.trx\" --results-directory ${TEST_RESULTS_DIR} || exit /b 0"

// //             bat """
// //               dotnet tool install -g trx2junit || powershell -Command "Write-Host 'trx2junit may already be installed'"
// //               set PATH=%PATH%;${DOTNET_TOOLS_WIN}
// //               for %%F in (${TEST_RESULTS_DIR}\\*.trx) do (
// //                 if exist "%%F" (
// //                   ${DOTNET_TOOLS_WIN}\\trx2junit.exe "%%F" -o "${TEST_RESULTS_DIR}\\%%~nF.xml" || echo convert failed for %%F
// //                 )
// //               )
// //             """
// //           }
// //         }
// //       }
// //       post {
// //         always {
// //           junit allowEmptyResults: true, testResults: "${TEST_RESULTS_DIR}/*.xml"
// //           archiveArtifacts artifacts: "${TEST_RESULTS_DIR}/**/*", allowEmptyArchive: true, fingerprint: true
// //         }
// //       }
// //     }

// //     stage('Build Frontend (Angular)') {
// //       steps {
// //         echo "Building frontend in ${FRONTEND_DIR}"
// //         dir("${FRONTEND_DIR}") {
// //           script {
// //             if (isUnix()) {
// //               sh 'npm ci'
// //               sh 'npm run build --if-present'
// //             } else {
// //               bat 'npm ci'
// //               bat 'npm run build --if-present'
// //             }
// //           }
// //         }
// //       }
// //     }

// //     stage('Run Frontend Tests (Angular)') {
// //       steps {
// //         echo "Running frontend tests in ${FRONTEND_DIR}"
// //         dir("${FRONTEND_DIR}") {
// //           script {
// //             if (isUnix()) {
// //               sh 'mkdir -p test-results || true'
// //               sh 'npm test -- --watch=false || true'
// //             } else {
// //               bat 'if not exist test-results mkdir test-results'
// //               bat 'npm test -- --watch=false || exit /b 0'
// //             }
// //           }
// //         }
// //       }
// //       post {
// //         always {
// //           junit allowEmptyResults: true, testResults: "${FRONTEND_DIR}/test-results/*.xml"
// //           archiveArtifacts artifacts: "${FRONTEND_DIR}/test-results/**/*.*", allowEmptyArchive: true
// //         }
// //       }
// //     }

// //     stage('Optional: Docker Compose') {
// //       steps {
// //         script {
// //           if (fileExists('docker-compose.yml')) {
// //             if (isUnix()) { sh 'docker compose build || true' } else { bat 'docker compose build || exit /b 0' }
// //           } else {
// //             echo "No docker-compose.yml found; skipping"
// //           }
// //         }
// //       }
// //     }

// //     stage('Summary') {
// //       steps { echo "Done — check Tests & Artifacts in Jenkins UI." }
// //     }
// //   }

// //   post {
// //     success { echo "SUCCESS ✅" }
// //     unstable { echo "UNSTABLE ⚠️" }
// //     failure { echo "FAILED ❌" }
// //     always { echo "Status: ${currentBuild.currentResult}" }
// //   }
// // }


// pipeline {
//     agent any  // means Jenkins can run this on any available agent (your local one)

//     options {
//         timestamps()  // show timestamps in logs
//     }

//     environment {
//         // === Folder setup ===
//         BACKEND_DIR    = 'DevopsBasic'
//         FRONTEND_DIR   = 'students-ui'

//         // === Tool paths ===
//         DOTNET_TOOLS   = "${env.USERPROFILE}\\.dotnet\\tools"  // where trx2junit lives
//     }

//     stages {

//         // 1️⃣ --- Checkout code from GitHub ---
//         stage('Checkout Code') {
//             steps {
//                 echo "Pulling latest code from GitHub..."
//                 git branch: 'main', url: 'https://github.com/sudeepthi-16/DevopsBasic.git'
//             }
//         }

//         // 2️⃣ --- Build the .NET Backend ---
//         stage('Build Backend (.NET)') {
//             steps {
//                 dir("${BACKEND_DIR}") {
//                     echo "Building backend project..."
//                     bat 'dotnet restore'
//                     bat 'dotnet build --configuration Release --no-restore'
//                 }
//             }
//         }

//         // 3️⃣ --- Run Backend Tests (.NET) ---
//         stage('Test Backend (.NET)') {
//             steps {
//                 dir("${BACKEND_DIR}") {
//                     echo "Running .NET tests..."
//                     bat 'if not exist TestResults mkdir TestResults'
//                     bat 'dotnet test --configuration Release --no-build --logger "trx;LogFileName=testresults.trx" || exit /b 0'

//                     // Convert TRX → JUnit XML
//                     withEnv(["PATH=${DOTNET_TOOLS};${env.PATH}"]) {
//                         bat 'for /R %i in (*.trx) do trx2junit "%i"'
//                     }
//                 }
//             }
//             post {
//                 always {
//                     // Publish test results to Jenkins
//                     junit allowEmptyResults: true, testResults: "${BACKEND_DIR}/**/testresults*.xml"
//                     archiveArtifacts artifacts: "${BACKEND_DIR}/**/TestResults/*.*", allowEmptyArchive: true
//                 }
//             }
//         }

//         // 4️⃣ --- Install & Build Frontend (Angular) ---
//         stage('Build Frontend (Angular)') {
//             steps {
//                 dir("${FRONTEND_DIR}") {
//                     echo "Installing dependencies and building frontend..."
//                     bat 'npm ci'
//                     bat 'npm run build --if-present'
//                 }
//             }
//         }

//         // 5️⃣ --- Run Frontend Tests (Angular) ---
//         stage('Test Frontend (Angular)') {
//             steps {
//                 dir("${FRONTEND_DIR}") {
//                     echo "Running Angular tests..."
//                     bat 'npm test || exit /b 0'
//                 }
//             }
//             post {
//                 always {
//                     junit allowEmptyResults: true, testResults: "${FRONTEND_DIR}/test-results/*.xml"
//                     archiveArtifacts artifacts: "${FRONTEND_DIR}/test-results/*.xml", allowEmptyArchive: true
//                 }
//             }
//         }

//         // 6️⃣ --- Optional: Build Docker Images ---
//         stage('Build Docker Images (Optional)') {
//             steps {
//                 echo "Building Docker images (optional step)..."
//                 bat 'docker compose build'
//             }
//         }

//         // 7️⃣ --- Wrap Up ---
//         stage('Summary') {
//             steps {
//                 echo "✅ All stages completed. Check the 'Test Result' tab for test summaries."
//             }
//         }
//     }

//     post {
//         success {
//             echo "🎉 Pipeline finished successfully!"
//         }
//         failure {
//             echo "❌ Pipeline failed. Check the console output and test reports for details."
//         }
//     }
// }




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
                script {
                    sh """
                        docker build -t ${BACKEND_IMAGE}:latest ./DevopsBasic
                    """
                }
            }
        }

        stage('Build Frontend Docker Image') {
            steps {
                script {
                    sh """
                        docker build -t ${FRONTEND_IMAGE}:latest ./students-ui
                    """
                }
            }
        }

        stage('Deploy using Docker Compose') {
            steps {
                script {
                    sh """
                        docker-compose down --remove-orphans
                        docker-compose build
                        docker-compose up -d
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Deployment SUCCESSFUL!"
            echo "Frontend running at: http://localhost:8080"
            echo "Backend running at: http://localhost:5000"
        }
        failure {
            echo "Deployment FAILED. Check console logs."
        }
    }
}
