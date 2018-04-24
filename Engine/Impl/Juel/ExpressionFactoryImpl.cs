using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Expression factory implementation.
	/// 
	/// This class is also used as an EL "service provider". The <em>JUEL</em> jar file specifies this
	/// class as el expression factory implementation in
	/// <code>META-INF/services/javax.el.ExpressionFactory</code>. Calling
	/// <seealso cref="ExpressionFactory#newInstance()"/> will then return an instance of this class, configured as
	/// described below.
	/// 
	/// If no properties are specified at construction time, properties are read from
	/// <ol>
	/// <li>
	/// If the file <code>JAVA_HOME/lib/el.properties</code> exists and if it contains property
	/// <code>javax.el.ExpressionFactory</code> whose value is the name of this class, these properties
	/// are taken as default properties.</li>
	/// <li>Otherwise, if system property <code>javax.el.ExpressionFactory</code> is set to the name of
	/// this class, the system properties <seealso cref="System#getProperties()"/> are taken as default properties.
	/// </li>
	/// <li>
	/// <code>el.properties</code> on your classpath. These properties override the properties from
	/// <code>JAVA_HOME/lib/el.properties</code> or <seealso cref="System#getProperties()"/>.</li>
	/// </ol>
	/// There are also constructors to explicitly pass in an instance of <seealso cref="Properties"/>.
	/// 
	/// Having this, the following properties are read:
	/// <ul>
	/// <li>
	/// <code>javax.el.cacheSize</code> - cache size (int, default is 1000)</li>
	/// <li>
	/// <code>javax.el.methodInvocations</code> - allow method invocations as in
	/// <code>${foo.bar(baz)}</code> (boolean, default is <code>false</code>).</li>
	/// <li>
	/// <code>javax.el.nullProperties</code> - resolve <code>null</code> properties as in
	/// <code>${foo[null]}</code> (boolean, default is <code>false</code>).</li>
	/// <li>
	/// <code>javax.el.varArgs</code> - support function/method calls using varargs (boolean, default is
	/// <code>false</code>).</li>
	/// </ul>
	/// 
	/// 
	/// </summary>
	public class ExpressionFactoryImpl : ExpressionFactory
	{
		/// <summary>
		/// A profile provides a default set of language features that will define the builder's
		/// behavior. A profile can be adjusted using the <code>javax.el.methodInvocations</code>,
		/// <code>javax.el.varArgs</code> and <code>javax.el.nullProperties</code> properties.
		/// 
		/// 
		/// </summary>
		public sealed class Profile
		{
			/// <summary>
			/// JEE5: none
			/// </summary>
			public static readonly Profile JEE5;// = new Profile("JEE5", InnerEnum.JEE5, EnumSet.NoneOf(typeof(Builder.Feature)));
            /// <summary>
            /// JEE6: <code>javax.el.methodInvocations</code>, <code>javax.el.varArgs</code>. This is the
            /// default profile.
            /// </summary>
            public static readonly Profile JEE6 ;//= new Profile("JEE6", InnerEnum.JEE6, EnumSet.Of(Builder.Feature.METHOD_INVOCATIONS, Builder.Feature.VARARGS));

            private static readonly IList<Profile> valueList = new List<Profile>();

			static Profile()
			{
				valueList.Add(JEE5);
				valueList.Add(JEE6);
			}

			public enum InnerEnum
			{
				JEE5,
				JEE6
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			//internal readonly EnumSet<Builder.Feature> features;

			//internal Profile(string name, InnerEnum innerEnum, EnumSet<Builder.Feature> features)
			//{
			//	this.features = features;

			//	nameValue = name;
			//	ordinalValue = nextOrdinal++;
			//	innerEnumValue = innerEnum;
			//}

			//internal Builder.Feature[] Features()
			//{
			//	return features.ToArray(new Builder.Feature[features.Size()]);
			//}

			//internal bool Contains(Builder.Feature feature)
			//{
			//	return features.Contains(feature);
			//}

			public static IList<Profile> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Profile valueOf(string name)
			{
				//foreach (Profile enumInstance in Profile.Values())
				//{
				//	if (enumInstance.nameValue == name)
				//	{
				//		return enumInstance;
				//	}
				//}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// <code>javax.el.methodInvocations</code>
		/// </summary>
		public const string PROP_METHOD_INVOCATIONS = "javax.el.methodInvocations";

		/// <summary>
		/// <code>javax.el.varArgs</code>
		/// </summary>
		public const string PROP_VAR_ARGS = "javax.el.varArgs";

		/// <summary>
		/// <code>javax.el.nullProperties</code>
		/// </summary>
		public const string PROP_NULL_PROPERTIES = "javax.el.nullProperties";

		/// <summary>
		/// <code>javax.el.cacheSize</code>
		/// </summary>
		public const string PROP_CACHE_SIZE = "javax.el.cacheSize";

		private readonly TreeStore store;
		private readonly ITypeConverter converter;

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured from <code>el.properties</code> (see above). The maximum
		/// cache size will be 1000 unless overridden in <code>el.properties</code>. The builder profile
		/// is <seealso cref="Profile#JEE6"/> (features may be overridden in <code>el.properties</code>).
		/// </summary>
		public ExpressionFactoryImpl() : this(Profile.JEE6)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured from the specified profile and <code>el.properties</code>
		/// (see above). The maximum cache size will be 1000 unless overridden in
		/// <code>el.properties</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (features may be overridden in <code>el.properties</code>)
		///  </param>
		public ExpressionFactoryImpl(Profile profile)
		{
            //Properties properties = LoadProperties("el.properties");
            this.store = CreateTreeStore(1000, profile);
            this.converter = CreateTypeConverter();
        }

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified properties. The maximum cache size will
		/// be 1000 unless overridden by property <code>javax.el.cacheSize</code>. The builder profile is
		/// <seealso cref="Profile#JEE6"/> (features may be overridden in <code>properties</code>).
		/// </summary>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		public ExpressionFactoryImpl(Core.Model.Properties properties) : this(Profile.JEE6, properties)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified profile and properties. The maximum
		/// cache size will be 1000 unless overridden by property <code>javax.el.cacheSize</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (individual features may be overridden in properties) </param>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>)
		/// 
		///  </param>
		public ExpressionFactoryImpl(Profile profile, Core.Model.Properties properties)
		{
			this.store = CreateTreeStore(1000, profile);
			this.converter = CreateTypeConverter();
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified properties. The maximum cache size will
		/// be 1000 unless overridden by property <code>javax.el.cacheSize</code>. The builder profile is
		/// <seealso cref="Profile#JEE6"/> (individual features may be overridden in <code>properties</code>).
		/// </summary>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		/// <param name="converter">
		///            custom type converter </param>
		public ExpressionFactoryImpl(Core.Model.Properties properties, ITypeConverter converter) : this(Profile.JEE6, properties, converter)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified profile and properties. The maximum
		/// cache size will be 1000 unless overridden by property <code>javax.el.cacheSize</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (individual features may be overridden in properties) </param>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		/// <param name="converter">
		///            custom type converter
		/// 
		///  </param>
		public ExpressionFactoryImpl(Profile profile, Core.Model.Properties properties, ITypeConverter converter)
		{
			this.store = CreateTreeStore(1000, profile);
			this.converter = converter;
		}

		/// <summary>
		/// Create a new expression factory.
		/// </summary>
		/// <param name="store">
		///            the tree store used to parse and cache parse trees. </param>
		public ExpressionFactoryImpl(TreeStore store) : this(store, TypeConverterFields.Default)
		{
		}

		/// <summary>
		/// Create a new expression factory.
		/// </summary>
		/// <param name="store">
		///            the tree store used to parse and cache parse trees. </param>
		/// <param name="converter">
		///            custom type converter </param>
		public ExpressionFactoryImpl(TreeStore store, ITypeConverter converter)
		{
			this.store = store;
			this.converter = converter;
		}

//		private Properties LoadDefaultProperties()
//		{
//			string home = System.GetProperty("java.home");
//			string path = home + File.separator + "lib" + File.separator + "el.properties";
//			File file = new File(path);
//			if (file.Exists())
//			{
//				Properties properties = new Properties();
//				System.IO.Stream input = null;
//				try
//				{
//					properties.Load(input = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read));
//				}
//				catch (IOException e)
//				{
//					throw new ELException("Cannot read default EL properties", e);
//				}
//				finally
//				{
//					try
//					{
//						input.Close();
//					}
//					catch (IOException)
//					{
//						// ignore...
//					}
//				}
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//				if (this.GetType().FullName.Equals(properties.GetProperty("javax.el.ExpressionFactory")))
//				{
//					return properties;
//				}
//			}
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//			if (this.GetType().FullName.Equals(System.GetProperty("javax.el.ExpressionFactory")))
//			{
//				return System.Properties;
//			}
//			return null;
//		}

//		private Properties LoadProperties(string path)
//		{
//			Properties properties = new Properties(LoadDefaultProperties());

//			// try to find and load properties
//			System.IO.Stream input = null;
//			try
//			{
//				input = Thread.CurrentThread.ContextClassLoader.GetResourceAsStream(path);
//			}
//			catch (SecurityException)
//			{
//				input = ClassLoader.GetSystemResourceAsStream(path);
//			}
//			if (input != null)
//			{
//				try
//				{
//					properties.Load(input);
//				}
//				catch (IOException e)
//				{
//					throw new ELException("Cannot read EL properties", e);
//				}
//				finally
//				{
//					try
//					{
//						input.Close();
//					}
//					catch (IOException)
//					{
//						// ignore...
//					}
//				}
//			}

//			return properties;
//		}

//		private bool GetFeatureProperty(Profile profile, Properties properties, Builder.Feature feature, string property)
//		{
//			return Convert.ToBoolean(properties.GetProperty(property, profile.Contains(feature).ToString()));
//		}

		/// <summary>
		/// Create the factory's tree store. This implementation creates a new tree store using the
		/// default builder and cache implementations. The builder and cache are configured using the
		/// specified properties. The maximum cache size will be as specified unless overridden by
		/// property <code>javax.el.cacheSize</code>.
		/// </summary>
		protected internal virtual TreeStore CreateTreeStore(int defaultCacheSize, Profile profile)
		{
			// create builder
			TreeBuilder builder = null;
            //if (properties == null)
            //{
                builder = CreateTreeBuilder(null);
            //}
            //else
            //{
            //    EnumSet<Builder.Feature> features = EnumSet.NoneOf(typeof(Builder.Feature));
            //    if (GetFeatureProperty(profile, properties, Builder.Feature.METHOD_INVOCATIONS, PROP_METHOD_INVOCATIONS))
            //    {
            //        features.Add(Builder.Feature.METHOD_INVOCATIONS);
            //    }
            //    if (GetFeatureProperty(profile, properties, Builder.Feature.VARARGS, PROP_VAR_ARGS))
            //    {
            //        features.Add(Builder.Feature.VARARGS);
            //    }
            //    if (GetFeatureProperty(profile, properties, Builder.Feature.NULL_PROPERTIES, PROP_NULL_PROPERTIES))
            //    {
            //        features.Add(Builder.Feature.NULL_PROPERTIES);
            //    }
            //    builder = CreateTreeBuilder(properties, features.ToArray(new Builder.Feature[0]));
            //}

            // create cache
            int cacheSize = defaultCacheSize;
			//if (properties != null && properties.ContainsKey(PROP_CACHE_SIZE))
			//{
				try
				{
				    cacheSize = 10000;//int.Parse(properties.GetProperty(PROP_CACHE_SIZE));
				}
				catch (System.FormatException e)
				{
					throw new ELException("Cannot parse EL property " + PROP_CACHE_SIZE, e);
				}
			//}
			Cache cache = cacheSize > 0 ? new Cache(cacheSize) : null;

			return new TreeStore(builder, cache);
		}

		/// <summary>
		/// Create the factory's type converter. This implementation takes the
		/// <code>de.odysseus.el.misc.TypeConverter</code> property as the name of a class implementing
		/// the <code>de.odysseus.el.misc.TypeConverter</code> interface. If the property is not set, the
		/// default converter (<code>TypeConverter.DEFAULT</code>) is used.
		/// </summary>
		protected internal virtual ITypeConverter CreateTypeConverter()
		{
			Type clazz = Load(typeof(ITypeConverter));
			//if (clazz == null)
			//{
				return TypeConverterFields.Default;
			//}
			//try
			//{
			//	return typeof(ITypeConverter).Cast(clazz.NewInstance());
			//}
			//catch (Exception e)
			//{
			//	throw new ELException("TypeConverter " + clazz + " could not be instantiated", e);
			//}
		}

		/// <summary>
		/// Create the factory's builder. This implementation takes the
		/// <code>de.odysseus.el.tree.TreeBuilder</code> property as a name of a class implementing the
		/// <code>de.odysseus.el.tree.TreeBuilder</code> interface. If the property is not set, a plain
		/// <code>de.odysseus.el.tree.impl.Builder</code> is used. If the configured class is a subclass
		/// of <code>de.odysseus.el.tree.impl.Builder</code> and which provides a constructor taking an
		/// array of <code>Builder.Feature</code>, this constructor will be invoked. Otherwise, the
		/// default constructor will be used.
		/// </summary>
		protected internal virtual TreeBuilder CreateTreeBuilder(Core.Model.Properties properties, params Builder.Feature[] features)
		{
			Type clazz = Load(typeof(TreeBuilder));
			//if (clazz == null)
			//{
				return new Builder(features);
			//}
			//try
			//{
			//	if (clazz.IsSubclassOf(typeof(Builder)))
			//	{
			//		Constructor<object> constructor = clazz.GetConstructor(typeof(Builder.Feature[]));
			//		if (constructor == null)
			//		{
			//			if (features == null || features.Length == 0)
			//			{
			//				return typeof(TreeBuilder).Cast(clazz.NewInstance());
			//			}
			//			else
			//			{
			//				throw new ELException("Builder " + clazz + " is missing constructor (can't pass features)");
			//			}
			//		}
			//		else
			//		{
			//			return typeof(TreeBuilder).Cast(constructor.NewInstance((object) features));
			//		}
			//	}
			//	else
			//	{
			//		return typeof(TreeBuilder).Cast(clazz.NewInstance());
			//	}
			//}
			//catch (Exception e)
			//{
			//	throw new ELException("TreeBuilder " + clazz + " could not be instantiated", e);
			//}
		}

		private Type Load(Type clazz )
		{
			//if (properties != null)
			//{
			//	string className = properties.GetProperty(clazz.FullName);
			//	if (!string.ReferenceEquals(className, null))
			//	{
			//		ClassLoader loader;
			//		try
			//		{
			//			loader = Thread.CurrentThread.ContextClassLoader;
			//		}
			//		catch (Exception e)
			//		{
			//			throw new ELException("Could not get context class loader", e);
			//		}
			//		try
			//		{
			//			return loader == null ? Type.GetType(className) : loader.LoadClass(className);
			//		}
			//		catch (ClassNotFoundException e)
			//		{
			//			throw new ELException("Class " + className + " not found", e);
			//		}
			//		catch (Exception e)
			//		{
			//			throw new ELException("Class " + className + " could not be instantiated", e);
			//		}
			//	}
			//}
			return null;
		}

		public override object CoerceToType(object obj, Type targetType)
		{
			return converter.Convert<object>(obj, targetType);
		}

		public override ValueExpression CreateValueExpression(object instance, Type expectedType)
		{
			return new ObjectValueExpression(converter, instance, expectedType);
		}

		public override ValueExpression CreateValueExpression(ELContext context, string expression, Type expectedType)
		{
			return new TreeValueExpression(store, context.FunctionMapper, context.VariableMapper, converter, expression, expectedType);
		}

		public override MethodExpression CreateMethodExpression(ELContext context, string expression, Type expectedReturnType, Type[] expectedParamTypes)
		{
			return new TreeMethodExpression(store, context.FunctionMapper, context.VariableMapper, converter, expression, expectedReturnType, expectedParamTypes);
		}
	}

}