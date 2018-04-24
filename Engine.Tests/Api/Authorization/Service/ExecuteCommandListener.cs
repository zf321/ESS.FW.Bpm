using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{
    
    public class ExecuteCommandListener : MyDelegationService, IDelegateListener<IBaseDelegateExecution>
    {
        public void Notify(IBaseDelegateExecution execution)
        {
            logAuthentication(execution as IDelegateExecution);
            executeCommand(execution as IDelegateExecution);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void Notify(org.camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void Notify(IDelegateExecution execution)
        //{
        //    logAuthentication(execution);
        //    executeCommand(execution);
        //}

    }

}