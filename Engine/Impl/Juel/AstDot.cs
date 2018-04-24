using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    public class AstDot : AstProperty
	{
		protected internal readonly string property;

		public AstDot(AstNode @base, string property, bool lvalue) : base(@base, lvalue, true)
		{
			this.property = property;
		}
		protected internal override object GetProperty(Bindings bindings, ELContext context)
		{
			return property;
		}

		public override string ToString()
		{
			return ". " + property;
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			((AstDot)GetChild(0)).AppendStructure(b, bindings);
			b.Append(".");
			b.Append(property);
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}
	}

}