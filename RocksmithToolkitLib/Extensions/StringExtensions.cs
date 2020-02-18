using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage;


/*
Non-Sortable Artist, Title, Album Notes:
  Diacritics, Alpha, Numeric are allowed in any case combination
  Most special characters and puncuations are allowed with a few exceptions
  
Sortable Artist, Title, Album Notes:
  ( ) are always stripped
  / is replaced with a space
  - usage is inconsistent (so for consistency remove it)
  , is stripped (in titles)
  ' is not stripped
  . and ? usage are inconsistent (so for consistency leave these)
  Abbreviations/symbols like 'Mr.' and '&' are replaced with words
  Diacritics are replaced with their ASCII approximations if available

DLC Key, and Tone Key Notes:
  Limited to a maximum length of 30 charactures, minimum of 6 charactures for uniqueness
  Only Ascii Alpha and Numeric may be used
  No spaces, no special characters, no puncuation
  All alpha lower, upper, or mixed case are allowed
  All numeric is allowed
  
Reserved XML Characters:
  Double quotes usage must be escaped ==> &quot;
  Ampersand usage must be escaped ==> &amp;
  Dash usage must be escaped if not the first/last character ==> &#8211; or use "--"
*/

// "return value;" is used to aid with debugging validation methods

namespace RocksmithToolkitLib.Extensions
{
    public static class StringExtensions
    {
        #region Class Methods

        /// <summary>
        /// Capitalize the first character without changing the rest of the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Capitalize(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = string.Format("{0}{1}", value.Substring(0, 1).ToUpper(), value.Substring(1));
            return value;
        }

        public static string GetValidAcronym(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var v = Regex.Split(value, @"[\W\s]+").Where(r => !string.IsNullOrEmpty(r)).ToArray();
            if (v.Length > 1)
                return string.Join(string.Empty, v.Select(s => s[0])).ToUpper();

            value = value.ReplaceDiacritics();
            value = value.StripNonAlphaNumeric();
            return value;
        }

        public static string GetValidAppIdSixDigits(this string value)
        {            
            value = value.Trim();
            
            // social engineering code
            if (value.Equals("221680"))
                throw new InvalidDataException("<WARNING> Sentinel has detected futile human resistance ..." + Environment.NewLine +
                    "Buy Cherub Rock and you wont have to mess around changing AppId's.");

            // simple six digit number validation, eg. 248750
            // can be seven digits too eg. 1089163
            if (Regex.IsMatch(value, ("^([0-9]{6}|[0-9]{7})$")))
                return value;

            return "";
        }

        /// <summary>
        /// Gets a valid Artist, Title, Album (ATA) name with spaces
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValidAtaSpaceName(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // ODLC artist, title, album character use allows these
            // allow use of accents Über ñice \\p{L} diacritics
            // allow use of unicode punctuation \\p{P\\{S} not currently implimented
            // may need to be escaped \t\n\f\r#$()*+.?[\^{|  ... '-' needs to be escaped if not at the beginning or end of regex sequence
            // allow use of only these special characters \\-_ /&.:',!?()\"#
            // allow use of alphanumerics a-zA-Z0-9
            // tested and working ... Üuber!@#$%^&*()_+=-09{}][":';<>.,?/ñice 
            value = value.ReplaceSpecialCharacters();
            Regex rgx = new Regex("[^a-zA-Z0-9\\-_/&:',!.?()\"#\\p{L} ]*");
            value = rgx.Replace(value, "");
            // commented out because some ODLC have these
            // value = value.StripLeadingSpecialCharacters(); 
            return value;
        }

        public static string GetValidFileName(this string fileName)
        {
            fileName = fileName.Replace(",", ""); // remove commas even though valid
            fileName = fileName.StripExcessWhiteSpace();
            fileName = String.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return fileName;
        }

        public static string GetValidFilePath(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var fileName = Path.GetFileName(value);
            var pathName = Path.GetDirectoryName(value);
            fileName = fileName.GetValidFileName();
            pathName = pathName.GetValidPathName();
            value = Path.Combine(pathName, fileName);
            return value;
        }

