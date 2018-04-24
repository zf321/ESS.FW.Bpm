using ESS.FW.Bpm.Engine.Impl.Juel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    /// <summary>
	/// A simple implementation of Bindings backed by
	/// a <code>HashMap</code> or some other specified <code>Map</code>.
	/// 
	/// @author Mike Grogan
	/// @since 1.6
	/// </summary>
	public class SimpleBindings : IBindings
    {

        /// <summary>
        /// The <code>Map</code> field stores the attributes.
        /// </summary>
        private IDictionary<string, object> map;
        /// <summary>
        /// Default constructor uses a <code>HashMap</code>.
        /// </summary>
        public SimpleBindings() : this(new Dictionary<string, object>())
        {
        }
        /// <summary>
        /// Constructor uses an existing <code>Map</code> to store the values. </summary>
        /// <param name="m"> The <code>Map</code> backing this <code>SimpleBindings</code>. </param>
        /// <exception cref="NullPointerException"> if m is null </exception>
        public SimpleBindings(IDictionary<string, object> m)
        {
            if (m == null)
            {
                throw new System.NullReferenceException();
            }
            this.map = m;
        }

        /// <summary>
        /// Sets the specified key/value in the underlying <code>map</code> field.
        /// </summary>
        /// <param name="name"> Name of value </param>
        /// <param name="value"> Value to set.
        /// </param>
        /// <returns> Previous value for the specified key.  Returns null if key was previously
        /// unset.
        /// </returns>
        /// <exception cref="NullPointerException"> if the name is null. </exception>
        /// <exception cref="IllegalArgumentException"> if the name is empty. </exception>
        public virtual object Put(string name, object value)
        {
            CheckKey(name);
            return map[name] = value;
        }
        public IDictionary<string,object> GetAll()
        {
            return map;
        }

        /// <summary>
        /// <code>putAll</code> is implemented using <code>Map.putAll</code>.
        /// </summary>
        /// <param name="toMerge"> The <code>Map</code> of values to add.
        /// </param>
        /// <exception cref="NullPointerException">
        ///         if toMerge map is null or if some key in the map is null. </exception>
        /// <exception cref="IllegalArgumentException">
        ///         if some key in the map is an empty String. </exception>
        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        //ORIGINAL LINE: public void putAll(java.util.Map<? extends String, ? extends Object> toMerge)
        public virtual void PutAll(IDictionary<string,object> toMerge)// where T1 : String where ? : Object
        {
            if (toMerge == null)
            {
                throw new System.NullReferenceException("toMerge map is null");
            }
            foreach (KeyValuePair <string, object> entry in toMerge)
			{
                string key = entry.Key;
                CheckKey(key);
                Put(key, entry.Value);
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual void Clear()
        {
            map.Clear();
        }

        /// <summary>
        /// Returns <tt>true</tt> if this map contains a mapping for the specified
        /// key.  More formally, returns <tt>true</tt> if and only if
        /// this map contains a mapping for a key <tt>k</tt> such that
        /// <tt>(key==null ? k==null : key.equals(k))</tt>.  (There can be
        /// at most one such mapping.)
        /// </summary>
        /// <param name="key"> key whose presence in this map is to be tested. </param>
        /// <returns> <tt>true</tt> if this map contains a mapping for the specified
        ///         key.
        /// </returns>
        /// <exception cref="NullPointerException"> if key is null </exception>
        /// <exception cref="ClassCastException"> if key is not String </exception>
        /// <exception cref="IllegalArgumentException"> if key is empty String </exception>
        public virtual bool ContainsKey(string key)
        {
            CheckKey(key);
            return map.ContainsKey(key);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual bool ContainsValue(object value)
        {
            return map.Values.Contains(value);//.ContainsValue(value);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual IEnumerable<KeyValuePair<string, object>> EntrySet()
        {
            //JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
            return map.DefaultIfEmpty();//.EntrySet();
        }

        /// <summary>
        /// Returns the value to which this map maps the specified key.  Returns
        /// <tt>null</tt> if the map contains no mapping for this key.  A return
        /// value of <tt>null</tt> does not <i>necessarily</i> indicate that the
        /// map contains no mapping for the key; it's also possible that the map
        /// explicitly maps the key to <tt>null</tt>.  The <tt>containsKey</tt>
        /// operation may be used to distinguish these two cases.
        /// 
        /// <para>More formally, if this map contains a mapping from a key
        /// <tt>k</tt> to a value <tt>v</tt> such that <tt>(key==null ? k==null :
        /// key.equals(k))</tt>, then this method returns <tt>v</tt>; otherwise
        /// it returns <tt>null</tt>.  (There can be at most one such mapping.)
        /// 
        /// </para>
        /// </summary>
        /// <param name="key"> key whose associated value is to be returned. </param>
        /// <returns> the value to which this map maps the specified key, or
        ///         <tt>null</tt> if the map contains no mapping for this key.
        /// </returns>
        /// <exception cref="NullPointerException"> if key is null </exception>
        /// <exception cref="ClassCastException"> if key is not String </exception>
        /// <exception cref="IllegalArgumentException"> if key is empty String </exception>
        public virtual object Get(string key)
        {
            CheckKey(key);
            return map[key];
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual bool IsEmpty()
        {
            return map.Count == 0;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual ICollection<string> keySet()
        {
            return map.Keys;
        }

        /// <summary>
        /// Removes the mapping for this key from this map if it is present
        /// (optional operation).   More formally, if this map contains a mapping
        /// from key <tt>k</tt> to value <tt>v</tt> such that
        /// <code>(key==null ?  k==null : key.equals(k))</code>, that mapping
        /// is removed.  (The map can contain at most one such mapping.)
        /// 
        /// <para>Returns the value to which the map previously associated the key, or
        /// <tt>null</tt> if the map contained no mapping for this key.  (A
        /// <tt>null</tt> return can also indicate that the map previously
        /// associated <tt>null</tt> with the specified key if the implementation
        /// supports <tt>null</tt> values.)  The map will not contain a mapping for
        /// the specified  key once the call returns.
        /// 
        /// </para>
        /// </summary>
        /// <param name="key"> key whose mapping is to be removed from the map. </param>
        /// <returns> previous value associated with specified key, or <tt>null</tt>
        ///         if there was no mapping for key.
        /// </returns>
        /// <exception cref="NullPointerException"> if key is null </exception>
        /// <exception cref="ClassCastException"> if key is not String </exception>
        /// <exception cref="IllegalArgumentException"> if key is empty String </exception>
        public virtual object Remove(string key)
        {
            CheckKey(key);
            return map.Remove(key);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int size()
        {
            return map.Count;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual ICollection<object> Values()
        {
            return map.Values;
        }

        private void CheckKey(object key)
        {
            if (key == null)
            {
                throw new System.NullReferenceException("key can not be null");
            }
            if (!(key is string))
            {
                throw new System.InvalidCastException("key should be a String");
            }
            if (key.Equals(""))
            {
                throw new System.ArgumentException("key can not be empty");
            }
        }

    }

}
