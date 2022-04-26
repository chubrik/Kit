using System;
using System.Collections.Generic;

namespace Chubrik.Kit
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
                target.Add(element);
        }
    }
}
