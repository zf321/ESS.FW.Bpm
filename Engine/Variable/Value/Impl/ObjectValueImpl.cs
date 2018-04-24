using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ObjectValueImpl : AbstractTypedValue<object>, IObjectValue
    {
        private const long SerialVersionUid = 1L;
        

        protected internal string serializedValue;

        public ObjectValueImpl(object deserializedValue, string serializedValue, string serializationDataFormat,
            string objectTypeName, bool isDeserialized) : base(deserializedValue, ValueTypeFields.Object)
        {
            this.serializedValue = serializedValue;
            this.SerializationDataFormat = serializationDataFormat;
            this.ObjectTypeName = objectTypeName;
            IsDeserialized = isDeserialized;
        }

        public ObjectValueImpl(object value) : this(value, null, null, null, true)
        {
        }

        public virtual string SerializedValue
        {
            set { serializedValue = value; }
        }

        public virtual string SerializationDataFormat { get; set; }


        public virtual string ObjectTypeName { get; set; }


        public virtual string ValueSerialized
        {
            get { return serializedValue; }
        }

        public virtual bool IsDeserialized { get; set; }

        public override object Value
        {
            get
            {
                if (IsDeserialized)
                    return base.Value;
                throw new InvalidOperationException("Object is not deserialized.");
            }
        }

        public object GetValue(System.Type type)
        {
            return value;
        }

        public virtual System.Type ObjectType
        {
            get
            {
                var value = Value;

                if (value == null)
                    return null;
                return value.GetType();
            }
        }

        public override IValueType Type
        {
            get {
                return (ISerializableValueType) base.Type;
            }
        }

        public virtual T GetValue<T>(System.Type type)
        {
            var value = Value;
            if (type.IsAssignableFrom(value.GetType()))
                return (T) value;
            throw new ArgumentException("Value '" + value + "' is not of type '" + type + "'.");
        }

        public override string ToString()
        {
            return "ObjectValue [" + "value=" + value + ", isDeserialized=" + IsDeserialized +
                   ", serializationDataFormat=" + SerializationDataFormat + ", objectTypeName=" + ObjectTypeName +
                   ", serializedValue=" +
                   (!ReferenceEquals(serializedValue, null) ? serializedValue.Length + " chars" : null) + "]";
        }
    }
}