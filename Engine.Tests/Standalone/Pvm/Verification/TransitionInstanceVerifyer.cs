using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm.Verification
{
    public class TransitionInstanceVerifyer : IDelegateListener<IBaseDelegateExecution>
    {
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            Assert.Null(((IDelegateExecution) execution).ActivityInstanceId);
        }
    }
}