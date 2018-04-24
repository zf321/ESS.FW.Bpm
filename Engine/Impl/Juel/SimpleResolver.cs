using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    /// <summary>
	/// Simple resolver implementation. This resolver handles root properties (top-level identifiers).
	/// Resolving "real" properties (<code>base != null</code>) is delegated to a resolver specified at
	/// construction time.
	/// 
	/// 
	/// </summary>
	public class SimpleResolver : ELResolver
	{
		private static readonly ELResolver DEFAULT_RESOLVER_READ_ONLY = new CompositeELResolverAnonymousInnerClass();

		private class CompositeELResolverAnonymousInnerClass : CompositeELResolver
		{
			public CompositeELResolverAnonymousInnerClass()
			{
                Add(new ArrayELResolver(true));
                Add(new ListELResolver(true));
                Add(new MapELResolver(true));
                Add(new ResourceBundleElResolver());
                Add(new ObjectELResolver(true));
            }

            //		{
            //			add(new ArrayELResolver(true));
            //			add(new ListELResolver(true));
            //			add(new MapELResolver(true));
            //			add(new ResourceBundleELResolver());
            //			add(new BeanELResolver(true));
            //		}
        }
		private static readonly ELResolver DEFAULT_RESOLVER_READ_WRITE = new CompositeELResolverAnonymousInnerClass2();

		private class CompositeELResolverAnonymousInnerClass2 : CompositeELResolver
		{
			public CompositeELResolverAnonymousInnerClass2()
			{
                Add(new ArrayELResolver(false));

                Add(new ListELResolver(false));

                Add(new MapELResolver(false));

                Add(new ResourceBundleElResolver());

                Add(new ObjectELResolver(false));
			}
    }

		private readonly RootPropertyResolver root;
		private readonly CompositeELResolver @delegate;

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers. Everything else is passed to
		/// the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver, bool readOnly)
		{
			@delegate = new CompositeELResolver();
			@delegate.Add(root = new RootPropertyResolver(readOnly));
			@delegate.Add(resolver);
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers. Everything else is
		/// passed to the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver) : this(resolver, false)
		{
		}

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers, array values, list values, map
		/// values, resource values and bean properties.
		/// </summary>
		public SimpleResolver(bool readOnly) : this(readOnly ? DEFAULT_RESOLVER_READ_ONLY : DEFAULT_RESOLVER_READ_WRITE, readOnly)
		{
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers, array values, list
		/// values, map values, resource values and bean properties.
		/// </summary>
		public SimpleResolver() : this(DEFAULT_RESOLVER_READ_WRITE, false)
		{
		}

		/// <summary>
		/// Answer our root resolver which provides an API to access top-level properties.
		/// </summary>
		/// <returns> root property resolver </returns>
		public virtual RootPropertyResolver RootPropertyResolver
		{
			get
			{
				return root;
			}
		}

		public override Type GetCommonPropertyType(ELContext context, object @base)
		{
			return @delegate.GetCommonPropertyType(context, @base);
		}

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            return @delegate.GetMemberInfos(context, @base);
        }

        public override Type GetType(ELContext context, object @base, object property)
		{
			return @delegate.GetType(context, @base, property);
		}

		public override object GetValue(ELContext context, object @base, object property)
		{
			return @delegate.GetValue(context, @base, property);
		}

		public override bool IsReadOnly(ELContext context, object @base, object property)
		{
			return @delegate.IsReadOnly(context, @base, property);
		}

		public override void SetValue(ELContext context, object @base, object property, object value)
		{
			@delegate.SetValue(context, @base, property, value);
		}

		public override object Invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			return @delegate.Invoke(context, @base, method, paramTypes, @params);
		}
	}

}