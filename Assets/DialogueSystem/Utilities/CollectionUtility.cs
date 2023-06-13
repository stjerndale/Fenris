using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Utilities
{

    public static class CollectionUtility
    {
        public static void AddItem<K, V>(this SerializableDictionary<K, List<V>> serializableDictionay, K key, V value)
        {
            if (serializableDictionay.ContainsKey(key))
            {
                serializableDictionay[key].Add(value);
            }
            else
            {
                serializableDictionay.Add(key, new List<V>() { value});
            }
        }
    }

}
