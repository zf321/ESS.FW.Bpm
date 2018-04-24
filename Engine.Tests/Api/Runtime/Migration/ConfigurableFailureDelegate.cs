using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    public class ConfigurableFailureDelegate : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public void Execute(IBaseDelegateExecution execution)
        {
            var shouldFail = (bool) execution.GetVariable("shouldFail");

            if (shouldFail != null && shouldFail)
                throw new System.Exception("I Assert.Fail as commanded");
        }
    }
}