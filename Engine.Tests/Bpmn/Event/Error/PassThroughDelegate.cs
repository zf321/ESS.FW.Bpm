using System;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{

    [Serializable]
    public class PassThroughDelegate : AbstractBpmnActivityBehavior
    {

        public const long serialVersionUID = 1L;

        public override void Execute(IActivityExecution execution)
        {
            base.Execute(execution);
        }

        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            base.Leave(execution);            
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
        //public override void Execute(ActivityExecution execution)
        //{
        //    base.execute(execution);
        //}

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void signal(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
        //public override void signal(ActivityExecution execution, string signalName, object signalData)
        //{
        //    base.leave(execution);
        //}


    }

}