using System;
using System.Collections.Generic;

namespace DulceSueño.Collections
{
    // Nodo interno para manejar colisiones mediante encadenamiento separado (chaining).
    internal class HashMapNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public HashMapNode<TKey, TValue> Next;

        public HashMapNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    // Diccionario propio implementado con tabla hash + encadenamiento separado. No usa System.Collections.Generic.Dictionary en ningún punto de su lógica interna: esta es tu estructura de datos NO LINEAL implementada desde cero.
    public class HashMap<TKey, TValue>
    {
        private HashMapNode<TKey, TValue>[] buckets;
        private int count;
        private const int DefaultCapacity = 16;
        private const float MaxLoadFactor = 0.75f;

        public HashMap(int capacity = DefaultCapacity)
        {
            buckets = new HashMapNode<TKey, TValue>[capacity];
            count = 0;
        }
        public int Count => count;

        // Convierte la key en un índice de bucket válido dentro del arreglo.
        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            return Math.Abs(hash) % buckets.Length;
        }
        //-> O(1) promedio, O(n) en el peor caso (todas las keys colisionan en el mismo bucket)
        public void Add(TKey key, TValue value)
        {
            if ((float)(count + 1) / buckets.Length > MaxLoadFactor)
                Resize();

            int index = GetBucketIndex(key);
            HashMapNode<TKey, TValue> current = buckets[index];

            // Si la key ya existe, actualizamos el valor en vez de duplicarla
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            HashMapNode<TKey, TValue> newNode = new HashMapNode<TKey, TValue>(key, value);
            newNode.Next = buckets[index];
            buckets[index] = newNode;
            count++;
        }
        //-> O(1) promedio, O(n) en el peor caso
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = GetBucketIndex(key);
            HashMapNode<TKey, TValue> current = buckets[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    value = current.Value;
                    return true;
                }
                current = current.Next;
            }

            value = default;
            return false;
        }
        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        //-> O(1) promedio, O(n) en el peor caso
        public bool Remove(TKey key)
        {
            int index = GetBucketIndex(key);
            HashMapNode<TKey, TValue> current = buckets[index];
            HashMapNode<TKey, TValue> previous = null;

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    if (previous == null)
                        buckets[index] = current.Next;
                    else
                        previous.Next = current.Next;

                    count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }
            return false;
        }
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;
                throw new KeyNotFoundException($"La clave '{key}' no existe en el HashMap.");
            }
            set => Add(key, value);
        }
        //-> O(n): se recorren todos los buckets y nodos existentes para reubicarlos. Ocurre muy pocas veces (solo cuando se supera el 75% de carga), no en cada Add().
        private void Resize()
        {
            HashMapNode<TKey, TValue>[] oldBuckets = buckets;
            buckets = new HashMapNode<TKey, TValue>[oldBuckets.Length * 2];
            count = 0;

            foreach (HashMapNode<TKey, TValue> node in oldBuckets)
            {
                HashMapNode<TKey, TValue> current = node;
                while (current != null)
                {
                    Add(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }
        public List<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (HashMapNode<TKey, TValue> node in buckets)
            {
                HashMapNode<TKey, TValue> current = node;
                while (current != null)
                {
                    keys.Add(current.Key);
                    current = current.Next;
                }
            }
            return keys;
        }
    }
}
