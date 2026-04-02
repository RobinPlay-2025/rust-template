# Настройка кодировки под системную (обычно Windows-1251 для РФ)
[Console]::OutputEncoding = [System.Text.Encoding]::Default

function Write-Header($text) {
    Write-Host "`n>>> $text <<<`n" -ForegroundColor Yellow
}

Write-Header "ОБНОВЛЕНИЕ ШАБЛОНА (UPDATE)"

# 1. Скачиваем актуальное состояние с GitHub
Write-Host "--- Шаг 1: Проверка связи с GitHub ---" -ForegroundColor Cyan
git fetch origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "!!! ОШИБКА: Нет связи с сервером !!!" -ForegroundColor Red
    exit 1
}

# 2. Список системных путей
$SystemPaths = @(
    "Managed/",
    ".github/",
    "update.bat",
    "update.ps1",
    "rust-template.sln",
    "rust.template.csproj"
)

Write-Host "--- Шаг 2: Замена системных файлов ---" -ForegroundColor Magenta

foreach ($path in $SystemPaths) {
    # Принудительно вытягиваем файл из FETCH_HEAD (последний скачанный main)
    git checkout FETCH_HEAD -- $path 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Обновлено: $path" -ForegroundColor Gray
    }
}

Write-Header "ГОТОВО! ОБНОВЛЕНИЕ ЗАВЕРШЕНО"
Write-Host "Ваши плагины в plugins/ не пострадали." -ForegroundColor Green
