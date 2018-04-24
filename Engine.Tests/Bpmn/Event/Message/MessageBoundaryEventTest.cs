using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{
    [TestFixture]
    public class MessageBoundaryEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testSingleBoundaryMessageEvent()
        {
            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);

            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution);

            // 1. case: message received cancels the task

            runtimeService.MessageEventReceived("messageName", execution.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // 2nd. case: complete the User task cancels the message subscription

            runtimeService.StartProcessInstanceByKey("process");

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

        }

        public virtual void testDoubleBoundaryMessageEventSameMessageId()
        {
            // deployment fails when two boundary message events have the same messageId
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageBoundaryEventTest.testDoubleBoundaryMessageEventSameMessageId.bpmn20.xml").Deploy();
                Assert.Fail("Deployment should Assert.Fail because Activiti cannot handle two boundary message events with same messageId.");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Cannot have more than one message event subscription with name 'messageName' for scope 'task'", e.Message);
                Assert.AreEqual(0, repositoryService.CreateDeploymentQuery().Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testDoubleBoundaryMessageEvent()
        {
            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);

            // the executions for both messageEventSubscriptionNames are the same
            IExecution execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
            Assert.NotNull(execution1);

            IExecution execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
            Assert.NotNull(execution2);

            Assert.AreEqual(execution1.Id, execution2.Id);

            ///////////////////////////////////////////////////////////////////////////////////
            // 1. first message received cancels the task and the execution and both subscriptions
            runtimeService.MessageEventReceived("messageName_1", execution1.Id);

            // this should then throw an exception because execution2 no longer exists
            try
            {
                runtimeService.MessageEventReceived("messageName_2", execution2.Id);
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("does not have a subscription to a message event with name 'messageName_2'", e.Message);
            }

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage_1", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            /////////////////////////////////////////////////////////////////////
            // 2. complete the IUser task cancels the message subscriptions

            runtimeService.StartProcessInstanceByKey("process");

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
            Assert.IsNull(execution1);
            execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
            Assert.IsNull(execution2);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testDoubleBoundaryMessageEventMultiInstance()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            // assume we have 7 executions
            // one process instance
            // one execution for scope created for boundary message event
            // five execution because we have loop cardinality 5
            Assert.AreEqual(7, runtimeService.CreateExecutionQuery().Count());

            Assert.AreEqual(5, taskService.CreateTaskQuery().Count());

            IExecution execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
            IExecution execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
            // both executions are the same
            Assert.AreEqual(execution1.Id, execution2.Id);

            ///////////////////////////////////////////////////////////////////////////////////
            // 1. first message received cancels all tasks and the executions and both subscriptions
            runtimeService.MessageEventReceived("messageName_1", execution1.Id);

            // this should then throw an exception because execution2 no longer exists
            try
            {
                runtimeService.MessageEventReceived("messageName_2", execution2.Id);
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("does not have a subscription to a message event with name 'messageName_2'", e.Message);
            }

            // only process instance left
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage_1", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            AssertProcessEnded(processInstance.Id);


            ///////////////////////////////////////////////////////////////////////////////////
            // 2. complete the IUser task cancels the message subscriptions

            processInstance = runtimeService.StartProcessInstanceByKey("process");
            // assume we have 7 executions
            // one process instance
            // one execution for scope created for boundary message event
            // five execution because we have loop cardinality 5
            Assert.AreEqual(7, runtimeService.CreateExecutionQuery().Count());

            Assert.AreEqual(5, taskService.CreateTaskQuery().Count());

            execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
            execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
            // both executions are the same
            Assert.AreEqual(execution1.Id, execution2.Id);

            IList<ITask> userTasks = taskService.CreateTaskQuery().ToList();
            Assert.NotNull(userTasks);
            Assert.AreEqual(5, userTasks.Count);

            // as long as tasks exists, the message subscriptions exist
            for (int i = 0; i < userTasks.Count - 1; i++)
            {
                ITask task = userTasks[i];
                taskService.Complete(task.Id);

                execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
                Assert.NotNull(execution1);
                execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
                Assert.NotNull(execution2);
            }

            // only one task left
            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            // after last task is completed, no message subscriptions left
            execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_1")*/.First();
            Assert.IsNull(execution1);
            execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_2")*/.First();
            Assert.IsNull(execution2);

            // complete last task to end process
            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventInsideSubprocess()
        {

            // this time the boundary events are placed on a IUser task that is contained inside a sub process

            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(3, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);

            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution);

            ///////////////////////////////////////////////////
            // 1. case: message received cancels the task

            runtimeService.MessageEventReceived("messageName", execution.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            ///////////////////////////////////////////////////
            // 2nd. case: complete the IUser task cancels the message subscription

            runtimeService.StartProcessInstanceByKey("process");

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventOnSubprocessAndInsideSubprocess()
        {

            // this time the boundary events are placed on a IUser task that is contained inside a sub process
            // and on the subprocess itself

            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(3, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);

            IExecution execution1 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution1);

            IExecution execution2 = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.NotNull(execution2);

            //assertNotSame(execution1.Id, execution2.Id);
            Assert.AreNotEqual(execution1.Id, execution2.Id);

            /////////////////////////////////////////////////////////////
            // first case: we complete the inner usertask.

            taskService.Complete(userTask.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);

            // the inner subscription is cancelled
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            // the outer subscription still exists
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.NotNull(execution);

            // now complete the second usertask
            taskService.Complete(userTask.Id);

            // now the outer event subscription is cancelled as well
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterSubprocess", userTask.TaskDefinitionKey);

            // now complete the outer usertask
            taskService.Complete(userTask.Id);

            /////////////////////////////////////////////////////////////
            // second case: we signal the inner message event

            runtimeService.StartProcessInstanceByKey("process");

            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            runtimeService.MessageEventReceived("messageName", execution.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);

            // the inner subscription is removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            // the outer subscription still exists
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.NotNull(execution);

            // now complete the second usertask
            taskService.Complete(userTask.Id);

            // now the outer event subscription is cancelled as well
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterSubprocess", userTask.TaskDefinitionKey);

            // now complete the outer usertask
            taskService.Complete(userTask.Id);

            /////////////////////////////////////////////////////////////
            // third case: we signal the outer message event

            runtimeService.StartProcessInstanceByKey("process");

            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            runtimeService.MessageEventReceived("messageName2", execution.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterOuterMessageBoundary", userTask.TaskDefinitionKey);

            // the inner subscription is removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            // the outer subscription is removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.First();
            Assert.IsNull(execution);

            // now complete the second usertask
            taskService.Complete(userTask.Id);

            // and we are done

        }


        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventOnSubprocess()
        {
            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);

            // 1. case: message one received cancels the task

            IExecution executionMessageOne = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_one")*/.First();
            Assert.NotNull(executionMessageOne);

            runtimeService.MessageEventReceived("messageName_one", executionMessageOne.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage_one", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // 2nd. case: message two received cancels the task

            runtimeService.StartProcessInstanceByKey("process");

            IExecution executionMessageTwo = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_two")*/.First();
            Assert.NotNull(executionMessageTwo);

            runtimeService.MessageEventReceived("messageName_two", executionMessageTwo.Id);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage_two", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());


            // 3rd. case: complete the IUser task cancels the message subscription

            runtimeService.StartProcessInstanceByKey("process");

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            executionMessageOne = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_one")*/.First();
            Assert.IsNull(executionMessageOne);

            executionMessageTwo = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName_two")*/.First();
            Assert.IsNull(executionMessageTwo);

            userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterSubProcess", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

        }

        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventOnSubprocessWithIntermediateMessageCatch()
        {

            // given
            // a process instance waiting inside the intermediate message catch inside the subprocess
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // when
            // I cancel the subprocess
            runtimeService.CorrelateMessage("cancelMessage");

            // then
            // the process instance is ended
            AssertProcessEnded(processInstance.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // and all activity instances in history have an end time set
                IList<IHistoricActivityInstance> hais = historyService.CreateHistoricActivityInstanceQuery().ToList();
                foreach (IHistoricActivityInstance historicActivityInstance in hais)
                {
                    Assert.NotNull(historicActivityInstance.EndTime);
                }
            }
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventOnSubprocessAndInsideSubprocessMultiInstance()
        {

            // this time the boundary events are placed on a IUser task that is contained inside a sub process
            // and on the subprocess itself

            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(17, runtimeService.CreateExecutionQuery().Count());

            // 5 IUser tasks
            IList<ITask> userTasks = taskService.CreateTaskQuery().ToList();
            Assert.NotNull(userTasks);
            Assert.AreEqual(5, userTasks.Count);

            // there are 5 event subscriptions to the event on the inner IUser task
            IList<IExecution> executions = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.ToList();
            Assert.NotNull(executions);
            Assert.AreEqual(5, executions.Count);

            // there is a single event subscription for the event on the subprocess
            executions = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName2")*/.ToList();
            Assert.NotNull(executions);
            Assert.AreEqual(1, executions.Count);

            // if we complete the outer message event, all inner executions are removed
            IExecution outerScopeExecution = executions[0];
            runtimeService.MessageEventReceived("messageName2", outerScopeExecution.Id);

            executions = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.ToList();
            Assert.AreEqual(0, executions.Count);

            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterOuterMessageBoundary", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);

            // and we are done

        }

        [Test]
        [Deployment]
        public virtual void testBoundaryMessageEventConcurrent()
        {
            runtimeService.StartProcessInstanceByKey("boundaryEvent");

            IEventSubscription eventSubscriptionTask1 = runtimeService.CreateEventSubscriptionQuery(c=>c.ActivityId == "messageBoundary1").First();
            Assert.NotNull(eventSubscriptionTask1);

            IEventSubscription eventSubscriptionTask2 = runtimeService.CreateEventSubscriptionQuery(c=>c.ActivityId == "messageBoundary2").First();
            Assert.NotNull(eventSubscriptionTask2);

            // when I trigger the boundary event for task1
            runtimeService.CorrelateMessage("task1Message");

            // then the event subscription for task2 still exists
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());
            Assert.NotNull(runtimeService.CreateEventSubscriptionQuery(c=>c.ActivityId == "messageBoundary2").First());

        }

        [Test]
        [Deployment]
        public virtual void testExpressionInBoundaryMessageEventName()
        {

            // given a process instance with its variables
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";
            runtimeService.StartProcessInstanceByKey("process", variables);


            // when message is received
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName-bar")*/.First();
            Assert.NotNull(execution);
            runtimeService.MessageEventReceived("messageName-bar", execution.Id);

            // then then a task should be completed
            ITask userTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

        }

    }

}