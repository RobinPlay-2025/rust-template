@echo off
:: RUST-TEMPLATE LAUNCHER
chcp 65001 >nul
echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================
powershell -NoProfile -ExecutionPolicy Bypass -File "update.ps1"
pause
