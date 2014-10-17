﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class IdGenerator
    {
        public static string LLID()
        {
            return String.Format("{0}{1}{2}{3}-0000-0000-0000-000000000000",
                GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte()).ToLower();
        }

        public static Guid LLIDGuid()
        {
            Guid o;
            System.Guid.TryParse (LLID (), out o);
            return o;
        }

        public static string Id()
        {
            return String.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte(),
                GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte(), GetHexRandomByte());
        }

        public static Guid Guid()
        {
            Guid guid;
            do
                guid = System.Guid.NewGuid();
            while (guid == System.Guid.Empty);
            return guid;
        }

        private static string GetHexRandomByte()
        {
            return RandomGenerator.NextByte().ToString("X").PadLeft(2, '0');
        }
    }
}
