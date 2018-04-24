using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public abstract class MyServiceTaskActivityBehavior : MyDelegationService, ISignallableActivityBehavior
    {
        public void Execute(IActivityExecution execution)
        {
            throw new NotImplementedException();
        }

        public void Signal(IActivityExecution execution, string signalEvent, object signalData)
        {
            logAuthentication(execution);
            logInstancesCount(execution);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void signal(org.camunda.bpm.Engine.impl.pvm.Delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
        //public virtual void signal(IActivityExecution execution, string signalEvent, object signalData)
        //{
        //    logAuthentication(execution);
        //    logInstancesCount(execution);
        //}

    }

}