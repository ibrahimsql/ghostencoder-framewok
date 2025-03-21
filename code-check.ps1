# PowerShell script to check for potential issues in the codebase
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "    GhostsEncoder by ibrahimsql - Code Check Tool" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host ""

# Check for common security issues
Write-Host "Checking for potential security issues..." -ForegroundColor Yellow

$securityIssues = 0

# Check for hardcoded credentials
$files = Get-ChildItem -Path .\ -Filter *.cs -Recurse
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName
    
    # Check for potential hardcoded passwords or keys
    $regex = "password\s*=\s*[""'][^""']+[""']|key\s*=\s*[""'][^""']+[""']"
    $matches = $content | Select-String -Pattern $regex -AllMatches
    
    if ($matches.Count -gt 0) {
        Write-Host "WARNING: Potential hardcoded credentials in $($file.Name):" -ForegroundColor Red
        foreach ($match in $matches) {
            Write-Host "  - $($match)" -ForegroundColor Red
        }
        $securityIssues++
    }
}

# Check for weak cryptography
$weakCrypto = @("MD5", "SHA1", "DES")
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName
    
    foreach ($algo in $weakCrypto) {
        $matches = $content | Select-String -Pattern $algo -AllMatches
        
        if ($matches.Count -gt 0) {
            Write-Host "WARNING: Potentially weak cryptography ($algo) in $($file.Name)" -ForegroundColor Red
            $securityIssues++
        }
    }
}

# Check for native library issues
if (-not (Test-Path -Path ".\NativeCrypto.dll")) {
    Write-Host "WARNING: Native library (NativeCrypto.dll) not found. Native features will use fallback implementations." -ForegroundColor Yellow
}

# Check for Docker issues
if (-not (Test-Path -Path ".\Dockerfile")) {
    Write-Host "WARNING: Dockerfile not found. Docker support will not work." -ForegroundColor Yellow
}

# Check for main executable
if (-not (Test-Path -Path ".\GhostsEncoder.exe")) {
    Write-Host "ERROR: GhostsEncoder.exe not found!" -ForegroundColor Red
}

# Build verification
Write-Host ""
Write-Host "Checking build configuration..." -ForegroundColor Yellow

if (Test-Path -Path ".\ExposiveToolsEncoder.csproj") {
    $projFile = Get-Content -Path ".\ExposiveToolsEncoder.csproj"
    
    # Check target framework
    $framework = $projFile | Select-String -Pattern "<TargetFrameworkVersion>"
    if ($framework) {
        $version = $framework -replace ".*<TargetFrameworkVersion>v", "" -replace "</TargetFrameworkVersion>.*", ""
        if ([version]$version -lt [version]"4.6.1") {
            Write-Host "WARNING: Target framework ($version) is below recommended v4.6.1" -ForegroundColor Red
        } else {
            Write-Host "Target framework: $version" -ForegroundColor Green
        }
    }
} else {
    Write-Host "WARNING: Project file not found." -ForegroundColor Red
}

# Summary
Write-Host ""
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "                   Summary" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan

if ($securityIssues -gt 0) {
    Write-Host "$securityIssues potential security issues found. Please review warnings above." -ForegroundColor Red
} else {
    Write-Host "No major security issues detected." -ForegroundColor Green
}

Write-Host ""
Write-Host "Recommendations:" -ForegroundColor Yellow
Write-Host "1. Ensure all encryption algorithms are properly implemented with secure parameters" -ForegroundColor Yellow
Write-Host "2. Verify error handling to prevent information leakage" -ForegroundColor Yellow
Write-Host "3. Compile and test the native library on target platforms" -ForegroundColor Yellow
Write-Host "4. Test Docker container functionality" -ForegroundColor Yellow
Write-Host ""
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "Tool created by ibrahimsql - https://github.com/ibrahimsql" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan 