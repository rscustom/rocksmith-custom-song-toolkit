using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using X360.STFS;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitLib.DLCPackage
{
    public class DLCPackageData
    {
        public GameVersion GameVersion;
        
        public bool Pc { get; set; }
        public bool Mac { get; set; }
        public bool XBox360 { get; set; }
        public bool PS3 { get; set; }

        public string AppId { get; set; }
        public string Name { get; set; }
        public SongInfo SongInfo { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; }
        public string OggPreviewPath { get; set; }
        public List<Arrangement> Arrangements { get; set; }
        public float Volume { get; set; }
        public PackageMagic SignatureType { get; set; }

        private List<XBox360License> xbox360Licenses = null;
        public List<XBox360License> XBox360Licenses
        {
            get
            {
                if (xbox360Licenses == null)
                {
                    xbox360Licenses = new List<XBox360License>();
                    return xbox360Licenses;
                }
                else
                    return xbox360Licenses;
            }
            set { xbox360Licenses = value; }
        }

        #region RS1 only

        public List<Tone.Tone> Tones { get; set; }

        #endregion

        #region RS2014 only

        public List<Tone2014> TonesRS2014 { get; set; }

        // cache album art conversion
        public Dictionary<int, string> AlbumArt { get; set; }

        #endregion

        // needs to be called after all packages for platforms are created
        public void CleanCache() {
            if (AlbumArt != null) {
                foreach (var path in AlbumArt.Values) {
                    try {
                        File.Delete(path);
                    } catch { }
                }
                AlbumArt = null;
            }
            foreach (var a in Arrangements)
                a.CleanCache();
        }

        ~DLCPackageData()
        {
            CleanCache();
        }
    }
}
