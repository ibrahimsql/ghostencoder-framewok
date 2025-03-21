# Docker Guide for GhostsEncoder by ibrahimsql

This guide explains how to use GhostsEncoder with Docker for increased portability and isolation.

## Prerequisites

- Docker installed on your system
- Basic familiarity with command line and Docker concepts

## Getting Started

### Building the Docker Image

1. Open a terminal/command prompt in the directory containing the Dockerfile
2. Build the Docker image with:

```bash
docker build -t ghostsencoder-ibrahimsql .
```

This will create a Docker image named "ghostsencoder-ibrahimsql" with all the necessary dependencies.

## Usage Options

### Command-Line Mode

To run the tool in command-line mode within a Docker container:

```bash
docker run ghostsencoder-ibrahimsql encrypt aes-cbc -p "YourPassword" -i "/data/input.exe" -o "/data/output.encrypted"
```

### Interactive Mode

For an interactive shell:

```bash
docker run -it ghostsencoder-ibrahimsql
```

This will open a command prompt within the container where you can run encryption commands.

### Mounting Local Files

To access files from your local system:

```bash
docker run -v /path/on/host:/data ghostsencoder-ibrahimsql encrypt aes-cbc -p "YourPassword" -i "/data/input.exe" -o "/data/output.encrypted"
```

Replace `/path/on/host` with the actual path on your system containing the files you want to encrypt.

## Advanced Docker Usage

### Creating a Docker Compose Configuration

Create a file named `docker-compose.yml` with the following content:

```yaml
version: '3'
services:
  encryption-tool:
    build: .
    volumes:
      - ./data:/data
    command: encrypt aes-cbc -p "YourPassword" -i "/data/input.exe" -o "/data/output.encrypted"
```

Then run:

```bash
docker-compose up
```

### Creating a Persistent Container

If you need to use the tool repeatedly:

```bash
docker create --name ghostsencoder-ibrahimsql -v /path/on/host:/data ghostsencoder-ibrahimsql
```

Then start it when needed:

```bash
docker start -i ghostsencoder-ibrahimsql
```

## Troubleshooting

### Error: No such file or directory

Make sure you've correctly mounted the volumes and are using the right paths within the container.

### Permission Denied

Files created by the Docker container might have different permissions. To fix:

```bash
chmod +rw /path/on/host/output.encrypted
```

### Docker Image Size Too Large

Use the multi-stage build approach in the Dockerfile to keep the image size minimal.

## Security Considerations

- The Docker container isolates GhostsEncoder from your system
- Passwords passed via command line might be visible in process lists or history
- Consider using a secure environment variable for the password instead

---

For more information on Docker, visit: https://docs.docker.com/

---

**Copyright © 2025 ibrahimsql. All Rights Reserved. Unauthorized reproduction or distribution of this software is strictly prohibited.** 