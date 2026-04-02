# RUST-TEMPLATE SYSTEM UPDATE SCRIPT
# Using pure ASCII to avoid encoding issues

function Write-Header($text) {
    Write-Host "`n>>> $text <<<`n" -ForegroundColor Yellow
}

Write-Header "STARTING SYSTEM UPDATE"

# 1. Fetching updates from GitHub
Write-Host "--- Step 1: Connecting to GitHub ---" -ForegroundColor Cyan
git fetch origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "!!! ERROR: Could not connect to GitHub !!!" -ForegroundColor Red
    exit 1
}

# 2. System paths list (update scripts last)
$SystemPaths = @(
    "Managed/",
    ".github/",
    "rust-template.sln",
    "rust.template.csproj",
    "update.bat",
    "update.ps1"
)

Write-Host "--- Step 2: Replacing system files ---" -ForegroundColor Magenta

foreach ($path in $SystemPaths) {
    # Force update from remote FETCH_HEAD
    git checkout FETCH_HEAD -- $path 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Updated: $path" -ForegroundColor Gray
    }
}

Write-Header "DONE! UPDATE COMPLETE"
Write-Host "Your plugins in 'plugins/' folder are safe and sound." -ForegroundColor Green
