using System;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Object wrapper expression.
	/// 
	/// 
	/// </summary>
	public sealed class ObjectValueExpression : ValueExpression
	{
		private const long serialVersionUID = 1L;

		private readonly ITypeConverter converter;
		private readonly object @object;
		private readonly Type type;

		/// <summary>
		/// Wrap an object into a value expression. </summary>
		/// <param name="converter"> type converter </param>
		/// <param name="object"> the object to wrap </param>
		/// <param name="type"> the expected type this object will be coerced in <seealso cref="#getValue(ELContext)"/>. </param>
		public ObjectValueExpression(ITypeConverter converter, object @object, Type type) : base()
		{

			this.converter = converter;
			this.@object = @object;
			this.type = type;

			if (type == null)
			{
				throw new System.NullReferenceException("error.value.notype");
			}
		}

		/// <summary>
		/// Two object value expressions are equal if and only if their wrapped objects are equal.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == this.GetType())
			{
				ObjectValueExpression other = (ObjectValueExpression)obj;
				if (type != other.type)
				{
					return false;
				}
				return @object == other.@object || @object != null && @object.Equals(other.@object);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return @object == null ? 0 : @object.GetHashCode();
		}

		/// <summary>
		/// Answer the wrapped object, coerced to the expected type.
		/// </summary>
		public override object GetValue(ELContext context)
		{
			return converter.Convert<object>(@object, type);
		}

		/// <summary>
		/// Answer <code>null</code>.
		/// </summary>
		public override string ExpressionString
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Answer <code>false</code>.
		/// </summary>
		public override bool IsLiteralText
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Answer <code>null</code>.
		/// </summary>
		public override Type GetType(ELContext context)
		{
			return null;
		}

		/// <summary>
		/// Answer <code>true</code>.
		/// </summary>
		public override bool IsReadOnly(ELContext context)
		{
			return true;
		}

		/// <summary>
		/// Throw an exception.
		/// </summary>
		public override void SetValue(ELContext context, object value)
		{
			throw new ELException("error.value.set.rvalue<object value expression>");
		}

		public override string ToString()
		{
			return "ValueExpression(" + @object + ")";
		}

		public override Type ExpectedType
		{
			get
			{
				return type;
			}
		}
	}

}