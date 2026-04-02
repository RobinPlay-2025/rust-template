# RUST-TEMPLATE SYSTEM UPDATE SCRIPT
# Using pure ASCII to avoid encoding issues

function Write-Header($text) {
    Write-Host "`n>>> $text <<<`n" -ForegroundColor Yellow
}

Write-Header "STARTING SYSTEM UPDATE"

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
    if ($found -eq $null) {
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
