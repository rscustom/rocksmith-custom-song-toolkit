using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NDesk.Options;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Sng;

namespace Xml2Sng
{
    internal class Arguments
    {
        public bool ShowHelp;
        public string InputFile;
        public string OutputFile;
        public Platform Platform;
        public ArrangementType ArrangementType;
        public InstrumentTuning Tuning { get; set; }
    }

    internal class Program
    {
        private static Arguments DefaultArguments()
        {
            return new Arguments
            {
                Platform = new Platform(GamePlatform.Pc, GameVersion.None),
                ArrangementType = ArrangementType.Guitar,
                Tuning = InstrumentTuning.Standard
            };
        }

        private static InstrumentTuning ParseTuning(string input)
        {
            var regex = new Regex(@"^(-?\d+),(-?\d+),(-?\d+),(-?\d+),(-?\d+),(-?\d+)$");
            var match = regex.Match(input);
            if (!match.Success)
                throw new OptionException("Invalid syntax for tuning", "tuning");

            var tuningValues = new []
            {
                Int16.Parse(match.Groups[1].Value),
                Int16.Parse(match.Groups[2].Value),
                Int16.Parse(match.Groups[3].Value),
                Int16.Parse(match.Groups[4].Value),
                Int16.Parse(match.Groups[5].Value),
                Int16.Parse(match.Groups[6].Value)
            };

            var supportedTuning = Enum.GetValues(typeof(InstrumentTuning))
                .Cast<InstrumentTuning?>()
                .FirstOrDefault(t => t.HasValue && t.Value.GetOffsets().SequenceEqual(tuningValues));
            if (!supportedTuning.HasValue)
                throw new OptionException("Unsupported alternate tuning", "tuning");
            return supportedTuning.Value;
        }

        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "h|?|help",
                    "Show this help message and exit.",
                    v => outputArguments.ShowHelp = v != null },
                { "i|input=",
                    "The input XML file (required)",
                    v => outputArguments.InputFile = v },
                { "o|output=",
                    "The output SNG file",
                    v => outputArguments.OutputFile = v },
                { "console",
                    "Generate a big-endian (console) file instead of little-endian (PC)",
                    v => { if (v != null) outputArguments.Platform = new Platform(GamePlatform.XBox360, GameVersion.None); /*Same as PS3*/ }},
                { "vocal",
                    "Generate from a vocal XML file instead of a guitar XML file",
                    v => { if (v != null) outputArguments.ArrangementType = ArrangementType.Vocal; }},
                { "bass",
                    "Generate from a bass XML file instead of a guitar XML file",
                    v => { if (v != null) outputArguments.ArrangementType = ArrangementType.Bass; }},
                { "tuning=",
                    "Use an alternate tuning for this song file."
                    + " Tuning parameter should be comma-separated offsets from standard EADGBe tuning."
                    + " For example, Drop D looks like: tuning=-2,0,0,0,0,0",
                    v => outputArguments.Tuning = ParseTuning(v) }
            };
        }

        static int Main(string[] args)
        {
            var arguments = DefaultArguments();
            var options = GetOptions(arguments);
            try
            {
                options.Parse(args);
                if (arguments.ShowHelp)
                {
                    options.WriteOptionDescriptions(Console.Out);
                    return 0;
                }
                if (string.IsNullOrEmpty(arguments.InputFile))
                {
                    ShowHelpfulError("Must specify an input file.");
                    return 1;
                }
                if (!File.Exists(arguments.InputFile))
                {
                    ShowHelpfulError("Specified input file does not exist.");
                    return 1;
                }
                if (string.IsNullOrEmpty(arguments.OutputFile))
                    arguments.OutputFile = Path.ChangeExtension(arguments.InputFile, "sng");
            }
            catch (OptionException ex)
            {
                ShowHelpfulError(ex.Message);
                return 1;
            }

            SngFileWriter.Write(arguments.InputFile, arguments.OutputFile, arguments.ArrangementType, arguments.Platform);

            Console.WriteLine(string.Format("Successfully converted XML file to SNG file."));
            Console.WriteLine("\tInput:  " + arguments.InputFile);
            Console.WriteLine("\tOutput: " + arguments.OutputFile);

            return 0;
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("xml2sng: ");
            Console.WriteLine(message);
            Console.WriteLine("Try `xml2sng --help` for more information.");
        }
    }
}
