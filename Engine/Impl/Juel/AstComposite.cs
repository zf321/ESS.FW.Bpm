using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public class AstComposite : AstRightValue
	{
		private readonly IList<AstNode> nodes;

		public AstComposite(IList<AstNode> nodes)
		{
			this.nodes = nodes;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			StringBuilder b = new StringBuilder(16);
			for (int i = 0; i < Cardinality; i++)
			{
				b.Append(bindings.Convert<string>(nodes[i].Eval(bindings, context), typeof(string)));
			}
			return b.ToString();
		}

		public override string ToString()
		{
			return "composite";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			for (int i = 0; i < Cardinality; i++)
			{
				nodes[i].AppendStructure(b, bindings);
			}
		}

		public override int Cardinality
		{
			get
			{
				return nodes.Count;
			}
		}

		public override INode GetChild(int i)
		{
			return nodes[i];
		}
	}

}