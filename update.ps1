# Скрипт для безопасного обновления шаблона rust-template
# CHANGE: Упрощена логика для PowerShell 5.1

# Установка кодировки для консоли
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$env:LC_ALL = 'ru_RU.UTF-8'

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   ОБНОВЛЕНИЕ ШАБЛОНА RUST-TEMPLATE       " -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# 1. Сохраняем локальные изменения
Write-Host "`n>>> 1. Сохранение локальных изменений (git stash)..." -ForegroundColor Cyan
$dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$stashMsg = "Auto-update $dateStr"
git stash push -m "$stashMsg"

# 2. Получаем последние обновления
Write-Host "`n>>> 2. Получение обновлений (git pull)..." -ForegroundColor Cyan
git fetch origin main
git pull origin main 2>&1

# 3. Возвращаем локальные изменения назад
Write-Host "`n>>> 3. Восстановление локальных изменений (git stash pop)..." -ForegroundColor Cyan
$lastStash = git stash list -n 1
if ($lastStash -like "*$stashMsg*") {
    git stash pop
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n[!] ВНИМАНИЕ: Возникли конфликты при восстановлении ваших изменений." -ForegroundColor Yellow
        Write-Host "Ваши изменения сохранены в stash. Вы можете применить их вручную командой: git stash pop" -ForegroundColor Gray
    } else {
        Write-Host "[OK] Изменения успешно восстановлены." -ForegroundColor Green
    }
} else {
    Write-Host "[OK] Нет локальных изменений для восстановления." -ForegroundColor Green
}

Write-Host "`n==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
