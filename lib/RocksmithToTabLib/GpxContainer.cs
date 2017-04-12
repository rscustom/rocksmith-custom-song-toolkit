using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RocksmithToTabLib
{
    public class GpxContainer
    {
        public static Stream CreateGPXContainer(Stream score)
        {
            const int HEADER_BCFS = 1397113666;
            const int sectorSize = 0x1000;
            // The container format used in GPX is some sort of file system with a FAT.
            // I don't understand the format fully, and it seems rather ridiculous for the 
            // purpose. I only understand enough to "fake" it. The data is organised in 
            // sectors of 4096 bytes each, where a sector can be a file entry containing
            // a filename, filesize and list of other sectors containing the actual file data.
            // The very first sector after the file header is almost complete filled with 0xff,
            // except for the first four bytes, which can vary. I don't know their purpose, but
            // from tests it seems that Guitar Pro doesn't mind at all if you fill this first
            // sector with random data.
            // The next sector appears to be a root file index. I don't fully know all the details,
            // but it seems to be mostly constant.
            // Apart from that, we only need to create a single sector for a single file entry,
            // score.gpix, and as many data sectors as are needed to store the file.

            var output = new MemoryStream();
            var writer = new BinaryWriter(output);
            var firstSector = new byte[sectorSize];
            var nullData = new byte[sectorSize];
            for (var i = 0; i < sectorSize; ++i)
            {
                firstSector[i] = 0xff;
                nullData[i] = 0x00;
            }
            firstSector[0] = firstSector[1] = 0x00;

            // prepare file data
            byte[] data;
            using (var mem = new MemoryStream())
            {
                score.CopyTo(mem);
                data = mem.ToArray();
            }
            int numberOfSectors = (data.Length + sectorSize - 1) / sectorSize;

            // write header
            writer.Write(HEADER_BCFS);
            // write first sector
            writer.Write(firstSector, 0, sectorSize);
            // next sector (root dir?) is identified by the number 1
            writer.Write((int)1);
            // write filename '/'
            writer.Write('/');
            writer.Write(nullData, 0, 131);
            // I don't fully understand what the next numbers mean, but these
            // static values seem to work
            writer.Write((int)1);
            writer.Write((int)4); // might be the last sector that contains a file entry
            writer.Write((int)0);
            writer.Write((int)3); // first sector with file entry?
            writer.Write(nullData, 0, 3944); // fill up rest of sector

            // now comes the file entry for the score.gpif file, identified by number 2
            writer.Write((int)2);
            writer.Write("score.gpif".ToArray());
            writer.Write(nullData, 0, 122);
            writer.Write((int)1);
            // file size
            writer.Write(data.Length);
            writer.Write((int)0);
            // next is the list of sectors containing the file contents
            for (int i = 4; i < 4 + numberOfSectors; ++i)
                writer.Write(i);
            // fill up sector
            writer.Write(nullData, 0, 3948 - numberOfSectors * 4);

            // this next sector links to other sectors containing file entries (I think)
            // but we don't have those, so it's basically empty.
            writer.Write((int)2);
            writer.Write(nullData, 0, sectorSize - 4);

            // finally, write actual file content
            writer.Write(data, 0, data.Length);
            // fill final sector
            writer.Write(nullData, 0, sectorSize - (data.Length % sectorSize));

            // one filler byte, apparently required
            writer.Write((byte)0);

            output.Position = 0;
            return output;
        }

    }
}
