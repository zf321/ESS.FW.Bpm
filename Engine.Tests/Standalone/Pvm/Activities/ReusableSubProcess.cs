using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class ReusableSubProcess : ISubProcessActivityBehavior
    {
        private readonly IPvmProcessDefinition _processDefinition;

        public ReusableSubProcess(IPvmProcessDefinition processDefinition)
        {
            _processDefinition = processDefinition;
        }

        public  void  Execute(IActivityExecution execution)
        {
            var subProcessInstance = execution.CreateSubProcessInstance(_processDefinition);

            subProcessInstance.Start();
        }

        public void PassOutputVariables(IActivityExecution targetExecution, IVariableScope calledElementInstance)
        {
        }

        public void Completed(IActivityExecution execution)
        {
            var outgoingTransitions = (List<IPvmTransition>) execution.Activity.OutgoingTransitions;
            execution.LeaveActivityViaTransitions(outgoingTransitions, null);
        }
    }
}