# Safe Rust Template Update
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

$ErrorActionPreference = 'SilentlyContinue'

function Write-Step([string]$msg) {
    Write-Host ""
    Write-Host ">>> $msg" -ForegroundColor Cyan
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   RUST-TEMPLATE UPDATE (ENGLISH)         " -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# 0. Git Initialize
if (!(Test-Path ".git")) {
    Write-Step "Initializing Git..."
    git init
}

git config user.name "User" 2>$null >$null
git config user.email "user@example.com" 2>$null >$null

# Set Remote
$remoteUrl = git remote get-url origin 2>$null
if ($LASTEXITCODE -ne 0) {
    git remote add origin https://github.com/RobinPlay-2025/rust-template.git
} else {
    git remote set-url origin https://github.com/RobinPlay-2025/rust-template.git
}

if (Test-Path ".git/MERGE_HEAD") {
    git merge --abort
}

git rev-parse HEAD 2>$null >$null
if ($LASTEXITCODE -ne 0) {
    git add .
    git commit -m "Init"
    git branch -M main
}
git branch --set-upstream-to=origin/main main 2>$null >$null

# 1. Backup Changes
Write-Step "Step 1: Saving your local work..."
$stashed = $false
$changes = (git status --porcelain -uno | Out-String).Trim()
if ($changes) {
    $dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    git stash push -m "Backup $dateStr"
    Write-Host "[OK] Your changes saved safely." -ForegroundColor Green
    $stashed = $true
} else {
    Write-Host "[OK] No local changes to save." -ForegroundColor Green
}

# 2. Update from GitHub
Write-Step "Step 2: Checking for updates on GitHub..."
git fetch origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] Error: GitHub connection failed." -ForegroundColor Red
} else {
    $newCommits = (git log HEAD..FETCH_HEAD --oneline | Out-String).Trim()
    if (!$newCommits) {
        Write-Host "[OK] You already have the latest version." -ForegroundColor Green
    } else {
        Write-Host "Found new updates:" -ForegroundColor Yellow
        git log HEAD..FETCH_HEAD --oneline --color
        
        Write-Host ">>> Downloading updates..." -ForegroundColor Magenta
        git pull origin main --allow-unrelated-histories -X ours --progress
        
        if (Test-Path ".git/MERGE_HEAD") {
            git checkout --theirs Managed/* 2>$null
            git add Managed/*
            git commit -m "Updated Managed Libraries"
        }
        Write-Host "[OK] Update successful!" -ForegroundColor Green
    }

    # 3. Restore Changes
    Write-Step "Step 3: Restoring your plugins..."
    if ($stashed) {
        git stash pop
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[!] Warning: Merge conflicts found. Check your files manually." -ForegroundColor Yellow
        } else {
            Write-Host "[OK] Your plugins restored successfully." -ForegroundColor Green
        }
    } else {
        Write-Host "[OK] Nothing to restore." -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " DONE! EVERYTHING IS SAFE." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
