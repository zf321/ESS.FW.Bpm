using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// A method expression is ready to be evaluated (by calling either
	/// <seealso cref="#invoke(ELContext, Object[])"/> or <seealso cref="#getMethodInfo(ELContext)"/>).
	/// 
	/// Instances of this class are usually created using an <seealso cref="ExpressionFactoryImpl"/>.
	/// 
	/// 
	/// </summary>
	public sealed class TreeMethodExpression : MethodExpression
	{
		private const long serialVersionUID = 1L;

		private readonly TreeBuilder builder;
		private readonly Bindings bindings;
		private readonly string expr;
		private readonly Type type;
		private readonly Type[] types;
		private readonly bool deferred;

		[NonSerialized]
		private IExpressionNode node;

		private string structure;

		/// <summary>
		/// Create a new method expression.
		/// The expression must be an lvalue expression or literal text.
		/// The expected return type may be <code>null</code>, meaning "don't care".
		/// If it is an lvalue expression, the parameter types must not be <code>null</code>.
		/// If it is literal text, the expected return type must not be <code>void</code>. </summary>
		/// <param name="store"> used to get the parse tree from. </param>
		/// <param name="functions"> the function mapper used to bind functions </param>
		/// <param name="variables"> the variable mapper used to bind variables </param>
		/// <param name="expr"> the expression string </param>
		/// <param name="returnType"> the expected return type (may be <code>null</code>) </param>
		/// <param name="paramTypes"> the expected parameter types (must not be <code>null</code> for lvalues) </param>
		public TreeMethodExpression(TreeStore store, FunctionMapper functions, VariableMapper variables, ITypeConverter converter, string expr, Type returnType, Type[] paramTypes) : base()
		{

			Tree tree = store.Get(expr);

			this.builder = store.Builder;
			this.bindings = tree.Bind(functions, variables, converter);
			this.expr = expr;
			this.type = returnType;
			this.types = paramTypes;
			this.node = tree.Root;
			this.deferred = tree.Deferred;

			if (node.LiteralText)
			{
				if (returnType == typeof(void))
				{
					throw new ELException(LocalMessages.Get("error.method.literal.void", expr));
				}
			}
			else if (!node.MethodInvocation)
			{
				if (!node.LeftValue)
				{
					throw new ELException(LocalMessages.Get("error.method.invalid", expr));
				}
				if (paramTypes == null)
				{
					throw new ELException(LocalMessages.Get("error.method.notypes"));
				}
			}
		}

		private string StructuralId
		{
			get
			{
				if (string.ReferenceEquals(structure, null))
				{
					structure = node.GetStructuralId(bindings);
				}
				return structure;
			}
		}

	  /// <summary>
	  /// Evaluates the expression and answers information about the method </summary>
	  /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>) </param>
	  /// <returns> method information or <code>null</code> for literal expressions </returns>
	  /// <exception cref="ELException"> if evaluation fails (e.g. suitable method not found) </exception>
		public override MethodInfo GetMethodInfo(ELContext context)
		{
			return node.GetMethodInfo(bindings, context, type, types);
		}

		public override string ExpressionString
		{
			get
			{
				return expr;
			}
		}

		/// <summary>
		/// Evaluates the expression and invokes the method. </summary>
		/// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>) </param>
		/// <param name="paramValues"> </param>
		/// <returns> method result or <code>null</code> if this is a literal text expression </returns>
		/// <exception cref="ELException"> if evaluation fails (e.g. suitable method not found) </exception>
		public override object Invoke(ELContext context, object[] paramValues)
		{
			return node.Invoke(bindings, context, type, types, paramValues);
		}

		/// <returns> <code>true</code> if this is a literal text expression </returns>
		public override bool IsLiteralText
		{
			get
			{
				return node.LiteralText;
			}
		}

		/// <returns> <code>true</code> if this is a method invocation expression </returns>
		public override bool ParmetersProvided
		{
			get
			{
				return node.MethodInvocation;
			}
		}

		/// <summary>
		/// Answer <code>true</code> if this is a deferred expression (starting with <code>#{</code>)
		/// </summary>
		public bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		/// <summary>
		/// Expressions are compared using the concept of a <em>structural id</em>:
		/// variable and function names are anonymized such that two expressions with
		/// same tree structure will also have the same structural id and vice versa.
		/// Two method expressions are equal if
		/// <ol>
		/// <li>their builders are equal</li>
		/// <li>their structural id's are equal</li>
		/// <li>their bindings are equal</li>
		/// <li>their expected types match</li>
		/// <li>their parameter types are equal</li>
		/// </ol>
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == this.GetType())
			{
				TreeMethodExpression other = (TreeMethodExpression)obj;
				if (!builder.Equals(other.builder))
				{
					return false;
				}
				if (type != other.type)
				{
					return false;
				}
				if (!Enumerable.SequenceEqual(types, other.types))
				{
					return false;
				}
				return StructuralId.Equals(other.StructuralId) && bindings.Equals(other.bindings);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return StructuralId.GetHashCode();
		}

		public override string ToString()
		{
			return "TreeMethodExpression(" + expr + ")";
		}

		/// <summary>
		/// Print the parse tree. </summary>
		/// <param name="writer"> </param>
		public void Dump(StreamWriter writer)
		{
			NodePrinter.Dump(writer, node);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(StreamReader @in)
		{
			//@in.DefaultReadObject();
			//try
			//{
			//	node = builder.Build(expr).Root;
			//}
			//catch (ELException e)
			//{
			//	throw new IOException(e.Message);
			//}
		}
	}

}