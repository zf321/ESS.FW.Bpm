namespace ESS.FW.Bpm.Engine.Variable.Value.Builder
{
    /// <summary>
    /// </summary>
    public interface IObjectValueBuilder : ITypedValueBuilder<IObjectValue>
    {
        IObjectValueBuilder SerializationDataFormat(string dataFormatName);

        IObjectValueBuilder SerializationDataFormat(ISerializationDataFormat dataFormat);
    }
}