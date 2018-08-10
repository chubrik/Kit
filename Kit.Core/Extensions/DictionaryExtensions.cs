using System.Collections.Generic;

namespace Kit
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }

        public static bool Contains<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue value) =>
            dictionary.ContainsKey(key) && dictionary[key].Equals(value);
    }
}
