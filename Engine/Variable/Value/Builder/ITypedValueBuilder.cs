namespace ESS.FW.Bpm.Engine.Variable.Value.Builder
{
    /// <summary>
    /// </summary>
    public interface ITypedValueBuilder<T> //where T : ITypedValue
    {
        T Create();
    }
}