# Скрипт ЖЕСТКОГО обновления системных файлов rust-template
$OutputEncoding = [Console]::InputEncoding = [Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($true)

function Write-Header($text) {
    Write-Host "`n==========================================" -ForegroundColor Yellow
    Write-Host "   $text" -ForegroundColor Yellow
    Write-Host "==========================================`n" -ForegroundColor Yellow
}

Write-Header "ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ ШАБЛОНА"

# 1. Скачиваем актуальное состояние с GitHub
Write-Host ">>> Шаг 1: Получение обновлений с сервера..." -ForegroundColor Cyan
git fetch origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "[FAIL] Не удалось подключиться к GitHub!" -ForegroundColor Red
    exit 1
}

# 2. Список системных путей, которые нужно ОБЯЗАТЕЛЬНО обновить/вернуть
$SystemPaths = @(
    "Managed/",
    ".github/",
    "update.bat",
    "update.ps1",
    "rust-template.sln",
    "rust.template.csproj"
)

Write-Host ">>> Шаг 2: Принудительная перезапись системных файлов..." -ForegroundColor Magenta

foreach ($path in $SystemPaths) {
    Write-Host "Обновление: $path" -ForegroundColor Gray
    # Сбрасываем локальные изменения в этих путях и заменяем их на версии из FETCH_HEAD
    git checkout FETCH_HEAD -- $path 2>$null
}

Write-Header "ГОТОВО! Системные файлы обновлены."
Write-Host "Ваши плагины в папке plugins/ не были затронуты." -ForegroundColor Green
Write-Host "Нажмите любую клавишу для выхода..." -ForegroundColor Gray
