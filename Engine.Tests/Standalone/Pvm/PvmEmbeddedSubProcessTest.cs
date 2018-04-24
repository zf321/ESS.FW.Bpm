using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmEmbeddedSubProcessTest
    {
        /// <summary>
        ///               +------------------------------+
        ///               | embedded subprocess          |
        ///     +-----+   |  +-----------+   +---------+ |   +---+
        ///     |start|-->|  |startInside|-->|endInside| |-->|end|
        ///     +-----+   |  +-----------+   +---------+ |   +---+
        ///               +------------------------------+
        /// </summary>
        [Test]
        public virtual void TestEmbeddedSubProcess()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedActiveActivityIds = new List<string>();
            expectedActiveActivityIds.Add("end");

            Assert.AreEqual(expectedActiveActivityIds, processInstance.FindActiveActivityIds());
        }

        /// <summary>
        ///               +----------------------------------------+
        ///               | embeddedsubprocess        +----------+ |
        ///               |                     +---->|endInside1| |
        ///               |                     |     +----------+ |
        ///               |                     |                  |
        ///     +-----+   |  +-----------+   +----+   +----------+ |   +---+
        ///     |start|-->|  |startInside|-->|fork|-->|endInside2| |-->|end|
        ///     +-----+   |  +-----------+   +----+   +----------+ |   +---+
        ///               |                     |                  |
        ///               |                     |     +----------+ |
        ///               |                     +---->|endInside3| |
        ///               |                           +----------+ |
        ///               +----------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestMultipleConcurrentEndsInsideEmbeddedSubProcess()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("endInside1")
                    .Transition("endInside2")
                    .Transition("endInside3")
                    .EndActivity()
                    .CreateActivity("endInside1")
                    .Behavior(new End())
                    .EndActivity()
                    .CreateActivity("endInside2")
                    .Behavior(new End())
                    .EndActivity()
                    .CreateActivity("endInside3")
                    .Behavior(new End())
                    .EndActivity()
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

        /// <summary>
        ///               +-------------------------------------------------+
        ///               | embeddedsubprocess        +----------+          |
        ///               |                     +---->|endInside1|          |
        ///               |                     |     +----------+          |
        ///               |                     |                           |
        ///     +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
        ///     |start|-->|  |startInside|-->|fork|-->|Wait|-->|endInside2| |-->|end|
        ///     +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
        ///               |                     |                           |
        ///               |                     |     +----------+          |
        ///               |                     +---->|endInside3|          |
        ///               |                           +----------+          |
        ///               +-------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestMultipleConcurrentEndsInsideEmbeddedSubProcessWithWaitState()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("endInside1")
                    .Transition("Wait")
                    .Transition("endInside3")
                    .EndActivity()
                    .CreateActivity("endInside1")
                    .Behavior(new End())
                    .EndActivity()
                    .CreateActivity("Wait")
                    .Behavior(new WaitState())
                    .Transition("endInside2")
                    .EndActivity()
                    .CreateActivity("endInside2")
                    .Behavior(new End())
                    .EndActivity()
                    .CreateActivity("endInside3")
                    .Behavior(new End())
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.IsFalse(processInstance.IsEnded);
            IPvmExecution execution = processInstance.FindExecution("Wait");
            execution.Signal(null, null);

            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +-------------------------------------------------------+
        ///     | embedded subprocess                                   |
        ///     |                  +--------------------------------+   |
        ///     |                  | nested embedded subprocess     |   |
        ///     +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
        ///     |start|-->| |startInside|--> |  |startInside|-->|endInside|   |   |-->|end|
        ///     +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
        ///     |                  +--------------------------------+   |
        ///     |                                                       |
        ///     +-------------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestNestedSubProcessNoEnd()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("nestedSubProcess")
                    .EndActivity()
                    .CreateActivity("nestedSubProcess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startNestedInside")
                    .Behavior(new Automatic())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .EndActivity()
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedActiveActivityIds = new List<string>();
            expectedActiveActivityIds.Add("end");

            Assert.AreEqual(expectedActiveActivityIds, processInstance.FindActiveActivityIds());
        }

        /// <summary>
        ///               +------------------------------+
        ///               | embedded subprocess          |
        ///     +-----+   |  +-----------+               |
        ///     |start|-->|  |startInside|               |
        ///     +-----+   |  +-----------+               |
        ///               +------------------------------+
        /// </summary>
        [Test]
        public virtual void TestEmbeddedSubProcessWithoutEndEvents()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +-------------------------------------------------------+
        ///     | embedded subprocess                                   |
        ///     |                  +--------------------------------+   |
        ///     |                  | nested embedded subprocess     |   |
        ///     +-----+   | +-----------+    |  +-----------+                 |   |
        ///     |start|-->| |startInside|--> |  |startInside|                 |   |
        ///     +-----+   | +-----------+    |  +-----------+                 |   |
        ///     |                  +--------------------------------+   |
        ///     |                                                       |
        ///     +-------------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestNestedSubProcessBothNoEnd()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("nestedSubProcess")
                    .EndActivity()
                    .CreateActivity("nestedSubProcess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startNestedInside")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .EndActivity()
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }


        /// <summary>
        ///     +------------------------------+
        ///     | embedded subprocess          |
        ///     +-----+   |  +-----------+   +---------+ |
        ///     |start|-->|  |startInside|-->|endInside| |
        ///     +-----+   |  +-----------+   +---------+ |
        ///     +------------------------------+
        /// </summary>
        [Test]
        public virtual void TestEmbeddedSubProcessNoEnd()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .EndActivity()
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +------------------------------+
        ///     | embedded subprocess          |
        ///     +-----+   |  +-----------+   +---------+ |   +---+
        ///     |start|-->|  |startInside|-->|endInside| |-->|end|
        ///     +-----+   |  +-----------+   +---------+ |   +---+
        ///     +------------------------------+
        /// </summary>
        [Test]
        public virtual void TestStartInScope()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance =
                ((ProcessDefinitionImpl) processDefinition).CreateProcessInstanceForInitial(
                    (ActivityImpl) processDefinition.FindActivity("startInside"));
            processInstance.Start();

            IList<string> expectedActiveActivityIds = new List<string>();
            expectedActiveActivityIds.Add("end");

            Assert.AreEqual(expectedActiveActivityIds, processInstance.FindActiveActivityIds());
        }
    }
}