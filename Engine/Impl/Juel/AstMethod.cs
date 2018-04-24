using System;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    


	public class AstMethod : AstNode
	{
		private readonly AstProperty property;
		private readonly AstParameters @params;

		public AstMethod(AstProperty property, AstParameters @params)
		{
			this.property = property;
			this.@params = @params;
		}

		public override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		public override Type GetType(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override bool IsReadOnly(Bindings bindings, ELContext context)
		{
			return true;
		}

		public override void SetValue(Bindings bindings, ELContext context, object value)
		{
			throw new ELException(LocalMessages.Get("error.value.set.rvalue", GetStructuralId(bindings)));
		}

		public override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public override bool LeftValue
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
				return true;
			}
		}

		public sealed override ValueReference GetValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override void AppendStructure(StringBuilder builder, Bindings bindings)
		{
			property.AppendStructure(builder, bindings);
			@params.AppendStructure(builder, bindings);
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return Invoke(bindings, context, null, null, null);
		}

		public override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			object @base = property.Prefix.Eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", property.Prefix));
			}
			object method = property.GetProperty(bindings, context);
			if (method == null)
			{
				throw new PropertyNotFoundException(LocalMessages.Get("error.property.method.notfound", "null", @base));
			}
			string name = bindings.Convert<string>(method, typeof(string));
			paramValues = (object[]) @params.Eval(bindings, context);

			context.PropertyResolved = false;
			object result = context.ELResolver.Invoke(context, @base, name, paramTypes, paramValues);
			if (!context.PropertyResolved)
			{
				throw new MethodNotFoundException(LocalMessages.Get("error.property.method.notfound", name, @base.GetType()));
			}
	//		if (returnType != null && !returnType.isInstance(result)) { // should we check returnType for method invocations?
	//			throw new MethodNotFoundException(LocalMessages.get("error.property.method.notfound", name, base.getClass()));
	//		}
			return result;
		}

		public override int Cardinality
		{
			get
			{
				return 2;
			}
		}

		public override INode GetChild(int i)
		{
			return i == 0 ?  (INode) property : i == 1 ? @params : null;
		}

		public override string ToString()
		{
			return "<method>";
		}
	}

}