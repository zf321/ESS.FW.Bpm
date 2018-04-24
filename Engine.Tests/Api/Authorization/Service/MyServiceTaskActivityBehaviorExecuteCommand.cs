
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class MyServiceTaskActivityBehaviorExecuteCommand : MyServiceTaskActivityBehavior
    {

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.camunda.bpm.Engine.impl.pvm.Delegate.ActivityExecution execution) throws Exception
        public virtual void execute(IActivityExecution execution)
        {
            logAuthentication(execution);
            executeCommand(execution);
        }

    }

}