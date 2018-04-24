using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{


	/// <summary>
	/// Defines property resolution behavior on instances of java.Util.Map. This resolver handles base
	/// objects of type java.Util.Map. It accepts any object as a property and uses that object as a key
	/// in the map. The resulting value is the value in the map that is associated with that key. This
	/// resolver can be constructed in read-only mode, which means that isReadOnly will always return
	/// true and <seealso cref="#setValue(ELContext, Object, Object, Object)"/> will always throw
	/// PropertyNotWritableException. ELResolvers are combined together using <seealso cref="CompositeELResolver"/>
	/// s, to define rich semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver"/>
	/// for details.
	/// </summary>
	public class MapELResolver : ELResolver
	{
		private readonly bool readOnly;

		/// <summary>
		/// Creates a new read/write MapELResolver.
		/// </summary>
		public MapELResolver() : this(false)
		{
		}

		/// <summary>
		/// Creates a new MapELResolver whose read-only status is determined by the given parameter.
		/// </summary>
		public MapELResolver(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		/// <summary>
		/// If the base object is a map, returns the most general type that this resolver accepts for the
		/// property argument. Otherwise, returns null. Assuming the base is a Map, this method will
		/// always return Object.class. This is because Maps accept any object as a key.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
		/// <returns> null if base is not a Map; otherwise Object.class. </returns>
		public override Type GetCommonPropertyType(ELContext context, object @base)
		{
			return isResolvable(@base) ? typeof(object) : null;
		}

        /// <summary>
        /// If the base object is a map, returns an Iterator containing the set of keys available in the
        /// Map. Otherwise, returns null. The Iterator returned must contain zero or more instances of
        /// java.beans.MemberDescriptor. Each info object contains information about a key in the Map,
        /// and is initialized as follows:
        /// <ul>
        /// <li>displayName - The return value of calling the toString method on this key, or "null" if
        /// the key is null</li>
        /// <li>name - Same as displayName property</li>
        /// <li>shortDescription - Empty string</li>
        /// <li>expert - false</li>
        /// <li>hidden - false</li>
        /// <li>preferred - true</li>
        /// </ul>
        /// In addition, the following named attributes must be set in the returned MemberInfos:
        /// <ul>
        /// <li><seealso cref="ELResolver#TYPE"/> - The return value of calling the getClass() method on this key,
        /// or null if the key is null.</li>
        /// <li><seealso cref="ELResolver#RESOLVABLE_AT_DESIGN_TIME"/> - true</li>
        /// </ul>
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
        /// <returns> An Iterator containing zero or more (possibly infinitely more) MemberDescriptor
        ///         objects, each representing a key in this map, or null if the base object is not a
        ///         map. </returns>
        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            if (isResolvable(@base))
            {
                IDictionary<object, object> map = (IDictionary<object, object>)@base;
                IEnumerator<object> keys = map.Keys.GetEnumerator();
                //return new IteratorAnonymousInnerClass(this, keys);
            }
            return null;
        }

        //		private class IteratorAnonymousInnerClass : IEnumerator<MemberInfo>
        //		{
        //			private readonly MapELResolver outerInstance;

        ////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        ////ORIGINAL LINE: private IEnumerator<object> keys;
        //			private IEnumerator<object> keys;

        //			public IteratorAnonymousInnerClass<T1>(MapELResolver outerInstance, IEnumerator<T1> keys)
        //			{
        //				this.outerInstance = outerInstance;
        //				this.keys = keys;
        //			}

        //			public virtual bool hasNext()
        //			{
        ////JAVA TO C# CONVERTER TODO ITask: Java iterators are only converted within the context of 'while' and 'for' loops:
        //				return keys.hasNext();
        //			}
        //			public virtual MemberDescriptor next()
        //			{
        ////JAVA TO C# CONVERTER TODO ITask: Java iterators are only converted within the context of 'while' and 'for' loops:
        //				object key = keys.next();
        //				MemberDescriptor feature = new MemberDescriptor();
        //				feature.DisplayName = key == null ? "null" : key.ToString();
        //				feature.Name = feature.DisplayName;
        //				feature.ShortDescription = "";
        //				feature.Expert = true;
        //				feature.Hidden = false;
        //				feature.Preferred = true;
        //				feature.setValue(TYPE, key == null ? null : key.GetType());
        //				feature.setValue(RESOLVABLE_AT_DESIGN_TIME, true);
        //				return feature;

        //			}
        //			public virtual void remove()
        //			{
        //				throw new System.NotSupportedException("cannot remove");
        //			}
        //		}

        /// <summary>
        /// If the base object is a map, returns the most general acceptable type for a value in this
        /// map. If the base is a Map, the propertyResolved property of the ELContext object must be set
        /// to true by this resolver, before returning. If this property is not true after this method is
        /// called, the caller should ignore the return value. Assuming the base is a Map, this method
        /// will always return Object.class. This is because Maps accept any object as the value for a
        /// given key.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
        /// <param name="property">
        ///            The key to return the acceptable type for. Ignored by this resolver. </param>
        /// <returns> If the propertyResolved property of ELContext was set to true, then the most general
        ///         acceptable type; otherwise undefined. </returns>
        /// <exception cref="NullPointerException">
        ///             if context is null </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing the property or variable resolution.
        ///             The thrown exception must be included as the cause property of this exception, if
        ///             available. </exception>
        public override Type GetType(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			Type result = null;
			if (isResolvable(@base))
			{
				result = typeof(object);
				context.PropertyResolved = true;
			}
			return result;
		}

		/// <summary>
		/// If the base object is a map, returns the value associated with the given key, as specified by
		/// the property argument. If the key was not found, null is returned. If the base is a Map, the
		/// propertyResolved property of the ELContext object must be set to true by this resolver,
		/// before returning. If this property is not true after this method is called, the caller should
		/// ignore the return value. Just as in java.Util.Map.get(Object), just because null is returned
		/// doesn't mean there is no mapping for the key; it's also possible that the Map explicitly maps
		/// the key to null.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
		/// <param name="property">
		///            The key to return the acceptable type for. Ignored by this resolver. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then the value
		///         associated with the given key or null if the key was not found. Otherwise, undefined. </returns>
		/// <exception cref="ClassCastException">
		///             if the key is of an inappropriate type for this map (optionally thrown by the
		///             underlying Map). </exception>
		/// <exception cref="NullPointerException">
		///             if context is null, or if the key is null and this map does not permit null keys
		///             (the latter is optionally thrown by the underlying Map). </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override object GetValue(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			object result = null;
			if (isResolvable(@base))
			{
                if(@base is IDictionary)
                {
                    if (((IDictionary)@base).Contains(property.ToString()))
                    {
                        result = ((IDictionary)@base).Contains(property.ToString());
                        context.PropertyResolved = true;
                    }
                }
                if (@base is IDictionary<string,object>)
                {
                    if (((IDictionary<string, object>)@base).TryGetValue(property.ToString(), out result)){
                        context.PropertyResolved = true;
                    }
                }
            }
			return result;
		}

		/// <summary>
		/// If the base object is a map, returns whether a call to
		/// <seealso cref="#setValue(ELContext, Object, Object, Object)"/> will always fail. If the base is a Map,
		/// the propertyResolved property of the ELContext object must be set to true by this resolver,
		/// before returning. If this property is not true after this method is called, the caller should
		/// ignore the return value. If this resolver was constructed in read-only mode, this method will
		/// always return true. If a Map was created using java.Util.Collections.unmodifiableMap(Map),
		/// this method must return true. Unfortunately, there is no Collections API method to detect
		/// this. However, an implementation can create a prototype unmodifiable Map and query its
		/// runtime type to see if it matches the runtime type of the base object as a workaround.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
		/// <param name="property">
		///            The key to return the acceptable type for. Ignored by this resolver. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then true if calling
		///         the setValue method will always fail or false if it is possible that such a call may
		///         succeed; otherwise undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override bool IsReadOnly(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			if (isResolvable(@base))
			{
				context.PropertyResolved = true;
			}
			return readOnly;
		}

		/// <summary>
		/// If the base object is a map, attempts to set the value associated with the given key, as
		/// specified by the property argument. If the base is a Map, the propertyResolved property of
		/// the ELContext object must be set to true by this resolver, before returning. If this property
		/// is not true after this method is called, the caller can safely assume no value was set. If
		/// this resolver was constructed in read-only mode, this method will always throw
		/// PropertyNotWritableException. If a Map was created using
		/// java.Util.Collections.unmodifiableMap(Map), this method must throw
		/// PropertyNotWritableException. Unfortunately, there is no Collections API method to detect
		/// this. However, an implementation can create a prototype unmodifiable Map and query its
		/// runtime type to see if it matches the runtime type of the base object as a workaround.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The map to analyze. Only bases of type Map are handled by this resolver. </param>
		/// <param name="property">
		///            The key to return the acceptable type for. Ignored by this resolver. </param>
		/// <param name="value">
		///            The value to be associated with the specified key. </param>
		/// <exception cref="ClassCastException">
		///             if the class of the specified key or value prevents it from being stored in this
		///             map. </exception>
		/// <exception cref="NullPointerException">
		///             if context is null, or if this map does not permit null keys or values, and the
		///             specified key or value is null. </exception>
		/// <exception cref="IllegalArgumentException">
		///             if some aspect of this key or value prevents it from being stored in this map. </exception>
		/// <exception cref="PropertyNotWritableException">
		///             if this resolver was constructed in read-only mode, or if the put operation is
		///             not supported by the underlying map. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override void SetValue(ELContext context, object @base, object property, object value)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			if (isResolvable(@base))
			{
				if (readOnly)
				{
					throw new PropertyNotWritableException("resolver is read-only");
				}
				((IDictionary) @base)[property] = value;
				context.PropertyResolved = true;
			}
		}

		/// <summary>
		/// Test whether the given base should be resolved by this ELResolver.
		/// </summary>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> base instanceof Map </returns>
		private bool isResolvable(object @base)
		{
			return @base is IDictionary || @base is IDictionary<string,object>;
		}
	}

}

