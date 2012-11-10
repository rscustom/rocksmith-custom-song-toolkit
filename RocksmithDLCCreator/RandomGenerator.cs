using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator
{
    class RandomGenerator
    {
        private static Random Instance = new Random();
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
