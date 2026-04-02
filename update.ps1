# RUST-TEMPLATE SYSTEM UPDATE SCRIPT
# Using pure ASCII to avoid encoding issues

function Write-Header($text) {
    Write-Host "`n>>> $text <<<`n" -ForegroundColor Yellow
}

Write-Header "STARTING SYSTEM UPDATE"

# 1. Fetching updates from GitHub with VISIBLE PROGRESS
Write-Host "--- Step 1: Downloading updates from GitHub ---" -ForegroundColor Cyan
git fetch --progress origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "!!! ERROR: Could not connect to GitHub !!!" -ForegroundColor Red
    exit 1
}

# 2. System paths list
$SystemPaths = @(
    "Managed/",
    ".github/",
    "rust-template.sln",
    "rust.template.csproj",
    "update.bat",
    "update.ps1"
)

Write-Host "--- Step 2: Extracting and replacing files ---" -ForegroundColor Magenta

foreach ($path in $SystemPaths) {
    # Force update and SHOW which files were changed
    Write-Host "Checking: $path" -ForegroundColor Gray
    
    # We use git checkout and capture the output to show files
    git checkout FETCH_HEAD -- $path 2>&1 | Out-Default
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Done: $path" -ForegroundColor Green
    }
}

Write-Header "DONE! UPDATE COMPLETE"
Write-Host "Your plugins in 'plugins/' folder were NOT touched." -ForegroundColor Green
