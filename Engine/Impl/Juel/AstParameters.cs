using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
	public class AstParameters : AstRightValue
	{
		private readonly IList<AstNode> nodes;

		public AstParameters(IList<AstNode> nodes)
		{
			this.nodes = nodes;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			object[] result = new object[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				result[i] = nodes[i].Eval(bindings, context);
			}
			return result;
		}

		public override string ToString()
		{
			return "(...)";
		}

		public override void AppendStructure(StringBuilder builder, Bindings bindings)
		{
			builder.Append("(");
			for (int i = 0; i < nodes.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(", ");
				}
				nodes[i].AppendStructure(builder, bindings);
			}
			builder.Append(")");
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