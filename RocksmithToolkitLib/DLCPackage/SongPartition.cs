using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.DLCPackage
{
    public class SongPartition
    {
        private int[] songPartitionCount = { 0 /* Combo count */, 0 /* Lead count */, 0 /* Rhythm count */, 0 /* Bass Count */ };

        public string GetArrangementFileName(ArrangementName arrangementName, ArrangementType arrangementType)
        {
            var name = arrangementName.ToString();
            var count = GetSongPartition(arrangementName, arrangementType);
            if (count > 1)
                name += count.ToString();
            return name;
        }

        public int GetSongPartition(ArrangementName arrangementName, ArrangementType arrangementType)
        {
            switch (arrangementType)
            {
                case Sng.ArrangementType.Bass:
                    songPartitionCount[3]++;
                    return songPartitionCount[3];
                default:
                    switch (arrangementName)
                    {
                        case RocksmithToolkitLib.Sng.ArrangementName.Lead:
                            songPartitionCount[1]++;
                            return songPartitionCount[1];
                        case RocksmithToolkitLib.Sng.ArrangementName.Rhythm:
                            songPartitionCount[2]++;
                            return songPartitionCount[2];
                        default:
                            songPartitionCount[0]++;
                            return songPartitionCount[0];
                    }
            };
        }
    }
}
