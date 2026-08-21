import os, sys, json
import numpy as np

scratch_dir = r"C:\Users\casan\.gemini\antigravity-ide\brain\8f359a7f-d372-4b02-87f4-fb708f856538\scratch"
if scratch_dir not in sys.path: sys.path.insert(0, scratch_dir)

repo_dir = r"c:\Users\casan\Documents\github\VF5REVOWS_PXDArchiver_GatherToolset"
if repo_dir not in sys.path: sys.path.insert(0, repo_dir)

from yk_gmd_blender.gmdlib.io import read_gmd_structures, read_abstract_scene_from_filedata_object
from yk_gmd_blender.gmdlib.errors.error_reporter import LenientErrorReporter
from yk_gmd_blender.gmdlib.converters.common.to_abstract import FileImportMode, VertexImportMode
from yk_gmd_blender.gmdlib.abstract.gmd_mesh import GMDSkinnedMesh
from yk_gmd_blender.gmdlib.abstract.nodes.gmd_object import GMDSkinnedObject, GMDUnskinnedObject

error_reporter = LenientErrorReporter(set())

common_bones = {
  'buki1_c_n', 'buki2_c_n', 'c_kata_l', 'c_kata_r', 'center_c_n', 'cl_kao', 'cl_momo_l', 'cl_momo_r',
  'cl_mune', 'face_c_n', 'j_kao_wj', 'j_kata_l_wj_cu', 'j_kata_r_wj_cu', 'j_momo_l_wj', 'j_momo_r_wj',
  'j_mune_wj', 'j_sune_l_wj', 'j_sune_r_wj', 'j_ude_l_wj', 'j_ude_r_wj', 'kl_ago_wj', 'kl_asi_l_wj_co', 'kl_asi_r_wj_co',
  'kl_eye_l_wj', 'kl_eye_r_wj', 'kl_kosi_etc_wj', 'kl_mabu_d_l_wj', 'kl_mabu_d_r_wj', 'kl_mabu_l_wj',
  'kl_mabu_r_wj', 'kl_mabu_u_l_wj', 'kl_mabu_u_r_wj', 'kl_mune_b_wj', 'kl_te_l_wj', 'kl_te_r_wj',
  'kl_toe_l_wj', 'kl_toe_r_wj', 'kl_waki_l_wj', 'kl_waki_r_wj', 'mesh', 'n_hara_b_wj_ex', 'n_hara_c_wj_ex',
  'n_hiji_l_wj_ex', 'n_hiji_r_wj_ex', 'n_hiza_l_wj_ex', 'n_hiza_r_wj_ex', 'n_kubi_wj_ex', 'n_momo_b_l_wj_ex',
  'n_momo_b_r_wj_ex', 'n_momo_c_l_wj_ex', 'n_momo_c_r_wj_ex', 'n_skata_b_l_wj_cd_cu_ex', 'n_skata_b_r_wj_cd_cu_ex',
  'n_skata_c_l_wj_cd_cu_ex', 'n_skata_c_r_wj_cd_cu_ex', 'n_skata_l_wj_cd_ex', 'n_skata_r_wj_cd_ex',
  'n_ste_l_wj_ex', 'n_ste_r_wj_ex', 'n_sude_b_l_wj_ex', 'n_sude_b_r_wj_ex', 'n_sude_l_wj_ex', 'n_sude_r_wj_ex',
  'nl_hito_b_l_wj', 'nl_hito_b_r_wj', 'nl_hito_c_l_wj', 'nl_hito_c_r_wj', 'nl_hito_l_wj', 'nl_hito_r_wj',
  'nl_ko_b_l_wj', 'nl_ko_b_r_wj', 'nl_ko_c_l_wj', 'nl_ko_c_r_wj', 'nl_ko_l_wj', 'nl_ko_r_wj',
  'nl_kusu_b_l_wj', 'nl_kusu_b_r_wj', 'nl_kusu_c_l_wj', 'nl_kusu_c_r_wj', 'nl_kusu_l_wj', 'nl_kusu_r_wj',
  'nl_naka_b_l_wj', 'nl_naka_b_r_wj', 'nl_naka_c_l_wj', 'nl_naka_c_r_wj', 'nl_naka_l_wj', 'nl_naka_r_wj',
  'nl_oya_b_l_wj', 'nl_oya_b_r_wj', 'nl_oya_c_l_wj', 'nl_oya_c_r_wj', 'nl_oya_l_wj', 'nl_oya_r_wj',
  'pattern_c_n', 'sync_c_n', 'vector_c_n',
  'tl_ago_wj', 'tl_ha_wj', 'tl_hoho_b_l_wj', 'tl_hoho_b_r_wj', 'tl_hoho_c_l_wj', 'tl_hoho_c_r_wj', 'tl_hoho_l_wj', 'tl_hoho_r_wj',
  'tl_kuti_d_l_wj', 'tl_kuti_d_r_wj', 'tl_kuti_d_wj', 'tl_kuti_l_wj', 'tl_kuti_r_wj', 'tl_kuti_u_l_wj', 'tl_kuti_u_r_wj', 'tl_kuti_u_wj',
  'tl_mayu_b_l_wj', 'tl_mayu_b_r_wj', 'tl_mayu_c_l_wj', 'tl_mayu_c_r_wj', 'tl_mayu_l_wj', 'tl_mayu_r_wj'
}

