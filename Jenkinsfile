pipeline {
    agent any
    environment {
        DOTNET_CLI_HOME = '/tmp'
        SONARQUBE_SERVER = 'http://sonarqube-server:9000'
        SONARQUBE_TOKEN = credentials('sonar-auth-token')
    }
    stages {
        stage('Checkout') {
            steps {
                git branch: 'master', url: 'https://github.com/Arijhajjaji22/projetPFE.git'
            }
        }
        stage('Build') {
            steps {
                sh '/snap/bin/dotnet build App_plateforme_de_recrutement.sln --configuration Release'
            }
        }
        stage('SonarQube Analysis') {
            steps {
                script {
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
                        -Dsonar.login=$SONARQUBE_TOKEN \
                        -Dsonar.cs.vstest.reportsPaths=**/TestResults/*.xml \
                        -Dsonar.coverage.cobertura.reportPaths=**/TestResults/coverage.cobertura.xml || exit 1
                    '''
                }
            }
        }
        stage('Test') {
            steps {
                sh '/snap/bin/dotnet test App_plateforme_de_recrutement.sln --configuration Release --collect:"XPlat Code Coverage"'
            }
        }
        stage('Verify Dockerfile') {
            steps {
                script {
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
                        sh 'echo $DOCKER_PASSWORD | docker login -u $DOCKER_USERNAME --password-stdin'
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
                node {
                    cleanWs()
                }
            }
            echo 'Pipeline finished.'
        }
    }
}
