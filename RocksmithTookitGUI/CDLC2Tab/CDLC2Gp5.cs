using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.XML;
using RocksmithToTabLib;

namespace RocksmithToolkitGUI.CDLC2Tab
{
    public class CDLC2Gp5 : IDisposable
    {
        public void Dispose() { }

        #region PSARC to SongList

        /// <summary>
        /// create SongInfo List from PSARC file 
        /// with option to output to *.txt file
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDir is optional"></param>
        /// <returns>SongInfo List</returns>
        public IList<SongInfo> PsarcSongList(string inputFilePath, string outputDir = null)
        {
            var browser = new PsarcBrowser(inputFilePath);
            var songList = browser.GetSongList();

            if (outputDir != null)
            {
                var songInfo = String.Format("ARCHIVE  -  {0}  -  SONG LIST INFO", Path.GetFileName(inputFilePath));
                songInfo += Environment.NewLine + Environment.NewLine;
                songInfo += "[Song Identifier]  Artist - Title  (Album, Year)  {Arrangements}";
                songInfo += Environment.NewLine;
                songInfo += "----------------------------------------------------------------";
                songInfo += Environment.NewLine + Environment.NewLine;

                foreach (var song in songList)
                {
                    songInfo += String.Format("[{0}]  {1} - {2}  ({3}, {4})  {{{5}}}", song.Identifier,
                                              song.Artist, song.Title, song.Album, song.Year,
                                              string.Join(", ", song.Arrangements));
                    songInfo += Environment.NewLine;
                }
                songInfo += Environment.NewLine + Environment.NewLine;
                songInfo += "End of Report";

                var outputFile = Path.GetFileNameWithoutExtension(inputFilePath).ToLower();
                var outputPath = Path.Combine(outputDir, String.Format("{0}_songlist.txt", outputFile));

                using (TextWriter tw = new StreamWriter(outputPath))
                {
                    tw.Write(songInfo);
                }
            }

            return songList;
        }

        #endregion

        #region PSARC to Song2014 (for a specific song and/or arrangement)

        /// <summary>
        /// extract Song2014 from PSARC file for a specific
        /// songId (short song title) and arrangement (lead, rhythm, bass)
        /// defaults to the first songId and arrangement if not specified
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="songId"></param>
        /// <param name="arrangement"></param>
        /// <returns>Song2014</returns>
        public Song2014 PsarcToSong2014(string inputFilePath, string songId = null, string arrangement = null)
        {
            var browser = new PsarcBrowser(inputFilePath);
            var songList = browser.GetSongList();

            if (songId == null) // grab the first song.Identifier
            {
                songId = songList.FirstOrDefault().Identifier;
            }
            else // check if songId exists in song.Identifier
            {
                if (songList.FirstOrDefault(x => x.Identifier.Contains(songId)) == null)
                {
                    Console.WriteLine("Could not find songId: " + songId);
                    return null;
                }
            }

            if (arrangement == null) // grab the first song.Arrangment[0]
            {
                arrangement = songList.FirstOrDefault().Arrangements[0];
            }
            else // check if track exists in song.Arrangments
            {
                if (songList.FirstOrDefault(x => x.Arrangements.Contains(arrangement)) == null)
                {
                    Console.WriteLine("Could not find arrangement: " + arrangement);
                    return null;
                }
            }

            // push Song2014 into memory for this arrangement
            Song2014 arrSong2014 = browser.GetArrangement(songId, arrangement);
            Console.WriteLine("Pushed To Memory: [{0}] {{{1}}}", songId, arrangement);

            return arrSong2014;
        }

        #endregion
    
        #region Song2014 to GuitarPro *.gp5 file

