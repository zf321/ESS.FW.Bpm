using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Tree builder.
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class Builder : TreeBuilder
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Feature enumeration type.
		/// </summary>
		public enum Feature
		{
			/// <summary>
			/// Method invocations as in <code>${foo.bar(1)}</code> as specified in JSR 245,
			/// maintenance release 2.
			/// The method to be invoked is resolved at evaluation time by calling
			/// <seealso cref="ELResolver#invoke(javax.el.ELContext, Object, Object, Class[], Object[])"/>.
			/// </summary>
			METHOD_INVOCATIONS,
			/// <summary>
			/// For some reason we don't understand, the specification does not allow to resolve
			/// <code>null</code> property values. E.g. <code>${map[key]}</code> will always
			/// return <code>null</code> if <code>key</code> evaluates to <code>null</code>.
			/// Enabling this feature will allow <em>JUEL</em> to pass <code>null</code> to
			/// the property resolvers just like any other property value.
			/// </summary>
			NULL_PROPERTIES,
			/// <summary>
			/// Allow for use of Java 5 varargs in function calls.
			/// </summary>
			VARARGS
		}

		//protected internal readonly EnumSet<Feature> features;

		public Builder()
		{
			//this.features = EnumSet.NoneOf(typeof(Feature));
		}

		public Builder(params Feature[] features)
		{
			if (features == null || features.Length == 0)
			{
				//this.features = EnumSet.NoneOf(typeof(Feature));
			}
			else if (features.Length == 1)
			{
				//this.features = EnumSet.Of(features[0]);
			}
			else
			{
				Feature[] rest = new Feature[features.Length - 1];
				for (int i = 1; i < features.Length; i++)
				{
					rest[i - 1] = features[i];
				}
				//this.features = EnumSet.Of(features[0], rest);
			}
		}

        /// <returns> <code>true</code> iff the specified feature is supported. </returns>
        public virtual bool IsEnabled(Feature feature)
        {
            return true;
            //return features.Contains(feature);
        }

        /// <summary>
        /// Parse expression.
        /// </summary>
        public virtual Tree Build(string expression)
		{
			try
			{
				return CreateParser(expression).Tree();
			}
			catch (Scanner.ScanException e)
			{
				throw new TreeBuilderException(expression, e.position, e.encountered, e.expected, e.Message);
			}
			catch (Parser.ParseException e)
			{
				throw new TreeBuilderException(expression, e.position, e.encountered, e.expected, e.Message);
			}
		}

		protected internal virtual Parser CreateParser(string expression)
		{
			return new Parser(this, expression);
		}

		//public override bool Equals(object obj)
		//{
		//	if (obj == null || obj.GetType() != this.GetType())
		//	{
		//		return false;
		//	}
		//	//return features.Equals(((Builder)obj).features);
		//}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		/// <summary>
		/// Dump out abstract syntax tree for a given expression
		/// </summary>
		/// <param name="args"> array with one element, containing the expression string </param>
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.Error.WriteLine("usage: java " + typeof(Builder).FullName + " <expression string>");
				Environment.Exit(1);
			}
			//StreamWriter @out = new StreamWriter();
			//Tree tree = null;
			//try
			//{
			//	tree = (new Builder(Feature.METHOD_INVOCATIONS)).Build(args[0]);
			//}
			//catch (TreeBuilderException e)
			//{
			//	Console.WriteLine(e.Message);
			//	Environment.Exit(0);
			//}
			//NodePrinter.Dump(@out, tree.Root);
			//if (!tree.FunctionNodes.GetEnumerator().HasNext() && !tree.IdentifierNodes.GetEnumerator().HasNext())
			//{
			//	ELContext context = new ELContextAnonymousInnerClass();
			//	@out.Print(">> ");
			//	try
			//	{
			//		@out.Println(tree.Root.GetValue(new Bindings(null, null), context, null));
			//	}
			//	catch (ELException e)
			//	{
			//		@out.Println(e.Message);
			//	}
			//}
			//@out.Flush();
		}

		private class ELContextAnonymousInnerClass : ELContext
		{
			public ELContextAnonymousInnerClass()
			{
			}

			public override VariableMapper VariableMapper
			{
				get
				{
					return null;
				}
			}
			public override FunctionMapper FunctionMapper
			{
				get
				{
					return null;
				}
			}
			public override ELResolver ELResolver
			{
				get
				{
					return null;
				}
			}
		}
	}

}