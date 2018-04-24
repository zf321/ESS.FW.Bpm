using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public sealed class AstNested : AstRightValue
	{
		private readonly AstNode child;

		public AstNested(AstNode child)
		{
			this.child = child;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return child.Eval(bindings, context);
		}

		public override string ToString()
		{
			return "(...)";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append("(");
			child.AppendStructure(b, bindings);
			b.Append(")");
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