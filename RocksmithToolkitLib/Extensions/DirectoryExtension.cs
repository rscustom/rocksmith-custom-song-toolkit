using System;
using System.IO;

namespace RocksmithToolkitLib.Extensions
{
    public static class DirectoryExtension
    {
        public static void Move(string sourceDirName, string destDirName)
        {
            #region Validation checks

            if (null == sourceDirName) { throw new ArgumentNullException("sourceDirName", "The source directory cannot be null."); }
            if (null == destDirName) { throw new ArgumentNullException("destDirName", "The destination directory cannot be null."); }
 
            sourceDirName = sourceDirName.Trim();
            destDirName = destDirName.Trim();

            if ((sourceDirName.Length == 0) || (destDirName.Length == 0)) { throw new ArgumentException("sourceDirName or destDirName is a zero-length string."); }

            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            if (sourceDirName.Contains(invalidChars)) { throw new ArgumentException("The directory contains invalid path characters.", "sourceDirName"); }
            if (destDirName.Contains(invalidChars)) { throw new ArgumentException("The directory contains invalid path characters.", "destDirName"); }

            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirName);
            DirectoryInfo destDir = new DirectoryInfo(destDirName);

            if (!sourceDir.Exists) { throw new DirectoryNotFoundException("The path specified by sourceDirName is invalid: " + sourceDirName); }
            if (destDir.Exists) { throw new IOException("The path specified by destDirName already exists: " + destDirName); }

            #endregion

            
            if (sourceDir.Root.Name.Equals(destDir.Root.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                Directory.Move(sourceDirName, destDirName);
            }
            else
            {
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
                    DirectoryExtension.Move(subDir.FullName, newPath);
                }

                Directory.Delete(sourceDirName, true);
            }
        }
    }
}