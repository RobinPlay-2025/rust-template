# Скрипт для безопасного обновления шаблона rust-template
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$env:LC_ALL = 'ru_RU.UTF-8'

$ErrorActionPreference = 'SilentlyContinue'

function Write-Step([string]$msg) {
    Write-Host ""
    Write-Host ">>> $msg" -ForegroundColor Cyan
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   ОБНОВЛЕНИЕ ШАБЛОНА RUST-TEMPLATE       " -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# 0. Подготовка Git
if (!(Test-Path ".git")) {
    Write-Step "Подготовка репозитория..."
    git init 2>$null >$null
}

$gitUser = git config user.name 2>$null
if (!$gitUser) {
    git config user.email "template-user@example.com" 2>$null >$null
    git config user.name "Rust-Template User" 2>$null >$null
}

git remote get-url origin 2>$null >$null
if ($LASTEXITCODE -ne 0) {
    git remote add origin https://github.com/RobinPlay-2025/rust-template.git 2>$null >$null
} else {
    git remote set-url origin https://github.com/RobinPlay-2025/rust-template.git 2>$null >$null
}

if (Test-Path ".git/MERGE_HEAD") {
    git merge --abort 2>$null >$null
}

git rev-parse HEAD 2>$null >$null
if ($LASTEXITCODE -ne 0) {
    git add . 2>$null >$null
    git commit -m "Initial State" 2>$null >$null
    git branch -M main 2>$null >$null
}
git branch --set-upstream-to=origin/main main 2>$null >$null

# 1. Сохранение изменений
Write-Step "1. Сохранение ваших файлов..."
$stashed = $false
$changes = (git status --porcelain -uno 2>$null | Out-String).Trim()
if ($changes) {
    $dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    git stash push -m "Auto-backup $dateStr" 2>$null >$null
    Write-Host "[OK] Ваши файлы сохранены в бэкап." -ForegroundColor Green
    $stashed = $true
} else {
    Write-Host "[OK] Изменений не найдено." -ForegroundColor Green
}

# 2. Обновление
Write-Step "2. Проверка обновлений на GitHub..."
git fetch origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] Ошибка: Нет связи с GitHub." -ForegroundColor Red
} else {
    $newCommits = (git log HEAD..FETCH_HEAD --oneline 2>$null | Out-String).Trim()
    if (!$newCommits) {
        Write-Host "[OK] У вас последняя версия." -ForegroundColor Green
    } else {
        Write-Host "Доступны обновления:" -ForegroundColor Yellow
        git log HEAD..FETCH_HEAD --oneline --color
        Write-Host ">>> Скачивание новых файлов..." -ForegroundColor Magenta
        git pull origin main --allow-unrelated-histories -X ours --progress

        if (Test-Path ".git/MERGE_HEAD") {
            git checkout --theirs Managed/* 2>$null >$null
            git add Managed/* 2>$null >$null
            git commit -m "Updated Managed libraries" 2>$null >$null
        }
        Write-Host "[OK] Обновление завершено." -ForegroundColor Green
    }

    # 3. Восстановление
    Write-Step "3. Возврат ваших плагинов на место..."
    if ($stashed) {
        git stash pop 2>$null >$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[!] Внимание: Есть конфликты. Проверьте файлы вручную." -ForegroundColor Yellow
        } else {
            Write-Host "[OK] Все ваши файлы восстановлены." -ForegroundColor Green
        }
    } else {
        Write-Host "[OK] Ваши файлы не менялись." -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
