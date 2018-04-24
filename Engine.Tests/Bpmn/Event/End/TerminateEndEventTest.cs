using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.End
{
    [TestFixture]
    public class TerminateEndEventTest : PluggableProcessEngineTestCase
    {

        public static int serviceTaskInvokedCount = 0;

        public class CountDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                serviceTaskInvokedCount++;

                // leave only 3 out of n subprocesses
                execution.SetVariableLocal("terminate", serviceTaskInvokedCount > 3);
            }

            ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            ////ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
            //public virtual void execute(IDelegateExecution execution)
            //{
            //    serviceTaskInvokedCount++;

            //    // leave only 3 out of n subprocesses
            //    execution.SetVariableLocal("terminate", serviceTaskInvokedCount > 3);
            //}
        }

        public static int serviceTaskInvokedCount2 = 0;

        public class CountDelegate2 : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                serviceTaskInvokedCount2++;
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
            //public virtual void execute(IDelegateExecution execution)
            //{
            //    serviceTaskInvokedCount2++;
            //}
        }

        [Test]
        [Deployment]
        public virtual void testProcessTerminate()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long executionEntities = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi.Id).Count();
            Assert.AreEqual(3, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preTerminateTask").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTerminateWithSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the process and
            long executionEntities = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi.Id).Count();
            Assert.AreEqual(4, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preTerminateEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateWithCallActivity.bpmn", "resources/bpmn/event/end/TerminateEndEventTest.subProcessNoTerminate.bpmn" })]
        public virtual void testTerminateWithCallActivity()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long executionEntities = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi.Id).Count();
            Assert.AreEqual(4, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preTerminateEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcess()
        {
            serviceTaskInvokedCount = 0;

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the subprocess and continue the parent
            long executionEntities = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==pi.Id).Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessShouldNotInvokeProcessEndListeners()
        {
            RecorderExecutionListener.Clear();

            // when process instance is started and terminate end event in subprocess executed
            runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // then the outer task still exists
            ITask outerTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(outerTask);
            Assert.AreEqual("outerTask", outerTask.TaskDefinitionKey);

            // and the process end listener was not invoked
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessConcurrentShouldNotInvokeProcessEndListeners()
        {
            RecorderExecutionListener.Clear();

            // when process instance is started and terminate end event in subprocess executed
            runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // then the outer task still exists
            ITask outerTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(outerTask);
            Assert.AreEqual("outerTask", outerTask.TaskDefinitionKey);

            // and the process end listener was not invoked
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcess.bpmn" })]
        public virtual void testTerminateInSubProcessShouldNotEndProcessInstanceInHistory()
        {
            // when process instance is started and terminate end event in subprocess executed
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // then the historic process instance should not appear ended
            AssertProcessNotEnded(pi.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery().First();

                Assert.NotNull(hpi);
                Assert.IsNull(hpi.EndTime);
                Assert.IsNull(hpi.DurationInMillis);
                Assert.IsNull(hpi.DeleteReason);
            }
        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessConcurrent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcessConcurrent.bpmn" })]
        public virtual void testTerminateInSubProcessConcurrentShouldNotEndProcessInstanceInHistory()
        {
            // when process instance is started and terminate end event in subprocess executed
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // then the historic process instance should not appear ended
            AssertProcessNotEnded(pi.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery().First();

                Assert.NotNull(hpi);
                Assert.IsNull(hpi.EndTime);
                Assert.IsNull(hpi.DurationInMillis);
                Assert.IsNull(hpi.DeleteReason);
            }
        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessConcurrentMultiInstance()
        {
            serviceTaskInvokedCount = 0;

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(12, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            long executionEntities2 = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(10, executionEntities2);

            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask t in tasks)
            {
                taskService.Complete(t.Id);
            }

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessMultiInstance()
        {
            serviceTaskInvokedCount = 0;

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }


        [Test]
        [Deployment]
        public virtual void testTerminateInSubProcessSequentialConcurrentMultiInstance()
        {
            serviceTaskInvokedCount = 0;
            serviceTaskInvokedCount2 = 0;

            // Starting multi instance with 5 instances; terminating 2, finishing 3
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            long remainingExecutions = runtimeService.CreateExecutionQuery().Count();

            // outer execution still available
            Assert.AreEqual(1, remainingExecutions);

            // three finished
            Assert.AreEqual(3, serviceTaskInvokedCount2);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id && c.TaskDefinitionKey == "preNormalEnd").First();
            taskService.Complete(task.Id);

            // last task remaining
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivity.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" })]
        public virtual void testTerminateInCallActivity()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the called process and continue the parent
            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityMulitInstance.bpmn", "resources/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" })]
        public virtual void testTerminateInCallActivityMulitInstance()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the called process and continue the parent
            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrent.bpmn", "resources/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" })]
        public virtual void testTerminateInCallActivityConcurrent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the called process and continue the parent
            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrentMulitInstance.bpmn", "resources/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" })]
        public virtual void testTerminateInCallActivityConcurrentMulitInstance()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("terminateEndEventExample");

            // should terminate the called process and continue the parent
            long executionEntities = runtimeService.CreateExecutionQuery().Count();
            Assert.AreEqual(1, executionEntities);

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id&& c.TaskDefinitionKey=="preNormalEnd").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }
    }
}