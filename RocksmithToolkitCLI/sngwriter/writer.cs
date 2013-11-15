using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib;
using System.IO;
using System.Text;

namespace Writer
{
    public class SngWriter
    {
        public static void Main(string[] args)
        {
            var xmlfile = args[0];
            var sngfile = args[1];

            // TODO we need this only to decide encryption and endianness, not for parsing XML, it should be removed from Write()
            Platform platform = new Platform(GamePlatform.Pc, GameVersion.RS2014);
            // TODO we only need Guitar / Bass and maybe Vocals in the future
            ArrangementType instrument = ArrangementType.Bass;
            using (FileStream fs = new FileStream(sngfile, FileMode.Create)) {
                // TODO here we can decide endianness
                BinaryWriter sngdata = new BinaryWriter(fs);
                Sng2014FileWriter.Write(xmlfile, sngdata, instrument, platform);
                // TODO here we can pack, encrypt and wrap raw SNG data (memory writer)
                //      and write correct SNG to sngfile (writing raw SNG data for now)
            }
        }
    }
}
