using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{


	public sealed class AstNull : AstLiteral
	{
		public override object Eval(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override string ToString()
		{
			return "null";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append("null");
		}
	}

}