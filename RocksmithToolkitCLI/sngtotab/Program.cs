using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.SngToTab;

namespace SngToTab
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Out.WriteLine("Syntax: sngtotab.exe [files]");
                return;
            }

            int difficulty = Common.MAX_DIFFICULTY_ONLY;

            try {
                foreach (string filename in args)
                {
                    if (filename.StartsWith("-l"))
                    {
                        string difficultyString = filename.Substring(2);
                        if (difficultyString.Equals("a", StringComparison.Ordinal))
                            difficulty = Common.ALL_DIFFICULTIES;
                        else if (difficultyString.Equals("m", StringComparison.Ordinal))
                            difficulty = Common.MAX_DIFFICULTY_ONLY;
                        else
                            difficulty = int.Parse(difficultyString);

                        continue;
                    }

                    SngFile sngFile = new SngFile(filename);

                    int maxDifficulty = Common.getMaxDifficulty(sngFile);

                    int[] difficulties;
                    switch (difficulty)
                    {
                        case Common.MAX_DIFFICULTY_ONLY:
                            difficulties = new int[] { maxDifficulty };
                            break;

                        case Common.ALL_DIFFICULTIES:
                            difficulties = Enumerable.Range(0, maxDifficulty + 1).ToArray();
                            break;

                        default:
                            difficulties = new int[] { difficulty };
                            break;
                    }

                    foreach (int d in difficulties)
                    {
                        TabFile tabFile = new TabFile(sngFile, d);

                        string outputFilename;
                        if (filename.EndsWith(".sng"))
                            outputFilename = filename.Substring(0, filename.Length - 4) + "." + d + ".txt";
                        else
                            outputFilename = filename + "." + d + ".txt";
                        TextWriter tw = new StreamWriter(outputFilename);
                        tw.Write(tabFile.ToString());
                        tw.Close();
                    }
                }
                Console.WriteLine("The conversion is complete.");
            } catch (Exception ex){
                Console.WriteLine("Error occurred: " + ex.Message + Environment.NewLine + ex.InnerException);
            }
        }
    }

}
