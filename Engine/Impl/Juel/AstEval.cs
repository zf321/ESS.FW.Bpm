using System;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
	public sealed class AstEval : AstNode
	{
		private readonly AstNode child;
		private readonly bool deferred;

		public AstEval(AstNode child, bool deferred)
		{
			this.child = child;
			this.deferred = deferred;
		}

		public bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		public override bool LeftValue
		{
			get
			{
				return ((AstEval)GetChild(0)).LeftValue;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return ((AstEval)GetChild(0)).MethodInvocation;
			}
		}

		public override ValueReference GetValueReference(Bindings bindings, ELContext context)
		{
			return child.GetValueReference(bindings, context);
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return child.Eval(bindings, context);
		}

		public override string ToString()
		{
			return (deferred ? "#" : "$") + "{...}";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(deferred ? "#{" : "${");
			child.AppendStructure(b, bindings);
			b.Append("}");
		}

		public override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return child.GetMethodInfo(bindings, context, returnType, paramTypes);
		}

		public override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			return child.Invoke(bindings, context, returnType, paramTypes, paramValues);
		}

		public override Type GetType(Bindings bindings, ELContext context)
		{
			return child.GetType(bindings, context);
		}

		public override bool LiteralText
		{
			get
			{
				return child.LiteralText;
			}
		}

		public override bool IsReadOnly(Bindings bindings, ELContext context)
		{
			return child.IsReadOnly(bindings, context);
		}

		public override void SetValue(Bindings bindings, ELContext context, object value)
		{
			child.SetValue(bindings, context, value);
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}

		public override INode GetChild(int i)
		{
			return i == 0 ? child : null;
		}
	}

}