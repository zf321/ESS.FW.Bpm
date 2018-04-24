using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{

    [TestFixture]
    public class MessageEventSubprocessTest : PluggableProcessEngineTestCase
    {
        public MessageEventSubprocessTest()
        {
            //TearDownAfterEvent += tearDown;

        }
        [TearDown]
        public override void TearDown()        {
            try
            {
                base.TearDown();
            }
            finally
            {
                TestExecutionListener.Reset();
            }
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingUnderProcessDefinition()
        {
            testInterruptingUnderProcessDefinition(1);
        }

        [Test]
        [Deployment]
        public virtual void testTwoInterruptingUnderProcessDefinition()
        {
            testInterruptingUnderProcessDefinition(2);
        }

        private void testInterruptingUnderProcessDefinition(int expectedNumberOfEventSubscriptions)
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // the process instance must have a message event subscription:
            IExecution execution = runtimeService.CreateExecutionQuery(c=>c.Id == processInstance.Id)/*.MessageEventSubscriptionName("newMessage")*/.First();
            Assert.NotNull(execution);
            Assert.AreEqual(expectedNumberOfEventSubscriptions, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            // if we trigger the usertask, the process terminates and the event subscription is removed:
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // now we start a new instance but this time we trigger the event subprocess:
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.MessageEventReceived("newMessage", processInstance.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("eventSubProcessTask", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testEventSubprocessListenersInvoked()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CorrelateMessage("message");

            ITask taskInEventSubProcess = taskService.CreateTaskQuery().First();
            Assert.AreEqual("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

            taskService.Complete(taskInEventSubProcess.Id);

            IList<string> CollectedEvents = TestExecutionListener.CollectedEvents;

            Assert.AreEqual("taskInMainFlow-start", CollectedEvents[0]);
            Assert.AreEqual("taskInMainFlow-end", CollectedEvents[1]);
            Assert.AreEqual("eventSubProcess-start", CollectedEvents[2]);
            Assert.AreEqual("startEventInSubProcess-start", CollectedEvents[3]);
            Assert.AreEqual("startEventInSubProcess-end", CollectedEvents[4]);
            Assert.AreEqual("taskInEventSubProcess-start", CollectedEvents[5]);
            Assert.AreEqual("taskInEventSubProcess-end", CollectedEvents[6]);
            Assert.AreEqual("eventSubProcess-end", CollectedEvents[7]);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.Canceled).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "startEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInEventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "endEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "eventSubProcess" && c.EndTime !=null).Count());
            }

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventSubprocessListenersInvoked()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CorrelateMessage("message");

            ITask taskInMainFlow = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskInMainFlow").First();
            Assert.NotNull(taskInMainFlow);

            ITask taskInEventSubProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskInEventSubProcess").First();
            Assert.NotNull(taskInEventSubProcess);

            taskService.Complete(taskInMainFlow.Id);
            taskService.Complete(taskInEventSubProcess.Id);

            IList<string> CollectedEvents = TestExecutionListener.CollectedEvents;

            Assert.AreEqual("taskInMainFlow-start", CollectedEvents[0]);
            Assert.AreEqual("eventSubProcess-start", CollectedEvents[1]);
            Assert.AreEqual("startEventInSubProcess-start", CollectedEvents[2]);
            Assert.AreEqual("startEventInSubProcess-end", CollectedEvents[3]);
            Assert.AreEqual("taskInEventSubProcess-start", CollectedEvents[4]);
            Assert.AreEqual("taskInMainFlow-end", CollectedEvents[5]);
            Assert.AreEqual("taskInEventSubProcess-end", CollectedEvents[6]);
            Assert.AreEqual("eventSubProcess-end", CollectedEvents[7]);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "startEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInEventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "endEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "eventSubProcess" && c.EndTime !=null).Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testNestedEventSubprocessListenersInvoked()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CorrelateMessage("message");

            ITask taskInEventSubProcess = taskService.CreateTaskQuery().First();
            Assert.AreEqual("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

            taskService.Complete(taskInEventSubProcess.Id);

            IList<string> CollectedEvents = TestExecutionListener.CollectedEvents;

            Assert.AreEqual("taskInMainFlow-start", CollectedEvents[0]);
            Assert.AreEqual("taskInMainFlow-end", CollectedEvents[1]);
            Assert.AreEqual("eventSubProcess-start", CollectedEvents[2]);
            Assert.AreEqual("startEventInSubProcess-start", CollectedEvents[3]);
            Assert.AreEqual("startEventInSubProcess-end", CollectedEvents[4]);
            Assert.AreEqual("taskInEventSubProcess-start", CollectedEvents[5]);
            Assert.AreEqual("taskInEventSubProcess-end", CollectedEvents[6]);
            Assert.AreEqual("eventSubProcess-end", CollectedEvents[7]);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.Canceled).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "startEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInEventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "endEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "eventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "subProcess" && c.EndTime !=null).Count());
            }

        }

        [Test]
        [Deployment]
        public virtual void testNestedNonInterruptingEventSubprocessListenersInvoked()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CorrelateMessage("message");

            ITask taskInMainFlow = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskInMainFlow").First();
            Assert.NotNull(taskInMainFlow);

            ITask taskInEventSubProcess = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskInEventSubProcess").First();
            Assert.NotNull(taskInEventSubProcess);

            taskService.Complete(taskInMainFlow.Id);
            taskService.Complete(taskInEventSubProcess.Id);

            IList<string> CollectedEvents = TestExecutionListener.CollectedEvents;

            Assert.AreEqual("taskInMainFlow-start", CollectedEvents[0]);
            Assert.AreEqual("eventSubProcess-start", CollectedEvents[1]);
            Assert.AreEqual("startEventInSubProcess-start", CollectedEvents[2]);
            Assert.AreEqual("startEventInSubProcess-end", CollectedEvents[3]);
            Assert.AreEqual("taskInEventSubProcess-start", CollectedEvents[4]);
            Assert.AreEqual("taskInMainFlow-end", CollectedEvents[5]);
            Assert.AreEqual("taskInEventSubProcess-end", CollectedEvents[6]);
            Assert.AreEqual("eventSubProcess-end", CollectedEvents[7]);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "startEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInEventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "endEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "eventSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "subProcess" && c.EndTime !=null).Count());
            }

        }

        [Test]
        [Deployment]
        public virtual void testEventSubprocessBoundaryListenersInvoked()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CorrelateMessage("message");

            ITask taskInEventSubProcess = taskService.CreateTaskQuery().First();
            Assert.AreEqual("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

            runtimeService.CorrelateMessage("message2");

            IList<string> CollectedEvents = TestExecutionListener.CollectedEvents;


            Assert.AreEqual("taskInMainFlow-start", CollectedEvents[0]);
            Assert.AreEqual("taskInMainFlow-end", CollectedEvents[1]);
            Assert.AreEqual("eventSubProcess-start", CollectedEvents[2]);
            Assert.AreEqual("startEventInSubProcess-start", CollectedEvents[3]);
            Assert.AreEqual("startEventInSubProcess-end", CollectedEvents[4]);
            Assert.AreEqual("taskInEventSubProcess-start", CollectedEvents[5]);
            Assert.AreEqual("taskInEventSubProcess-end", CollectedEvents[6]);
            Assert.AreEqual("eventSubProcess-end", CollectedEvents[7]);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInMainFlow" && c.Canceled).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "startEventInSubProcess" && c.EndTime !=null).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "taskInEventSubProcess" && c.Canceled).Count());
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "eventSubProcess" && c.EndTime !=null).Count());
            }

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingUnderProcessDefinition()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // the process instance must have a message event subscription:
            IExecution execution = runtimeService.CreateExecutionQuery(c=>c.Id == processInstance.Id)/*.MessageEventSubscriptionName("newMessage")*/.First();
            Assert.NotNull(execution);
            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            // if we trigger the usertask, the process terminates and the event subscription is removed:
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // ###################### now we start a new instance but this time we trigger the event subprocess:
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.MessageEventReceived("newMessage", processInstance.Id);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            // now let's first complete the task in the main flow:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // we still have 2 executions (one for process instance, one for event subprocess):
            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            // now let's complete the task in the event subprocess
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // #################### again, the other way around:

            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.MessageEventReceived("newMessage", processInstance.Id);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // we still have 1 execution:
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingUnderProcessDefinitionScope()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // the process instance must have a message event subscription:
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("newMessage")*/.First();
            Assert.NotNull(execution);
            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            // if we trigger the usertask, the process terminates and the event subscription is removed:
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // ###################### now we start a new instance but this time we trigger the event subprocess:
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.CorrelateMessage("newMessage");

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());

            // now let's first complete the task in the main flow:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // we still have 2 executions (one for process instance, one for subprocess Scope):
            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            // now let's complete the task in the event subprocess
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // #################### again, the other way around:

            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.CorrelateMessage("newMessage");

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // we still have 2 executions (usertask in main flow is Scope):
            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingInEmbeddedSubprocess()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // the process instance must have a message event subscription:
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("newMessage")*/.First();
            Assert.NotNull(execution);
            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());

            // if we trigger the usertask, the process terminates and the event subscription is removed:
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // ###################### now we start a new instance but this time we trigger the event subprocess:
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.CorrelateMessage("newMessage");

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            // now let's first complete the task in the main flow:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // we still have 3 executions:
            Assert.AreEqual(3, runtimeService.CreateExecutionQuery().Count());

            // now let's complete the task in the event subprocess
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            // #################### again, the other way around:

            processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.CorrelateMessage("newMessage");

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            taskService.Complete(task.Id);
            // we still have 2 executions:
            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);
            // Done!
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testMultipleNonInterruptingInEmbeddedSubprocess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // the process instance must have a message event subscription:
            IExecution subProcess = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("newMessage")*/.First();
            Assert.NotNull(subProcess);
            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());

            ITask subProcessTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="subProcessTask").First();
            Assert.NotNull(subProcessTask);

            // start event sub process multiple times
            for (int i = 1; i < 3; i++)
            {
                runtimeService.MessageEventReceived("newMessage", subProcess.Id);

                // check that now i event sub process tasks exist
                IList<ITask> eventSubProcessTasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").ToList();
                Assert.AreEqual(i, eventSubProcessTasks.Count);
            }

            ExecutionTree executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            // check that the parent execution of the event sub process task execution is the event
            // sub process execution
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child(null)
                .Scope().Child("subProcessTask").Concurrent().NoScope().Up().Child(null).Concurrent().NoScope().Child("eventSubProcessTask")
                .Scope().Up().Up().Child(null).Concurrent().NoScope().Child("eventSubProcessTask").Scope().Done());

            // complete sub process task
            taskService.Complete(subProcessTask.Id);

            // after complete the sub process task all task should be deleted because of the terminating end event
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

            // and the process instance should be ended
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        private IQueryable<IEventSubscription> CreateEventSubscriptionQuery()
        {
            Expression<Func<EventSubscriptionEntity, bool>> expression = c => true;
            return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CreateQueryCmd<EventSubscriptionEntity>(expression));
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingInMultiParallelEmbeddedSubprocess()
        {
            // #################### I. start process and only complete the tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // assert execution tree: Scope (process) > Scope (subprocess) > 2 x subprocess + usertask
            Assert.AreEqual(6, runtimeService.CreateExecutionQuery().Count());

            // expect: two subscriptions, one for each instance
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery().Count());

            // expect: two subprocess instances, i.e. two tasks created
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            // then: complete both tasks
            foreach (ITask task in tasks)
            {
                Assert.AreEqual("subUserTask", task.TaskDefinitionKey);
                taskService.Complete(task.Id);
            }

            // expect: the event subscriptions are removed
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: complete the last task of the main process
            taskService.Complete(taskService.CreateTaskQuery().First().Id);
            AssertProcessEnded(processInstance.Id);

            // #################### II. start process and correlate messages to trigger subprocesses instantiation
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            foreach (IEventSubscription es in runtimeService.CreateEventSubscriptionQuery().ToList())
            {
                runtimeService.MessageEventReceived("message", es.ExecutionId); // trigger
            }

            // expect: both subscriptions are remaining and they can be re-triggered as long as the subprocesses are active
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery().Count());

            // expect: two additional task, one for each triggered process
            tasks = taskService.CreateTaskQuery(c=>c.Name =="Message IUser Task").ToList();
            Assert.AreEqual(2, tasks.Count);
            foreach (ITask task in tasks)
            { // complete both tasks
                taskService.Complete(task.Id);
            }

            // then: complete one subprocess
            taskService.Complete(taskService.CreateTaskQuery(c=>c.Name =="Sub IUser Task").First().Id);

            // expect: only the subscription of the second subprocess instance is left
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: trigger the second subprocess again
            runtimeService.MessageEventReceived("message", runtimeService.CreateEventSubscriptionQuery().First().ExecutionId);

            // expect: one message subprocess task exist
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.Name =="Message IUser Task").Count());

            // then: complete all inner subprocess tasks
            tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            // expect: no subscription is left
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: complete the last task of the main process
            taskService.Complete(taskService.CreateTaskQuery().First().Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingInMultiSequentialEmbeddedSubprocess()
        {
            // start process and trigger the first message sub process
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.MessageEventReceived("message", runtimeService.CreateEventSubscriptionQuery().First().ExecutionId);

            // expect: one subscription is remaining for the first instance
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: complete both tasks (subprocess and message subprocess)
            taskService.Complete(taskService.CreateTaskQuery(c=>c.Name =="Message IUser Task").First().Id);
            taskService.Complete(taskService.CreateTaskQuery(c=>c.Name =="Sub IUser Task").First().Id);

            // expect: the second instance is started
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: just complete this
            taskService.Complete(taskService.CreateTaskQuery(c=>c.Name =="Sub IUser Task").First().Id);

            // expect: no subscription is left
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            // then: complete the last task of the main process
            taskService.Complete(taskService.CreateTaskQuery().First().Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithParallelForkInsideEmbeddedSubProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.MessageEventReceived("newMessage", runtimeService.CreateEventSubscriptionQuery().First().ExecutionId);

            ExecutionTree executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child(null).Scope().Child("firstUserTask").Concurrent().NoScope().Up().Child("secondUserTask")
                .Concurrent().NoScope().Up().Child(null).Concurrent().NoScope().Child("eventSubProcessTask").Done());


            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(processInstance.Id);

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithReceiveTask()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            // check that the parent execution of the event sub process task execution is the event
            // sub process execution
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child(null).Concurrent().NoScope().Child("receiveTask").Scope().Up().Up().Child(null)
                .Concurrent().NoScope().Child("eventSubProcessTask").Scope().Done());

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="userTask").First();
            Assert.NotNull(task2);

            executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            // check that the parent execution of the event sub process task execution is the event
            // sub process execution
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child("userTask").Concurrent().NoScope().Up().Child(null)
                .Concurrent().NoScope().Child("eventSubProcessTask").Scope().Done());
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithAsyncConcurrentTask()
        {
            // given a process instance with an asyncBefore IUser task
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // and a triggered non-interrupting subprocess with a IUser task
            runtimeService.CorrelateMessage("message");

            // then triggering the async job should be successful
            IJob asyncJob = managementService.CreateJobQuery().First();
            Assert.NotNull(asyncJob);
            managementService.ExecuteJob(asyncJob.Id);

            // and there should be two tasks now that can be completed successfully
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            ITask processTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="userTask").First();
            ITask eventSubprocessTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();

            Assert.NotNull(processTask);
            Assert.NotNull(eventSubprocessTask);

            taskService.Complete(processTask.Id);
            taskService.Complete(eventSubprocessTask.Id);


            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithReceiveTaskInsideEmbeddedSubProcess()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "eventSubProcessTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task1Execution).ParentId));

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "eventSubProcessTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task1Execution).ParentId));

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="userTask").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "eventSubProcessTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task2Execution).ParentId));

            // both have the same parent (but it is not the process instance)
            Assert.True(((ExecutionEntity)task1Execution).ParentId.Equals(((ExecutionEntity)task2Execution).ParentId));

            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithUserTaskAndBoundaryEventInsideEmbeddedSubProcess()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when
            runtimeService.CorrelateMessage("newMessage");

            // then
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "eventSubProcessTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task1Execution).ParentId));

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "eventSubProcessTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task2Execution).ParentId));

            // both have the same parent (but it is not the process instance)
            Assert.True(((ExecutionEntity)task1Execution).ParentId.Equals(((ExecutionEntity)task2Execution).ParentId));

            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingOutsideEmbeddedSubProcessWithReceiveTaskInsideEmbeddedSubProcess()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").First();
            Assert.NotNull(task1);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="userTask").First();
            Assert.NotNull(task2);

            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingActivityInstanceTree()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("process");
            string ProcessInstanceId = instance.Id;

            // when
            runtimeService.CorrelateMessage("newMessage");

            // then
            IActivityInstance tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                .BeginScope("subProcess").BeginScope("eventSubProcess").Activity("eventSubProcessTask").EndScope().EndScope().Done());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingActivityInstanceTree()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("process");
            string ProcessInstanceId = instance.Id;

            // when
            runtimeService.CorrelateMessage("newMessage");

            // then
            IActivityInstance tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                .BeginScope("subProcess").Activity("innerTask").BeginScope("eventSubProcess").Activity("eventSubProcessTask").EndScope().EndScope().Done());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithTerminatingEndEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Inner IUser Task"));
            runtimeService.CorrelateMessage("message");

            ITask eventSubprocessTask = taskService.CreateTaskQuery(c=>c.Name =="Event IUser Task").First();

            Assert.That(eventSubprocessTask, Is.Not.Null);
            taskService.Complete(eventSubprocessTask.Id);

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .BeginScope("SubProcess_1").Activity("UserTask_1").EndScope().EndScope().Done());
        }

        [Test]
        [Deployment]
        public virtual void testExpressionInMessageNameInInterruptingSubProcessDefinition()
        {
            // given an process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // when receiving the message
            runtimeService.MessageEventReceived("newMessage-foo", processInstance.Id);

            // the the subprocess is triggered and we can complete the task
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("eventSubProcessTask", task.TaskDefinitionKey);
            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
        }

    }

}