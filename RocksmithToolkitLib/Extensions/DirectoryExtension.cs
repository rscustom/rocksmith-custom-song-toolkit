using System;
using System.IO;

namespace RocksmithToolkitLib.Extensions
{
    public static class DirectoryExtension
    {
        public static void Move(string sourceDirName, string destDirName, bool overwrite = false)
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
                    SafeDelete(destDirName, true);
                Directory.Move(sourceDirName, destDirName);
            }
            else
            {
                if (overwrite)
                    SafeDelete(destDirName, true);
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

        public static void SafeDelete(string path, bool recursive = true)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive);
            }
            catch { /*Don't worry*/ }
        }

        /// <summary>
        /// Returns rel paths.
        /// </summary>
        /// <param name="objectPath">Full path to the File or Folder</param>
        /// <param name="basePath">Base folder</param>
        /// <returns>Convert to rel path "absolutePath"</returns>
        public static string RelativeTo(this string objectPath, string basePath )
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

            //If we didn't find a common prefix then return wo results
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
    }
}