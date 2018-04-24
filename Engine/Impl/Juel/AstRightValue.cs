using System;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	/// <summary>
	/// 
	/// </summary>
	public abstract class AstRightValue : AstNode
	{
		/// <summary>
		/// Answer <code>false</code>
		/// </summary>
		public sealed override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// according to the spec, the result is undefined for rvalues, so answer <code>null</code>
		/// </summary>
		public sealed override Type GetType(Bindings bindings, ELContext context)
		{
			return null;
		}

		/// <summary>
		/// non-lvalues are always readonly, so answer <code>true</code>
		/// </summary>
		public sealed override bool IsReadOnly(Bindings bindings, ELContext context)
		{
			return true;
		}

		/// <summary>
		/// non-lvalues are always readonly, so throw an exception
		/// </summary>
		public sealed override void SetValue(Bindings bindings, ELContext context, object value)
		{
			throw new ELException(LocalMessages.Get("error.value.set.rvalue", GetStructuralId(bindings)));
		}

		public sealed override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public sealed override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			throw new ELException(LocalMessages.Get("error.method.invalid", GetStructuralId(bindings)));
		}

		public sealed override bool LeftValue
		{
			get
			{
				return false;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return false;
			}
		}

		public sealed override ValueReference GetValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}
	}

}