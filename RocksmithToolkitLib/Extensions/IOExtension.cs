using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Linq;

namespace RocksmithToolkitLib.Extensions
{
    public static class IOExtension
    {
        private const string MESSAGEBOX_CAPTION = "IOExtensions";

        public static void MoveDirectory(string sourceDirName, string destDirName, bool overwrite = false)
        {
            #region Validation checks

            if (String.IsNullOrWhiteSpace(sourceDirName)) { throw new ArgumentNullException("sourceDirName", "The source directory cannot be null."); }
            if (String.IsNullOrWhiteSpace(destDirName)) { throw new ArgumentNullException("destDirName", "The destination directory cannot be null."); }

            sourceDirName = sourceDirName.Trim();
            destDirName = destDirName.Trim();

            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            if (sourceDirName.Contains(invalidChars)) { throw new ArgumentException("The directory contains invalid path characters.", "sourceDirName"); }
            if (destDirName.Contains(invalidChars)) { throw new ArgumentException("The directory contains invalid path characters.", "destDirName"); }

            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirName);
            DirectoryInfo destDir = new DirectoryInfo(destDirName);

            if (!sourceDir.Exists) { throw new DirectoryNotFoundException("The path specified by sourceDirName is invalid: " + sourceDirName); }
            if (!overwrite)
                if (destDir.Exists) { throw new IOException("The path specified by destDirName already exists: " + destDirName); }

            #endregion

            if (sourceDir.Root.Name.Equals(destDir.Root.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (overwrite)
                    DeleteDirectory(destDirName, true);

                // make sure the destination subroot directory exists to avoid
                // throwing System.IO error 'Could not find a part of path' 
                if (!Directory.Exists(Path.GetDirectoryName(destDirName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destDirName));

                Directory.Move(sourceDirName, destDirName);
            }
            else
            {
                if (overwrite)
                    DeleteDirectory(destDirName, true);

                Directory.CreateDirectory(destDirName);

                FileInfo[] files = sourceDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string newPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(newPath);
                }

                DirectoryInfo[] subDirs = sourceDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string newPath = Path.Combine(destDirName, subDir.Name);
                    IOExtension.MoveDirectory(subDir.FullName, newPath);
                }

                Directory.Delete(sourceDirName, true);
            }
        }

        public static bool DeleteDirectory(string dirPath, bool includeSubDirs = true)
        {
            const int magicDust = 10;
            for (var gnomes = 1; gnomes <= magicDust; gnomes++)
            {
                try
                {
                    Directory.Delete(dirPath, includeSubDirs);
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                catch (IOException)
                {
                    // System.IO.IOException: The directory is not empty so delete as many files as possible
                    var files = Directory.EnumerateFiles(dirPath, "*", SearchOption.AllDirectories).ToList();
                    foreach (var file in files)
                        DeleteFile(file);

                    Debug.WriteLine("Gnomes prevent deletion of {0}! Applying magic dust, attempt #{1}.", dirPath, gnomes);
                    Thread.Sleep(50);
                    continue;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("Unauthorized access to: " + dirPath);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns rel paths.
        /// </summary>
        /// <param name="objectPath">Full path to the File or Folder</param>
        /// <param name="basePath">Base folder</param>
        /// <returns>Convert to rel path "absolutePath"</returns>
        public static string RelativeTo(this string objectPath, string basePath)
        {
            string[] absoluteDirectories = basePath.Split('\\');
            string[] relativeDirectories = objectPath.Split('\\');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then return w\o results
            if (lastCommonRoot == -1)
                return objectPath;

            //Build up the relative path
            var relativePath = new System.Text.StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append("..\\");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "\\");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }

        public static string AbsoluteTo(this string relPath, string basePath)
        {
            return Path.GetFullPath(Path.Combine(basePath, relPath));
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static bool MoveFile(string fileFrom, string fileTo, bool verbose = true)
        {
             if (File.Exists(fileTo))
                if (!verbose)
                    File.Delete(fileTo);
                else if (!PromptOverwrite(fileTo))
                    return false;
                else
                    File.Delete(fileTo);
            else
            {
                var fileToDir = Path.GetDirectoryName(fileTo);
                if (!Directory.Exists(fileToDir))
                    MakeDirectory(fileToDir);
            }

            try
            {
                File.Copy(fileFrom, fileTo);
                DeleteFile(fileFrom);
                return true;
            }
            catch (IOException e)
            {
                BetterDialog2.ShowDialog(
                    "Could not move the file " + fileFrom + "  Error Code: " + e.Message,
                    MESSAGEBOX_CAPTION, null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning ...", 150, 150);
                return false;
            }
        }

        public static bool DeleteFile(string filePath)
        {
            const int magicDust = 10;
            for (var gnomes = 1; gnomes <= magicDust; gnomes++)
            {
                try
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                    return true;
                }
                catch (FileNotFoundException)
                {
                    return false; // file does not exist
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                catch (IOException)
                {
                    // IOException: The process cannot access the file ...
                    Debug.WriteLine("Gnomes prevent deletion of {0}! Applying magic dust, attempt #{1}.", filePath, gnomes);
                    Thread.Sleep(50);
                    continue;
                }
            }

            return false;
        }
        public static bool CopyFile(string fileFrom, string fileTo, bool overWrite, bool verbose = true)
        {
            if (verbose)
                if (!PromptOverwrite(fileTo))
                    return false;
                else
                    overWrite = true;

            var fileToDir = Path.GetDirectoryName(fileTo);
            if (!Directory.Exists(fileToDir)) MakeDirectory(fileToDir);

            try
            {
                File.SetAttributes(fileFrom, FileAttributes.Normal);
                File.Copy(fileFrom, fileTo, overWrite);
                return true;
            }
            catch (IOException e)
            {
                if (!overWrite) return true; // be nice don't throw error
                BetterDialog2.ShowDialog(
                    "Could not copy file " + fileFrom + "\r\nError Code: " + e.Message +
                    "\r\nMake sure associated file/folders are closed.",
                    MESSAGEBOX_CAPTION, null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning ...", 150, 150);
                return false;
            }
        }

        public static bool MakeDirectory(string dirPath)
        {
            try
            {
                Directory.CreateDirectory(dirPath);
                return true;
            }
            catch (IOException e)
            {
                BetterDialog2.ShowDialog(
                    "Could not create the directory structure.  Error Code: " + e.Message,
                    MESSAGEBOX_CAPTION, null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning ...", 150, 150);
                return false;
            }
        }

        public static bool PromptOverwrite(string destPath)
        {
            if (File.Exists(destPath))
            {
                if (BetterDialog2.ShowDialog(Path.GetFileName(destPath) + @" already exists." +
                    Environment.NewLine + Environment.NewLine + @"Overwrite the existing file?", @"Warning: Overwrite File Message",
                    null, "Yes", "No", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning ...", 150, 150)
                    == DialogResult.No)
                    return false;
            }
            return true;
        }



    }
}