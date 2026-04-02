@echo off
:: RUST-TEMPLATE LAUNCHER
:: Force UTF-8 encoding for CMD
chcp 65001 >nul
echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================

:: Run PowerShell with UTF-8 input/output
powershell -NoProfile -ExecutionPolicy Bypass -Command "$OutputEncoding = [Console]::InputEncoding = [Console]::OutputEncoding = New-Object System.Text.UTF8Encoding; & './update.ps1'"

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Update failed. Check your internet connection or git installation.
)

pause
