using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public sealed class AstNumber : AstLiteral
	{
		private readonly decimal value;

		public AstNumber(decimal value)
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