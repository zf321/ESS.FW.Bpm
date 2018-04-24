
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ExecuteQueryTaskListener : MyDelegationService, ITaskListener
    {

        public virtual void Notify(IDelegateTask delegateTask)
        {
            logAuthentication(delegateTask);
            logInstancesCount(delegateTask);
        }

    }

}