@echo off
chcp 65001 >nul
cd /d "%~dp0"

echo --- Обновление rust-template с GitHub ---

echo 1. Сохранение локальных изменений (stash)...
git stash

echo 2. Получение обновлений (pull)...
git pull origin main

echo 3. Восстановление локальных изменений (stash pop)...
git stash pop

echo.
echo Готово! Ваши плагины в папке 'plugins/' в безопасности, а файлы шаблона и DLL обновлены.
pause
