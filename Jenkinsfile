pipeline {
    options {
        disableConcurrentBuilds()
    }
    agent {
        node {
            label 'master'
            customWorkspace 'workspace/Dangl.OpenCDE'
        }
    }
    environment {
        KeyVaultBaseUrl = credentials('AzureCiKeyVaultBaseUrl')
        KeyVaultClientId = credentials('AzureCiKeyVaultClientId')
        KeyVaultClientSecret = credentials('AzureCiKeyVaultClientSecret')
        IGNORE_NORMALISATION_GIT_HEAD_MOVE = 1
    }
    stages {
        stage ('Test') {
            steps {
                powershell './build.ps1 Coverage -Configuration Debug'
            }
            post {
                always {
                    recordIssues(
                        tools: [
                            msBuild(), 
                            taskScanner(
                                excludePattern: '**/*/node_modules/**/*, src/server/Dangl.OpenCDE/wwwroot/dist/**/*, output/**/*', 
                                highTags: 'HACK, FIXME', 
                                ignoreCase: true, 
                                includePattern: '**/*.cs, **/*.g4, **/*.ts, **/*.js, **/*.html, **/*.scss', 
                                normalTags: 'TODO')
                            ])
                   xunit(
                       testTimeMargin: '3000',
                       thresholdMode: 1,
                       thresholds: [
                           failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'),
                           skipped(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
                       ],
                       tools: [
                           xUnitDotNet(deleteOutputFiles: true, failIfNotNew: true, pattern: '**/*testresults.xml', stopProcessingIfError: true)
                       ])
                }
            }
        }
        stage ('Publish Docs & Assets') {
            steps {
                powershell './build.ps1 UploadDocumentation+PublishGitHubRelease -Configuration Release'
            }
        }
		stage ('Deploy Docker') {
			agent {
				node {
					label 'linux'
				}
			}
			steps {
				sh 'bash build.sh PushDocker'
			}
		}
    }
    post {
        always {
            cleanWs()
        }
    }
}
