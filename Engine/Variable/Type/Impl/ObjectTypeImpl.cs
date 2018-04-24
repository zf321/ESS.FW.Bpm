using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Builder;

namespace ESS.FW.Bpm.Engine.Variable.Type.Impl
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ObjectTypeImpl : AbstractValueTypeImpl, ISerializableValueType
    {
        private const long SerialVersionUid = 1L;

        public const string TypeName = "object";

        public ObjectTypeImpl() : base(TypeName)
        {
        }

        public override bool IsPrimitiveValueType
        {
            get { return false; }
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            var builder = Variables.ObjectValue(value);

            if (valueInfo != null)
                ApplyValueInfo(builder, valueInfo);

            return builder.Create();
        }

        public override IDictionary<string, object> GetValueInfo(ITypedValue typedValue)
        {
            if (!(typedValue is IObjectValue))
                throw new ArgumentException("Value not of type Object.");
            var objectValue = (IObjectValue) typedValue;

            IDictionary<string, object> valueInfo = new Dictionary<string, object>();

            var serializationDataFormat = objectValue.SerializationDataFormat;
            if (!ReferenceEquals(serializationDataFormat, null))
                valueInfo[SerializableValueTypeFields.ValueInfoSerializationDataFormat] = serializationDataFormat;

            var objectTypeName = objectValue.ObjectTypeName;
            if (!ReferenceEquals(objectTypeName, null))
                valueInfo[SerializableValueTypeFields.ValueInfoObjectTypeName] = objectTypeName;

            return valueInfo;
        }

        public virtual ISerializableValue CreateValueFromSerialized(string serializedValue,
            IDictionary<string, object> valueInfo)
        {
            var builder = Variables.SerializedObjectValue(serializedValue);

            if (valueInfo != null)
                ApplyValueInfo(builder, valueInfo);

            return builder.Create();
        }

        protected internal virtual void ApplyValueInfo(IObjectValueBuilder builder,
            IDictionary<string, object> valueInfo)
        {
            var objectValueTypeName = (string) valueInfo[SerializableValueTypeFields.ValueInfoObjectTypeName];
            if (builder is ISerializedObjectValueBuilder)
                ((ISerializedObjectValueBuilder) builder).ObjectTypeName(objectValueTypeName);

            var serializationDataFormat =
                (string) valueInfo[SerializableValueTypeFields.ValueInfoSerializationDataFormat];
            if (!ReferenceEquals(serializationDataFormat, null))
                builder.SerializationDataFormat(serializationDataFormat);
        }
    }
}