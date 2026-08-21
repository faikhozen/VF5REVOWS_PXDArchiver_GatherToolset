# VF5Revo World Stage Mod Compiler (v2.0)

**Developed by:** Fai Khozen  
**GitHub Repository:** [faikhozen/VF5REVOWS_PXDArchiver_GatherToolset](https://github.com/faikhozen/VF5REVOWS_PXDArchiver_GatherToolset)

---

## 🌟 Overview
A high-performance C# utility tool designed to compile, merge, and organize **Virtua Fighter 5 R.E.V.O. World Stage** mods directly into your game's archives (`chara.par` and `vf5fs_data.par`).

This tool supports three main types of mod compilation:
1. **Standard Character DLC Skin Mods (`chara.par`)**: Merges standalone DLC costume mods into `chara.par`.
2. **Sound & Audio Mods (`vf5fs_data.par`)**: Merges BGM, SFX, and voice lines into `vf5fs_data.par` with automatic extraction of `auth_voice` lines.
3. **Master Skin Cross-Slot Compiler**: Deploys master skin models across character slots with smart breast/chest physics routing for female characters (AOI & SAR) and pre-scan bone weight safety checks.

This compiler operates **completely in-memory**, eliminating the need for extracting archives to disk or copying files to temporary directories. It overlays your mods on top of the original archive in a single pass, saving gigabytes of SSD write wear and completing compiles in seconds.

---

## ✨ Key Features

### 👕 1. Standard Character Skin Mods (`chara.par`)
- **In-Memory PAR Overlay**: Merges skin mod folders into `chara.par` without temporary disk extraction.
- **Built-in GMD Conflict Detection**: Scans and compares skin mod files recursively. If multiple mods try to overwrite the same costume slot, the tool prints a color-coded conflict warning table.

### 🎵 2. Sound Mods (`vf5fs_data.par`) & Voice Extraction
- **Audio Archive Overlay**: Merges BGM, SFX, and ADX2 audio into `vf5fs_data.par`.
- **Automated `auth_voice` Extraction**: Extracts `sound/voice/auth_voice` files to a clean `./output/auth_voice` directory for manual placement, ensuring voice lines play correctly in-game.

### 👗 3. Master Skin Cross-Slot Compiler Mode
- **Master Skins Compiled (`master_skins/compiled/`)**: Compiles pre-packed master skin folders across whitelisted costume slots for all active character slots.
- **Master Skins Mods (`master_skins/mods/`)**: Scans individual mod directories under `master_skins/mods` and automatically deploys master skin `.gmd` models and `.dds` textures across whitelisted costume slots.
- **Smart Chest Physics Routing (AOI & SAR)**: Automatically detects breast/chest physics vertex weights (`j_0_munl_000wj`/`j_0_munr_000wj` for AOI, `j_opal_015wj`/`j_opar_014wj` for SAR) and enforces dedicated placement in `master_skins/special_chest_physics/`.
- **Bone Weight Safety & Interactive Prompt**: Pre-scans all `.gmd` models for vertex weights on non-common dummy bones (e.g. hair, cape, skirt, coat physics), generates `./output/master_skin_bone_error_dump.log`, and provides an interactive prompt letting users choose to **Proceed anyway** or **Abort compilation** to fix weights in Blender.
- **Categorical Vertex Weight Audit Report**: Option 3 Suboption 4 scans all `.gmd` models and logs a categorical breakdown of vertex weights to `./output/master_skin_bone_weight_audit_report.log`.

---

## 📂 Directory Layout

```text
VF5REVOWS_PXDArchiver_GatherToolset/
├── VF5REVOWS_mod_compiler.exe     <-- Main executable
├── mods/                          <-- Standard DLC character skin & sound mods
│   ├── skin_mod_01/ (chara/)
│   └── sound_mod_01/ (rom/)
├── master_skins/                  <-- Master skin cross-slot mods
│   ├── compiled/                  <-- Pre-packed master skin folders (Mode 1)
│   ├── mods/                      <-- Standard non-chest physics master skins (Mode 2)
│   └── special_chest_physics/      <-- Chest physics master skins (AOI & SAR)
└── output/                        <-- Compiled chara.par, vf5fs_data.par & logs
```

---

## 🎮 How to Use

### 1. Standard Character Skin & Sound Mods
1. Place mod folders containing `chara/` (skins) or `rom/` (sounds) inside `./mods/`.
2. Double-click `VF5REVOWS_mod_compiler.exe`.
3. Select **1** for Character Skins (`chara.par`) or **2** for Sound Mods (`vf5fs_data.par`).
4. Select your game's reference `chara.par` or `vf5fs_data.par` when prompted by the file picker.
5. Copy compiled files from `./output/` to your game directory.

### 2. Master Skin Cross-Slot Mods
1. Place master skin mods in `master_skins/mods/` (non-chest physics) or `master_skins/special_chest_physics/` (AOI & SAR chest physics).
2. Double-click `VF5REVOWS_mod_compiler.exe`.
3. Select **3** for Master Skin Cross-Slot Mod Compiler, then pick **1** (`master_skins/compiled`) or **2** (`master_skins/mods`).
4. Select reference `chara.par`.
5. Copy `output/chara.par` to your game's `media/data/` folder.

---

## 📊 AOI & SAR Master Skin Deployment Matrix

| Character | Target Slot / Path | Physics Type | Source Folder |
| :--- | :--- | :--- | :--- |
| **AOI** | `tops/c_v31_VF5_AOI_VF1/c_v31_VF5_AOI_VF1.gmd`<br>`tops/c_v31_VF5_AOI_VF1_2/c_v31_VF5_AOI_VF1_2.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **AOI Items** | `vf5item/AOI/AOIITM003/AOI003_JO_OUT_01.gmd`<br>`vf5item/AOI/AOIITM023/AOI023_JO_OUT_03.gmd`<br>`vf5item/AOI/AOIITM033/AOI033_JO_OUT_04.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **AOI** | `tops/c_v52_VF5_AOI_RYU/c_v52_VF5_AOI_RYU.gmd`<br>`tops/c_v73_VF5_AOI_TEK/c_v73_VF5_AOI_TEK.gmd`<br>`tops/c_v8e_VF5_AOI_SWIM/c_v8e_VF5_AOI_SWIM.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |
| **AOI Items** | `vf5item/AOI/AOIITM13/AOI013_JO_OUT_02.gmd`<br>`vf5item/AOI/AOIITM343/AOI343_JO_OUT_05.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |
| **SAR** | `tops/c_v01_VF5_SAR/c_v01_VF5_SAR.gmd`<br>`tops/c_v22_VF5_SAR_VF1/c_v22_VF5_SAR_VF1.gmd`<br>`tops/c_v43_VF5_SAR_RYU/c_v43_VF5_SAR_RYU.gmd`<br>`tops/c_v85_VF5_SAR_SWIM/c_v85_VF5_SAR_SWIM.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **SAR** | `tops/c_v64_VF5_SAR_TEK/c_v64_VF5_SAR_TEK.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |

---

## 🛠️ Diagnostics & Error Logs

Diagnostic logs are automatically created under `./output/`:
- `master_skin_error_dump.log`: Multiple `.gmd` file conflicts per character code.
- `master_skin_bone_error_dump.log`: Models containing vertex weights on non-common bones.
- `master_skin_chest_physics_error_dump.log`: Chest physics placement folder mismatches.
- `master_skin_bone_weight_audit_report.log`: Full categorical vertex weight audit (Suboption 4).

---

## 📜 Credits & Acknowledgments
- Developed by **Fai Khozen**.
- Uses code and libraries from the [Ret-HZ/pxdArchiverCE](https://github.com/Ret-HZ/pxdArchiverCE) project.
