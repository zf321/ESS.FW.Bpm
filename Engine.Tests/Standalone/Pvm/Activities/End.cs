using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class End : IActivityBehavior
    {
        public virtual void Execute(IActivityExecution execution)
        {
            execution.End(true);
        }

    }

}
