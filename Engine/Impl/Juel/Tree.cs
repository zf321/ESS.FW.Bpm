using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    


	/// <summary>
	/// Parsed expression, usually created by a <seealso cref="TreeBuilder"/>.
	/// The <seealso cref="#bind(FunctionMapper, VariableMapper)"/> method is used to create
	/// <seealso cref="Bindings"/>, which are needed at evaluation time to
	/// lookup functions and variables. The tree itself does not contain such information,
	/// because it would make the tree depend on the function/variable mapper supplied at
	/// parse time.
	/// 
	/// 
	/// </summary>
	public class Tree
	{
		private readonly IExpressionNode root;
		private readonly ICollection<IFunctionNode> functions;
		private readonly ICollection<IIdentifierNode> identifiers;
		private readonly bool deferred;

		/// 
		/// <summary>
		/// Constructor. </summary>
		/// <param name="root"> root node </param>
		/// <param name="functions"> collection of function nodes </param>
		/// <param name="identifiers"> collection of identifier nodes </param>
		public Tree(IExpressionNode root, ICollection<IFunctionNode> functions, ICollection<IIdentifierNode> identifiers, bool deferred) : base()
		{
			this.root = root;
			this.functions = functions;
			this.identifiers = identifiers;
			this.deferred = deferred;
		}

		/// <summary>
		/// Get function nodes (in no particular order)
		/// </summary>
		public virtual IEnumerable<IFunctionNode> FunctionNodes
		{
			get
			{
				return functions;
			}
		}

		/// <summary>
		/// Get identifier nodes (in no particular order)
		/// </summary>
		public virtual IEnumerable<IIdentifierNode> IdentifierNodes
		{
			get
			{
				return identifiers;
			}
		}

		/// <returns> root node </returns>
		public virtual IExpressionNode Root
		{
			get
			{
				return root;
			}
		}

		public virtual bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		public override string ToString()
		{
			return Root.GetStructuralId(null);
		}

		/// <summary>
		/// Create a bindings. </summary>
		/// <param name="fnMapper"> the function mapper to use </param>
		/// <param name="varMapper"> the variable mapper to use </param>
		/// <returns> tree bindings </returns>
		public virtual Bindings Bind(FunctionMapper fnMapper, VariableMapper varMapper)
		{
			return Bind(fnMapper, varMapper, null);
		}

		/// <summary>
		/// Create a bindings. </summary>
		/// <param name="fnMapper"> the function mapper to use </param>
		/// <param name="varMapper"> the variable mapper to use </param>
		/// <param name="converter"> custom type converter </param>
		/// <returns> tree bindings </returns>
		public virtual Bindings Bind(FunctionMapper fnMapper, VariableMapper varMapper, ITypeConverter converter)
		{
			MethodInfo[] methods = null;
			if (functions.Count > 0)
			{
				if (fnMapper == null)
				{
					throw new ELException(LocalMessages.Get("error.function.nomapper"));
				}
				methods = new MethodInfo[functions.Count];
				foreach (IFunctionNode node in functions)
				{
					string image = node.Name;
                    MethodInfo method = null;
					int colon = image.IndexOf(':');
					if (colon < 0)
					{
						method = fnMapper.ResolveFunction("", image);
					}
					else
					{
						method = fnMapper.ResolveFunction(image.Substring(0, colon), image.Substring(colon + 1));
					}
					if (method == null)
					{
						throw new ELException(LocalMessages.Get("{0} error.function.notfound", image));
					}
					//if (node.VarArgs && method.VarArgs)
					//{
					//	if (method.ParameterTypes.length > node.ParamCount + 1)
					//	{
					//		throw new ELException(LocalMessages.Get("error.function.params", image));
					//	}
					//}
					//else
					//{
					//	if (method.ParameterTypes.length != node.ParamCount)
					//	{
					//		throw new ELException(LocalMessages.Get("error.function.params", image));
					//	}
					//}
					methods[node.Index] = method;
				}
			}
			ValueExpression[] expressions = null;
			if (identifiers.Count > 0)
			{
				expressions = new ValueExpression[identifiers.Count];
				foreach (IIdentifierNode node in identifiers)
				{
					ValueExpression expression = null;
					if (varMapper != null)
					{
						expression = varMapper.ResolveVariable(node.Name);
					}
					expressions[node.Index] = expression;
				}
			}
			return new Bindings(methods, expressions, converter);
		}
	}

}