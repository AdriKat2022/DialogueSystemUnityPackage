using System.Collections.Generic;

namespace AdriKat.DialogueSystem.Utility
{
    public static class CollectionUtility
    {
        public static void AddItem<K, V>(this SerializableDictionary<K, List<V>> serializableDictionnary, K key, V value)
        {
            if (serializableDictionnary.ContainsKey(key))
            {
                serializableDictionnary[key].Add(value);
                return;
            }

            serializableDictionnary.Add(key, new List<V> { value });
        }
    }
}