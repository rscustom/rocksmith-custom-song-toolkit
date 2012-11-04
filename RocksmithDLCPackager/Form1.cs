using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Security.Cryptography;

namespace RocksmithDLCPackager
{
    public partial class asd2134tc3 : Form
    {
        RijndaelManaged rij;
        public asd2134tc3()
        {
            InitializeComponent();
            rij = new RijndaelManaged();
            rij.Padding = PaddingMode.None;
            rij.Mode = CipherMode.ECB;
            rij.BlockSize = 128;
            rij.IV = new byte[16];
            rij.Key = new byte[32] { 0xFA, 0x6F, 0x4F, 0x42, 0x3E, 0x66, 0x9F, 0x9E, 0x6A, 0xD2, 0x3A, 0x2F, 0x8F, 0xE5, 0x81, 0x88, 0x63, 0xD9, 0xB8, 0xFD, 0xED, 0xDF, 0xFE, 0xBD, 0x12, 0xB2, 0x7F, 0x76, 0x80, 0xD1, 0x51, 0x41 };
        }

        private void asdasfsasdagq3wr_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var name = GetLastDirectory(fbd.SelectedPath);
            var psarc = new PSARC.PSARC();
            foreach (var x in Directory.EnumerateFiles(fbd.SelectedPath))
            {
                var entry = new PSARC.Entry() { Name = Path.GetFileName(x), Data = File.OpenRead(x) };
                entry.Length = (ulong)entry.Data.Length;
                psarc.AddEntry(entry);
            }
            foreach (var x in Directory.EnumerateDirectories(fbd.SelectedPath))
            {
                var dName = GetLastDirectory(x);
                var psarc2 = new PSARC.PSARC();
                WalkTroughDirectory("", x, (a, b) => psarc2.AddEntry(a, File.OpenRead(b)));
                var dt = new MemoryStream();
                psarc2.Write(dt);
                foreach (var y in psarc2.Entries)
                    y.Data.Close();
                psarc.AddEntry(dName + ".psarc", dt);
            }
            if (ck3249wkjrhi532nk.Checked)
            {
                var data = new MemoryStream();
                psarc.Write(data);
                data.Seek(0, SeekOrigin.Begin);
                using (var str = File.Create(sfd.FileName))
                    Crypto(data, str, rij.CreateEncryptor());
            }
            else
                using (var str = File.Create(sfd.FileName))
                    psarc.Write(str);
        }

        void WalkTroughDirectory(string baseDir, string directory, Action<string, string> action)
        {
            foreach (var fl in Directory.GetFiles(directory))
                action(String.Format("{0}/{1}", baseDir, Path.GetFileName(fl)).TrimStart('/'), fl);
            foreach (var dr in Directory.GetDirectories(directory))
                WalkTroughDirectory(String.Format("{0}/{1}", baseDir, GetLastDirectory(dr)), dr, action);
        }

        string GetLastDirectory(string dir)
        {
            return dir.Split(new string[4] { "/", "//", @"\", @"\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private void extractPSARC(string filename, string path, Stream dt)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var psarc = new PSARC.PSARC();
            psarc.Read(dt);
            for (int i = 1; i < psarc.Entries.Count; i++)
            {
                var x = psarc.Entries[i];
                var fullfilename = path + "\\" + name + "\\" + x.Name;
                x.Data.Seek(0, SeekOrigin.Begin);
                if (Path.GetExtension(x.Name).ToLower() == ".psarc")
                {
                    extractPSARC(fullfilename, path + "\\" + name, x.Data);
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

        private void Crypto(Stream input, Stream output, ICryptoTransform transform)
        {
            var buffer = new byte[1024];
            var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
            for (long i = 0; i < input.Length; i += buffer.Length)
            {
                int sz = (int)Math.Min(input.Length - input.Position, buffer.Length);
                input.Read(buffer, 0, sz);
                cs.Write(buffer, 0, sz);
            }
            int pad = buffer.Length - (int)(input.Length % buffer.Length);
            if (pad > 0)
                cs.Write(new byte[pad], 0, pad);
            cs.Flush();
            output.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private void asd324fdd34_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            using (var str = File.OpenRead(ofd.FileName))
            {
                if (ck3249wkjrhi532nk.Checked)
                {
                    var ms = new MemoryStream((int)str.Length);
                    Crypto(str, ms, rij.CreateDecryptor());
                    extractPSARC(ofd.FileName, fbd.SelectedPath, ms);
                }
                else
                    extractPSARC(ofd.FileName, fbd.SelectedPath, str);
            }

        }

    }
}
