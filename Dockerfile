# Étape 1 : Utilisation de l'image officielle de .NET SDK pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copier les fichiers .csproj et restaurer les packages en cache pour des constructions plus rapides
COPY *.sln ./
COPY App_plateforme_de_recurtement.csproj ./
RUN dotnet restore

# Copier tous les fichiers restants du projet
COPY . ./

# Compiler le projet en mode Release
RUN dotnet build -c Release -o /app/build

# Étape 2 : Utilisation de l'image officielle .NET Runtime pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/build ./

# Exposer le port sur lequel l'application écoute (par exemple : 80)
EXPOSE 80

# Démarrer l'application
ENTRYPOINT ["dotnet", "App_plateforme_de_recurtement.dll"]
