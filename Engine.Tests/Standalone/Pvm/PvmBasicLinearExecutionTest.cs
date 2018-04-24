using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmBasicLinearExecutionTest
    {
        /// <summary>
        ///     +-------+   +-----+
        ///     | start |-->| end |
        ///     +-------+   +-----+
        /// </summary>
        [Test]
        public virtual void TestStartEnd()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +-----+   +-----+   +-------+
        ///     | one |-->| two |-->| three |
        ///     +-----+   +-----+   +-------+
        /// </summary>
        [Test]
        public virtual void TestSingleAutomatic()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("one")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new Automatic())
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +-----+   +-----+   +-------+
        ///     | one |-->| two |-->| three |
        ///     +-----+   +-----+   +-------+
        /// </summary>
        [Test]
        public virtual void TestSingleWaitState()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("one")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new WaitState())
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            var activityInstance = processInstance.FindExecution("two");
            Assert.NotNull(activityInstance);

            activityInstance.Signal(null, null);

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +-----+   +-----+   +-------+   +------+    +------+
        ///     | one |-->| two |-->| three |-->| four |--> | five |
        ///     +-----+   +-----+   +-------+   +------+    +------+
        /// </summary>
        [Test]
        public virtual void TestCombinationOfWaitStatesAndAutomatics()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("one")
                    .EndActivity()
                    .CreateActivity("one")
                    .Behavior(new WaitState())
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new WaitState())
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new Automatic())
                    .Transition("four")
                    .EndActivity()
                    .CreateActivity("four")
                    .Behavior(new Automatic())
                    .Transition("five")
                    .EndActivity()
                    .CreateActivity("five")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            var activityInstance = processInstance.FindExecution("one");
            Assert.NotNull(activityInstance);
            activityInstance.Signal(null, null);

            activityInstance = processInstance.FindExecution("two");
            Assert.NotNull(activityInstance);
            activityInstance.Signal(null, null);

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }

        /// <summary>
        ///     +----------------------------+
        ///     v                            |
        ///     +-------+   +------+   +-----+   +-----+    +-------+
        ///     | start |-->| loop |-->| one |-->| two |--> | three |
        ///     +-------+   +------+   +-----+   +-----+    +-------+
        ///     |
        ///     |   +-----+
        ///     +-->| end |
        ///     +-----+
        /// </summary>
        [Test]
        public virtual void TestWhileLoop()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("loop")
                    .EndActivity()
                    .CreateActivity("loop")
                    .Behavior(new While("Count", 0, 10))
                    .Transition("one", "more")
                    .Transition("end", "done")
                    .EndActivity()
                    .CreateActivity("one")
                    .Behavior(new Automatic())
                    .Transition("two")
                    .EndActivity()
                    .CreateActivity("two")
                    .Behavior(new Automatic())
                    .Transition("three")
                    .EndActivity()
                    .CreateActivity("three")
                    .Behavior(new Automatic())
                    .Transition("loop")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }
    }
}