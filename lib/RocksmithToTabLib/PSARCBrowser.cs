using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.DLCPackage.Manifest2014;

namespace RocksmithToTabLib
{
    /// <summary>
    /// Reads in a Rocksmith PSARC archive and collects information on the 
    /// contained tracks. Can extract specific tracks for processing.
    /// </summary>
    public class PsarcBrowser
    {
        private PSARC archive;

        /// <summary>
        /// Create a new PsarcBrowser from a specified archive file.
        /// </summary>
        /// <param name="fileName">Path of the .psarc file to open.</param>
        public PsarcBrowser(string fileName)
        {
            archive = new PSARC();
            var stream = File.OpenRead(fileName);
            archive.Read(stream);
        }


        /// <summary>
        /// Retrieve a list of all song contained in the archive.
        /// Returned info includes song title, artist, album and year,
        /// as well as the available arrangements.
        /// </summary>
        /// <returns>List of included songs.</returns>
        public IList<SongInfo> GetSongList()
        {
            // Each song has a corresponding .json file within the archive containing
            // information about it.
            var infoFiles = archive.Entries.Where(x => x.Name.StartsWith("manifests/songs")
                && x.Name.EndsWith(".json")).OrderBy(x => x.Name);

            var songList = new List<SongInfo>();
            SongInfo currentSong = null;

            foreach (var entry in infoFiles)
            {
                // the entry's filename is identifier_arrangement.json
                var fileName = Path.GetFileNameWithoutExtension(entry.Name);
                var splitPoint = fileName.LastIndexOf('_');
                var identifier = fileName.Substring(0, splitPoint);
                var arrangement = fileName.Substring(splitPoint + 1);
                
                // temporary: exclude vocals from list until we can actually deal with them
                if (arrangement.ToLower().StartsWith("vocals") || arrangement.ToLower().StartsWith("jvocals"))
                    continue;

                if (currentSong == null || currentSong.Identifier != identifier)
                {
                    // extract song info from the .json file
                    using (var reader = new StreamReader(entry.Data.OpenStream()))
                    {
                        try
                        {
                            JObject o = JObject.Parse(reader.ReadToEnd());
                            var attributes = o["Entries"].First.Last["Attributes"];

                            currentSong = new SongInfo()
                            {
                                Title = attributes["SongName"].ToString(),
                                Artist = attributes["ArtistName"].ToString(),
                                ArtistSort = attributes["ArtistNameSort"].ToString(),
                                Album = attributes["AlbumName"].ToString(),
                                Year = attributes["SongYear"].ToString(),
                                Identifier = identifier,
                                Arrangements = new List<string>()
                            };
                            songList.Add(currentSong);
                        }
                        catch (NullReferenceException)
                        {
                            // It appears the vocal arrangements don't contain all the track
                            // information. Just ignore this.
                        }
                    }
                }

                currentSong.Arrangements.Add(arrangement);
            }

            return songList;
        }


        /// <summary>
        /// Extract a particular arrangement of a song from the archive
        /// and return the corresponding Song2014 object.
        /// </summary>
        /// <param name="identifier">Identifier of the song to load.</param>
        /// <param name="arrangement">The arrangement to use.</param>
        /// <returns>A Song2014 object containing the arrangement.</returns>
        public Song2014 GetArrangement(string identifier, string arrangement)
        {
            // In order to instantiate a Rocksmith Song2014 object, we need both
            // the binary .sng file and the attributes contained in the corresponding
            // .json manifest.
            Console.WriteLine(" Opening arrangement {1} for song id {0}...", identifier, arrangement);
            Platform platform = new Platform(GamePlatform.Pc, GameVersion.RS2014);
            var jsonFile = archive.Entries.FirstOrDefault(x => x.Name.StartsWith("manifests/songs") &&
                x.Name.EndsWith("/" + identifier + "_" + arrangement + ".json"));
            var sngFile = archive.Entries.FirstOrDefault(x => x.Name == "songs/bin/generic/" +
                identifier + "_" + arrangement + ".sng");
            if (sngFile == null)
            {
                // this might be a Mac archive, try with Mac path
                sngFile = archive.Entries.FirstOrDefault(x => x.Name == "songs/bin/macos/" +
                    identifier + "_" + arrangement + ".sng");
                platform.platform = GamePlatform.Mac;
            }
            if (sngFile == null || jsonFile == null)
            {
                if (sngFile == null)
                    Console.WriteLine("sngFile is null.");
                if (jsonFile == null)
                    Console.WriteLine("jsonFile is null.");
                return null;
            }

            // read out attributes from .json manifest
            Attributes2014 attr;
            using (var reader = new StreamReader(jsonFile.Data.OpenStream()))
            {
                var manifest = JsonConvert.DeserializeObject<Manifest2014<Attributes2014>>(
                    reader.ReadToEnd());
                if (manifest == null)
                    return null;
                attr = manifest.Entries.ToArray()[0].Value.ToArray()[0].Value;
            }

            // get contents of .sng file
            Sng2014File sng = Sng2014File.ReadSng(sngFile.Data.OpenStream(), platform);

            return new Song2014(sng, attr);
        }


        public ToolkitInfo GetToolkitInfo()
        {
            // see if there's a toolkit.version file inside the archive.
            // this will only be the case for CDLCs
            var infoFile = archive.Entries.FirstOrDefault(x => x.Name == "toolkit.version");
            if (infoFile == null)
                return null;

            var info = new ToolkitInfo();
            using (var reader = new StreamReader(infoFile.Data.OpenStream()))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    // we need to decipher what this line contains;
                    // older toolkit versions just put a single line with the version number
                    // newer versions put several lines in the format "key : value"
                    var tokens = line.Split(new char[] { ':' });
                    // trim all tokens of surrounding whitespaces
                    for (int i = 0; i < tokens.Length; ++i)
                        tokens[i] = tokens[i].Trim();

                    if (tokens.Length == 1)
                    {
                        // this is probably just the version number
                        info.ToolkitVersion = tokens[0];
                    }
                    else if (tokens.Length == 2)
                    {
                        // key/value attribute
                        var key = tokens[0].ToLower();
                        switch (key)
                        {
                            case "toolkit version":
                                info.ToolkitVersion = tokens[1]; break;
                            case "package author":
                                info.PackageAuthor = tokens[1]; break;
                            case "package version":
                                info.PackageVersion = tokens[1]; break;
                            default:
                                Console.WriteLine("  Notice: Unknown key in toolkit.version: {0}", key);
                                break;
                        }
                    }
                    else
                    {
                        // ???
                        Console.WriteLine("  Notice: Unrecognized line in toolkit.version: {0}", line);
                    }
                }
            }

            return info;
        }
    }


    /// <summary>
    /// Struct containing info about a single track.
    /// </summary>
    public class SongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtistSort { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Identifier { get; set; }
        public IList<string> Arrangements { get; set; }
    }

    /// <summary>
    /// For custom DLCs, provides information about the used Toolkit version,
    /// the CDLC's author and the package version.
    /// </summary>
    public class ToolkitInfo
    {
        public string ToolkitVersion { get; set; }
        public string PackageAuthor { get; set; }
        public string PackageVersion { get; set; }
    }
}
