using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Header;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;

// PsarcLoader methods are used to load archives into memory
// More efficient and faster than unpacking to physical files
// RS2014 ONLY

namespace RocksmithToolkitLib.PSARC
{
    public sealed class PsarcLoader : IDisposable
    {
        private PSARC _archive;
        private string _filePath;
        private Stream _fileStream;
        public string ErrMsg { get; set; }

        // Loads song archive file to memory.
        public PsarcLoader(string fileName, bool useMemory = true)
        {
            _filePath = fileName;
            _archive = new PSARC(useMemory);
            _fileStream = File.OpenRead(_filePath);
            _archive.Read(_fileStream);
        }

        public Stream ExtractEntryData(Func<Entry, bool> entryLINQ)
        {
            var entry = _archive.TOC.Where(entryLINQ).FirstOrDefault();
            if (entry != null)
            {
                MemoryStream ms = new MemoryStream();
                _archive.InflateEntry(entry);
                if (entry.Data == null)
                    return null;

                entry.Data.Position = 0;
                entry.Data.CopyTo(ms);
                entry.Dispose();
                ms.Position = 0;
                return ms;
            }

            return null;
        }

        public List<string> ExtractEntryNames()
        {
            var sw = new Stopwatch();
            sw.Restart();
            var entryNames = new List<string>();
            var step = Math.Round(1.0 / (_archive.TOC.Count + 2) * 100, 3);
            double progress = 0;
            GlobalExtension.ShowProgress("Inflating Entries ...");


            // this iterates through all entries in archive
            foreach (var entry in _archive.TOC)
            {
                // do something interesting with the entry
                entryNames.Add(entry.Name);
                // needed to free memory (prevent crashing)
                if (entry.Data != null)
                    entry.Data.Dispose();

                progress += step;
                GlobalExtension.UpdateProgress.Value = (int)progress;
            }

            GlobalExtension.UpdateProgress.Value = 100;
            return entryNames;
        }

        // produces a comprehensive list of archive entry errors
        public List<string> FullErrorCheck()
        {
            List<string> errorLog = new List<string>();

            // this iterates through all entries in archive looking for errors
            foreach (var entry in _archive.TOC)
            {
                _archive.InflateEntry(entry);
                if (!String.IsNullOrEmpty(_archive.ErrMsg.ToString()))
                    errorLog.Add(_archive.ErrMsg.ToString());

                if (entry.Data == null)
                    errorLog.Add("Null Entry Error: " + entry.Name);
                else
                {
                    entry.Data.Position = 0;
                    var ms = new MemoryStream();
                    using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 65536)) //4Kb is default alloc size for windows .. 64Kb is default PSARC alloc
                    {
                        try
                        {
                            var canRead = reader.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            errorLog.Add("Error Reading Entry: " + entry.Name + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }

            return errorLog;
        }


        // this method will work for Song Packs too!
        public IEnumerable<Manifest2014<Attributes2014>> ExtractJsonManifests()
        {
            var sw = new Stopwatch();
            sw.Restart();

            // every song contains gamesxblock but may not contain showlights.xml
            var xblockEntries = _archive.TOC.Where(x => x.Name.StartsWith("gamexblocks/nsongs") && x.Name.EndsWith(".xblock")).ToList();
            if (!xblockEntries.Any())
                throw new Exception("Could not find valid xblock file in archive.");

            var jsonData = new List<Manifest2014<Attributes2014>>();
            // this foreach loop addresses song packs otherwise it is only done one time
            foreach (var xblockEntry in xblockEntries)
            {
                // CAREFUL with use of Contains and Replace to avoid creating duplicates
                var strippedName = xblockEntry.Name.Replace(".xblock", "").Replace("gamexblocks/nsongs", "");
                if (strippedName.Contains("_fcp_dlc"))
                    strippedName = strippedName.Replace("fcp_dlc", "");

                var jsonEntries = _archive.TOC.Where(x => x.Name.StartsWith("manifests/songs") &&
                    x.Name.EndsWith(".json") && x.Name.Contains(strippedName)).OrderBy(x => x.Name).ToList();

                // looping through song multiple times gathering each arrangement
                foreach (var jsonEntry in jsonEntries)
                {
                    var dataObj = new Manifest2014<Attributes2014>();

                    _archive.InflateEntry(jsonEntry);
                    jsonEntry.Data.Position = 0;
                    var ms = new MemoryStream();
                    using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 65536)) //4Kb is default alloc size for windows .. 64Kb is default PSARC alloc
                    {
                        jsonEntry.Data.Position = 0;
                        jsonEntry.Data.CopyTo(ms);
                        ms.Position = 0;
                        var jsonObj = JObject.Parse(reader.ReadToEnd());
                        dataObj = JsonConvert.DeserializeObject<Manifest2014<Attributes2014>>(jsonObj.ToString());
                    }

                    jsonData.Add(dataObj);
                }
            }

            sw.Stop();
            GlobalExtension.ShowProgress(String.Format("{0} parsing json manifest entries took: {1} (msec)", Path.GetFileName(_filePath), sw.ElapsedMilliseconds));
            return jsonData;
        }

