using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ExecuteCommandDelegate : MyDelegationService, IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            logAuthentication(execution as IDelegateExecution);
            executeCommand(execution as IDelegateExecution);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    logAuthentication(execution);
        //    executeCommand(execution);
        //}

    }

}