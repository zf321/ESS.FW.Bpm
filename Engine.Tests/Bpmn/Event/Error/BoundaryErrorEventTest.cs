using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Error
{
    [TestFixture]
    public class BoundaryErrorEventTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public  void setUp()
        {

            // Normally the UI will do this automatically for us
            identityService.AuthenticatedUserId = "kermit";
        }
        
        [TearDown]
        public  void tearDown()
        {
            identityService.ClearAuthentication();
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnEmbeddedSubprocess()
        {
            var proc = runtimeService.StartProcessInstanceByKey("boundaryErrorOnEmbeddedSubprocess");

            // After process start, usertask in subprocess should exist
            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == proc.Id).First();
            Assert.AreEqual("subprocessTask", task.Name);

            // After task completion, error end event is reached and caught
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == proc.Id).First();
            Assert.AreEqual("task after catching the error", task.Name);
        }

        [Test]
        public virtual void testThrowErrorWithoutErrorCode()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/error/BoundaryErrorEventTest.testThrowErrorWithoutErrorCode.bpmn20.xml").Deploy();
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("'errorCode' is mandatory on errors referenced by throwing error event definitions", re.Message);
            }
        }

        [Test]
        public virtual void testThrowErrorWithEmptyErrorCode()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/error/BoundaryErrorEventTest.testThrowErrorWithEmptyErrorCode.bpmn20.xml").Deploy();
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("'errorCode' is mandatory on errors referenced by throwing error event definitions", re.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnEmbeddedSubprocessWithEmptyErrorCode()
        {
            testCatchErrorOnEmbeddedSubprocess();
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnEmbeddedSubprocessWithoutErrorCode()
        {
            testCatchErrorOnEmbeddedSubprocess();
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOfInnerSubprocessOnOuterSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("boundaryErrorTest");

            IList<ITask> tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Inner subprocess task 1", tasks[0].Name);
            Assert.AreEqual("Inner subprocess task 2", tasks[1].Name);

            // Completing task 2, will cause the end error event to throw error with code 123
            taskService.Complete(tasks[1].Id);
            tasks = taskService.CreateTaskQuery().ToList();
            ITask taskAfterError = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task outside subprocess", taskAfterError.Name);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorInConcurrentEmbeddedSubprocesses()
        {
            assertErrorCaughtInConcurrentEmbeddedSubprocesses("boundaryEventTestConcurrentSubprocesses");
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorInConcurrentEmbeddedSubprocessesThrownByScriptTask()
        {
            assertErrorCaughtInConcurrentEmbeddedSubprocesses("catchErrorInConcurrentEmbeddedSubprocessesThrownByScriptTask");
        }

        private void assertErrorCaughtInConcurrentEmbeddedSubprocesses(string ProcessDefinitionKey)
        {
            // Completing task A will lead to task D
            string procId = runtimeService.StartProcessInstanceByKey(ProcessDefinitionKey).Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("task A", tasks[0].Name);
            Assert.AreEqual("task B", tasks[1].Name);
            taskService.Complete(tasks[0].Id);
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task D", task.Name);
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);

            // Completing task B will lead to task C
            procId = runtimeService.StartProcessInstanceByKey(ProcessDefinitionKey).Id;
            tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("task A", tasks[0].Name);
            Assert.AreEqual("task B", tasks[1].Name);
            taskService.Complete(tasks[1].Id);

            tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("task A", tasks[0].Name);
            Assert.AreEqual("task C", tasks[1].Name);
            taskService.Complete(tasks[1].Id);
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task A", task.Name);

            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task D", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testDeeplyNestedErrorThrown()
        {

            // Input = 1 -> error1 will be thrown, which will destroy ALL BUT ONE
            // subprocess, which leads to an end event, which ultimately leads to ending the process instance
            string procId = runtimeService.StartProcessInstanceByKey("deeplyNestedErrorThrown").Id;
            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == procId).First();
            Assert.AreEqual("Nested task", task.Name);
            taskService.Complete(task.Id, CollectionUtil.SingletonMap("input", 1));
            AssertProcessEnded(procId);

            // Input == 2 -> error2 will be thrown, leading to a userTask outside all subprocesses
            procId = runtimeService.StartProcessInstanceByKey("deeplyNestedErrorThrown").Id;
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Nested task", task.Name);
            taskService.Complete(task.Id, CollectionUtil.SingletonMap("input", 2));
            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == procId).First();
            Assert.AreEqual("task after catch", task.Name);
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testDeeplyNestedErrorThrownOnlyAutomaticSteps()
        {
            // input == 1 -> error2 is thrown -> caught on subprocess2 -> end event in subprocess -> proc inst end 1
            string procId = runtimeService.StartProcessInstanceByKey("deeplyNestedErrorThrown", CollectionUtil.SingletonMap("input", 1)).Id;
            AssertProcessEnded(procId);

            IHistoricProcessInstance hip;
            int historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                hip = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == procId).First();
                Assert.AreEqual("processEnd1", hip.EndActivityId);
            }
            // input == 2 -> error2 is thrown -> caught on subprocess1 -> proc inst end 2
            procId = runtimeService.StartProcessInstanceByKey("deeplyNestedErrorThrown", CollectionUtil.SingletonMap("input", 1)).Id;
            AssertProcessEnded(procId);

            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                hip = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == procId).First();
                Assert.AreEqual("processEnd1", hip.EndActivityId);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorOnCallActivity-parent.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void testCatchErrorOnCallActivity()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorOnCallActivity").Id;
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("ITask in subprocess", task.Name);

            // Completing the task will reach the end error event,
            // which is caught on the call activity boundary
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated ITask", task.Name);

            // Completing the task will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorOnCallActivity-parent.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void FAILING_testCatchErrorOnCallActivityShouldEndCalledProcessProperly()
        {
            // given a process instance that has instantiated (called) a sub process instance
            var id = runtimeService.StartProcessInstanceByKey("catchErrorOnCallActivity").Id;
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("ITask in subprocess", task.Name);

            // when an error end event is triggered in the sub process instance and catched in the super process instance
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated ITask", task.Name);

            // then the called historic process instance should have properly ended
            IHistoricProcessInstance historicSubProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey == "simpleSubProcess").First();
            Assert.NotNull(historicSubProcessInstance);
            Assert.IsNull(historicSubProcessInstance.DeleteReason);
            Assert.AreEqual("theEnd", historicSubProcessInstance.EndActivityId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void testUncaughtError()
        {
            runtimeService.StartProcessInstanceByKey("simpleSubProcess");
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Task in subprocess", task.Name);

            try
            {
                // Completing the task will reach the end error event,
                // which is never caught in the process
                taskService.Complete(task.Id);
            }
            catch (BpmnError e)
            {
                AssertTextPresent("No catching boundary event found for error with errorCode 'myError', neither in same process nor in parent process", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testUncaughtErrorOnCallActivity-parent.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void testUncaughtErrorOnCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("uncaughtErrorOnCallActivity");
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Task in subprocess", task.Name);

            try
            {
                // Completing the task will reach the end error event,
                // which is never caught in the process
                taskService.Complete(task.Id);
            }
            catch (BpmnError e)
            {
                AssertTextPresent("No catching boundary event found for error with errorCode 'myError', neither in same process nor in parent process", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnSubprocess.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByCallActivityOnSubprocess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorOnSubprocess").Id;
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Task in subprocess", task.Name);

            // Completing the task will reach the end error event,
            // which is caught on the call activity boundary
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Task", task.Name);

            // Completing the task will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnCallActivity.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess2ndLevel.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByCallActivityOnCallActivity()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorOnCallActivity2ndLevel").Id;

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Task in subprocess", task.Name);

            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Task", task.Name);

            // Completing the task will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnParallelMultiInstance()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorOnParallelMi").Id;
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(5, tasks.Count);

            // Complete two subprocesses, just to make it a bit more complex
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["throwError"] = false;
            taskService.Complete(tasks[2].Id, vars);
            taskService.Complete(tasks[3].Id, vars);

            // Reach the error event
            vars["throwError"] = true;
            taskService.Complete(tasks[1].Id, vars);

            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnSequentialMultiInstance()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorOnSequentialMi").Id;

            // complete one task
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["throwError"] = false;
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id, vars);

            // complete second task and throw error
            vars["throwError"] = true;
            task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id, vars);

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownBySignallableActivityBehaviour()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownBySignallableActivityBehaviour").Id;
            Assert.NotNull("Didn't get a process id from runtime service", procId);
            IActivityInstance processActivityInstance = runtimeService.GetActivityInstance(procId);
            IActivityInstance serviceTask = processActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("Expected the service task to be active after starting the process", "serviceTask", serviceTask.ActivityId);
            runtimeService.Signal(serviceTask.ExecutionIds[0]);
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnServiceTask()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTask").Id;
            assertThatErrorHasBeenCaught(procId);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTask", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnServiceTaskNotCancelActivity()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTaskNotCancelActiviti").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnServiceTaskWithErrorCode()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTaskWithErrorCode").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcess").Id;
            assertThatErrorHasBeenCaught(procId);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcess", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcessInduction()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcessInduction").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-parent.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByJavaDelegateOnCallActivity()
        {
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-parent").Id;
            assertThatErrorHasBeenCaught(procId);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-parent", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
        public virtual void testUncaughtErrorThrownByJavaDelegateOnServiceTask()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-child");
            }
            catch (BpmnError e)
            {
                AssertTextPresent("No catching boundary event found for error with errorCode '23', neither in same process nor in parent process", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownBySignalMethodOfAbstractBpmnActivityBehavior()
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
        [Deployment]
        public virtual void testCatchExceptionExpressionThrownByFollowUpTask()
        {
            try
            {
                IDictionary<string, object> vars = ThrowErrorDelegate.throwException();
                var id = runtimeService.StartProcessInstanceByKey("testProcess", vars).Id;
                Assert.Fail("should Assert.Fail and not catch the error on the first task");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            Assert.IsNull(taskService.CreateTaskQuery().First());
        }

        [Test]
        [Deployment]
        public virtual void testCatchExceptionClassDelegateThrownByFollowUpTask()
        {
            try
            {
                IDictionary<string, object> vars = ThrowErrorDelegate.throwException();
                var id = runtimeService.StartProcessInstanceByKey("testProcess", vars).Id;
                Assert.Fail("should Assert.Fail");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            Assert.IsNull(taskService.CreateTaskQuery().First());
        }

        [Test]
        [Deployment]
        public virtual void testCatchExceptionExpressionThrownByFollowUpScopeTask()
        {
            try
            {
                IDictionary<string, object> vars = ThrowErrorDelegate.throwException();
                var id = runtimeService.StartProcessInstanceByKey("testProcess", vars).Id;
                Assert.Fail("should Assert.Fail and not catch the error on the first task");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }
            Assert.IsNull(taskService.CreateTaskQuery().First());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownBySignalMethodOfDelegateExpression()
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
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testUncaughtErrorThrownByJavaDelegateOnCallActivity-parent.bpmn20.xml", "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
        public virtual void testUncaughtErrorThrownByJavaDelegateOnCallActivity()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("uncaughtErrorThrownByJavaDelegateOnCallActivity-parent");
            }
            catch (BpmnError e)
            {
                AssertTextPresent("No catching boundary event found for error with errorCode '23', neither in same process nor in parent process", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["executionsBeforeError"] = 2;
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential", variables).Id;
            assertThatErrorHasBeenCaught(procId);

            variables["executionsBeforeError"] = 2;
            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["executionsBeforeError"] = 2;
            string procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel", variables).Id;
            assertThatErrorHasBeenCaught(procId);

            variables["executionsBeforeError"] = 2;
            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testErrorThrownByJavaDelegateNotCaughtByOtherEventType()
        {
            string procId = runtimeService.StartProcessInstanceByKey("testErrorThrownByJavaDelegateNotCaughtByOtherEventType").Id;
            assertThatErrorHasBeenCaught(procId);
        }

        private void assertThatErrorHasBeenCaught(string procId)
        {
            // The service task will throw an error event,
            // which is caught on the service task boundary
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count(),"No tasks found in task list." );
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Task", task.Name);

            // Completing the task will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        private void assertThatExceptionHasBeenCaught(string procId)
        {
            // The service task will throw an error event,
            // which is caught on the service task boundary
            Assert.AreEqual( 1, taskService.CreateTaskQuery().Count(),"No tasks found in task list.");
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalated Exception Task", task.Name);

            // Completing the task will end the process instance
            taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testConcurrentExecutionsInterruptedOnDestroyScope()
        {

            // this test makes sure that if the first concurrent execution destroys the scope
            // (due to the interrupting boundary catch), the second concurrent execution does not
            // move forward.

            // if the test fails, it produces a constraint violation in db.

            runtimeService.StartProcessInstanceByKey("process");
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByExpressionOnServiceTask()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["bpmnErrorBean"] = new BpmnErrorBean();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchErrorThrownByExpressionOnServiceTask", variables).Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByDelegateExpressionOnServiceTask()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["bpmnErrorBean"] = new BpmnErrorBean();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchErrorThrownByDelegateExpressionOnServiceTask", variables).Id;
            assertThatErrorHasBeenCaught(procId);

            variables["exceptionType"] = true;
            procId = runtimeService.StartProcessInstanceByKey("testCatchErrorThrownByDelegateExpressionOnServiceTask", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorThrownByJavaDelegateProvidedByDelegateExpressionOnServiceTask()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["bpmnErrorBean"] = new BpmnErrorBean();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchErrorThrownByJavaDelegateProvidedByDelegateExpressionOnServiceTask", variables).Id;
            assertThatErrorHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchExceptionThrownByExpressionOnServiceTask()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["bpmnErrorBean"] = new BpmnErrorBean();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchExceptionThrownByExpressionOnServiceTask", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchExceptionThrownByScriptTask()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchExceptionThrownByScriptTask", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testCatchSpecializedExceptionThrownByDelegate()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["bpmnErrorBean"] = new BpmnErrorBean();
            string procId = runtimeService.StartProcessInstanceByKey("testCatchSpecializedExceptionThrownByDelegate", variables).Id;
            assertThatExceptionHasBeenCaught(procId);
        }

        [Test]
        [Deployment]
        public virtual void testUncaughtRuntimeException()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("testUncaughtRuntimeException");
                Assert.Fail("error should not be caught");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("This should not be caught!", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testUncaughtBusinessExceptionWrongErrorCode()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("testUncaughtBusinessExceptionWrongErrorCode");
                Assert.Fail("error should not be caught");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("couldn't execute activity <serviceTask id=\"serviceTask\" ..>: Business Exception", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnSubprocessThrownByNonInterruptingEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IEventSubscription messageSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            runtimeService.MessageEventReceived("message", messageSubscription.ExecutionId);

            // should successfully have reached the task following the boundary event
            IExecution taskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "afterBoundaryTask").First();
            Assert.NotNull(taskExecution);
            ITask task = taskService.CreateTaskQuery(c=>c.Id == taskExecution.Id).First();
            Assert.NotNull(task);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnSubprocessThrownByInterruptingEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IEventSubscription messageSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            runtimeService.MessageEventReceived("message", messageSubscription.ExecutionId);

            // should successfully have reached the task following the boundary event
            IExecution taskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "afterBoundaryTask").First();
            Assert.NotNull(taskExecution);
            ITask task = taskService.CreateTaskQuery(c=>c.Id == taskExecution.Id).First();
            Assert.NotNull(task);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnSubprocessThrownByNestedEventSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            // trigger outer event subprocess
            IEventSubscription messageSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            runtimeService.MessageEventReceived("outerMessage", messageSubscription.ExecutionId);

            // trigger inner event subprocess
            messageSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            runtimeService.MessageEventReceived("innerMessage", messageSubscription.ExecutionId);

            // should successfully have reached the task following the boundary event
            IExecution taskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "afterBoundaryTask").First();
            Assert.NotNull(taskExecution);
            ITask task = taskService.CreateTaskQuery(c=>c.Id == taskExecution.Id).First();
            Assert.NotNull(task);
        }

        [Test]
        [Deployment]
        public virtual void testCatchErrorOnSubprocessSetsErrorVariables()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorVariable";
            object errorCode = "error1";

            checkErrorVariable(VariableName, errorCode);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/ThrowErrorProcess.bpmn", "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnSubprocessSetsErrorCodeVariable.bpmn" })]
        public virtual void testCatchErrorThrownByCallActivityOnSubprocessSetsErrorVariables()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorVariable";
            //the code we gave the thrown error
            object errorCode = "error";

            checkErrorVariable(VariableName, errorCode);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByMultiInstanceSubProcessSetsErrorCodeVariable.bpmn" })]
        public virtual void testCatchErrorThrownByMultiInstanceSubProcessSetsErrorVariables()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorVariable";
            //the code we gave the thrown error
            object errorCode = "error";

            checkErrorVariable(VariableName, errorCode);
        }

        private void checkErrorVariable(string VariableName, object expectedValue)
        {
            IVariableInstance errorVariable = runtimeService.CreateVariableInstanceQuery()/*.VariableName(VariableName)*/.First();
            Assert.That(errorVariable, Is.Not.Null);
            Assert.That(errorVariable, Is.Not.Null);
            Assert.That(errorVariable.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/BoundaryErrorEventTest.testCatchBpmnErrorThrownByJavaDelegateInCallActivityOnSubprocessSetsErrorVariables.bpmn", "resources/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
        public virtual void testCatchBpmnErrorThrownByJavaDelegateInCallActivityOnSubprocessSetsErrorVariables()
        {
            runtimeService.StartProcessInstanceByKey("Process_1");
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);
            //the name used in "camunda:errorCodeVariable" in the BPMN
            string VariableName = "errorCode";
            //the code we gave the thrown error
            object errorCode = "errorCode";
            checkErrorVariable(VariableName, errorCode);
            checkErrorVariable("errorMessageVariable", "ouch!");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/error/reviewSalesLead.bpmn20.xml" })]
        public virtual void testReviewSalesLeadProcess()
        {

            // After starting the process, a task should be assigned to the 'initiator' (normally set by GUI)
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["details"] = "very interesting";
            variables["customerName"] = "Alfresco";
            string procId = runtimeService.StartProcessInstanceByKey("reviewSaledLead", variables).Id;
            ITask task = taskService.CreateTaskQuery(c=>c.Assignee == "kermit").First();
            Assert.AreEqual("Provide new sales lead", task.Name);

            // After completing the task, the review subprocess will be active
            taskService.Complete(task.Id);
            ITask ratingTask = taskService.CreateTaskQuery()/*.TaskCandidateGroup("accountancy")*/.First();
            Assert.AreEqual("Review customer rating", ratingTask.Name);
            ITask profitabilityTask = taskService.CreateTaskQuery()/*.TaskCandidateGroup("management")*/.First();
            Assert.AreEqual("Review profitability", profitabilityTask.Name);

            // Complete the management task by stating that not enough info was provided
            // This should throw the error event, which closes the subprocess
            variables = new Dictionary<string, object>();
            variables["notEnoughInformation"] = true;
            taskService.Complete(profitabilityTask.Id, variables);

            // The 'provide additional details' task should now be active
            ITask provideDetailsTask = taskService.CreateTaskQuery(c=>c.Assignee == "kermit").First();
            Assert.AreEqual("Provide additional details", provideDetailsTask.Name);

            // Providing more details (ie. completing the task), will activate the subprocess again
            taskService.Complete(provideDetailsTask.Id);
            IList<ITask> reviewTasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual("Review customer rating", reviewTasks[0].Name);
            Assert.AreEqual("Review profitability", reviewTasks[1].Name);

            // Completing both tasks normally ends the process
            taskService.Complete(reviewTasks[0].Id);
            variables["notEnoughInformation"] = false;
            taskService.Complete(reviewTasks[1].Id, variables);
            AssertProcessEnded(procId);
        }
    }

}