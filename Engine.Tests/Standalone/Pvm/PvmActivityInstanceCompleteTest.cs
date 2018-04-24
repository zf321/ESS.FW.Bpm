using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmActivityInstanceCompleteTest
    {
        /// <summary>
        ///     +-------+   +-----+
        ///     | start |-->| end |
        ///     +-------+   +-----+
        /// </summary>
        [Test]
        public virtual void TestSingleEnd()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            verifier.AssertNonCompletingActivityInstance("start", 1);
            verifier.AssertIsCompletingActivityInstance("end", 1);
        }

        /// <summary>
        ///     +----+
        ///     +--->|end1|
        ///     |    +----+
        ///     |
        ///     +-----+   +----+
        ///     |start|-->|fork|
        ///     +-----+   +----+
        ///     |
        ///     |    +----+
        ///     +--->|end2|
        ///     +----+
        /// </summary>
        [Test]
        public virtual void TestTwoEnds()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("end1")
                    .Transition("end2")
                    .EndActivity()
                    .CreateActivity("end1")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .CreateActivity("end2")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            verifier.AssertNonCompletingActivityInstance("start", 1);
            verifier.AssertNonCompletingActivityInstance("fork", 1);
            verifier.AssertIsCompletingActivityInstance("end1", 1);
            verifier.AssertIsCompletingActivityInstance("end2", 1);
        }

        /// <summary>
        ///     +----+
        ///     +--->| a1 |---+
        ///     |    +----+   |
        ///     |             v
        ///     +-----+   +----+       +------+    +-----+
        ///     |start|-->|fork|       | join |--->| end |
        ///     +-----+   +----+       +------+    +-----+
        ///     |             ^
        ///     |    +----+   |
        ///     +--->| a2 |---+
        ///     +----+
        /// </summary>
        [Test]
        public virtual void TestSingleEndAfterParallelJoin()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("a1")
                    .Transition("a2")
                    .EndActivity()
                    .CreateActivity("a1")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("a2")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("join")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            verifier.AssertNonCompletingActivityInstance("start", 1);
            verifier.AssertNonCompletingActivityInstance("fork", 1);
            verifier.AssertNonCompletingActivityInstance("a1", 1);
            verifier.AssertNonCompletingActivityInstance("a2", 1);
            verifier.AssertNonCompletingActivityInstance("join", 2);
            verifier.AssertIsCompletingActivityInstance("end", 1);
        }

        /// <summary>
        ///               +-------------------------------+
        ///               | embeddedsubprocess            |
        ///               |                               |
        ///     +-----+   |  +-----------+   +---------+  |   +---+
        ///     |start|-->|  |startInside|-->|endInside|  |-->|end|
        ///     +-----+   |  +-----------+   +---------+  |   +---+
        ///               |                               |
        ///               |                               |
        ///               +-------------------------------+
        /// </summary>
        [Test]
        public virtual void TestSimpleSubProcess()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            verifier.AssertNonCompletingActivityInstance("start", 1);
            verifier.AssertNonCompletingActivityInstance("embeddedsubprocess", 1);
            verifier.AssertNonCompletingActivityInstance("startInside", 1);
            verifier.AssertIsCompletingActivityInstance("endInside", 1);
            verifier.AssertIsCompletingActivityInstance("end", 1);
        }

        /// <summary>
        ///     +----------+
        ///     | userTask |
        ///     |          |
        ///     +-----+   |          |    +------+
        ///     |start|-->|          |--->| end1 |
        ///     +-----+   | +-----+  |
        ///     +-|timer|--+
        ///     +-----+
        ///     |          +------+
        ///     +--------->| end2 |
        ///     +------+
        /// </summary>
        [Test]
        public virtual void TestBoundaryEvent()
        {
            var verifier = new ActivityInstanceVerification();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("userTask")
                    .EndActivity()
                    .CreateActivity("userTask")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .Transition("end1")
                    .EndActivity()
                    .CreateActivity("timer")
                    .Behavior(new WaitState())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .AttachedTo("userTask", true)
                    .Transition("end2")
                    .EndActivity()
                    .CreateActivity("end1")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .CreateActivity("end2")
                    .Behavior(new End())
                    .ExecutionListener(ExecutionListenerFields.EventNameEnd, verifier)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            var userTaskExecution = processInstance.FindExecution("userTask");
            ((PvmExecutionImpl) userTaskExecution).ExecuteActivity(processDefinition.FindActivity("timer"));

            var timerExecution = processInstance.FindExecution("timer");
            timerExecution.Signal(null, null);

            verifier.AssertNonCompletingActivityInstance("start", 1);
            verifier.AssertNonCompletingActivityInstance("userTask", 1);
            verifier.AssertIsCompletingActivityInstance("end2", 1);
        }
    }
}