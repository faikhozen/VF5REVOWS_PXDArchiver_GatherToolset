// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace ParTool.Options
{
    using CommandLine;

    /// <summary>
    /// PAR archive gather and compile options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("gather-compile", HelpText = "Gather mods and compile a Yakuza PAR archive.")]
    internal class GatherCompile
    {
        /// <summary>
        /// Gets or sets the input PAR archive path.
        /// </summary>
        [Value(0, MetaName = "input", Required = true, HelpText = "Input PAR archive path (reference chara.par).")]
        public string InputParArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the mods directory path.
        /// </summary>
        [Option('m', "mods", Default = "mods", HelpText = "Folder containing individual mod directories.")]
        public string ModsDirectory { get; set; }

        /// <summary>
        /// Gets or sets the final output PAR archive path.
        /// </summary>
        [Option('o', "output", Default = "output/chara.par", HelpText = "Output compiled PAR path.")]
        public string OutputParArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the compression algorithm to use.
        /// </summary>
        [Option('c', "compression", Default = 0x00, HelpText = "SLLZ algorithm (0 = uncompressed, 1 = SLLZ).")]
        public int Compression { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to compile as a sound mod (vf5fs_data.par) instead of character mods.
        /// </summary>
        [Option('s', "sound", Default = false, HelpText = "Compile as a sound mod (looks for 'rom' folder inside mods).")]
        public bool IsSoundMod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to compile as a Master Skin Mod (blanking items + applying master skins).
        /// </summary>
        [Option("master-skin", Default = false, HelpText = "Compile as Master Skin Mod (blanking items + applying master skins).")]
        public bool IsMasterSkinMod { get; set; }

        /// <summary>
        /// Gets or sets the Master Skin suboption (1 = master_skins/compiled, 2 = master_skins/mods).
        /// </summary>
        [Option("master-skin-suboption", Default = 1, HelpText = "Master Skin suboption mode: 1 = master_skins/compiled, 2 = master_skins/mods.")]
        public int MasterSkinSuboption { get; set; } = 1;
    }
}

