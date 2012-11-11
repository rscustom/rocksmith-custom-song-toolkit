using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class Packer
    {
        public static void Pack(string sourcePath, string saveFileName, bool useCryptography)
        {
            var psarc = new PSARC.PSARC();
            foreach (var x in Directory.EnumerateFiles(sourcePath))
            {
                var entry = new PSARC.Entry
                {
                    Name = Path.GetFileName(x),
                    Data = File.OpenRead(x)
                };
                entry.Length = (ulong) entry.Data.Length;
                psarc.AddEntry(entry);
            }
            foreach (var x in Directory.EnumerateDirectories(sourcePath))
            {
                var dName = Path.GetFileName(x);
                var psarc2 = new PSARC.PSARC();
                WalkThroughDirectory("", x, (a, b) => psarc2.AddEntry(a, File.OpenRead(b)));
                var dt = new MemoryStream();
                psarc2.Write(dt);
                foreach (var y in psarc2.Entries)
                    y.Data.Close();
                psarc.AddEntry(dName + ".psarc", dt);
            }
            if (useCryptography)
            {
                var data = new MemoryStream();
                psarc.Write(data);
                data.Seek(0, SeekOrigin.Begin);
                using (var str = File.Create(saveFileName))
                    RijndaelEncryptor.Encrypt(data, str);
            }
            else
                using (var str = File.Create(saveFileName))
                    psarc.Write(str);
        }

        public static void Unpack(string sourceFileName, string savePath, bool useCryptography)
        {
            using (var stream = File.OpenRead(sourceFileName))
            {
                Stream destinationStream = stream;
                if (useCryptography)
                {
                    var ms = new MemoryStream((int) stream.Length);
                    RijndaelEncryptor.Decrypt(stream, ms);
                    destinationStream = ms;
                }
                ExtractPSARC(sourceFileName, savePath, destinationStream);
            }
        }

        private static void WalkThroughDirectory(string baseDir, string directory, Action<string, string> action)
        {
            foreach (var fl in Directory.GetFiles(directory))
                action(String.Format("{0}/{1}", baseDir, Path.GetFileName(fl)).TrimStart('/'), fl);
            foreach (var dr in Directory.GetDirectories(directory))
                WalkThroughDirectory(String.Format("{0}/{1}", baseDir, Path.GetFileName(dr)), dr, action);
        }

        private static void ExtractPSARC(string filename, string path, Stream dt)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var psarc = new PSARC.PSARC();
            psarc.Read(dt);
            for (int i = 1; i < psarc.Entries.Count; i++)
            {
                var x = psarc.Entries[i];
                var fullfilename = Path.Combine(path, name, x.Name);
                x.Data.Seek(0, SeekOrigin.Begin);
                if (Path.GetExtension(x.Name).ToLower() == ".psarc")
                {
                    ExtractPSARC(fullfilename, path + "\\" + name, x.Data);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullfilename));
                    using (var str = File.Create(fullfilename))
                    {
                        var buff = new byte[x.Data.Length];
                        x.Data.Read(buff, 0, buff.Length);
                        str.Write(buff, 0, buff.Length);
                        x.Data.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
        }
    }
}
