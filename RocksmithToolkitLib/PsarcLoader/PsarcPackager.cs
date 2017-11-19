using System;
using System.IO;
using System.Linq;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.PsarcLoader
{
    public class PsarcPackager : IDisposable
    {
        private string packageDir;
        private bool _deleteOnClose;

        public PsarcPackager(bool deleteOnClose = false)
        {
            _deleteOnClose = deleteOnClose;
        }

        /// <summary>
        /// Unpacks and Reads DLCPackageData
        /// </summary>
        /// <param name="fixMultitoneEx">convert multi tones to single tone, prevents some in-game hangs</param>
        /// <param name="fixLowBass">fix low bass tuning issues</param>
        /// <param name="decodeAudio">converts wem to ogg files</param>
        /// <returns>DLCPackageData</returns>
        public DLCPackageData ReadPackage(string srcPath, bool fixMultitoneEx = false, bool fixLowBass = false, bool decodeAudio = false)
        {
            // UNPACK
            packageDir = Packer.Unpack(srcPath, Path.GetTempPath(), decodeAudio);

            // REORGANIZE
            packageDir = DLCPackageData.DoLikeProject(packageDir);

            // LOAD DATA
            DLCPackageData info = new DLCPackageData();
            var packagePlatform = srcPath.GetPlatform();
            info = DLCPackageData.LoadFromFolder(packageDir, packagePlatform, packagePlatform, fixMultitoneEx, fixLowBass);

            return info;
        }

        /// <summary>
        /// Repacks DLCPackage Data
        /// </summary>
        /// <param name="srcPath">if provided (optional) sets the Platform to same as source file</param>
        public void WritePackage(string destPath, DLCPackageData packageData, string srcPath = "")
        {
            // if the srcPath does not exist GetPlatform returns 'None'
            // this generates an error condition so need to check platform of srcPath
            Platform platform;
            if (String.IsNullOrEmpty(srcPath))
                platform = destPath.GetPlatform();
            else
                platform = srcPath.GetPlatform();

            DLCPackageCreator.Generate(destPath, packageData, platform);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
                if (_deleteOnClose)
                    DirectoryExtension.SafeDelete(packageDir);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}