        public static string GetValidInlayName(this string value, bool frets24 = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // remove all special characters, and leading numbers and replace spaces with underscore
            Regex rgx = new Regex("[^a-zA-Z0-9]_ ");
            value = rgx.Replace(value, "");
            value = value.StripLeadingNumbers();
            value = value.StripLeadingSpecialCharacters();

            // make sure (24) fret appears in the proper placement
            if (frets24)
            {
                if (value.Contains("24"))
                {
                    value = value.Replace("_24_", "_");
                    value = value.Replace("_24", "");
                    value = value.Replace("24_", "");
                    value = value.Replace(" 24 ", " ");
                    value = value.Replace("24 ", " ");
                    value = value.Replace(" 24", " ");
                    value = value.Replace("24", "");
                }
                value = value.Trim() + " 24";
            }

            value = value.ReplaceSpaceWith("_");
            return value;
        }

        /// <summary>
        /// Format string as valid DLCKey (aka SongKey), or ToneKey
        /// <para>CRITICAL: Provide 'songTitle' to prevent RS1 in-game hanging after tuning</para>
        /// </summary>
        /// <param name="value">DLCKey or ToneKey for verification</param>
        /// <param name="songTitle">optional SongTitle varification comparison for DLCKey </param>
        /// <param name="isTone">If set to <c>true</c> validate tone name and tone key</param>
        /// <returns>contains no spaces, no accents, or special characters but can begin with or be all numbers or all lower case</returns>
        public static string GetValidKey(this string value, string songTitle = "", bool isTone = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = value.StripNonAlphaNumeric();

            // CRITICAL: prevents RS1 in game hanging after tuning
            // check if same, if so then add 'Song' to make key unique, skip check if isTone
            if (value == songTitle.StripNonAlphaNumeric() && !isTone)
                value = "Song" + value;

            // limit max Key length to 30
            value = value.Substring(0, Math.Min(30, value.Length)).Trim();

            // ensure min DLCKey length is 6, skip check if isTone
            if (value.Length < 7 && !isTone)
            {
                value = string.Concat(Enumerable.Repeat(value, 6));
                value = value.Substring(0, 6);
            }

            return value;
        }

        /// <summary>
        /// Validate lyric character usage
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValidLyric(this string value)
        {
            // standard ODLC valid lyric character set, i.e., ã can not be used (confirmed by testing)
            //!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_abcdefghijklmnopqrstuvwxyz{|}~¡¢¥¦§¨ª«°²³´•¸¹º»¼½¾¿ÀÁÂÄÅÆÇÈÉÊËÌÎÏÑÒÓÔÖØÙÚÛÜÞßàáâäåæçèéêëìíîïñòóôöøùúûüŒœŠšž„…€™␀★➨
            string validSpecialCharacters = " !\"#$%&'()*+,-./:;<=>?@[\\]^_{|}~¡¢¥¦§¨ª«°²³´•¸¹º»¼½¾¿ÀÁÂÄÅÆÇÈÉÊËÌÎÏÑÒÓÔÖØÙÚÛÜÞßàáâäåæçèéêëìíîïñòóôöøùúûüŒœŠšž€™␀★➨";
            string validAlphaNumerics = "a-zA-Z0-9";

            Regex rgx = new Regex("[^" + validAlphaNumerics + validSpecialCharacters + "]*");
            value = rgx.Replace(value, "");
            return value;
        }

        public static string GetValidPathName(this string pathName)
        {
            pathName = String.Concat(pathName.Split(Path.GetInvalidPathChars()));
            return pathName;
        }

