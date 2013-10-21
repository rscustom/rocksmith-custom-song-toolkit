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

    public static class ListExtension
    {
        /// <summary>
        /// Returns the index of the first item in a collection that passes the given predicate.
        /// The predicate takes in both the current and next item in the collection, or null for the last item.
        /// </summary>
        public static int? IndexOf<TItem>(this IList<TItem> items, Func<TItem, TItem, bool> predicate) where TItem : class
        {
            for (int i = 0, len = items.Count; i < len; i++)
            {
                var item = items[i];
                var next = i + 1 < len ? items[i + 1] : null;
                if (predicate(item, next))
                    return i;
            }
            return null;
        }
    }
}
