/*
 * inspired from RSTabExplorer by andulv (https://github.com/andulv/RSTabExplorer)
 * modded for Rocksmith 2014 Tab Converter by Holger Frydrych http://github/fholger/RocksmithToTab 
 * this version customized for CSC Toolkit
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib.Song2014ToTab
{
    /// <summary>
    /// Reads in a Rocksmith PSARC archive and collects information on the 
    /// contained tracks. Can extract specific tracks for processing.
    /// </summary>
    public class PsarcBrowser
    {
        private PSARC.PSARC archive;
        private Platform platform;

        /// <summary>
        /// Loads archive file to memory.
        /// </summary>
        /// <param name="fileName">Path of the .psarc file to open.</param>
        public PsarcBrowser(string fileName)
        {
            archive = new PSARC.PSARC();
            platform = fileName.GetPlatform();
            using (var stream = File.OpenRead(fileName))
            {
                archive.Read(stream);
            }
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

                // exclude vocals from list
                if (arrangement.ToLower() == "vocals" || arrangement.ToLower() == "jvocals")
                    continue;

                // exclude showlights from list
                if (arrangement.ToLower() == "showlights" || arrangement.ToLower() == "jshowlights")
                    continue;

                if (currentSong == null || currentSong.Identifier != identifier)
                {
                    // extract song info from the .json file
                    using (var ms = new MemoryStream())
                    using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 1024))
                    {
                        try
                        {
                            entry.Data.CopyTo(ms);
                            entry.Data.Position = 0;
                            ms.Position = 0;
                            JObject o = JObject.Parse(reader.ReadToEnd());
                            var attributes = o["Entries"].First.Last["Attributes"];
                            var title = attributes["SongName"].ToString();
                            var artist = attributes["ArtistName"].ToString();
                            var album = attributes["AlbumName"].ToString();
                            var year = attributes["SongYear"].ToString();

                            currentSong = new SongInfo()
                                {
                                    Title = attributes["SongName"].ToString(),
                                    Artist = attributes["ArtistName"].ToString(),
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
        /// <param name="identifier">Identifier (short title, aka key) of the song to load.</param>
        /// <param name="arrangement">The arrangement (lead, rythum, bass) to use.</param>
        /// <returns>A Song2014 object containing the arrangement.</returns>
        public Song2014 GetArrangement(string identifier, string arrangement)
        {
            // In order to instantiate a Rocksmith Song2014 object, we need both
            // the binary .sng file and the attributes contained in the corresponding
            // .json manifest.
            Console.WriteLine("GetArrangement called with identifier [{0}], arrangement {{{1}}}", identifier,
                              arrangement);
            var sngFile = archive.Entries.FirstOrDefault(x => x.Name == "songs/bin/generic/" +
                                                                        identifier + "_" + arrangement + ".sng");
            var jsonFile = archive.Entries.FirstOrDefault(x => x.Name.StartsWith("manifests/songs") &&
                                                               x.Name.EndsWith("/" + identifier + "_" + arrangement +
                                                                               ".json"));
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
            using (var ms = new MemoryStream())
            using (var reader = new StreamReader(ms, new UTF8Encoding(), false, 1024))
            {
                jsonFile.Data.CopyTo(ms);
                ms.Position = 0;
                var manifest = JsonConvert.DeserializeObject<Manifest2014<Attributes2014>>(
                    reader.ReadToEnd());
                if (manifest == null)
                    return null;
                attr = manifest.Entries.ToArray()[0].Value.ToArray()[0].Value;
            }

            // get contents of .sng file
            Sng2014File sng = Sng2014File.ReadSng(sngFile.Data, platform);

            return new Song2014(sng, attr);
        }
    }

    /// <summary>
    /// Struct containing info about a single track.
    /// </summary>
    public class SongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Identifier { get; set; }
        public IList<string> Arrangements { get; set; }
    }

    /// <summary>
    /// Struct containing short info about a single track.
    /// </summary>
    public class SongInfoShort
    {
        public string Identifier { get; set; }
        public string Arrangement { get; set; }
    }

}