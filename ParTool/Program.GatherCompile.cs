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

        private static void RunGatherCompileInternal(Options.GatherCompile opts, bool skipBackup)
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
                            return;
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
                return;
            }

            Console.WriteLine("Step 1/2: Scanning mods and mapping overlay files...");
            var filesToOverlay = new List<(string PhysicalPath, string VirtualPath)>();
            var gmdHistory = new Dictionary<string, List<GmdEntry>>(StringComparer.OrdinalIgnoreCase);

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
            Console.WriteLine($"Scanning complete. Found {filesToOverlay.Count} files to overlay directly into PAR.\n");

            // 7. Perform compiling & merging (in-memory overlay)
            string outputDir = Path.GetDirectoryName(outputPar);
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
                    return;
                }
            }

            var readerParameters = new ParArchiveReaderParameters
            {
                Recursive = true,
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

                Console.Write("Building virtual mod overlay... ");
                node = Yarhl.FileSystem.NodeFactory.CreateContainer("mods_overlay");
                foreach (var overlay in filesToOverlay)
                {
                    AddFileToVirtualNode(node, overlay.PhysicalPath, overlay.VirtualPath);
                }
                Console.WriteLine("DONE!");

                Console.Write("Adding files... ");
                Node destinationNode = FindDestinationNode(par);
                node.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(destinationNode, true);
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
            Console.ResetColor();
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

            Console.WriteLine("PREREQUISITES:");
            Console.WriteLine(" - The './mods' folder must exist in the same folder as this program.");
            Console.WriteLine(" - An original 'chara.par' reference file is required (you can also select backups like 'chara_backup.par', 'chara_original.par', or 'chara__bak.par').");
            Console.WriteLine();
            Console.WriteLine("GAME DIRECTORY REFERENCE:");
            Console.WriteLine(" - VFREVO.exe is usually located at:");
            Console.WriteLine(@"     {steam_directory}\steamapps\common\VFREVO\runtime\media\VFREVO.exe");
            Console.WriteLine(" - The original chara.par is located inside:");
            Console.WriteLine(@"     {steam_directory}\steamapps\common\VFREVO\runtime\media\data\chara.par");
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.Write("Press any key to select your reference 'chara.par' (original or backup) file...");
            Console.ReadKey(true);
            Console.WriteLine("\n");

            string selectedParPath = SelectCharaPar();
            if (string.IsNullOrEmpty(selectedParPath) || !File.Exists(selectedParPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: No valid 'chara.par' file was selected.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Selected reference PAR: {selectedParPath}\n");

            // Compute free space and copy locally if we have >= 15GB
            string parInputPath = selectedParPath;
            bool isLocalCopyCreated = false;
            string localParPath = Path.Combine(currentDir, "temp_chara_copy.par");

            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(currentDir));
                long freeSpaceBytes = drive.AvailableFreeSpace;
                long requiredSpaceBytes = 15L * 1024L * 1024L * 1024L; // 15 GB

                Console.WriteLine("Checking disk space for local copy...");
                Console.WriteLine($"Available free space: {freeSpaceBytes / (1024.0 * 1024.0 * 1024.0):F2} GB");

                if (freeSpaceBytes >= requiredSpaceBytes)
                {
                    Console.WriteLine("Space check PASSED (>= 15 GB free). Copying chara.par locally for faster processing...");
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

            // We compile it into ./output/chara.par
            string outputPar = Path.Combine(currentDir, "output", "chara.par");

            // Setup options for compile
            var opts = new Options.GatherCompile
            {
                InputParArchivePath = parInputPath,
                ModsDirectory = modsDir,
                OutputParArchivePath = outputPar,
                Compression = 0 // Disable compression (0 = uncompressed) for 100x faster compilation
            };

            try
            {
                // Run compiler (skipping reference PAR backup copy as requested)
                RunGatherCompileInternal(opts, skipBackup: true);
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
            string gameParDir = Path.GetDirectoryName(selectedParPath);
            Console.WriteLine($"    {gameParDir}");
            Console.ResetColor();
            Console.WriteLine(" 2. BACK UP the original 'chara.par' (e.g. rename it to 'chara_original.par').");
            Console.WriteLine(" 3. Copy the newly compiled 'chara.par' from:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"    {outputPar}");
            Console.ResetColor();
            Console.WriteLine("    into the game directory:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"    {gameParDir}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private static string SelectCharaPar()
        {
            try
            {
                // Run PowerShell to display file dialog
                string script = "[System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms') | Out-Null; " +
                                "$dialog = New-Object System.Windows.Forms.OpenFileDialog; " +
                                "$dialog.Title = 'Select Original chara.par'; " +
                                "$dialog.Filter = 'chara.par|chara.par|PAR files (*.par)|*.par|All files (*.*)|*.*'; " +
                                "$dialog.FileName = 'chara.par'; " +
                                "if ($dialog.ShowDialog() -eq 'OK') { Write-Output $dialog.FileName }";

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
                Console.Write("Enter absolute path to original 'chara.par': ");
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
            while (dest.Children.Count > 0 && dest.Children[0].Name == ".")
            {
                dest = dest.Children[0];
            }
            return dest;
        }
    }
}
