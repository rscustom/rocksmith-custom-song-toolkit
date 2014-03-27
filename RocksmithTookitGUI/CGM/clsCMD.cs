using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.CGM
{
    /*
    Cozy1's custom code to generate random names, and run CMD instructions
    */

    internal class clsCMD
    {
        private const string MESSAGEBOX_CAPTION = "Class CMD of CMG";

        // must call Random() as external method to generate a new number each time
        private static readonly Random randNum = new Random();

        // Cozy1 favorite method - clean and simple, uses hex ranges
        public static string RandomName(int iLen)
        {
            var builder = new StringBuilder(iLen);
            for (int i = 0; i < iLen; i++)
            {
                builder.Append((char) randNum.Next(0x61, 0x7A));
                // Alpha Lower Case Only
            }
            return builder.ToString();
        }

        // simple way to get a long random number in a range
        public static long RandomLong(long lMin, long lMax)
        {
            return lMin + randNum.Next()%(lMax - lMin);
        }

        public static bool DeleteDir(string strDirPath, bool overWrite)
        {
            if (Directory.Exists(strDirPath))
            {
                try
                {
                    Directory.Delete(strDirPath, overWrite);
                }
                catch (IOException e)
                {
                    MessageBox.Show(
                            "Could not delete the directory structure.  Please close any folder/files if you have them open.  Error Code: " +
                            e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        public static bool MakeDir(string strDirPath)
        {           
                try
                {
                    Directory.CreateDirectory(strDirPath);
                    return true;
                }
                catch (IOException e)
                {
                    MessageBox.Show(
                            "Could not create the directory structure.  Error Code: " +
                            e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    return false;
                }
        }

        public static bool CopyFile(string fileFrom, string fileTo,
                                    bool overWrite)
        {
            try
            {
                File.Copy(fileFrom, fileTo, overWrite);
                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show(
                        "Could not copy the file " + fileFrom + "  Error Code: " +
                        e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                return false;
            }
        }

        public static bool MoveFile(string fileFrom, string fileTo)
        {
            try
            {
                File.Move(fileFrom, fileTo);
                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show(
                        "Could not move the file " + fileFrom + "  Error Code: " +
                        e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                return false;
            }
        }

        public static bool RunExtExe(string runFileExe, string strArgs = "")
        {
            Application.DoEvents();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            // startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = Path.GetDirectoryName(runFileExe);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = runFileExe;
            startInfo.Arguments = strArgs;

            try
            {
                // Start the process with the info specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show(
                        "Could  not run the file " + runFileExe +
                        "  Error Code: " + e.Message, MESSAGEBOX_CAPTION,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public static bool WriteTextFile(string fileName, string strText,
                                         bool bAppend = true)
        {
            try
            {
                // File.WriteAllText(fileName, strText); // stream of text
                TextWriter tw = new StreamWriter(fileName, bAppend);
                tw.Write(strText); // IMPORTANT no CRLF added to end
                // tw.WriteLine(strText);  // causes CRLF to be added
                tw.Close();
                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show(
                        "Could not create text file " + fileName +
                        "  Error Code: " + e.Message, MESSAGEBOX_CAPTION,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public static string String2Hex(string strInput)
        {
            byte[] bArray = Encoding.Default.GetBytes(strInput);
            var hexString = BitConverter.ToString(bArray);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static byte[] HexStr2ByteAry(string strHex)
        {
            return Enumerable.Range(0, strHex.Length)
                    .Where(x => x%2 == 0)
                    .Select(x => Convert.ToByte(strHex.Substring(x, 2), 16))
                    .ToArray();
        }

        public string GetResource(string resName)
        {
// very usefull, uncomment the next line to determine valid "resName" 
// string[] names = this.GetType().Assembly.GetManifestResourceNames();
//
            Assembly assem = Assembly.GetExecutingAssembly();
            var stream = assem.GetManifestResourceStream(resName);
            if (stream != null) 
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            MessageBox.Show("Error: Could not access resource file " + resName,
                            MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            return null;
        }
    }
}