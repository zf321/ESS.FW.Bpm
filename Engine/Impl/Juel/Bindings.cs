using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

    

	/// <summary>
	/// Bindings, usually created by a <seealso cref="Tree"/>.
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class Bindings : ITypeConverter
	{
		private const long serialVersionUID = 1L;

		private static readonly MethodInfo[] NO_FUNCTIONS = new MethodInfo[0];
		private static readonly ValueExpression[] NO_VARIABLES = new ValueExpression[0];

		/// <summary>
		/// Wrap a <seealso cref="Method"/> for serialization.
		/// </summary>
		[Serializable]
		private class MethodWrapper
		{
			internal const long serialVersionUID = 1L;

			[NonSerialized]
			internal MethodInfo method;
			internal MethodWrapper(MethodInfo method)
			{
				this.method = method;
			}
			//internal virtual void WriteObject(Stream @out)
			//{
			//	@out.DefaultWriteObject();
			//	@out.WriteObject(method.DeclaringClass);
			//	@out.WriteObject(method.Name);
			//	@out.WriteObject(method.ParameterTypes);
			//}
			//internal virtual void ReadObject(Stream @in)
			//{
			//	@in.DefaultReadObject();
			//	Type type = (Type)@in.ReadObject();
			//	string name = (string)@in.ReadObject();
			//	Type[] args = (Type[])@in.ReadObject();
			//	try
			//	{
			//		method = type.GetDeclaredMethod(name, args);
			//	}
			//	catch (NoSuchMethodException e)
			//	{
			//		throw new IOException(e.Message);
			//	}
			//}
		}

		[NonSerialized]
		private MethodInfo[] functions;
		private readonly ValueExpression[] variables;
		private readonly ITypeConverter converter;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Bindings(MethodInfo[] functions, ValueExpression[] variables) : this(functions, variables, TypeConverterFields.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Bindings(MethodInfo[] functions, ValueExpression[] variables, ITypeConverter converter) : base()
		{

			this.functions = functions == null || functions.Length == 0 ? NO_FUNCTIONS : functions;
			this.variables = variables == null || variables.Length == 0 ? NO_VARIABLES : variables;
			this.converter = converter == null ? TypeConverterFields.Default : converter;
		}

		/// <summary>
		/// Get function by index. </summary>
		/// <param name="index"> function index </param>
		/// <returns> method </returns>
		public virtual MethodInfo GetFunction(int index)
		{
			return functions[index];
		}

		/// <summary>
		/// Test if given index is bound to a function.
		/// This method performs an index check. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> <code>true</code> if the given index is bound to a function </returns>
		public virtual bool IsFunctionBound(int index)
		{
			return index >= 0 && index < functions.Length;
		}

		/// <summary>
		/// Get variable by index. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> value expression </returns>
		public virtual ValueExpression GetVariable(int index)
		{
			return variables[index];
		}

		/// <summary>
		/// Test if given index is bound to a variable.
		/// This method performs an index check. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> <code>true</code> if the given index is bound to a variable </returns>
		public virtual bool IsVariableBound(int index)
		{
			return index >= 0 && index < variables.Length && variables[index] != null;
		}

		/// <summary>
		/// Apply type conversion. </summary>
		/// <param name="value"> value to convert </param>
		/// <param name="type"> target type </param>
		/// <returns> converted value </returns>
		/// <exception cref="ELException"> </exception>
		public virtual T Convert<T>(object value, Type type)
		{
			return converter.Convert<T>(value, type);
		}

		public override bool Equals(object obj)
		{
			if (obj is Bindings)
			{
				Bindings other = (Bindings)obj;
				return functions.SequenceEqual(other.functions) && variables.SequenceEqual(other.variables) && converter.Equals(other.converter);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (functions).GetHashCode() ^ (variables).GetHashCode() ^ converter.GetHashCode();
		}
        
		//private void WriteObject(ObjectOutputStream @out)
		//{
		//	@out.DefaultWriteObject();
		//	MethodWrapper[] wrappers = new MethodWrapper[functions.Length];
		//	for (int i = 0; i < wrappers.Length; i++)
		//	{
		//		wrappers[i] = new MethodWrapper(functions[i]);
		//	}
		//	@out.WriteObject(wrappers);
		//}
        
		//private void ReadObject(ObjectInputStream @in)
		//{
		//	@in.DefaultReadObject();
		//	MethodWrapper[] wrappers = (MethodWrapper[])@in.ReadObject();
		//	if (wrappers.Length == 0)
		//	{
		//		functions = NO_FUNCTIONS;
		//	}
		//	else
		//	{
		//		functions = new MethodInfo[wrappers.Length];
		//		for (int i = 0; i < functions.Length; i++)
		//		{
		//			functions[i] = wrappers[i].method;
		//		}
		//	}
		//}
	}

}