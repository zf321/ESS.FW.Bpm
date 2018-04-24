
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class MyTaskService : MyDelegationService
    {

        public virtual string assignTask(IDelegateExecution execution)
        {
            logAuthentication(execution);
            logInstancesCount(execution);

            return "demo";
        }

    }

}