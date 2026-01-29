$ErrorActionPreference = "Stop"

# Configuration
$pluginName = "AudioSwitcher"
$projectPath = Join-Path "src" "AudioSwitcherPlugin.csproj"
$releaseDir = Join-Path "bin" "Release"
$stagingDir = "dist"
$outputPackage = "${pluginName}.lplug4"

# 0. Prerequisites
Write-Host "`n=== Checking Prerequisites ===" -ForegroundColor Cyan
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) { Write-Error "dotnet SDK not found."; exit 1 }
if (-not (Get-Command "logiplugintool" -ErrorAction SilentlyContinue)) { Write-Error "logiplugintool not found."; exit 1 }

# 1. Clean
Write-Host "`n=== Cleaning ===" -ForegroundColor Cyan
$dirsToClean = @("bin", "obj", $stagingDir, "src/bin", "src/obj")
foreach ($dir in $dirsToClean) {
    if (Test-Path $dir) { Remove-Item $dir -Recurse -Force }
}
if (Test-Path $outputPackage) { Remove-Item $outputPackage -Force }

# 2. Build
Write-Host "`n=== Building Project ===" -ForegroundColor Cyan
dotnet build $projectPath -c Release
if ($LASTEXITCODE -ne 0) { Write-Error "Build failed."; exit 1 }

# 3. Stage Files
Write-Host "`n=== Staging Files ===" -ForegroundColor Cyan
$metadataSrc = Join-Path $releaseDir "metadata"
$binSrc = Join-Path $releaseDir "win"
$metadataDest = Join-Path $stagingDir "metadata"
$binDest = Join-Path $stagingDir "win"

if (-not (Test-Path $metadataSrc)) { Write-Error "Metadata not found."; exit 1 }
if (-not (Test-Path $binSrc)) { Write-Error "Binaries not found."; exit 1 }

New-Item -ItemType Directory -Path $metadataDest -Force | Out-Null
New-Item -ItemType Directory -Path $binDest -Force | Out-Null

Copy-Item "$metadataSrc\*" $metadataDest -Recurse
Get-ChildItem $binSrc | Where-Object { 
    $_.Name -ne "PluginApi.dll" -and 
    $_.Name -ne "PluginApi.xml" -and
    $_.Name -ne "LoupedeckPackage.yaml" 
} | Copy-Item -Destination $binDest

# 4. Pack
Write-Host "`n=== Packaging ===" -ForegroundColor Cyan
logiplugintool pack $stagingDir $outputPackage
if ($LASTEXITCODE -ne 0) { Write-Error "Packaging failed."; exit 1 }

# 5. Verify
Write-Host "`n=== Verifying Package ===" -ForegroundColor Cyan
logiplugintool verify $outputPackage
if ($LASTEXITCODE -ne 0) { Write-Error "Verification failed."; exit 1 }

Write-Host "`nSUCCESS: Package created at $outputPackage" -ForegroundColor Green
