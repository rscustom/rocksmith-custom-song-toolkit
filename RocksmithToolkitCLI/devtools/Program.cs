using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RocksmithToolkitLib.XML;
using System.Xml.Serialization;

namespace devtools
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoveCrapFromDecryptedFiles(@"C:\Projects\RS\RS XML Clean");
            RenameRsXmlFiles(@"C:\Projects\RS\RS XML Clean");
            Console.WriteLine("Finished - press any key to exit");
            Console.Read();
        }

        /// <summary>
        /// Truncates all files in the provided directory at their first 0 byte
        /// </summary>
        public static void RemoveCrapFromDecryptedFiles(string directory)
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                try
                {
                    string dirtyXml = File.ReadAllText(file);
                    int index = dirtyXml.IndexOf("</vocals>");
                    if (index >0) {
                        index += 9;
                    } else
                    {
                        index = dirtyXml.IndexOf(@"</song>") + 7;
                        if (index == 6)
                        {
                            continue;
                        }
                    }
                    File.WriteAllText(file, dirtyXml.Substring(0, index));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Truncates all files in the provided directory at their first 0 byte
        /// </summary>
        public static void RenameRsXmlFiles(string directory)
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                string newName = null;
                try
                {
                    try
                    {
                        using (var reader = new StreamReader(file))
                        {

                            var serializer = new XmlSerializer(typeof(Vocals));
                            var vocals = (Vocals)serializer.Deserialize(reader);
                            newName = "Vocals-";
                            foreach (var vocal in vocals.Vocal)
                            {
                                if (newName.Length > 50)
                                {
                                    break;
                                }
                                newName += vocal.Lyric;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        if (e.InnerException == null || !"<song xmlns=''> was not expected.".Equals(e.InnerException.Message))
                        {
                            throw;
                        }
                        using (var reader = new StreamReader(file))
                        {
                            var serializer = new XmlSerializer(typeof(Song));
                            var song = (Song)serializer.Deserialize(reader);
                            newName = string.Format("{0} - {1}", song.Title, song.Arrangement);
                        }
                    }
                    if (newName != null)
                    {
                        newName = newName.Replace('.', ' ').Replace('?', ' ') + ".xml";
                        if (newName != Path.GetFileName(file))
                        {
                            File.Move(file, Path.Combine(directory, newName));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
