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
    }
}
