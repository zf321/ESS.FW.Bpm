using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmTest
    {
        [Test]
        public void TestPvmWaitState()
        {
            IPvmProcessDefinition processDefinition = new ProcessDefinitionBuilder()
              .CreateActivity("a")
                .Initial()
                .Behavior(new WaitState())
                .Transition("b")
              .EndActivity()
              .CreateActivity("b")
                .Behavior(new WaitState())
                .Transition("c")
              .EndActivity()
              .CreateActivity("c")
                .Behavior(new WaitState())
              .EndActivity()
            .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IPvmExecution activityInstance = processInstance.FindExecution("a");
            Assert.NotNull(activityInstance);

            activityInstance.Signal(null, null);

            activityInstance = processInstance.FindExecution("b");
            Assert.NotNull(activityInstance);

            activityInstance.Signal(null, null);

            activityInstance = processInstance.FindExecution("c");
            Assert.NotNull(activityInstance);
        }

        [Test]
        public void TestPvmAutomatic()
        {
            IPvmProcessDefinition processDefinition = new ProcessDefinitionBuilder()
              .CreateActivity("a")
                .Initial()
                .Behavior(new Automatic())
                .Transition("b")
              .EndActivity()
              .CreateActivity("b")
                .Behavior(new Automatic())
                .Transition("c")
              .EndActivity()
                .CreateActivity("c")
                .Behavior(new WaitState())
              .EndActivity()
            .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.NotNull(processInstance.FindExecution("c"));
        }

        [Test]
        public void TestPvmDecision()
        {
            IPvmProcessDefinition processDefinition = new ProcessDefinitionBuilder()
              .CreateActivity("start")
                .Initial()
                .Behavior(new Automatic())
                .Transition("checkCredit")
              .EndActivity()
              .CreateActivity("checkCredit")
                .Behavior(new Decision())
                .Transition("askDaughterOut", "wow")
                .Transition("takeToGolf", "nice")
                .Transition("ignore", "default")
              .EndActivity()
              .CreateActivity("takeToGolf")
                .Behavior(new WaitState())
              .EndActivity()
              .CreateActivity("askDaughterOut")
                .Behavior(new WaitState())
              .EndActivity()
              .CreateActivity("ignore")
                .Behavior(new WaitState())
              .EndActivity()
            .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.SetVariable("creditRating", "Aaa-");
            processInstance.Start();
            Assert.NotNull(processInstance.FindExecution("takeToGolf"));

            processInstance = processDefinition.CreateProcessInstance();
            processInstance.SetVariable("creditRating", "AAA+");
            processInstance.Start();
            Assert.NotNull(processInstance.FindExecution("askDaughterOut"));

            processInstance = processDefinition.CreateProcessInstance();
            processInstance.SetVariable("creditRating", "bb-");
            processInstance.Start();
            Assert.NotNull(processInstance.FindExecution("ignore"));
        }
    }
}
