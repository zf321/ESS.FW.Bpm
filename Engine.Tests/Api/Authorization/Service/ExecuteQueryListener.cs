
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ExecuteQueryListener : MyDelegationService, IDelegateListener<IBaseDelegateExecution>
    {

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void Notify(org.camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            logAuthentication(execution as IDelegateExecution);
            logInstancesCount(execution as IDelegateExecution);
        }

    }

}