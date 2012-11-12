using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using RocksmithToolkitLib.Sng;

namespace Xml2Sng
{
    internal class Arguments
    {
        public bool ShowHelp;
        public string InputFile;
        public string OutputFile;
        public GamePlatform Platform;
        public ArrangementType ArrangementType;
    }

    internal class Program
    {
        private static Arguments DefaultArguments()
        {
            return new Arguments
            {
                Platform = GamePlatform.Pc,
                ArrangementType = ArrangementType.Instrument
            };
        }
        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "h|?|help", "Show this help message and exit.", v => outputArguments.ShowHelp = v != null },
                { "i|input=", "The input XML file (required)", v => outputArguments.InputFile = v },
                { "o|output=", "The output SNG file", v => outputArguments.OutputFile = v },
                { "console", "Generate a big-endian (console) file instead of little-endian (PC)", v => { if (v != null) outputArguments.Platform = GamePlatform.Console; }},
                { "vocal", "Generate from a vocal XML file instead of a song XML file", v => { if (v != null) outputArguments.ArrangementType = ArrangementType.Vocal; }}
            };
        }

        static void Main(string[] args)
        {
            var arguments = DefaultArguments();
            var options = GetOptions(arguments);
            try
            {
                options.Parse(args);
                if (arguments.ShowHelp)
                {
                    options.WriteOptionDescriptions(Console.Out);
                    return;
                }
                if (string.IsNullOrEmpty(arguments.InputFile))
                {
                    ShowHelpfulError("Must specify an input file.");
                    return;
                }
                if (!File.Exists(arguments.InputFile))
                {
                    ShowHelpfulError("Specified input file does not exist.");
                    return;
                }
                if (string.IsNullOrEmpty(arguments.OutputFile))
                    arguments.OutputFile = Path.ChangeExtension(arguments.InputFile, "sng");
            }
            catch (OptionException ex)
            {
                ShowHelpfulError(ex.Message);
                return;
            }

            SngFileWriter.Write(arguments.InputFile, arguments.OutputFile, arguments.ArrangementType, arguments.Platform);
            Console.WriteLine(string.Format("Successfully converted XML file to SNG file."));
            Console.WriteLine("\tInput:  " + arguments.InputFile);
            Console.WriteLine("\tOutput: " + arguments.OutputFile);
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("xml2sng: ");
            Console.WriteLine(message);
            Console.WriteLine("Try `xml2sng --help` for more information.");
        }
    }
}
