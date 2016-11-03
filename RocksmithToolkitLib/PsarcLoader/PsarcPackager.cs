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

        public DLCPackageData ReadPackage(string inputPath)
        {
            // UNPACK
            packageDir = Packer.Unpack(inputPath, Path.GetTempPath(), true);

            // REORGANIZE
            packageDir = DLCPackageData.DoLikeProject(packageDir);

            // LOAD DATA
            DLCPackageData info = new DLCPackageData();
            var packagePlatform = inputPath.GetPlatform();
            info = DLCPackageData.LoadFromFolder(packageDir, packagePlatform, packagePlatform);
            var foundShowlights = Directory.EnumerateFiles(packageDir, "*_showlights.xml", SearchOption.AllDirectories).Any();

            // toolkit will generate showlights if none was found
            if (!foundShowlights)
                info.Showlights = true;

            return info;
        }

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