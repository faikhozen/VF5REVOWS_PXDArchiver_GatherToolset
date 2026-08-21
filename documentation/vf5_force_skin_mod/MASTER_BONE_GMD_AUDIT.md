# VF5 REVO - Roster Character Master Bone Audit (`c_v00` - `c_v20`)

**Source Directory Path:** `I:\_games\_pir\VFREVOBETA_mod\chara.par.unpack\bone`  
**Selection Filter:** Primary Roster Master Bone `.gmd` Files (`c_v00_VF5_*` through `c_v20_VF5_*`)  
**Audit Date:** August 20, 2026  
**Total Selected Files:** `21` master `.gmd` bone rig files  

---

## 📊 Summary Overview

* **Total Selected Bone Files:** `21` files
* **Character Roster Coverage:** `21` sequential character slots (`v00` through `v20`)
* **Roster Inclusion:** All 19 main roster fighters + 1 Developer Test slot (`TST`) + 1 Boss slot (`DURAL`)
* **Exclusions:** Swimsuit variants (`v8a`–`v8f`), legacy files (`v0_AKI`, `v1_MON`), generic mannequin (`c_man_bone`), and binary containers (`c_e42_bone.gma`) excluded per user selection.

---

## 👤 Selected Master Bone Inventory (`v00` - `v20`)

| # | Slot | Code | Full Character Name | File Name | Size (KB) | Size (Bytes) | Description |
| :---: | :---: | :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | `v00` | `AKI` | Akira Yuki | `c_v00_VF5_AKI_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Akira Yuki |
| 2 | `v01` | `SAR` | Sarah Bryant | `c_v01_VF5_SAR_bone.gmd` | `32 KB` | `32768` | Primary master skeletal bone armature for Sarah Bryant |
| 3 | `v02` | `LAU` | Lau Chan | `c_v02_VF5_LAU_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Lau Chan |
| 4 | `v03` | `SHU` | Shun Di | `c_v03_VF5_SHU_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Shun Di |
| 5 | `v04` | `JEF` | Jeffry McWild | `c_v04_VF5_JEF_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Jeffry McWild |
| 6 | `v05` | `PAI` | Pai Chan | `c_v05_VF5_PAI_bone.gmd` | `32 KB` | `32768` | Primary master skeletal bone armature for Pai Chan |
| 7 | `v06` | `JAK` | Jacky Bryant | `c_v06_VF5_JAK_bone.gmd` | `56 KB` | `57344` | Primary master skeletal bone armature for Jacky Bryant |
| 8 | `v07` | `KAG` | Kage-Maru | `c_v07_VF5_KAG_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Kage-Maru |
| 9 | `v08` | `LIO` | Lion Rafale | `c_v08_VF5_LIO_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Lion Rafale |
| 10 | `v09` | `WOL` | Wolf Hawkfield | `c_v09_VF5_WOL_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Wolf Hawkfield |
| 11 | `v10` | `AOI` | Aoi Umenokouji | `c_v10_VF5_AOI_bone.gmd` | `32 KB` | `32768` | Primary master skeletal bone armature for Aoi Umenokouji |
| 12 | `v11` | `LEI` | Lei-Fei | `c_v11_VF5_LEI_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Lei-Fei |
| 13 | `v12` | `VAN` | Vanessa Lewis | `c_v12_VF5_VAN_bone.gmd` | `32 KB` | `32768` | Primary master skeletal bone armature for Vanessa Lewis |
| 14 | `v13` | `BRA` | Brad Burns | `c_v13_VF5_BRA_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Brad Burns |
| 15 | `v14` | `GOH` | Goh Hinogami | `c_v14_VF5_GOH_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Goh Hinogami |
| 16 | `v15` | `MON` | El Blaze | `c_v15_VF5_MON_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for El Blaze |
| 17 | `v16` | `MSK` | Eileen | `c_v16_VF5_MSK_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Eileen |
| 18 | `v17` | `KRT` | Jean Kujo | `c_v17_VF5_KRT_bone.gmd` | `28 KB` | `28672` | Primary master skeletal bone armature for Jean Kujo |
| 19 | `v18` | `TAK` | Taka-Arashi | `c_v18_VF5_TAK_bone.gmd` | `40 KB` | `40960` | Primary master skeletal bone armature for Taka-Arashi (Large sumo frame) |
| 20 | `v19` | `TST` | Test / Dev Slot | `c_v19_VF5_TST_bone.gmd` | `108 KB` | `110592` | Master test skeleton containing extended debugging and physics bone nodes |
| 21 | `v20` | `DUR` | Dural | `c_v20_VF5_DUR_bone.gmd` | `32 KB` | `32768` | Primary master skeletal bone armature for Dural (Boss) |

---

## 🏷️ Bone File Size & Archetype Analysis

| Category Archetype | File Size Range | Included Characters | Modding Characteristics |
| :--- | :---: | :--- | :--- |
| **Standard Medium Male / Female** | `28 KB` | `AKI`, `LAU`, `SHU`, `JEF`, `KAG`, `LIO`, `WOL`, `LEI`, `BRA`, `GOH`, `MON`, `MSK`, `KRT` | Standard 28 KB armature baseline; high skeletal weight interchangeability |
| **Dynamic Female / Medium Heavy** | `32 KB` | `SAR`, `PAI`, `AOI`, `VAN`, `DUR` | 32 KB armature with extra bone chains for hair/skirt/bust dynamic physics |
| **Sumo Heavy Frame** | `40 KB` | `TAK` (Taka-Arashi) | Expanded 40 KB bone node layout for large sumo body volume |
| **High-Node Jacky Rig** | `56 KB` | `JAK` (Jacky Bryant) | 56 KB specialized armature with expanded jacket/zipper/hair physics nodes |
| **Master Developer Rig** | `108 KB` | `TST` (Test Slot) | 108 KB debug skeleton containing complete node hierarchy for all test features |

---

## 🔍 Technical Observations & Force-Skin Modding Insights

1. **Role of Master Bone (`.gmd`) Files:**
   - Master bone files define root transforms, joint hierarchies, bone indices, and default bind poses for each character slot.
   - When loading model meshes from `tops` or `vf5item`, the engine binds vertex weights to the bone indices defined in these `.gmd` master bone files.
2. **Interchangeability & Model Swapping:**
   - Standard 28 KB bone characters (`AKI`, `LAU`, `GOH`, `KRT`, `BRA`, `LIO`, `WOL`, `JEF`, `SHU`, `MON`, `MSK`) feature matching root bone indexing, making forced skin / mesh swaps between them cleaner.
   - Swapping models onto heavy frames like Taka-Arashi (`v18` / 40 KB) or Jacky (`v06` / 56 KB) requires preserving or adjusting bone index weights to avoid mesh stretching.
3. **Developer Debug Skeleton (`c_v19_VF5_TST_bone.gmd`):**
   - Slot `v19` (`TST`) is the largest bone file (108 KB), acting as the master reference skeleton with full debug nodes enabled.
