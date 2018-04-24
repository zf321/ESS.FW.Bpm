using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     Properties that maps property keys to values. The properties cannot contain
    ///     duplicate property names; each property name can map to at most one value.
    ///     
    /// </summary>
    public class Properties
    {
        protected internal readonly IDictionary<string, object> properties;

        public Properties() : this(new Dictionary<string, object>())
        {
        }

        public Properties(IDictionary<string, object> properties)
        {
            this.properties = properties;
        }

        /// <summary>
        ///     Returns the value to which the specified property key is mapped, or
        ///     <code>null</code> if this properties contains no mapping for the property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose associated value is to be returned
        /// </param>
        /// <returns>
        ///     the value to which the specified property key is mapped, or
        ///     <code>null</code> if this properties contains no mapping for the property key
        /// </returns>
        public virtual T Get<T>(PropertyKey<T> property)
        {
            //添加找不到返回Null
            if (properties.ContainsKey(property.Name))
                return (T) properties[property.Name];
            return default(T);
        }
        
        /// <summary>
        ///     Returns the list to which the specified property key is mapped, or
        ///     an empty list if this properties contains no mapping for the property key.
        ///     Note that the empty list is not mapped to the property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose associated list is to be returned
        /// </param>
        /// <returns>
        ///     the list to which the specified property key is mapped, or
        ///     an empty list if this properties contains no mapping for the property key
        /// </returns>
        /// <seealso cref= # addListItem( PropertyListKey, Object
        /// )
        /// </seealso>
        public virtual IList<T> Get<T>(PropertyListKey<T> property)
        {
            if (Contains(property))
                return (IList<T>) properties[property.Name];
            return new List<T>();
        }

        /// <summary>
        ///     Returns the map to which the specified property key is mapped, or
        ///     an empty map if this properties contains no mapping for the property key.
        ///     Note that the empty map is not mapped to the property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose associated map is to be returned
        /// </param>
        /// <returns>
        ///     the map to which the specified property key is mapped, or
        ///     an empty map if this properties contains no mapping for the property key
        /// </returns>
        /// <seealso cref= # putMapEntry( PropertyMapKey, Object, Object
        /// )
        /// </seealso>
        public virtual IDictionary<TK, TV> Get<TK, TV>(PropertyMapKey<TK, TV> property)
        {
            if (Contains(property))
                return (IDictionary<TK, TV>) properties[property.Name];
            return new Dictionary<TK, TV>();
        }

        /// <summary>
        ///     Associates the specified value with the specified property key. If the properties previously contained a mapping
        ///     for the property key, the old
        ///     value is replaced by the specified value.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the value </param>
        ///     <param name="property">
        ///         the property key with which the specified value is to be associated
        ///     </param>
        ///     <param name="value">
        ///         the value to be associated with the specified property key
        ///     </param>
        public virtual void Set<T>(PropertyKey<T> property, T value)
        {
            properties[property.Name] = value;
        }

        /// <summary>
        ///     Associates the specified list with the specified property key. If the properties previously contained a mapping for
        ///     the property key, the old
        ///     value is replaced by the specified list.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of elements in the list </param>
        ///     <param name="property">
        ///         the property key with which the specified list is to be associated
        ///     </param>
        ///     <param name="value">
        ///         the list to be associated with the specified property key
        ///     </param>
        public virtual void Set<T>(PropertyListKey<T> property, IList<T> value)
        {
            properties[property.Name] = value;
        }

        /// <summary>
        ///     Associates the specified map with the specified property key. If the properties previously contained a mapping for
        ///     the property key, the old
        ///     value is replaced by the specified map.
        /// </summary>
        /// @param
        /// <K>
        ///     the type of keys maintained by the map </param>
        ///     @param
        ///     <V>
        ///         the type of mapped values </param>
        ///         <param name="property">
        ///             the property key with which the specified map is to be associated
        ///         </param>
        ///         <param name="value">
        ///             the map to be associated with the specified property key
        ///         </param>
        public virtual void Set<TK, TV>(PropertyMapKey<TK, TV> property, IDictionary<TK, TV> value)
        {
            properties[property.Name] = value;
        }
        /// <summary>
        ///     Append the value to the list to which the specified property key is mapped. If
        ///     this properties contains no mapping for the property key, the value append to
        ///     a new list witch is associate the the specified property key.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of elements in the list </param>
        ///     <param name="property">
        ///         the property key whose associated list is to be added
        ///     </param>
        ///     <param name="value">
        ///         the value to be appended to list
        ///     </param>
        public virtual void AddListItem<T>(PropertyListKey<T> property, T value)
        {
            var list = Get(property);
            list.Add(value);

            if (!Contains(property))
                Set(property, list);
        }

        /// <summary>
        ///     Insert the value to the map to which the specified property key is mapped. If
        ///     this properties contains no mapping for the property key, the value insert to
        ///     a new map witch is associate the the specified property key.
        /// </summary>
        /// @param
        /// <K>
        ///     the type of keys maintained by the map </param>
        ///     @param
        ///     <V>
        ///         the type of mapped values </param>
        ///         <param name="property">
        ///             the property key whose associated list is to be added
        ///         </param>
        ///         <param name="value">
        ///             the value to be appended to list
        ///         </param>
        public virtual void PutMapEntry<TK, TV>(PropertyMapKey<TK, TV> property, TK key, TV value)
        {
            var map = Get(property);

            if (!property.AllowsOverwrite && map.ContainsKey(key))
                throw new ProcessEngineException("Cannot overwrite property key " + key + ". Key already exists");

            map[key] = value;

            if (!Contains(property))
            {
                Set(property, map);
            }
        }

        /// <summary>
        ///     Returns <code>true</code> if this properties contains a mapping for the specified property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose presence is to be tested
        /// </param>
        /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
        public virtual bool Contains<TK>(PropertyKey<TK> property)
        {
            return properties.ContainsKey(property.Name);
        }

        /// <summary>
        ///     Returns <code>true</code> if this properties contains a mapping for the specified property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose presence is to be tested
        /// </param>
        /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
        public virtual bool Contains<TK>(PropertyListKey<TK> property)
        {
            return properties.ContainsKey(property.Name);
        }
        public virtual bool ContainsKey(string key)
        {
            return properties.ContainsKey(key);
        }
        /// <summary>
        ///     Returns <code>true</code> if this properties contains a mapping for the specified property key.
        /// </summary>
        /// <param name="property">
        ///     the property key whose presence is to be tested
        /// </param>
        /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
        public virtual bool Contains<TK, TV>(PropertyMapKey<TK,TV> property)
        {
            return properties.ContainsKey(property.Name);
        }
        /// <summary>
        ///     Returns a map view of this properties. Changes to the map are not reflected
        ///     to the properties.
        /// </summary>
        /// <returns> a map view of this properties </returns>
        public virtual IDictionary<string, object> ToMap()
        {
            return new Dictionary<string, object>(properties);
        }

        public override string ToString()
        {
            return "Properties [properties=" + properties + "]";
        }
    }
}