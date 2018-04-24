using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmParallelTest
    {
        [Test]
        public virtual void TestSimpleAutmaticConcurrency()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("c1")
                    .Transition("c2")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("join")
                    .Behavior(new ParallelGateway())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }

        [Test]
        public virtual void TestSimpleWaitStateConcurrency()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("c1")
                    .Transition("c2")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new WaitState())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new WaitState())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("join")
                    .Behavior(new ParallelGateway())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IPvmExecution activityInstanceC1 = processInstance.FindExecution("c1");
            Assert.NotNull(activityInstanceC1);

            IPvmExecution activityInstanceC2 = processInstance.FindExecution("c2");
            Assert.NotNull(activityInstanceC2);

            activityInstanceC1.Signal(null, null);
            activityInstanceC2.Signal(null, null);

            IList<string> activityNames = processInstance.FindActiveActivityIds();
            IList<string> expectedActivityNames = new List<string>();
            expectedActivityNames.Add("end");

            Assert.AreEqual(expectedActivityNames, activityNames);
        }

        [Test]
        public virtual void TestUnstructuredConcurrencyTwoJoins()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("c1")
                    .Transition("c2")
                    .Transition("c3")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new Automatic())
                    .Transition("join1")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new Automatic())
                    .Transition("join1")
                    .EndActivity()
                    .CreateActivity("c3")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("join1")
                    .Behavior(new ParallelGateway())
                    .Transition("c4")
                    .EndActivity()
                    .CreateActivity("c4")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("join2")
                    .Behavior(new ParallelGateway())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.NotNull(processInstance.FindExecution("end"));
        }

        [Test]
        public virtual void TestUnstructuredConcurrencyTwoForks()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork1")
                    .EndActivity()
                    .CreateActivity("fork1")
                    .Behavior(new ParallelGateway())
                    .Transition("c1")
                    .Transition("c2")
                    .Transition("fork2")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("fork2")
                    .Behavior(new ParallelGateway())
                    .Transition("c3")
                    .Transition("c4")
                    .EndActivity()
                    .CreateActivity("c3")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("c4")
                    .Behavior(new Automatic())
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("join")
                    .Behavior(new ParallelGateway())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.NotNull(processInstance.FindExecution("end"));
        }

        [Test]
        public virtual void TestJoinForkCombinedInOneParallelGateway()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("c1")
                    .Transition("c2")
                    .Transition("c3")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new Automatic())
                    .Transition("join1")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new Automatic())
                    .Transition("join1")
                    .EndActivity()
                    .CreateActivity("c3")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("join1")
                    .Behavior(new ParallelGateway())
                    .Transition("c4")
                    .Transition("c5")
                    .Transition("c6")
                    .EndActivity()
                    .CreateActivity("c4")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("c5")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("c6")
                    .Behavior(new Automatic())
                    .Transition("join2")
                    .EndActivity()
                    .CreateActivity("join2")
                    .Behavior(new ParallelGateway())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.NotNull(processInstance.FindExecution("end"));
        }
    }
}