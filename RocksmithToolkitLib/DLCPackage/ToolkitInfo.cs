using System;

namespace RocksmithToolkitLib.DLCPackage
{
    /// <summary>
    /// For CDLC provides information about Toolkit version,
    /// the package author, the package version and a comment.
    /// </summary>
    public class ToolkitInfo
    {
        public string ToolkitVersion { get; set; }
        public string PackageAuthor { get; set; }
        public string PackageVersion { get; set; }
        public string PackageComment { get; set; }
    }
}

