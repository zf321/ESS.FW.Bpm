using System;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

    public class AstChoice : AstRightValue
	{
		private readonly AstNode question, yes, no;

		public AstChoice(AstNode question, AstNode yes, AstNode no)
		{
			this.question = question;
			this.yes = yes;
			this.no = no;
		}
        
		public override object Eval(Bindings bindings, ELContext context)
		{
			bool? value = bindings.Convert<bool>(question.Eval(bindings, context), typeof(Boolean));
			return value.Value ? yes.Eval(bindings, context) : no.Eval(bindings, context);
		}

		public override string ToString()
		{
			return "?";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			question.AppendStructure(b, bindings);
			b.Append(" ? ");
			yes.AppendStructure(b, bindings);
			b.Append(" : ");
			no.AppendStructure(b, bindings);
		}

		public override int Cardinality
		{
			get
			{
				return 3;
			}
		}

		public override INode GetChild(int i)
		{
			return i == 0 ? question : i == 1 ? yes : i == 2 ? no : null;
		}
	}

}