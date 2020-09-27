using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            Debug.Assert(source != null);

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
                target.Add(element);
        }
    }
}
