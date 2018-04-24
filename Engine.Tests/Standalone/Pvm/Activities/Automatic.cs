using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class Automatic : IActivityBehavior
    {

        public void Execute(IActivityExecution execution)
        {
            IList<IPvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;
            if (!outgoingTransitions.Any())
            {
                execution.End(true);
            }
            else
            {
                execution.LeaveActivityViaTransition(outgoingTransitions[0]);
            }
        }
    }
}
