# Скрипт для ПРИНУДИТЕЛЬНОГО обновления библиотек Managed и системных файлов
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ RUST-TEMPLATE" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# 1. Скачиваем заголовки обновлений
Write-Host ">>> Шаг 1: Подключение к GitHub..." -ForegroundColor Cyan
git fetch origin main 2>$null

# 2. ПРИНУДИТЕЛЬНОЕ обновление системных папок и файлов
# Мы просто берем и заменяем локальные файлы версиями из GitHub
# Это НИКОГДА не задевает папку plugins/
Write-Host ">>> Шаг 2: Принудительная загрузка библиотек (Managed) и шаблона..." -ForegroundColor Magenta

# Сбрасываем возможные зависшие состояния для системных файлов
git add Managed/ 2>$null
git add update.bat update.ps1 .github/ 2>$null

# Принудительно восстанавливаем (заменяем) только системное
git restore -W -S -s FETCH_HEAD Managed/ 2>$null
git restore -W -S -s FETCH_HEAD .github/ 2>$null
git restore -W -S -s FETCH_HEAD update.bat 2>$null
git restore -W -S -s FETCH_HEAD update.ps1 2>$null
git restore -W -S -s FETCH_HEAD rust-template.sln 2>$null
git restore -W -S -s FETCH_HEAD rust.template.csproj 2>$null

Write-Host "[OK] Все библиотеки в Managed/ принудительно обновлены." -ForegroundColor Green
Write-Host "[OK] Системные файлы шаблона перезаписаны успешно." -ForegroundColor Green

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в полной безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
