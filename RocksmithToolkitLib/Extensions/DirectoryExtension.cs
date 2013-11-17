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

        /// <summary>
        /// Returns rel paths..
        /// </summary>
        /// <param name="BasePath">Base folder</param>
        /// <param name="ObjectPath">Full path to the File or Folder</param>
        /// <returns>Convert to rel path "absolutePath"</returns>
        public static string RelativeTo(this string BasePath, string ObjectPath)
        {
            string[] absoluteDirectories = BasePath.Split('\\');
            string[] relativeDirectories = ObjectPath.Split('\\');

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
                return ObjectPath;

            //Build up the relative path
            System.Text.StringBuilder relativePath = new System.Text.StringBuilder();

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

        public static string AbsoluteTo(this Uri baseUri, string path)
        {
            return new Uri(baseUri, path).AbsolutePath.Replace("%25", "%").Replace("%20", " ");
        }
    }
}