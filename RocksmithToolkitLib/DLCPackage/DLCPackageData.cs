using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Song2014ToTab;
using RocksmithToolkitLib.Xml;
using X360.STFS;

using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Sng;
using Tone = RocksmithToolkitLib.DLCPackage.Manifest.Tone.Tone;

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
        public string PackageVersion { get; set; }

        private List<XBox360License> xbox360Licenses;
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

        public List<Tone> Tones { get; set; }

        // Load RS1 CDLC into PackageCreator
        public static DLCPackageData RS1LoadFromFolder(string unpackedDir, Platform targetPlatform, bool convert)
        {
            var data = new DLCPackageData();
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();
            data.Tones = new List<Tone>();

            data.GameVersion = (convert ? GameVersion.RS2014 : GameVersion.RS2012);
            data.SignatureType = PackageMagic.CON;
            // set default volumes
            data.Volume = (float)-5.5; // - 7 default a little too quite
            data.PreviewVolume = data.Volume;

            //Load song manifest
            var songsManifestJson = Directory.GetFiles(unpackedDir, "songs.manifest.json", SearchOption.AllDirectories);
            if (songsManifestJson.Length < 1)
                throw new DataException("No songs.manifest.json file found.");
            if (songsManifestJson.Length > 1)
                throw new DataException("More than one songs.manifest.json file found.");

            List<Attributes> attr = new List<Attributes>();
            var songsManifest = Manifest.Manifest.LoadFromFile(songsManifestJson[0]).Entries.ToArray();

            for (int smIndex = 0; smIndex < songsManifest.Count(); smIndex++)
            {
                var smData = songsManifest[smIndex].Value.ToArray()[0].Value;
                attr.Add(smData);
            }

            if (attr.FirstOrDefault() == null)
                throw new DataException("songs.manifest.json file did not parse correctly.");

            // Fill SongInfo
            data.SongInfo = new SongInfo();
            data.SongInfo.SongDisplayName = attr.FirstOrDefault().SongName;
            data.SongInfo.SongDisplayNameSort = attr.FirstOrDefault().SongNameSort;
            data.SongInfo.Album = attr.FirstOrDefault().AlbumName;
            data.SongInfo.SongYear = (attr.FirstOrDefault().SongYear == 0 ? 2012 : attr.FirstOrDefault().SongYear);
            data.SongInfo.Artist = attr.FirstOrDefault().ArtistName;
            data.SongInfo.ArtistSort = attr.FirstOrDefault().ArtistNameSort;
            data.Name = attr.FirstOrDefault().SongKey;

            //Load tone manifest, even poorly formed tone_bass.manifest.json
            var toneManifestJson = Directory.GetFiles(unpackedDir, "*tone*.manifest.json", SearchOption.AllDirectories);
            if (toneManifestJson.Length < 1)
                throw new DataException("No tone.manifest.json file found.");

            // toolkit produces multiple tone.manifest.json files when packing RS1 CDLC files
            // rather than change toolkit behavior just merge manifest files for now
            if (toneManifestJson.Length > 1)
            {
                var mergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union };
                JObject toneObject1 = new JObject();

                foreach (var tone in toneManifestJson)
                {
                    JObject toneObject2 = JObject.Parse(File.ReadAllText(tone));
                    //(toneObject1.SelectToken("Entries") as JArray).Merge(toneObject2.SelectToken("Entries"));
                    toneObject1.Merge(toneObject2, mergeSettings);
                }

                toneManifestJson = new string[1];
                toneManifestJson[0] = Path.Combine(unpackedDir, "merged.tone.manifest");
                string json = JsonConvert.SerializeObject(toneObject1, Formatting.Indented);
                File.WriteAllText(toneManifestJson[0], json);
            }

            List<Tone> tones = new List<Tone>();
            var toneManifest = Manifest.Tone.Manifest.LoadFromFile(toneManifestJson[0]);

            for (int tmIndex = 0; tmIndex < toneManifest.Entries.Count(); tmIndex++)
            {
                var tmData = toneManifest.Entries[tmIndex];
                tones.Add(tmData);
            }

            data.Tones = tones;

            // Load xml arrangements
            var xmlFiles = Directory.GetFiles(unpackedDir, "*.xml", SearchOption.AllDirectories);
            if (xmlFiles.Length <= 0)
                throw new DataException("Can not find any XML arrangement files");

            List<Tone2014> tones2014 = new List<Tone2014>();
            foreach (var xmlFile in xmlFiles)
            {
                if (xmlFile.ToLower().Contains("metadata")) // skip DF file
                    continue;

                // some poorly formed RS1 CDLC use just "vocal"
                if (xmlFile.ToLower().Contains("vocal"))
                {
                    var voc = new Arrangement();
                    voc.Name = ArrangementName.Vocals;
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.ScrollSpeed = 20;
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.SongFile = new SongFile { File = "" };
                    voc.CustomFont = false;

                    // Add Vocal Arrangement
                    data.Arrangements.Add(voc);
                }
                else
                {
                    Attributes2014 attr2014 = new Attributes2014();
                    Song rsSong = new Song();
                    bool foundToneForArrangement = false;

                    using (var obj1 = new Rs1Converter())
                    {
                        rsSong = obj1.XmlToSong(xmlFile);
                        data.SongInfo.AverageTempo = (int)obj1.AverageBPM(rsSong);
                    }

                    // matchup rsSong, songs.manifest, and tone.manifest files
                    foreach (var arrangement in attr)
                    {
                        // apply best guesstimate matching, RS1 CDLC are very inconsistent
                        // TODO: improve accuracy possibly by matching .xblock data
                        string xmlArr = rsSong.Arrangement.ToLowerInvariant();
                        // var matchLead = Regex.Match(xmlArr.ToLower(), "^lead$", RegexOptions.IgnoreCase);
                        // if(matchLead.Success)

                        if (xmlArr.ToLower().Contains("guitar") && !xmlArr.ToLower().Equals("lead") && !xmlArr.ToLower().Equals("rhythm") && !xmlArr.ToLower().Equals("combo") && !xmlArr.ToLower().Equals("bass"))
                        {
                            if (xmlArr.ToUpper().Equals("PART REAL_GUITAR_22")) // 
                            {
                                if (arrangement.ArrangementName.ToLower().Contains("combo"))
                                    rsSong.Arrangement = arrangement.ArrangementName = "Rhythm";
                                else
                                    rsSong.Arrangement = arrangement.ArrangementName = "Lead";
                            }
                            else
                            {
                                if (arrangement.ArrangementName.ToLower().Contains("combo"))
                                    rsSong.Arrangement = arrangement.ArrangementName = "Rhythm";
                                else
                                    rsSong.Arrangement = arrangement.ArrangementName = "Lead";
                            }
                        }

                        if (xmlArr.ToLower().Contains("lead") && arrangement.ArrangementName.ToLower().Contains("rhythm"))
                            rsSong.Arrangement = arrangement.ArrangementName = "Lead";

                        if (xmlArr.ToLower().Contains("rhythm") && arrangement.ArrangementName.ToLower().Contains("lead"))
                            rsSong.Arrangement = arrangement.ArrangementName = "Rhythm";

                        if (xmlArr.ToLower().Contains("lead"))
                            if (!arrangement.ArrangementName.ToLower().Contains("lead"))
                                continue;
                        if (xmlArr.ToLower().Contains("rhythm"))
                            if (!arrangement.ArrangementName.ToLower().Contains("rhythm"))
                                continue;
                        if (xmlArr.ToLower().Contains("combo"))
                            if (!arrangement.ArrangementName.ToLower().Contains("combo"))
                                continue;
                        if (xmlArr.ToLower().Contains("bass"))
                            if (!arrangement.ArrangementName.ToLower().Contains("bass"))
                                continue;

                        if (rsSong.Part == arrangement.SongPartition || tones.Count == 1) // this is inaccurate for some
                        {
                            foreach (var tone in tones) // tone.manifest
                            {
                                if (String.IsNullOrEmpty(arrangement.EffectChainName))
                                    arrangement.EffectChainName = "Default";

                                if (arrangement.EffectChainName.ToLower() == tone.Key.ToLower() || tones.Count == 1) // ok
                                {
                                    if (convert)
                                    {
                                        using (var obj1 = new Rs1Converter())
                                            tones2014.Add(obj1.ToneToTone2014(tone));

                                        // added for consistent naming
                                        tone.Name = tones2014[tones2014.Count - 1].Name;
                                        tone.Key = tones2014[tones2014.Count - 1].Key;
                                    }

                                    // load attr2014 with RS1 mapped values for use by Arrangement()
                                    attr2014.Tone_Base = tone.Name;
                                    attr2014.ArrangementName = arrangement.ArrangementName;
                                    attr2014.CentOffset = 0;
                                    attr2014.DynamicVisualDensity = new List<float>() { 2 };
                                    attr2014.SongPartition = arrangement.SongPartition;
                                    attr2014.PersistentID = IdGenerator.Guid().ToString();
                                    attr2014.MasterID_RDV = RandomGenerator.NextInt();

                                    attr2014.ArrangementProperties = new SongArrangementProperties2014();

                                    if (arrangement.ArrangementName.ToLower().Contains("lead"))
                                    {
                                        attr2014.ArrangementType = 0;
                                        attr2014.ArrangementProperties.RouteMask = 1;
                                    }
                                    else if (arrangement.ArrangementName.ToLower().Contains("rhythm"))
                                    {
                                        attr2014.ArrangementType = 1;
                                        attr2014.ArrangementProperties.RouteMask = 2;
                                    }
                                    else if (arrangement.ArrangementName.ToLower().Contains("combo"))
                                    {
                                        attr2014.ArrangementType = 2;
                                        attr2014.ArrangementProperties.RouteMask = arrangement.EffectChainName.ToLower().Contains("lead") ? 1 : 2;
                                    }
                                    else if (arrangement.ArrangementName.ToLower().Contains("bass"))
                                    {
                                        attr2014.ArrangementType = 3;
                                        attr2014.ArrangementProperties.RouteMask = 4;
                                    }
                                    else
                                    {
                                        // some RS1 CDLC do not have a valid ArrangementName
                                        if (rsSong.Arrangement.ToLower().Contains("guitar"))
                                        {
                                            attr2014.ArrangementName = "Lead";
                                            attr2014.ArrangementType = 0;
                                            attr2014.ArrangementProperties.RouteMask = 1;
                                        }
                                        else if (rsSong.Arrangement.ToLower().Contains("bass"))
                                        {
                                            attr2014.ArrangementName = "Bass";
                                            attr2014.ArrangementType = 3;
                                            attr2014.ArrangementProperties.RouteMask = 4;
                                        }
                                        else // default to rhythm
                                        {
                                            attr2014.ArrangementName = "Rhythm";
                                            attr2014.ArrangementType = 1;
                                            attr2014.ArrangementProperties.RouteMask = 2;
                                        }
                                    }

                                    if (arrangement.Tuning == "E Standard")
                                        rsSong.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                                    else if (arrangement.Tuning == "DropD")
                                        rsSong.Tuning = new TuningStrings { String0 = -2, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                                    else if (arrangement.Tuning == "OpenG")
                                        rsSong.Tuning = new TuningStrings { String0 = -2, String1 = -2, String2 = 0, String3 = 0, String4 = 0, String5 = -2 };
                                    else if (arrangement.Tuning == "EFlat")
                                        rsSong.Tuning = new TuningStrings { String0 = -1, String1 = -1, String2 = -1, String3 = -1, String4 = -1, String5 = -1 };
                                    else // default to standard tuning
                                        rsSong.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };

                                    foundToneForArrangement = true;
                                    break;
                                }
                            }
                        }

                        if (foundToneForArrangement)
                            break;
                    }

                    if (!foundToneForArrangement)
                        Console.WriteLine(@"Could not determine the arrangement tone pairing");

                    // write the tones to file
                    using (var obj1 = new Rs1Converter())
                        obj1.SongToXml(rsSong, xmlFile, true);

                    if (convert) // RS1 -> RS2
                    {
                        Song2014 rsSong2014 = new Song2014();

                        using (var obj1 = new Rs1Converter())
                            rsSong2014 = obj1.SongToSong2014(rsSong);

                        using (var obj2 = new Rs2014Converter())
                            obj2.Song2014ToXml(rsSong2014, xmlFile, true);
                    }

                    // Adding Song Arrangement
                    try
                    {
                        data.Arrangements.Add(new Arrangement(attr2014, xmlFile));
                    }
                    catch (Exception ex)
                    {
                        // mainly for the benifit of convert2012 CLI users
                        Console.WriteLine(@"This CDLC could not be auto converted." + Environment.NewLine + "You can still try manually adding the arrangements and assests." + Environment.NewLine + ex.Message);
                    }
                }
            }

            data.TonesRS2014 = tones2014;

            //Get Album Artwork DDS Files
            var artFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (artFiles.Length < 1)
                throw new DataException("No Album Artwork file found.");
            if (artFiles.Length > 1)
                throw new DataException("More than one Album Artwork file found.");

            var targetArtFiles = new List<DDSConvertedFile>();
            data.AlbumArtPath = artFiles[0];
            targetArtFiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = artFiles[0], destinationFile = artFiles[0].CopyToTempFile(".dds") });
            data.ArtFiles = targetArtFiles;

            //Audio files
            var targetAudioFiles = new List<string>();
            var audioFiles = Directory.GetFiles(unpackedDir, "*.ogg", SearchOption.AllDirectories);
            if (audioFiles.Length < 1)
                throw new DataException("No Audio file found.");
            if (audioFiles.Length > 2)
                throw new DataException("Too many Audio files found.");

            int i;
            for (i = 0; i < audioFiles.Length; i++)
            {
                if (convert && audioFiles[i].Contains("_fixed.ogg")) // use it
                    break;
                if (!convert && !audioFiles[i].Contains("_fixed.ogg"))
                    break;
            }

            var sourcePlatform = unpackedDir.GetPlatform();
            if (targetPlatform.IsConsole != (sourcePlatform = audioFiles[i].GetAudioPlatform()).IsConsole)
            {
                var newFile = Path.Combine(Path.GetDirectoryName(audioFiles[i]), String.Format("{0}_cap.ogg", Path.GetFileNameWithoutExtension(audioFiles[i])));
                OggFile.ConvertAudioPlatform(audioFiles[i], newFile);
                audioFiles[i] = newFile;
            }

            targetAudioFiles.Add(audioFiles[i]);

            if (!targetAudioFiles.Any())
                throw new DataException("Audio file not found.");

            FileInfo a = new FileInfo(audioFiles[i]);
            data.OggPath = a.FullName;

            //AppID
            if (!convert)
            {
                var appidFile = Directory.GetFiles(unpackedDir, "*APP_ID*", SearchOption.AllDirectories);
                if (appidFile.Length > 0)
                    data.AppId = File.ReadAllText(appidFile[0]);
            }
            else
                data.AppId = "248750";

            //Package version
            var versionFile = Directory.GetFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories);
            if (versionFile.Length > 0)
                data.PackageVersion = GeneralExtensions.ReadPackageVersion(versionFile[0]);
            else data.PackageVersion = "1";

            if (convert)
                data.Tones = null;

            return data;
        }

        #endregion

        #region RS2014 only

        public List<Tone2014> TonesRS2014 { get; set; }
        public float? PreviewVolume { get; set; }
        public string LyricArtPath { get; set; }

        // Cache art image conversion
        public List<DDSConvertedFile> ArtFiles { get; set; }

        /// <summary>
        /// Loads required DLC info from folder.
        /// </summary>
        /// <returns>The DLCPackageData info.</returns>
        /// <param name="unpackedDir">Unpacked dir.</param>
        /// <param name="targetPlatform">Target platform.</param>
        /// <param name = "sourcePlatform"></param>
        public static DLCPackageData LoadFromFolder(string unpackedDir, Platform targetPlatform, Platform sourcePlatform = null)
        {
            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            data.SignatureType = PackageMagic.CON;
            if (sourcePlatform == null)
                sourcePlatform = unpackedDir.GetPlatform();

            //Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            //Load files
            var jsonFiles = Directory.EnumerateFiles(unpackedDir, "*.json", SearchOption.AllDirectories).ToArray();
            foreach (var json in jsonFiles)
            {
                var attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;
                var xmlName = attr.SongXml.Split(':')[3];
                var xmlFile = Directory.EnumerateFiles(unpackedDir, xmlName + ".xml", SearchOption.AllDirectories).FirstOrDefault();

                if (attr.Phrases != null)
                {
                    if (data.SongInfo == null)
                    {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = (attr.SongVolume == 0 ? -12 : attr.SongVolume);
                        data.PreviewVolume = (attr.PreviewVolume ?? data.Volume);

                        // Fill SongInfo
                        data.SongInfo = new SongInfo();
                        data.SongInfo.SongDisplayName = attr.SongName;
                        data.SongInfo.SongDisplayNameSort = attr.SongNameSort;
                        data.SongInfo.Album = attr.AlbumName;
                        data.SongInfo.SongYear = attr.SongYear ?? 0;
                        data.SongInfo.Artist = attr.ArtistName;
                        data.SongInfo.ArtistSort = attr.ArtistNameSort;
                        data.SongInfo.AverageTempo = (int)attr.SongAverageTempo;
                    }

                    // Adding Tones
                    foreach (var jsonTone in attr.Tones)
                    {
                        if (jsonTone == null) continue;
                        var key = jsonTone.Key;
                        if (data.TonesRS2014.All(t => t.Key != key))
                        {
                            // fix tones names that do not have the correct alphacase for cross matching
                            if (attr.Tone_Base.ToLower() == jsonTone.Name.ToLower() && attr.Tone_Base != jsonTone.Name)
                                jsonTone.Name = attr.Tone_Base;
                            if (attr.Tone_A.ToLower() == jsonTone.Name.ToLower() && attr.Tone_A != jsonTone.Name)
                                jsonTone.Name = attr.Tone_A;
                            if (attr.Tone_B.ToLower() == jsonTone.Name.ToLower() && attr.Tone_B != jsonTone.Name)
                                jsonTone.Name = attr.Tone_B;
                            if (attr.Tone_C.ToLower() == jsonTone.Name.ToLower() && attr.Tone_C != jsonTone.Name)
                                jsonTone.Name = attr.Tone_C;
                            if (attr.Tone_D.ToLower() == jsonTone.Name.ToLower() && attr.Tone_D != jsonTone.Name)
                                jsonTone.Name = attr.Tone_D;

                            data.TonesRS2014.Add(jsonTone);
                        }
                    }

                    // Adding Arrangement
                    data.Arrangements.Add(new Arrangement(attr, xmlFile));
                }
                else if (xmlFile.ToLower().Contains("_vocals"))
                {
                    var voc = new Arrangement();
                    voc.Name = attr.JapaneseVocal == true ? ArrangementName.JVocals : ArrangementName.Vocals;
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.ScrollSpeed = 20;
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.SongFile = new SongFile { File = "" };
                    voc.CustomFont = attr.JapaneseVocal == true;

                    // Get symbols stuff from _vocals.xml
                    var fontSng = Path.Combine(unpackedDir, xmlName + ".sng");
                    var vocSng = Sng2014FileWriter.ReadVocals(xmlFile);

                    if (vocSng.IsCustomFont())
                    {
                        voc.CustomFont = true;
                        voc.FontSng = fontSng;
                        vocSng.WriteChartData(fontSng, new Platform(GamePlatform.Pc, GameVersion.None));
                    }

                    voc.Sng2014 = Sng2014File.ConvertXML(xmlFile, ArrangementType.Vocal, voc.FontSng);

                    // Adding Arrangement
                    data.Arrangements.Add(voc);
                }
            }

            //ShowLights XML
            var xmlShowLights = Directory.EnumerateFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(xmlShowLights))
            {
                var shl = new Arrangement();
                shl.ArrangementType = ArrangementType.ShowLight;
                shl.Name = ArrangementName.ShowLights;
                shl.SongXml = new SongXML { File = xmlShowLights };
                shl.SongFile = new SongFile { File = "" };

                // Adding ShowLights
                data.Arrangements.Add(shl);
            }

            //Get DDS Files
            var ddsFiles = Directory.EnumerateFiles(unpackedDir, "album_*.dds", SearchOption.AllDirectories).ToArray();
            if (ddsFiles.Any())
            {
                var ddsFilesC = new List<DDSConvertedFile>();
                foreach (var file in ddsFiles)
                    switch (Path.GetFileNameWithoutExtension(file).Split('_')[2])
                    {

                        case "256":
                            data.AlbumArtPath = file;
                            ddsFilesC.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                            break;
                        case "128":
                            ddsFilesC.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                            break;
                        case "64":
                            ddsFilesC.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                            break;
                    } data.ArtFiles = ddsFilesC;
            }

            // Lyric Art
            var LyricArt = Directory.EnumerateFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories).ToArray();
            if (LyricArt.Any())
                data.LyricArtPath = LyricArt.FirstOrDefault();

            //Get other files
            //Audio files
            var targetAudioFiles = new List<string>();
            var sourceAudioFiles = Directory.EnumerateFiles(unpackedDir, "*.wem", SearchOption.AllDirectories).ToArray();

            foreach (var file in sourceAudioFiles)
            {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                if (targetPlatform.IsConsole != (sourcePlatform = file.GetAudioPlatform()).IsConsole)
                {
                    OggFile.ConvertAudioPlatform(file, newFile);
                    targetAudioFiles.Add(newFile);
                }
                else targetAudioFiles.Add(file);
            }

            if (!targetAudioFiles.Any())
                throw new InvalidDataException("Audio files not found.");

            string audioPath = null, audioPreviewPath = null;
            FileInfo a = new FileInfo(targetAudioFiles[0]);
            FileInfo b = null;

            if (targetAudioFiles.Count == 2)
            {
                b = new FileInfo(targetAudioFiles[1]);

                if (a.Length > b.Length)
                {
                    audioPath = a.FullName;
                    audioPreviewPath = b.FullName;
                }
                else
                {
                    audioPath = b.FullName;
                    audioPreviewPath = a.FullName;
                }
            }
            else
                audioPath = a.FullName;

            data.OggPath = audioPath;

            //Make Audio preview with expected name when rebuild
            if (!String.IsNullOrEmpty(audioPreviewPath))
            {
                var newPreviewFileName = Path.Combine(Path.GetDirectoryName(audioPath), String.Format("{0}_preview{1}", Path.GetFileNameWithoutExtension(audioPath), Path.GetExtension(audioPath)));
                File.Move(audioPreviewPath, newPreviewFileName);
                data.OggPreviewPath = newPreviewFileName;
            }

            //AppID
            var appidFile = Directory.EnumerateFiles(unpackedDir, "*.appid", SearchOption.AllDirectories).FirstOrDefault();
            if (appidFile != null)
                data.AppId = File.ReadAllText(appidFile);

            //Package version
            var versionFile = Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            if (versionFile != null)
                data.PackageVersion = GeneralExtensions.ReadPackageVersion(versionFile);
            else data.PackageVersion = "1";

            return data;
        }

        #region RS2014 Inlay only

        [XmlIgnore]
        public InlayData Inlay { get; set; }

        #endregion

        // needs to be called after all packages for platforms are created
        public void CleanCache()
        {
            if (ArtFiles != null)
            {
                foreach (var file in ArtFiles)
                {
                    try
                    {
                        File.Delete(file.destinationFile);
                    }
                    catch { }
                }
                ArtFiles = null;
            }

            if (Arrangements != null)
                foreach (var a in Arrangements)
                    a.CleanCache();
        }

        ~DLCPackageData()
        {
            CleanCache();
        }

        /// <summary>
        /// Transforms unpacked Song into project-like folder structure.
        /// </summary>
        /// <returns>Output folder path.</returns>
        /// <param name="unpackedDir">Unpacked dir.</param>
        public static string DoLikeProject(string unpackedDir)
        {
            const string EOF = "EOF";
            const string KIT = "Toolkit";
            string outdir, eofdir, kitdir;
            string SongName = "SongName";
            string songVersion = "v0";

            // Get name for a new folder
            var jsonFiles = Directory.EnumerateFiles(unpackedDir, "*.json", SearchOption.AllDirectories).ToArray();
            var attr = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles[0]).Entries.ToArray()[0].Value.ToArray()[0].Value;
            var dirParts = unpackedDir.Split('_');
            if (dirParts.Length > 3)
                songVersion = dirParts[2];
            SongName = attr.FullName.Split('_')[0];

            //Create dir sruct
            outdir = Path.Combine(Path.GetDirectoryName(unpackedDir), String.Format("{0}_{1}_{2}", attr.ArtistName.GetValidSortName(), attr.SongName.GetValidSortName(), songVersion).Replace(" ", "-"));
            eofdir = Path.Combine(outdir, EOF);
            kitdir = Path.Combine(outdir, KIT);
            attr = null; //dispose

            // Don't work in same dir
            if (Directory.Exists(outdir))
            {
                if (outdir == unpackedDir)
                    return unpackedDir;
                DirectoryExtension.SafeDelete(outdir);
            }

            Directory.CreateDirectory(outdir);
            Directory.CreateDirectory(eofdir);
            Directory.CreateDirectory(kitdir);

            var xmlFiles = Directory.EnumerateFiles(unpackedDir, "*.xml", SearchOption.AllDirectories).ToArray();
            var sngFiles = Directory.EnumerateFiles(unpackedDir, "*vocals.sng", SearchOption.AllDirectories).ToArray();

            foreach (var json in jsonFiles)
            {
                var Name = Path.GetFileNameWithoutExtension(json);
                var xmlFile = xmlFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == Name);
                var sngFile = sngFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == Name);

                //Move all pair JSON\XML
                File.Move(json, Path.Combine(kitdir, Name + ".json"));
                File.Move(xmlFile, Path.Combine(eofdir, Name + ".xml"));
                if (Name.EndsWith("vocals", StringComparison.Ordinal))
                    File.Move(sngFile, Path.Combine(kitdir, Name + ".sng"));
            }

            // move showlights.xml
            var showlightPath = Directory.EnumerateFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).ToArray();
            if (showlightPath.Any())
                File.Move(showlightPath[0], Path.Combine(eofdir, Path.GetFileName(showlightPath[0])));

            //Move all art_size.dds to KIT folder
            var ArtFiles = Directory.EnumerateFiles(unpackedDir, "album_*_*.dds", SearchOption.AllDirectories).ToArray();
            if (ArtFiles.Any())
                foreach (var art in ArtFiles)
                    File.Move(art, Path.Combine(kitdir, Path.GetFileName(art)));
            var LyricArt = Directory.EnumerateFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories).ToArray();
            if (LyricArt.Any())
                foreach (var art in LyricArt)
                    File.Move(art, Path.Combine(kitdir, Path.GetFileName(art)));

            //Move ogg to EOF folder + rename
            var OggFiles = Directory.EnumerateFiles(unpackedDir, "*_fixed.ogg", SearchOption.AllDirectories).ToArray();
            if (!OggFiles.Any())
                throw new InvalidDataException("Audio files not found.");
            //TODO: read names from bnk and rename.
            var a0 = new FileInfo(OggFiles[0]);
            FileInfo b0 = null;
            if (OggFiles.Count() == 2)
            {
                b0 = new FileInfo(OggFiles[1]);

                if (a0.Length > b0.Length)
                {
                    File.Move(a0.FullName, Path.Combine(eofdir, SongName + ".ogg"));
                    File.Move(b0.FullName, Path.Combine(eofdir, SongName + "_preview.ogg"));
                }
                else
                {
                    File.Move(b0.FullName, Path.Combine(eofdir, SongName + ".ogg"));
                    File.Move(a0.FullName, Path.Combine(eofdir, SongName + "_preview.ogg"));
                }
            }
            else File.Move(a0.FullName, Path.Combine(eofdir, SongName + ".ogg"));

            //Move wem to KIT folder + rename
            var WemFiles = Directory.EnumerateFiles(unpackedDir, "*.wem", SearchOption.AllDirectories).ToArray();
            if (!WemFiles.Any())
                throw new InvalidDataException("Audio files not found.");

            var a1 = new FileInfo(WemFiles[0]);
            FileInfo b1 = null;
            if (WemFiles.Count() == 2)
            {
                b1 = new FileInfo(WemFiles[1]);

                if (a1.Length > b1.Length)
                {
                    File.Move(a1.FullName, Path.Combine(kitdir, SongName + ".wem"));
                    File.Move(b1.FullName, Path.Combine(kitdir, SongName + "_preview.wem"));
                }
                else
                {
                    File.Move(b1.FullName, Path.Combine(kitdir, SongName + ".wem"));
                    File.Move(a1.FullName, Path.Combine(kitdir, SongName + "_preview.wem"));
                }
            }
            else File.Move(a1.FullName, Path.Combine(kitdir, SongName + ".wem"));

            //Move Appid for correct template generation.
            var appidFile = Directory.EnumerateFiles(unpackedDir, "*.appid", SearchOption.AllDirectories).FirstOrDefault();
            if (appidFile != null)
                File.Move(appidFile, Path.Combine(kitdir, Path.GetFileName(appidFile)));

            //Move toolkit.version
            var toolkitVersion = Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            if (toolkitVersion != null)
                File.Move(toolkitVersion, Path.Combine(kitdir, Path.GetFileName(toolkitVersion)));

            //Remove old folder
            DirectoryExtension.SafeDelete(unpackedDir);

            return outdir;
        }
        #endregion
    }

    public class DDSConvertedFile
    {
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public string sourceFile { get; set; }
        public string destinationFile { get; set; }
    }

    public class InlayData
    {
        public string DLCSixName { get; set; }
        public string InlayPath { get; set; }
        public string IconPath { get; set; }
        public Guid Id { get; set; }
        public bool Frets24 { get; set; }
        public bool Colored { get; set; }

        public InlayData()
        {
            Id = IdGenerator.Guid();
        }
    }
}

