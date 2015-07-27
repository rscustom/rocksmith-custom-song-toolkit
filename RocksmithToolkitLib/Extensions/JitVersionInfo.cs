using System;

namespace RocksmithToolkitLib.Extensions
{
    public enum JitVersion
    {
        Mono,
        MsX86,
        MsX64,
        RyuJit
    }

    public class JitVersionInfo
    {
        public static JitVersion GetJitVersion()
        {
            if (IsMono())
                return JitVersion.Mono;
            if (IsMsX86())
                return JitVersion.MsX86;
            //if (IsMsX64())
            //    return JitVersion.MsX64;
            return JitVersion.RyuJit;
        }

        private int bar;
        private bool IsMsX64(int step = 1)
        {
            var value = 0;
            for (int i = 0; i < step; i++)
            {
                bar = i + 10;
                for (int j = 0; j < 2*step; j += step)
                    value = j + 10;
            }
            return value == 20 + step;
        }

        public static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static bool IsMsX86()
        {
            return !IsMono() && IntPtr.Size == 4;
        }
    }
}