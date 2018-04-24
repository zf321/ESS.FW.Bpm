using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Escalation
{
    [TestFixture]
    public class EscalationEventSubprocessTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testCatchEscalationEventInsideSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testCatchEscalationEventFromEmbeddedSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess outside the subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testCatchEscalationEventFromCallActivity.bpmn20.xml"})]
        public virtual void testCatchEscalationEventFromCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");
            // when throw an escalation event on called process

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the called process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after thrown escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testCatchEscalationEventFromTopLevelProcess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event from top level process

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess on the top level process should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after thrown escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testCatchEscalationEventFromMultiInstanceSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside a multi-instance subprocess

            Assert.AreEqual(10, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess outside the subprocess should catch every escalation event
            Assert.AreEqual(5, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(5, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testPreferEscalationEventSubprocessToBoundaryEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            // (the boundary event on the subprocess should not catch the escalation event since the event subprocess consume this event)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testEscalationEventSubprocessWithEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess with escalationCode=1

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess with escalationCode=1 should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation 1").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testEscalationEventSubprocessWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess without escalationCode should catch the escalation event (and all other escalation events)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testInterruptionEscalationEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the interrupting event subprocess inside the subprocess should catch the escalation event event and cancel the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testInterruptingEscalationEventSubprocessWithCallActivity.bpmn20.xml" })]
        public virtual void testInterruptingEscalationEventSubprocessWithCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");
            // when throw an escalation event on called process

            // the interrupting event subprocess should catch the escalation event and cancel the called process
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testInterruptionEscalationEventSubprocessWithMultiInstanceSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the multi-instance subprocess

            // the interrupting event subprocess outside the subprocess should catch the first escalation event and cancel all instances of the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
        }

        [Test]
        [Deployment]
        public virtual void testReThrowEscalationEventToBoundaryEvent()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").First();
            Assert.NotNull(task);

            // when re-throw the escalation event from the escalation event subprocess
            taskService.Complete(task.Id);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event on subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation on boundary event").Count());
            // and continue the process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testReThrowEscalationEventToBoundaryEventWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").First();
            Assert.NotNull(task);

            // when re-throw the escalation event from the escalation event subprocess
            taskService.Complete(task.Id);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting boundary event on subprocess without escalationCode should catch the escalation event (and all other escalation events)
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation on boundary event").Count());
            // and continue the process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testReThrowEscalationEventToEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").First();
            Assert.NotNull(task);

            // when re-throw the escalation event from the escalation event subprocess
            taskService.Complete(task.Id);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the non-interrupting event subprocess on process level should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation on process level").Count());
            // and continue the process
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testReThrowEscalationEventIsNotCatched()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the non-interrupting event subprocess inside the subprocess should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess").First();
            Assert.NotNull(task);

            // when re-throw the escalation event from the escalation event subprocess
            taskService.Complete(task.Id);

            // continue the subprocess, no activity should catch the re-thrown escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testThrowEscalationEventToEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the first non-interrupting event subprocess inside the subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess1").Count());
            // and continue the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());

            // when throw a second escalation event from the first event subprocess
            string taskId = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess1").First().Id;
            taskService.Complete(taskId);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            // the second non-interrupting event subprocess inside the subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation inside subprocess2").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task in subprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesWhileCatchEscalationOnCallActivity()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // the non-interrupting event subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and set the output variable of the called process to the process
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesTwoTimes()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // (1) the variables has been passed for the first time (from sub process to super process)
            ITask taskInSuperProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(taskInSuperProcess);
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));

            // change variable "input" in sub process
            ITask taskInSubProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "input", 999);
            taskService.Complete(taskInSubProcess.Id);

            // (2) the variables has been passed for the second time (from sub process to super process)
            Assert.AreEqual(999, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity.bpmn20.xml" })]
        public virtual void testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input"] = 42;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchEscalationProcess", variables).Id;
            // when throw an escalation event on called process

            // the interrupting event subprocess should catch the escalation event
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").Count());
            // and set the output variable of the called process to the process
            Assert.AreEqual(42, runtimeService.GetVariable(ProcessInstanceId, "output"));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testRetrieveEscalationCodeVariableOnEventSubprocess()
        public virtual void testRetrieveEscalationCodeVariableOnEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the event subprocess should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment]
        public virtual void testRetrieveEscalationCodeVariableOnEventSubprocessWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            // when throw an escalation event inside the subprocess

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.Name =="task after catched escalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml" })]
        public virtual void testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode()
        {
            runtimeService.StartProcessInstanceByKey("catchEscalationProcess");

            // the event subprocess without escalationCode should catch the escalation event
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterCatchedEscalation").First();
            Assert.NotNull(task);

            // and set the escalationCode of the escalation event to the declared variable
            Assert.AreEqual("escalationCode", runtimeService.GetVariable(task.ExecutionId, "escalationCodeVar"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingEscalationTriggeredTwice.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationTriggeredTwiceWithMainTaskCompletedFirst()
        {

            // given
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            ITask taskInMainprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInMainprocess").First();

            // when
            taskService.Complete(taskInMainprocess.Id);

            // then
            Assert.AreEqual(2, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingEscalationTriggeredTwice.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationTriggeredTwiceWithSubprocessTaskCompletedFirst()
        {

            // given
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            ITask taskInMainprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInMainprocess").First();
            ITask taskInSubprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").First();

            // when
            taskService.Complete(taskInSubprocess.Id);
            taskService.Complete(taskInMainprocess.Id);

            // then
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingEscalationTriggeredTwiceByIntermediateEvent.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationTriggeredTwiceByIntermediateEventWithMainTaskCompletedFirst()
        {

            // given
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            ITask taskInMainprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="FirstTaskInMainprocess").First();

            // when
            taskService.Complete(taskInMainprocess.Id);

            // then
            Assert.AreEqual(2, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="SecondTaskInMainprocess").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testNonInterruptingEscalationTriggeredTwiceByIntermediateEvent.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationTriggeredTwiceByIntermediateEventWithSubprocessTaskCompletedFirst()
        {

            // given
            runtimeService.StartProcessInstanceByKey("escalationProcess");
            ITask taskInMainprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="FirstTaskInMainprocess").First();
            ITask taskInSubprocess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").First();

            // when
            taskService.Complete(taskInSubprocess.Id);
            taskService.Complete(taskInMainprocess.Id);

            // then
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="TaskInSubprocess").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="SecondTaskInMainprocess").Count());
        }

    }
}