# Estágio base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo de projeto para restaurar dependências
COPY ["Api/minimal-api.csproj", "Api/"]

# Restaura dependências
RUN dotnet restore "Api/minimal-api.csproj"

# Copia todo o código para dentro do container
COPY . .

# Compila o projeto
RUN dotnet build "Api/minimal-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio para publicar
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api/minimal-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio final: runtime com artefatos publicados
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "minimal-api.dll"]
