#!/bin/bash

echo "==============================================="
echo "  GhostsEncoder by ibrahimsql - Linux Verification Tool"
echo "==============================================="
echo

echo "Checking Mono/.NET Core installation..."
if command -v mono >/dev/null 2>&1; then
    echo "[OK] Mono runtime found"
    mono --version | head -n 1
elif command -v dotnet >/dev/null 2>&1; then
    echo "[OK] .NET Core found"
    dotnet --version
else
    echo "[ERROR] Neither Mono nor .NET Core found!"
    echo "Please install Mono or .NET Core to run the application."
    echo "  - Install Mono: sudo apt-get install mono-complete"
    echo "  - Or install .NET: https://dotnet.microsoft.com/download"
fi

echo
echo "Checking for native components..."
if [ -f "libNativeCrypto.so" ]; then
    echo "[OK] Native library found"
else
    echo "[WARNING] Native library (libNativeCrypto.so) not found!"
    echo "Native encryption features will use fallback implementations."
    echo
    echo "To compile the native library, run:"
    echo "  make"
fi

echo
echo "Checking for Docker..."
if command -v docker >/dev/null 2>&1; then
    echo "[OK] Docker found"
    docker --version
else
    echo "[WARNING] Docker not found! Docker features will not be available."
    echo "To install Docker, follow instructions at: https://docs.docker.com/engine/install/"
fi

echo
echo "Checking for GCC (needed for native components)..."
if command -v gcc >/dev/null 2>&1; then
    echo "[OK] GCC found"
    gcc --version | head -n 1
else
    echo "[WARNING] GCC not found! You won't be able to compile native components."
    echo "To install GCC: sudo apt-get install build-essential"
fi

echo
echo "Checking system resources..."
echo "Memory:"
free -h | grep "Mem:" | awk '{print "Total: " $2 ", Available: " $7}'
echo "Recommended: At least 4GB RAM"

echo
echo "Checking file permissions..."
if [ -f "GhostsEncoder.exe" ]; then
    if [ -x "GhostsEncoder.exe" ]; then
        echo "[OK] Executable has correct permissions"
    else
        echo "[WARNING] Executable permissions need to be fixed!"
        echo "Run: chmod +x GhostsEncoder.exe"
    fi
else
    echo "[ERROR] GhostsEncoder.exe not found!"
    echo "Make sure you're running this script from the correct directory."
fi

if [ -f "libNativeCrypto.so" ]; then
    if [ -x "libNativeCrypto.so" ]; then
        echo "[OK] Native library has correct permissions"
    else
        echo "[WARNING] Native library permissions need to be fixed!"
        echo "Run: chmod +x libNativeCrypto.so"
    fi
fi

echo
echo "==============================================="
echo "Verification complete! See above for any issues."
echo "==============================================="
echo "Tool created by ibrahimsql - https://github.com/ibrahimsql"
echo "===============================================" 