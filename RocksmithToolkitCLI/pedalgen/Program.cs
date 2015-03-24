using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.ToolkitTone;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace pedalgen
{
    internal class Arguments
    {
        public bool ShowHelp;
        public string Version;
        public string InputDirectory;
    }

    internal class Program
    {
        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "h|?|help", "Show this help message and exit", v => outputArguments.ShowHelp = v != null },
                { "v|version", "Version of the Rocksmith Game [RS2012 or RS2014], default is RS2012", v => outputArguments.Version = v },
                { "i|dir|input=", "The input directory (defaults to the input directory)", v => outputArguments.InputDirectory = v }
            };
        }

        static void Main(string[] args)
        {
            //GeneratePedalsRS2014(@"W:\MUSICA\ROCKSMITH\RS2014_UNPACKED\gears_Pc2014\manifests\gears"); return;
            var arguments = new Arguments();
            var options = GetOptions(arguments);

            if (args.Length == 0)
	        {
                    options.WriteOptionDescriptions(Console.Out);
                    return;
	       	}
            try
            {
                options.Parse(args);
                if (arguments.ShowHelp)
                {
                    options.WriteOptionDescriptions(Console.Out);
                    return;
                }

                if (String.IsNullOrEmpty(arguments.Version))
                {
                    ShowHelpfulError("Must especify the Rocksmith version. 'RS2012' for Rocksmith 1 or 'RS2014' for Rocksmith 2014");
                    return;
                }
                if (String.IsNullOrEmpty(arguments.InputDirectory))
                {
                    ShowHelpfulError("Must especify the input directory.");
                    return;
                }
                if (!Directory.Exists(arguments.InputDirectory)){
                    ShowHelpfulError("The input directory doesn't exists.");
                    return;
                }

                switch (arguments.Version.ToUpper())
                {
                    case "RS2014":
                        GeneratePedalsRS2014(arguments.InputDirectory);
                        break;
                    default:
                        GeneratePedals(new string[] { arguments.InputDirectory });
                        break;
                }
            }
            catch (OptionException ex)
            {
                ShowHelpfulError(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                ShowHelpfulError(ex.Message);
                return;
            }
        }

        private static void GeneratePedalsRS2014(string inputDir) {
            //inputDir like "......\ROCKSMITH\gears_Pc\manifests\gears" (gears_Pc is gears.psarc unpacked)
            var gearsJsonFiles = Directory.EnumerateFiles(inputDir, "*.json");
            List<ToolkitPedal> toolkitPedals = new List<ToolkitPedal>();
            foreach (var file in gearsJsonFiles)
            {
                var gearManifest = Manifest2014<RsToneRS2014>.LoadFromFile(file);
                foreach (var pedal in gearManifest.Entries)
                {
                    toolkitPedals.Add(pedal.Value["Attributes"].ToPedal());
                }
            }
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            File.WriteAllText("pedals2014.json", JsonConvert.SerializeObject(toolkitPedals, jss)); 
        }

        private static void GeneratePedals(string[] args) {
            var dirs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            { dirs.Add(args[i]); }
            var pedals = new List<RsTone>();
            foreach (string file in dirs)
            {
                try
                {
                    var pedalsJson = File.ReadAllText(file);
                    //var junk = pedalsJson.IndexOf("\n}");
                    //if (junk > -1)
                    //{
                    //    var lastCurly = junk + 2;
                    //    while (lastCurly < pedalsJson.Length && (pedalsJson[lastCurly] == '}' || pedalsJson[lastCurly] == ' '))
                    //    {
                    //        ++lastCurly;
                    //    }
                    //    if (lastCurly < pedalsJson.Length)
                    //    {
                    //        pedalsJson = pedalsJson.Substring(0, lastCurly);
                    //    }
                    //}
                    //pedalsJson.Replace("TonePedalRTPCName", "Key");
                    var filePedals = JsonConvert.DeserializeObject<RsJsonFile<RsTone>>(pedalsJson);
                    if ("GRPedal".Equals(filePedals.ModelName))
                    {
                        pedals.AddRange(filePedals.Entries);
                    }
                }
                catch (Exception ex)
                {
                    ShowHelpfulError(ex.Message);
                    return;
                }
            }
            var bad = pedals.Where(p => p.Type == null);
            var goodPedals = pedals.Where(p => p.Type != null);
            var toolkitPedals = goodPedals.Select(pedal => pedal.ToPedal());
            foreach (var i in dirs)
                File.WriteAllText(Path.Combine(i, "pedals.json"), JsonConvert.SerializeObject(toolkitPedals, Formatting.Indented));
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("pedalgen: ");
            Console.WriteLine(message);
            Console.WriteLine("Try 'pedalgen --help' for more information.");
        }
    }
}
