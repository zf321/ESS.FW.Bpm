using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

    

	/// <summary>
	/// Simple root property resolver implementation. This resolver handles root properties (i.e.
	/// <code>base == null &amp;&amp; property instanceof String</code>), which are stored in a map. The
	/// properties can be accessed via the <seealso cref="#getProperty(String)"/>,
	/// <seealso cref="#setProperty(String, Object)"/>, <seealso cref="#isProperty(String)"/> and <seealso cref="#properties()"/>
	/// methods.
	/// 
	/// 
	/// </summary>
	public class RootPropertyResolver : ELResolver
	{
		private readonly ConcurrentDictionary<string, object> map = new ConcurrentDictionary<string, object>();
		private readonly bool readOnly;

		/// <summary>
		/// Create a read/write root property resolver
		/// </summary>
		public RootPropertyResolver() : this(false)
		{
		}

		/// <summary>
		/// Create a root property resolver
		/// </summary>
		/// <param name="readOnly"> </param>
		public RootPropertyResolver(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		private bool IsResolvable(object @base)
		{
			return @base == null;
		}

		private bool Resolve(ELContext context, object @base, object property)
		{
			context.PropertyResolved = IsResolvable(@base) && property is string;
			return context.PropertyResolved;
		}

		public override Type GetCommonPropertyType(ELContext context, object @base)
		{
			return IsResolvable(context) ? typeof(string) : null;
		}

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            return null;
        }

        public override Type GetType(ELContext context, object @base, object property)
		{
			return Resolve(context, @base, property) ? typeof(object) : null;
		}

		public override object GetValue(ELContext context, object @base, object property)
		{
			if (Resolve(context, @base, property))
			{
				if (!IsProperty((string) property))
				{
					throw new PropertyNotFoundException("Cannot find property " + property);
				}
				return GetProperty((string) property);
			}
			return null;
		}

		public override bool IsReadOnly(ELContext context, object @base, object property)
		{
			return Resolve(context, @base, property) ? readOnly : false;
		}
        
		public override void SetValue(ELContext context, object @base, object property, object value)
		{
			if (Resolve(context, @base, property))
			{
				if (readOnly)
				{
					throw new PropertyNotWritableException("Resolver is read only!");
				}
				SetProperty((string) property, value);
			}
		}

		public override object Invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			if (Resolve(context, @base, method))
			{
				throw new System.NullReferenceException("Cannot invoke method " + method + " on null");
			}
			return null;
		}

		/// <summary>
		/// Get property value
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <returns> value associated with the given property </returns>
		public virtual object GetProperty(string property)
		{
			return map[property];
		}

		/// <summary>
		/// Set property value
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <param name="value">
		///            property value </param>
		public virtual void SetProperty(string property, object value)
		{
			map[property] = value;
		}

		/// <summary>
		/// Test property
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <returns> <code>true</code> if the given property is associated with a value </returns>
		public virtual bool IsProperty(string property)
		{
			return map.ContainsKey(property);
		}

		/// <summary>
		/// Get properties
		/// </summary>
		/// <returns> all property names (in no particular order) </returns>
		public virtual IEnumerable<string> Properties()
		{
			return map.Keys;
		}
	}

}