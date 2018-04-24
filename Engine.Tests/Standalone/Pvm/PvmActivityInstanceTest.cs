using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using Engine.Tests.Standalone.Pvm.Verification;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmActivityInstanceTest
    {
        /// <summary>
        ///     +-----+   +-----+   +-------+
        ///     | one |-->| two |-->| three |
        ///     +-----+   +-----+   +-------+
        /// </summary>
        [Test]
        public virtual void TestSequence()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("one")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            verifier.AssertStartInstanceCount(1, "one");
            verifier.AssertStartInstanceCount(1, "two");
            verifier.AssertStartInstanceCount(1, "three");
        }

        /// <summary>
        ///                      +----------------------------+
        ///                      v                            |
        ///     +-------+   +------+   +-----+   +-----+    +-------+
        ///     | start |-->| loop |-->| one |-->| two |--> | three |
        ///     +-------+   +------+   +-----+   +-----+    +-------+
        ///                      |
        ///                      |   +-----+
        ///                      +-->| end |
        ///                          +-----+
        /// </summary>
        [Test]
        public virtual void TestWhileLoop()
        {
            var verifier = new ActivityInstanceVerification();
            var transitionVerifier = new TransitionInstanceVerifyer();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .StartTransition("loop")
                    .ExecutionListener(ExecutionListenerFields.EventNameTake, transitionVerifier)
                    .EndTransition()
                    .EndActivity()
                    .CreateActivity("loop")
                    .Behavior(new While("Count", 0, 10))
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .StartTransition("one", "more")
                    .ExecutionListener(ExecutionListenerFields.EventNameTake, transitionVerifier)
                    .EndTransition()
                    .StartTransition("end", "done")
                    .ExecutionListener(ExecutionListenerFields.EventNameTake, transitionVerifier)
                    .EndTransition()
                    .EndActivity()
                    .CreateActivity("one")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("loop")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(1, "start");
            verifier.AssertProcessInstanceParent("start", processInstance);

            verifier.AssertStartInstanceCount(11, "loop");
            verifier.AssertProcessInstanceParent("loop", processInstance);

            verifier.AssertStartInstanceCount(10, "one");
            verifier.AssertProcessInstanceParent("one", processInstance);

            verifier.AssertStartInstanceCount(10, "two");
            verifier.AssertProcessInstanceParent("two", processInstance);

            verifier.AssertStartInstanceCount(10, "three");
            verifier.AssertProcessInstanceParent("three", processInstance);

            verifier.AssertStartInstanceCount(1, "end");
            verifier.AssertProcessInstanceParent("end", processInstance);
        }


        /// <summary>
        ///     +-------------------------------------------------+
        ///     | embeddedsubprocess        +----------+          |
        ///     |                     +---->|endInside1|          |
        ///     |                     |     +----------+          |
        ///     |                     |                           |
        ///     +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
        ///     |start|-->|  |startInside|-->|fork|-->|Wait|-->|endInside2| |-->|end|
        ///     +-----+   |  +-----------+   +----+   +----+   +----------+ |   +---+
        ///     |                     |                           |
        ///     |                     |     +----------+          |
        ///     |                     +---->|endInside3|          |
        ///     |                           +----------+          |
        ///     +-------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestMultipleConcurrentEndsInsideEmbeddedSubProcessWithWaitState()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("endInside1")
                    .Transition("Wait")
                    .Transition("endInside3")
                    .EndActivity()
                    .CreateActivity("endInside1")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .CreateActivity("Wait")
                    .Behavior(new WaitState())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("endInside2")
                    .EndActivity()
                    .CreateActivity("endInside2")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .CreateActivity("endInside3")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.IsFalse(processInstance.IsEnded);
            IPvmExecution execution = processInstance.FindExecution("Wait");
            execution.Signal(null, null);

            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(1, "start");
            verifier.AssertProcessInstanceParent("start", processInstance);

            verifier.AssertStartInstanceCount(1, "embeddedsubprocess");
            verifier.AssertProcessInstanceParent("embeddedsubprocess", processInstance);

            verifier.AssertStartInstanceCount(1, "startInside");
            verifier.AssertParent("startInside", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "fork");
            verifier.AssertParent("fork", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "Wait");
            verifier.AssertParent("Wait", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "endInside1");
            verifier.AssertParent("endInside1", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "endInside2");
            verifier.AssertParent("endInside2", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "endInside3");
            verifier.AssertParent("endInside3", "embeddedsubprocess");

            verifier.AssertStartInstanceCount(1, "end");
            verifier.AssertProcessInstanceParent("end", processInstance);
        }

        /// <summary>
        ///               +-------------------------------------------------------+
        ///               | embedded subprocess                                   |
        ///               |                  +--------------------------------+   |
        ///               |                  | nested embedded subprocess     |   |
        ///     +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
        ///     |start|-->| |startInside|--> |  |startInside|-->|endInside|   |   |-->|end|
        ///     +-----+   | +-----------+    |  +-----------+   +---------+   |   |   +---+
        ///               |                  +--------------------------------+   |
        ///               |                                                       |
        ///               +-------------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestNestedSubProcessNoEnd()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("nestedSubProcess")
                    .EndActivity()
                    .CreateActivity("nestedSubProcess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startNestedInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();
            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(1, "start");
            verifier.AssertProcessInstanceParent("start", processInstance);
            verifier.AssertStartInstanceCount(1, "embeddedsubprocess");
            verifier.AssertProcessInstanceParent("embeddedsubprocess", processInstance);
            verifier.AssertStartInstanceCount(1, "startInside");
            verifier.AssertParent("startInside", "embeddedsubprocess");
            verifier.AssertStartInstanceCount(1, "nestedSubProcess");
            verifier.AssertParent("nestedSubProcess", "embeddedsubprocess");
            verifier.AssertStartInstanceCount(1, "startNestedInside");
            verifier.AssertParent("startNestedInside", "nestedSubProcess");
            verifier.AssertStartInstanceCount(1, "endInside");
            verifier.AssertParent("endInside", "nestedSubProcess");
            verifier.AssertStartInstanceCount(1, "end");
            verifier.AssertProcessInstanceParent("end", processInstance);
        }

        /// <summary>
        ///               +-------------------------------------------------------+
        ///               | embedded subprocess                                   |
        ///               |                  +--------------------------------+   |
        ///               |                  | nested embedded subprocess     |   |
        ///     +-----+   | +-----------+    |  +-----------+                 |   |
        ///     |start|-->| |startInside|--> |  |startInside|                 |   |
        ///     +-----+   | +-----------+    |  +-----------+                 |   |
        ///               |                  +--------------------------------+   |
        ///               |                                                       |
        ///               +-------------------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestNestedSubProcessBothNoEnd()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("nestedSubProcess")
                    .EndActivity()
                    .CreateActivity("nestedSubProcess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startNestedInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .EndActivity()
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(1, "start");
            verifier.AssertProcessInstanceParent("start", processInstance);
            verifier.AssertStartInstanceCount(1, "embeddedsubprocess");
            verifier.AssertProcessInstanceParent("embeddedsubprocess", processInstance);
            verifier.AssertStartInstanceCount(1, "startInside");
            verifier.AssertParent("startInside", "embeddedsubprocess");
            verifier.AssertStartInstanceCount(1, "nestedSubProcess");
            verifier.AssertParent("nestedSubProcess", "embeddedsubprocess");
            verifier.AssertStartInstanceCount(1, "startNestedInside");
            verifier.AssertParent("startNestedInside", "nestedSubProcess");
        }


        public virtual void TestSubProcessNoEnd()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .EndActivity()
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(1, "start");
            verifier.AssertProcessInstanceParent("start", processInstance);
            verifier.AssertStartInstanceCount(1, "embeddedsubprocess");
            verifier.AssertProcessInstanceParent("embeddedsubprocess", processInstance);
            verifier.AssertStartInstanceCount(1, "startInside");
            verifier.AssertStartInstanceCount(1, "startInside");
        }

        [Test]
        public virtual void TestStartInSubProcess()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance =
                ((ProcessDefinitionImpl) processDefinition).CreateProcessInstanceForInitial(
                    (ActivityImpl) processDefinition.FindActivity("endInside"));
            processInstance.Start();

            Assert.True(processInstance.IsEnded);

            verifier.AssertStartInstanceCount(0, "start");
            verifier.AssertStartInstanceCount(1, "embeddedsubprocess");
            verifier.AssertProcessInstanceParent("embeddedsubprocess", processInstance);
            verifier.AssertStartInstanceCount(0, "startInside");
            verifier.AssertIsCompletingActivityInstance("endInside", 1);
            verifier.AssertStartInstanceCount(1, "end");
        }


        /// <summary>
        ///     +-----+   +-----+   +-------+
        ///     | one |-->| two |-->| three |
        ///     +-----+   +-----+   +-------+
        /// </summary>
        [Test]
        public virtual void TestScopeActivity()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("one")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Scope()
                    .Behavior(new WaitState())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameStart, verifier)
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IPvmExecution childExecution = processInstance.FindExecution("two");
            var parentActivityInstanceId = ((ExecutionImpl) childExecution).ParentActivityInstanceId;
            Assert.AreEqual(((ExecutionImpl) processInstance).Id, parentActivityInstanceId);

            childExecution.Signal(null, null);

            verifier.AssertStartInstanceCount(1, "one");
            verifier.AssertStartInstanceCount(1, "two");
            verifier.AssertProcessInstanceParent("two", processInstance);
            verifier.AssertStartInstanceCount(1, "three");
        }
    }
}