        public ManifestHeader2014<AttributesHeader2014> ExtractHsanManifest()
        {
            var sw = new Stopwatch();
            sw.Restart();
            // every song and song pack contain only one hsan file
            var hsanEntry = _archive.TOC.FirstOrDefault(x => x.Name.StartsWith("manifests/songs") && x.Name.EndsWith(".hsan"));

            if (hsanEntry == null)
                throw new Exception("Could not find valid hsan manifest in archive.");

            var hsanData = new ManifestHeader2014<AttributesHeader2014>(new Platform(GamePlatform.Pc, GameVersion.RS2014));
            _archive.InflateEntry(hsanEntry);
            var ms = new MemoryStream();
            using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 65536)) //4Kb is default alloc size for windows .. 64Kb is default PSARC alloc
            {
                hsanEntry.Data.Position = 0;
                hsanEntry.Data.CopyTo(ms);
                ms.Position = 0;
                var jsonObj = JObject.Parse(reader.ReadToEnd());
                hsanData = JsonConvert.DeserializeObject<ManifestHeader2014<AttributesHeader2014>>(jsonObj.ToString());
            }

            sw.Stop();
            GlobalExtension.ShowProgress(String.Format("{0} parsing hsan manifest entry took: {1} (msec)", Path.GetFileName(_filePath), sw.ElapsedMilliseconds));
            return hsanData;
        }

        public ToolkitInfo ExtractToolkitInfo()
        {
            var tkInfo = new ToolkitInfo();
            var toolkitVersionEntry = _archive.TOC.FirstOrDefault(x => (x.Name.Equals("toolkit.version")));

            if (toolkitVersionEntry != null)
            {
                _archive.InflateEntry(toolkitVersionEntry);
                tkInfo = GeneralExtension.GetToolkitInfo(new StreamReader(toolkitVersionEntry.Data));
            }
            else
            {
                // this helps prevent null exceptions
                tkInfo.ToolkitVersion = "Null";
                tkInfo.PackageAuthor = "Ubisoft";
                tkInfo.PackageVersion = "0";
                tkInfo.PackageComment = "Null";
                tkInfo.PackageRating = "5";
            }

            return tkInfo;
        }

        public string ExtractAppId()
        {
            var appId = String.Empty;

            var appIdEntry = _archive.TOC.FirstOrDefault(x => (x.Name.Equals("appid.appid")));
            if (appIdEntry != null)
            {
                _archive.InflateEntry(appIdEntry);

                using (var reader = new StreamReader(appIdEntry.Data))
                    appId = reader.ReadLine();
            }

            return appId;
        }

        // ===================== FOR FUTURE ====================

        public Bitmap ExtractAlbumArt(bool extractTaggerOrg)
        {
            Bitmap imageData = null;
            var sw = new Stopwatch();
            sw.Restart();

            //Func<Entry, bool> entryLINQ;
            //if (extractTaggerOrg)
            //    entryLINQ = entry => entry.Name == "tagger.org";
            //else
            //    entryLINQ = x => x.Name.Contains("256.dds");

            //var albumArtEntry = _archive.TOC.FirstOrDefault(entryLINQ);
            //if (albumArtEntry == null && extractTaggerOrg)
            //    RSTKTools.GlobalExtension.ShowProgress("Could not find tagger.org entry in archive.");

            //if (albumArtEntry != null)
            //{
            //    _archive.InflateEntry(albumArtEntry);
            //    var ms = new MemoryStream();
            //    using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 65536)) //4Kb is default alloc size for windows .. 64Kb is default PSARC alloc
            //    {
            //        albumArtEntry.Data.Position = 0;
            //        albumArtEntry.Data.CopyTo(ms);
            //        ms.Position = 0;

            //        var b = ImageExtensions.DDStoBitmap(ms);
            //        if (b != null)
            //            imageData = b;
            //    }
            //}

            sw.Stop();
            GlobalExtension.ShowProgress(String.Format("{0} parsing albumart entry took: {1} (msec)", Path.GetFileName(_filePath), sw.ElapsedMilliseconds));
            return imageData;
        }


        /// <summary>
        /// Convert wem archive entries to ogg files
        /// </summary>
        /// <param name="wems"></param>
        /// <param name="audioOggPath"></param>
        /// <param name="previewOggPath"></param>
        /// <returns></returns>
        public bool ConvertWemEntries(List<Entry> wems, string audioOggPath, string previewOggPath = "")
        {
            // TODO: Debug this untested revised code before first use

            bool result = false;

            if (wems.Count > 1)
            {
                wems.Sort((e1, e2) =>
                {
                    if (e1.Length < e2.Length)
                        return 1;
                    if (e1.Length > e2.Length)
                        return -1;
                    return 0;
                });
            }

            if (wems.Count > 0)
            {
                var top = wems[0]; // wem audio with internal TOC path
                var tempAudioPath = Path.Combine(Path.GetTempPath(), top.Name);
                top.Data.Position = 0;

                using (var fs = File.Create(tempAudioPath))
                {
                    top.Data.CopyTo(fs);
                    try
                    {
                        OggFile.Revorb(tempAudioPath, audioOggPath, Path.GetExtension(tempAudioPath).GetWwiseVersion());
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }

            if (!String.IsNullOrEmpty(previewOggPath) && result && wems.Count > 0)
            {
                var bottom = wems.Last();
                var tempAudioPath = Path.Combine(Path.GetTempPath(), bottom.Name);
                bottom.Data.Position = 0;
                using (var fs = File.Create(tempAudioPath))
                {
                    bottom.Data.CopyTo(fs);
                    try
                    {
                        OggFile.Revorb(tempAudioPath, previewOggPath, Path.GetExtension(tempAudioPath).GetWwiseVersion());
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
            if (_archive != null)
            {
                _archive.Dispose();
                _archive = null;
            }

            GC.SuppressFinalize(this);
        }




    }
}