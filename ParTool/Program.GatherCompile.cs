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
            WriteHeader();

            // 1. Resolve Reference PAR and Backup Path
            string refPar = Path.GetFullPath(opts.InputParArchivePath);
            string parDir = Path.GetDirectoryName(refPar);
            string backupPath = Path.Combine(parDir, "chara__bak.par");

            // If the original refPar exists, we make sure backupPath exists
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

            // 3. Resolve Directories
            string modsDir = Path.GetFullPath(opts.ModsDirectory);
            string allDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "__all");
            string outputPar = Path.GetFullPath(opts.OutputParArchivePath);

            if (!Directory.Exists(modsDir))
            {
                Console.WriteLine($"Creating mods directory at: {modsDir}");
                Directory.CreateDirectory(modsDir);
                Console.WriteLine("Please place your mod folders inside the 'mods' directory, then run this tool again.");
                return;
            }

            // 4. Clean and Setup destination folders inside __all
            Console.WriteLine("Step 1/2: Gathering files from mods...");
            try
            {
                if (Directory.Exists(allDir))
                {
                    Console.WriteLine($"Cleaning up {allDir}...");
                    Directory.Delete(allDir, true);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Failed to clean __all directory: {ex.Message}");
                Console.ResetColor();
                return;
            }

            Directory.CreateDirectory(Path.Combine(allDir, "chara", "bone"));
            Directory.CreateDirectory(Path.Combine(allDir, "chara", "dds"));
            Directory.CreateDirectory(Path.Combine(allDir, "chara", "tops"));
            Directory.CreateDirectory(Path.Combine(allDir, "chara", "vf5item"));

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

                // Copy chara/bone
                string boneSrc = Path.Combine(modPath, "chara", "bone");
                if (Directory.Exists(boneSrc))
                {
                    string boneDst = Path.Combine(allDir, "chara", "bone");
                    foreach (string file in Directory.GetFiles(boneSrc))
                    {
                        File.Copy(file, Path.Combine(boneDst, Path.GetFileName(file)), true);
                    }
                }

                // Copy chara/dds
                string ddsSrc = Path.Combine(modPath, "chara", "dds");
                if (Directory.Exists(ddsSrc))
                {
                    string ddsDst = Path.Combine(allDir, "chara", "dds");
                    foreach (string file in Directory.GetFiles(ddsSrc))
                    {
                        File.Copy(file, Path.Combine(ddsDst, Path.GetFileName(file)), true);
                    }
                }

                // Copy chara/dds_append to chara/dds
                string ddsAppSrc = Path.Combine(modPath, "chara", "dds_append");
                if (Directory.Exists(ddsAppSrc))
                {
                    string ddsDst = Path.Combine(allDir, "chara", "dds");
                    foreach (string file in Directory.GetFiles(ddsAppSrc))
                    {
                        File.Copy(file, Path.Combine(ddsDst, Path.GetFileName(file)), true);
                    }
                }

                // Copy chara/vf5item
                string vf5itemSrc = Path.Combine(modPath, "chara", "vf5item");
                if (Directory.Exists(vf5itemSrc))
                {
                    string vf5itemDst = Path.Combine(allDir, "chara", "vf5item");
                    CopyDirectory(vf5itemSrc, vf5itemDst);

                    // Scan for .gmd files recursively
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

                // Copy chara/tops
                string topsSrc = Path.Combine(modPath, "chara", "tops");
                if (Directory.Exists(topsSrc))
                {
                    // Scan directly inside tops
                    foreach (string gmdFile in Directory.GetFiles(topsSrc, "*.gmd"))
                    {
                        hasGmdFiles = true;
                        Console.WriteLine($"Gathering from: {modName} || /{Path.GetFileName(gmdFile)}");
                    }

                    // Scan and copy immediate subdirectories
                    foreach (string subDir in Directory.GetDirectories(topsSrc))
                    {
                        string subDirName = Path.GetFileName(subDir);
                        foreach (string gmdFile in Directory.GetFiles(subDir, "*.gmd"))
                        {
                            hasGmdFiles = true;
                            string filenameNoExt = Path.GetFileNameWithoutExtension(gmdFile);
                            string relPath = $"{subDirName}/{Path.GetFileName(gmdFile)}";
                            Console.WriteLine($"Gathering from: {modName} || /{relPath}");

                            if (string.Equals(subDirName, filenameNoExt, StringComparison.OrdinalIgnoreCase))
                            {
                                AddToHistory(gmdHistory, gmdFile, $"/{relPath}", modName);
                            }
                        }

                        // Copy the entire subdirectory tree
                        string destSubDir = Path.Combine(allDir, "chara", "tops", subDirName);
                        CopyDirectory(subDir, destSubDir);
                    }
                }

                if (!hasGmdFiles)
                {
                    Console.WriteLine($"Gathering from: {modName}");
                }
            }

            // 6. Check conflicts
            CheckConflicts(gmdHistory);
            Console.WriteLine("Gathering complete.\n");

            // 7. Perform compiling & merging (identical to ParTool add workflow)
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

            string tempParPath = Path.Combine(Path.GetDirectoryName(outputPar), "temp_add_data.par");
            if (File.Exists(tempParPath))
            {
                try { File.Delete(tempParPath); } catch {}
            }

            var readerParameters = new ParArchiveReaderParameters
            {
                Recursive = true,
            };

            var writerParameters = new ParArchiveWriterParameters
            {
                CompressorVersion = opts.Compression,
                OutputPath = tempParPath,
            };

            Console.WriteLine("Step 2/2: Merging files into PAR archive (in-memory/stream overlay)...");
            Console.Write("Reading PAR file... ");
            Node par = NodeFactory.FromFile(backupPath, Yarhl.IO.FileOpenMode.Read);
            par.TransformWith(new ParArchiveReader(readerParameters));
            writerParameters.IncludeDots = par.Children[0].Name == ".";
            Console.WriteLine("DONE!");

            Console.Write("Reading input directory... ");
            string allDirNodeName = new DirectoryInfo(allDir).Name;
            Node node = ReadDirectory(allDir, allDirNodeName);
            node.TransformWith(new ParArchiveWriter(writerParameters)).TransformWith(new ParArchiveReader(readerParameters));
            Console.WriteLine("DONE!");

            Console.Write("Adding files... ");
            node.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(par, true);
#pragma warning disable CA1308 // Normalize strings to uppercase
            par.SortChildren((x, y) => string.CompareOrdinal(x.Name.ToLowerInvariant(), y.Name.ToLowerInvariant()));
#pragma warning restore CA1308 // Normalize strings to uppercase
            Console.WriteLine("DONE!");

            ParArchiveWriter.NestedParCreating += sender => Console.WriteLine($"Creating nested PAR {sender.Name}... ");
            ParArchiveWriter.NestedParCreated += sender => Console.WriteLine($"{sender.Name} created!");
            ParArchiveWriter.FileCompressing += sender => Console.WriteLine($"Compressing {sender.Name}... ");

            Console.WriteLine("Creating PAR (this may take a while)... ");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPar));
            writerParameters.OutputPath = outputPar;
            writerParameters.IncludeDots = false;
            par.TransformWith(new ParArchiveWriter(writerParameters));
            par.Dispose();
            node.Dispose();

            if (File.Exists(tempParPath))
            {
                try
                {
                    File.Delete(tempParPath);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Could not delete temporary PAR file '{tempParPath}': {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("DONE!\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================================================================================");
            Console.WriteLine($"SUCCESS: New PAR archive compiled successfully at:\n  {outputPar}");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
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
    }
}
