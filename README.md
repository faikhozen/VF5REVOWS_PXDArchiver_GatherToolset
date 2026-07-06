# VF5Revo World Stage Mod Compiler (v1.0)

**Developed by:** Fai Khozen  
**GitHub Repository:** [faikhozen/VF5REVOWS_PXDArchiver_GatherToolset](https://github.com/faikhozen/VF5REVOWS_PXDArchiver_GatherToolset)

---

## Description
A high-performance C# utility tool designed to compile, merge, and organize **VF5Revo World Stage** mods directly into your game's `chara.par` archive. 

This updated compiler operates **completely in-memory**, eliminating the need for extracting archives to disk or copying files to a temporary `__all` directory. It overlays your mods on top of the original archive in a single pass, resulting in a **3.2x speedup** and saving gigabytes of SSD write wear.

> ⚠️ **Important Requirement:** This tool is strictly compatible **only** with the **StandAlone DLC costume** format.

---

## Key Features
* 🚀 **In-Memory Compilation**: Compiles your modded `chara.par` archive in a single pass without copying files to the disk. Merges 5,000+ files in under 2 minutes!
* 🖥️ **Guided Interactive Mode**: Double-clicking the executable launches an interactive wizard that lets you pick your game's `chara.par` using a standard Windows file-picker dialog.
* 💾 **Disk Space Optimization**: The tool automatically checks your drive space. If at least 15 GB is free, it copies the PAR locally next to the compiler for maximum SSD read/write speeds, then automatically cleans it up when finished.
* 📊 **Visual Completion Bar**: Displays a real-time, in-place progress bar during compilation:
  `Progress: [██████████████████░░░░░░░░░░░░] 60% (11124/18546)`
* 🔍 **Built-in GMD Conflict Detection**: Scans and compares all mod files recursively. If multiple mods are trying to overwrite the same StandAlone DLC slots, the tool prints a color-coded warning table showing which mod currently holds priority.
* 📁 **Correct Nesting Structure**: Resolves the original archive's nested dot directories (`/./` and `/././`) automatically, ensuring modded slots overwrite the correct target files so that the game registers them successfully.

---

## Files Included
* `VF5REVOWS_mod_compiler.exe` — The main compiled executable. Can be double-clicked for a guided compile, or run from the CLI.
* `compile_mods_directly.bat` — A quick batch script shortcut to run the compiler on `chara_bak.par`.

---

## How to Use

### Guided Interactive Mode (Recommended)
1. **Setup Folder**: Ensure `VF5REVOWS_mod_compiler.exe` is placed in a folder of your choice.
2. **Add Mods**: Place your StandAlone DLC mod folders inside the `./mods` directory next to the executable (the tool will create this folder for you on first launch if it's missing).
3. **Run the Compiler**: Double-click `VF5REVOWS_mod_compiler.exe`.
4. **Select chara.par**: When prompted, press any key. A Windows file-picker dialog will open. Navigate to your Steam installation directory and select the reference `chara.par` file (you can also select backups like `chara_backup.par`, `chara_original.par`, or `chara__bak.par`):
   `{Steam_directory}\steamapps\common\VFREVO\runtime\media\data\chara.par`
5. **Compile**: The tool will scan your mods, report any slot conflicts, and merge your files.
6. **Deploy**:
   * Back up your original `chara.par` in your Steam folder (e.g. rename it to `chara_original.par`).
   * Copy the newly compiled `chara.par` from the compiler's `./output/chara.par` folder directly into your Steam game data folder:
     `{Steam_directory}\steamapps\common\VFREVO\runtime\media\data\`

---

## Troubleshooting & Tips
* **Fast Compilation**: SLLZ compression is disabled by default (`--compression 0`) to compile your PAR files in seconds. Uncompressed archives are fully supported by the game. If you wish to save disk space and use SLLZ compression, you can compile via the CLI or edit the batch file to pass `--compression 1` (note: this will take several minutes to run).
* **Game Crashes / Mods Overlapping**: If your mods overlap, check the **GMD CONFLICTS DETECTED** table in the console output. It lists which mods are trying to use the same slot and tells you which mod is currently taking priority.

---

## Credits & Attributions
* This project uses code and libraries from the [Ret-HZ/pxdArchiverCE](https://github.com/Ret-HZ/pxdArchiverCE) project.

