using System;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
	public abstract class AstNode : IExpressionNode
	{
		public abstract INode GetChild(int i);
		public abstract int Cardinality {get;}
		public abstract object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues);
		public abstract MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes);
		public abstract void SetValue(Bindings bindings, ELContext context, object value);
		public abstract bool IsReadOnly(Bindings bindings, ELContext context);
		public abstract Type GetType(Bindings bindings, ELContext context);
		public abstract ValueReference GetValueReference(Bindings bindings, ELContext context);
		public abstract bool MethodInvocation {get;}
		public abstract bool LeftValue {get;}
		public abstract bool LiteralText {get;}
		/// <summary>
		/// evaluate and return the (optionally coerced) result.
		/// </summary>
		public object GetValue(Bindings bindings, ELContext context, Type type)
		{
			object value = Eval(bindings, context);
			if (type != null)
			{
				value = bindings.Convert<object>(value, type);
			}
			return value;
		}

		public abstract void AppendStructure(StringBuilder builder, Bindings bindings);

		public abstract object Eval(Bindings bindings, ELContext context);

		public string GetStructuralId(Bindings bindings)
		{
			StringBuilder builder = new StringBuilder();
			AppendStructure(builder, bindings);
			return builder.ToString();
		}
	}

}