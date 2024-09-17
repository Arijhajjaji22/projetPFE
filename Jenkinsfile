pipeline {
    agent any
    environment {
        DOTNET_CLI_HOME = '/tmp' // Directory to avoid permission issues
        SONARQUBE_SERVER = 'http://sonarqube-server:9000' // Remplacez par l'adresse correcte
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
                    // Exécuter SonarScanner via Docker avec sudo si nécessaire
                    sh '''
                    echo "Starting SonarQube analysis..."
                    sudo docker run --rm \
                        --network host \
                        -e SONAR_HOST_URL=$SONARQUBE_SERVER \
                        -e SONAR_LOGIN=$SONARQUBE_TOKEN \
                        -v $(pwd):/usr/src \
                        -w /usr/src \
                        sonarsource/sonar-scanner-cli:latest \
                        sonar-scanner \
                        -Dsonar.projectKey=projetPFE \
                        -Dsonar.sources=. \
                        -Dsonar.host.url=$SONARQUBE_SERVER \
                        -Dsonar.login=$SONARQUBE_TOKEN || exit 1
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
            script {
                // Nettoyage de l'espace de travail en utilisant le contexte de nœud
                node {
                    cleanWs() // Clean workspace
                }
            }
            echo 'Pipeline finished.'
        }
    }
}
