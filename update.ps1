# Safe Rust Template Update (English Version for Total Stability)
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

$ErrorActionPreference = 'SilentlyContinue'

function Write-Step([string]$msg) {
    Write-Host ""
    Write-Host ">>> $msg" -ForegroundColor Cyan
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   RUST-TEMPLATE UPDATE TOOL              " -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

if (!(Test-Path ".git")) {
    git init
}

git config user.name "User" 2>$null >$null
git config user.email "user@example.com" 2>$null >$null

git remote get-url origin 2>$null >$null
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

Write-Step "Step 1: Saving your local work..."
$stashed = $false
$changes = (git status --porcelain -uno | Out-String).Trim()
if ($changes) {
    $dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    git stash push -m "Backup $dateStr" 2>$null >$null
    Write-Host "[OK] Changes saved successfully." -ForegroundColor Green
    $stashed = $true
}

Write-Step "Step 2: Checking for updates..."
git fetch origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] Error: No connection to GitHub." -ForegroundColor Red
} else {
    $newCommits = (git --no-pager log HEAD..FETCH_HEAD --oneline | Out-String).Trim()
    if (!$newCommits) {
        Write-Host "[OK] You have the latest version." -ForegroundColor Green
    } else {
        Write-Host "New updates found on GitHub:" -ForegroundColor Yellow
        git --no-pager log HEAD..FETCH_HEAD --oneline --color
        Write-Host ">>> Downloading updates..." -ForegroundColor Magenta
        git pull origin main --allow-unrelated-histories -X ours --no-edit --no-pager --progress

        if (Test-Path ".git/MERGE_HEAD") {
            git checkout --theirs Managed/* 2>$null >$null
            git add Managed/*
            git commit --no-edit -m "Updated libraries" 2>$null >$null
        }
        Write-Host "[OK] Update complete." -ForegroundColor Green
    }

    Write-Step "Step 3: Restoring your plugins..."
    if ($stashed) {
        git stash pop 2>$null >$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[!] Warning: Review merge conflicts manually." -ForegroundColor Yellow
        } else {
            Write-Host "[OK] Plugins restored successfully." -ForegroundColor Green
        }
    } else {
        Write-Host "[OK] No files were touched." -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " UPDATE COMPLETE! You can now CLOSE this window." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
