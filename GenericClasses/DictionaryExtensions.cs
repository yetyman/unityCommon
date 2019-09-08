using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class DictionaryExtensions
    {
        public static void SafeSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool addIfNotFound = true)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else if(addIfNotFound) dictionary.Add(key, value);
        }
        public static TValue SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return default(TValue);
        }
        public static bool SafeAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool setIfExists = false)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            else if (setIfExists)
                dictionary[key] = value;
            return false;
        }

        public static void SafeSetAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other, bool addIfNotFound = true)
        {
            foreach (var kv in other)
                dictionary.SafeSet(kv.Key, kv.Value, addIfNotFound);
        }
        public static void SafeAddAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other, bool setIfExists = false)
        {
            foreach (var kv in other)
                dictionary.SafeAdd(kv.Key, kv.Value, setIfExists);
        }
    }
}
