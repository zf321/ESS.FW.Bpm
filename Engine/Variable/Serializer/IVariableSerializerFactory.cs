using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Serializer
{
    

    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IVariableSerializerFactory
    {
        
        ITypedValueSerializer GetSerializer<T>(string serializerName) where T:ITypedValue;
        
        ITypedValueSerializer GetSerializer<T>(ITypedValue value) where T:ITypedValue;
    }
}
