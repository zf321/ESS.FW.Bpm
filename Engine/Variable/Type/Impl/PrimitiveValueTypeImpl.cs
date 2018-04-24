using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Type.Impl
{
    /// <summary>
    ///     Implementation of the primitive variable value types
    ///     
    /// </summary>
    [Serializable]
    public abstract class PrimitiveValueTypeImpl : AbstractValueTypeImpl, IPrimitiveValueType
    {
        private const long SerialVersionUid = 1L;

        protected internal System.Type type;

        public PrimitiveValueTypeImpl(System.Type type) : this(type.Name.ToLower(), type)
        {
        }

        public PrimitiveValueTypeImpl(string name, System.Type type) : base(name)
        {
            this.type = type;
        }

        public abstract override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo);

        public virtual System.Type NetType
        {
            get { return type; }
        }

        public override bool IsPrimitiveValueType
        {
            get { return true; }
        }

        public override IDictionary<string, object> GetValueInfo(ITypedValue typedValue)
        {
            return new ConcurrentDictionary<string, object>();
        }

        public override string ToString()
        {
            return "PrimitiveValueType[" + Name + "]";
        }
        // concrete types ///////////////////////////////////////////////////
    }

    [Serializable]
    public class BooleanTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public BooleanTypeImpl() : base(typeof(bool))
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.BooleanValue((bool?) value);
        }
    }

    [Serializable]
    public class BytesTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public BytesTypeImpl() : base("bytes", typeof(byte[]))
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.ByteArrayValue((byte[]) value);
        }
    }

    [Serializable]
    public class DateTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public DateTypeImpl() : base(typeof(DateTime))
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.DateValue((DateTime) value);
        }
    }

    [Serializable]
    public class DoubleTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public DoubleTypeImpl() : base(typeof(double))
        {
        }

        public override IValueType Parent
        {
            get { return ValueTypeFields.Number; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.DoubleValue((double?) value);
        }

        public override bool CanConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                return false;

            return true;
        }

        public override ITypedValue ConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                throw UnsupportedConversion(typedValue.Type);

            var numberValue = (INumberValue) typedValue;
            if (numberValue.Value != null)
                return Variables.DoubleValue((double?) numberValue.Value);
            return Variables.DoubleValue(null);
        }
    }

    [Serializable]
    public class IntegerTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public IntegerTypeImpl() : base(typeof(int))
        {
        }

        public override IValueType Parent
        {
            get { return ValueTypeFields.Number; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.IntegerValue((int?) value);
        }

        public override bool CanConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                return false;

            if (typedValue.Value != null)
            {
                var numberValue = (INumberValue) typedValue;
                var doubleValue = (double) numberValue.Value;

                // returns false if the value changes due to conversion (e.g. by overflows
                // or by loss in precision)
                if ((int)numberValue.Value != (decimal) doubleValue)
                    return false;
            }

            return true;
        }

        public override ITypedValue ConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                throw UnsupportedConversion(typedValue.Type);

            var numberValue = (INumberValue) typedValue;
            if (numberValue.Value != null)
                return Variables.IntegerValue((int?) numberValue.Value);
            return Variables.IntegerValue(null);
        }
    }

    [Serializable]
    public class LongTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public LongTypeImpl() : base(typeof(long))
        {
        }

        public override IValueType Parent
        {
            get { return ValueTypeFields.Number; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.LongValue((long?) value);
        }

        public override bool CanConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                return false;

            if (typedValue.Value != null)
            {
                var numberValue = (INumberValue) typedValue;
                var doubleValue = numberValue.Value;

                // returns false if the value changes due to conversion (e.g. by overflows
                // or by loss in precision)
                if (numberValue.Value != doubleValue)
                    return false;
            }

            return true;
        }

        public override ITypedValue ConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                throw UnsupportedConversion(typedValue.Type);

            var numberValue = (INumberValue) typedValue;

            if (numberValue.Value != null)
                return Variables.LongValue((long?) numberValue.Value);
            return Variables.LongValue(null);
        }
    }

    [Serializable]
    public class NullTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public NullTypeImpl() : base("null", null)
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.UntypedNullValue();
        }
    }

    [Serializable]
    public class ShortTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public ShortTypeImpl() : base(typeof(short))
        {
        }

        public override IValueType Parent
        {
            get { return ValueTypeFields.Number; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.ShortValue((short?) value);
        }

        public override ITypedValue ConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                throw UnsupportedConversion(typedValue.Type);

            var numberValue = (INumberValue) typedValue;
            if (numberValue.Value != null)
                return Variables.ShortValue((short?) numberValue.Value);
            return Variables.ShortValue(null);
        }

        public override bool CanConvertFromTypedValue(ITypedValue typedValue)
        {
            if (typedValue.Type != ValueTypeFields.Number)
                return false;

            if (typedValue.Value != null)
            {
                var numberValue = (INumberValue) typedValue;
                var doubleValue = (double) numberValue.Value;

                // returns false if the value changes due to conversion (e.g. by overflows
                // or by loss in precision)
                if ((short)numberValue.Value != (decimal) doubleValue)
                    return false;
            }

            return true;
        }
    }

    [Serializable]
    public class StringTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public StringTypeImpl() : base(typeof(string))
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.StringValue((string) value);
        }
    }

    [Serializable]
    public class NumberTypeImpl : PrimitiveValueTypeImpl
    {
        internal const long SerialVersionUid = 1L;

        public NumberTypeImpl() : base(typeof(decimal))
        {
        }

        public override bool IsAbstract
        {
            get { return true; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            return Variables.NumberValue((decimal) value);
        }
    }
}
