# VF5Revo World Stage Mod Compiler (v2.0)

**Developed by:** Fai Khozen  
**GitHub Repository:** [faikhozen/VF5REVOWS_PXDArchiver_GatherToolset](https://github.com/faikhozen/VF5REVOWS_PXDArchiver_GatherToolset)

---

## 🌟 Overview
A high-performance C# utility tool designed to compile, merge, and organize **Virtua Fighter 5 R.E.V.O. World Stage** mods directly into your game's archives (`chara.par` and `vf5fs_data.par`).

Featuring **Master Skin Cross-Slot Replacement**, **Automated Breast/Chest Physics Handling for AOI & SAR**, **Non-Common Bone Weight Pre-Scanning**, and **In-Memory Archive Overlay**, this tool streamlines VF5 REVO modding without high SSD write wear or tedious manual file replacements.

---

## ✨ Key Features

### 👗 1. Master Skin Cross-Slot Compiler Mode
- **Master Skins Compiled (`master_skins/compiled/`)**: Compiles pre-packed master skin folders across whitelisted costume slots for all active character slots.
- **Master Skins Mods (`master_skins/mods/`)**: Scans individual mod directories under `master_skins/mods` and automatically deploys master skin `.gmd` models and `.dds` textures across whitelisted costume slots.
- **Whitelisted Common Dummy Bones Guide**: Interactive console guide detailing the 118 Whitelisted Common Dummy Bones and strict rigging rules.
- **Bone Weight Categorical Audit Report**: Scans all `.gmd` models across `master_skins/` directories and logs a categorical breakdown of vertex weights (allowed common dummy bones, allowed physics bones, and non-common violation bones) to `./output/master_skin_bone_weight_audit_report.log`.

### 👙 2. Smart Chest Physics Routing (AOI & SAR)
- **Automatic Physics Weight Detection**: Scans `.gmd` vertex weights for female characters (`AOI` and `SAR`) for breast/chest physics weights (`j_0_munl_000wj`/`j_0_munr_000wj` for AOI, `j_opal_015wj`/`j_opar_014wj` for SAR).
- **Dedicated Special Chest Physics Directory (`master_skins/special_chest_physics/`)**:
  - Chest physics models for AOI and SAR are placed in `master_skins/special_chest_physics/`.
  - Non-chest physics models remain in `master_skins/mods/` or `master_skins/compiled/`.
- **Target Slot & Item Mappings**:
  - **SAR**: Chest physics model targets `c_v64_VF5_SAR_TEK`. Standard non-chest model targets `c_v01_VF5_SAR`, `c_v22_VF5_SAR_VF1`, `c_v43_VF5_SAR_RYU`, `c_v85_VF5_SAR_SWIM`.
  - **AOI**: Chest physics model targets `c_v52_VF5_AOI_RYU`, `c_v73_VF5_AOI_TEK`, `c_v8e_VF5_AOI_SWIM`, and item slots `AOI013_JO_OUT_02.gmd`, `AOI343_JO_OUT_05.gmd`. Non-chest physics model targets `c_v31_VF5_AOI_VF1`, `c_v31_VF5_AOI_VF1_2`, and item slots `AOI003_JO_OUT_01.gmd`, `AOI023_JO_OUT_03.gmd`, `AOI033_JO_OUT_04.gmd`. Base `c_v10_VF5_AOI.gmd` is excluded.

### 🛡️ 3. Bone Weight Safety & Warning System
- **Pre-Scan Validation**: Pre-scans all `.gmd` models for vertex weights on non-common dummy bones (e.g. hair, cape, skirt, or coat physics).
- **Diagnostic Dump Logging**: Automatically generates detailed error/warning logs (`master_skin_bone_error_dump.log` or `master_skin_chest_physics_error_dump.log`) under `./output/`.
- **Interactive Warning Prompt**: Warns users of potential in-game visual mesh distortion when loading non-common weights on dummy armatures and lets users choose to **Proceed anyway** or **Abort compilation** to fix weights in Blender.

### 🚀 4. Performance & Convenience
- **In-Memory PAR Overlay**: Compiles modded PAR archives in a single pass without un-archiving gigabytes of files to temporary folders on disk.
- **Sound Mod Support (`vf5fs_data.par`)**: Compiles sound, BGM, and SFX mods with automatic extraction of `auth_voice` lines to `./output/auth_voice`.
- **Guided Windows Interactive Wizard**: Double-click execution with file pickers, color-coded summaries, and progress bars.

---

## 📂 Master Skin Folder Structure & Mappings

### Directory Layout
```text
VF5REVOWS_PXDArchiver_GatherToolset/
├── VF5REVOWS_mod_compiler.exe
├── master_skins/
│   ├── compiled/                   <-- Mode 1: Pre-packed master skin folders
│   │   └── chara/tops/
│   │       ├── c_v01_VF5_AKI/
│   │       └── ...
│   ├── mods/                       <-- Mode 2: Standard non-chest physics mods
│   │   ├── my_aoi_mod/
│   │   └── ...
│   └── special_chest_physics/       <-- Chest physics mods (AOI & SAR with breast weights)
│       ├── aoi_swimsuit_mod/
│       └── sar_tekken_mod/
├── mods/                           <-- Standard DLC character skin & sound mods
└── output/                         <-- Compiled chara.par, vf5fs_data.par & diagnostic logs
```

### AOI & SAR Master Skin Deployment Matrix

| Character | Target Slot / Path | Physics Type | Source Location |
| :--- | :--- | :--- | :--- |
| **AOI** | `tops/c_v31_VF5_AOI_VF1/c_v31_VF5_AOI_VF1.gmd`<br>`tops/c_v31_VF5_AOI_VF1_2/c_v31_VF5_AOI_VF1_2.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **AOI Items** | `vf5item/AOI/AOIITM003/AOI003_JO_OUT_01.gmd`<br>`vf5item/AOI/AOIITM023/AOI023_JO_OUT_03.gmd`<br>`vf5item/AOI/AOIITM033/AOI033_JO_OUT_04.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **AOI** | `tops/c_v52_VF5_AOI_RYU/c_v52_VF5_AOI_RYU.gmd`<br>`tops/c_v73_VF5_AOI_TEK/c_v73_VF5_AOI_TEK.gmd`<br>`tops/c_v8e_VF5_AOI_SWIM/c_v8e_VF5_AOI_SWIM.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |
| **AOI Items** | `vf5item/AOI/AOIITM13/AOI013_JO_OUT_02.gmd`<br>`vf5item/AOI/AOIITM343/AOI343_JO_OUT_05.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |
| **SAR** | `tops/c_v01_VF5_SAR/c_v01_VF5_SAR.gmd`<br>`tops/c_v22_VF5_SAR_VF1/c_v22_VF5_SAR_VF1.gmd`<br>`tops/c_v43_VF5_SAR_RYU/c_v43_VF5_SAR_RYU.gmd`<br>`tops/c_v85_VF5_SAR_SWIM/c_v85_VF5_SAR_SWIM.gmd` | Non-Chest Physics | `master_skins/mods/` |
| **SAR** | `tops/c_v64_VF5_SAR_TEK/c_v64_VF5_SAR_TEK.gmd` | **Chest Physics** | `master_skins/special_chest_physics/` |

---

## 🎮 How to Use

### Step 1: Run the Executable
Double-click `VF5REVOWS_mod_compiler.exe`.

### Step 2: Main Menu Selection
Choose your desired compilation mode:
1. **Compile Character Skins (`chara.par`)** — Standard DLC slot costume overlay.
2. **Compile Sound Mod (`vf5fs_data.par`)** — BGM, SFX, and voice mod overlay.
3. **Master Skin Cross-Slot Mod Compiler** — Deploy master skins across character slots.

### Step 3: Master Skin Suboptions (Main Option 3)
If you select Option 3:
- **Suboption 1 (`master_skins/compiled`)**: Scans `master_skins/compiled`.
- **Suboption 2 (`master_skins/mods`)**: Scans individual mod folders in `master_skins/mods`.
- **Suboption 3 (`Whitelisted Common Dummy Bones Guide`)**: Displays rigging guidelines.
- **Suboption 4 (`Bone Weight Categorical Audit Report`)**: Audits vertex weights across all models and logs to `./output/master_skin_bone_weight_audit_report.log`.

### Step 4: Reference PAR Selection
When prompted, select your game's reference PAR:
- **Skins**: `{Steam}\steamapps\common\VFREVO\runtime\media\data\chara.par`
- **Sounds**: `{Steam}\steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_data.par`

### Step 5: Output & Deployment
Compiled PAR files will be saved in `./output/`:
- Copy `output/chara.par` to `{Steam}\steamapps\common\VFREVO\runtime\media\data\`.
- Copy `output/vf5fs_data.par` to `{Steam}\steamapps\common\VFREVO\runtime\media\vf5fs\`.
- *(For sound mods with `auth_voice`)*: Copy files from `output/auth_voice/` into `{Steam}\steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_media\rom\sound\voice\auth_voice\`.

---

## 🛠️ Diagnostics & Error Logs

All diagnostic dump logs are automatically generated under `./output/`:
- `master_skin_error_dump.log`: Multiple `.gmd` file conflicts per character code.
- `master_skin_bone_error_dump.log`: Models containing vertex weights on non-common bones.
- `master_skin_chest_physics_error_dump.log`: Chest physics placement folder mismatches.
- `master_skin_bone_weight_audit_report.log`: Full categorical vertex weight audit (Suboption 4).

---

## 📜 Credits & Acknowledgments
- Developed by **Fai Khozen**.
- Uses code and libraries from the [Ret-HZ/pxdArchiverCE](https://github.com/Ret-HZ/pxdArchiverCE) project.
