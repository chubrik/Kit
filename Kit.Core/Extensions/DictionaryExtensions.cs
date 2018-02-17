using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit {
    public static class DictionaryExtensions {

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            Debug.Assert(dictionary.ContainsKey(key));

            if (!dictionary.ContainsKey(key))
                throw new InvalidOperationException();

            return dictionary.GetValueOrDefault(key);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }
    }
}
