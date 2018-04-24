using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Escalation
{
    [TestFixture]
    public class EscalationEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testThrowEscalationEventFromEmbeddedSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testThrowEscalationEventHierarchical()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event inside the subprocess should catch the escalation event (and not the boundary event on process)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.nonInterruptingEscalationBoundaryEventOnCallActivity.bpmn20.xml" })]
        public virtual void testThrowEscalationEventFromCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");
            // when throw an escalation event on called process

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event on call activity should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the called process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after thrown escalation").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml" })]
        public virtual void testThrowEscalationEventNotCaught()
        {
            runtimeService.StartProcessInstanceByKey("throwEscalationProcess");
            // when throw an escalation event

            // continue the process instance, no activity should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after thrown escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryEventWithEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess with escalationCode=1

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event with escalationCode=1 should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation 1").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryEventWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event without escalationCode should catch the escalation event (and all other escalation events)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryEventWithEmptyEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event with empty escalationCode should catch the escalation event (and all other escalation events)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testBoundaryEventWithoutEscalationRef()
        public virtual void testBoundaryEventWithoutEscalationRef()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event without escalationRef should catch the escalation event (and all other escalation events)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the multi-instance subprocess

            // the interrupting boundary event should catch the first escalation event and cancel all instances of the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the multi-instance subprocess

            Assert.AreEqual(10, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event should catch every escalation event
            Assert.AreEqual(5, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(5, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testImplicitNonInterruptingEscalationBoundaryEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the implicit non-interrupting boundary event ('cancelActivity' is not defined) should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingEscalationBoundaryEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the interrupting boundary should catch the escalation event event and cancel the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.interruptingEscalationBoundaryEventOnCallActivity.bpmn20.xml" })]
        public virtual void testInterruptingEscalationBoundaryEventOnCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");
            // when throw an escalation event on called process

            // the interrupting boundary event on call activity should catch the escalation event and cancel the called process
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testParallelEscalationEndEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation end event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the parallel flow in subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testEscalationEndEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation end event inside the subprocess

            // the subprocess should end and
            // the non-interrupting boundary event should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesWhileCatchEscalationOnCallActivity()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // the non-interrupting boundary event on call activity should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.NameWithoutCascade=="task after catched escalation").Count());
            // and set the output variable of the called process to the process
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesTwoTimes()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            ITask taskInSuperProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(taskInSuperProcess);

            // (1) the variables has been passed for the first time (from sub process to super process)
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));

            // change variable "input" in sub process
            ITask taskInSubProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "input", 999);
            taskService.Complete(taskInSubProcess.Id);

            // (2) the variables has been passed for the second time (from sub process to super process)
            Assert.AreEqual(999, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // the interrupting boundary event on call activity should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and set the output variable of the called process to the process
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWithoutCatchEscalation.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesWithoutCatchEscalation()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // then the output variable of the called process should be set to the process
            // also if the escalation is not caught by the process
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testRetrieveEscalationCodeVariableOnBoundaryEvent()
        public virtual void testRetrieveEscalationCodeVariableOnBoundaryEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the boundary event should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment]
        public virtual void testRetrieveEscalationCodeVariableOnBoundaryEventWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the boundary event without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: Deployment(new string[] {"resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml"}) public void testInterruptingRetrieveEscalationCodeInSuperProcess()
        public virtual void testInterruptingRetrieveEscalationCodeInSuperProcess()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml" })]
        public virtual void testInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testNonInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml" })]
        public virtual void testNonInterruptingRetrieveEscalationCodeInSuperProcess()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventTest.testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml" })]
        public virtual void testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }
    }

}