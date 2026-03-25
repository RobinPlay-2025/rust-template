@echo off
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "update.ps1"
pause
