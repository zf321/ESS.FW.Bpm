using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmEventTest
    {
        /// <summary>
        ///     +-------+   +-----+
        ///     | start |-->| end |
        ///     +-------+   +-----+
        /// </summary>
        [Test]
        public virtual void TestStartEndEvents()
        {
            var eventCollector = new EventCollector();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder("events").ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .StartTransition("end")
                    .ExecutionListener(eventCollector)
                    .EndTransition()
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedEvents = new List<string>();
            expectedEvents.Add("start on ProcessDefinition(events)");
            expectedEvents.Add("start on Activity(start)");
            expectedEvents.Add("end on Activity(start)");
            expectedEvents.Add("take on (start)-->(end)");
            expectedEvents.Add("start on Activity(end)");
            expectedEvents.Add("end on Activity(end)");
            expectedEvents.Add("end on ProcessDefinition(events)");

            Assert.AreEqual(string.Join(",",expectedEvents),string.Join(",",eventCollector.Events));
        }

        /// <summary>
        ///     +------------------------------+
        ///     +-----+   | +-----------+   +----------+ |   +---+
        ///     |start|-->| |startInside|-->|endInsdide| |-->|end|
        ///     +-----+   | +-----------+   +----------+ |   +---+
        ///     +------------------------------+
        /// </summary>
        [Test]
        public virtual void TestEmbeddedSubProcessEvents()
        {
            var eventCollector = new EventCollector();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder("events").ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedEvents = new List<string>();
            expectedEvents.Add("start on ProcessDefinition(events)");
            expectedEvents.Add("start on Activity(start)");
            expectedEvents.Add("end on Activity(start)");
            expectedEvents.Add("start on Activity(embeddedsubprocess)");
            expectedEvents.Add("start on Activity(startInside)");
            expectedEvents.Add("end on Activity(startInside)");
            expectedEvents.Add("start on Activity(endInside)");
            expectedEvents.Add("end on Activity(endInside)");
            expectedEvents.Add("end on Activity(embeddedsubprocess)");
            expectedEvents.Add("start on Activity(end)");
            expectedEvents.Add("end on Activity(end)");
            expectedEvents.Add("end on ProcessDefinition(events)");

            Assert.AreEqual(string.Join(",", expectedEvents),
                            string.Join(",", eventCollector.Events), "expected " + expectedEvents + ", but was \n" + eventCollector + "\n");
        }


        /// <summary>
        ///                      +--+
        ///                 +--->|c1|---+
        ///                 |    +--+   |
        ///                 |           v
        ///     +-----+   +----+       +----+   +---+
        ///     |start|-->|fork|       |join|-->|end|
        ///     +-----+   +----+       +----+   +---+
        ///                 |           ^
        ///                 |    +--+   |
        ///                 +--->|c2|---+
        ///                      +--+
        /// </summary>
        [Test]
        public virtual void TestSimpleAutmaticConcurrencyEvents()
        {
            var eventCollector = new EventCollector();

            var processDefinition =
                new ProcessDefinitionBuilder("events").ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("fork")
                    .EndActivity()
                    .CreateActivity("fork")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("c1")
                    .Transition("c2")
                    .EndActivity()
                    .CreateActivity("c1")
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("c2")
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("join")
                    .EndActivity()
                    .CreateActivity("join")
                    .Behavior(new ParallelGateway())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedEvents = new List<string>();
            expectedEvents.Add("start on ProcessDefinition(events)");
            expectedEvents.Add("start on Activity(start)");
            expectedEvents.Add("end on Activity(start)");
            expectedEvents.Add("start on Activity(fork)");
            expectedEvents.Add("end on Activity(fork)");
            expectedEvents.Add("start on Activity(join)");
            expectedEvents.Add("start on Activity(c2)");
            expectedEvents.Add("end on Activity(c2)");
            expectedEvents.Add("start on Activity(c1)");
            expectedEvents.Add("end on Activity(c1)");
            expectedEvents.Add("start on Activity(join)");
            expectedEvents.Add("end on Activity(join)");
            expectedEvents.Add("end on Activity(join)");
            expectedEvents.Add("start on Activity(end)");
            expectedEvents.Add("end on Activity(end)");
            expectedEvents.Add("end on ProcessDefinition(events)");

            Assert.AreEqual(string.Join(",", expectedEvents),
                string.Join(",", eventCollector.Events));
        }

        /// <summary>
        ///     +-----------------------------------------------+
        ///     +-----+   | +-----------+   +------------+   +----------+ |   +---+
        ///     |start|-->| |startInside|-->| taskInside |-->|endInsdide| |-->|end|
        ///     +-----+   | +-----------+   +------------+   +----------+ |   +---+
        ///     +-----------------------------------------------+
        /// </summary>
        [Test]
        public virtual void TestEmbeddedSubProcessEventsDelete()
        {
            var eventCollector = new EventCollector();

            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder("events").ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EmbeddedSubProcess())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("taskInside")
                    .EndActivity()
                    .CreateActivity("taskInside")
                    .Behavior(new WaitState())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new End())
                    .ExecutionListener(PvmEvent.EventNameStart, eventCollector)
                    .ExecutionListener(PvmEvent.EventNameEnd, eventCollector)
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            var execution = (ExecutionImpl) processInstance;
            var cmd = new FoxDeleteProcessInstanceCmd(null, null);
            IList<PvmExecutionImpl> collectExecutionToDelete = cmd.CollectExecutionToDelete(execution);
            foreach (var interpretableExecution in collectExecutionToDelete)
                interpretableExecution.DeleteCascade2("");

            IList<string> expectedEvents = new List<string>();
            expectedEvents.Add("start on ProcessDefinition(events)");
            expectedEvents.Add("start on Activity(start)");
            expectedEvents.Add("end on Activity(start)");
            expectedEvents.Add("start on Activity(embeddedsubprocess)");
            expectedEvents.Add("start on Activity(startInside)");
            expectedEvents.Add("end on Activity(startInside)");
            expectedEvents.Add("start on Activity(taskInside)");
            expectedEvents.Add("end on Activity(taskInside)");
            expectedEvents.Add("end on Activity(embeddedsubprocess)");
            expectedEvents.Add("end on ProcessDefinition(events)");

            Assert.AreEqual(string.Join(",", expectedEvents),
                string.Join(",",eventCollector.Events), "expected " + expectedEvents + ", but was \n" + eventCollector + "\n");
        }
    }
}