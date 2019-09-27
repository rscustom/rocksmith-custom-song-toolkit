using System;
using System.IO;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.PSARC
{
    public class PsarcPackager : IDisposable
    {
        private string packageDir;
        private bool _deleteOnClose;

        /// <summary>
        /// Use for reading (unpacking) and writing (packing) packages as a composite in/out operation
        /// <para>Ensure proper usage of deleteOnClose: [false] for [read], and [true] for [write] operations</para>
        /// </summary>
        public PsarcPackager(bool deleteOnClose = false)
        {
            _deleteOnClose = deleteOnClose;
        }

        /// <summary>
        /// Loads DLCPackageData from unpacked archive (RS1 and RS2014 Compatible)
        /// </summary>
        /// <param name="fixMultitoneEx">convert multi tones to single tone, prevents some in-game hangs</param>
        /// <param name="fixLowBass">fix low bass tuning issues</param>
        /// <param name="decodeAudio">converts wem to ogg files</param>
        /// <returns>DLCPackageData</returns>
        public DLCPackageData ReadPackage(string srcPath, bool fixMultiTone = false, bool fixLowBass = false, bool decodeAudio = false)
        {
            // UNPACK
            packageDir = Packer.Unpack(srcPath, Path.GetTempPath(), decodeAudio: decodeAudio);

            // LOAD DLCPackageData
            var srcPlatform = Packer.GetPlatform(srcPath);
            DLCPackageData info = null;
            if (srcPlatform.version == GameVersion.RS2014)
            {
                // REORGANIZE (RS2014)
                packageDir = DLCPackageData.DoLikeProject(packageDir);
                info = DLCPackageData.LoadFromFolder(packageDir, srcPlatform, srcPlatform, fixMultiTone, fixLowBass);
            }
            else
                info = DLCPackageData.RS1LoadFromFolder(packageDir, srcPlatform, false);

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
                if (_deleteOnClose && Directory.Exists(packageDir))
                    IOExtension.DeleteDirectory(packageDir);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}