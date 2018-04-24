using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{
    public class ThrowBpmnErrorSignallableActivityBehaviour : AbstractBpmnActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
           
        }

        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            throw new BpmnError("23", "Testing bpmn error in SignallableActivityBehaviour#signal");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
        //public override void execute(ActivityExecution execution)
        //{
        //}

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void signal(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
        //public override void signal(ActivityExecution execution, string signalName, object signalData)
        //{
        //    throw new BpmnError("23", "Testing bpmn error in SignallableActivityBehaviour#signal");
        //}
    }

}