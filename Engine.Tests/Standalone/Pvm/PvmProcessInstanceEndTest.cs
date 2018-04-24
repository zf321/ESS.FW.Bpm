using System;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmProcessInstanceEndTest
    {
        [Test]
        public virtual void TestSimpleProcessInstanceEnd()
        {
            var eventCollector = new EventCollector();

            var processDefinition =
                new ProcessDefinitionBuilder().ExecutionListener(
                        PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("Wait")
                    .EndActivity()
                    .CreateActivity("Wait")
                    .Behavior(new WaitState())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            Console.Error.WriteLine(eventCollector);

            processInstance.DeleteCascade("test");

            Console.Error.WriteLine();
            Console.Error.WriteLine(eventCollector);
        }
    }
}