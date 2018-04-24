namespace ESS.FW.Bpm.Engine.Variable.Value.Builder
{
    /// <summary>
    /// </summary>
    public interface ISerializedObjectValueBuilder : IObjectValueBuilder
    {
        ISerializedObjectValueBuilder SerializedValue(string value);

        ISerializedObjectValueBuilder ObjectTypeName(string typeName);
    }
}