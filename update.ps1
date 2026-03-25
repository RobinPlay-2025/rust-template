# Скрипт для безопасного обновления шаблона rust-template
# CHANGE: Добавлена поддержка --allow-unrelated-histories для первого запуска

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$env:LC_ALL = 'ru_RU.UTF-8'

function Write-Step([string]$msg) {
    Write-Host ""
    Write-Host ">>> $msg" -ForegroundColor Cyan
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   ОБНОВЛЕНИЕ ШАБЛОНА RUST-TEMPLATE       " -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# Проверка на наличие git
git --version >$null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] ОШИБКА: Git не установлен! Скачайте его с https://git-scm.com/" -ForegroundColor Red
    return
}

# Проверка, является ли папка репозиторием
if (!(Test-Path ".git")) {
    Write-Host "[!] ВНИМАНИЕ: Папка не является Git-репозиторием." -ForegroundColor Yellow
    Write-Host "Инициализация и привязка к GitHub..." -ForegroundColor Gray
    
    git init
    git config user.email "rustr@example.com"
    git config user.name "RustR"
    git remote add origin https://github.com/RobinPlay-2025/rust-template.git
    git fetch origin main
}

# Проверяем, есть ли коммиты
git rev-parse HEAD >$null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] Первый запуск: Создание базового состояния..." -ForegroundColor Gray
    git add .
    git commit -m "Initial local state"
    git branch -M main
    git branch --set-upstream-to=origin/main main
}

# 1. Сохраняем локальные изменения
Write-Step "1. Сохранение локальных изменений (git stash)..."
$dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$stashMsg = "Auto-update $dateStr"
$stashRes = git stash push -m "$stashMsg"
Write-Host $stashRes

# 2. Получаем последние обновления
Write-Step "2. Получение обновлений (git pull)..."
git fetch origin main
# Используем --allow-unrelated-histories для первого слияния
git pull origin main --allow-unrelated-histories -X ours 2>&1

# 3. Возвращаем локальные изменения назад
Write-Step "3. Восстановление локальных изменений (git stash pop)..."
if ($stashRes -notlike "*No local changes*") {
    git stash pop
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "[!] ВНИМАНИЕ: Возникли конфликты при восстановлении ваших изменений." -ForegroundColor Yellow
        Write-Host "Ваши изменения сохранены в stash. Вы можете применить их вручную командой: git stash pop" -ForegroundColor Gray
    } else {
        Write-Host "[OK] Изменения успешно восстановлены." -ForegroundColor Green
    }
} else {
    Write-Host "[OK] Нет локальных изменений для восстановления." -ForegroundColor Green
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
