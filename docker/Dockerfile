FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
COPY packages.config ./
RUN nuget restore

# Copy everything else and build
COPY . ./
RUN msbuild /p:Configuration=Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8
WORKDIR /app
COPY --from=build /app/bin/Release ./

# Entry point
ENTRYPOINT ["GhostsEncoder.exe"]

# Default to help screen
CMD []

# Metadata labels
LABEL maintainer="ibrahimsql"
LABEL version="2.0"
LABEL description="GhostsEncoder - Advanced Malware Encryption & Obfuscation Tool by ibrahimsql" 