using System;
using System.IO;

namespace RocksmithToolkitLib.Extensions
{
    public class TempFileStream : FileStream
    {
        public TempFileStream()
            : base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose) { }

        public TempFileStream(FileAccess access)
            : base(Path.GetTempFileName(), FileMode.Create, access, FileShare.Read, 4096, FileOptions.DeleteOnClose) { }

        public TempFileStream(FileAccess access, FileShare share)
            : base(Path.GetTempFileName(), FileMode.Create, access, share, 4096, FileOptions.DeleteOnClose) { }

        public TempFileStream(FileAccess access, FileShare share, int bufferSize)
            : base(Path.GetTempFileName(), FileMode.Create, access, share, bufferSize, FileOptions.DeleteOnClose) { }
    }
}
