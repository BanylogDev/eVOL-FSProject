# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore
COPY ["eVOL.Client/eVOL.Client.csproj", "eVOL.Client/"]
COPY ["../eVOL.Client.Client/eVOL.Client.Client.csproj", "eVOL.Client.Client/"]
RUN dotnet restore "eVOL.Client/eVOL.Client.csproj"

# Copy everything else (from build context)
COPY . .

# Build the hosted app (this triggers the WASM client build too)
WORKDIR "/src/eVOL.Client"
RUN dotnet build "eVOL.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eVOL.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eVOL.Client.dll"]
