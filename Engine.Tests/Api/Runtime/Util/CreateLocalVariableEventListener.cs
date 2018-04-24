using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    public class CreateLocalVariableEventListener : ITaskListener
    {
        public virtual void Notify(IDelegateTask delegateTask)
        {
            delegateTask.SetVariableLocal("var", "foo");
        }
    }
}