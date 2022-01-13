FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./*.sln ./
COPY ./src/Gonis.Assistant.Server/Gonis.Assistant.Server.csproj ./src/Gonis.Assistant.Server/Gonis.Assistant.Server.csproj
COPY ./src/Gonis.Assistant.Core/Gonis.Assistant.Core.csproj ./src/Gonis.Assistant.Core/Gonis.Assistant.Core.csproj
COPY ./src/Gonis.Assistant.Telegram/Gonis.Assistant.Telegram.csproj ./src/Gonis.Assistant.Telegram/Gonis.Assistant.Telegram.csproj
RUN dotnet restore
WORKDIR /src/Gonis.Assistant.Server
COPY . .
# Copy everything else and build
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Gonis.Assistant.Server.dll"]