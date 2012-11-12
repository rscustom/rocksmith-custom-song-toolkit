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
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public GamePlatform Platform { get; set; }
        public ArrangementType ArrangementType { get; set; }
        public InstrumentTuning Tuning { get; set; }
        public bool ShowHelp { get; set; }
    }

    internal class Program
    {
        private readonly Arguments _arguments;
        private readonly OptionSet _options;

        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        internal Program()
        {
            _arguments = new Arguments
            {
                Platform = GamePlatform.Pc,
                ArrangementType = ArrangementType.Instrument,
                Tuning = InstrumentTuning.Standard
            };
            _options = new OptionSet
            {
                { "h|?|help", "Show this help message and exit.", v => _arguments.ShowHelp = v != null },
                { "i|input=", "The input XML file (required)", v => _arguments.InputFile = v },
                { "o|output=", "The output SNG file", v => _arguments.OutputFile = v },
                { "console", "Generate a big-endian (console) file instead of little-endian (PC)", v => { if (v != null) _arguments.Platform = GamePlatform.Console; }},
                { "vocal", "Generate from a vocal XML file instead of a song XML file", v => { if (v != null) _arguments.ArrangementType = ArrangementType.Vocal; }}                
                // TODO: add parameter to pass tuning option
            };
        }

        private void Run(IEnumerable<string> args)
        {
            try
            {
                _options.Parse(args);
                if (_arguments.ShowHelp)
                {
                    _options.WriteOptionDescriptions(Console.Out);
                    return;
                }
                if (string.IsNullOrEmpty(_arguments.InputFile))
                    throw new InvalidOperationException("Must specify an input file.");
                if (!File.Exists(_arguments.InputFile))
                    throw new InvalidOperationException("Specified input file does not exist.");

                if (string.IsNullOrEmpty(_arguments.OutputFile))
                    _arguments.OutputFile = Path.ChangeExtension(_arguments.InputFile, "sng");
            }
            catch (InvalidOperationException ex)
            {
                Console.Write("xml2sng: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `xml2sng --help` for more information.");
                return;
            }
            catch (OptionException ex)
            {
                Console.Write("xml2sng: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `xml2sng --help` for more information.");
                return;
            }

            SngFileWriter.Write(_arguments.InputFile, _arguments.OutputFile, _arguments.ArrangementType, _arguments.Platform, _arguments.Tuning);
            Console.WriteLine(string.Format("Successfully converted XML file to SNG file."));
            Console.WriteLine("\tInput:  " + _arguments.InputFile);
            Console.WriteLine("\tOutput: " + _arguments.OutputFile);
        }
    }
}