        /// <summary>
        /// Standard short file name format for CDLC file names "{0}_{1}_{2}"
        /// </summary>
        /// <param name="stringFormat"></param>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <param name="acronym">use artist acronym instead of full artist name</param>
        /// <returns></returns>
        public static string GetValidShortFileName(string artist, string title, string version, bool acronym = false)
        {
            if (String.IsNullOrEmpty(artist) || String.IsNullOrEmpty(title) || String.IsNullOrEmpty(version))
                throw new DataException("Artist, title, or version field is null or empty ...");

            // cleanup version numbering
            version = version.Replace(".", "_");

            string value;
            if (!acronym)
                value = String.Format("{0}_{1}_{2}", artist.GetValidAtaSpaceName(), title.GetValidAtaSpaceName(), version).Replace(" ", "-");
            else
                value = String.Format("{0}_{1}_{2}", artist.GetValidAcronym(), title.GetValidAtaSpaceName(), version).Replace(" ", "-");

            value = value.GetValidFileName().StripExcessWhiteSpace();
            return value;
        }

        public static string GetValidSortableName(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // processing order is important to achieve output like ODLC
            value = value.ReplaceSpecialCharacters();
            value = value.ReplaceAbbreviations();
            value = value.ReplaceDiacritics();
            value = value.StripSpecialCharacters();
            value = value.StripLeadingSpecialCharacters();
            value = value.ShortWordMover(); // "The Beatles" becomes "Beatles, The"
            value = value.Capitalize(); // "blink-182" becomes "Blink 182"
            value = value.StripExcessWhiteSpace();
            // remove periods from sortable fields, periods screw up file naming
            value = value.Replace(".", "");

            return value;
        }

        public static string GetValidTempo(this string value)
        {
            float tempo = 0;
            float.TryParse(value.Trim(), out tempo);
            int bpm = (int)Math.Round(tempo);
            // check for valid tempo
            if (bpm > 0 && bpm < 999)  // allow insane tempo
                return bpm.ToString();

            // return "120"; // do not use a default tempo as this causes problems elsewhere
            // force user to make entry rather than defaulting
            return "";
        }

        public static string GetValidVersion(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            Regex rgx = new Regex(@"^[\d\.]*");
            var match = rgx.Match(value);
            if (match.Success)
                return match.Value.Trim();

            // force user to make entry rather than defaulting
            return "";
        }

        public static bool IsVolumeValid(this float? value)
        {
            if (value == null)
                return false;

            // check for valid volume
            float volume = (float)Math.Round((double)value, 1);
            if (volume >= -45.0F && volume <= 45.0F)
                return true;

            return false;
        }

        public static float GetValidVolume(this float? value, float defaultVolume = -7.0F)
        {
            if (value == null)
                return defaultVolume;

            // check for valid volume
            float volume = (float)Math.Round((double)value, 1);
            if (volume >= -45.0F && volume <= 45.0F)
                return volume;

            // use default volume
            return defaultVolume;
        }

        public static string GetValidYear(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // check for valid four digit song year 
            if (!Regex.IsMatch(value, "^(15[0-9][0-9]|16[0-9][0-9]|17[0-9][0-9]|18[0-9][0-9]|19[0-9][0-9]|20[0-3][0-9])"))
                value = ""; // clear if not valid

            return value;
        }

        public static bool IsAppIdSixDigits(this string value)
        {
            if (String.IsNullOrEmpty(GetValidAppIdSixDigits(value)))
                return false;

            return true;
        }

        public static bool IsFilePathLengthValid(this string filePath)
        {
            if (Environment.OSVersion.Version.Major >= 6 && filePath.Length > 260)
                return false;

            if (Environment.OSVersion.Version.Major < 6 && filePath.Length > 215)
                return false;

            return true;
        }

