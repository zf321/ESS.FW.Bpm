using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    /// </summary>
    public class VariableUpdateExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        private IExpression _varName;

        public virtual void Notify(IBaseDelegateExecution execution)
        {
            var variableName = (string) _varName.GetValue(execution);
            execution.SetVariable(variableName, "Event: " + execution.EventName);
        }
    }
}