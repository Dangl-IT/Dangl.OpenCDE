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
        KeyVaultTenantId = credentials('AzureKeyVaultTenantId')
        IGNORE_NORMALISATION_GIT_HEAD_MOVE = 1
    }
    stages {
        stage ('Test') {
			agent {
				node {
					label 'linux'
				}
			}
            steps {
                sh 'bash build.sh Coverage -Configuration Debug'
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
        stage ('Deployment') {
            parallel {
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
		        stage ('Publish Electron App Windows') {
		        	steps {
                        powershell './build.ps1 PublishElectronApp -BuildElectronWindowsTargets'
		        	}
		        }
		        stage ('Publish Electron App Linux & Mac') {
		        	agent {
		        		node {
		        			label 'linux'
		        		}
		        	}
		        	steps {
                        sh 'if [ -d "src/client/Dangl.OpenCDE.Client/bin" ]; then rm -Rf src/client/Dangl.OpenCDE.Client/bin; fi'
		        		sh 'bash build.sh PublishElectronApp -BuildElectronUnixTargets'
		        	}
                    post {
                        always {
                            cleanWs()
                        }
                    }
		        }
            }
        }
    }
    post {
        always {
            cleanWs()
        }
    }
}
