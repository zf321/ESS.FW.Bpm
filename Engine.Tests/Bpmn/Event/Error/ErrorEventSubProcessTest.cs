using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Error
{
    [TestFixture]
	public class ErrorEventSubProcessTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testEventSubprocessTakesPrecedence()
        {
            // an event subprocesses takes precedence over a boundary event
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testErrorCodeTakesPrecedence()
        {
            // an event subprocess with errorCode takes precedence over a catch-all handler
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;

            // The process will throw an error event,
            // which is caught and escalated by a IUser Task
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterErrorCatch2").Count(), "No tasks found in task list."); // <!>
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Task", task.Name);

            // Completing the ITask will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);

        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorInEmbeddedSubProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByScriptTaskInEmbeddedSubProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorThrownByScriptTaskInEmbeddedSubProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByScriptTaskInEmbeddedSubProcessWithErrorCode()
        {
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorThrownByScriptTaskInEmbeddedSubProcessWithErrorCode").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByScriptTaskInTopLevelProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorThrownByScriptTaskInTopLevelProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByScriptTaskInsideSubProcessInTopLevelProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("CatchErrorThrownByScriptTaskInsideSubProcessInTopLevelProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
        public virtual void testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownByExecuteOfAbstractBpmnActivityBehavior()
        {
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwException()).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByExecuteOfAbstractBpmnActivityBehavior()
        {
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwError()).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownBySignalOfAbstractBpmnActivityBehavior()
        {
            string pi = runtimeService.StartProcessInstanceByKey("testProcess").Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi&& c.ActivityId =="serviceTask").First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool)runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchErrorThrownBySignalOfAbstractBpmnActivityBehavior()
        {
            string pi = runtimeService.StartProcessInstanceByKey("testProcess").Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi&& c.ActivityId =="serviceTask").First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool)runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownByExecuteOfDelegateExpression()
        {
            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwException() as IDictionary<string, ITypedValue>);
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", variables).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByExecuteOfDelegateExpression()
        {
            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwError() as IDictionary<string, ITypedValue>);
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", variables).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownBySignalOfDelegateExpression()
        {
            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myDelegate", new ThrowErrorDelegate());
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", variables).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi&& c.ActivityId =="serviceTask").First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool)runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchErrorThrownBySignalOfDelegateExpression()
        {
            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myDelegate", new ThrowErrorDelegate());
            string pi = runtimeService.StartProcessInstanceByKey("testProcess", variables).Id;

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi&& c.ActivityId =="serviceTask").First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool)runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool)runtimeService.GetVariable(pi, "signaled"));

            ITask userTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi).First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        private void assertThatErrorHasBeenCaught(string procId)
        {
            // The process will throw an error event,
            // which is caught and escalated by a IUser Task
            Assert.AreEqual( 1, taskService.CreateTaskQuery().Count(),"No tasks found in task list.");
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Task", task.Name);

            // Completing the ITask will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorEventSubprocessSetErrorVariables()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorCode";
            IVariableInstance errorVariable = runtimeService.CreateVariableInstanceQuery()/*.VariableName(VariableName)*/.First();

            Assert.That(errorVariable, Is.Not.Null);
            //the code we gave the thrown error
            object errorCode = "error";
            Assert.That(errorVariable.Value, Is.EqualTo(errorCode));

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ThrowErrorProcess.bpmn", "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorFromCallActivitySetsErrorVariables.bpmn" })]
        public virtual void testCatchErrorFromCallActivitySetsErrorVariable()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorCode";
            IVariableInstance errorVariable = runtimeService.CreateVariableInstanceQuery()/*.VariableName(VariableName)*/.First();

            Assert.That(errorVariable, Is.Not.Null);
            //the code we gave the thrown error
            object errorCode = "error";
            Assert.That(errorVariable.Value, Is.EqualTo(errorCode));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testCatchBpmnErrorFromJavaDelegateInsideCallActivitySetsErrorVariable.bpmn", "resources/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
        public virtual void testCatchBpmnErrorFromJavaDelegateInsideCallActivitySetsErrorVariable()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorCode";
            //the code we gave the thrown error
            object errorCode = "errorCode";
            IVariableInstance errorVariable = runtimeService.CreateVariableInstanceQuery()/*.VariableName(VariableName)*/.First();
            Assert.That(errorVariable.Value, Is.EqualTo(errorCode));

            errorVariable = runtimeService.CreateVariableInstanceQuery()/*.VariableName("errorMessageVariable")*/.First();
            Assert.That(errorVariable.Value, Is.EqualTo((object)"ouch!"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoop.bpmn20.xml" })]
        public virtual void testShouldNotThrowErrorInLoop()
        {
            runtimeService.StartProcessInstanceByKey("looping-error");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("WaitState", task.Name);
            taskService.Complete(task.Id);

            Assert.AreEqual("ErrorHandlingUserTask", taskService.CreateTaskQuery().First().Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopWithCallActivity.bpmn20.xml", "resources/bpmn/event/error/ThrowErrorToCallActivity.bpmn20.xml" })]
        public virtual void testShouldNotThrowErrorInLoopWithCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("CallActivityErrorInLoop");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("ErrorLog", task.Name);
            taskService.Complete(task.Id);

            Assert.AreEqual("ErrorHandlingUserTask", taskService.CreateTaskQuery().First().Name);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: Deployment(new string[]{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopWithMultipleSubProcess.bpmn20.xml"}) public void testShouldNotThrowErrorInLoopForMultipleSubProcess()
        [Test]
        [Deployment(new string[] { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopWithMultipleSubProcess.bpmn20.xml" })]
        public virtual void testShouldNotThrowErrorInLoopForMultipleSubProcess()
        {
            runtimeService.StartProcessInstanceByKey("looping-error");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("LoggerTask", task.Name);
            taskService.Complete(task.Id);

            Assert.AreEqual("ErrorHandlingTask", taskService.CreateTaskQuery().First().Name);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: Deployment(new string[]{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopFromCallActivityToEventSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/ThrowErrorToCallActivity.bpmn20.xml" }) public void FAILING_testShouldNotThrowErrorInLoopFromCallActivityToEventSubProcess()
        public virtual void FAILING_testShouldNotThrowErrorInLoopFromCallActivityToEventSubProcess()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("userTask", task.Name);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("ErrorLog", task.Name);
            taskService.Complete(task.Id);

            // TODO: Loop exists when error thrown from call activity to event sub process
            // as they both have different process definition - CAM-6212
            Assert.AreEqual("BoundaryEventTask", taskService.CreateTaskQuery().First().Name);
        }


    }

}