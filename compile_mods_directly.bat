@echo off
title PAR Mod Compiler (In-Memory)
echo =================================================================
echo PAR Mod Compiler (In-Memory / Space-Efficient)
echo =================================================================
echo.

if not exist "chara_bak.par" (
    if exist "chara.par" (
        echo Creating reference backup (chara_bak.par)...
        copy "chara.par" "chara_bak.par"
    ) else (
        echo ERROR: Neither "chara_bak.par" nor "chara.par" was found in this folder.
        echo Please ensure your original chara.par or chara_bak.par is in the same directory.
        echo.
        pause
        exit /b 1
    )
)

echo Running in-memory overlay compile:
echo   Reference:   chara_bak.par
echo   Mods Folder: mods/
echo   Output:      output/chara.par
echo.

REM Runs compilation uncompressed for maximum speed (use -c 1 for SLLZ compression)
.\VF5REVOWS_mod_compiler.exe gather-compile chara_bak.par --mods mods --output output/chara.par --compression 0

echo.
pause
