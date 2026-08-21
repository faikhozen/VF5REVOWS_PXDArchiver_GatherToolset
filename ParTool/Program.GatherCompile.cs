// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace ParTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ParLibrary.Converter;
    using Yarhl.FileSystem;

    /// <summary>
    /// Gather and Compile functionality.
    /// </summary>
    internal static partial class Program
    {
        private class GmdEntry
        {
            public string FilePath { get; set; }
            public string ModName { get; set; }
        }

        private static void GatherCompile(Options.GatherCompile opts)
        {
            RunGatherCompileInternal(opts, skipBackup: false);
        }

                private static readonly HashSet<string> WhitelistedCommonBones = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "buki1_c_n", "buki2_c_n", "c_kata_l", "c_kata_r", "center_c_n", "cl_kao", "cl_momo_l", "cl_momo_r",
            "cl_mune", "face_c_n", "j_kao_wj", "j_kata_l_wj_cu", "j_kata_r_wj_cu", "j_momo_l_wj", "j_momo_r_wj",
            "j_mune_wj", "j_sune_l_wj", "j_sune_r_wj", "j_ude_l_wj", "j_ude_r_wj", "kl_ago_wj", "kl_asi_l_wj_co", "kl_asi_r_wj_co",
            "kl_eye_l_wj", "kl_eye_r_wj", "kl_kosi_etc_wj", "kl_mabu_d_l_wj", "kl_mabu_d_r_wj", "kl_mabu_l_wj",
            "kl_mabu_r_wj", "kl_mabu_u_l_wj", "kl_mabu_u_r_wj", "kl_mune_b_wj", "kl_te_l_wj", "kl_te_r_wj",
            "kl_toe_l_wj", "kl_toe_r_wj", "kl_waki_l_wj", "kl_waki_r_wj", "mesh", "n_hara_b_wj_ex", "n_hara_c_wj_ex",
            "n_hiji_l_wj_ex", "n_hiji_r_wj_ex", "n_hiza_l_wj_ex", "n_hiza_r_wj_ex", "n_kubi_wj_ex", "n_momo_b_l_wj_ex",
            "n_momo_b_r_wj_ex", "n_momo_c_l_wj_ex", "n_momo_c_r_wj_ex", "n_skata_b_l_wj_cd_cu_ex", "n_skata_b_r_wj_cd_cu_ex",
            "n_skata_c_l_wj_cd_cu_ex", "n_skata_c_r_wj_cd_cu_ex", "n_skata_l_wj_cd_ex", "n_skata_r_wj_cd_ex",
            "n_ste_l_wj_ex", "n_ste_r_wj_ex", "n_sude_b_l_wj_ex", "n_sude_b_r_wj_ex", "n_sude_l_wj_ex", "n_sude_r_wj_ex",
            "nl_hito_b_l_wj", "nl_hito_b_r_wj", "nl_hito_c_l_wj", "nl_hito_c_r_wj", "nl_hito_l_wj", "nl_hito_r_wj",
            "nl_ko_b_l_wj", "nl_ko_b_r_wj", "nl_ko_c_l_wj", "nl_ko_c_r_wj", "nl_ko_l_wj", "nl_ko_r_wj",
            "nl_kusu_b_l_wj", "nl_kusu_b_r_wj", "nl_kusu_c_l_wj", "nl_kusu_c_r_wj", "nl_kusu_l_wj", "nl_kusu_r_wj",
            "nl_naka_b_l_wj", "nl_naka_b_r_wj", "nl_naka_c_l_wj", "nl_naka_c_r_wj", "nl_naka_l_wj", "nl_naka_r_wj",
            "nl_oya_b_l_wj", "nl_oya_b_r_wj", "nl_oya_c_l_wj", "nl_oya_c_r_wj", "nl_oya_l_wj", "nl_oya_r_wj",
            "pattern_c_n", "sync_c_n", "vector_c_n",
            "tl_ago_wj", "tl_ha_wj", "tl_hoho_b_l_wj", "tl_hoho_b_r_wj", "tl_hoho_c_l_wj", "tl_hoho_c_r_wj", "tl_hoho_l_wj", "tl_hoho_r_wj",
            "tl_kuti_d_l_wj", "tl_kuti_d_r_wj", "tl_kuti_d_wj", "tl_kuti_l_wj", "tl_kuti_r_wj", "tl_kuti_u_l_wj", "tl_kuti_u_r_wj", "tl_kuti_u_wj",
            "tl_mayu_b_l_wj", "tl_mayu_b_r_wj", "tl_mayu_c_l_wj", "tl_mayu_c_r_wj", "tl_mayu_l_wj", "tl_mayu_r_wj"
        };

        private static readonly Dictionary<string, HashSet<string>> CharacterSpecificAllowedBones = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "AOI", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_0_munl_000wj", "j_0_munr_000wj" } },
            { "SAR", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_opal_015wj", "j_opar_014wj" } },
            { "PAI", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_opal_050wj", "j_opar_051wj" } },
            { "VAN", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_opal_058wj", "j_opar_059wj" } },
            { "DUR", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_opal_058wj", "j_opar_059wj" } }
        };

                                                private static void ExtractEmbeddedScannerToTemp(string tempDir)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                string prefixYk = "ParTool.EmbeddedYkGmdBlender.";
                foreach (string resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName.StartsWith(prefixYk, StringComparison.OrdinalIgnoreCase))
                    {
                        string relativePath = resourceName.Substring(prefixYk.Length);
                        string diskRelativePath = relativePath.Replace('.', Path.DirectorySeparatorChar);
                        int lastDot = diskRelativePath.LastIndexOf(Path.DirectorySeparatorChar);
                        if (lastDot >= 0)
                        {
                            diskRelativePath = diskRelativePath.Substring(0, lastDot) + ".py";
                        }

                        string targetPath = Path.Combine(tempDir, "yk_gmd_blender", diskRelativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                        if (!File.Exists(targetPath))
                        {
                            using var stream = assembly.GetManifestResourceStream(resourceName);
                            if (stream != null)
                            {
                                using var fileStream = File.Create(targetPath);
                                stream.CopyTo(fileStream);
                            }
                        }
                    }
                }

                string prefixScanner = "ParTool.EmbeddedScanner.";
                foreach (string resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName.StartsWith(prefixScanner, StringComparison.OrdinalIgnoreCase))
                    {
                        string filename = resourceName.Substring(prefixScanner.Length);
                        string targetPath = Path.Combine(tempDir, filename);
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                        using var stream = assembly.GetManifestResourceStream(resourceName);
                        if (stream != null)
                        {
                            using var fileStream = File.Create(targetPath);
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch { }
        }

        private static bool IsInSpecialChestPhysicsFolder(string filePath, string masterSkinsDir)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            string relPath = (!string.IsNullOrEmpty(masterSkinsDir) && filePath.StartsWith(masterSkinsDir, StringComparison.OrdinalIgnoreCase))
                ? filePath.Substring(masterSkinsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : filePath;

            string[] parts = relPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                if (string.Equals(part, "special_chest_physics", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static (Dictionary<string, List<string>> InvalidGmdsMap, Dictionary<string, List<string>> AllWeightedBonesMap) CheckAllMasterSkinNonCommonBones(string targetPath)
        {
            var invalidGmdsMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var allWeightedBonesMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(targetPath) && !File.Exists(targetPath)) return (invalidGmdsMap, allWeightedBonesMap);

            try
            {
                string tempScanDir = Path.Combine(Path.GetTempPath(), "VF5REVOWS_gmd_scanner");
                Directory.CreateDirectory(tempScanDir);
                ExtractEmbeddedScannerToTemp(tempScanDir);

                string scannerPy = Path.Combine(tempScanDir, "scan_gmd_weights.py");

                if (File.Exists(scannerPy))
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{scannerPy}\" \"{targetPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var proc = System.Diagnostics.Process.Start(psi);
                    if (proc != null)
                    {
                        string jsonOutput = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit();
                        if (proc.ExitCode == 0 && !string.IsNullOrWhiteSpace(jsonOutput))
                        {
                            using var doc = System.Text.Json.JsonDocument.Parse(jsonOutput);
                            var root = doc.RootElement;
                            if (root.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                foreach (var elem in root.EnumerateArray())
                                {
                                    string itemPath = elem.TryGetProperty("path", out var pElem) ? (pElem.GetString() ?? "") : "";
                                    string charCode = MatchCharacterCode(itemPath);

                                    if (elem.TryGetProperty("all_weighted_bones", out var allBonesElem))
                                    {
                                        var allList = new List<string>();
                                        foreach (var b in allBonesElem.EnumerateArray())
                                        {
                                            allList.Add(b.GetString() ?? "");
                                        }
                                        if (!string.IsNullOrEmpty(itemPath))
                                        {
                                            allWeightedBonesMap[itemPath] = allList;
                                        }
                                    }

                                    if (elem.TryGetProperty("non_common_bones", out var bonesElem))
                                    {
                                        var boneList = new List<string>();
                                        foreach (var b in bonesElem.EnumerateArray())
                                        {
                                            string boneName = b.GetString() ?? "";
                                            if (string.IsNullOrEmpty(boneName)) continue;

                                            // Exclude experimental breast physics bones for specific female characters
                                            if (!string.IsNullOrEmpty(charCode) &&
                                                CharacterSpecificAllowedBones.TryGetValue(charCode, out var allowedBones) &&
                                                allowedBones.Contains(boneName))
                                            {
                                                continue;
                                            }

                                            boneList.Add(boneName);
                                        }
                                        if (boneList.Count > 0 && !string.IsNullOrEmpty(itemPath))
                                        {
                                            invalidGmdsMap[itemPath] = boneList;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return (invalidGmdsMap, allWeightedBonesMap);
        }

        private static bool RunChestPhysicsTest(
            int suboption,
            string masterSkinsDir,
            Dictionary<string, List<string>> charToGmdFiles,
            Dictionary<string, List<string>> scanAllWeightedMap)
        {
            var chestPhysicsTestResults = new List<string>();
            var chestLocationViolations = new List<string>();
            var chestDumpLogBuilder = new System.Text.StringBuilder();

            chestDumpLogBuilder.AppendLine("================================================================================");
            chestDumpLogBuilder.AppendLine("VF5REVOWS MOD COMPILATION - CHEST PHYSICS LOCATION ERROR DUMP LOG");
            chestDumpLogBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            chestDumpLogBuilder.AppendLine("================================================================================\n");

            var aoiChestBones = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_0_munl_000wj", "j_0_munr_000wj" };
            var sarChestBones = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "j_opal_015wj", "j_opar_014wj" };

            var aoiKnownNoPhysics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AOI003_JO_OUT_01.gmd", "AOI023_JO_OUT_03.gmd", "AOI033_JO_OUT_04.gmd"
            };

            var aoiKnownHasPhysics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "c_v8e_VF5_AOI_SWIM.gmd", "c_v73_VF5_AOI_TEK.gmd", "AOI013_JO_OUT_02.gmd", "AOI343_JO_OUT_05.gmd"
            };

            foreach (var kvp in charToGmdFiles)
            {
                string charCode = kvp.Key;
                if (string.Equals(charCode, "AOI", StringComparison.OrdinalIgnoreCase) || string.Equals(charCode, "SAR", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string gmdFile in kvp.Value)
                    {
                        string fileName = Path.GetFileName(gmdFile);
                        string relPath = (!string.IsNullOrEmpty(masterSkinsDir) && gmdFile.StartsWith(masterSkinsDir, StringComparison.OrdinalIgnoreCase))
                            ? gmdFile.Substring(masterSkinsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            : gmdFile;

                        bool inSpecialFolder = IsInSpecialChestPhysicsFolder(gmdFile, masterSkinsDir);
                        bool hasChestPhysics = false;
                        string detectedBone = "";

                        if (string.Equals(charCode, "AOI", StringComparison.OrdinalIgnoreCase))
                        {
                            if (aoiKnownHasPhysics.Contains(fileName))
                            {
                                hasChestPhysics = true;
                                detectedBone = "Known AOI Chest Physics Model";
                            }
                            else if (aoiKnownNoPhysics.Contains(fileName))
                            {
                                hasChestPhysics = false;
                            }
                            else if (scanAllWeightedMap.TryGetValue(gmdFile, out var weightedBones))
                            {
                                foreach (string b in weightedBones)
                                {
                                    if (aoiChestBones.Contains(b))
                                    {
                                        hasChestPhysics = true;
                                        detectedBone = b;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (string.Equals(charCode, "SAR", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.Equals(fileName, "c_v64_VF5_SAR_TEK.gmd", StringComparison.OrdinalIgnoreCase))
                            {
                                hasChestPhysics = false;
                            }
                            else if (scanAllWeightedMap.TryGetValue(gmdFile, out var weightedBones))
                            {
                                foreach (string b in weightedBones)
                                {
                                    if (sarChestBones.Contains(b))
                                    {
                                        hasChestPhysics = true;
                                        detectedBone = b;
                                        break;
                                    }
                                }
                            }
                        }

                        string standardFolder = suboption == 2 ? "master_skins/mods" : "master_skins/compiled";
                        string folderTag = inSpecialFolder ? "master_skins/special_chest_physics" : standardFolder;

                        if (hasChestPhysics)
                        {
                            string line = $"  - [{charCode} CHEST PHYSICS TEST] '{relPath}': HAS CHEST PHYSICS [Folder: {folderTag}]";
                            chestPhysicsTestResults.Add(line);
                            if (!inSpecialFolder)
                            {
                                string err = $"  - [{charCode}] '{relPath}' has breast/chest physics weights ({detectedBone}).";
                                chestLocationViolations.Add(err);
                                chestDumpLogBuilder.AppendLine(err);
                            }
                        }
                        else
                        {
                            string line = $"  - [{charCode} CHEST PHYSICS TEST] '{relPath}': NO CHEST PHYSICS [Folder: {folderTag}]";
                            chestPhysicsTestResults.Add(line);
                        }
                    }
                }
            }

            if (chestPhysicsTestResults.Count > 0)
            {
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("CHEST PHYSICS TEST REPORT (AOI & SAR)");
                Console.WriteLine("================================================================================");
                foreach (string res in chestPhysicsTestResults)
                {
                    Console.WriteLine(res);
                }
                Console.WriteLine("================================================================ algorithm\n");
            }

            if (chestLocationViolations.Count > 0)
            {
                string stdFolder = suboption == 2 ? "master_skins/mods/" : "master_skins/compiled/";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] Found {chestLocationViolations.Count} mod file(s) for AOI / SAR with breast/chest physics in standard '{stdFolder}':\n");
                foreach (string err in chestLocationViolations)
                {
                    Console.WriteLine(err);
                }

                Console.WriteLine("\n================================================================================");
                Console.WriteLine("INSTRUCTIONS:");
                Console.WriteLine("Mods for AOI / SAR that contain breast/chest physics MUST be placed in:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  master_skins/special_chest_physics/");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nMods WITHOUT breast/chest physics (e.g. SAR 'c_v64_VF5_SAR_TEK.gmd' or AOI 'AOI003_JO_OUT_01.gmd') may remain in:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {stdFolder}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("================================================================================");
                Console.ResetColor();

                chestDumpLogBuilder.AppendLine("\nINSTRUCTIONS:");
                chestDumpLogBuilder.AppendLine("Mods for AOI / SAR that contain breast/chest physics MUST be placed in:");
                chestDumpLogBuilder.AppendLine("  master_skins/special_chest_physics/");
                chestDumpLogBuilder.AppendLine($"\nMods WITHOUT breast/chest physics (e.g. SAR 'c_v64_VF5_SAR_TEK.gmd' or AOI 'AOI003_JO_OUT_01.gmd') may remain in:");
                chestDumpLogBuilder.AppendLine($"  {stdFolder}");

                string dumpPath = SaveErrorDumpLog(chestDumpLogBuilder.ToString(), "master_skin_chest_physics_error_dump.log");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("================================================================================");
                Console.WriteLine("[INFO] Complete error dump log has been generated and saved to:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {dumpPath}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("================================================================================");
                Console.ResetColor();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey(true);
                return false;
            }

            return true;
        }

        private static bool RunGatherCompileInternal(Options.GatherCompile opts, bool skipBackup)
        {
            WriteHeader();

            // 1. Resolve Reference PAR and Backup Path
            string refPar = Path.GetFullPath(opts.InputParArchivePath);
            string parDir = Path.GetDirectoryName(refPar);
            string backupPath = Path.Combine(parDir, "chara__bak.par");

            // If the original refPar exists, we make sure backupPath exists
            if (!skipBackup)
            {
                if (File.Exists(refPar))
                {
                    if (!File.Exists(backupPath))
                    {
                        Console.WriteLine($"Creating backup of original PAR file at:\n  {backupPath}");
                        try
                        {
                            File.Copy(refPar, backupPath, false);
                            Console.WriteLine("Backup created successfully.\n");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"WARNING: Could not create backup of original PAR: {ex.Message}\n");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Backup already exists at: {backupPath}\n");
                    }
                }
                else
                {
                    // If refPar (chara.par) doesn't exist, check if backupPath (chara__bak.par) exists
                    if (File.Exists(backupPath))
                    {
                        Console.WriteLine($"Reference PAR 'chara.par' not found, but backup 'chara__bak.par' exists. Using backup as reference.\n");
                    }
                    else
                    {
                        // Fallback to check if chara.par exists in the current folder (BaseDirectory)
                        string localChara = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chara.par");
                        string localBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chara__bak.par");
                        if (File.Exists(localChara))
                        {
                            refPar = localChara;
                            parDir = Path.GetDirectoryName(refPar);
                            backupPath = Path.Combine(parDir, "chara__bak.par");
                            if (!File.Exists(backupPath))
                            {
                                Console.WriteLine($"Creating backup of original PAR file at:\n  {backupPath}");
                                File.Copy(refPar, backupPath, false);
                            }
                        }
                        else if (File.Exists(localBackup))
                        {
                            backupPath = localBackup;
                            Console.WriteLine($"Using local backup 'chara__bak.par' as reference.\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"ERROR: Reference PAR file '{opts.InputParArchivePath}' or backup '{backupPath}' not found!");
                            Console.ResetColor();
                            return false;
                        }
                    }
                }
            }
            else
            {
                // If skipping backup, the reference PAR is just the refPar itself, no backup needed
                backupPath = refPar;
            }

            // 3. Resolve Directories
            string modsDir = Path.GetFullPath(opts.ModsDirectory);
            string outputPar = Path.GetFullPath(opts.OutputParArchivePath);

            if (!Directory.Exists(modsDir))
            {
                Console.WriteLine($"Creating mods directory at: {modsDir}");
                Directory.CreateDirectory(modsDir);
                Console.WriteLine("Please place your mod folders inside the 'mods' directory, then run this tool again.");
                return false;
            }

            Console.WriteLine("Step 1/2: Scanning mods and mapping overlay files...");
            var filesToOverlay = new List<(string PhysicalPath, string VirtualPath)>();
            var gmdHistory = new Dictionary<string, List<GmdEntry>>(StringComparer.OrdinalIgnoreCase);

            int authVoiceFilesCopied = 0;
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string outputDir = Path.GetDirectoryName(outputPar) ?? string.Empty;

            var readerParameters = new ParArchiveReaderParameters
            {
                Recursive = false,
            };

            if (opts.IsMasterSkinMod)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n================================================================================");
                if (opts.MasterSkinSuboption == 2)
                {
                    Console.WriteLine("OPTION 3 SUBOPTION 2: MASTER SKIN MODS (master_skins/mods)");
                }
                else
                {
                    Console.WriteLine("OPTION 3 SUBOPTION 1: MASTER SKINS COMPILED (master_skins/compiled)");
                }
                Console.WriteLine("================================================================\n");
                Console.ResetColor();

                string dummyDir = null;
                string[] dummyCandidates = new[]
                {
                    Path.Combine(currentDir, "vf5revows_dummy_gmds"),
                    Path.Combine(Environment.CurrentDirectory, "vf5revows_dummy_gmds"),
                    Path.Combine(currentDir, "documentation", "vf5_force_skin_mod", "vf5revows_dummy_gmds"),
                    Path.Combine(currentDir, "..", "vf5revows_dummy_gmds")
                };

                foreach (string cand in dummyCandidates)
                {
                    if (Directory.Exists(cand) && Directory.GetFiles(cand, "*.gmd").Length > 0)
                    {
                        dummyDir = cand;
                        break;
                    }
                }

                if (dummyDir == null)
                {
                    Console.WriteLine("Extracting embedded dummy GMD resources internally...");
                    dummyDir = ExtractEmbeddedDummyGmdsToTemp();
                }

                var charToVariantFolders = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var charToMasterFiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var charToGmdFiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var presentCharCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var ddsFilesToOverlay = new List<(string PhysicalPath, string VirtualPath)>();

                string masterSkinsDir = null;

                if (opts.MasterSkinSuboption == 2)
                {
                    // MODE 2: master_skins/mods
                    string[] candidates = new[]
                    {
                        Path.Combine(currentDir, "master_skins", "mods"),
                        Path.Combine(Environment.CurrentDirectory, "master_skins", "mods"),
                        Path.Combine(currentDir, "..", "master_skins", "mods")
                    };

                    foreach (string cand in candidates)
                    {
                        if (Directory.Exists(cand))
                        {
                            masterSkinsDir = cand;
                            break;
                        }
                    }

                    if (masterSkinsDir == null)
                    {
                        masterSkinsDir = Path.Combine(currentDir, "master_skins", "mods");
                        try { Directory.CreateDirectory(masterSkinsDir); } catch { }
                    }

                    Console.WriteLine($"Using dummy GMD templates folder: {dummyDir ?? "NOT FOUND"}");
                    Console.WriteLine($"Using master_skins/mods folder:   {masterSkinsDir}");

                    if (Directory.Exists(masterSkinsDir))
                    {
                        string[] modDirs = Directory.GetDirectories(masterSkinsDir);
                        foreach (string modDir in modDirs)
                        {
                            string modName = Path.GetFileName(modDir);

                            // Find all .gmd files in this mod directory
                            string[] gmdFiles = Directory.GetFiles(modDir, "*.gmd", SearchOption.AllDirectories);
                            foreach (string gmdFile in gmdFiles)
                            {
                                string charCode = MatchCharacterCode(gmdFile) ?? MatchCharacterCode(modName);
                                if (!string.IsNullOrEmpty(charCode))
                                {
                                    presentCharCodes.Add(charCode);

                                    if (!charToGmdFiles.ContainsKey(charCode))
                                    {
                                        charToGmdFiles[charCode] = new List<string>();
                                        charToMasterFiles[charCode] = new List<string>();
                                        charToVariantFolders[charCode] = new List<string>();
                                    }
                                    charToGmdFiles[charCode].Add(gmdFile);
                                    if (!charToVariantFolders[charCode].Exists(x => string.Equals(x, modName, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        charToVariantFolders[charCode].Add(modName);
                                    }

                                    // Collect all files in the directory containing the .gmd
                                    string gmdParentDir = Path.GetDirectoryName(gmdFile);
                                    if (Directory.Exists(gmdParentDir))
                                    {
                                        string[] pFiles = Directory.GetFiles(gmdParentDir, "*", SearchOption.TopDirectoryOnly);
                                        foreach (string pf in pFiles)
                                        {
                                            if (!charToMasterFiles[charCode].Exists(x => string.Equals(x, pf, StringComparison.OrdinalIgnoreCase)))
                                            {
                                                charToMasterFiles[charCode].Add(pf);
                                            }
                                        }
                                    }
                                }
                            }

                            // Gather .dds textures in this mod directory
                            string[] ddsFiles = Directory.GetFiles(modDir, "*.dds", SearchOption.AllDirectories);
                            foreach (string ddsFile in ddsFiles)
                            {
                                string fileName = Path.GetFileName(ddsFile);
                                ddsFilesToOverlay.Add((ddsFile, $"dds/{fileName}"));
                            }
                        }
                    }

                    // Duplicate Check for Mode 2
                    bool hasConflict = false;
                    var dumpLogBuilder = new System.Text.StringBuilder();
                    dumpLogBuilder.AppendLine("================================================================================");
                    dumpLogBuilder.AppendLine("VF5REVOWS MOD COMPILATION - MASTER SKIN MODS DUPLICATE ERROR DUMP LOG");
                    dumpLogBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    dumpLogBuilder.AppendLine("================================================================================\n");

                    foreach (var kvp in charToGmdFiles)
                    {
                        if (kvp.Value.Count > 1)
                        {
                            hasConflict = true;
                            string charCode = kvp.Key;
                            string charName = CodeToCharacterName.TryGetValue(charCode, out string name) ? name : charCode;

                            string errHeader = $"[ERROR] Multiple master skin .gmd files found for character '{charCode}' ({charName}):";
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n{errHeader}");
                            dumpLogBuilder.AppendLine(errHeader);

                            foreach (string f in kvp.Value)
                            {
                                string relPath = f.StartsWith(masterSkinsDir, StringComparison.OrdinalIgnoreCase)
                                    ? f.Substring(masterSkinsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                    : f;
                                Console.WriteLine($"  - {relPath}");
                                dumpLogBuilder.AppendLine($"  - {relPath}");
                            }

                            string errDetail1 = "Master Skins Mods mode supports only ONE master skin .gmd file per character across 'master_skins/mods'.";
                            string errDetail2 = $"Please remove the extra master skin .gmd file(s) for character '{charCode}' ({charName}) and try again.";
                            Console.WriteLine(errDetail1);
                            Console.WriteLine(errDetail2);
                            Console.ResetColor();

                            dumpLogBuilder.AppendLine(errDetail1);
                            dumpLogBuilder.AppendLine(errDetail2);
                            dumpLogBuilder.AppendLine();
                        }
                    }

                    if (hasConflict)
                    {
                        string dumpPath = SaveErrorDumpLog(dumpLogBuilder.ToString(), "master_skin_error_dump.log");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nCompilation aborted due to duplicate character master skin .gmd conflicts.");
                        Console.ResetColor();

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("================================================================================");
                        Console.WriteLine("[INFO] Complete error dump log has been generated and saved to:");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  {dumpPath}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("================================================================================");
                        Console.ResetColor();

                        Console.WriteLine("\nPress any key to exit...");
                        Console.ReadKey(true);
                        return false;
                    }
                }
                else
                {
                    // MODE 1: master_skins/compiled
                    string[] masterSkinsCandidates = new[]
                    {
                        Path.Combine(currentDir, "master_skins", "compiled"),
                        Path.Combine(Environment.CurrentDirectory, "master_skins", "compiled"),
                        Path.Combine(currentDir, "master_skins"),
                        Path.Combine(Environment.CurrentDirectory, "master_skins"),
                        Path.Combine(currentDir, "..", "master_skins"),
                        @"I:\_games\_pir\VF5revo_1.05\runtime\media\_\_______actual\New folder\master_skins"
                    };

                    foreach (string cand in masterSkinsCandidates)
                    {
                        if (Directory.Exists(cand))
                        {
                            masterSkinsDir = cand;
                            break;
                        }
                    }

                    Console.WriteLine($"Using dummy GMD templates folder: {dummyDir ?? "NOT FOUND"}");
                    Console.WriteLine($"Using master_skins folder:        {masterSkinsDir ?? "NOT FOUND"}");

                    string masterSkinsTopsDir = null;
                    if (masterSkinsDir != null && Directory.Exists(masterSkinsDir))
                    {
                        string[] candidateTops = new[]
                        {
                            Path.Combine(masterSkinsDir, "chara", "tops"),
                            Path.Combine(masterSkinsDir, "tops"),
                            Path.Combine(masterSkinsDir, "chara"),
                            masterSkinsDir
                        };

                        foreach (string cand in candidateTops)
                        {
                            if (Directory.Exists(cand))
                            {
                                string[] subdirs = Directory.GetDirectories(cand);
                                bool containsMatchingChar = false;
                                foreach (string sd in subdirs)
                                {
                                    if (!string.IsNullOrEmpty(MatchCharacterCode(Path.GetFileName(sd))))
                                    {
                                        containsMatchingChar = true;
                                        break;
                                    }
                                }
                                if (containsMatchingChar)
                                {
                                    masterSkinsTopsDir = cand;
                                    break;
                                }
                            }
                        }
                    }

                    if (masterSkinsTopsDir != null && Directory.Exists(masterSkinsTopsDir))
                    {
                        string[] variantDirs = Directory.GetDirectories(masterSkinsTopsDir);
                        foreach (string vdir in variantDirs)
                        {
                            string folderName = Path.GetFileName(vdir);
                            string charCode = MatchCharacterCode(folderName);
                            if (!string.IsNullOrEmpty(charCode))
                            {
                                string[] filesInVariant = Directory.GetFiles(vdir, "*", SearchOption.AllDirectories);
                                if (filesInVariant.Length > 0)
                                {
                                    presentCharCodes.Add(charCode);

                                    if (!charToVariantFolders.ContainsKey(charCode))
                                    {
                                        charToVariantFolders[charCode] = new List<string>();
                                        charToMasterFiles[charCode] = new List<string>();
                                        charToGmdFiles[charCode] = new List<string>();
                                    }
                                    charToVariantFolders[charCode].Add(folderName);
                                    charToMasterFiles[charCode].AddRange(filesInVariant);
                                    foreach (string f in filesInVariant)
                                    {
                                        if (f.EndsWith(".gmd", StringComparison.OrdinalIgnoreCase))
                                        {
                                            charToGmdFiles[charCode].Add(f);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Duplicate Check for Mode 1
                    bool hasConflict = false;
                    var dumpLogBuilder = new System.Text.StringBuilder();
                    dumpLogBuilder.AppendLine("================================================================================");
                    dumpLogBuilder.AppendLine("VF5REVOWS MOD COMPILATION - MASTER SKIN COMPILED DUPLICATE ERROR DUMP LOG");
                    dumpLogBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    dumpLogBuilder.AppendLine("================================================================================\n");

                    foreach (var kvp in charToVariantFolders)
                    {
                        if (kvp.Value.Count > 1)
                        {
                            hasConflict = true;
                            string charCode = kvp.Key;
                            string charName = CodeToCharacterName.TryGetValue(charCode, out string name) ? name : charCode;

                            string errHeader = $"[ERROR] Multiple master skin folders found for character '{charCode}' ({charName}):";
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n{errHeader}");
                            dumpLogBuilder.AppendLine(errHeader);

                            foreach (string f in kvp.Value)
                            {
                                Console.WriteLine($"  - {f}");
                                dumpLogBuilder.AppendLine($"  - {f}");
                            }

                            string errDetail1 = "Master Skins Compiled mode supports only ONE master skin folder per character under 'master_skins/compiled/chara/tops'.";
                            string errDetail2 = $"Please remove the extra master skin folder(s) for '{charCode}' ({charName}) and try again.";
                            Console.WriteLine(errDetail1);
                            Console.WriteLine(errDetail2);
                            Console.ResetColor();

                            dumpLogBuilder.AppendLine(errDetail1);
                            dumpLogBuilder.AppendLine(errDetail2);
                            dumpLogBuilder.AppendLine();
                        }
                    }

                    if (hasConflict)
                    {
                        string dumpPath = SaveErrorDumpLog(dumpLogBuilder.ToString(), "master_skin_error_dump.log");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nCompilation aborted due to character master skin folder conflicts.");
                        Console.ResetColor();

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("================================================================================");
                        Console.WriteLine("[INFO] Complete error dump log has been generated and saved to:");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  {dumpPath}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("================================================================================");
                        Console.ResetColor();

                        Console.WriteLine("\nPress any key to exit...");
                        Console.ReadKey(true);
                        return false;
                    }

                    // Mode 1 DDS textures
                    string masterSkinsDdsDir = Path.Combine(masterSkinsDir, "chara", "dds");
                    if (!Directory.Exists(masterSkinsDdsDir))
                    {
                        masterSkinsDdsDir = Path.Combine(masterSkinsDir, "dds");
                    }
                    if (Directory.Exists(masterSkinsDdsDir))
                    {
                        string[] ddsFiles = Directory.GetFiles(masterSkinsDdsDir, "*.dds", SearchOption.AllDirectories);
                        foreach (string ddsFile in ddsFiles)
                        {
                            string fileName = Path.GetFileName(ddsFile);
                            ddsFilesToOverlay.Add((ddsFile, $"dds/{fileName}"));
                        }
                    }
                }

                // Scan master_skins/special_chest_physics directory for models & textures across Suboption 1 & 2
                string[] specialChestScanPaths = new[]
                {
                    Path.Combine(currentDir, "master_skins", "special_chest_physics"),
                    Path.Combine(Environment.CurrentDirectory, "master_skins", "special_chest_physics"),
                    Path.Combine(currentDir, "master_skins", "mods", "special_chest_physics"),
                    Path.Combine(currentDir, "master_skins", "compiled", "special_chest_physics")
                };

                foreach (string specialDir in specialChestScanPaths)
                {
                    if (Directory.Exists(specialDir))
                    {
                        string[] gmdFiles = Directory.GetFiles(specialDir, "*.gmd", SearchOption.AllDirectories);
                        foreach (string gmdFile in gmdFiles)
                        {
                            string charCode = MatchCharacterCode(gmdFile);
                            if (!string.IsNullOrEmpty(charCode))
                            {
                                presentCharCodes.Add(charCode);
                                if (!charToGmdFiles.ContainsKey(charCode))
                                {
                                    charToGmdFiles[charCode] = new List<string>();
                                    charToMasterFiles[charCode] = new List<string>();
                                    charToVariantFolders[charCode] = new List<string>();
                                }
                                if (!charToGmdFiles[charCode].Contains(gmdFile))
                                {
                                    charToGmdFiles[charCode].Add(gmdFile);
                                }
                                string gmdParentDir = Path.GetDirectoryName(gmdFile);
                                if (Directory.Exists(gmdParentDir))
                                {
                                    string[] pFiles = Directory.GetFiles(gmdParentDir, "*", SearchOption.TopDirectoryOnly);
                                    foreach (string pf in pFiles)
                                    {
                                        if (!charToMasterFiles[charCode].Contains(pf))
                                        {
                                            charToMasterFiles[charCode].Add(pf);
                                        }
                                    }
                                }
                            }
                        }

                        string[] ddsFiles = Directory.GetFiles(specialDir, "*.dds", SearchOption.AllDirectories);
                        foreach (string ddsFile in ddsFiles)
                        {
                            string fileName = Path.GetFileName(ddsFile);
                            if (!ddsFilesToOverlay.Exists(x => x.PhysicalPath.Equals(ddsFile, StringComparison.OrdinalIgnoreCase)))
                            {
                                ddsFilesToOverlay.Add((ddsFile, $"dds/{fileName}"));
                            }
                        }
                    }
                }

                // CHEST PHYSICS TEST FOR AOI & SAR (BOTH SUBOPTION 1 AND SUBOPTION 2)
                var (_, scanAllWeightedMap) = masterSkinsDir != null && (Directory.Exists(masterSkinsDir) || File.Exists(masterSkinsDir))
                    ? CheckAllMasterSkinNonCommonBones(masterSkinsDir)
                    : (new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));

                foreach (string specialDir in specialChestScanPaths)
                {
                    if (Directory.Exists(specialDir))
                    {
                        var (_, extraWeightedMap) = CheckAllMasterSkinNonCommonBones(specialDir);
                        foreach (var kvp in extraWeightedMap)
                        {
                            scanAllWeightedMap[kvp.Key] = kvp.Value;
                        }
                    }
                }

                if (!RunChestPhysicsTest(opts.MasterSkinSuboption, masterSkinsDir, charToGmdFiles, scanAllWeightedMap))
                {
                    return false;
                }

                // MCC ROSTER CHECK
                var presentList = new List<string>();
                var missingList = new List<string>();

                foreach (string code in CharacterCodes)
                {
                    string name = CodeToCharacterName.TryGetValue(code, out string charName) ? charName : code;
                    string displayName = $"{name} ({code})";

                    if (presentCharCodes.Contains(code))
                    {
                        presentList.Add(displayName);
                    }
                    else
                    {
                        missingList.Add(displayName);
                    }
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("================================================================================");
                Console.WriteLine("MCC ROSTER COMPLETENESS CHECK");
                Console.WriteLine("================================================================================");
                Console.ResetColor();

                bool isMccComplete = (missingList.Count == 0);

                Console.WriteLine($"Total MCC Roster Slots:  {CharacterCodes.Length}");
                Console.WriteLine($"Present Master Skins:    {presentList.Count} / {CharacterCodes.Length}");
                Console.WriteLine($"Missing Master Skins:    {missingList.Count} / {CharacterCodes.Length}");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- PRESENT CHARACTERS ({presentList.Count}) ---");
                foreach (string item in presentList)
                {
                    Console.WriteLine($"  [PRESENT] {item}");
                }
                Console.ResetColor();
                Console.WriteLine();

                if (missingList.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"--- MISSING CHARACTERS ({missingList.Count}) ---");
                    foreach (string item in missingList)
                    {
                        Console.WriteLine($"  [MISSING] {item}");
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("NOTICE: Missing MCC characters will be OMITTED from the rest of the compilation");
                    Console.WriteLine("process (including Stage 1 GMD dummy blanking and Stage 2 master skin deployment).");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("SUCCESS: All 21 MCC roster characters are present! Proceeding normally.");
                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("================================================================================");
                Console.WriteLine("PROMPT: PROCEED WITH COMPILATION?");
                Console.WriteLine("================================================================================");
                Console.ResetColor();
                Console.WriteLine("1. Yes - Proceed with compilation (omitting missing MCC roster characters if incomplete)");
                Console.WriteLine("2. No  - Abort compilation and exit");
                Console.Write("\nChoice [1/2] (or Y/N, default=1): ");
                string answer = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(answer) && (answer == "2" || answer.StartsWith("n", StringComparison.OrdinalIgnoreCase) || answer.StartsWith("N", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nCompilation cancelled by user.");
                    Console.ResetColor();
                    return false;
                }

                var activeCharCodes = isMccComplete
                    ? new HashSet<string>(CharacterCodes, StringComparer.OrdinalIgnoreCase)
                    : presentCharCodes;

                Console.WriteLine("\nStep 1/3: Reading reference PAR archive for virtual GMD and tops folder mapping...");
                Node tempPar = NodeFactory.FromFile(backupPath, Yarhl.IO.FileOpenMode.Read);
                tempPar.TransformWith(new ParArchiveReader(readerParameters));
                Node tempDest = FindDestinationNode(tempPar);

                var virtualGmds = new List<(string VirtualPath, string CharCode)>();
                TraverseAndCollectVirtualGmds(tempDest, string.Empty, virtualGmds);

                var characterCostumeFolders = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                TraverseAndCollectVirtualTopsFolders(tempDest, string.Empty, characterCostumeFolders);

                tempPar.Dispose();

                Console.WriteLine($"Found {virtualGmds.Count} virtual GMD files across reference archive.");
                int totalCostumeFolderCount = 0;
                foreach (var kvp in characterCostumeFolders) totalCostumeFolderCount += kvp.Value.Count;
                Console.WriteLine($"Found {totalCostumeFolderCount} tops costume folders across {characterCostumeFolders.Count} character slots.");

                if (dummyDir != null && Directory.Exists(dummyDir))
                {
                    int blankedCount = 0;
                    foreach (var item in virtualGmds)
                    {
                        if (!activeCharCodes.Contains(item.CharCode))
                        {
                            continue; // OMIT character not included in active MCC roster
                        }

                        if (CodeToDummyBoneFile.TryGetValue(item.CharCode, out string dummyFilename))
                        {
                            string dummyFilePath = Path.Combine(dummyDir, dummyFilename);
                            if (File.Exists(dummyFilePath))
                            {
                                filesToOverlay.Add((dummyFilePath, item.VirtualPath));
                                blankedCount++;
                            }
                        }
                    }
                    Console.WriteLine($"[STAGE 1] Mapped {blankedCount} GMD files for dummy blanking ({activeCharCodes.Count} character slots active).");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARNING: Dummy GMD templates folder not found!");
                    Console.ResetColor();
                }

                int masterSkinsDeployed = 0;

                // Weight checking on all active master skin .gmd files
                Console.WriteLine("[STAGE 2] Pre-scanning all master skin .gmd files for bone weight compatibility...");
                var (rawInvalidMap, _) = masterSkinsDir != null && (Directory.Exists(masterSkinsDir) || File.Exists(masterSkinsDir))
                    ? CheckAllMasterSkinNonCommonBones(masterSkinsDir)
                    : (new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));

                var invalidGmdsMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in rawInvalidMap)
                {
                    string charCode = MatchCharacterCode(kvp.Key);
                    if (!string.IsNullOrEmpty(charCode) && activeCharCodes.Contains(charCode))
                    {
                        invalidGmdsMap[kvp.Key] = kvp.Value;
                    }
                }

                if (invalidGmdsMap.Count > 0)
                {
                    var dumpLogBuilder = new System.Text.StringBuilder();
                    dumpLogBuilder.AppendLine("================================================================================");
                    dumpLogBuilder.AppendLine("VF5REVOWS MOD COMPILATION - BONE WEIGHT ERROR DUMP LOG");
                    dumpLogBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    dumpLogBuilder.AppendLine("================================================================ me\n");
                    dumpLogBuilder.AppendLine($"Found {invalidGmdsMap.Count} master skin .gmd file(s) with non-common bone weights:\n");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERROR] Found {invalidGmdsMap.Count} master skin .gmd file(s) with non-common bone weights:\n");

                    foreach (var kvp in invalidGmdsMap)
                    {
                        string gmdFile = kvp.Key;
                        List<string> nonCommonBones = kvp.Value;
                        string relPath = (masterSkinsDir != null && gmdFile.StartsWith(masterSkinsDir, StringComparison.OrdinalIgnoreCase))
                            ? gmdFile.Substring(masterSkinsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            : Path.GetFileName(gmdFile);

                        Console.WriteLine($"--------------------------------------------------------------------------------");
                        Console.WriteLine($"Master Skin GMD: {relPath} ({nonCommonBones.Count} non-common weighted bones)");
                        Console.WriteLine($"--------------------------------------------------------------------------------");
                        dumpLogBuilder.AppendLine($"--------------------------------------------------------------------------------");
                        dumpLogBuilder.AppendLine($"Master Skin GMD: {relPath} ({nonCommonBones.Count} non-common weighted bones)");
                        dumpLogBuilder.AppendLine($"--------------------------------------------------------------------------------");

                        foreach (string ncb in nonCommonBones)
                        {
                            Console.WriteLine($"  - {ncb}");
                            dumpLogBuilder.AppendLine($"  - {ncb}");
                        }
                        Console.WriteLine();
                        dumpLogBuilder.AppendLine();
                    }

                    string errOverview1 = "================================================================================";
                    string errOverview2 = "ERROR: Master skin mods use basic dummy armatures for cross-slot replacement.";
                    string errOverview3 = "Files with weights on non-common bones (such as hair or coat physics) will distort or crash in-game when loaded on dummy armatures.";
                    string errOverview4 = "Please fix the master skin model(s) listed above and try again.";
                    string errOverview5 = "================================================================================";

                    Console.WriteLine(errOverview1);
                    Console.WriteLine(errOverview2);
                    Console.WriteLine(errOverview3);
                    Console.WriteLine(errOverview4);
                    Console.WriteLine(errOverview5);
                    Console.ResetColor();

                    dumpLogBuilder.AppendLine(errOverview1);
                    dumpLogBuilder.AppendLine(errOverview2);
                    dumpLogBuilder.AppendLine(errOverview3);
                    dumpLogBuilder.AppendLine(errOverview4);
                    dumpLogBuilder.AppendLine(errOverview5);

                    string dumpPath = SaveErrorDumpLog(dumpLogBuilder.ToString(), "master_skin_bone_error_dump.log");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nCompilation aborted due to non-common bone weights error.");
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("================================================================================");
                    Console.WriteLine("[INFO] Complete error dump log has been generated and saved to:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {dumpPath}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("================================================================================");
                    Console.ResetColor();

                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey(true);
                    return false;
                }

                foreach (var kvp in charToMasterFiles)
                {
                    string charCode = kvp.Key;
                    if (!activeCharCodes.Contains(charCode)) continue; // OMIT missing character

                    List<string> sourceFiles = kvp.Value;

                    // 1. Deploy to all whitelisted costume folders for this character
                    foreach (string targetFolder in WhitelistedCostumeFolders)
                    {
                        if (string.Equals(MatchCharacterCode(targetFolder), charCode, StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (string srcFile in sourceFiles)
                            {
                                string ext = Path.GetExtension(srcFile);
                                string srcNameNoExt = Path.GetFileNameWithoutExtension(srcFile);
                                string vdirName = Path.GetFileName(Path.GetDirectoryName(srcFile));

                                // Special case for AOI: do not use c_v10_VF5_AOI.gmd for master skin
                                if (string.Equals(charCode, "AOI", StringComparison.OrdinalIgnoreCase) &&
                                    string.Equals(Path.GetFileName(srcFile), "c_v10_VF5_AOI.gmd", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                string targetFileName;
                                if (string.Equals(charCode, "AOI", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(srcNameNoExt, vdirName, StringComparison.OrdinalIgnoreCase))
                                {
                                    targetFileName = $"{targetFolder}{ext}";
                                }
                                else
                                {
                                    string suffix = srcNameNoExt.StartsWith(vdirName, StringComparison.OrdinalIgnoreCase)
                                        ? srcNameNoExt.Substring(vdirName.Length)
                                        : $"_{srcNameNoExt}";
                                    targetFileName = $"{targetFolder}{suffix}{ext}";
                                }

                                string targetVirtualPath = $"tops/{targetFolder}/{targetFileName}";
                                filesToOverlay.Add((srcFile, targetVirtualPath));
                                masterSkinsDeployed++;
                                Console.WriteLine($"[STAGE 2] Master skin '{vdirName}/{Path.GetFileName(srcFile)}' -> {targetVirtualPath}");
                            }
                        }
                    }

                    // 2. Deploy master skin .gmd files to special item whitelist overrides for this character
                    foreach (var special in SpecialItemMappings)
                    {
                        if (string.Equals(special.CharCode, charCode, StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (string srcFile in sourceFiles)
                            {
                                if (Path.GetExtension(srcFile).Equals(".gmd", StringComparison.OrdinalIgnoreCase))
                                {
                                    filesToOverlay.Add((srcFile, special.VirtualPath));
                                    masterSkinsDeployed++;
                                    Console.WriteLine($"[STAGE 2] Special Master Skin Item Override: '{Path.GetFileName(srcFile)}' -> {special.VirtualPath}");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"[STAGE 2] Deployed {masterSkinsDeployed} master skin costume files across whitelisted slots.");

                foreach (var ddsItem in ddsFilesToOverlay)
                {
                    filesToOverlay.Add(ddsItem);
                }
                Console.WriteLine($"[STAGE 3] Deployed {ddsFilesToOverlay.Count} DDS texture assets.");
            }

            if (!opts.IsMasterSkinMod)
            {
                // 5. Scan mods folders
                string[] subDirs = Directory.GetDirectories(modsDir);
            foreach (string modPath in subDirs)
            {
                string modName = Path.GetFileName(modPath);
                if (string.Equals(modName, "__all", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool hasGmdFiles = false;

                if (opts.IsSoundMod)
                {
                    string romSrc = Path.Combine(modPath, "rom");
                    if (Directory.Exists(romSrc))
                    {
                        int copiedCount = 0;
                        foreach (string file in Directory.GetFiles(romSrc, "*", SearchOption.AllDirectories))
                        {
                            string relPath = file.Substring(romSrc.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            string normRelPath = relPath.Replace('\\', '/');

                            if (normRelPath.StartsWith("sound/voice/auth_voice/", StringComparison.OrdinalIgnoreCase))
                            {
                                string fileName = Path.GetFileName(file);
                                string destFolder = Path.Combine(outputDir, "auth_voice");
                                Directory.CreateDirectory(destFolder);
                                File.Copy(file, Path.Combine(destFolder, fileName), true);
                                copiedCount++;
                                authVoiceFilesCopied++;
                            }
                            else
                            {
                                filesToOverlay.Add((file, $"rom/{normRelPath}"));
                            }
                        }
                        if (copiedCount > 0)
                        {
                            Console.WriteLine($"Gathering from: {modName} ({copiedCount} auth_voice files copied to output)");
                        }
                        else
                        {
                            Console.WriteLine($"Gathering from: {modName}");
                        }
                    }
                    continue; // Skip the chara mod logic
                }


                // 1. bone
                string boneSrc = Path.Combine(modPath, "chara", "bone");
                if (Directory.Exists(boneSrc))
                {
                    foreach (string file in Directory.GetFiles(boneSrc))
                    {
                        filesToOverlay.Add((file, $"bone/{Path.GetFileName(file)}"));
                    }
                }

                // 2. dds
                string ddsSrc = Path.Combine(modPath, "chara", "dds");
                if (Directory.Exists(ddsSrc))
                {
                    foreach (string file in Directory.GetFiles(ddsSrc))
                    {
                        filesToOverlay.Add((file, $"dds/{Path.GetFileName(file)}"));
                    }
                }

                // 3. dds_append
                string ddsAppSrc = Path.Combine(modPath, "chara", "dds_append");
                if (Directory.Exists(ddsAppSrc))
                {
                    foreach (string file in Directory.GetFiles(ddsAppSrc))
                    {
                        filesToOverlay.Add((file, $"dds/{Path.GetFileName(file)}"));
                    }
                }

                // 4. vf5item
                string vf5itemSrc = Path.Combine(modPath, "chara", "vf5item");
                if (Directory.Exists(vf5itemSrc))
                {
                    // Scan files recursively
                    string[] allFiles = Directory.GetFiles(vf5itemSrc, "*", SearchOption.AllDirectories);
                    foreach (string file in allFiles)
                    {
                        string relPath = file.Substring(vf5itemSrc.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        filesToOverlay.Add((file, $"vf5item/{relPath.Replace('\\', '/')}"));
                    }

                    // Scan for .gmd files recursively for history check
                    string[] gmdFiles = Directory.GetFiles(vf5itemSrc, "*.gmd", SearchOption.AllDirectories);
                    foreach (string gmdFile in gmdFiles)
                    {
                        hasGmdFiles = true;
                        string relPath = gmdFile.Substring(vf5itemSrc.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        string displayPath = $"/vf5item/{relPath.Replace('\\', '/')}";
                        Console.WriteLine($"Gathering from: {modName} || {displayPath}");
                        AddToHistory(gmdHistory, gmdFile, displayPath, modName);
                    }
                }

                // 5. tops
                string topsSrc = Path.Combine(modPath, "chara", "tops");
                if (Directory.Exists(topsSrc))
                {
                    // Scan directly inside tops
                    foreach (string gmdFile in Directory.GetFiles(topsSrc, "*.gmd"))
                    {
                        hasGmdFiles = true;
                        Console.WriteLine($"Gathering from: {modName} || /{Path.GetFileName(gmdFile)}");
                    }

                    // Scan immediate subdirectories recursively
                    foreach (string subDir in Directory.GetDirectories(topsSrc))
                    {
                        string subDirName = Path.GetFileName(subDir);
                        foreach (string file in Directory.GetFiles(subDir, "*", SearchOption.AllDirectories))
                        {
                            string relPath = file.Substring(subDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            filesToOverlay.Add((file, $"tops/{subDirName}/{relPath.Replace('\\', '/')}"));

                            if (file.EndsWith(".gmd", StringComparison.OrdinalIgnoreCase))
                            {
                                hasGmdFiles = true;
                                string filenameNoExt = Path.GetFileNameWithoutExtension(file);
                                string displayPath = $"{subDirName}/{Path.GetFileName(file)}";
                                Console.WriteLine($"Gathering from: {modName} || /{displayPath}");

                                if (string.Equals(subDirName, filenameNoExt, StringComparison.OrdinalIgnoreCase))
                                {
                                    AddToHistory(gmdHistory, file, $"/{displayPath}", modName);
                                }
                            }
                        }
                    }
                }

                if (!hasGmdFiles)
                {
                    Console.WriteLine($"Gathering from: {modName}");
                }
            }

                // 6. Check conflicts
                CheckConflicts(gmdHistory);
            }
            Console.WriteLine($"Scanning complete. Found {filesToOverlay.Count} files to overlay directly into PAR.\n");

            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (File.Exists(outputPar))
            {
                try
                {
                    File.Delete(outputPar);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: Could not remove existing output PAR file: {ex.Message}");
                    Console.ResetColor();
                    return false;
                }
            }

            var masterReaderParams = new ParArchiveReaderParameters
            {
                Recursive = false,
            };

            var writerParameters = new ParArchiveWriterParameters
            {
                CompressorVersion = opts.Compression,
                OutputPath = outputPar,
                IncludeDots = false,
            };

            Node par = null;
            Node node = null;

            try
            {
                Console.WriteLine("Step 2/2: Merging files into PAR archive (in-memory/stream overlay)...");
                Console.Write("Reading PAR file... ");
                par = NodeFactory.FromFile(backupPath, Yarhl.IO.FileOpenMode.Read);
                par.TransformWith(new ParArchiveReader(readerParameters));
                Console.WriteLine("DONE!");

                Console.Write("Adding files directly to destination node... ");
                Node destinationNode = FindDestinationNode(par);
                foreach (var overlay in filesToOverlay)
                {
                    AddFileToVirtualNode(destinationNode, overlay.PhysicalPath, overlay.VirtualPath);
                }

#pragma warning disable CA1308 // Normalize strings to uppercase
                destinationNode.SortChildren((x, y) => string.CompareOrdinal(x.Name.ToLowerInvariant(), y.Name.ToLowerInvariant()));
#pragma warning restore CA1308 // Normalize strings to uppercase
                Console.WriteLine("DONE!");

                // Setup static variables for progress bar
                totalFilesToCompress = CountFiles(par);
                compressedFilesCount = 0;

                ParArchiveWriter.NestedParCreating += sender => Console.WriteLine($"\nCreating nested PAR {sender.Name}... ");
                ParArchiveWriter.NestedParCreated += sender => Console.WriteLine($"{sender.Name} created!");
                ParArchiveWriter.FileCompressing += OnFileCompressing;

                Console.WriteLine("Creating PAR (this may take a while)... ");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPar));

                // Draw initial progress bar
                DrawProgressBar(0, totalFilesToCompress);

                par.TransformWith(new ParArchiveWriter(writerParameters));

                // Force 100% completion bar draw at the end
                DrawProgressBar(totalFilesToCompress, totalFilesToCompress);
                Console.WriteLine();
            }
            finally
            {
                // Unhook event handlers
                ParArchiveWriter.FileCompressing -= OnFileCompressing;
                
                // Release the lock on the reference PAR file
                par?.Dispose();
                node?.Dispose();
            }

            Console.WriteLine("DONE!\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================================================================================");
            Console.WriteLine($"SUCCESS: New PAR archive compiled successfully at:\n  {outputPar}");
            Console.WriteLine("================================================================================");
            if (opts.IsSoundMod && authVoiceFilesCopied > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=================== [IMPORTANT INSTRUCTIONS FOR AUTH_VOICE] ===================");
                Console.WriteLine("Some sound mod files were copied to the local output folder instead of the PAR:");
                Console.WriteLine($" -> {Path.Combine(outputDir, "auth_voice")}");
                Console.WriteLine();
                Console.WriteLine("Please follow these steps to apply them to your game:");
                Console.WriteLine("1. Create a BACKUP copy of your original 'auth_voice' folder located at:");
                Console.WriteLine(@"   \steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_media\rom\sound\voice\auth_voice");
                Console.WriteLine("2. Copy all the files from the newly created folder:");
                Console.WriteLine($"   {Path.Combine(outputDir, "auth_voice")}");
                Console.WriteLine("3. Paste and overwrite them into the game's folder:");
                Console.WriteLine(@"   \steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_media\rom\sound\voice\auth_voice");
                Console.WriteLine("================================================================================");
                Console.ResetColor();
                Console.WriteLine();
            }

            return true;
        }

                                private static void ShowCommonBonesGuide()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("          VF5REVO MASTER SKIN RIGGING & STRICT COMMON BONE GUIDE                ");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
STRICT WEIGHT PAINTING RULE FOR MASTER SKINS:
  - 100% of geometry vertex weights (weight_data > 0) MUST be assigned ONLY
    to the 118 Whitelisted Common Dummy Bones listed below (or allowed female breast physics bones).
  - Models WITH breast/chest physics (AOI & SAR) MUST be placed in 'master_skins/special_chest_physics/'.
  - DO NOT assign weights to character-unique physics bones (e.g. hair strands 'j_kami_xxx',
    coat tails 'j_1_fukua_xxx', capes, skirts, or extra facial accessory bones).
  - Unique physics bones will distort or crash the game when loaded on other character slots.");
            Console.ResetColor();

            Console.WriteLine($@"
--------------------------------------------------------------------------------
COMMON BONES CATEGORY BREAKDOWN ({WhitelistedCommonBones.Count} BONES TOTAL)
--------------------------------------------------------------------------------
  1. Root & System Nodes (10) : center_c_n, sync_c_n, pattern_c_n, vector_c_n, etc.
  2. Major Body Joints  (28) : j_mune_wj, j_kata_l/r, j_ude_l/r, j_momo_l/r, etc.
  3. Hand & Finger Rigs (30) : Full 3-joint rigs for both hands (nl_oya, nl_hito, etc.)
  4. Facial & Jaw Rig   (38) : Full facial rig (cl_kao, j_kao_wj, kl_ago_wj, tl_ago_wj, tl_ha_wj, etc.)
  5. Control Nodes      (12) : e_kao_cp, e_mune_cp, j_opal_058wj, c_opal_021_osg, etc.");

            Console.WriteLine($@"
--------------------------------------------------------------------------------
COMPLETE LIST OF {WhitelistedCommonBones.Count} COMMON DUMMY BONES (ALPHABETICAL):
--------------------------------------------------------------------------------");

            var sortedBones = new List<string>(WhitelistedCommonBones);
            sortedBones.Sort();

            for (int i = 0; i < sortedBones.Count; i += 3)
            {
                string b1 = sortedBones[i].PadRight(26);
                string b2 = (i + 1 < sortedBones.Count) ? sortedBones[i + 1].PadRight(26) : "";
                string b3 = (i + 2 < sortedBones.Count) ? sortedBones[i + 2] : "";
                Console.WriteLine($"  {b1} {b2} {b3}");
            }

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Guide complete. Press any key to return to main menu...");
            Console.ReadKey(true);
        }

        private static string CategorizeCommonBone(string boneName)
        {
            string lower = boneName.ToLowerInvariant();
            if (lower.Contains("center_c_n") || lower.Contains("sync_c_n") || lower.Contains("pattern_c_n") || lower.Contains("vector_c_n") || lower.EndsWith("_c_n"))
                return "Root & System Nodes";
            if (lower.StartsWith("nl_") || lower.Contains("finger") || lower.Contains("te_"))
                return "Hand & Finger Rigs";
            if (lower.StartsWith("cl_") || lower.StartsWith("kl_") || lower.StartsWith("tl_") || lower.Contains("kao") || lower.Contains("eye") || lower.Contains("mabu") || lower.Contains("mayu") || lower.Contains("kuti") || lower.Contains("hoho") || lower.Contains("ago") || lower.Contains("ha"))
                return "Facial & Jaw Rig";
            if (lower.StartsWith("e_") || lower.Contains("_cp") || lower.Contains("opal") || lower.Contains("_osg"))
                return "Control Nodes";
            
            return "Major Body Joints";
        }

        private static void RunMasterSkinBoneWeightAudit()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("        VF5REVO MASTER SKIN BONE WEIGHT AUDIT & CATEGORICAL REPORT              ");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine();

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var scanDirectories = new List<string>
            {
                Path.Combine(currentDir, "master_skins", "mods"),
                Path.Combine(currentDir, "master_skins", "compiled"),
                Path.Combine(currentDir, "master_skins", "special_chest_physics"),
                Path.Combine(currentDir, "master_skins")
            };

            var targetGmdFiles = new List<string>();
            foreach (string dir in scanDirectories)
            {
                if (Directory.Exists(dir))
                {
                    string[] files = Directory.GetFiles(dir, "*.gmd", SearchOption.AllDirectories);
                    foreach (string f in files)
                    {
                        if (!targetGmdFiles.Contains(f))
                        {
                            targetGmdFiles.Add(f);
                        }
                    }
                }
            }

            if (targetGmdFiles.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO] No .gmd files found under 'master_skins/' directories to audit.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Found {targetGmdFiles.Count} .gmd model file(s) across master_skins directories.");
            Console.WriteLine("Running vertex weight scanner...\n");

            var reportBuilder = new System.Text.StringBuilder();
            reportBuilder.AppendLine("================================================================================");
            reportBuilder.AppendLine("VF5REVOWS MASTER SKIN BONE WEIGHT CATEGORICAL AUDIT REPORT");
            reportBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            reportBuilder.AppendLine("================================================================================\n");

            int passedCount = 0;
            int failedCount = 0;

            for (int index = 0; index < targetGmdFiles.Count; index++)
            {
                string gmdPath = targetGmdFiles[index];
                string charCode = MatchCharacterCode(gmdPath);
                string charName = !string.IsNullOrEmpty(charCode) && CodeToCharacterName.TryGetValue(charCode, out var name) ? name : (charCode ?? "UNKNOWN");

                var (_, allWeightedMap) = CheckAllMasterSkinNonCommonBones(gmdPath);
                List<string> weightedBones = allWeightedMap.TryGetValue(gmdPath, out var list) ? list : new List<string>();

                var commonBonesMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var allowedCharBones = new List<string>();
                var nonCommonBones = new List<string>();

                HashSet<string> allowedForChar = !string.IsNullOrEmpty(charCode) && CharacterSpecificAllowedBones.TryGetValue(charCode, out var allowedSet)
                    ? allowedSet
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (string boneName in weightedBones)
                {
                    if (WhitelistedCommonBones.Contains(boneName))
                    {
                        string cat = CategorizeCommonBone(boneName);
                        if (!commonBonesMap.ContainsKey(cat))
                        {
                            commonBonesMap[cat] = new List<string>();
                        }
                        commonBonesMap[cat].Add(boneName);
                    }
                    else if (allowedForChar.Contains(boneName))
                    {
                        allowedCharBones.Add(boneName);
                    }
                    else
                    {
                        nonCommonBones.Add(boneName);
                    }
                }

                bool isPassed = (nonCommonBones.Count == 0);
                if (isPassed) passedCount++; else failedCount++;

                string relPath = gmdPath.StartsWith(currentDir, StringComparison.OrdinalIgnoreCase)
                    ? gmdPath.Substring(currentDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    : gmdPath;

                string header = $"[{index + 1}/{targetGmdFiles.Count}] MODEL: {relPath}";
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine(header);
                Console.WriteLine($"Character Slot: {charCode ?? "N/A"} ({charName})");
                Console.WriteLine($"Total Weighted Bones: {weightedBones.Count}");

                reportBuilder.AppendLine("--------------------------------------------------------------------------------");
                reportBuilder.AppendLine(header);
                reportBuilder.AppendLine($"Character Slot: {charCode ?? "N/A"} ({charName})");
                reportBuilder.AppendLine($"Total Weighted Bones: {weightedBones.Count}");

                if (isPassed)
                {
                    string statusStr = allowedCharBones.Count > 0
                        ? "[PASSED WITH ALLOWED CHARACTER PHYSICS BONES]"
                        : "[PASSED - 100% COMMON DUMMY BONES]";

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Compatibility Status: {statusStr}");
                    Console.ResetColor();
                    reportBuilder.AppendLine($"Compatibility Status: {statusStr}");
                }
                else
                {
                    string statusStr = $"[FAILED - CONTAINS {nonCommonBones.Count} NON-COMMON BONES]";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Compatibility Status: {statusStr}");
                    Console.ResetColor();
                    reportBuilder.AppendLine($"Compatibility Status: {statusStr}");
                }

                Console.WriteLine();
                reportBuilder.AppendLine();

                // A. Allowed Common Dummy Bones
                int totalCommon = 0;
                foreach (var kvp in commonBonesMap) totalCommon += kvp.Value.Count;

                Console.WriteLine($"  A. ALLOWED COMMON DUMMY BONES ({totalCommon} bones):");
                reportBuilder.AppendLine($"  A. ALLOWED COMMON DUMMY BONES ({totalCommon} bones):");

                if (commonBonesMap.Count > 0)
                {
                    foreach (var kvp in commonBonesMap)
                    {
                        string catLine = $"     - {kvp.Key} ({kvp.Value.Count}): {string.Join(", ", kvp.Value)}";
                        Console.WriteLine(catLine);
                        reportBuilder.AppendLine(catLine);
                    }
                }
                else
                {
                    Console.WriteLine("     None");
                    reportBuilder.AppendLine("     None");
                }

                // B. Allowed Character-Specific Bones
                Console.WriteLine($"\n  B. ALLOWED CHARACTER-SPECIFIC PHYSICS BONES ({allowedCharBones.Count} bones):");
                reportBuilder.AppendLine($"\n  B. ALLOWED CHARACTER-SPECIFIC PHYSICS BONES ({allowedCharBones.Count} bones):");
                if (allowedCharBones.Count > 0)
                {
                    foreach (string ab in allowedCharBones)
                    {
                        string abLine = $"     - {ab} (Allowed for {charCode})";
                        Console.WriteLine(abLine);
                        reportBuilder.AppendLine(abLine);
                    }
                }
                else
                {
                    Console.WriteLine("     None");
                    reportBuilder.AppendLine("     None");
                }

                // C. Outside / Non-Common Bones
                Console.WriteLine($"\n  C. OUTSIDE / NON-COMMON BONES ({nonCommonBones.Count} bones):");
                reportBuilder.AppendLine($"\n  C. OUTSIDE / NON-COMMON BONES ({nonCommonBones.Count} bones):");
                if (nonCommonBones.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (string ncb in nonCommonBones)
                    {
                        string ncbLine = $"     - [NON-COMMON] {ncb}";
                        Console.WriteLine(ncbLine);
                        reportBuilder.AppendLine(ncbLine);
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("     None [PASSED]");
                    Console.ResetColor();
                    reportBuilder.AppendLine("     None [PASSED]");
                }

                Console.WriteLine();
                reportBuilder.AppendLine();
            }

            string summaryHeader = "================================================================================\n" +
                                  "AUDIT SUMMARY REPORT\n" +
                                 $"Total Models Scanned: {targetGmdFiles.Count}\n" +
                                 $"PASSED Models:        {passedCount}\n" +
                                 $"FAILED Models:        {failedCount}\n" +
                                  "================================================================================";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(summaryHeader);
            Console.ResetColor();
            reportBuilder.AppendLine(summaryHeader);

            string dumpPath = SaveErrorDumpLog(reportBuilder.ToString(), "master_skin_bone_weight_audit_report.log");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("[INFO] Complete Categorical Audit Report saved to:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {dumpPath}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey(true);
        }

        private static void RunInteractiveCompile()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("VF5REVOWS Mod Compiler - Developed by Fai Khozen");
            Console.WriteLine("GitHub: github.com/faikhozen/VF5REVOWS_PXDArchiver_GatherToolset");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine();

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string modsDir = Path.Combine(currentDir, "mods");

            // Check mods folder
            if (!Directory.Exists(modsDir))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[INFO] Creating 'mods' folder at: {modsDir}");
                Console.ResetColor();
                try
                {
                    Directory.CreateDirectory(modsDir);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] Failed to create mods folder: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                    return;
                }
            }

            Console.WriteLine("Select mod type to compile:");
            Console.WriteLine("1. Character Skin mods (chara.par)");
            Console.WriteLine("2. Sound mods (vf5fs_data.par)");
            Console.WriteLine("3. Master Skin Mod (chara.par)");
            Console.Write("Choice [1/2/3]: ");
            string choice = Console.ReadLine()?.Trim();
            bool isSoundMod = choice == "2";
            bool isMasterSkinMod = choice == "3";

            int masterSkinSuboption = 1;

            if (isMasterSkinMod)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("================================================================================");
                Console.WriteLine("OPTION 3: MASTER SKIN MOD");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("NOTICE: Master skins default to common dummy armatures.");
                Console.WriteLine("Models WITH breast/chest physics (AOI & SAR) MUST be placed in 'master_skins/special_chest_physics/'.");
                Console.WriteLine("Models WITHOUT breast/chest physics stay in 'master_skins/compiled/' or 'master_skins/mods/'.");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("================================================================================");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Select Master Skin source mode:");
                Console.WriteLine("1. Master Skins Compiled (master_skins/compiled)");
                Console.WriteLine("   [INFO] Contains all textures in master_skins/compiled/chara/dds.");
                Console.WriteLine("   All characters are in each available character slot under");
                Console.WriteLine("   master_skins/compiled/chara/tops (one per character).");
                Console.WriteLine("   [CHEST PHYSICS] Models WITH breast/chest physics (AOI & SAR) MUST be in");
                Console.WriteLine("   'master_skins/special_chest_physics/'. Models WITHOUT physics stay in 'master_skins/compiled/'.");
                Console.WriteLine();
                Console.WriteLine("2. Master Skins Mods (master_skins/mods)");
                Console.WriteLine("   [INFO] Scans mod folders in master_skins/mods.");
                Console.WriteLine("   Gathers master skin .gmd models and .dds textures per mod folder.");
                Console.WriteLine("   [CHEST PHYSICS] Mod folders WITH breast/chest physics (AOI & SAR) MUST be in");
                Console.WriteLine("   'master_skins/special_chest_physics/'. Mod folders WITHOUT physics stay in 'master_skins/mods/'.");
                Console.WriteLine();
                Console.WriteLine("3. Common Dummy Bones Information & Strict Weight Guide");
                Console.WriteLine($"   [INFO] View the complete list of {WhitelistedCommonBones.Count} Whitelisted Common Dummy Bones, permitted");
                Console.WriteLine("   AOI/SAR breast physics bones, and rigging guidelines for cross-slot compatibility.");
                Console.WriteLine();
                Console.WriteLine("4. Master Skin Bone Weight Audit & Categorical Inspection Report");
                Console.WriteLine("   [INFO] Scans all master skin .gmd files across master_skins folders (compiled, mods, special_chest_physics).");
                Console.WriteLine("   Generates a comprehensive categorical audit report of vertex weights per model (common bones vs non-common bones),");
                Console.WriteLine("   and saves the audit report to './output/master_skin_bone_weight_audit_report.log'.");
                Console.WriteLine();
                Console.Write("Choice [1/2/3/4]: ");
                string subChoice = Console.ReadLine()?.Trim();

                if (subChoice == "1")
                {
                    masterSkinSuboption = 1;
                }
                else if (subChoice == "2")
                {
                    masterSkinSuboption = 2;
                }
                else if (subChoice == "3")
                {
                    ShowCommonBonesGuide();
                    return;
                }
                else if (subChoice == "4")
                {
                    RunMasterSkinBoneWeightAudit();
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Invalid choice selected.");
                    Console.ResetColor();
                    return;
                }
            }
            
            string expectedParName = isSoundMod ? "vf5fs_data.par" : "chara.par";
            string tempCopyName = isSoundMod ? "temp_vf5fs_data_copy.par" : "temp_chara_copy.par";
            string initialDir = isSoundMod ? @"\steamapps\common\VFREVO\runtime\media\vf5fs" : "";

            Console.WriteLine("\nPREREQUISITES:");
            if (isMasterSkinMod)
            {
                if (masterSkinSuboption == 2)
                {
                    Console.WriteLine(" - The './master_skins/mods/' folder contains mod folders WITHOUT breast/chest physics.");
                    Console.WriteLine("   Mod folders WITH breast/chest physics (AOI & SAR) MUST be placed in './master_skins/special_chest_physics/'.");
                    Console.WriteLine("   Both folders will be scanned for master skin .gmd and .dds files.");
                }
                else
                {
                    Console.WriteLine(" - The './master_skins/compiled/' folder contains models WITHOUT breast/chest physics.");
                    Console.WriteLine("   Models WITH breast/chest physics (AOI & SAR) MUST be placed in './master_skins/special_chest_physics/'.");
                    Console.WriteLine("   Contains all textures in master_skins/compiled/chara/dds and character slots under");
                    Console.WriteLine("   master_skins/compiled/chara/tops (one per character).");
                }
            }
            else
            {
                Console.WriteLine(" - The './mods' folder must exist in the same folder as this program.");
            }
            Console.WriteLine($" - An original '{expectedParName}' reference file is required.");
            Console.WriteLine();
            Console.WriteLine("GAME DIRECTORY REFERENCE:");
            Console.WriteLine(" - VFREVO.exe is usually located at:");
            Console.WriteLine(@"     {steam_directory}\steamapps\common\VFREVO\runtime\media\VFREVO.exe");
            if (isSoundMod)
            {
                Console.WriteLine($" - The original {expectedParName} is located inside:");
                Console.WriteLine(@"     {steam_directory}\steamapps\common\VFREVO\runtime\media\vf5fs\vf5fs_data.par");
            }
            else
            {
                Console.WriteLine($" - The original {expectedParName} is located inside:");
                Console.WriteLine(@"     {steam_directory}\steamapps\common\VFREVO\runtime\media\data\chara.par");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.Write($"Press any key to select your reference '{expectedParName}' file...");
            Console.ReadKey(true);
            Console.WriteLine("\n");

            string selectedParPath = SelectPar(expectedParName, initialDir);
            if (string.IsNullOrEmpty(selectedParPath) || !File.Exists(selectedParPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: No valid '{expectedParName}' file was selected.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Selected reference PAR: {selectedParPath}\n");

            // Compute free space and copy locally if we have >= 15GB
            string parInputPath = selectedParPath;
            bool isLocalCopyCreated = false;
            string localParPath = Path.Combine(currentDir, tempCopyName);

            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(currentDir));
                long freeSpaceBytes = drive.AvailableFreeSpace;
                long requiredSpaceBytes = 15L * 1024L * 1024L * 1024L; // 15 GB

                Console.WriteLine("Checking disk space for local copy...");
                Console.WriteLine($"Available free space: {freeSpaceBytes / (1024.0 * 1024.0 * 1024.0):F2} GB");

                if (freeSpaceBytes >= requiredSpaceBytes)
                {
                    Console.WriteLine($"Space check PASSED (>= 15 GB free). Copying {expectedParName} locally for faster processing...");
                    Console.Write("Copying... ");
                    File.Copy(selectedParPath, localParPath, true);
                    Console.WriteLine("DONE!");
                    parInputPath = localParPath;
                    isLocalCopyCreated = true;
                }
                else
                {
                    Console.WriteLine("Space check SKIPPED (< 15 GB free). Processing directly from original path.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: Disk space check or local copy failed: {ex.Message}");
                Console.WriteLine("Processing directly from original path.");
                Console.ResetColor();
            }

            // We compile it into ./output/expectedParName
            string outputPar = Path.Combine(currentDir, "output", expectedParName);

            // Setup options for compile
            var opts = new Options.GatherCompile
            {
                InputParArchivePath = parInputPath,
                ModsDirectory = modsDir,
                OutputParArchivePath = outputPar,
                Compression = 0, // Disable compression (0 = uncompressed) for 100x faster compilation
                IsSoundMod = isSoundMod,
                IsMasterSkinMod = isMasterSkinMod,
                MasterSkinSuboption = masterSkinSuboption
            };

            bool success = false;
            try
            {
                // Run compiler (skipping reference PAR backup copy as requested)
                success = RunGatherCompileInternal(opts, skipBackup: true);
            }
            finally
            {
                // Clean up the local copied chara.par if it was created
                if (isLocalCopyCreated && File.Exists(localParPath))
                {
                    Console.Write("Cleaning up temporary local copy... ");
                    try
                    {
                        File.Delete(localParPath);
                        Console.WriteLine("DONE!");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"WARNING: Could not delete temporary copy: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            if (!success)
            {
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("MOD COMPILATION COMPLETE!");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Output location:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {outputPar}");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("INSTRUCTIONS:");
            Console.ResetColor();
            Console.WriteLine(" 1. Go to your game directory:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string gameParDir = Path.GetDirectoryName(selectedParPath) ?? string.Empty;
            Console.WriteLine($"    {gameParDir}");
            Console.ResetColor();
            
            string parFileName = isSoundMod ? "vf5fs_data.par" : "chara.par";
            string parBaseName = Path.GetFileNameWithoutExtension(parFileName);
            Console.WriteLine($" 2. BACK UP the original '{parFileName}' (e.g. rename it to '{parBaseName}_original.par').");
            Console.WriteLine($" 3. Copy the newly compiled '{parFileName}' from:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"    {outputPar}");
            Console.ResetColor();
            Console.WriteLine("    into the game directory:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"    {gameParDir}");
            Console.ResetColor();

            string outputDir = Path.GetDirectoryName(outputPar) ?? string.Empty;
            string authVoiceDir = Path.Combine(outputDir, "auth_voice");
            if (isSoundMod && Directory.Exists(authVoiceDir) && Directory.GetFiles(authVoiceDir).Length > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=================== [IMPORTANT INSTRUCTIONS FOR AUTH_VOICE] ===================");
                Console.WriteLine("Some sound mod files were copied to the local output folder instead of the PAR:");
                Console.WriteLine($" -> {authVoiceDir}");
                Console.WriteLine();
                Console.WriteLine("Please follow these steps to apply them to your game:");
                Console.WriteLine("1. Create a BACKUP copy of your original 'auth_voice' folder located at:");
                Console.WriteLine($@"   {Path.Combine(gameParDir, "vf5fs_media", "rom", "sound", "voice", "auth_voice")}");
                Console.WriteLine("2. Copy all the files from the newly created folder:");
                Console.WriteLine($"   {authVoiceDir}");
                Console.WriteLine("3. Paste and overwrite them into the game's folder:");
                Console.WriteLine($@"   {Path.Combine(gameParDir, "vf5fs_media", "rom", "sound", "voice", "auth_voice")}");
                Console.WriteLine("================================================================================");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private static string SelectPar(string expectedName, string initialDir)
        {
            try
            {
                // Run PowerShell to display file dialog
                string script = "[System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms') | Out-Null; " +
                                "$dialog = New-Object System.Windows.Forms.OpenFileDialog; " +
                                $"$dialog.Title = 'Select Original {expectedName}'; " +
                                $"$dialog.Filter = '{expectedName}|{expectedName}|PAR files (*.par)|*.par|All files (*.*)|*.*'; " +
                                $"$dialog.FileName = '{expectedName}'; ";
                
                if (!string.IsNullOrEmpty(initialDir))
                {
                    script += $"$dialog.InitialDirectory = '{initialDir}'; ";
                }
                
                script += "if ($dialog.ShowDialog() -eq 'OK') { Write-Output $dialog.FileName }";

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(output) && File.Exists(output))
                    {
                        return output;
                    }
                }
            }
            catch
            {
                // Fallback to text prompt
            }

            // Fallback to text prompt
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Could not open graphical file dialog. Please paste the path manually.");
            Console.ResetColor();
            while (true)
            {
                Console.Write($"Enter absolute path to original '{expectedName}': ");
                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    return null;
                }
                if (input.StartsWith("\"") && input.EndsWith("\"") || input.StartsWith("'") && input.EndsWith("'"))
                {
                    input = input.Substring(1, input.Length - 2);
                }
                if (File.Exists(input))
                {
                    return Path.GetFullPath(input);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File does not exist. Please try again or press Enter to cancel.");
                Console.ResetColor();
            }
        }

        private static string SaveErrorDumpLog(string logContent, string filename = "master_skin_error_dump.log")
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string outputDir = Path.Combine(currentDir, "output");
            try { Directory.CreateDirectory(outputDir); } catch { }

            string logPath = Path.Combine(outputDir, filename);
            try
            {
                File.WriteAllText(logPath, logContent);
            }
            catch
            {
                logPath = Path.Combine(currentDir, filename);
                try { File.WriteAllText(logPath, logContent); } catch { }
            }
            return logPath;
        }

        private static void AddFileToVirtualNode(Node container, string filePath, string relPath)
        {
            string[] parts = relPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            Node current = container;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                string dirName = parts[i];
                Node child = current.Children[dirName];
                if (child == null)
                {
                    child = Yarhl.FileSystem.NodeFactory.CreateContainer(dirName);
                    current.Add(child);
                }
                current = child;
            }

            string fileName = parts[parts.Length - 1];
            Node existing = current.Children[fileName];
            if (existing != null)
            {
                current.Remove(fileName);
                existing.Dispose();
            }

            Node fileNode = Yarhl.FileSystem.NodeFactory.FromFile(filePath, Yarhl.IO.FileOpenMode.Read);
            fileNode.Name = fileName;
            current.Add(fileNode);
        }

        private static void AddToHistory(Dictionary<string, List<GmdEntry>> history, string gmdFilePath, string relPath, string modName)
        {
            string filename = Path.GetFileName(gmdFilePath);
            if (!history.ContainsKey(filename))
            {
                history[filename] = new List<GmdEntry>();
            }
            history[filename].Add(new GmdEntry { FilePath = relPath, ModName = modName });
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }

        private static void CheckConflicts(Dictionary<string, List<GmdEntry>> gmdHistory)
        {
            var conflicts = new Dictionary<string, List<GmdEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in gmdHistory)
            {
                var mods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in kvp.Value)
                {
                    mods.Add(entry.ModName);
                }

                if (mods.Count > 1)
                {
                    conflicts[kvp.Key] = kvp.Value;
                }
            }

            if (conflicts.Count > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("========== GMD CONFLICTS DETECTED ==========");
                Console.WriteLine("The following files will overwrite each other:");
                Console.WriteLine();

                foreach (var kvp in conflicts)
                {
                    string filename = kvp.Key;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File: {filename}");
                    Console.WriteLine("  Found in mods:");

                    var list = kvp.Value;
                    string lastMod = list[list.Count - 1].ModName;

                    var seenMods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var entry in list)
                    {
                        if (seenMods.Add(entry.ModName))
                        {
                            if (string.Equals(entry.ModName, lastMod, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine($"    - CURRENTLY SET AS: {entry.ModName} ({entry.FilePath})");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"    - {entry.ModName} ({entry.FilePath})");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("============================================\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"All {gmdHistory.Count} .gmd files are unique - no conflicts detected.\n");
                Console.ResetColor();
            }
        }

        private static int totalFilesToCompress = 0;
        private static int compressedFilesCount = 0;

        private static void OnFileCompressing(Node sender)
        {
            compressedFilesCount++;
            DrawProgressBar(compressedFilesCount, totalFilesToCompress);
        }

        private static void DrawProgressBar(int current, int total)
        {
            int percentage = total > 0 ? (int)((double)current / total * 100) : 0;
            if (percentage > 100) percentage = 100;

            int barWidth = 40;
            int completedWidth = total > 0 ? (int)((double)current / total * barWidth) : 0;
            if (completedWidth > barWidth) completedWidth = barWidth;

            string bar = new string('█', completedWidth) + new string('░', barWidth - completedWidth);
            Console.Write($"\rProgress: [{bar}] {percentage}% ({current}/{total})");
        }

        private static int CountFiles(Node node)
        {
            if (node.Children == null || node.Children.Count == 0)
            {
                return 1;
            }
            int count = 0;
            foreach (Node child in node.Children)
            {
                if (child.Children != null && child.Children.Count > 0)
                {
                    count += CountFiles(child);
                }
                else
                {
                    count++;
                }
            }
            return count;
        }
        private static Node FindDestinationNode(Node root)
        {
            Node dest = root;
            while (dest.Children != null && dest.Children["."] != null)
            {
                dest = dest.Children["."];
            }
            return dest;
        }

        private static readonly string[] CharacterCodes = new[]
        {
            "AKI", "SAR", "LAU", "SHU", "JEF", "PAI", "JAK", "KAG", "LIO", "WOL",
            "AOI", "LEI", "VAN", "BRA", "GOH", "MON", "MSK", "KRT", "TAK", "TST", "DUR"
        };

        private static readonly Dictionary<string, string> CodeToCharacterName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AKI", "Akira Yuki" },
            { "SAR", "Sarah Bryant" },
            { "LAU", "Lau Chan" },
            { "SHU", "Shun Di" },
            { "JEF", "Jeffry McWild" },
            { "PAI", "Pai Chan" },
            { "JAK", "Jacky Bryant" },
            { "KAG", "Kage-Maru" },
            { "LIO", "Lion Rafale" },
            { "WOL", "Wolf Hawkfield" },
            { "AOI", "Aoi Umenokouji" },
            { "LEI", "Lei-Fei" },
            { "VAN", "Vanessa Lewis" },
            { "BRA", "Brad Burns" },
            { "GOH", "Goh Hinogami" },
            { "MON", "El Blaze" },
            { "MSK", "Eileen" },
            { "KRT", "Jean Kujo" },
            { "TAK", "Taka-Arashi" },
            { "TST", "Test / Dev Slot" },
            { "DUR", "Dural" }
        };

        private static readonly Dictionary<string, string> CodeToDummyBoneFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AKI", "c_v00_VF5_AKI_bone.gmd" },
            { "SAR", "c_v01_VF5_SAR_bone.gmd" },
            { "LAU", "c_v02_VF5_LAU_bone.gmd" },
            { "SHU", "c_v03_VF5_SHU_bone.gmd" },
            { "JEF", "c_v04_VF5_JEF_bone.gmd" },
            { "PAI", "c_v05_VF5_PAI_bone.gmd" },
            { "JAK", "c_v06_VF5_JAK_bone.gmd" },
            { "KAG", "c_v07_VF5_KAG_bone.gmd" },
            { "LIO", "c_v08_VF5_LIO_bone.gmd" },
            { "WOL", "c_v09_VF5_WOL_bone.gmd" },
            { "AOI", "c_v10_VF5_AOI_bone.gmd" },
            { "LEI", "c_v11_VF5_LEI_bone.gmd" },
            { "VAN", "c_v12_VF5_VAN_bone.gmd" },
            { "BRA", "c_v13_VF5_BRA_bone.gmd" },
            { "GOH", "c_v14_VF5_GOH_bone.gmd" },
            { "MON", "c_v15_VF5_MON_bone.gmd" },
            { "MSK", "c_v16_VF5_MSK_bone.gmd" },
            { "KRT", "c_v17_VF5_KRT_bone.gmd" },
            { "TAK", "c_v18_VF5_TAK_bone.gmd" },
            { "TST", "c_v19_VF5_TST_bone.gmd" },
            { "DUR", "c_v20_VF5_DUR_bone.gmd" }
        };

        private static readonly (string CharCode, string VirtualPath)[] SpecialItemMappings = new[]
        {
            ("LAU", "vf5item/LAU/LAUITM452/LAU452_AT_MET_01.gmd"),
            ("LEI", "vf5item/LEI/LEIITM304/LEI304_AT_ATAMA_22.gmd"),
            ("MSK", "vf5item/MSK/MSKITM415/MSK415_AT_MASK_10.gmd"),
            ("GOH", "vf5item/GOH/GOHITM416/GOH416_AT_MASK_01.gmd")
        };

        private static string ExtractEmbeddedDummyGmdsToTemp()
        {
            try
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), "VF5REVOWS_dummy_gmds");
                Directory.CreateDirectory(tempFolder);

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string[] resourceNames = assembly.GetManifestResourceNames();

                int extractedCount = 0;
                foreach (string resName in resourceNames)
                {
                    if (resName.EndsWith(".gmd", StringComparison.OrdinalIgnoreCase) && resName.Contains("c_v"))
                    {
                        string fileName = resName;
                        int lastDotIndex = resName.LastIndexOf(".gmd", StringComparison.OrdinalIgnoreCase);
                        int secondLastDotIndex = resName.LastIndexOf('.', lastDotIndex - 1);
                        if (secondLastDotIndex >= 0)
                        {
                            fileName = resName.Substring(secondLastDotIndex + 1);
                        }

                        string destPath = Path.Combine(tempFolder, fileName);
                        using (Stream resStream = assembly.GetManifestResourceStream(resName))
                        {
                            if (resStream != null)
                            {
                                using (FileStream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                                {
                                    resStream.CopyTo(fs);
                                }
                                extractedCount++;
                            }
                        }
                    }
                }
                if (extractedCount > 0)
                {
                    Console.WriteLine($"Extracted {extractedCount} embedded dummy GMD templates to temp directory.");
                    return tempFolder;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: Failed to extract embedded dummy GMD resources: {ex.Message}");
                Console.ResetColor();
            }
            return null;
        }

        private static readonly HashSet<string> WhitelistedCostumeFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "c_v00_VF5_AKI", "c_v01_VF5_SAR", "c_v02_VF5_LAU", "c_v03_VF5_SHU", "c_v04_VF5_JEF",
            "c_v05_VF5_PAI", "c_v06_VF5_JAK", "c_v07_VF5_KAG", "c_v08_VF5_LIO", "c_v8a_VF5_JAK_SWIM",
            "c_v8b_VF5_KAG_SWIM", "c_v8c_VF5_LIO_SWIM", "c_v8d_VF5_WOL_SWIM", "c_v8e_VF5_AOI_SWIM", "c_v8f_VF5_LEI_SWIM",
            "c_v09_VF5_WOL", "c_v10_VF5_AOI", "c_v11_VF5_LEI", "c_v12_VF5_VAN", "c_v13_VF5_BRA",
            "c_v14_VF5_GOH", "c_v15_VF5_MON", "c_v16_VF5_MSK", "c_v17_VF5_KRT", "c_v18_VF5_TAK",
            "c_v19_VF5_TST", "c_v20_VF5_DUR", "c_v21_VF5_AKI_VF1", "c_v21_VF5_AKI_VF1_2", "c_v22_VF5_SAR_VF1",
            "c_v22_VF5_SAR_VF1_2", "c_v23_VF5_LAU_VF1", "c_v23_VF5_LAU_VF1_2", "c_v24_VF5_SHU_VF1", "c_v24_VF5_SHU_VF1_2",
            "c_v25_VF5_JEF_VF1", "c_v25_VF5_JEF_VF1_2", "c_v26_VF5_PAI_VF1", "c_v26_VF5_PAI_VF1_2", "c_v27_VF5_JAK_VF1",
            "c_v27_VF5_JAK_VF1_2", "c_v28_VF5_KAG_VF1", "c_v28_VF5_KAG_VF1_2", "c_v29_VF5_LIO_VF1", "c_v29_VF5_LIO_VF1_2",
            "c_v30_VF5_WOL_VF1", "c_v30_VF5_WOL_VF1_2", "c_v31_VF5_AOI_VF1", "c_v31_VF5_AOI_VF1_2", "c_v32_VF5_LEI_VF1",
            "c_v32_VF5_LEI_VF1_2", "c_v33_VF5_VAN_VF1", "c_v33_VF5_VAN_VF1_2", "c_v34_VF5_BRA_VF1", "c_v34_VF5_BRA_VF1_2",
            "c_v35_VF5_GOH_VF1", "c_v35_VF5_GOH_VF1_2", "c_v36_VF5_MON_VF1", "c_v36_VF5_MON_VF1_2", "c_v37_VF5_MSK_VF1",
            "c_v37_VF5_MSK_VF1_2", "c_v38_VF5_KRT_VF1", "c_v38_VF5_KRT_VF1_2", "c_v39_VF5_TAK_VF1", "c_v39_VF5_TAK_VF1_2",
            "c_v40_VF5_TST_VF1", "c_v40_VF5_TST_VF1_2", "c_v41_VF5_DUR_VF1", "c_v41_VF5_DUR_VF1_2", "c_v42_VF5_AKI_RYU",
            "c_v43_VF5_SAR_RYU", "c_v44_VF5_LAU_RYU", "c_v45_vf5_shu_ryu", "c_v46_VF5_JEF_RYU", "c_v47_vf5_pai_ryu",
            "c_v48_VF5_JAK_RYU", "c_v49_vf5_kag_ryu", "c_v50_VF5_LIO_RYU", "c_v51_VF5_WOL_RYU", "c_v52_VF5_AOI_RYU",
            "c_v53_VF5_LEI_RYU", "c_v54_VF5_VAN_RYU", "c_v55_VF5_BRA_RYU", "c_v56_VF5_GOH_RYU", "c_v57_VF5_MON_RYU",
            "c_v58_VF5_MSK_RYU", "c_v59_VF5_KRT_RYU", "c_v60_VF5_TAK_RYU", "c_v61_VF5_TST_RYU", "c_v62_VF5_DUR_RYU",
            "c_v63_VF5_AKI_TEK", "c_v64_VF5_SAR_TEK", "c_v65_VF5_LAU_TEK", "c_v66_VF5_SHU_TEK", "c_v67_VF5_JEF_TEK",
            "c_v68_VF5_PAI_TEK", "c_v69_VF5_JAK_TEK", "c_v70_VF5_KAG_TEK", "c_v71_VF5_LIO_TEK", "c_v72_VF5_WOL_TEK",
            "c_v73_VF5_AOI_TEK", "c_v74_VF5_LEI_TEK", "c_v75_VF5_VAN_TEK", "c_v76_VF5_BRA_TEK", "c_v77_VF5_GOH_TEK",
            "c_v78_VF5_MON_TEK", "c_v79_VF5_MSK_TEK", "c_v80_VF5_KRT_TEK", "c_v81_VF5_TAK_TEK", "c_v82_VF5_TST_TEK",
            "c_v83_VF5_DUR_TEK", "c_v84_VF5_AKI_SWIM", "c_v85_VF5_SAR_SWIM", "c_v86_VF5_LAU_SWIM", "c_v87_VF5_SHU_SWIM",
            "c_v88_VF5_JEF_SWIM", "c_v89_VF5_PAI_SWIM", "c_v90_VF5_VAN_SWIM", "c_v91_VF5_BRA_SWIM", "c_v92_VF5_GOH_SWIM",
            "c_v93_VF5_MON_SWIM", "c_v94_VF5_MSK_SWIM", "c_v95_VF5_KRT_SWIM", "c_v96_VF5_TAK_SWIM", "c_v97_VF5_TST_SWIM",
            "c_v98_VF5_DUR_SWIM"
        };

        private static readonly Dictionary<string, string> CodeToBaseFolder = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AKI", "c_v00_VF5_AKI" },
            { "SAR", "c_v01_VF5_SAR" },
            { "LAU", "c_v02_VF5_LAU" },
            { "SHU", "c_v03_VF5_SHU" },
            { "JEF", "c_v04_VF5_JEF" },
            { "PAI", "c_v05_VF5_PAI" },
            { "JAK", "c_v06_VF5_JAK" },
            { "KAG", "c_v07_VF5_KAG" },
            { "LIO", "c_v08_VF5_LIO" },
            { "WOL", "c_v09_VF5_WOL" },
            { "AOI", "c_v10_VF5_AOI" },
            { "LEI", "c_v11_VF5_LEI" },
            { "VAN", "c_v12_VF5_VAN" },
            { "BRA", "c_v13_VF5_BRA" },
            { "GOH", "c_v14_VF5_GOH" },
            { "MON", "c_v15_VF5_MON" },
            { "MSK", "c_v16_VF5_MSK" },
            { "KRT", "c_v17_VF5_KRT" },
            { "TAK", "c_v18_VF5_TAK" },
            { "TST", "c_v19_VF5_TST" },
            { "DUR", "c_v20_VF5_DUR" }
        };

        private static readonly Dictionary<string, string> CodeToBaseCostumeSlot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AKI", "tops/c_v00_VF5_AKI/c_v00_VF5_AKI.gmd" },
            { "SAR", "tops/c_v01_VF5_SAR/c_v01_VF5_SAR.gmd" },
            { "LAU", "tops/c_v02_VF5_LAU/c_v02_VF5_LAU.gmd" },
            { "SHU", "tops/c_v03_VF5_SHU/c_v03_VF5_SHU.gmd" },
            { "JEF", "tops/c_v04_VF5_JEF/c_v04_VF5_JEF.gmd" },
            { "PAI", "tops/c_v05_VF5_PAI/c_v05_VF5_PAI.gmd" },
            { "JAK", "tops/c_v06_VF5_JAK/c_v06_VF5_JAK.gmd" },
            { "KAG", "tops/c_v07_VF5_KAG/c_v07_VF5_KAG.gmd" },
            { "LIO", "tops/c_v08_VF5_LIO/c_v08_VF5_LIO.gmd" },
            { "WOL", "tops/c_v09_VF5_WOL/c_v09_VF5_WOL.gmd" },
            { "AOI", "tops/c_v10_VF5_AOI/c_v10_VF5_AOI.gmd" },
            { "LEI", "tops/c_v11_VF5_LEI/c_v11_VF5_LEI.gmd" },
            { "VAN", "tops/c_v12_VF5_VAN/c_v12_VF5_VAN.gmd" },
            { "BRA", "tops/c_v13_VF5_BRA/c_v13_VF5_BRA.gmd" },
            { "GOH", "tops/c_v14_VF5_GOH/c_v14_VF5_GOH.gmd" },
            { "MON", "tops/c_v15_VF5_MON/c_v15_VF5_MON.gmd" },
            { "MSK", "tops/c_v16_VF5_MSK/c_v16_VF5_MSK.gmd" },
            { "KRT", "tops/c_v17_VF5_KRT/c_v17_VF5_KRT.gmd" },
            { "TAK", "tops/c_v18_VF5_TAK/c_v18_VF5_TAK.gmd" },
            { "TST", "tops/c_v19_VF5_TST/c_v19_VF5_TST.gmd" },
            { "DUR", "tops/c_v20_VF5_DUR/c_v20_VF5_DUR.gmd" }
        };

        private static string MatchCharacterCode(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            string[] parts = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string upper = part.ToUpperInvariant();
                foreach (string code in CharacterCodes)
                {
                    string cUpper = code.ToUpperInvariant();
                    if (upper.Contains($"_{cUpper}_") || upper.EndsWith($"_{cUpper}") || upper.StartsWith($"{cUpper}_") || upper == cUpper)
                    {
                        return code;
                    }
                }
            }
            return null;
        }
        private static void TraverseAndCollectVirtualTopsFolders(Node node, string currentPath, Dictionary<string, HashSet<string>> characterCostumeFolders)
        {
            if (node.Children == null) return;
            foreach (Node child in node.Children)
            {
                string path = string.IsNullOrEmpty(currentPath) ? child.Name : $"{currentPath}/{child.Name}";
                if (child.IsContainer)
                {
                    if (path.StartsWith("tops/", StringComparison.OrdinalIgnoreCase))
                    {
                        string folderName = child.Name;
                        string charCode = MatchCharacterCode(folderName);
                        if (!string.IsNullOrEmpty(charCode))
                        {
                            if (!characterCostumeFolders.ContainsKey(charCode))
                            {
                                characterCostumeFolders[charCode] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            }
                            characterCostumeFolders[charCode].Add(folderName);
                        }
                    }
                    TraverseAndCollectVirtualTopsFolders(child, path, characterCostumeFolders);
                }
            }
        }

        private static void TraverseAndCollectVirtualGmds(Node node, string currentPath, List<(string VirtualPath, string CharCode)> list)
        {
            if (node.Children == null) return;
            foreach (Node child in node.Children)
            {
                string path = string.IsNullOrEmpty(currentPath) ? child.Name : $"{currentPath}/{child.Name}";
                if (child.IsContainer)
                {
                    TraverseAndCollectVirtualGmds(child, path, list);
                }
                else if (child.Name.EndsWith(".gmd", StringComparison.OrdinalIgnoreCase))
                {
                    if (path.StartsWith("vf5item/", StringComparison.OrdinalIgnoreCase) || path.StartsWith("tops/", StringComparison.OrdinalIgnoreCase))
                    {
                        string charCode = MatchCharacterCode(path);
                        if (!string.IsNullOrEmpty(charCode))
                        {
                            list.Add((path, charCode));
                        }
                    }
                }
            }
        }
    }
}
