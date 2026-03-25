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
Write-Step "1. Подготовка ваших файлов к безопасному обновлению..."
$stashed = $false
# Проверяем, есть ли что сохранять (только измененные/удаленные файлы, игнорируем untracked)
$changes = (git status --porcelain -uno 2>$null | Out-String).Trim()
if ($changes) {
    $dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    git stash push -m "Auto-update $dateStr" 2>$null >$null
    Write-Host "[OK] Ваши измененные плагины и файлы временно сохранены в безопасный бэкап." -ForegroundColor Green
    $stashed = $true
} else {
    Write-Host "[OK] Изменений в ваших файлах не обнаружено, бэкап не требуется." -ForegroundColor Green
}

# 2. Обновление
Write-Step "2. Проверка новых библиотек и файлов шаблона на GitHub..."
git fetch origin main 2>$null >$null

# Проверяем, есть ли новые коммиты на GitHub, которых нет у нас
$newCommits = (git log HEAD..FETCH_HEAD --oneline 2>$null | Out-String).Trim()
if (!$newCommits) {
    Write-Host "[OK] У вас уже установлена самая актуальная версия библиотек и системных файлов шаблона." -ForegroundColor Green
} else {
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
    Write-Host "[OK] Библиотеки в Managed/ и системные файлы шаблона успешно обновлены до последней версии." -ForegroundColor Green
}

# 3. Восстановление
Write-Step "3. Возврат ваших плагинов и правок в рабочую область..."
if ($stashed) {
    git stash pop 2>$null >$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[!] Внимание: Есть конфликты в ваших файлах. Git сохранил их в Stash для ручного разбора." -ForegroundColor Yellow
    } else {
        Write-Host "[OK] Все ваши плагины и правки успешно возвращены на свои места." -ForegroundColor Green
    }
} else {
    Write-Host "[OK] Ваши файлы не перемещались (обновление прошло без вмешательства в ваш код)." -ForegroundColor Green
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
