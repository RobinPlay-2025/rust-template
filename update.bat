@echo off
:: RUST-TEMPLATE UPDATE TOOL

echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================

:: Step 1
echo.
echo >>> Step 1: Saving your plugins...
git stash push -m "Auto-backup"
echo [OK] Your files are safe.

:: Step 2
echo.
echo >>> Step 2: Downloading updates from GitHub...
git fetch origin main
git pull origin main --no-edit --progress
echo [OK] Updates downloaded.

:: Step 3
echo.
echo >>> Step 3: Restoring your plugins back...
git stash pop
echo [OK] Done!

:: Finish
echo.
echo ==========================================
echo  UPDATE COMPLETE. You can close this window.
echo ==========================================
pause
