@echo off
echo ===============================================
echo   GhostsEncoder by ibrahimsql - Verification Tool
echo ===============================================
echo.

echo Checking .NET Framework version...
reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Version 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET Framework 4.6.1 or later is required!
    echo Please install the latest .NET Framework from Microsoft's website.
) else (
    echo [OK] .NET Framework found
)

echo.
echo Checking for native components...
if exist NativeCrypto.dll (
    echo [OK] Native library found
) else (
    echo [WARNING] Native library (NativeCrypto.dll) not found!
    echo Native encryption features will use fallback implementations.
    echo.
    echo To compile the native library, run:
    echo   make
)

echo.
echo Checking for Docker...
docker --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Docker not found! Docker features will not be available.
    echo To install Docker, visit: https://www.docker.com/products/docker-desktop
) else (
    echo [OK] Docker found
)

echo.
echo Checking for GCC (needed for native components)...
gcc --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] GCC not found! You won't be able to compile native components.
    echo To install GCC on Windows, consider MinGW or MSYS2.
) else (
    echo [OK] GCC found
)

echo.
echo Checking system resources...
wmic os get TotalVisibleMemorySize /value | find "TotalVisibleMemorySize"
echo.
echo Recommended: At least 4GB RAM (4,000,000 KB)

echo.
echo Checking executable...
if exist GhostsEncoder.exe (
    echo [OK] GhostsEncoder.exe found
) else (
    echo [ERROR] GhostsEncoder.exe not found!
    echo Make sure you're running this script from the correct directory.
)

echo.
echo ===============================================
echo Verification complete! See above for any issues.
echo ===============================================
echo Tool created by ibrahimsql - https://github.com/ibrahimsql
echo ===============================================

pause 