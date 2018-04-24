using System;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
	/// <summary>
	/// Type Conversions as described in EL 2.1 specification (section 1.17).
	/// </summary>
	[Serializable]
	public class TypeConverterImpl : ITypeConverter
	{
	    protected internal virtual bool? CoerceToBoolean(object value)
		{
			if (value == null || "".Equals(value))
			{
				return false;
			}
			if (value is bool?)
			{
				return (bool?)value;
			}
			if (value is string s)
			{
				return System.Convert.ToBoolean(s);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(Boolean)));
		}

		protected internal virtual char? CoerceToCharacter(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToChar((char)0);
			}
			if (value is char?)
			{
				return (char?)value;
			}
			if (value is decimal c)
			{
				return System.Convert.ToChar((char)c);
			}
			if (value is string s)
			{
				return System.Convert.ToChar(s[0]);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(char)));
		}

		protected internal virtual decimal CoerceToBigDecimal(object value)
		{
			if (value == null || "".Equals(value))
			{
				return (0L);
			}
			if (value is decimal @decimal)
			{
				return @decimal;
			}
			if (value is long l)
			{
				return new decimal(l);
			}

		    if (value is string)
			{
				try
				{
					return decimal.Parse(value.ToString());
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", value, typeof(decimal)));
				}
			}
			if (value is char?)
			{
				return new decimal((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(decimal)));
		}

		protected internal virtual Int64 CoerceToBigInteger(object value)
		{
			if (value == null || "".Equals(value))
			{
				return (0L);
			}
			if (value is long l)
			{
				return l;
			}
			if (value is decimal)
			{
				return System.Convert.ToInt64(value);
			}

		    if (value is string s)
			{
				try
				{
					return long.Parse(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(Int64)));
				}
			}
			if (value is char?)
			{
				return ((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(Int64)));
		}

		protected internal virtual double? CoerceToDouble(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToDouble(0);
			}
			if (value is double?)
			{
				return (double?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToDouble(@decimal);
			}
			if (value is string s)
			{
				try
				{
					return System.Convert.ToDouble(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(Double)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToDouble((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(Double)));
		}

		protected internal virtual float? CoerceToFloat(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToSingle(0);
			}
			if (value is float?)
			{
				return (float?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToSingle(@decimal);
			}
			if (value is string s)
			{
				try
				{
					return System.Convert.ToSingle(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(float)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToSingle((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(float)));
		}

		protected internal virtual long? CoerceToLong(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToInt64(0L);
			}
			if (value is long?)
			{
				return (long?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToInt64(@decimal);
			}
		    if (value is int i)
		    {
		        return System.Convert.ToInt64(i);
		    }
		    if (value is string s)
			{
				try
				{
					return System.Convert.ToInt64(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(long)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToInt64((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(long)));
		}

		protected internal virtual int? CoerceToInteger(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToInt32(0);
			}
			if (value is int?)
			{
				return (int?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToInt32(@decimal);
			}
			if (value is string s)
			{
				try
				{
					return System.Convert.ToInt32(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(int)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToInt32((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(int)));
		}

		protected internal virtual short? CoerceToShort(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToInt16((short)0);
			}
			if (value is short?)
			{
				return (short?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToInt16(@decimal);
			}
			if (value is string s)
			{
				try
				{
					return System.Convert.ToInt16(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(short)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToInt16((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(short)));
		}

		protected internal virtual sbyte? CoerceToByte(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Convert.ToSByte((sbyte)0);
			}
			if (value is sbyte?)
			{
				return (sbyte?)value;
			}
			if (value is decimal @decimal)
			{
				return System.Convert.ToSByte(@decimal);
			}
			if (value is string s)
			{
				try
				{
					return System.Convert.ToSByte(s);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", s, typeof(Byte)));
				}
			}
			if (value is char?)
			{
				return System.Convert.ToSByte(System.Convert.ToInt16((short)((char?)value).Value));
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), typeof(Byte)));
		}

		protected internal virtual string CoerceToString(object value)
		{
			if (value == null)
			{
				return "";
			}
			if (value is string s)
			{
				return s;
			}
			if (value is Enum)
			{
				return value.ToString();
			}
			return value.ToString();
		}
        
		protected internal virtual T CoerceToEnum<T>(object value, Type type)
		{
			if (value == null || "".Equals(value))
			{
				return default(T);
			}
			if (type.IsInstanceOfType(value))
			{
				return (T)value;
			}
			if (value is string)
			{
				try
				{
					return (T) value;
				}
				catch (System.ArgumentException)
				{
					throw new ELException(LocalMessages.Get("error.coerce.value {0}  {1}", value, type));
				}
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), type));
		}

		protected internal virtual object CoerceStringToType(string value, Type type)
		{
            throw new NotImplementedException();
			//PropertyEditor editor = PropertyEditorManager.FindEditor(type);
			//if (editor == null)
			//{
			//	if ("".Equals(value))
			//	{
			//		return null;
			//	}
			//	throw new ELException(LocalMessages.Get("error.coerce.type", typeof(string), type));
			//}
			//else
			//{
			//	if ("".Equals(value))
			//	{
			//		try
			//		{
			//			editor.AsText = value;
			//		}
			//		catch (System.ArgumentException)
			//		{
			//			return null;
			//		}
			//	}
			//	else
			//	{
			//		try
			//		{
			//			editor.AsText = value;
			//		}
			//		catch (System.ArgumentException)
			//		{
			//			throw new ELException(LocalMessages.Get("error.coerce.value", value, type));
			//		}
			//	}
			//	return editor.Value;
			//}
		    return null;
		}
        
		protected internal virtual object CoerceToType(object value, Type type)
		{
			if (type == typeof(string))
			{
				return CoerceToString(value);
			}
			if (type == typeof(long) || type == typeof(long))
			{
				return CoerceToLong(value);
			}
			if (type == typeof(Double) || type == typeof(double))
			{
				return CoerceToDouble(value);
			}
			if (type == typeof(Boolean) || type == typeof(bool))
			{
				return CoerceToBoolean(value);
			}
			if (type == typeof(int) || type == typeof(int))
			{
				return CoerceToInteger(value);
			}
			if (type == typeof(float) || type == typeof(float))
			{
				return CoerceToFloat(value);
			}
			if (type == typeof(short) || type == typeof(short))
			{
				return CoerceToShort(value);
			}
			if (type == typeof(Byte) || type == typeof(sbyte))
			{
				return CoerceToByte(value);
			}
			if (type == typeof(char) || type == typeof(char))
			{
				return CoerceToCharacter(value);
			}
			if (type == typeof(decimal))
			{
				return CoerceToBigDecimal(value);
			}
			if (type == typeof(Int64))
			{
				return CoerceToBigInteger(value);
			}
			if (type.BaseType == typeof(Enum))
			{
				return CoerceToEnum<object>(value, type);
			}
			if (value == null || value.GetType() == type || type.IsInstanceOfType(value))
			{
				return value;
			}
			if (value is string)
			{
				return CoerceStringToType((string)value, type);
			}
			throw new ELException(LocalMessages.Get("error.coerce.type {0}  {1}", value.GetType(), type));
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == this.GetType();
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

        public virtual T Convert<T>(object value, Type type)
        {
            return (T)CoerceToType(value, type);
        }
    }

}