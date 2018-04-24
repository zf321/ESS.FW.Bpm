using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
	public class BooleanOperations
	{
		private static readonly ISet<Type> SIMPLE_INTEGER_TYPES = new HashSet<Type>();
		private static readonly ISet<Type> SIMPLE_FLOAT_TYPES = new HashSet<Type>();

		static BooleanOperations()
		{
			SIMPLE_INTEGER_TYPES.Add(typeof(byte));
			SIMPLE_INTEGER_TYPES.Add(typeof(short));
			SIMPLE_INTEGER_TYPES.Add(typeof(int));
			SIMPLE_INTEGER_TYPES.Add(typeof(long));
			SIMPLE_FLOAT_TYPES.Add(typeof(float));
			SIMPLE_FLOAT_TYPES.Add(typeof(double));
		}
        
		private static bool Lt0(ITypeConverter converter, object o1, object o2)
		{
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsSubclassOf(typeof(decimal)) || t2.IsSubclassOf(typeof(decimal)))
			{
				return converter.Convert<decimal>(o1, typeof(decimal)).CompareTo(converter.Convert<decimal>(o2, typeof(decimal))) < 0;
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.Convert<double>(o1, typeof(double)) < converter.Convert<double>(o2, typeof(double));
			}
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.Convert<long>(o1, typeof(long)) < converter.Convert<long>(o2, typeof(long));
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.Convert<string>(o1, typeof(string)).CompareTo(converter.Convert<string>(o2, typeof(string))) < 0;
			}
			if (o1 is IComparable)
			{
				return ((IComparable)o1).CompareTo(o2) < 0;
			}
			if (o2 is IComparable)
			{
				return ((IComparable)o2).CompareTo(o1) > 0;
			}
			throw new ELException(LocalMessages.Get("error.compare.types", o1.GetType(), o2.GetType()));
		}
        
		private static bool Gt0(ITypeConverter converter, object o1, object o2)
		{
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsSubclassOf(typeof(decimal)) || t2.IsSubclassOf(typeof(decimal)))
			{
				return converter.Convert<decimal>(o1, typeof(decimal)).CompareTo(converter.Convert<decimal>(o2, typeof(decimal))) > 0;
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.Convert<double>(o1, typeof(double)) > converter.Convert<double>(o2, typeof(double));
			}
			
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.Convert<long>(o1, typeof(long)) > converter.Convert<long>(o2, typeof(long));
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.Convert<string>(o1, typeof(string)).CompareTo(converter.Convert<string>(o2, typeof(string))) > 0;
			}
			if (o1 is IComparable)
			{
				return ((IComparable)o1).CompareTo(o2) > 0;
			}
			if (o2 is IComparable)
			{
				return ((IComparable)o2).CompareTo(o1) < 0;
			}
			throw new ELException(LocalMessages.Get("error.compare.types", o1.GetType(), o2.GetType()));
		}

		public static bool Lt(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return false;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return Lt0(converter, o1, o2);
		}

		public static bool Gt(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return false;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return Gt0(converter, o1, o2);
		}

		public static bool Ge(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return !Lt0(converter, o1, o2);
		}

		public static bool Le(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return !Gt0(converter, o1, o2);
		}

		public static bool Eq(ITypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsSubclassOf(typeof(decimal)) || t2.IsSubclassOf(typeof(decimal)))
			{
				return converter.Convert<decimal>(o1, typeof(decimal)).Equals(converter.Convert<decimal>(o2, typeof(decimal)));
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.Convert<double>(o1, typeof(double)).Equals(converter.Convert<double>(o2, typeof(double)));
			}
			
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.Convert<long>(o1, typeof(long)).Equals(converter.Convert<long>(o2, typeof(long)));
			}
			if (t1 == typeof(bool) || t2 == typeof(bool))
			{
				return converter.Convert<bool>(o1, typeof(bool)).Equals(converter.Convert<bool>(o2, typeof(bool)));
			}
			if (o1 is Enum)
			{
				return o1 == converter.Convert<Enum>(o2, o1.GetType());
			}
			if (o2 is Enum)
			{
				return converter.Convert<Enum>(o1, o2.GetType()) == o2;
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.Convert<string>(o1, typeof(string)).Equals(converter.Convert<string>(o2, typeof(string)));
			}
			return o1.Equals(o2);
		}

		public static bool Ne(ITypeConverter converter, object o1, object o2)
		{
			return !Eq(converter, o1, o2);
		}

		public static bool Empty(ITypeConverter converter, object o)
		{
			if (o == null || "".Equals(o))
			{
				return true;
			}
			if (o is object[])
			{
				return ((object[])o).Length == 0;
			}
			if (o is IDictionary)
			{
				return ((IDictionary)o).Count == 0;
			}
			if (o is ICollection)
			{
				return ((ICollection)o).Count == 0;
			}
			return false;
		}
	}

}