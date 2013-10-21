using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this String obj, char[] chars)
        {
            return (obj.IndexOfAny(chars) >= 0);
        }
    }
}
