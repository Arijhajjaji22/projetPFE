pipeline {
    agent any
    environment {
        DOTNET_CLI_HOME = '/tmp' // Répertoire pour éviter les problèmes de permissions
    }
    stages {
        stage('Cloner le dépôt Git') {
            steps {
                git branch: 'master', url: 'https://github.com/Arijhajjaji22/projetPFE.git'
            }
        }
        stage('Restaurer les packages NuGet') {
            steps {
                sh '/snap/bin/dotnet restore App_plateforme_de_recurtement.sln'
            }
        }
        stage('Construire la solution .NET') {
            steps {
                sh '/snap/bin/dotnet build App_plateforme_de_recurtement.sln --configuration Release'
            }
        }
        stage('Tester la solution .NET') {
            steps {
                sh '/snap/bin/dotnet test App_plateforme_de_recurtement.sln --configuration Release'
            }
        }
    }
    post {
        success {
            echo 'Le pipeline s\'est terminé avec succès.'
        }
        failure {
            echo 'Le pipeline a échoué.'
        }
        always {
            cleanWs() // Nettoyer les fichiers de travail
            echo 'Pipeline terminé.'
        }
    }
}
