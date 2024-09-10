pipeline {
    agent any
    environment {
        DOTNET_CLI_HOME = '/tmp' // Directory to avoid permission issues
    }
    stages {
        stage('Checkout') {
            steps {
                git branch: 'master', url: 'https://github.com/Arijhajjaji22/projetPFE.git'
            }
        }
        stage('Build') {
            steps {
                sh '/snap/bin/dotnet build App_plateforme_de_recurtement.sln --configuration Release'
            }
        }
        stage('Test') {
            steps {
                sh '/snap/bin/dotnet test App_plateforme_de_recurtement.sln --configuration Release'
            }
        }
    }
    post {
        success {
            echo 'Pipeline completed successfully.'
        }
        failure {
            echo 'Pipeline failed.'
        }
        always {
            cleanWs() // Clean workspace
            echo 'Pipeline finished.'
        }
    }
}