        /// <summary>
        /// Load a PSARC file into memory and
        /// convert to GuitarPro file(s)
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDir"></param>
        /// <param name="songListShort"></param>
        /// <param name="outputFormat"></param>
        /// <param name="allDif"></param>
        public void PsarcToGp5(string inputFilePath, string outputDir, IList<SongInfoShort> songListShort = null, string outputFormat = "gp5", bool allDif = false)
        {
            Console.WriteLine("Opening archive {0} ...", inputFilePath);
            Console.WriteLine();

            try
            {
                var browser = new PsarcBrowser(inputFilePath);
                var songList = browser.GetSongList();
                var toolkitInfo = browser.GetToolkitInfo();

                // collect all songs to convert
                var toConvert = new List<SongInfo>();
                // if nothing was specified, convert all songs
                if (songListShort == null || songListShort.Count == 0)
                    toConvert = toConvert.Concat(songList).ToList();
                else
                    // convert only the user selected songs and arrangements
                    toConvert = SongInfoShortToSongInfo(songListShort, songList);

                foreach (var song in toConvert)
                {
                    var score = new Score();
                    // get all default or user specified arrangements for the song 
                    var arrangements = song.Arrangements;
                    Console.WriteLine("Converting song " + song.Identifier + "...");

                    foreach (var arr in arrangements)
                    {
                        var arrangement = (Song2014)browser.GetArrangement(song.Identifier, arr);
                        // get maximum difficulty for the arrangement
                        var mf = new ManifestFunctions(GameVersion.RS2014);
                        int maxDif = mf.GetMaxDifficulty(arrangement);

                        if (allDif) // create separate file for each difficulty
                        {
                            for (int difLevel = 0; difLevel <= maxDif; difLevel++)
                            {
                                ExportArrangement(score, arrangement, difLevel, inputFilePath, toolkitInfo);
                                Console.WriteLine("Difficulty Level: {0}", difLevel);

                                var baseFileName = CleanFileName(
                                    String.Format("{0} - {1}", score.Artist, score.Title));
                                baseFileName += String.Format(" ({0})", arr);
                                baseFileName += String.Format(" (level {0:D2})", difLevel);

                                SaveScore(score, baseFileName, outputDir, outputFormat);
                                // remember to remove the track from the score again
                                score.Tracks.Clear();
                            }
                        }
                        else // combine maximum difficulty arrangements into one file
                        {
                            Console.WriteLine("Maximum Difficulty Level: {0}", maxDif);
                            ExportArrangement(score, arrangement, maxDif, inputFilePath, toolkitInfo);
                        }
                    }

                    if (!allDif) // only maximum difficulty
                    {
                        var baseFileName = CleanFileName(
                            String.Format("{0} - {1}", score.Artist, score.Title));
                        SaveScore(score, baseFileName, outputDir, outputFormat);
                    }
                }

                Console.WriteLine();
            }

            catch (IOException e)
            {
                Console.WriteLine("Error encountered:");
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Load a XML arrangment into memory and
        /// convert to GuitarPro file
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDir"></param>
        /// <param name="outputFormat"></param>
        /// <param name="allDif"></param>
        public void XmlToGp5(string inputFilePath, string outputDir, string outputFormat = "gp5", bool allDif = false)
        {
            Console.WriteLine("Opening arrangement {0} ...", inputFilePath);
            Console.WriteLine();
            var score = new Score();
            var arrangement = Song2014.LoadFromFile(inputFilePath);
            var toolkitInfo = new ToolkitInfo();
            toolkitInfo.ToolkitVersion = String.Format("CST v{0}", ToolkitVersion.RSTKGuiVersion);
            toolkitInfo.PackageAuthor = "XML To GP5 Converter";
            toolkitInfo.PackageVersion = arrangement.LastConversionDateTime;

            var comments = Song2014.ReadXmlComments(inputFilePath);
            foreach (var xComment in comments)
            {
                if (xComment.Value.Contains("CST"))
                {
                    toolkitInfo.ToolkitVersion = xComment.Value.Trim();
                    break;
                }
            }

            // get maximum difficulty for the arrangement
            var mf = new ManifestFunctions(GameVersion.RS2014);
            int maxDif = mf.GetMaxDifficulty(arrangement);

            if (allDif) // create separate file for each difficulty
            {
                for (int difLevel = 0; difLevel <= maxDif; difLevel++)
                {
                    ExportArrangement(score, arrangement, difLevel, inputFilePath, toolkitInfo);
                    Console.WriteLine("Difficulty Level: {0}", difLevel);

                    var baseFileName = CleanFileName(
                        String.Format("{0} - {1}", score.Artist, score.Title));
                    baseFileName += String.Format(" ({0})", arrangement.Arrangement);
                    baseFileName += String.Format(" (level {0:D2})", difLevel);

                    SaveScore(score, baseFileName, outputDir, outputFormat);
                    // remember to remove the track from the score again
                    score.Tracks.Clear();
                }
            }
            else // combine maximum difficulty arrangements into one file
            {
                Console.WriteLine("Maximum Difficulty Level: {0}", maxDif);
                ExportArrangement(score, arrangement, maxDif, inputFilePath, toolkitInfo);
            }

            if (!allDif) // only maximum difficulty
            {
                var baseFileName = CleanFileName(
                    String.Format("{0} - {1}", score.Artist, score.Title));
                SaveScore(score, baseFileName, outputDir, outputFormat);
            }

        }

        static void ExportArrangement(Score score, Song2014 arrangement, int difficulty,
               string originalFile, ToolkitInfo toolkitInfo)
        {
            var track = Converter.ConvertArrangement(arrangement, arrangement.Part.ToString(), difficulty);
            score.Tracks.Add(track);
            score.Title = arrangement.Title;
            score.Artist = arrangement.ArtistName;
            score.Album = arrangement.AlbumName;
            score.Year = arrangement.AlbumYear;
            score.Comments = new List<string>();
            score.Comments.Add("Generated by RocksmithToTab v" + VersionInfo.VERSION);
            score.Comments.Add("=> https://github.com/fholger/RocksmithToTab");
            score.Comments.Add("Created from archive: " + Path.GetFileName(originalFile));
            if (toolkitInfo != null && toolkitInfo.PackageAuthor != string.Empty)
            {
                score.Comments.Add("CDLC author:  " + toolkitInfo.PackageAuthor);
                score.Tabber = toolkitInfo.PackageAuthor;
            }
            if (toolkitInfo != null && toolkitInfo.PackageVersion != string.Empty)
                score.Comments.Add("CDLC version: " + toolkitInfo.PackageVersion);
        }


        static GpxExporter gpxExporter = new GpxExporter();
        static GP5File gp5Exporter = new GP5File();

        static void SaveScore(Score score, string baseFileName, string outputDirectory, string outputFormat)
        {
            string basePath = Path.Combine(outputDirectory, baseFileName);
            // create a separate file for each arrangement
            if (outputFormat == "gp5")
            {
                gp5Exporter.ExportScore(score, basePath + ".gp5");
            }
            else if (outputFormat == "gpif")
            {
                gpxExporter.ExportGpif(score, basePath + ".gpif");
            }
            else
            {
                gpxExporter.ExportGPX(score, basePath + ".gpx");
            }

        }


        static string CleanFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = fileName.Where(x => !invalidChars.Contains(x)).ToArray();
            return new string(cleaned);
        }
      
        #endregion

        #region SongInfoShort to SongInfo

        /// <summary>
        /// Convert SongInfoShort to SongInfo that contains only user selections or defaults
        /// </summary>
        /// <param name="songListShort"></param>
        /// <param name="songList"></param>
        /// <returns></returns>
        public List<SongInfo> SongInfoShortToSongInfo(IList<SongInfoShort> songListShort, IList<SongInfo> songList)
        {
            var songIdPre = String.Empty;
            var newSongList = new List<SongInfo>();
            var newSongNdx = 0;

            for (var i = 0; i < songListShort.Count(); i++)
            {
                var songIdShort = songListShort[i].Identifier;
                var arrangementShort = songListShort[i].Arrangement;

                if (songIdPre != songIdShort)
                {
                    // add the new song info
                    var songInfo = songList.FirstOrDefault(x => x.Identifier == songIdShort);
                    newSongList.Add(songInfo);
                    newSongNdx++;

                    // clear arrangements so we can add user selections
                    if (arrangementShort != null)
                    {
                        newSongList[newSongNdx - 1].Arrangements.Clear();
                        newSongList[newSongNdx - 1].Arrangements.Add(arrangementShort);
                    }
                }
                else if (songIdPre == songIdShort && arrangementShort != null)
                    newSongList[newSongNdx - 1].Arrangements.Add(arrangementShort);

                songIdPre = songIdShort;
            }

            return newSongList;
        }
        #endregion

    }

    /// <summary>
    /// Struct containing song Identifier and 
    /// song Arrangement information for a PSARC file
    /// </summary>
    public class SongInfoShort
    {
        public string Identifier { get; set; }
        public string Arrangement { get; set; }
    }

}
