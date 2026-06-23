# VF5Revo World Stage Mod Gatherer (v0.67)

**Developed by:** Fai Khozen

## Description
A lightweight utility tool designed to compile and organize **VF5Revo World Stage** mods into a clean, easy-to-use draggable format specifically tailored for **PXDArchiver**. 

> ⚠️ **Important Requirement:** This tool is strictly compatible **only** with the **StandAlone DLC costume** format.

## Features
* **Conflict Detection:** Includes a dedicated PowerShell script to check for `.gmd` file conflicts, preventing overlapping mod IDs from crashing your game.
* **Draggable Format:** No manual sorting required—just drag, drop, and archive.

## Files Included
* `VF5REVOWS__gather_files_PXDArchiver__v0.67.bat` — The main batch script that handles file compilation and structuring.
* `check_gmd_conflicts.ps1` — A helper script that scans your mods to ensure there are no duplicate/conflicting model data files.

## How to Use
📺 **Video Guide:** For an easy visual explanation on how to use this tool, please refer to this tutorial: **[YouTube Video Guide](https://youtu.be/A0wgoF7rPgk)**

1. **Setup Directory:** Ensure you have a `mods` folder located in the same directory as the script. Place your StandAlone DLC mods inside this `mods` folder.
2. **Run the Script:** Double-click `VF5REVOWS__gather_files_PXDArchiver__v0.67.bat` to begin the compilation process.
3. **Review Output & Conflicts:** The tool will gather the files into an `__all` directory and automatically run the conflict checker to ensure your StandAlone DLC slots aren't overwriting each other.
4. **Archive:** Once finished, refer to the video https://youtu.be/A0wgoF7rPgk on how to use it with PXDArchiver in mind

## Limitations
* **StandAlone DLC Only:** Does not support standard model replacements or loose texture overwrites unless they are properly structured within a StandAlone DLC layout.
