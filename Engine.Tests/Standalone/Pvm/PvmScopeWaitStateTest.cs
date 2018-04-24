using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmScopeWaitStateTest
    {
        /// <summary>
        ///     +-----+   +----------+   +---+
        ///     |start|-->|scopedWait|-->|end|
        ///     +-----+   +----------+   +---+
        /// </summary>
        [Test]
        public virtual void TestWaitStateScope()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("scopedWait")
                    .EndActivity()
                    .CreateActivity("scopedWait")
                    .Scope()
                    .Behavior(new WaitState())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            IPvmProcessInstance processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IPvmExecution execution = processInstance.FindExecution("scopedWait");
            Assert.NotNull(execution);

            execution.Signal(null, null);

            Assert.AreEqual(new List<string>(), processInstance.FindActiveActivityIds());
            Assert.True(processInstance.IsEnded);
        }
    }
}