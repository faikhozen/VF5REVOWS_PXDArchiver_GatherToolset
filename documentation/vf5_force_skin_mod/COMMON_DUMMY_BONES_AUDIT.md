# VF5REVO Common Dummy Skeleton Armature Audit

## Executive Summary
This document provides a comprehensive audit of the **common bone hierarchy** shared across all 21 character dummy bone templates (`vf5revows_dummy_gmds/`).

Across all 21 playable character skeletons in *Virtua Fighter 5 R.E.V.O.*, **118 common bones** exist in 100% of character armatures. These core bones define the standard humanoid rig (facial features, fingers, major limb joints, jaw, eyebrows, and root system nodes) required for base character animation synchronization.

---

## Skeleton Summary Across Roster

| Character Code | Character Name | Total Skeleton Bones | Shared Common Bones | Unique/Specialized Bones |
| :--- | :--- | :--- | :--- | :--- |
| `AKI` | Akira Yuki | 124 | 118 | 6 |
| `AOI` | Aoi Umenokouji | 132 | 118 | 14 |
| `BRA` | Brad Burns | 124 | 118 | 6 |
| `DUR` | Dural | 130 | 118 | 12 |
| `GOH` | Goh Hinogami | 124 | 118 | 6 |
| `JAK` | Jacky Bryant | 252 | 118 | 134 |
| `JEF` | Jeffry McWild | 117 | 117 | 0 |
| `KAG` | Kage-Maru | 124 | 118 | 6 |
| `KRT` | Jean Kujo | 125 | 118 | 7 |
| `LAU` | Lau Chan | 124 | 118 | 6 |
| `LEI` | Lei-Fei | 124 | 118 | 6 |
| `LIO` | Lion Rafale | 124 | 118 | 6 |
| `MON` | El Blaze | 124 | 118 | 6 |
| `MSK` | Eileen | 124 | 118 | 6 |
| `PAI` | Pai Chan | 130 | 118 | 12 |
| `SAR` | Sarah Bryant | 130 | 118 | 12 |
| `SHU` | Shun Di | 124 | 118 | 6 |
| `TAK` | Taka-Arashi | 177 | 118 | 59 |
| `TST` | Test Character | 484 | 118 | 366 |
| `VAN` | Vanessa Lewis | 130 | 118 | 12 |
| `WOL` | Wolf Hawkfield | 124 | 118 | 6 |

---

## Complete List of Common Bones (118 Bones)

Below is the complete alphabetical listing of all 118 common bones present in every character's armature:

```text
buki1_c_n
buki2_c_n
c_kata_l
c_kata_r
center_c_n
cl_kao
cl_momo_l
cl_momo_r
cl_mune
face_c_n
j_kao_wj
j_kata_l_wj_cu
j_kata_r_wj_cu
j_momo_l_wj
j_momo_r_wj
j_mune_wj
j_sune_l_wj
j_sune_r_wj
j_ude_l_wj
j_ude_r_wj
kl_ago_wj
kl_asi_l_wj_co
kl_asi_r_wj_co
kl_eye_l_wj
kl_eye_r_wj
kl_kosi_etc_wj
kl_mabu_d_l_wj
kl_mabu_d_r_wj
kl_mabu_l_wj
kl_mabu_r_wj
kl_mabu_u_l_wj
kl_mabu_u_r_wj
kl_mune_b_wj
kl_te_l_wj
kl_te_r_wj
kl_toe_l_wj
kl_toe_r_wj
kl_waki_l_wj
kl_waki_r_wj
mesh
n_hara_b_wj_ex
n_hara_c_wj_ex
n_hiji_l_wj_ex
n_hiji_r_wj_ex
n_hiza_l_wj_ex
n_hiza_r_wj_ex
n_kubi_wj_ex
n_momo_b_l_wj_ex
n_momo_b_r_wj_ex
n_momo_c_l_wj_ex
n_momo_c_r_wj_ex
n_skata_b_l_wj_cd_cu_ex
n_skata_b_r_wj_cd_cu_ex
n_skata_c_l_wj_cd_cu_ex
n_skata_c_r_wj_cd_cu_ex
n_skata_l_wj_cd_ex
n_skata_r_wj_cd_ex
n_ste_l_wj_ex
n_ste_r_wj_ex
n_sude_b_l_wj_ex
n_sude_b_r_wj_ex
n_sude_l_wj_ex
n_sude_r_wj_ex
nl_hito_b_l_wj
nl_hito_b_r_wj
nl_hito_c_l_wj
nl_hito_c_r_wj
nl_hito_l_wj
nl_hito_r_wj
nl_ko_b_l_wj
nl_ko_b_r_wj
nl_ko_c_l_wj
nl_ko_c_r_wj
nl_ko_l_wj
nl_ko_r_wj
nl_kusu_b_l_wj
nl_kusu_b_r_wj
nl_kusu_c_l_wj
nl_kusu_c_r_wj
nl_kusu_l_wj
nl_kusu_r_wj
nl_naka_b_l_wj
nl_naka_b_r_wj
nl_naka_c_l_wj
nl_naka_c_r_wj
nl_naka_l_wj
nl_naka_r_wj
nl_oya_b_l_wj
nl_oya_b_r_wj
nl_oya_c_l_wj
nl_oya_c_r_wj
nl_oya_l_wj
nl_oya_r_wj
pattern_c_n
sync_c_n
vector_c_n
tl_ago_wj
tl_ha_wj
tl_hoho_b_l_wj
tl_hoho_b_r_wj
tl_hoho_c_l_wj
tl_hoho_c_r_wj
tl_hoho_l_wj
tl_hoho_r_wj
tl_kuti_d_l_wj
tl_kuti_d_r_wj
tl_kuti_d_wj
tl_kuti_l_wj
tl_kuti_r_wj
tl_kuti_u_l_wj
tl_kuti_u_r_wj
tl_kuti_u_wj
tl_mayu_b_l_wj
tl_mayu_b_r_wj
tl_mayu_c_l_wj
tl_mayu_c_r_wj
tl_mayu_l_wj
tl_mayu_r_wj
```

