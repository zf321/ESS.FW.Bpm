using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmParallelEndTest
    {
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
        ///        
        [Test]
        public virtual void TestParallelEnd()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .Transition("end1")
                    .Transition("end2")
                    .EndActivity()
                    .CreateActivity("end1")
                    .Behavior(new End())
                    .EndActivity()
                    .CreateActivity("end2")
                    .Behavior(new End())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Assert.True(processInstance.IsEnded);
        }
    }
}