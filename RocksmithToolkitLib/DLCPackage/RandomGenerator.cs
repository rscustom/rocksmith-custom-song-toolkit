using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class RandomGenerator
    {
        private static readonly Random Instance = new Random();
        public static int NextInt()
        {
            return Instance.Next();
        }
        public static byte NextByte()
        {
            return (byte)Instance.Next(256);
        }
    }
}
