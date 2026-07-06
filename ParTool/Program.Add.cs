// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace ParTool
{
    using System;
    using System.IO;
    using ParLibrary.Converter;
    using Yarhl.FileSystem;

    /// <summary>
    /// Node adding functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Add(Options.Add opts)
        {
            WriteHeader();

            if (!File.Exists(opts.InputParArchivePath))
            {
                Console.WriteLine($"ERROR: \"{opts.InputParArchivePath}\" not found!!!!");
                return;
            }

            if (!Directory.Exists(opts.AddDirectory))
            {
                Console.WriteLine($"ERROR: \"{opts.AddDirectory}\" not found!!!!");
                return;
            }

            if (File.Exists(opts.OutputParArchivePath))
            {
                Console.WriteLine("WARNING: Output file already exists. It will be overwritten.");
                Console.Write("Continue? (y/N) ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer) && answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }

                File.Delete(opts.OutputParArchivePath);
            }

            var readerParameters = new ParArchiveReaderParameters
            {
                Recursive = true,
            };

            string tempParPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(opts.OutputParArchivePath)), "temp_add_data.par");
            if (File.Exists(tempParPath))
            {
                File.Delete(tempParPath);
            }

            var writerParameters = new ParArchiveWriterParameters
            {
                CompressorVersion = opts.Compression,
                OutputPath = tempParPath,
            };

            Console.Write("Reading PAR file... ");
            Node par = NodeFactory.FromFile(opts.InputParArchivePath, Yarhl.IO.FileOpenMode.Read);
            par.TransformWith(new ParArchiveReader(readerParameters));
            writerParameters.IncludeDots = par.Children[0].Name == ".";
            Console.WriteLine("DONE!");

            Console.Write("Reading input directory... ");
            string nodeName = new DirectoryInfo(opts.AddDirectory).Name;
            Node node = ReadDirectory(opts.AddDirectory, nodeName);
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
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(opts.OutputParArchivePath)));
            writerParameters.OutputPath = opts.OutputParArchivePath;
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
                    Console.WriteLine($"WARNING: Could not delete temporary PAR file '{tempParPath}': {ex.Message}");
                }
            }

            Console.WriteLine("DONE!");
        }
    }
}
