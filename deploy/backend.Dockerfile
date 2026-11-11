# ==============================================================================
# TanErp Backend Production Dockerfile (.NET 10 Web API)
# Multi-stage build: SDK build/publish -> ASP.NET Core 10 Runtime
# ==============================================================================

# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project definitions for layer caching
COPY backend/Directory.Build.props backend/Directory.Packages.props backend/global.json ./backend/
COPY backend/TanErp.slnx ./backend/
COPY backend/src/TanErp.Domain/TanErp.Domain.csproj ./backend/src/TanErp.Domain/
COPY backend/src/TanErp.Application/TanErp.Application.csproj ./backend/src/TanErp.Application/
COPY backend/src/TanErp.Infrastructure/TanErp.Infrastructure.csproj ./backend/src/TanErp.Infrastructure/
COPY backend/src/TanErp.Api/TanErp.Api.csproj ./backend/src/TanErp.Api/

# Restore NuGet dependencies
RUN dotnet restore ./backend/src/TanErp.Api/TanErp.Api.csproj

# Copy application source code
COPY backend/src/ ./backend/src/

# Publish Release build
WORKDIR /src/backend/src/TanErp.Api
RUN dotnet publish TanErp.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install native dependencies required by QuestPDF (libfontconfig1) and health probes (curl)
RUN apt-get update && \
    apt-get install -y --no-install-recommends libfontconfig1 curl && \
    rm -rf /var/lib/apt/lists/*

# Production environment defaults
ENV ASPNETCORE_HTTP_PORTS=5179 \
    ASPNETCORE_ENVIRONMENT=Production \
    Storage__BasePath=/var/lib/tan-erp/storage \
    DOTNET_RUNNING_IN_CONTAINER=true

# Create persistent storage folder and grant permissions to default non-root user (app: uid 1654)
RUN mkdir -p /var/lib/tan-erp/storage && \
    chown -R app:app /var/lib/tan-erp/storage /app

# Copy published application binaries
COPY --from=build --chown=app:app /app/publish .

# Switch to non-root user
USER app

EXPOSE 5179

ENTRYPOINT ["dotnet", "TanErp.Api.dll"]
