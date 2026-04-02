@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================
powershell -NoProfile -ExecutionPolicy Bypass -File "update.ps1"
pause
