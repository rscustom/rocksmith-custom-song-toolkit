using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.DLCPackage.XBlock;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.XML;
using X360.STFS;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Sng;
using Tone = RocksmithToolkitLib.DLCPackage.Manifest.Tone.Tone;
using RocksmithToolkitLib.Conversion;
using RocksmithToolkitLib.XmlRepository;
using System.Diagnostics;
using System.Drawing;

namespace RocksmithToolkitLib.DLCPackage
{

    public class DLCPackageData
    {
        private const string MESSAGEBOX_CAPTION = "DLCPackageData";
        private const float DEFAULT_AUDIO_VOLUME = -7.0f;
        private const float DEFAULT_PREVIEW_VOLUME = -5.0f;

        // DO NOT change these variable names ... there are hidden dependancies
        public GameVersion GameVersion;
        public bool Pc { get; set; }
        public bool Mac { get; set; }
        public bool XBox360 { get; set; }
        public bool PS3 { get; set; }
        public bool DefaultShowlights { get; set; }
        public string AppId { get; set; }
        public string Name { get; set; } // aka DLCKey <=> SongKey //TODO: implement deserialize here, with workaround for DLCKey=Name and rename Name to DLCKey finnaly!
        public SongInfo SongInfo { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; } // wem/ogg path
        public string OggPreviewPath { get; set; } // wem/ogg preview path
        public decimal OggQuality { get; set; }
        public List<Arrangement> Arrangements { get; set; }
        public float Volume { get; set; }
        public PackageMagic SignatureType { get; set; }
        public ToolkitInfo ToolkitInfo { get; set; }

        // loads the old toolkit version info from template (if any)
        // writes current toolkit version to package template file
        private string _version;
        public string Version
        {
            get
            {
                if (_version == null)
                    _version = String.Format("Toolkit Version {0}", ToolkitVersion.RSTKGuiVersion);
                return _version;
            }
            set
            {
                if (value == null)
                    _version = String.Format("Toolkit Version {0}", ToolkitVersion.RSTKGuiVersion);
                _version = value;
            }
        }

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
        public static DLCPackageData RS1LoadFromFolder(string unpackedDir, Platform platform, bool convert)
        {
            var data = new DLCPackageData();
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();
            data.Tones = new List<Tone>();

            data.GameVersion = (convert ? GameVersion.RS2014 : GameVersion.RS2012);
            data.SignatureType = PackageMagic.CON;

            // set default volumes
            data.Volume = DEFAULT_AUDIO_VOLUME;
            data.PreviewVolume = DEFAULT_PREVIEW_VOLUME;

            //Load song manifest
            string[] fileExt = new string[] { "songs.manifest.json", "songs_bass.manifest.json" };
            var songsManifestJson = Directory.EnumerateFiles(unpackedDir, "*", SearchOption.AllDirectories).Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
            if (!songsManifestJson.Any())
                throw new DataException("<CRITICAL ERROR> No songs.manifest.json file found." + Environment.NewLine +
                    "Please confirm " + Packer.RecycleUnpackedDir(unpackedDir) + " is an RS1 CDLC ..." + Environment.NewLine +
                    "The 'Game Version' must be properly set in the 'General Config' menu." + Environment.NewLine + Environment.NewLine);

            // newer RS1 may have two manifest files
            if (songsManifestJson.Count > 2)
                throw new DataException("<ERROR> More than two *.manifest.json files found." + Environment.NewLine + Environment.NewLine);

            var attrList = new List<Attributes>();
            foreach (var manifestFile in songsManifestJson)
            {
                var songsManifest = Manifest.Manifest.LoadFromFile(manifestFile).Entries.ToArray();
                for (int smIndex = 0; smIndex < songsManifest.Count(); smIndex++)
                {
                    var smData = songsManifest[smIndex].Value.ToArray()[0].Value;
                    attrList.Add(smData);
                }
            }

            attrList = attrList.Where(x => !x.ArrangementName.Equals("Vocals")).ToList();
            if (!attrList.Any())
                throw new DataException("<ERROR> songs.manifest.json file did not parse correctly.");

            var attrFirst = attrList.First();
            // Fill SongInfo
            data.SongInfo = new SongInfo();
            data.Name = attrFirst.SongKey;
            data.SongInfo.Artist = attrFirst.ArtistName;
            data.SongInfo.ArtistSort = StringExtensions.GetValidSortableName(String.IsNullOrEmpty(attrFirst.ArtistNameSort) ? attrFirst.ArtistName : attrFirst.ArtistNameSort);
            data.SongInfo.SongDisplayName = attrFirst.SongName;
            data.SongInfo.SongDisplayNameSort = StringExtensions.GetValidSortableName(String.IsNullOrEmpty(attrFirst.SongNameSort) ? attrFirst.SongName : attrFirst.SongNameSort);
            data.SongInfo.Album = attrFirst.AlbumName;
            data.SongInfo.AlbumSort = StringExtensions.GetValidSortableName(String.IsNullOrEmpty(attrFirst.AlbumNameSort) ? attrFirst.AlbumName : attrFirst.AlbumNameSort);
            data.SongInfo.SongYear = (attrFirst.SongYear == 0 ? 2012 : attrFirst.SongYear);
            data.SongInfo.AverageTempo = attrFirst.AverageTempo;

            //Load tone manifest, even poorly formed tone_bass.manifest.json
            var toneManifestJson = Directory.GetFiles(unpackedDir, "*tone*.manifest.json", SearchOption.AllDirectories);
            if (!toneManifestJson.Any())
                throw new DataException("<ERROR> Did not find any tone.manifest.json files ..." + Environment.NewLine + Environment.NewLine);

            // toolkit produces multiple tone.manifest.json files when packing RS1 CDLC files
            // rather than change toolkit behavior just merge manifest files
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
                toneManifestJson[0] = Path.Combine(unpackedDir, "merged.tone.manifest.json");
                string json = JsonConvert.SerializeObject(toneObject1, Formatting.Indented);
                File.WriteAllText(toneManifestJson[0], json);
            }

