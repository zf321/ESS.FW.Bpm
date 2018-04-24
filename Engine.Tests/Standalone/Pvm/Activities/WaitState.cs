using System;
using System.Diagnostics;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class WaitState : ISignallableActivityBehavior
    {

        public void Execute(IActivityExecution execution)
        {
            Debug.WriteLine("some debug point");

        }

        public void Signal(IActivityExecution execution, String signalName, Object signalData)
        {
            IPvmTransition transition = execution.Activity.OutgoingTransitions[0];
            execution.LeaveActivityViaTransition(transition);

        }
        
    }
}
