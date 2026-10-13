# =============================================================================
#  EcoTrack API — Cidades ESG Inteligentes
#  Dockerfile multi-stage: (1) restore/build  (2) test (opcional)  (3) publish  (4) runtime
# =============================================================================

# ---------- Stage 1: build ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas .sln e .csproj primeiro -> o "dotnet restore" fica em cache
# e só é refeito quando alguma dependência muda (builds mais rápidos no CI).
COPY ["EcoTrackNet.sln", "./"]
COPY ["src/EcoTrack.API/EcoTrack.API.csproj", "src/EcoTrack.API/"]
COPY ["src/EcoTrack.Application/EcoTrack.Application.csproj", "src/EcoTrack.Application/"]
COPY ["src/EcoTrack.Domain/EcoTrack.Domain.csproj", "src/EcoTrack.Domain/"]
COPY ["src/EcoTrack.Infrastructure/EcoTrack.Infrastructure.csproj", "src/EcoTrack.Infrastructure/"]
COPY ["tests/EcoTrack.Tests/EcoTrack.Tests.csproj", "tests/EcoTrack.Tests/"]
RUN dotnet restore "EcoTrackNet.sln"

# Copia o restante do código-fonte e compila em Release
COPY . .
RUN dotnet build "EcoTrackNet.sln" -c Release --no-restore

# ---------- Stage 2: test (usado com --target test) ---------------------------
# Permite rodar os testes dentro do container: docker build --target test .
FROM build AS test
RUN dotnet test "EcoTrackNet.sln" -c Release --no-build --verbosity normal

# ---------- Stage 3: publish -------------------------------------------------
FROM build AS publish
RUN dotnet publish "src/EcoTrack.API/EcoTrack.API.csproj" -c Release -o /app/publish \
    --no-restore /p:UseAppHost=false

# ---------- Stage 4: runtime (imagem final, enxuta) ---------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Versão da aplicação (o pipeline passa o SHA do commit)
ARG APP_VERSION=local

LABEL org.opencontainers.image.title="EcoTrack API" \
      org.opencontainers.image.description="EcoTrack .NET API - Cidades ESG Inteligentes (Gestão de Resíduos e Reciclagem)" \
      org.opencontainers.image.version="${APP_VERSION}" \
      org.opencontainers.image.authors="Ana Luiza Arcanjo - RM562568"

WORKDIR /app

# curl é usado pelo HEALTHCHECK
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Usuário sem privilégios (não roda como root)
RUN addgroup --system ecotrack && adduser --system --ingroup ecotrack ecotrack

COPY --from=publish --chown=ecotrack:ecotrack /app/publish .

USER ecotrack

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    AutoMigrate=true \
    APP_VERSION=${APP_VERSION}

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EcoTrack.API.dll"]
