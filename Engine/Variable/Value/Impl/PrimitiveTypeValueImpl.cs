using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class PrimitiveTypeValueImpl<T> : AbstractTypedValue<T>
    {
        private const long SerialVersionUid = 1L;

        public PrimitiveTypeValueImpl(T value, IPrimitiveValueType type) : base(value, type)
        {
        }

        public override object Value
        {
            get { return value; }
        }


        public override IValueType Type
        {
            get { return (IPrimitiveValueType)base.Type; }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + (type == null ? 0 : type.GetHashCode());
            result = prime * result + (value == null ? 0 : value.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            if (obj is ITypedValue)
            {
                //TODO 值类型
                var o = (ITypedValue)obj;
                if (o.Type.Name == this.Type.Name && o.Value.ToString() == this.Value.ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (obj == this.Value)
                {
                    return true;
                }
            }
            var other = (PrimitiveTypeValueImpl<object>)obj;
            if (type == null)
            {
                if (other.type != null)
                    return false;
            }
            else if (!type.Equals(other.type))
            {
                return false;
            }
            if (value == null)
            {
                if (other.value != null)
                    return false;
            }
            else if (!value.Equals(other.value))
            {
                return false;
            }
            return true;
        }
    }

    // value type implemenations ////////////////////////////////////

    [Serializable]
    public class BooleanValueImpl : PrimitiveTypeValueImpl<bool?>, IBooleanValue
    {
        internal const long SerialVersionUid = 1L;

        public BooleanValueImpl(bool? value) : base(value, ValueTypeFields.Boolean)
        {
        }
    }

    [Serializable]
    public class BytesValueImpl : PrimitiveTypeValueImpl<byte[]>, IBytesValue
    {
        internal const long SerialVersionUid = 1L;

        public BytesValueImpl(byte[] value) : base(value, ValueTypeFields.Bytes)
        {
        }
    }

    [Serializable]
    public class DateValueImpl : PrimitiveTypeValueImpl<DateTime>, IDateValue
    {
        internal const long SerialVersionUid = 1L;

        public DateValueImpl(DateTime value) : base(value, ValueTypeFields.Date)
        {
        }
    }

    [Serializable]
    public class DoubleValueImpl : PrimitiveTypeValueImpl<double?>, IDoubleValue
    {
        internal const long SerialVersionUid = 1L;

        public DoubleValueImpl(double? value) : base(value, ValueTypeFields.Double)
        {
        }
    }

    [Serializable]
    public class IntegerValueImpl : PrimitiveTypeValueImpl<int?>, IIntegerValue
    {
        internal const long SerialVersionUid = 1L;

        public IntegerValueImpl(int? value) : base(value, ValueTypeFields.Integer)
        {
        }
    }

    [Serializable]
    public class LongValueImpl : PrimitiveTypeValueImpl<long?>, ILongValue
    {
        internal const long SerialVersionUid = 1L;

        public LongValueImpl(long? value) : base(value, ValueTypeFields.Long)
        {
        }
    }

    [Serializable]
    public class ShortValueImpl : PrimitiveTypeValueImpl<short?>, IShortValue
    {
        internal const long SerialVersionUid = 1L;

        public ShortValueImpl(short? value) : base(value, ValueTypeFields.Short)
        {
        }
    }

    [Serializable]
    public class StringValueImpl : PrimitiveTypeValueImpl<string>, IStringValue
    {
        internal const long SerialVersionUid = 1L;

        public StringValueImpl(string value) : base(value, ValueTypeFields.String)
        {
        }
        public static explicit operator string(StringValueImpl val)
        {
            return val.value;
        }
        static public implicit operator StringValueImpl(string value)
        {
            return new StringValueImpl(value);
        }
    }

    [Serializable]
    public class NumberValueImpl : PrimitiveTypeValueImpl<decimal>, INumberValue
    {
        internal const long SerialVersionUid = 1L;

        public NumberValueImpl(decimal value) : base(value, ValueTypeFields.Number)
        {
        }
    }
}