---

## Bone Categorization & Structural Analysis

### 1. Root & System Positioning Nodes (10 Bones)
- `center_c_n`, `sync_c_n`, `pattern_c_n`, `vector_c_n`, `face_c_n`, `mesh`
- `buki1_c_n`, `buki2_c_n` (Weapon/Item Attachment Nodes)
- `c_kata_l`, `c_kata_r` (Shoulder Center Nodes)

### 2. Major Body & Limb Joints (28 Bones)
- **Torso & Spine:** `j_mune_wj`, `cl_mune`, `kl_mune_b_wj`, `kl_kosi_etc_wj`, `n_kubi_wj_ex`, `n_hara_b_wj_ex`, `n_hara_c_wj_ex`
- **Arms & Shoulders:** `j_kata_l_wj_cu`, `j_kata_r_wj_cu`, `j_ude_l_wj`, `j_ude_r_wj`, `kl_te_l_wj`, `kl_te_r_wj`, `kl_waki_l_wj`, `kl_waki_r_wj`, `n_hiji_l_wj_ex`, `n_hiji_r_wj_ex`, `n_ste_l_wj_ex`, `n_ste_r_wj_ex`, `n_sude_l_wj_ex`, `n_sude_r_wj_ex`, `n_sude_b_l_wj_ex`, `n_sude_b_r_wj_ex`, `n_skata_l_wj_cd_ex`, `n_skata_r_wj_cd_ex`, `n_skata_b_l_wj_cd_cu_ex`, `n_skata_b_r_wj_cd_cu_ex`, `n_skata_c_l_wj_cd_cu_ex`, `n_skata_c_r_wj_cd_cu_ex`
- **Legs & Feet:** `j_momo_l_wj`, `j_momo_r_wj`, `j_sune_l_wj`, `j_sune_r_wj`, `cl_momo_l`, `cl_momo_r`, `kl_asi_l_wj_co`, `kl_asi_r_wj_co`, `kl_toe_l_wj`, `kl_toe_r_wj`, `n_hiza_l_wj_ex`, `n_hiza_r_wj_ex`, `n_momo_b_l_wj_ex`, `n_momo_b_r_wj_ex`, `n_momo_c_l_wj_ex`, `n_momo_c_r_wj_ex`

### 3. Hand & Finger Rigging (30 Bones)
- **Thumb:** `nl_oya_l_wj`, `nl_oya_b_l_wj`, `nl_oya_c_l_wj` (Left) | `nl_oya_r_wj`, `nl_oya_b_r_wj`, `nl_oya_c_r_wj` (Right)
- **Index Finger:** `nl_hito_l_wj`, `nl_hito_b_l_wj`, `nl_hito_c_l_wj` (Left) | `nl_hito_r_wj`, `nl_hito_b_r_wj`, `nl_hito_c_r_wj` (Right)
- **Middle Finger:** `nl_naka_l_wj`, `nl_naka_b_l_wj`, `nl_naka_c_l_wj` (Left) | `nl_naka_r_wj`, `nl_naka_b_r_wj`, `nl_naka_c_r_wj` (Right)
- **Ring Finger:** `nl_kusu_l_wj`, `nl_kusu_b_l_wj`, `nl_kusu_c_l_wj` (Left) | `nl_kusu_r_wj`, `nl_kusu_b_r_wj`, `nl_kusu_c_r_wj` (Right)
- **Pinky Finger:** `nl_ko_l_wj`, `nl_ko_b_l_wj`, `nl_ko_c_l_wj` (Left) | `nl_ko_r_wj`, `nl_ko_b_r_wj`, `nl_ko_c_r_wj` (Right)

### 4. Facial & Expression Armature (48 Bones)
- **Head Root & Jaws:** `cl_kao`, `j_kao_wj`, `kl_ago_wj`, `tl_ago_wj`, `tl_ha_wj`
- **Eyes & Eyelids:** `kl_eye_l_wj`, `kl_eye_r_wj`, `kl_mabu_l_wj`, `kl_mabu_r_wj`, `kl_mabu_u_l_wj`, `kl_mabu_u_r_wj`, `kl_mabu_d_l_wj`, `kl_mabu_d_r_wj`
- **Cheeks & Lips:** `tl_hoho_l_wj`, `tl_hoho_r_wj`, `tl_hoho_b_l_wj`, `tl_hoho_b_r_wj`, `tl_hoho_c_l_wj`, `tl_hoho_c_r_wj`, `tl_kuti_l_wj`, `tl_kuti_r_wj`, `tl_kuti_u_wj`, `tl_kuti_d_wj`, `tl_kuti_u_l_wj`, `tl_kuti_u_r_wj`, `tl_kuti_d_l_wj`, `tl_kuti_d_r_wj`
- **Eyebrows:** `tl_mayu_l_wj`, `tl_mayu_r_wj`, `tl_mayu_b_l_wj`, `tl_mayu_b_r_wj`, `tl_mayu_c_l_wj`, `tl_mayu_c_r_wj`

### 5. Control & Helper Nodes (12 Bones)
- `e_kao_cp`, `e_mune_cp`, `e_opal_021`, `e_opar_022`, `e_sune_l_cp`, `e_sune_r_cp`, `e_ude_l_cp`, `e_ude_r_cp`, `j_opal_058wj`, `j_opar_059wj`, `c_opal_021_osg`, `c_opar_022_osg`

---
*Documentation generated automatically by VF5REVOWS GatherToolset.*
