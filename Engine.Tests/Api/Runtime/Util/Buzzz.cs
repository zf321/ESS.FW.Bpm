using System;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class Buzzz : IActivityBehavior
    {
        public void Execute(IActivityExecution execution)
        {
            throw new NotImplementedException();
        }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.impl.pvm.Delegate.ActivityExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            throw new ProcessEngineException("Buzzz");
        }
    }
}