using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib;
using System.IO;
using System.Text;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace Writer
{
    public class SngWriter
    {
        public static void Main(string[] args)
        {
            var xmlfile = args[0];
            var sngfile = args[1];

            using (FileStream fs = new FileStream(sngfile, FileMode.Create)) {
                // parse from XML
                // TODO we only need Guitar / Bass and maybe Vocals in the future
                ArrangementType instrument = ArrangementType.Bass;
                Sng2014File sng = new Sng2014File(xmlfile, instrument);

                // write raw SNG data for diffing and inspection
                var raw = new FileStream(sngfile + ".raw", FileMode.Create);
                EndianBitConverter conv = EndianBitConverter.Little;
                EndianBinaryWriter w = new EndianBinaryWriter(conv, raw);
                sng.Write(w);

                // write fully packed SNG
                Platform platform = new Platform(GamePlatform.Pc, GameVersion.RS2014);
                sng.writeSng(fs, platform);
            }
        }
    }
}
