using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public interface IExpression :Bpm.Engine.Delegate.IExpression
    {
        object GetValue(IVariableScope variableScope, IBaseDelegateExecution contextExecution);

        void SetValue(object value, IVariableScope variableScope, IBaseDelegateExecution contextExecution);
    }
}