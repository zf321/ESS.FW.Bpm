using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmReusableSubProcessTest
    {
        [Test]
        public virtual void TestReusableSubProcess()
        {
            IPvmProcessDefinition subProcessDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("subEnd")
                    .EndActivity()
                    .CreateActivity("subEnd")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessDefinition superProcessDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("subprocess")
                    .EndActivity()
                    .CreateActivity("subprocess")
                    .Behavior(new ReusableSubProcess(subProcessDefinition))
                    .Transition("superEnd")
                    .EndActivity()
                    .CreateActivity("superEnd")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = superProcessDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }
    }
}