character_allowed_bones = {
  'AOI': {'j_0_munl_000wj', 'j_0_munr_000wj'},
  'SAR': {'j_opal_015wj', 'j_opar_014wj'},
  'PAI': {'j_opal_050wj', 'j_opar_051wj'},
  'VAN': {'j_opal_058wj', 'j_opar_059wj'},
  'DUR': {'j_opal_058wj', 'j_opar_059wj'},
}

def match_character_code(path):
    path_upper = path.upper()
    codes = ['AKI', 'SAR', 'LAU', 'SHU', 'JEF', 'PAI', 'JAK', 'KAG', 'LIO', 'WOL', 'AOI', 'LEI', 'VAN', 'BRA', 'GOH', 'MON', 'MSK', 'KRT', 'TAK', 'TST', 'DUR']
    parts = [p for p in path_upper.replace('/', '\\').split('\\') if p]
    for part in parts:
        for code in codes:
            if f'_{code}_' in part or f'_{code}.' in part or part.endswith(f'_{code}') or part.startswith(f'{code}_'):
                return code
            if part.startswith(code):
                rest = part[len(code):]
                if not rest or not rest[0].isalpha():
                    return code
    return None

def scan_single_gmd(gmd_path):
    try:
        version_props, header, contents = read_gmd_structures(gmd_path, error_reporter)
        scene = read_abstract_scene_from_filedata_object(
            version_props, FileImportMode.SKINNED, VertexImportMode.IMPORT_VERTICES, contents, error_reporter
        )
        weighted_bones = set()
        for node in scene.overall_hierarchy:
            if isinstance(node, (GMDSkinnedObject, GMDUnskinnedObject)):
                for mesh in node.mesh_list:
                    if isinstance(mesh, GMDSkinnedMesh) and mesh.vertices_data.weight_data is not None:
                        weights = mesh.vertices_data.weight_data
                        bone_indices = mesh.vertices_data.bone_data.astype(int)
                        mask = weights > 0
                        active = set(int(x) for x in np.unique(bone_indices[mask]).flatten())
                        active.discard(-1)
                        for idx in active:
                            if 0 <= idx < len(mesh.relevant_bones):
                                weighted_bones.add(mesh.relevant_bones[idx].name)
        char_code = match_character_code(gmd_path)
        allowed_bones = character_allowed_bones.get(char_code, set()) if char_code else set()
        non_common = sorted(list(weighted_bones - common_bones - allowed_bones))
        return {
            "path": gmd_path,
            "filename": os.path.basename(gmd_path),
            "all_weighted_bones": sorted(list(weighted_bones)),
            "non_common_bones": non_common
        }
    except Exception as ex:
        return {"path": gmd_path, "filename": os.path.basename(gmd_path), "error": str(ex), "all_weighted_bones": [], "non_common_bones": []}

def scan_path(target_path):
    results = []
    if os.path.isdir(target_path):
        for root, dirs, files in os.walk(target_path):
            for f in files:
                if f.endswith('.gmd'):
                    gmd_p = os.path.join(root, f)
                    res = scan_single_gmd(gmd_p)
                    results.append(res)
    elif os.path.isfile(target_path):
        results.append(scan_single_gmd(target_path))
    return results

if __name__ == "__main__":
    if len(sys.argv) > 1:
        out = scan_path(sys.argv[1])
        print(json.dumps(out, indent=2))

