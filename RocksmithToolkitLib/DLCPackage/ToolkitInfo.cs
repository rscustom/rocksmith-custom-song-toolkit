using System;

namespace RocksmithToolkitLib.DLCPackage
{
    /// <summary>
    /// For custom DLCs, provides information about the used Toolkit version,
    /// the CDLC's author and the package version.
    /// </summary>
    public class ToolkitInfo
    {
        public string ToolkitVersion { get; set; }
        public string PackageAuthor { get; set; }
        public string PackageVersion { get; set; }
    }
}

