using System;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public sealed class AstText : AstNode
	{
		private readonly string value;

		public AstText(string value)
		{
			this.value = value;
		}

		public override bool LiteralText
		{
			get
			{
				return true;
			}
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

		public override ValueReference GetValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return value;
		}

		public override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			return returnType == null ? value : bindings.Convert<string>(value, returnType);
		}

		public override string ToString()
		{
			return "\"" + value + "\"";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			int end = value.Length - 1;
			for (int i = 0; i < end; i++)
			{
				char c = value[i];
				if ((c == '#' || c == '$') && value[i + 1] == '{')
				{
					b.Append('\\');
				}
				b.Append(c);
			}
			if (end >= 0)
			{
				b.Append(value[end]);
			}
		}

		public override int Cardinality
		{
			get
			{
				return 0;
			}
		}

		public override INode GetChild(int i)
		{
			return null;
		}
	}

}