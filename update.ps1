# Скрипт для безопасного обновления шаблона rust-template
# Сохраняет ваши изменения, подтягивает обновления с GitHub и возвращает изменения назад.

$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "--- Обновление rust-template с GitHub ---" -ForegroundColor Cyan

# 1. Сохраняем локальные изменения (если вы меняли файлы шаблона, например .csproj или конфиги)
Write-Host "1. Сохранение локальных изменений (stash)..."
git stash

# 2. Получаем последние обновления (включая новые DLL в папке Managed)
Write-Host "2. Получение обновлений (pull)..."
git pull origin main

# 3. Возвращаем ваши локальные изменения назад
Write-Host "3. Восстановление локальных изменений (stash pop)..."
git stash pop

Write-Host "`nГотово! Ваши плагины в папке 'plugins/' в безопасности, а файлы шаблона и DLL обновлены." -ForegroundColor Green
