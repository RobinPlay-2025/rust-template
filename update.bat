@echo off
:: RUST-TEMPLATE SIMPLE UPDATE TOOL

echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================

:: Step 1: Backup your plugins
echo.
echo >>> Step 1: Saving your local work...
git stash push -m "Auto-backup"
echo [OK] Your plugins are saved safely.

:: Step 2: Download Updates
echo.
echo >>> Step 2: Downloading updates from GitHub...
git fetch origin main
git pull origin main --no-edit --progress
echo [OK] Updates downloaded successfully.

:: Step 3: Restore your plugins
echo.
echo >>> Step 3: Restoring your plugins back...
git stash pop
echo [OK] Your files are restored.

:: Finish
echo.
echo ==========================================
echo  DONE! You can now close this window.
echo ==========================================
pause
