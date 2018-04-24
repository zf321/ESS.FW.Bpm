using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public sealed class AstBoolean : AstLiteral
	{
		private readonly bool value;

		public AstBoolean(bool value)
		{
			this.value = value;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return value;
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(value);
		}
	}

}