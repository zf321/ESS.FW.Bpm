using System;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

    public class AstUnary : AstRightValue
	{
		public interface Operator
		{
			object Eval(Bindings bindings, ELContext context, AstNode node);
		}
		public abstract class SimpleOperator : Operator
		{
			public virtual object Eval(Bindings bindings, ELContext context, AstNode node)
			{
				return Apply(bindings, node.Eval(bindings, context));
			}

			protected internal abstract object Apply(ITypeConverter converter, object o);
		}
		public static readonly Operator EMPTY = new SimpleOperatorAnonymousInnerClass();

		private class SimpleOperatorAnonymousInnerClass : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o)
			{
				return BooleanOperations.Empty(converter, o);
			}
			public override string ToString()
			{
				return "empty";
			}
		}
		public static readonly Operator NEG = new SimpleOperatorAnonymousInnerClass2();

		private class SimpleOperatorAnonymousInnerClass2 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass2()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o)
			{
				return NumberOperations.Neg(converter, o);
			}
			public override string ToString()
			{
				return "-";
			}
		}
		public static readonly Operator NOT = new SimpleOperatorAnonymousInnerClass3();

		private class SimpleOperatorAnonymousInnerClass3 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass3()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o)
			{
				return !converter.Convert<bool>(o, typeof(Boolean));
			}
			public override string ToString()
			{
				return "!";
			}
		}

		private readonly Operator @operator;
		private readonly AstNode child;

		public AstUnary(AstNode child, AstUnary.Operator @operator)
		{
			this.child = child;
			this.@operator = @operator;
		}

		public virtual Operator GetOperator()
		{
			return @operator;
		}
        
		public override object Eval(Bindings bindings, ELContext context)
		{
			return @operator.Eval(bindings, context, child);
		}

		public override string ToString()
		{
			return "'" + @operator.ToString() + "'";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(@operator);
			b.Append(' ');
			child.AppendStructure(b, bindings);
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