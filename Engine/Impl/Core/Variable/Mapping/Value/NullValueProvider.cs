using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value
{
    /// <summary>
    ///     
    /// </summary>
    public class NullValueProvider : IParameterValueProvider
    {
        public virtual object GetValue(IVariableScope variableScope)
        {
            return null;
        }
    }
}