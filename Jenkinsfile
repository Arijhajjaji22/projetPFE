pipeline {
    agent any
    environment {
        DOTNET_CLI_HOME = '/tmp' // Directory to avoid permission issues
        SONARQUBE_SERVER = 'http://localhost:9000' // URL de votre serveur SonarQube
        SONARQUBE_TOKEN = credentials('sonar-auth-token') // Token SonarQube stocké dans Jenkins credentials
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
        stage('SonarQube Analysis') {
            steps {
                script {
                    // Analyse SonarQube
                    sh '''
                    dotnet tool restore
                    dotnet sonarscanner begin /k:"projetPFE" /d:sonar.host.url=$SONARQUBE_SERVER /d:sonar.login=$SONARQUBE_TOKEN
                    dotnet build App_plateforme_de_recurtement.sln --configuration Release
                    dotnet sonarscanner end /d:sonar.login=$SONARQUBE_TOKEN
                    '''
                }
            }
        }
        stage('Test') {
            steps {
                sh '/snap/bin/dotnet test App_plateforme_de_recurtement.sln --configuration Release'
            }
        }
        stage('Verify Dockerfile') {
            steps {
                script {
                    // Vérifiez la présence du Dockerfile
                    if (fileExists('Dockerfile')) {
                        echo 'Dockerfile is present.'
                    } else {
                        error 'Dockerfile not found!'
                    }
                }
            }
        }
        stage('Build Docker Image') {
            steps {
                sh 'docker build -t arij978/plateformederecrutement:latest .'
            }
        }
        stage('Push Docker Image') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: 'docker-hub-credentials', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                        // Connexion à Docker Hub
                        sh 'echo $DOCKER_PASSWORD | docker login -u $DOCKER_USERNAME --password-stdin'
                        // Pousser l'image Docker
                        sh 'docker push arij978/plateformederecrutement:latest'
                    }
                }
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
