using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Returns the index of the first item in an enumeration that passes the given predicate.
        /// </summary>
        public static int? IndexOf<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> predicate)
        {
            return items.Select((value, index) => new { value, index })
                .Where(pair => predicate(pair.value))
                .Select(pair => (int?)pair.index)
                .FirstOrDefault();
        }
    }
}
