@echo off
setlocal enabledelayedexpansion

REM =====================================================
REM VF5REVO Mod File Gatherer v0.67
REM Developed by: Fai Khozen
REM =====================================================

echo.
echo =====================================================
echo VF5REVO Mod File Gatherer v0.67
echo Developed by: Fai Khozen
echo.
echo Description: This tool organizes mod folder structures
echo for use with PXDArchiver software in VF5REVO 
echo WorldStage modding. It gathers character files (.gmd,
echo .dds, .bone) from multiple mod directories into a
echo unified structure and detects file conflicts.
echo =====================================================
echo.
set "SCRIPT_DIR=%~dp0"
REM Source directory is mods subfolder within script directory
set "SOURCE_DIR=%SCRIPT_DIR%mods"

REM Destination stays in the script folder
set "ALL_DIR=%SCRIPT_DIR%__all"

REM 1. Delete the __all directory to clean up existing files
if exist "%ALL_DIR%" (
    echo Cleaning up %ALL_DIR%...
    rmdir /s /q "%ALL_DIR%"
)

REM Recreate the necessary directory structure
mkdir "%ALL_DIR%\chara\bone" 2>nul
mkdir "%ALL_DIR%\chara\dds" 2>nul
mkdir "%ALL_DIR%\chara\dds_append" 2>nul
mkdir "%ALL_DIR%\chara\tops" 2>nul
mkdir "%ALL_DIR%\chara\vf5item" 2>nul

REM Create temp files for tracking
set "CONFLICT_LOG=%SCRIPT_DIR%gmd_conflicts.txt"
set "GMD_HISTORY=%SCRIPT_DIR%gmd_history.txt"
del /f /q "%CONFLICT_LOG%" 2>nul
del /f /q "%GMD_HISTORY%" 2>nul

REM 2. Loop through all folders in the mods directory
if not exist "%SOURCE_DIR%\" (
    echo Source directory not found: %SOURCE_DIR%
    pause
    exit /b 1
)

