using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
    public class AstBracket : AstProperty
	{
		protected internal readonly AstNode property;

		public AstBracket(AstNode @base, AstNode property, bool lvalue, bool strict) : base(@base, lvalue, strict)
		{
			this.property = property;
		}
        
		protected internal override object GetProperty(Bindings bindings, ELContext context)
		{
			return property.Eval(bindings, context);
		}

		public override string ToString()
		{
			return "[...]";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			((AstNode)GetChild(0)).AppendStructure(b, bindings);
			b.Append("[");
            ((AstNode)GetChild(1)).AppendStructure(b, bindings);
			b.Append("]");
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
			return i == 1 ? property : base.GetChild(i);
		}
	}

}