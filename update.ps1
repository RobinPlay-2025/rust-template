param (
    [Parameter(Mandatory=$false)]
    [ValidateSet("Oxide", "Carbon", "Ask")]
    [string]$Framework = "Ask"
)

if ($Framework -eq "Ask") {
    Write-Host "==========================================" -ForegroundColor Cyan
    Write-Host "   SELECT FRAMEWORK TO UPDATE" -ForegroundColor Cyan
    Write-Host "==========================================" -ForegroundColor Cyan
    Write-Host "1. Oxide (Default)" -ForegroundColor White
    Write-Host "2. Carbon" -ForegroundColor White
    Write-Host ""
    $choice = Read-Host "Choose option [1-2]"
    if ($choice -eq "2") { $Framework = "Carbon" }
    else { $Framework = "Oxide" }

    # --- Automatic VS Code Setting Update ---
    $SettingsPath = Join-Path $PSScriptRoot ".vscode/settings.json"
    if (Test-Path $SettingsPath) {
        try {
            $json = Get-Content $SettingsPath -Raw | ConvertFrom-Json
            $json.'dotnet.defaultSolutionConfiguration' = $Framework
            $json | ConvertTo-Json -Depth 100 | Set-Content $SettingsPath
            Write-Host "--- VS Code configuration set to: $Framework ---" -ForegroundColor Green
        } catch {
            Write-Warning "Could not update .vscode/settings.json: $_"
        }
    }
}

# RUST-TEMPLATE SYSTEM UPDATE SCRIPT
# Using pure ASCII to avoid encoding issues

function Write-Header($text) {
    Write-Host "`n>>> $text <<<`n" -ForegroundColor Yellow
}

Write-Header "STARTING SYSTEM UPDATE (Framework: $Framework)"

# 1. Fetching updates from GitHub
Write-Host "--- Step 1: Downloading objects from GitHub ---" -ForegroundColor Cyan
git fetch --progress origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "!!! ERROR: Could not connect to GitHub !!!" -ForegroundColor Red
    exit 1
}

# 2. System paths list
$SystemPaths = @(
    "Managed",
    ".github",
    ".rust-analyzer",
    "rust-template.sln",
    "rust.template.csproj",
    "update.bat",
    "update.ps1"
)

Write-Host "--- Step 2: Overwriting system files (With Progress %) ---" -ForegroundColor Magenta

# Gather all files first to calculate total count
$allFiles = @()
foreach ($item in $SystemPaths) {
    $found = git ls-tree -r --name-only FETCH_HEAD $item
    if ($null -eq $found) {
        $allFiles += $item
    } else {
        $allFiles += $found
    }
}

$totalFiles = $allFiles.Count
$current = 0

foreach ($file in $allFiles) {
    if (-not [string]::IsNullOrWhiteSpace($file)) {
        $current++
        $percent = [Math]::Floor(($current / $totalFiles) * 100)
        
        # Display progress: [XX%] Overwriting: path ...
        Write-Host "[$percent%] Overwriting: $file ..." -ForegroundColor Gray
        
        git checkout FETCH_HEAD -- $file 2>$null
    }
}

Write-Header "DONE! UPDATE COMPLETE (100%)"
Write-Host "Total $totalFiles files processed. Your plugins folder is safe." -ForegroundColor Green
