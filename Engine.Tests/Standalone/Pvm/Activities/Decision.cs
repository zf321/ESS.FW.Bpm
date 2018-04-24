using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class Decision : IActivityBehavior
    {
        public void Execute(IActivityExecution execution)
        {
            IPvmTransition transition = null;
            var creditRating = (string) execution.GetVariable("creditRating");
            if (creditRating.Equals("AAA+")) transition = execution.Activity.FindOutgoingTransition("wow");
            else if (creditRating.Equals("Aaa-")) transition = execution.Activity.FindOutgoingTransition("nice");
            else transition = execution.Activity.FindOutgoingTransition("default");

            execution.LeaveActivityViaTransition(transition);
        }
    }
}