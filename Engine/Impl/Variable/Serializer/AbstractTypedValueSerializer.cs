using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public abstract class AbstractTypedValueSerializer<T> : ITypedValueSerializer where T : ITypedValue
    {
        public static readonly ISet<string> BINARY_VALUE_TYPES = new HashSet<string>();

        protected internal IValueType valueType;

        static AbstractTypedValueSerializer()
        {
            BINARY_VALUE_TYPES.Add(ValueTypeFields.Bytes.Name);
            BINARY_VALUE_TYPES.Add(ValueTypeFields.File.Name);
        }

        public AbstractTypedValueSerializer(IValueType type)
        {
            valueType = type;
        }

        public abstract ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue);
        public abstract ITypedValue ReadValue(IValueFields valueFields, bool deserializeValue);
        public abstract void WriteValue(ITypedValue value, IValueFields valueFields);
        public abstract string Name { get; }

        public virtual IValueType Type
        {
            get { return valueType; }
        }

        public virtual string SerializationDataFormat
        {
            get
            {
                // default implementation returns null
                return null;
            }
        }

        public virtual bool CanHandle(ITypedValue value)
        {
            if (value.Type != null && !valueType.GetType()
                    .IsAssignableFrom(value.Type.GetType()))
                return false;
            return CanWriteValue(value);
        }

        public virtual bool IsMutableValue(ITypedValue typedValue)
        {
            // default
            return false;
        }

        protected internal abstract bool CanWriteValue(ITypedValue value);
    }
}