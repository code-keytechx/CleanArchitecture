# Auto-generated Dockerfile for CleanArchitecture (.NET)
# Generated on 2025-08-08T12:39:09.991Z

# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Set working directory
WORKDIR /app

# Copy the entire source code
COPY . ./

# Set working directory
WORKDIR /app/SentraUnitTests

RUN dotnet restore

# Build the project
RUN dotnet build --configuration Debug --no-restore

# Optional: Run tests if this is a test project
# RUN dotnet test --configuration Debug --no-build --verbosity normal

# Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app/SentraUnitTests
COPY --from=build-env /app/bin/Debug/net9.0/ ./

# Set the entry point
ENTRYPOINT ["dotnet", "CleanArchitecture.dll"]
