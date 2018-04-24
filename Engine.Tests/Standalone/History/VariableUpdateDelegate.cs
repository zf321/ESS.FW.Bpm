using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    /// </summary>
    public class VariableUpdateDelegate : IJavaDelegate
    {
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariable("zzz", 123456789L);
        }
    }
}