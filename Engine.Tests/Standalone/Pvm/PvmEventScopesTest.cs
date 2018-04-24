using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmEventScopesTest
    {
        /// <summary>
        ///                           create evt Scope --+
        ///                                              |
        ///                                              v
        ///               +------------------------------+
        ///               | embedded subprocess          |
        ///     +-----+   |  +-----------+   +---------+ |   +----+   +---+
        ///     |start|-->|  |startInside|-->|endInside| |-->|Wait|-->|end|
        ///     +-----+   |  +-----------+   +---------+ |   +----+   +---+
        ///               +------------------------------+
        ///                                                               ^
        ///                                                               |
        ///                                           destroy evt Scope --+
        /// </summary>
        [Test]
        public virtual void TestActivityEndDestroysEventScopes()
        {
            IPvmProcessDefinition processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("start")
                    .Initial()
                    .Behavior(new Automatic())
                    .Transition("embeddedsubprocess")
                    .EndActivity()
                    .CreateActivity("embeddedsubprocess")
                    .Scope()
                    .Behavior(new EventScopeCreatingSubprocess())
                    .CreateActivity("startInside")
                    .Behavior(new Automatic())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .Transition("Wait")
                    .EndActivity()
                    .CreateActivity("Wait")
                    .Behavior(new WaitState())
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            var eventScopeFound = false;
            var executions =  ((ExecutionImpl) processInstance).Executions;
            foreach (var executionImpl in executions)
                if (executionImpl.IsEventScope)
                {
                    eventScopeFound = true;
                    break;
                }

            Assert.True(eventScopeFound);

            processInstance.Signal(null, null);

            Assert.True(processInstance.IsEnded);
        }


        /// <summary>
        ///               +----------------------------------------------------------------------+
        ///               | embedded subprocess                                                  |
        ///               |                                                                      |
        ///               |                                create evt Scope --+                  |
        ///               |                                                   |                  |
        ///               |                                                   v                  |
        ///               |                                                                      |
        ///               |                  +--------------------------------+                  |
        ///               |                  | nested embedded subprocess     |                  |
        ///     +-----+   | +-----------+    |  +-----------------+           |   +----+   +---+ |   +---+
        ///     |start|-->| |startInside|--> |  |startNestedInside|           |-->|Wait|-->|end| |-->|end|
        ///     +-----+   | +-----------+    |  +-----------------+           |   +----+   +---+ |   +---+
        ///               |                  +--------------------------------+                  |
        ///               |                                                                      |
        ///               +----------------------------------------------------------------------+
        ///                                                                                      ^
        ///                                                                                      |
        ///                                                                  destroy evt Scope --+
        /// </summary>
        [Test]
        public virtual void TestTransitionDestroysEventScope()
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
                    .Behavior(new EventScopeCreatingSubprocess())
                    .CreateActivity("startNestedInside")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .Transition("Wait")
                    .EndActivity()
                    .CreateActivity("Wait")
                    .Behavior(new WaitState())
                    .Transition("endInside")
                    .EndActivity()
                    .CreateActivity("endInside")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .Transition("end")
                    .EndActivity()
                    .CreateActivity("end")
                    .Behavior(new Automatic())
                    .EndActivity()
                    .BuildProcessDefinition();

            var processInstance = processDefinition.CreateProcessInstance();
            processInstance.Start();

            IList<string> expectedActiveActivityIds = new List<string>();
            expectedActiveActivityIds.Add("Wait");
            Assert.AreEqual(expectedActiveActivityIds, processInstance.FindActiveActivityIds());


            IPvmExecution execution = processInstance.FindExecution("Wait");
            execution.Signal(null, null);

            Assert.True(processInstance.IsEnded);
        }
    }
}