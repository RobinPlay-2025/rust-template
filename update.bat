@echo off
echo ==========================================
echo    RUST-TEMPLATE UPDATE TOOL
echo ==========================================
echo.
echo [Step 1] Saving your local work...
git stash push -m "Auto-update"
echo.
echo [Step 2] Downloading updates from GitHub...
git fetch origin main
git pull origin main --no-edit --progress 
echo.
echo [Step 3] Restoring your plugins...
git stash pop
echo.
echo ==========================================
echo  DONE! You can close this window now.
echo ==========================================
pause
