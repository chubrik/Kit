using System.Collections.Generic;

namespace Kit
{
    public static class DictionaryExtensions
    {
        public static bool Contains<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue value) =>
            dictionary.TryGetValue(key, out var i) && i!.Equals(value);
    }
}
