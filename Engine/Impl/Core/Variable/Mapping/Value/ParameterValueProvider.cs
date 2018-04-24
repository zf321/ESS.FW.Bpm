using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value
{
    /// <summary>
    ///     
    /// </summary>
    public interface IParameterValueProvider
    {
        /// <param name="variableScope"> the scope in which the value is to be resolved. </param>
        /// <returns> the value </returns>
        object GetValue(IVariableScope variableScope);
    }
}