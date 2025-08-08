# Auto-generated Dockerfile for SentraUnitTests (.NET)
# Generated on 2025-08-08T11:10:55.392Z

# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Set working directory
WORKDIR /app

# Copy all project files first to resolve dependencies
COPY . ./

# Build only the SentraUnitTests project
RUN dotnet build SentraUnitTests/ --configuration Debug

# Optional: Run tests if this is a test project
# RUN dotnet test --configuration Debug --no-build --verbosity normal

# Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/SentraUnitTests/bin/Debug/net9.0/ ./

# Set the entry point
ENTRYPOINT ["dotnet", "SentraUnitTests.dll"]