            var tones = new List<Tone>();
            var toneManifest = Manifest.Tone.Manifest.LoadFromFile(toneManifestJson[0]);

            for (int tmIndex = 0; tmIndex < toneManifest.Entries.Count(); tmIndex++)
            {
                var tmData = toneManifest.Entries[tmIndex];
                tones.Add(tmData);
            }

            data.Tones = tones;

            // Load AggregateGraph.nt 
            var songDir = Directory.EnumerateDirectories(unpackedDir, "*", SearchOption.AllDirectories).Where(di => !di.Contains("DLC_Tone_")).FirstOrDefault(); ;
            var aggFile = Directory.GetFiles(songDir, "*.nt", SearchOption.TopDirectoryOnly)[0];
            if (String.IsNullOrEmpty(aggFile))
                throw new FileNotFoundException("<ERROR> Did not find AggregateGraph.nt ..." + Environment.NewLine + Environment.NewLine);

            var aggGraphData = AggregateGraph.AggregateGraph.ReadFromFile(aggFile);

            // Load Exports\Songs\*.xblock
            var xblockDir = Path.Combine(songDir, "Exports\\Songs");
            var xblockFile = Directory.GetFiles(xblockDir, "*.xblock", SearchOption.TopDirectoryOnly)[0];
            var songsXblock = XblockX.LoadFromFile(xblockFile);

            // create project map for cross referencing arrangements with tones
            var projectMap = AggregateGraph.AggregateGraph.ProjectMap(aggGraphData, songsXblock, toneManifest);

            // Load xml arrangements
            var xmlFilePaths = Directory.GetFiles(unpackedDir, "*.xml", SearchOption.AllDirectories);
            if (xmlFilePaths.Length <= 0)
                throw new DataException("<ERROR> Can not find any XML arrangement files");

