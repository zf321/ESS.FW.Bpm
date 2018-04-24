using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Common.Cache
{
    /// <summary>
	/// A thread-safe LRU <seealso cref="Cache"/> with a fixed capacity. If the cache reaches
	/// the capacity, it discards the least recently used entry first.
	/// <para>
	/// *Note*: The consistency of the keys queue with the keys in the cache is not ensured! This means, the keys queue
	/// can contain duplicates of the same key and not all the keys of the queue are necessarily in the cache.
	/// However, all the keys of the cache are at least once contained in the keys queue.
	/// 
	/// </para>
	/// </summary>
	/// @param <K> the type of keys </param>
	/// @param <V> the type of mapped values </param>
	public class ConcurrentLruCache<K, V> : ICache<K, V> //where V:class
    {

        private readonly int capacity;
        
        private readonly ConcurrentDictionary<K, V> _cache = new ConcurrentDictionary<K, V>();
        private readonly ConcurrentBag<K> _keys = new ConcurrentBag<K>();

        /// <summary>
        /// Creates the cache with a fixed capacity.
        /// </summary>
        /// <param name="capacity"> max number of cache entries </param>
        ///  if capacity is negative </exception>
        public ConcurrentLruCache(int capacity)
        {
            if (capacity < 0)
            {
                throw new System.ArgumentException();
            }
            this.capacity = capacity;
        }

        public  V Get(K key)
        {
            V value;
            _cache.TryGetValue(key,out value);
            if (value != null)
            {
                _keys.TryTake(out key);
                _keys.Add(key);
            }
            return value;
        }

        public  void Put(K key, V value)
        {
            if (key == null || value == null)
            {
                throw new System.NullReferenceException();
            }

            //V previousValue = cache.put(key, value);
            bool flag= _cache.TryAdd(key, value);
            //if (previousValue != null)
            //{
            //    keys.remove(key);
            //}
            //if (flag)
            //{
            //    keys.TryTake(out key);
            //}
            //keys.Add(key);

            if (_cache.Count > capacity)
            {
                K lastK = _cache.Last().Key;
                V preV;
                _cache.TryRemove(lastK, out preV);
                //K lruKey = keys.poll();
                //if (lruKey != null)
                //{
                //    cache.remove(lruKey);

                //    // remove duplicated keys
                //    this.RemoveAll(lruKey);

                //    // queue may not contain any key of a possibly concurrently added entry of the same key in the cache
                //    if (cache.containsKey(lruKey))
                //    {
                //        keys.add(lruKey);
                //    }
                //}
            }
        }

        public  void Remove(K key)
        {
            V value;
            this._cache.TryRemove(key,out value);
            //keys.remove(key);
        }

        public  void Clear()
        {
            _cache.Clear();
            //keys.clear();
        }

        public  bool IsEmpty()
        {
            return _cache.IsEmpty;
        }

        public  ICollection<K> KeySet()
        {
            return _cache.Keys;
        }

        public  int Size()
        {
            return _cache.Count;
        }

        /// <summary>
        /// Removes all instances of the given key within the keys queue.
        /// </summary>
        protected internal virtual void RemoveAll(K key)
        {
            K v;
            while (_keys.TryTake(out v))
            {
            }
        }

    }

}
