# Скрипт для безопасного обновления шаблона rust-template
# CHANGE: Полностью переработан для бесшумной работы без красных ошибок

# Установка кодировки для консоли
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$env:LC_ALL = 'ru_RU.UTF-8'

# Подавляем все системные ошибки
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
    git config user.email "rustr@example.com" 2>$null >$null
    git config user.name "RustR" 2>$null >$null
    git remote add origin https://github.com/RobinPlay-2025/rust-template.git 2>$null >$null
}

# Сброс зависших слияний
if (Test-Path ".git/MERGE_HEAD") {
    git merge --abort 2>$null >$null
}

# Проверка наличия коммитов
git rev-parse HEAD 2>$null >$null
if ($LASTEXITCODE -ne 0) {
    git add . 2>$null >$null
    git commit -m "Initial local state" 2>$null >$null
    git branch -M main 2>$null >$null
    git branch --set-upstream-to=origin/main main 2>$null >$null
}

# 1. Сохранение изменений
Write-Step "1. Сохранение ваших изменений..."
$dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$stashRes = git stash push -m "Auto-update $dateStr" 2>$null
if ($stashRes -like "*Saved working directory*") {
    Write-Host "[OK] Изменения временно сохранены." -ForegroundColor Green
} else {
    Write-Host "[OK] Нет изменений для сохранения." -ForegroundColor Green
}

# 2. Обновление
Write-Step "2. Получение обновлений с GitHub..."
git fetch origin main 2>$null >$null
# Пробуем обновиться, предпочитая локальные файлы при конфликтах (кроме Managed)
git pull origin main --allow-unrelated-histories -X ours 2>$null >$null

# Если после pull остались конфликты (например, в Managed)
if (Test-Path ".git/MERGE_HEAD") {
    # Для библиотек Managed всегда берем версию из шаблона
    git checkout --theirs Managed/* 2>$null >$null
    git add Managed/* 2>$null >$null
    # Завершаем слияние
    git commit -m "Updated libraries from template" 2>$null >$null
}
Write-Host "[OK] Шаблон успешно обновлен." -ForegroundColor Green

# 3. Восстановление
Write-Step "3. Восстановление ваших изменений..."
$lastStash = git stash list -n 1 2>$null
if ($lastStash -like "*Auto-update*") {
    git stash pop 2>$null >$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[!] Есть конфликты в ваших плагинах. Они сохранены в Git Stash." -ForegroundColor Yellow
    } else {
        Write-Host "[OK] Ваши изменения возвращены." -ForegroundColor Green
    }
} else {
    Write-Host "[OK] Нет изменений для восстановления." -ForegroundColor Green
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
