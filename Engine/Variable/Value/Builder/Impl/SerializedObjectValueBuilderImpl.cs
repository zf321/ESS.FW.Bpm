using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Variable.Value.Builder.Impl
{
    /// <summary>
    /// </summary>
    public class SerializedObjectValueBuilderImpl : ISerializedObjectValueBuilder
    {
        protected internal ObjectValueImpl VariableValue;

        public SerializedObjectValueBuilderImpl()
        {
            VariableValue = new ObjectValueImpl(null, null, null, null, false);
        }

        public SerializedObjectValueBuilderImpl(IObjectValue value)
        {
            VariableValue = (ObjectValueImpl) value;
        }

        public virtual IObjectValueBuilder SerializationDataFormat(string dataFormatName)
        {
            VariableValue.SerializationDataFormat = dataFormatName;
            return this;
        }

        public virtual IObjectValueBuilder SerializationDataFormat(ISerializationDataFormat dataFormat)
        {
            return SerializationDataFormat(dataFormat.Name);
        }

        public virtual IObjectValue Create()
        {
            return VariableValue;
        }

        public virtual ISerializedObjectValueBuilder ObjectTypeName(string typeName)
        {
            VariableValue.ObjectTypeName = typeName;
            return this;
        }

        public virtual ISerializedObjectValueBuilder SerializedValue(string value)
        {
            VariableValue.SerializedValue = value;
            return this;
        }
    }
}