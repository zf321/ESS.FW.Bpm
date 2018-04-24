using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	public sealed class AstString : AstLiteral
	{
		private readonly string value;

		public AstString(string value)
		{
			this.value = value;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return value;
		}

		public override string ToString()
		{
			return "\"" + value + "\"";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append("'");
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c == '\\' || c == '\'')
				{
					b.Append('\\');
				}
				b.Append(c);
			}
			b.Append("'");
		}
	}

}