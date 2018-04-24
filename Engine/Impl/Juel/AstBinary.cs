using System;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public class AstBinary : AstRightValue
	{
		public interface Operator
		{
			object Eval(Bindings bindings, ELContext context, AstNode left, AstNode right);
		}
		public abstract class SimpleOperator : Operator
		{
			public virtual object Eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				return Apply(bindings, left.Eval(bindings, context), right.Eval(bindings, context));
			}

			protected internal abstract object Apply(ITypeConverter converter, object o1, object o2);
		}
		public static readonly Operator ADD = new SimpleOperatorAnonymousInnerClass();

		private class SimpleOperatorAnonymousInnerClass : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return NumberOperations.Add(converter, o1, o2);
			}
			public override string ToString()
			{
				return "+";
			}
		}
		public static readonly Operator AND = new OperatorAnonymousInnerClass();

		private class OperatorAnonymousInnerClass : Operator
		{
			public OperatorAnonymousInnerClass()
			{
			}

			public virtual object Eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.Convert<bool>(left.Eval(bindings, context), typeof(Boolean));
				return true.Equals(l) && bindings.Convert<bool>(right.Eval(bindings, context), typeof(Boolean));
			}
			public override string ToString()
			{
				return "&&";
			}
		}
		public static readonly Operator DIV = new SimpleOperatorAnonymousInnerClass2();

		private class SimpleOperatorAnonymousInnerClass2 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass2()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return NumberOperations.Div(converter, o1, o2);
			}
			public override string ToString()
			{
				return "/";
			}
		}
		public static readonly Operator EQ = new SimpleOperatorAnonymousInnerClass3();

		private class SimpleOperatorAnonymousInnerClass3 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass3()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Eq(converter, o1, o2);
			}
			public override string ToString()
			{
				return "==";
			}
		}
		public static readonly Operator GE = new SimpleOperatorAnonymousInnerClass4();

		private class SimpleOperatorAnonymousInnerClass4 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass4()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Ge(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">=";
			}
		}
		public static readonly Operator GT = new SimpleOperatorAnonymousInnerClass5();

		private class SimpleOperatorAnonymousInnerClass5 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass5()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Gt(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">";
			}
		}
		public static readonly Operator LE = new SimpleOperatorAnonymousInnerClass6();

		private class SimpleOperatorAnonymousInnerClass6 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass6()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Le(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<=";
			}
		}
		public static readonly Operator LT = new SimpleOperatorAnonymousInnerClass7();

		private class SimpleOperatorAnonymousInnerClass7 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass7()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Lt(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<";
			}
		}
		public static readonly Operator MOD = new SimpleOperatorAnonymousInnerClass8();

		private class SimpleOperatorAnonymousInnerClass8 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass8()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return NumberOperations.Mod(converter, o1, o2);
			}
			public override string ToString()
			{
				return "%";
			}
		}
		public static readonly Operator MUL = new SimpleOperatorAnonymousInnerClass9();

		private class SimpleOperatorAnonymousInnerClass9 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass9()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return NumberOperations.Mul(converter, o1, o2);
			}
			public override string ToString()
			{
				return "*";
			}
		}
		public static readonly Operator NE = new SimpleOperatorAnonymousInnerClass10();

		private class SimpleOperatorAnonymousInnerClass10 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass10()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.Ne(converter, o1, o2);
			}
			public override string ToString()
			{
				return "!=";
			}
		}
		public static readonly Operator OR = new OperatorAnonymousInnerClass2();

		private class OperatorAnonymousInnerClass2 : Operator
		{
			public OperatorAnonymousInnerClass2()
			{
			}

			public virtual object Eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.Convert<bool>(left.Eval(bindings, context), typeof(Boolean));
				return true.Equals(l) || bindings.Convert<bool>(right.Eval(bindings, context), typeof(Boolean));
			}
			public override string ToString()
			{
				return "||";
			}
		}
		public static readonly Operator SUB = new SimpleOperatorAnonymousInnerClass11();

		private class SimpleOperatorAnonymousInnerClass11 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClass11()
			{
			}

		    protected internal override object Apply(ITypeConverter converter, object o1, object o2)
			{
				return NumberOperations.Sub(converter, o1, o2);
			}
			public override string ToString()
			{
				return "-";
			}
		}

		private readonly Operator @operator;
		private readonly AstNode left, right;

		public AstBinary(AstNode left, AstNode right, Operator @operator)
		{
			this.left = left;
			this.right = right;
			this.@operator = @operator;
		}

		public virtual Operator GetOperator()
		{
			return @operator;
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			return @operator.Eval(bindings, context, left, right);
		}

		public override string ToString()
		{
			return "'" + @operator.ToString() + "'";
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			left.AppendStructure(b, bindings);
			b.Append(' ');
			b.Append(@operator);
			b.Append(' ');
			right.AppendStructure(b, bindings);
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
			return i == 0 ? left : i == 1 ? right : null;
		}
	}

}