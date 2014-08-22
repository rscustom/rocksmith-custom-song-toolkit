using System;
using System.IO;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.Song2014ToTab
{
    public class Rs1Converter : IDisposable
    {
        #region XML file to Song
        /// <summary>
        /// Convert XML file to RS1 (Song)
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>Song</returns>
        public Song XmlToSong(string xmlFilePath)
        {
            Song song = Song.LoadFromFile(xmlFilePath);
            return song;
        }
        #endregion

        #region Song to XML file
        /// <summary>
        /// Convert RS1 (Song) to XML file
        /// </summary>
        /// <param name="rs1Song"></param>
        /// <param name="outputDir"></param>
        /// <returns>RS1 XML file path</returns>
        public string SongToXml(Song rs1Song, string outputDir)
        {
            // apply consistent file naming
            var title = rs1Song.Title;
            var arrangement = rs1Song.Arrangement;
            int posOfLastDash = rs1Song.Title.LastIndexOf(" - ");
            if (posOfLastDash != -1)
            {
                title = rs1Song.Title.Substring(0, posOfLastDash);
                arrangement = rs1Song.Title.Substring(posOfLastDash + 3);
            }

            var outputFile = String.Format("{0}_{1}", title, arrangement);
            outputFile = String.Format("{0}{1}", outputFile.GetValidName(false, true), "_Rs1.xml");
            var outputPath = Path.Combine(outputDir, outputFile);
            if (File.Exists(outputPath)) File.Delete(outputPath);

            using (var stream = File.OpenWrite(outputPath))
            {
                rs1Song.Serialize(stream, false); ;
            }

            return outputPath;
        }
        #endregion

        #region Song to SngFile
        /// <summary>
        /// Converts Song to SngFile
        /// </summary>
        /// <param name="rs1Song"></param>
        /// <returns>SngFile</returns>
        public SngFile Song2SngFile(Song rs1Song, string outputDir)
        {
            var rs1SngPath = SongToSngFilePath(rs1Song, outputDir);
            SngFile sngFile = new SngFile(rs1SngPath);
            return sngFile;
        }
        #endregion

        #region Song to SngFilePath
        /// <summary>
        /// Converts Song to *.sng file
        /// </summary>
        /// <param name="rs1Song"></param>
        /// <returns>Path to binary *.sng file</returns>
        public string SongToSngFilePath(Song rs1Song, string outputDir)
        {
            string rs1XmlPath;
            using (var obj = new Rs1Converter())
                rs1XmlPath = obj.SongToXml(rs1Song, outputDir);

            ArrangementType arrangementType;
            if (rs1Song.Arrangement.ToLower() == "bass")
                arrangementType = ArrangementType.Bass;
            else
                arrangementType = ArrangementType.Guitar;

            var sngFilePath = Path.ChangeExtension(rs1XmlPath, ".sng");
            SngFileWriter.Write(rs1XmlPath, sngFilePath, arrangementType, new Platform(GamePlatform.Pc, GameVersion.None));

            if (File.Exists(rs1XmlPath)) File.Delete(rs1XmlPath);

            return sngFilePath;
        }
        #endregion

        #region SngFilePath to ASCII Tablature
        /// <summary>
        /// SngFilePath to ASCII Tablature
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDir"></param>
        /// <param name="allDif"></param>
        public void SngFilePathToAsciiTab(string inputFilePath, string outputDir, bool allDif)
        {
            using (var obj = new Sng2Tab())
                obj.Convert(inputFilePath, outputDir, allDif);
        }
        #endregion


        public void Dispose()
        {
        }

    }
}
