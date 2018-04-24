using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.EL
{
    [Serializable]
    public class ExecutionTestVariable
    {
        private const long serialVersionUID = 1L;

        public virtual void TestMethod(IDelegateExecution delegateExecution)
        {
            delegateExecution.SetVariable("testVar", "myValue");
        }
    }
}