for /d %%D in ("%SOURCE_DIR%\*") do (
    REM Skip __all folder
    if /i not "%%~nxD"=="__all" (
        set "HAS_GMD_FILES=0"
        
        if exist "%%D\chara\bone\*" (
            copy /y "%%D\chara\bone\*.*" "%ALL_DIR%\chara\bone\" >nul
        )
        if exist "%%D\chara\dds\*" (
            copy /y "%%D\chara\dds\*.*" "%ALL_DIR%\chara\dds\" >nul
        )
        if exist "%%D\chara\dds_append\*" (
            copy /y "%%D\chara\dds_append\*.*" "%ALL_DIR%\chara\dds_append\" >nul
        )
        if exist "%%D\chara\vf5item\*" (
            REM Copy the entire vf5item subdirectory tree
            xcopy /E /Y /Q "%%D\chara\vf5item\*" "%ALL_DIR%\chara\vf5item\" >nul 2>&1
            
            REM Check for .gmd files directly in chara\vf5item folder
            for %%G in ("%%D\chara\vf5item\*.gmd") do (
                if exist "%%G" (
                    set "HAS_GMD_FILES=1"
                    set "REL_PATH=%%G"
                    set "REL_PATH=!REL_PATH:%%D\chara\vf5item\=!"
                    set "REL_PATH_FWD=!REL_PATH:\=/!"
                    echo Gathering from: %%~nxD ^|^| /vf5item/!REL_PATH_FWD!
                    echo /vf5item/!REL_PATH_FWD!-%%~nxD >> "%GMD_HISTORY%"
                )
            )
            REM Check for .gmd files in immediate subdirectories (chara\vf5item\<CHAR_ID>)
            for /d %%C in ("%%D\chara\vf5item\*") do (
                if exist "%%C\" (
                    for %%G in ("%%C\*.gmd") do (
                        if exist "%%G" (
                            set "HAS_GMD_FILES=1"
                            set "REL_PATH=%%G"
                            set "REL_PATH=!REL_PATH:%%D\chara\vf5item\=!"
                            set "REL_PATH_FWD=!REL_PATH:\=/!"
                            echo Gathering from: %%~nxD ^|^| /vf5item/!REL_PATH_FWD!
                            echo /vf5item/!REL_PATH_FWD!-%%~nxD >> "%GMD_HISTORY%"
                        )
                    )
                    REM Check for .gmd files in nested subdirectories (chara\vf5item\<CHAR_ID>\<ITEM_ID>)
                    for /d %%I in ("%%C\*") do (
                        if exist "%%I\" (
                            for %%G in ("%%I\*.gmd") do (
                                if exist "%%G" (
                                    set "HAS_GMD_FILES=1"
                                    set "REL_PATH=%%G"
                                    set "REL_PATH=!REL_PATH:%%D\chara\vf5item\=!"
                                    set "REL_PATH_FWD=!REL_PATH:\=/!"
                                    echo Gathering from: %%~nxD ^|^| /vf5item/!REL_PATH_FWD!
                                    echo /vf5item/!REL_PATH_FWD!-%%~nxD >> "%GMD_HISTORY%"
                                )
                            )
                        )
                    )
                )
            )
        )

        REM Only copy subfolders inside chara\tops
        if exist "%%D\chara\tops\*" (
            REM Check for .gmd files directly in chara\tops folder (these get deleted, so skip logging)
            for %%G in ("%%D\chara\tops\*.gmd") do (
                if exist "%%G" (
                    set "HAS_GMD_FILES=1"
                    set "REL_PATH_FWD=%%~nxG"
                    
                    REM Display the mod and gmd file being gathered
                    echo Gathering from: %%~nxD ^|^| /!REL_PATH_FWD!
                    
                    REM Note: Not adding to history because these files are deleted after gathering
                )
            )
            
            REM Now check for .gmd files in immediate subdirectories
            for /d %%T in ("%%D\chara\tops\*") do (
                if exist "%%T\" (
                    for %%G in ("%%T\*.gmd") do (
                        if exist "%%G" (
                            set "HAS_GMD_FILES=1"
                            set "REL_PATH=%%G"
                            set "REL_PATH=!REL_PATH:%%D\chara\tops\=!"
                            set "REL_PATH_FWD=!REL_PATH:\=/!"
                            set "GMD_FILENAME=%%~nxG"
                            set "FOLDER_NAME=%%~nxT"
                            set "FILENAME_NO_EXT=%%~nG"
                            
                            REM Display the mod and gmd file being gathered
                            echo Gathering from: %%~nxD ^|^| /!REL_PATH_FWD!
                            
                            REM Only add to history if folder name matches filename (important files)
                            REM e.g., /c_v63_VF5_AKI_TEK/c_v63_VF5_AKI_TEK.gmd
                            if /i "!FOLDER_NAME!"=="!FILENAME_NO_EXT!" (
                                echo /!REL_PATH_FWD!-%%~nxD >> "%GMD_HISTORY%"
                            )
                        )
                    )
                    
                    REM Copy the entire subdirectory tree
                    xcopy /E /Y /Q "%%T\*" "%ALL_DIR%\chara\tops\%%~nxT\" >nul 2>&1
                )
            )
        )
        
        REM If no .gmd files, still show gathering message
        if "!HAS_GMD_FILES!"=="0" (
            echo Gathering from: %%~nxD
        )
    )
)

REM Check for conflicts using PowerShell helper script
if exist "%GMD_HISTORY%" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%check_gmd_conflicts.ps1" -HistoryFile "%GMD_HISTORY%"
) else (
    echo No GMD conflicts detected.
)

REM Clean up history
del /f /q "%GMD_HISTORY%" 2>nul
del /f /q "%CONFLICT_LOG%" 2>nul

echo Done!
pause