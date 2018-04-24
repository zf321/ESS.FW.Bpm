using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    public class SetLocalVariableTask : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariableLocal("test", "test2");
        }
    }
}