            foreach (var xmlFilePath in xmlFilePaths)
            {
                if (xmlFilePath.ToLower().Contains("metadata"))
                    continue;

                // some poorly formed RS1 CDLC use just "vocal"
                if (xmlFilePath.ToLower().Contains("vocal"))
                {
                    // Add Vocal Arrangement
                    data.Arrangements.Add(new Arrangement
                    {
                        ArrangementName = ArrangementName.Vocals,
                        ArrangementType = ArrangementType.Vocal,
                        ScrollSpeed = 20,
                        SongXml = new SongXML { File = xmlFilePath },
                        SongFile = new SongFile { File = "" },
                        CustomFont = false
                    });
                }
                else
                {
                    var attr = new Attributes();
                    var attr2014 = new Attributes2014();
                    var song2014 = Song2014.LoadFromFile(xmlFilePath);

                    // Detect Arrangement XML Version
                    var xmlVersion = GameVersion.None;
                    var verAttrib = Convert.ToInt32(song2014.Version);
                    if (verAttrib < 7) // RS1 format
                    {
                        xmlVersion = GameVersion.RS2012;
                    }
                    else if (verAttrib > 6) // RS2014 format 
                    {
                        // CAUTION newer RS1 may use RS2014 XML Arrangement format
                        xmlVersion = GameVersion.RS2014;
                    }
                    else
                        throw new DataException("<ERROR> Unknown Arrangement XML Version: " + verAttrib + Environment.NewLine);

                    // optimized tone matching effort using project mapping algo
                    var tone = new Tone();
                    if (projectMap.Count > 0)
                    {
                        var result = projectMap.First(m => String.Equals(Path.GetFileName(m.SongXmlPath), Path.GetFileName(xmlFilePath), StringComparison.CurrentCultureIgnoreCase));
                        tone = tones.First(t => t.Key == result.Tones[0]);

                        // works with newer RS1 multitone CDLC
                        attr = attrList.First(s => s.SongXml.ToLower().Contains(result.LLID));
                    }
                    else // tone matching is not enabled (single tone only)
                    {
                        tone = tones.First();
                        attr = attrList.First();
                    }

                    // Convert RS1 to RS2014 XML Arrangement
                    var tones2014 = new List<Tone2014>();
                    var destPath = xmlFilePath;

                    if (convert && xmlVersion == GameVersion.RS2012)
                    {
                        using (var obj = new Rs1Converter())
                        {
                            if (verAttrib < 7)
                                song2014 = obj.SongToSong2014(Song.LoadFromFile(xmlFilePath));

                            tones2014.Add(obj.ToneToTone2014(tone));
                        }

                        // get rid of duplicate tone names
                        tones2014 = tones2014.Where(p => p.Name != null)
                            .GroupBy(p => p.Name).Select(g => g.First()).ToList();

                        data.TonesRS2014 = tones2014;

                        // create a new Song2014 XML file name for converted arrangement
                        var srcDir = Path.GetDirectoryName(xmlFilePath);
                        var srcName = Path.GetFileNameWithoutExtension(xmlFilePath);
                        destPath = String.Format("{0}_{1}.xml", Path.Combine(srcDir, srcName), "CNVRT");
                    }

                    // calculate tempo
                    if (song2014.AverageTempo == 0.0F || data.SongInfo.AverageTempo == 0)
                    {
                        using (var obj1 = new Rs1Converter())
                            song2014.AverageTempo = obj1.AverageBPM(song2014);

                        data.SongInfo.AverageTempo = (int)song2014.AverageTempo;
                    }

                    if (song2014.ArrangementProperties == null)
                        song2014.ArrangementProperties = new SongArrangementProperties2014();

                    // use data from manefist where possible to improve Attributes2014 data
                    attr2014.ArrangementProperties = song2014.ArrangementProperties;
                    attr2014.Tone_Base = tone.Name;
                    attr2014.PersistentID = IdGenerator.Guid().ToString();
                    attr2014.MasterID_RDV = RandomGenerator.NextInt();
                    attr2014.CentOffset = String.IsNullOrEmpty(song2014.CentOffset) ? 0.0 : double.Parse(song2014.CentOffset);
                    attr2014.SongPartition = attr.SongPartition;
                    attr2014.DynamicVisualDensity = attr.DynamicVisualDensity == null ? new List<float>() { 2 } : attr.DynamicVisualDensity;
                    attr2014.ArrangementProperties.Represent = attr.RepresentativeArrangement ? 1 : 0;

                    if (attr.Tuning == "EStandard" || attr.Tuning == "E Standard")
                        attr2014.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                    else if (attr.Tuning == "DropD" || attr.Tuning == "Drop D")
                        attr2014.Tuning = new TuningStrings { String0 = -2, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                    else if (attr.Tuning == "OpenG" || attr.Tuning == "Open G")
                        attr2014.Tuning = new TuningStrings { String0 = -2, String1 = -2, String2 = 0, String3 = 0, String4 = 0, String5 = -2 };
                    else if (attr.Tuning == "EFlat" || attr.Tuning == "E Flat")
                        attr2014.Tuning = new TuningStrings { String0 = -1, String1 = -1, String2 = -1, String3 = -1, String4 = -1, String5 = -1 };
                    else // default to standard tuning
                    {
                        attr.Tuning = "E Standard";
                        attr2014.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                    }

                    // use data from manefist to improve Song2014 XML Arrangement data
                    song2014.Tuning = attr2014.Tuning;
                    song2014.Part = (short)attr2014.SongPartition;
                    song2014.ToneBase = attr2014.Tone_Base;

                    // RouteMask  None = 0, Lead = 1, Rhythm = 2, Any = 3, Bass = 4
                    // XML file names are usually meaningless to arrangement determination
                    if (song2014.Arrangement.ToLower().Contains("combo") || attr.ArrangementName.ToLower().Contains("combo"))
                    {
                        attr2014.ArrangementName = "Combo";
                        attr2014.ArrangementType = (int)ArrangementType.Guitar;
                        attr2014.ArrangementProperties.RouteMask = attr.EffectChainName.ToLower().Contains("lead") ? (int)RouteMask.Lead : (int)RouteMask.Rhythm;
                        attr2014.ArrangementProperties.PathLead = attr.EffectChainName.ToLower().Contains("lead") ? 1 : 0;
                        attr2014.ArrangementProperties.PathRhythm = attr.EffectChainName.ToLower().Contains("lead") ? 0 : 1;
                        attr2014.ArrangementProperties.PathBass = 0;
                    }
                    else if (song2014.Arrangement.ToLower().Contains("rhythm") || attr.ArrangementName.ToLower().Contains("rhythm"))
                    // || rsSong.Arrangement.ToLower().Contains("guitar"))
                    {
                        attr2014.ArrangementName = "Rhythm";
                        attr2014.ArrangementType = (int)ArrangementType.Guitar;
                        attr2014.ArrangementProperties.RouteMask = (int)RouteMask.Rhythm;
                        attr2014.ArrangementProperties.PathLead = 0;
                        attr2014.ArrangementProperties.PathRhythm = 1;
                        attr2014.ArrangementProperties.PathBass = 0;
                    }
                    else if (song2014.Arrangement.ToLower().Contains("lead") || attr.ArrangementName.ToLower().Contains("lead"))
                    {
                        attr2014.ArrangementName = "Lead";
                        attr2014.ArrangementType = (int)ArrangementType.Guitar;
                        attr2014.ArrangementProperties.RouteMask = (int)RouteMask.Lead;
                        attr2014.ArrangementProperties.PathLead = 1;
                        attr2014.ArrangementProperties.PathRhythm = 0;
                        attr2014.ArrangementProperties.PathBass = 0;
                    }
                    else if (song2014.Arrangement.ToLower().Contains("bass") || attr.ArrangementName.ToLower().Contains("bass"))
                    {
                        attr2014.ArrangementName = "Bass";
                        attr2014.ArrangementType = (int)ArrangementType.Bass;
                        attr2014.ArrangementProperties.RouteMask = (int)RouteMask.Bass;
                        attr2014.ArrangementProperties.PathLead = 0;
                        attr2014.ArrangementProperties.PathRhythm = 0;
                        attr2014.ArrangementProperties.PathBass = 1;
                    }
                    else
                    {
                        // default to Lead arrangement
                        attr2014.ArrangementName = "Lead";
                        attr2014.ArrangementType = (int)ArrangementType.Guitar;
                        attr2014.ArrangementProperties.RouteMask = (int)RouteMask.Lead;
                        attr2014.ArrangementProperties.PathLead = 1;
                        attr2014.ArrangementProperties.PathRhythm = 0;
                        attr2014.ArrangementProperties.PathBass = 0;

                        Console.WriteLine("RS1 -> RS2014 CDLC Conversion defaulted to 'Lead' arrangement");
                    }

                    // update ArrangementProperties
                    song2014.ArrangementProperties.RouteMask = attr2014.ArrangementProperties.RouteMask;
                    song2014.ArrangementProperties.PathLead = attr2014.ArrangementProperties.PathLead;
                    song2014.ArrangementProperties.PathRhythm = attr2014.ArrangementProperties.PathRhythm;
                    song2014.ArrangementProperties.PathBass = attr2014.ArrangementProperties.PathBass;
                    song2014.ArrangementProperties.StandardTuning = (attr.Tuning == "E Standard" ? 1 : 0);

                    // <note time="58.366" linkNext="0" accent="0" bend="0" fret="7" hammerOn="0" harmonic="0" hopo="0" ignore="0" leftHand="-1" mute="0" palmMute="0" pluck="-1" pullOff="0" slap="-1" slideTo="-1" string="3" sustain="0.108" tremolo="0" harmonicPinch="0" pickDirection="0" rightHand="-1" slideUnpitchTo="-1" tap="0" vibrato="0" />
                    if (song2014.Levels.Any(sl => sl.Notes.Any(sln => sln.Bend != 0)))
                        song2014.ArrangementProperties.Bends = 1;
                    if (song2014.Levels.Any(sl => sl.Notes.Any(sln => sln.Hopo != 0)))
                        song2014.ArrangementProperties.Hopo = 1;
                    if (song2014.Levels.Any(sl => sl.Notes.Any(sln => sln.SlideTo != -1)))
                        song2014.ArrangementProperties.Slides = 1;
                    if (song2014.Levels.Any(sl => sl.Notes.Any(sln => sln.Sustain > 0)))
                        song2014.ArrangementProperties.Sustain = 1;

                    // fixing times that are off
                    var lastEbeatsTime = song2014.Ebeats[song2014.Ebeats.Length - 1].Time;
                    var lastPhraseIterationsTime = song2014.PhraseIterations[song2014.PhraseIterations.Length - 1].Time;

                    // tested ... not source of in game hangs
                    // confirm last PhraseIterations time is less than last Ebeats time
                    if (lastPhraseIterationsTime > lastEbeatsTime)
                    {
                        song2014.PhraseIterations[song2014.PhraseIterations.Length - 1].Time = lastEbeatsTime;
                        song2014.Sections[song2014.Sections.Length - 1].StartTime = lastEbeatsTime;
                    }

                    // tested ... not source of in game hangs
                    // confirm SongLength at least equals last Ebeats time
                    if (song2014.SongLength < lastEbeatsTime)
                        song2014.SongLength = lastEbeatsTime;

                    // write changes to the XML Arrangement file
                    if (convert && xmlVersion == GameVersion.RS2012)
                    {
                        using (var obj2 = new Rs2014Converter())
                            obj2.Song2014ToXml(song2014, destPath, true);
                    }

                    // Adding Song Arrangement
                    try
                    {
                        data.Arrangements.Add(new Arrangement(attr2014, xmlFilePath));
                    }
                    catch (Exception ex)
                    {
                        // mainly for the benefit of convert2012 CLI users
                        Console.WriteLine("This CDLC could not be auto converted." + Environment.NewLine +
                            "You can still try manually adding arrangements and assets." + Environment.NewLine +
                            ex.Message);
                    }
                }
            }

            // Get Album Artwork DDS Files
            var artFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (artFiles.Length < 1)
                throw new DataException("<ERROR> No Album Artwork file found.");
            if (artFiles.Length > 1)
                throw new DataException("<ERROR> More than one Album Artwork file found.");

            var targetArtFiles = new List<DDSConvertedFile>();
            data.AlbumArtPath = artFiles[0];
            targetArtFiles.Add(new DDSConvertedFile()
                {
                    sizeX = 256,
                    sizeY = 256,
                    sourceFile = artFiles[0],
                    destinationFile = artFiles[0].CopyToTempFile(".dds")
                });

            data.ArtFiles = targetArtFiles;

            // Audio files
            var targetAudioFiles = new List<string>();
            var audioFiles = Directory.GetFiles(unpackedDir, "*.ogg", SearchOption.AllDirectories);
            if (audioFiles.Length < 1)
                throw new DataException("<ERROR> No Audio file found.");
            if (audioFiles.Length > 2)
                throw new DataException("<ERROR> Too many Audio files found.");

            int i;
            for (i = 0; i < audioFiles.Length; i++)
            {
                if (convert && audioFiles[i].Contains("_fixed.ogg")) // use it
                    break;
                if (!convert && !audioFiles[i].Contains("_fixed.ogg"))
                    break;
            }

            var sourcePlatform = unpackedDir.GetPlatform();
            if (platform.IsConsole != (sourcePlatform = audioFiles[i].GetAudioPlatform()).IsConsole)
            {
                var newFile = Path.Combine(Path.GetDirectoryName(audioFiles[i]), String.Format("{0}_cap.ogg", Path.GetFileNameWithoutExtension(audioFiles[i])));
                OggFile.ConvertAudioPlatform(audioFiles[i], newFile);
                audioFiles[i] = newFile;
            }

            targetAudioFiles.Add(audioFiles[i]);

            if (!targetAudioFiles.Any())
                throw new DataException("<ERROR> Audio file not found.");

            var a = new FileInfo(audioFiles[i]);
            data.OggPath = a.FullName;

            // AppID
            if (!sourcePlatform.IsConsole)
            {
                if (!convert)
                {
                    var appidFile = Directory.GetFiles(unpackedDir, "*APP_ID*", SearchOption.AllDirectories);
                    if (appidFile.Length > 0)
                        data.AppId = File.ReadAllText(appidFile[0]);
                }
                else
                    data.AppId = "248750";
            }

            // Package Info
            var versionFile = Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            if (versionFile != null)
                data.ToolkitInfo = GeneralExtension.ReadToolkitInfo(versionFile);
            else
                data.ToolkitInfo = new ToolkitInfo();

            if (String.IsNullOrEmpty(data.ToolkitInfo.PackageVersion))
                data.ToolkitInfo.PackageVersion = "1";

            if (convert)
            {
                data.ToolkitInfo.PackageComment += "(RS1 to RS2014 by CDLC Creator)";
                data.Tones = null;
            }
            else
            {
                // apply consistent tone name/key naming throughout to minimize tone issues
                foreach (var tone in data.Tones)
                {
                    tone.Name = tone.Name.GetValidAtaSpaceName();
                    tone.Key = tone.Name.GetValidKey(isTone: true);
                }
            }

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
        /// Loads CDLC artifacts from folder into DLCPackageData
        /// </summary>
        /// <returns>The DLCPackageData info.</returns>
        /// <param name="unpackedDir"></param>
        /// <param name="targetPlatform"></param>
        /// <param name = "sourcePlatform"></param>
        /// <param name="fixMultiTone">If set to <c>true</c> fix low bass tuning </param>
        /// <param name="fixLowBass">If set to <c>true</c> fix multitone exceptions </param>
        /// <param name="renameAudioPreview">If set to <c>true</c> rename preview audio with friendly name </param>
        public static DLCPackageData LoadFromFolder(string unpackedDir, Platform targetPlatform, Platform sourcePlatform = null, bool fixMultiTone = false, bool fixLowBass = false)
        {
            if (sourcePlatform == null || sourcePlatform.platform == GamePlatform.None || sourcePlatform.version == GameVersion.None)
                sourcePlatform = unpackedDir.GetPlatform();

            float? bnkAudioVolume = null;
            float? bnkPreviewVolume = null;
            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            data.SignatureType = PackageMagic.CON;
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

                        // Get song volume
                        bnkAudioVolume = attr.SongVolume;
                        bnkPreviewVolume = attr.PreviewVolume;

                        // Fill SongInfo
                        data.SongInfo = new SongInfo { JapaneseArtistName = attr.JapaneseArtistName, JapaneseSongName = attr.JapaneseSongName, SongDisplayName = attr.SongName, SongDisplayNameSort = attr.SongNameSort, Album = attr.AlbumName, AlbumSort = attr.AlbumNameSort, SongYear = attr.SongYear ?? 0, Artist = attr.ArtistName, ArtistSort = attr.ArtistNameSort, AverageTempo = (int)attr.SongAverageTempo };
                    }

                    // Adding Arrangement
                    data.Arrangements.Add(new Arrangement(attr, xmlFile, fixMultiTone, fixLowBass));

                    // Add Tuning Name
                    var tuningName = TuningDefinitionRepository.Instance.Detect(attr.Tuning, GameVersion.RS2014);
                    data.Arrangements.Last().Tuning = tuningName.UIName;

                    // make a list of tone names used in arrangements
                    var toneNames = new List<string>();
                    foreach (var arr in data.Arrangements)
                    {
                        if (!String.IsNullOrEmpty(arr.ToneA)) toneNames.Add(arr.ToneA);
                        if (!String.IsNullOrEmpty(arr.ToneB)) toneNames.Add(arr.ToneB);
                        if (!String.IsNullOrEmpty(arr.ToneC)) toneNames.Add(arr.ToneC);
                        if (!String.IsNullOrEmpty(arr.ToneD)) toneNames.Add(arr.ToneD);
                        if (!String.IsNullOrEmpty(arr.ToneBase)) toneNames.Add(arr.ToneBase);
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
                            if (attr.Tone_A != null && attr.Tone_A.ToLower() == jsonTone.Name.ToLower() && attr.Tone_A != jsonTone.Name)
                                jsonTone.Name = attr.Tone_A;
                            if (attr.Tone_B != null && attr.Tone_B.ToLower() == jsonTone.Name.ToLower() && attr.Tone_B != jsonTone.Name)
                                jsonTone.Name = attr.Tone_B;
                            if (attr.Tone_C != null && attr.Tone_C.ToLower() == jsonTone.Name.ToLower() && attr.Tone_C != jsonTone.Name)
                                jsonTone.Name = attr.Tone_C;
                            if (attr.Tone_D != null && attr.Tone_D.ToLower() == jsonTone.Name.ToLower() && attr.Tone_D != jsonTone.Name)
                                jsonTone.Name = attr.Tone_D;

                            // this is part of multitone exception handling auto convert to single tone arrangment
                            // make data.TonesRS2014 consistent with data.Arragment.Tones (toneNames)
                            if (toneNames.Contains(jsonTone.Name))
                                data.TonesRS2014.Add(jsonTone);
                        }
                    }
                }
                else if (xmlFile.ToLower().Contains("vocals")) // detect both jvocals and vocals
                {
                    var voc = new Arrangement
                        {
                            ArrangementName = attr.JapaneseVocal == true ? ArrangementName.JVocals : ArrangementName.Vocals,
                            ArrangementType = ArrangementType.Vocal,
                            ScrollSpeed = 20,
                            SongXml = new SongXML { File = xmlFile },
                            SongFile = new SongFile { File = "" },
                            CustomFont = attr.JapaneseVocal == true,
                            XmlComments = Song2014.ReadXmlComments(xmlFile)
                        };

                    // Get symbols stuff from vocals.xml
                    var fontSng = Path.Combine(unpackedDir, xmlName + ".sng");
                    var vocSng = Sng2014FileWriter.ReadVocals(xmlFile);

                    // TODO: explain/confirm function/usage of this conditional check
                    if (vocSng.IsCustomFont()) // always seems to be false, even for jvocals
                    {
                        voc.CustomFont = true;
                        voc.FontSng = fontSng;
                        vocSng.WriteChartData(fontSng, new Platform(GamePlatform.Pc, GameVersion.None));
                        throw new Exception("<CRITICAL ERROR> vocSng.IsCustomFont: " + xmlFile + Environment.NewLine + "Please report this error to the toolkit developers along with the song that you are currently working on." + Environment.NewLine + "This is important!");
                    }

                    voc.Sng2014 = Sng2014File.ConvertXML(xmlFile, ArrangementType.Vocal, voc.FontSng);

                    // Adding Arrangement
                    data.Arrangements.Add(voc);
                }
            }

            // Enumerate *.bnk files
            var bnkWemList = new List<BnkWemData>();
            var bnkFiles = Directory.EnumerateFiles(unpackedDir, "song_*.bnk", SearchOption.AllDirectories).ToList();
            if (!bnkFiles.Any()) // LOG, IGNORE, AND CONTINUE
            {
                var errMsg = "<WARNING> Did not find any *.bnk files ..." + Environment.NewLine + "You can still try loading an audio file by hand.  " + Environment.NewLine + Environment.NewLine;
                BetterDialog2.ShowDialog(errMsg, MESSAGEBOX_CAPTION, null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning ...", 150, 150);
            }
            else if (bnkFiles.Count > 2)
                throw new FileLoadException("<ERROR> Found too many *.bnk files ..." + Environment.NewLine + Environment.NewLine);
            else
            {
                // extract .bnk file data
                foreach (var bnkFile in bnkFiles)
                {
                    var bnkPlatform = sourcePlatform;
                    var sourceResult = SoundBankGenerator2014.ValidateBnkFile(bnkFile, sourcePlatform);
                    if (sourceResult.StartsWith("<ERROR>"))
                    {
                        // maybe this .bnk already has targetPlatform endians
                        var targetResult = SoundBankGenerator2014.ValidateBnkFile(bnkFile, targetPlatform);
                        if (targetResult.StartsWith("<ERROR>"))
                            throw new FormatException(sourceResult + Environment.NewLine + targetResult);

                        bnkPlatform = targetPlatform;
                    }

                    var bnkWemData = new BnkWemData { BnkFileName = bnkFile, WemFileId = SoundBankGenerator2014.ReadWemFileId(bnkFile, bnkPlatform), VolumeFactor = SoundBankGenerator2014.ReadVolumeFactor(bnkFile, bnkPlatform) };

                    bnkWemList.Add(bnkWemData);
                }

                // get volume from .bnk file
                if (bnkAudioVolume == null)
                    bnkAudioVolume = bnkWemList.Where(fn => !fn.BnkFileName.EndsWith("_preview.bnk")).Select(vf => vf.VolumeFactor).FirstOrDefault();

                if (bnkPreviewVolume == null)
                    bnkPreviewVolume = bnkWemList.Where(fn => fn.BnkFileName.EndsWith("_preview.bnk")).Select(vf => vf.VolumeFactor).FirstOrDefault();

                // validate bnk volumes
                var isAudioVolValid = bnkAudioVolume.IsVolumeValid(DEFAULT_AUDIO_VOLUME);
                var isPreviewVolValid = bnkPreviewVolume.IsVolumeValid(DEFAULT_PREVIEW_VOLUME);

                // use default volumes
                if (!isAudioVolValid || !isPreviewVolValid)
                {
                    bnkAudioVolume = DEFAULT_AUDIO_VOLUME;
                    bnkPreviewVolume = DEFAULT_PREVIEW_VOLUME;
                }
                else // use validated volumes
                {
                    bnkAudioVolume = bnkAudioVolume.GetValidVolume(DEFAULT_AUDIO_VOLUME);
                    bnkPreviewVolume = bnkPreviewVolume.GetValidVolume(DEFAULT_PREVIEW_VOLUME);
                }
            }

            // Use default volume if still null
            data.Volume = bnkAudioVolume ?? DEFAULT_AUDIO_VOLUME;
            data.PreviewVolume = bnkPreviewVolume ?? DEFAULT_PREVIEW_VOLUME;

            //ShowLights XML
            var xmlShowLights = Directory.EnumerateFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(xmlShowLights))
            {
                var shl = new Arrangement
                {
                    ArrangementType = ArrangementType.ShowLight,
                    ArrangementName = ArrangementName.ShowLights,
                    SongXml = new SongXML { File = xmlShowLights },
                    SongFile = new SongFile { File = "" },
                    XmlComments = Song2014.ReadXmlComments(xmlShowLights)
                };

                // Adding ShowLights
                data.Arrangements.Add(shl);
            }

            //Get DDS Files
            var ddsFiles = Directory.EnumerateFiles(unpackedDir, "album_*.dds", SearchOption.AllDirectories).ToArray();
            if (ddsFiles.Any())
            {
                var ddsFilesC = new List<DDSConvertedFile>();
                foreach (var file in ddsFiles)
                {
                    switch (Path.GetFileNameWithoutExtension(file).Split('_').Last())
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
                    }
                }

                data.ArtFiles = ddsFilesC;
            }

            // Lyric Art
            var lyricArt = Directory.EnumerateFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories).ToArray();
            if (lyricArt.Any())
                data.LyricArtPath = lyricArt.FirstOrDefault();

            // Audio Files
            // Give ogg files friendly names
            var fixedOggFiles = Directory.EnumerateFiles(unpackedDir, "*_fixed.ogg", SearchOption.AllDirectories).ToList();
            if (fixedOggFiles.Any())
            {
                if (fixedOggFiles.Count > 2)
                    throw new FileLoadException("<ERROR> Found too many *_fixed.ogg files ..." + Environment.NewLine + Environment.NewLine);

                // reset data.OggPath and data.OggPreviewPath
                data.OggPath = null;
                data.OggPreviewPath = null;
            }

            foreach (string fixedOggFile in fixedOggFiles)
            {
                foreach (var item in bnkWemList)
                {
                    if (Path.GetFileName(fixedOggFile).Contains(item.WemFileId))
                    {
                        // some old RS2014 CDLC only have main audio and no preview audio file
                        // these CDLC still may have two bnk files so need to catch that here
                        if (!File.Exists(fixedOggFile))
                            continue;

                        var friendlyFixedOggFile = Path.Combine(Path.GetDirectoryName(fixedOggFile), String.Format("{0}_fixed.ogg", Path.GetFileNameWithoutExtension(item.BnkFileName)));
                        File.Move(fixedOggFile, friendlyFixedOggFile);

                        if (Path.GetFileName(friendlyFixedOggFile).EndsWith("_preview.wem"))
                            data.OggPreviewPath = friendlyFixedOggFile;
                        else
                            data.OggPath = friendlyFixedOggFile;
                    }
                }
            }

            // Give wem files friendly names
            var wemFiles = Directory.EnumerateFiles(unpackedDir, "*.wem", SearchOption.AllDirectories).ToList();
            if (wemFiles.Any())
            {
                if (wemFiles.Count > 2)
                    throw new FileLoadException("<ERROR> Found too many *.wem files ..." + Environment.NewLine + Environment.NewLine);

                // reset data.OggPath and data.OggPreviewPath
                data.OggPath = null;
                data.OggPreviewPath = null;
            }

            foreach (string wemFile in wemFiles)
            {
                foreach (var item in bnkWemList)
                {
                    if (Path.GetFileName(wemFile).Contains(item.WemFileId))
                    {
                        var friendlyWemFile = Path.Combine(Path.GetDirectoryName(wemFile), Path.GetFileName(Path.ChangeExtension(item.BnkFileName, ".wem")));
                        File.Copy(wemFile, friendlyWemFile);

                        // both bnk files may reference the same wem file 
                        // where preview audio is the same as main audio
                        if (wemFiles.Count == 1)
                        {
                            data.OggPath = friendlyWemFile;
                            break;
                        }

                        // more efficient to use friendly name wem files with CDLC Creator
                        if (Path.GetFileName(friendlyWemFile).EndsWith("_preview.wem"))
                            data.OggPreviewPath = friendlyWemFile;
                        else
                            data.OggPath = friendlyWemFile;
                    }
                }
            }

            if (!wemFiles.Any() && !fixedOggFiles.Any())
                throw new InvalidDataException("<ERROR> Did not find any audio files found ..." + Environment.NewLine + Environment.NewLine);

            //AppID
            var appidFile = Directory.EnumerateFiles(unpackedDir, "*.appid", SearchOption.AllDirectories).FirstOrDefault();
            if (appidFile != null)
                data.AppId = File.ReadAllText(appidFile);

            // Package Info
            var versionFile = Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            if (versionFile != null)
                data.ToolkitInfo = GeneralExtension.ReadToolkitInfo(versionFile);
            else
            {
                data.ToolkitInfo = new ToolkitInfo();
                data.ToolkitInfo.PackageVersion = "0";
                data.ToolkitInfo.PackageAuthor = "Ubisoft";
                data.ToolkitInfo.PackageRating = "5";
            }

            return data;
        }

        #region RS2014 Inlay only

        [XmlIgnore]
        public InlayData Inlay { get; set; }

        #endregion

        // TODO: uncommon usage of dispose, took me 10 minutes to figure out where my files gone, aware of this. PS: needs to be called after all packages for platforms are created
        public void CleanCache()
        {
            if (ArtFiles != null)
            {
                foreach (var file in ArtFiles)
                {
                    if (file.sizeY == 0)
                        continue;
                    try
                    {
                        File.Delete(file.destinationFile);
                    }
                    catch { }
                }
                ArtFiles = null;
            }

            if (Arrangements == null) return;
            foreach (var a in Arrangements)
                a.ClearCache();
        }

        ~DLCPackageData()
        {
            CleanCache();
        }

        /// <summary>
        /// Transforms song artifacts into project-like folder structure
        /// unpackedDir is recycled (RS2014 ONLY)
        /// </summary>
        /// <param name="unpackedDir"></param>
        public static string DoLikeProject(string unpackedDir)
        {
            var platform = unpackedDir.GetPlatform();
            if (platform.version != GameVersion.RS2014)
                throw new FileLoadException("<ERROR> DoLikeProject method does not work with RS1 ..." + Environment.NewLine + Environment.NewLine);

            // Create temporary project directory structure
            var tmpProjectDir = Path.Combine(Path.GetTempPath(), "ProjectFiles", Path.GetFileName(unpackedDir));
            var eofDir = Path.Combine(tmpProjectDir, "EOF");
            var toolkitDir = Path.Combine(tmpProjectDir, "Toolkit");

            // Start clean/fresh
            IOExtension.DeleteDirectory(tmpProjectDir);
            IOExtension.MakeDirectory(tmpProjectDir);
            IOExtension.MakeDirectory(eofDir);
            IOExtension.MakeDirectory(toolkitDir);

            // Gather up the project files and songName
            var xmlFiles = Directory.EnumerateFiles(unpackedDir, "*.xml", SearchOption.AllDirectories).ToArray();
            var jsonFiles = Directory.EnumerateFiles(unpackedDir, "*.json", SearchOption.AllDirectories).ToArray();
            string songName = "SongName";
            var attr = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles[0]).Entries.ToArray()[0].Value.ToArray()[0].Value;
            songName = attr.FullName.Split('_')[0];
            attr = null; // dispose

            foreach (var json in jsonFiles)
            {
                // Move JSON
                var name = Path.GetFileNameWithoutExtension(json);
                IOExtension.MoveFile(json, Path.Combine(toolkitDir, name + ".json"));

                // Move matching XML
                var xmlFile = xmlFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name);
                if (!String.IsNullOrEmpty(xmlFile)) // in case there is no matching XML file
                    IOExtension.MoveFile(xmlFile, Path.Combine(eofDir, name + ".xml"));
            }

            // Move showlights.xml to EOF folder
            var showlightFile = Directory.EnumerateFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(showlightFile))
                IOExtension.MoveFile(showlightFile, Path.Combine(eofDir, Path.GetFileName(showlightFile)));

            // Move artwork png to EOF folder
            var artPngFiles = Directory.EnumerateFiles(unpackedDir, "*.png", SearchOption.AllDirectories).Where(fp => !Path.GetFileName(fp).Equals("Package Image.png") && !Path.GetFileName(fp).Equals("Content Image.png")).ToList();
            foreach (var pngFile in artPngFiles)
                IOExtension.MoveFile(pngFile, Path.Combine(eofDir, Path.GetFileName(pngFile)));

            // Convert tagger.org artwork to png and put into EOF folder
            // Move original dds artwork to Toolkit folder
            var taggerOrgFile = Directory.EnumerateFiles(unpackedDir, "tagger.org", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(taggerOrgFile))
            {
                ExternalApps.Dds2Png(taggerOrgFile, Path.Combine(eofDir, "album_org_256.png"));
                IOExtension.MoveFile(taggerOrgFile, Path.Combine(toolkitDir, "album_org_256.dds"));
            }

            // Move _fixed.ogg to EOF folder
            var oggFiles = Directory.EnumerateFiles(unpackedDir, "*_fixed.ogg", SearchOption.AllDirectories).ToList();
            foreach (var oggFile in oggFiles)
                IOExtension.MoveFile(oggFile, Path.Combine(eofDir, Path.GetFileName(oggFile)));

            // Move Xbox360 png files to Toolkit folder
            var xbox360PngFiles = Directory.EnumerateFiles(unpackedDir, "*.png", SearchOption.AllDirectories).Where(fp => Path.GetFileName(fp).Equals("Package Image.png") || Path.GetFileName(fp).Equals("Content Image.png")).ToList();
            foreach (var pngFile in xbox360PngFiles)
                IOExtension.MoveFile(pngFile, Path.Combine(toolkitDir, Path.GetFileName(pngFile)));

            // Move art_size.dds to Toolkit folder
            var artFiles = Directory.EnumerateFiles(unpackedDir, "album_*_*.dds", SearchOption.AllDirectories).ToList();
            foreach (var art in artFiles)
                IOExtension.MoveFile(art, Path.Combine(toolkitDir, Path.GetFileName(art)));

            var lyricArt = Directory.EnumerateFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories).ToList();
            foreach (var art in lyricArt)
                IOExtension.MoveFile(art, Path.Combine(toolkitDir, Path.GetFileName(art)));

            // Move song_*.bnk to Toolkit folder
            var bnkFiles = Directory.EnumerateFiles(unpackedDir, "song_*.bnk", SearchOption.AllDirectories).ToList();
            foreach (var bnkFile in bnkFiles)
                IOExtension.MoveFile(bnkFile, Path.Combine(toolkitDir, Path.GetFileName(bnkFile)));

            // Move wem to Toolkit folder
            var wemFiles = Directory.EnumerateFiles(unpackedDir, "*.wem", SearchOption.AllDirectories).ToList();
            if (!wemFiles.Any() && !oggFiles.Any())
                throw new InvalidDataException("<ERROR> Audio files not found ..." + Environment.NewLine + Environment.NewLine);

            foreach (string wemFile in wemFiles)
                IOExtension.MoveFile(wemFile, Path.Combine(toolkitDir, Path.GetFileName(wemFile)));

            // Move Appid for correct template generation.
            var appidFile = Directory.EnumerateFiles(unpackedDir, "*.appid", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(appidFile))
                IOExtension.MoveFile(appidFile, Path.Combine(toolkitDir, Path.GetFileName(appidFile)));

            // Move toolkit.version
            var toolkitVersion = Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            if (!String.IsNullOrEmpty(toolkitVersion))
                IOExtension.MoveFile(toolkitVersion, Path.Combine(toolkitDir, Path.GetFileName(toolkitVersion)));

            // Remove old unpackedDir
            IOExtension.DeleteDirectory(unpackedDir);
            // Move tmpProjectDir to unpackedDir
            IOExtension.MoveDirectory(tmpProjectDir, unpackedDir);

            return unpackedDir;
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

