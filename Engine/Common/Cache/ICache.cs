
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Common.Cache
{

    /// <summary>
    /// A Map-like data structure that stores key-value pairs and provides temporary
    /// access to it.
    /// </summary>
    /// @param <K> the type of keys </param>
    /// @param <V> the type of mapped values </param>
    public interface ICache<K, V>
    {

        /// <summary>
        /// Gets an entry from the cache.
        /// </summary>
        /// <param name="key"> the key whose associated value is to be returned </param>
        /// <returns> the element, or <code>null</code>, if it does not exist. </returns>
        V Get(K key);

        /// <summary>
        /// Associates the specified value with the specified key in the cache.
        /// </summary>
        /// <param name="key">   key with which the specified value is to be associated </param>
        /// <param name="value"> value to be associated with the specified key </param>
        /// <exception cref="NullPointerException"> if key is <code>null</code> or if value is <code>null</code> </exception>
        void Put(K key, V value);

        /// <summary>
        /// Clears the contents of the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes an entry from the cache.
        /// </summary>
        /// <param name="key"> key with which the specified value is to be associated. </param>
        void Remove(K key);

        /// <summary>
        /// Returns a Set view of the keys contained in this cache.
        /// </summary>
        ICollection<K> KeySet();

        /// <returns> the current size of the cache </returns>
        int Size();

        /// <summary>
        /// Returns <code>true</code> if this cache contains no key-value mappings.
        /// </summary>
        bool IsEmpty();

    }
}