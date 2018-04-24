using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Simple context implementation.
	/// 
	/// 
	/// </summary>
	public class SimpleContext : ELContext
	{
		 class Functions : FunctionMapper
		{
			internal IDictionary<string, MethodInfo> map = new Dictionary<string, MethodInfo>();

			public override MethodInfo ResolveFunction(string prefix, string localName)
			{
				return map[prefix + ":" + localName];
			}

			public virtual void SetFunction(string prefix, string localName, MethodInfo method)
			{
				if (map.Count == 0)
				{
					map = new Dictionary<string, MethodInfo>();
				}
				map[prefix + ":" + localName] = method;
			}
		}

		internal class Variables : VariableMapper
		{
            public static ITypedValue UntypedNullValue = NullValueImpl.Instance;
            internal IDictionary<string, ValueExpression> map = new Dictionary<string, ValueExpression>();

			public override ValueExpression ResolveVariable(string variable)
			{
				return map[variable];
			}

			public override ValueExpression SetVariable(string variable, ValueExpression expression)
			{
				if (map.Count == 0)
				{
					map = new Dictionary<string, ValueExpression>();
				}
				return map[variable] = expression;
			}
		}

		private Functions functions;
		private Variables variables;
		private ELResolver resolver;

		/// <summary>
		/// Create a context.
		/// </summary>
		public SimpleContext() : this(null)
		{
		}

		/// <summary>
		/// Create a context, use the specified resolver.
		/// </summary>
		public SimpleContext(ELResolver resolver)
		{
			this.resolver = resolver;
		}

		/// <summary>
		/// Define a function.
		/// </summary>
		public virtual void SetFunction(string prefix, string localName, MethodInfo method)
		{
			if (functions == null)
			{
				functions = new Functions();
			}
			functions.SetFunction(prefix, localName, method);
		}

		/// <summary>
		/// Define a variable.
		/// </summary>
		public virtual ValueExpression SetVariable(string name, ValueExpression expression)
		{
			if (variables == null)
			{
				variables = new Variables();
			}
			return variables.SetVariable(name, expression);
		}

		/// <summary>
		/// Get our function mapper.
		/// </summary>
		public override FunctionMapper FunctionMapper
		{
			get
			{
				if (functions == null)
				{
					functions = new Functions();
				}
				return functions;
			}
		}

		/// <summary>
		/// Get our variable mapper.
		/// </summary>
		public override VariableMapper VariableMapper
		{
			get
			{
				if (variables == null)
				{
					variables = new Variables();
				}
				return variables;
			}
		}

		/// <summary>
		/// Get our resolver. Lazy initialize to a <seealso cref="SimpleResolver"/> if necessary.
		/// </summary>
		public override ELResolver ELResolver
		{
			get
			{
				if (resolver == null)
				{
					resolver = new SimpleResolver();
				}
				return resolver;
			}
			//set
			//{
			//	this.resolver = value;
			//}
		}

	}

}