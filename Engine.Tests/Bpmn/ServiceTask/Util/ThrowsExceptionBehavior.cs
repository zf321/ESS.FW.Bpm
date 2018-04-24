using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    public class ThrowsExceptionBehavior : IActivityBehavior // ActivityBehavior
    {
        public void Execute(IActivityExecution execution)
        {
            string @var = (string)execution.GetVariable("var");

            IPvmTransition transition = null;
            try
            {
                ExecuteLogic(@var);
                transition = execution.Activity.FindOutgoingTransition("no-exception");
            }
            catch (System.Exception)
            {
                transition = execution.Activity.FindOutgoingTransition("exception");
            }
            execution.LeaveActivityViaTransition(transition);
        }

        protected internal virtual void ExecuteLogic(string value)
        {
            if (value.Equals("throw-exception"))
            {
                throw new System.Exception();
            }
        }

    }

}