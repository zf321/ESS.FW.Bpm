using System;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

    

	/// <summary>
	/// Arithmetic Operations as specified in chapter 1.7.
	/// 
	/// 
	/// </summary>
	public class NumberOperations
    {
		private static readonly long LONG_ZERO = 0L;

		private static bool IsDotEe(string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				switch (value[i])
				{
					case '.':
					case 'E':
					case 'e':
						return true;
				}
			}
			return false;
		}

		private static bool IsDotEe(object value)
		{
			return value is string && IsDotEe((string)value);
		}

		private static bool IsFloatOrDouble(object value)
		{
			return value is float? || value is double?;
		}

		private static bool IsFloatOrDoubleOrDotEe(object value)
		{
			return IsFloatOrDouble(value) || IsDotEe(value);
		}

		private static bool IsBigDecimalOrBigInteger(object value)
		{
		    return value is decimal || value is Int64;
		}

		private static bool IsBigDecimalOrFloatOrDoubleOrDotEe(object value)
		{
			return value is decimal || IsFloatOrDoubleOrDotEe(value);
		}

		public static decimal Add(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return (converter.Convert<decimal>(o1, typeof(decimal)))+(converter.Convert<decimal>(o2, typeof(decimal)));
			}
			if (IsFloatOrDoubleOrDotEe(o1) || IsFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is Int64 || o2 is Int64)
				{
					return (converter.Convert<decimal>(o1, typeof(decimal)))+(converter.Convert<decimal>(o2, typeof(decimal)));
				}
				return (decimal) (converter.Convert<double>(o1, typeof(double)) + converter.Convert<double>(o2, typeof(double)));
			}
			if (o1 is Int64 || o2 is Int64)
			{
				return converter.Convert<Int64>(o1, typeof(Int64))+(converter.Convert<Int64>(o2, typeof(Int64)));
			}
			return converter.Convert<long>(o1, typeof(long)) + converter.Convert<long>(o2, typeof(long));
		}

		public static decimal Sub(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return converter.Convert<decimal>(o1, typeof(decimal))-(converter.Convert<decimal>(o2, typeof(decimal)));
			}
			if (IsFloatOrDoubleOrDotEe(o1) || IsFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is Int64 || o2 is Int64)
				{
					return converter.Convert<decimal>(o1, typeof(decimal))-(converter.Convert<decimal>(o2, typeof(decimal)));
				}
				return (decimal) (converter.Convert<Double>(o1, typeof(Double)) - converter.Convert<Double>(o2, typeof(Double)));
			}
			if (o1 is Int64 || o2 is Int64)
			{
				return converter.Convert<Int64>(o1, typeof(Int64))-(converter.Convert<Int64>(o2, typeof(Int64)));
			}
			return converter.Convert<long>(o1, typeof(long)) - converter.Convert<long>(o2, typeof(long));
		}

		public static decimal Mul(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return converter.Convert<decimal>(o1, typeof(decimal))*(converter.Convert<decimal>(o2, typeof(decimal)));
			}
			if (IsFloatOrDoubleOrDotEe(o1) || IsFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is Int64 || o2 is Int64)
				{
					return converter.Convert<decimal>(o1, typeof(decimal))*(converter.Convert<decimal>(o2, typeof(decimal)));
				}
				return (decimal) (converter.Convert<Double>(o1, typeof(Double)) * converter.Convert<Double>(o2, typeof(Double)));
			}
			if (o1 is Int64 || o2 is Int64)
			{
				return converter.Convert<Int64>(o1, typeof(Int64))*(converter.Convert<Int64>(o2, typeof(Int64)));
			}
			return converter.Convert<long>(o1, typeof(long)) * converter.Convert<long>(o2, typeof(long));
		}

		public static decimal Div(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (IsBigDecimalOrBigInteger(o1) || IsBigDecimalOrBigInteger(o2))
			{
				return converter.Convert<decimal>(o1, typeof(decimal))/(converter.Convert<decimal>(o2, typeof(decimal)));
			}
			return (decimal) (converter.Convert<Double>(o1, typeof(Double)) / converter.Convert<Double>(o2, typeof(Double)));
		}

		public static decimal Mod(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (IsBigDecimalOrFloatOrDoubleOrDotEe(o1) || IsBigDecimalOrFloatOrDoubleOrDotEe(o2))
			{
				return (decimal) (converter.Convert<Double>(o1, typeof(Double)) % converter.Convert<Double>(o2, typeof(Double)));
			}
			if (o1 is Int64 || o2 is Int64)
			{
				return converter.Convert<Int64>(o1, typeof(Int64))%(converter.Convert<Int64>(o2, typeof(Int64)));
			}
			return converter.Convert<long>(o1, typeof(long)) % converter.Convert<long>(o2, typeof(long));
		}

		public static decimal Neg(ITypeConverter converter, object value)
		{
			if (value == null)
			{
				return LONG_ZERO;
			}
			if (value is decimal)
			{
				return -((decimal)value);
			}
			if (value is Int64)
			{
				return -((Int64)value);
			}
			if (value is double?)
			{
				return (decimal) (-((double?)value).Value);
			}
			if (value is float?)
			{
				return (decimal)(-((float?)value).Value);
			}
			if (value is string)
			{
				if (IsDotEe((string)value))
				{
					return Convert.ToDecimal(-converter.Convert<Double>(value, typeof(Double)));
				}
				return Convert.ToInt64(-converter.Convert<Double>(value, typeof(long)));
			}
			if (value is long?)
			{
				return Convert.ToInt64(-((long?)value).Value);
			}
			if (value is int?)
			{
				return Convert.ToInt32(-((int?)value).Value);
			}
			if (value is short?)
			{
				return Convert.ToInt16((short)-((short?)value).Value);
			}
			if (value is sbyte?)
			{
				return Convert.ToSByte((sbyte)-((sbyte?)value).Value);
			}
            //throw new ELException(LocalMessages.Get("error.negate", value.GetType()));
            throw new ELException("error.negate");
        }
    }

}