# VF5Revo World Stage Mod Compiler (v1.1)

**Developed by:** Fai Khozen  
**GitHub Repository:** [faikhozen/VF5REVOWS_PXDArchiver_GatherToolset](https://github.com/faikhozen/VF5REVOWS_PXDArchiver_GatherToolset)

---

## Description
A high-performance C# utility tool designed to compile, merge, and organize **VF5Revo World Stage** mods directly into your game's archives. 

This tool supports two types of compilations:
1. **Character Skin Mods (`chara.par`)**: Merges StandAlone DLC costumes into the character PAR.
2. **Sound Mods (`vf5fs_data.par`)**: Merges BGM, SFX, and voices into the sound data PAR, with special handling for voice line overrides.

This compiler operates **completely in-memory**, eliminating the need for extracting archives to disk or copying files to a temporary `__all` directory. It overlays your mods on top of the original archive in a single pass, saving gigabytes of SSD write wear and completing compiles in seconds.

---

## Key Features
* 🚀 **In-Memory Compilation**: Compiles your modded PAR archives in a single pass without copying files to disk.
* 🎵 **Sound Mod Support**: Compiles audio mods into `vf5fs_data.par` using the correct virtual structure (mapping to `rom/`).
* 🗣️ **Automated `auth_voice` Extraction**: Rather than injecting raw voice lines (`sound/voice/auth_voice`) directly into the sound PAR, the tool compiles them into a clean, dedicated `./output/auth_voice` directory for manual placement, ensuring the game runs them correctly.
* 🖥️ **Guided Interactive Mode**: Double-clicking the executable launches an interactive wizard that lets you choose the type of mod (Character Skins vs. Sounds) and pick the reference PAR using a standard Windows file-picker.
* 💾 **Disk Space Optimization**: For character skins, the tool automatically checks your drive space. If at least 15 GB is free, it copies the PAR locally next to the compiler for maximum SSD read/write speeds, then automatically cleans it up.
* ⚡ **Ultra-Fast Speeds**: Compilation for sound PARs disables recursive unpacking of nested archives. This speeds up compiling by **orders of magnitude** since untouched nested PARs are copied directly as binary objects.
* 📊 **Visual Completion Bar**: Displays a real-time, in-place progress bar during compilation.
* 🔍 **Built-in GMD Conflict Detection**: Scans and compares character mod files recursively. If multiple skin mods try to overwrite the same StandAlone DLC slots, the tool prints a color-coded warning table.

---

## Files Included
* `VF5REVOWS_mod_compiler.exe` — The main compiled executable. Can be double-clicked for a guided compile, or run from the CLI.
* `compile_mods_directly.bat` — A quick batch script shortcut to run the compiler on `chara_bak.par`.

---

## How to Use

### Guided Interactive Mode (Recommended)
1. **Setup Folder**: Ensure `VF5REVOWS_mod_compiler.exe` is placed in a folder of your choice.
2. **Add Mods**: Place your mod folders inside the `./mods` directory next to the executable (the tool will create this folder for you on first launch if it's missing).
   * **Skin Mods**: Place folders containing a `chara/` directory (with `bone`, `dds`, `tops`, etc.).
   * **Sound Mods**: Place folders containing a `rom/` directory (with `sound/` or `adx2_/`).
3. **Run the Compiler**: Double-click `VF5REVOWS_mod_compiler.exe`.
4. **Choose Mod Type**: Select whether you are compiling Character Skins (`chara.par`) or Sound mods (`vf5fs_data.par`).
5. **Select Reference PAR**: When the file-picker dialog opens, navigate to your game's directory and select the reference PAR file:
   * **Skins**: `{Steam_directory}\steamapps\common\VFREVO\runtime\media\data\chara.par`
   * **Sounds**: `{Steam_directory}\steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_data.par`
6. **Compile**: The tool will map your mods, output slot conflicts (if any), and merge the files.
7. **Deploy**:
   * **For Skins (`chara.par`)**: Copy the compiled `chara.par` from `./output/` to `{Steam_directory}\steamapps\common\VFREVO\runtime\media\data\`.
   * **For Sounds (`vf5fs_data.par`)**: 
     1. Copy the compiled `vf5fs_data.par` from `./output/` to `{Steam_directory}\steamapps\common\VFREVO\runtime\media\vf5fs\`.
     2. **Manual Voice Step**: If the compiler output contains an `auth_voice` folder, back up your original `auth_voice` folder at `{Steam_directory}\steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_media\rom\sound\voice\auth_voice`. Then copy all files from the compiled `output/auth_voice` and paste/overwrite them into that folder.

---

## Troubleshooting & Tips
* **Fast Compilation**: SLLZ compression is disabled by default (`--compression 0`) to compile your PAR files in seconds. Uncompressed archives are fully supported by the game. If you wish to save disk space and use SLLZ compression, you can compile via the CLI or edit the batch file to pass `--compression 1` (note: this will take several minutes to run).
* **Game Crashes / Mods Overlapping**: If your mods overlap, check the **GMD CONFLICTS DETECTED** table in the console output. It lists which mods are trying to use the same slot and tells you which mod is currently taking priority.

---

## Credits & Attributions
* This project uses code and libraries from the [Ret-HZ/pxdArchiverCE](https://github.com/Ret-HZ/pxdArchiverCE) project.
