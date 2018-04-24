using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Variable.Value.Builder.Impl
{
    /// <summary>
    /// </summary>
    public class ObjectVariableBuilderImpl : IObjectValueBuilder
    {
        protected internal ObjectValueImpl VariableValue;

        public ObjectVariableBuilderImpl(object value)
        {
            VariableValue = new ObjectValueImpl(value);
        }

        public ObjectVariableBuilderImpl(IObjectValue value)
        {
            VariableValue = (ObjectValueImpl) value;
        }

        public virtual IObjectValue Create()
        {
            return VariableValue;
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
    }
}