        public static bool IsFilePathNameValid(this string filePath)
        {
            try
            {
                // check if filePath is null or empty
                if (String.IsNullOrEmpty(filePath))
                    return false;

                // check drive is valid
                var pathRoot = Path.GetPathRoot(filePath);
                if (!Directory.GetLogicalDrives().Contains(pathRoot))
                    return false;

                var fileName = Path.GetFileName(filePath);
                if (String.IsNullOrEmpty(fileName))
                    return false;

                var dirName = Path.GetDirectoryName(filePath);
                if (String.IsNullOrEmpty(dirName))
                    return false;

                if (dirName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static bool IsFilePathValid(this string filePath)
        {
            if (filePath.IsFilePathLengthValid())
                if (filePath.IsFilePathNameValid())
                    return true;

            return false;
        }

        [Obsolete("Deprecated, please use appropriate StringExtension methods.", true)]
        public static string ObsoleteGetValidName(this string value, bool allowSpace = false, bool allowStartsWithNumber = false, bool underscoreSpace = false, bool frets24 = false)
        {
            // TODO: allow some additonal special charaters but not many

            // valid characters developed from actually reviewing ODLC artist, title, album names
            string name = String.Empty;

            if (!String.IsNullOrEmpty(value))
            {
                // ODLC artist, title, album character use allows these but not these
                // allow use of accents Über ñice \\p{L}
                // allow use of unicode punctuation \\p{P\\{S} not currently implimented
                // may need to be escaped \t\n\f\r#$()*+.?[\^{|  ... '-' needs to be escaped if not at the beginning or end of regex sequence
                // allow use of only these special characters \\-_ /&.:',!?()\"#
                // allow use of alphanumerics a-zA-Z0-9
                // tested and working ... Üuber!@#$%^&*()_+=-09{}][":';<>.,?/ñice

                Regex rgx = new Regex((allowSpace) ? "[^a-zA-Z0-9\\-_ /&.:',!?()\"#\\p{L}]" : "[^a-zA-Z0-9\\-_/&.:',!?()\"#\\p{L} ]");
                name = rgx.Replace(value, "");

                Regex rgx2 = new Regex(@"^[\d]*\s*");
                if (!allowStartsWithNumber)
                    name = rgx2.Replace(name, "");

                // prevent names from starting with special characters -_* etc
                Regex rgx3 = new Regex("^[^A-Za-z0-9]*");
                name = rgx3.Replace(name, "");

                if (frets24)
                {
                    if (name.Contains("24"))
                    {
                        name = name.Replace("_24_", "_");
                        name = name.Replace("_24", "");
                        name = name.Replace("24_", "");
                        name = name.Replace(" 24 ", " ");
                        name = name.Replace("24 ", " ");
                        name = name.Replace(" 24", " ");
                        name = name.Replace("24", "");
                    }
                    name = name.Trim() + " 24";
                }

                if (underscoreSpace)
                    name = name.Replace(" ", "_");
            }

            return name.Trim();
        }

        public static string ReplaceAbbreviations(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // this does a better job of replacing diacretics and special characters
            value = value.Replace(" & ", " and ");
            value = value.Replace("&", " and ");
            value = value.Replace("/", " ");
            value = value.Replace("-", " "); // inconsistent usage sometimes removed, sometimes replaced
            value = value.Replace(" + ", " plus ");
            value = value.Replace("+", " plus ");
            value = value.Replace(" @ ", " at ");
            value = value.Replace("@", " at ");
            value = value.Replace("Mr.", "Mister");
            value = value.Replace("Mrs.", "Misses");
            value = value.Replace("Ms.", "Miss");
            value = value.Replace("Jr.", "Junior");

            return value;
        }

        public static string ReplaceDiacritics(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = Regex.Replace(value, "[ÀÁÂÃÅÄĀĂĄǍǺ]", "A");
            value = Regex.Replace(value, "[ǻǎàáâãäåąāă]", "a");
            value = Regex.Replace(value, "[ÇĆĈĊČ]", "C");
            value = Regex.Replace(value, "[çčćĉċ]", "c");
            value = Regex.Replace(value, "[ĎĐ]", "D");
            value = Regex.Replace(value, "[ďđ]", "d");
            value = Regex.Replace(value, "[ÈÉÊËĒĔĖĘĚ]", "E");
            value = Regex.Replace(value, "[ěèéêëēĕėę]", "e");
            value = Regex.Replace(value, "[ĜĞĠĢ]", "G");
            value = Regex.Replace(value, "[ģĝğġ]", "g");
            value = Regex.Replace(value, "[Ĥ]", "H");
            value = Regex.Replace(value, "[ĥ]", "h");
            value = Regex.Replace(value, "[ÌÍÎÏĨĪĬĮİǏ]", "I");
            value = Regex.Replace(value, "[ǐıįĭīĩìíîï]", "i");
            value = Regex.Replace(value, "[Ĵ]", "J");
            value = Regex.Replace(value, "[ĵ]", "j");
            value = Regex.Replace(value, "[Ķ]", "K");
            value = Regex.Replace(value, "[ķĸ]", "k");
            value = Regex.Replace(value, "[ĹĻĽĿŁ]", "L");
            value = Regex.Replace(value, "[ŀľļĺł]", "l");
            value = Regex.Replace(value, "[ÑŃŅŇŊ]", "N");
            value = Regex.Replace(value, "[ñńņňŉŋ]", "n");
            value = Regex.Replace(value, "[ÒÓÔÖÕŌŎŐƠǑǾ]", "O");
            value = Regex.Replace(value, "[ǿǒơòóôõöøōŏő]", "o");
            value = Regex.Replace(value, "[ŔŖŘ]", "R");
            value = Regex.Replace(value, "[ŗŕř]", "r");
            value = Regex.Replace(value, "[ŚŜŞŠ]", "S");
            value = Regex.Replace(value, "[şŝśš]", "s");
            value = Regex.Replace(value, "[ŢŤ]", "T");
            value = Regex.Replace(value, "[ťţ]", "t");
            value = Regex.Replace(value, "[ÙÚÛÜŨŪŬŮŰŲƯǓǕǗǙǛ]", "U");
            value = Regex.Replace(value, "[ǜǚǘǖǔưũùúûūŭůűų]", "u");
            value = Regex.Replace(value, "[Ŵ]", "W");
            value = Regex.Replace(value, "[ŵ]", "w");
            value = Regex.Replace(value, "[ÝŶŸ]", "Y");
            value = Regex.Replace(value, "[ýÿŷ]", "y");
            value = Regex.Replace(value, "[ŹŻŽ]", "Z");
            value = Regex.Replace(value, "[žźż]", "z");
            value = Regex.Replace(value, "[œ]", "oe");
            value = Regex.Replace(value, "[Œ]", "Oe");
            value = Regex.Replace(value, "[°]", "o");
            value = Regex.Replace(value, "[¡]", "!");
            value = Regex.Replace(value, "[¿]", "?");
            value = Regex.Replace(value, "[«»\u201C\u201D\u201E\u201F\u2033\u2036]", "\"");
            value = Regex.Replace(value, "[\u2026]", "...");

            return value;
        }

        public static string ReplaceDiacriticsFast(this string value)
        {
            // this does a good quick job of replacing diacretics
            // using "ISO-8859-8" gives better results than ""ISO-8859-1"
            byte[] byteOuput = Encoding.GetEncoding("ISO-8859-8").GetBytes(value);
            var result = Encoding.GetEncoding("ISO-8859-8").GetString(byteOuput);
            return result;
        }

        /// <summary>
        /// Replace white space with user choice of replacement or remove all together
        /// </summary>
        /// <param name="value"></param>
        /// <param name="replacementValue">Default is underscore</param>
        /// <returns></returns>
        public static string ReplaceSpaceWith(this string value, string replacementValue = "_")
        {
            var result = Regex.Replace(value.Trim(), @"[\s]", replacementValue);
            return result;
        }


        public static string ReplaceSpecialCharacters(this string value)
        {
            // tilde not used in ODLC
            var result = value.Replace('~', '-');
            return result;
        }

        public static string RestoreCRLF(this string value)
        {
            // replace single lf with crlf
            return Regex.Replace(value, @"\r\n?|\n", "\r\n");
        }

        /// <summary>
        /// Moves short words like "The " from the begining of a string to the end ", The" 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="undoIt">Use to undo ShortWordMover strings</param>
        /// <returns></returns>
        public static string ShortWordMover(this string value, bool undoIt = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // Artist Sort may begin with "A ", e.g. 'A Flock of Seaguls'
            var shortWord = new string[] { "The ", "THE ", "the " };
            var newEnding = new string[] { ", The", ", THE", ", the" };

            for (int i = 0; i < shortWord.Length; i++)
            {
                if (undoIt)
                {
                    if (value.EndsWith(newEnding[i], StringComparison.InvariantCulture))
                        value = String.Format("{0}{1}", shortWord[i], value.Substring(0, value.Length - newEnding[i].Length - 1)).Trim();
                }
                else
                {
                    if (value.StartsWith(shortWord[i], StringComparison.InvariantCulture))
                        value = String.Format("{0}{1}", value.Substring(shortWord[i].Length, value.Length - shortWord[i].Length), newEnding[i]).Trim();
                }
            }

            return value;
        }

        public static string StripCRLF(this string value, string replacement = "")
        {
            // replace single lf and/or crlf
            return Regex.Replace(value, @"\r\n?|\n", replacement);
        }

        public static string StripDiacritics(this string value)
        {
            // test string = "áéíóúç";
            var result = Regex.Replace(value.Normalize(NormalizationForm.FormD), "[^A-Za-z| ]", String.Empty);

            return result;
        }

        public static string StripExcessWhiteSpace(this string value)
        {
            Regex rgx = new Regex("[ ]{2,}", RegexOptions.None);
            var result = rgx.Replace(value, " ");

            return result;
        }

        /// <summary>
        /// Strips non-printable characters and returns valid UTF8 XML
        /// </summary>
        public static Stream StripIllegalXMLChars(this string value)
        {
            const string pattern = @"[\x01-\x08\x0B-\x0C\x0E-\x1F\x7F-\x84\x86-\x9F]"; // XML1.1
            value = Regex.Replace(value, pattern, "", RegexOptions.IgnoreCase);

            return new MemoryStream(new UTF8Encoding(false).GetBytes(value));
        }


        public static string StripLeadingNumbers(this string value)
        {
            Regex rgx = new Regex(@"^[\d]*\s*");
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string StripLeadingSpecialCharacters(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            Regex rgx = new Regex("^[^A-Za-z0-9(]*");
            var result = rgx.Replace(value, "");
            return result;
        }

        /// <summary>
        /// removes all non alphanumeric and all white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(this string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9_]+");
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string StripSpecialCharacters(this string value)
        {
            // TEST ()½!$€£Test$€£()½!  ()½!Test()½!
            // value = Regex.Replace(value, "[`~#\\$€£*',.;:!?()[]\"{}/]", "");
            Regex rgx = new Regex("[^a-zA-Z0-9 _#:'.]"); // only these are acceptable
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string ToNullTerminatedAscii(this Byte[] bytes)
        {
            var result = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            return result;
        }

        public static string ToNullTerminatedUTF8(this Byte[] bytes)
        {
            var result = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
            return result;
        }

        public static string GetStringInBetween(this string strSource, string strBegin, string strEnd)
        {
            string result = "";
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    result = strSource.Substring(0, iEnd);
                }
            }
            return result;
        }

        /// <summary>
        /// Splits a text string so that it wraps to specified line length
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="lineLength"></param>
        /// <param name="splitOnSpace"></param>
        /// <returns></returns>
        public static string SplitString(string inputText, int lineLength, bool splitOnSpace = true)
        {
            var finalString = String.Empty;

            if (splitOnSpace)
            {
                var delimiters = new[] { " " }; // , "\\" };
                var stringSplit = inputText.Split(delimiters, StringSplitOptions.None);
                var charCounter = 0;

                for (int i = 0; i < stringSplit.Length; i++)
                {
                    finalString += stringSplit[i] + " ";
                    charCounter += stringSplit[i].Length;

                    if (charCounter > lineLength)
                    {
                        finalString += Environment.NewLine;
                        charCounter = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inputText.Length; i += lineLength)
                {
                    if (i + lineLength > inputText.Length)
                        lineLength = inputText.Length - i;

                    finalString += inputText.Substring(i, lineLength) + Environment.NewLine;
                }
                finalString = finalString.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return finalString;
        }

        /// <summary>
        /// Split contiguous string on caps and insert spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SplitCamelCase(string source)
        {
            string[] resultArray = Regex.Split(source, @"(?<!^)(?=[A-Z])");
            var result = String.Join(" ", resultArray).Trim();

            return result;
        }

        #endregion
    }
}
