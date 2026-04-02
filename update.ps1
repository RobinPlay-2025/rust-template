[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "   ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ RUST-TEMPLATE" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# Шаг 1
Write-Host ">>> Шаг 1: Подключение к GitHub..." -ForegroundColor Cyan
git fetch origin main 2>$null

# Шаг 2
Write-Host ">>> Шаг 2: Принудительная загрузка библиотек..." -ForegroundColor Magenta

# Сброс ошибок индекса
git add Managed/ 2>$null
git add update.bat update.ps1 .github/ 2>$null

# Прямая замена файлов
git restore -W -S -s FETCH_HEAD Managed/ 2>$null
git restore -W -S -s FETCH_HEAD .github/ 2>$null
git restore -W -S -s FETCH_HEAD update.bat 2>$null
git restore -W -S -s FETCH_HEAD update.ps1 2>$null
git restore -W -S -s FETCH_HEAD rust-template.sln 2>$null
git restore -W -S -s FETCH_HEAD rust.template.csproj 2>$null

Write-Host "[OK] Библиотеки в Managed/ обновлены." -ForegroundColor Green
Write-Host "[OK] Системные файлы перезаписаны." -ForegroundColor Green

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host " Готово! Ваши плагины в plugins/ в безопасности." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Yellow
