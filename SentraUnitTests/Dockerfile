# Auto-generated Dockerfile for SentraUnitTests (.NET)
# Generated on 2025-08-08T02:37:39.437Z

# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Set working directory
WORKDIR /app

# Copy project file(s) and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire source code
COPY . ./

# Build the project
RUN dotnet build --configuration Debug --no-restore

# Optional: Run tests if this is a test project
# RUN dotnet test --configuration Debug --no-build --verbosity normal

# Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/bin/Debug/net9.0/ ./

# Set the entry point
ENTRYPOINT ["dotnet", "SentraUnitTests.dll"]
