using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SubProcess.Transaction
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class TransactionSubProcessTest : PluggableProcessEngineTestCase
    {
        protected internal virtual IList<IExecution> collectExecutionsFor(params string[] activityIds)
        {
            IList<IExecution> executions = new List<IExecution>();

            foreach (var ActivityId in activityIds)
                ((List<IExecution>)executions).AddRange(
                    runtimeService.CreateExecutionQuery(c => c.ActivityId == ActivityId)
                        .ToList());

            return executions;
        }


        [Test]
        [Deployment("resources/bpmn/subprocess/transaction/TransactionSubProcessTest.testSimpleCase.bpmn20.xml")]
        public virtual void TestActivityInstanceTreeAfterSuccessfulCompletion()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // the tx task is present
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // making the tx succeed
            taskService.SetVariable(task.Id, "confirmed", true);
            taskService.Complete(task.Id);

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("afterSuccess")
                    .Done());
        }


        [Test]
        [Deployment]
        public virtual void TestCancelBoundaryNoTransactionFails()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/bpmn/subprocess/transaction/TransactionSubProcessTest.TestCancelBoundaryNoTransactionFails.bpmn20.xml")
                    .DeployAndReturnDefinitions();
                //Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                if (!e.Message.Contains("boundary event with cancelEventDefinition only supported on transaction subprocesses"))
                {
                    Assert.Fail("different exception expected");
                }
            }
        }


        [Test]
        [Deployment]
        public virtual void TestCancelEndConcurrent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // the task is present:
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // making the tx Assert.Fail:
            taskService.SetVariable(task.Id, "confirmed", false);
            taskService.Complete(task.Id);

            // we have no more compensate event subscriptions
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "compensate")
                .Count());

            // Assert that the compensation handlers have been invoked:
            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));

            // Signal compensation handler completion
            var compensationHandlerExecutions = collectExecutionsFor("undoBookHotel", "undoBookFlight");
            foreach (var execution in compensationHandlerExecutions)
                runtimeService.Signal(execution.Id);

            // now the process instance execution is sitting in the 'afterCancellation' task
            // -> has left the transaction using the cancel boundary event
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterCancellation"));

            // if we have history, we check that the invocation of the compensation handlers is recorded in history.
            if (!processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelNone))
            {
                Assert.AreEqual(1,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookFlight")
                        .Count());

                Assert.AreEqual(5,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                        .Count());
            }

            // end the process instance
            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }


        [Test]
        [Deployment]
        public virtual void TestCancelEndNoTransactionFails()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/bpmn/subprocess/transaction/TransactionSubProcessTest.TestCancelEndNoTransactionFails.bpmn20.xml")
                    .Deploy();
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                if (
                    !e.Message.Contains(
                        "end event with cancelEventDefinition only supported inside transaction subprocess"))
                    Assert.Fail("different exception expected");
            }
        }


        [Test]
        [Deployment]
        public virtual void TestCompensateSubprocess()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("txProcess");

            var innerTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(innerTask.Id);

            // when the transaction is cancelled
            runtimeService.SetVariable(instance.Id, "cancelTx", true);
            runtimeService.SetVariable(instance.Id, "compensate", false);
            var beforeCancelTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCancelTask.Id);

            // then compensation is triggered
            var compensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(compensationTask);
            Assert.AreEqual("undoInnerTask", compensationTask.TaskDefinitionKey);
            taskService.Complete(compensationTask.Id);

            // and the process instance ends successfully
            var afterBoundaryTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterCancel", afterBoundaryTask.TaskDefinitionKey);
            taskService.Complete(afterBoundaryTask.Id);
            AssertProcessEnded(instance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestCompensateSubprocessAfterTxCompletion()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("txProcess");

            var innerTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(innerTask.Id);

            // when the transaction is not cancelled
            runtimeService.SetVariable(instance.Id, "cancelTx", false);
            runtimeService.SetVariable(instance.Id, "compensate", true);
            var beforeTxEndTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeTxEndTask.Id);

            // but when compensation is thrown after the tx has completed successfully
            var afterTxTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(afterTxTask.Id);

            // then compensation for the subprocess is triggered
            var compensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(compensationTask);
            Assert.AreEqual("undoInnerTask", compensationTask.TaskDefinitionKey);
            taskService.Complete(compensationTask.Id);

            // and the process has ended
            AssertProcessEnded(instance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestCompensateSubprocessNotTriggered()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("txProcess");

            var innerTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(innerTask.Id);

            // when the transaction is not cancelled
            runtimeService.SetVariable(instance.Id, "cancelTx", false);
            runtimeService.SetVariable(instance.Id, "compensate", false);
            var beforeEndTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeEndTask.Id);

            // then
            var afterTxTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterTx", afterTxTask.TaskDefinitionKey);

            // and the process has ended
            taskService.Complete(afterTxTask.Id);
            AssertProcessEnded(instance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestCompensateTransactionWithEventSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("txProcess");
            var beforeCancelTask = taskService.CreateTaskQuery()
                .First();

            // when the transaction is cancelled and handled by an event subprocess
            taskService.Complete(beforeCancelTask.Id);

            // then completing compensation works
            var compensationHandler = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(compensationHandler);
            Assert.AreEqual("blackBoxCompensationHandler", compensationHandler.TaskDefinitionKey);

            taskService.Complete(compensationHandler.Id);

            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestCompensateTransactionWithEventSubprocessActivityInstanceTree()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("txProcess");
            var beforeCancelTask = taskService.CreateTaskQuery()
                .First();

            // when the transaction is cancelled and handled by an event subprocess
            taskService.Complete(beforeCancelTask.Id);

            // then the Activity instance tree is correct
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            //ActivityInstanceAssert.That(tree).HasStructure,,,,代码需要调整，没有找到HasStructure
            Assert.That(
                tree.Equals(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("tx")
                    .Activity("cancelEnd")
                    .BeginScope("innerSubProcess")
                    .Activity("blackBoxCompensationHandler")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessThrowCompensation")
                    .Done()));
        }

        /*
         * The cancel end event cancels all instances, compensation is performed for all instances
         *
         * see spec page 470:
         * "If the cancelActivity attribute is set, the Activity the IEvent is attached to is then
         * cancelled (in case of a multi-instance, all its instances are cancelled);"
         */


        [Test]
        [Deployment]
        public virtual void TestMultiInstanceTx()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // there are now 5 instances of the transaction:
            //IList<IEventSubscription>
            var eventSubscriptions = runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "compensate")
                .ToList();

            // there are 10 compensation event subscriptions
            Assert.AreEqual(10, eventSubscriptions.Count());

            var task = taskService.CreateTaskQuery() /*.ListPage(0, 1)*/
                .ToList()
                .FirstOrDefault();

            // canceling one instance triggers compensation for all other instances:
            taskService.SetVariable(task.Id, "confirmed", false);
            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));

            runtimeService.Signal(runtimeService.CreateExecutionQuery(c => c.ActivityId == "afterCancellation")
                .First()
                .Id);

            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment]
        public virtual void testMultiInstanceTx/*Successful*/()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // there are now 5 instances of the transaction:
            //IList<IEventSubscription>
            var eventSubscriptions = runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "compensate")
                .ToList();

            // there are 10 compensation event subscriptions
            Assert.AreEqual(10, eventSubscriptions.Count);

            // first complete the inner user-tasks
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            foreach (var task in tasks)
            {
                taskService.SetVariable(task.Id, "confirmed", true);
                taskService.Complete(task.Id);
            }

            // now complete the inner receive tasks
            var executions = runtimeService.CreateExecutionQuery(c => c.ActivityId == "receive")
                .ToList();
            foreach (var execution in executions)
                runtimeService.Signal(execution.Id);

            runtimeService.Signal(runtimeService.CreateExecutionQuery(c => c.ActivityId == "afterSuccess")
                .First()
                .Id);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery()
                .Count());
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestMultipleCancelBoundaryFails()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/bpmn/subprocess/transaction/TransactionSubProcessTest.TestMultipleCancelBoundaryFails.bpmn20.xml")
                    .Deploy();
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                if (
                    !e.Message.Contains(
                        "multiple boundary events with cancelEventDefinition not supported on same transaction"))
                    Assert.Fail("different exception expected");
            }
        }

        [Test]
        [Deployment]
        public virtual void TestNestedCancelInner()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookFlight")
                    .Count());

            // the tasks are present:
            var taskInner = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "innerTxaskCustomer")
                .First();
            var taskOuter = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "bookFlight")
                .First();
            Assert.NotNull(taskInner);
            Assert.NotNull(taskOuter);

            // making the tx Assert.Fail:
            taskService.SetVariable(taskInner.Id, "confirmed", false);
            taskService.Complete(taskInner.Id);

            // we have no more compensate event subscriptions for the inner tx
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookHotel")
                    .Count());
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookFlight")
                    .Count());

            // we do not have a subscription or the outer tx yet
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // Assert that the compensation handlers have been invoked:
            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "innerTxundoBookHotel"));
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "innerTxundoBookFlight"));

            // Signal compensation handler completion
            var compensationHandlerExecutions = collectExecutionsFor("innerTxundoBookFlight", "innerTxundoBookHotel");
            foreach (var execution in compensationHandlerExecutions)
                runtimeService.Signal(execution.Id);

            // now the process instance execution is sitting in the 'afterInnerCancellation' task
            // -> has left the transaction using the cancel boundary event
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterInnerCancellation"));

            // if we have history, we check that the invocation of the compensation handlers is recorded in history.
            if (!processEngineConfiguration.History.Equals(processEngineConfiguration.History)) //.HISTORY_NONE))
            {
                Assert.AreEqual(5,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "innerTxundoBookHotel")
                        .Count());

                Assert.AreEqual(1,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "innerTxundoBookFlight")
                        .Count());
            }

            // complete the task in the outer tx
            taskService.Complete(taskOuter.Id);

            // end the process instance (Signal the execution still sitting in afterInnerCancellation)
            runtimeService.Signal(runtimeService.CreateExecutionQuery(c => c.ActivityId == "afterInnerCancellation")
                .First()
                .Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }


        [Test]
        [Deployment]
        public virtual void TestNestedCancelOuter()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookFlight")
                    .Count());

            // the tasks are present:
            var taskInner = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "innerTxaskCustomer")
                .First();
            var taskOuter = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "bookFlight")
                .First();
            Assert.NotNull(taskInner);
            Assert.NotNull(taskOuter);

            // making the outer tx Assert.Fail (invokes cancel end event)
            taskService.Complete(taskOuter.Id);

            // now the process instance is sitting in 'afterOuterCancellation'
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterOuterCancellation"));

            // we have no more compensate event subscriptions
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookHotel")
                    .Count());
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "innerTxundoBookFlight")
                    .Count());
            Assert.AreEqual(0,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // the compensation handlers of the inner tx have not been invoked
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "innerTxundoBookHotel"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "innerTxundoBookFlight"));

            // the compensation handler in the outer tx has been invoked
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));

            // end the process instance (Signal the execution still sitting in afterOuterCancellation)
            runtimeService.Signal(runtimeService.CreateExecutionQuery(c => c.ActivityId == "afterOuterCancellation")
                .First()
                .Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }


        [Test]
        [Deployment]
        public virtual void TestParseWithDI()
        {
            // this Test simply makes sure we can parse a transaction subprocess with DI information
            // the actual transaction behavior is Tested by other Testcases

            //// failing case

            var processInstance = runtimeService.StartProcessInstanceByKey("TransactionSubProcessTest");

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.SetVariable(task.Id, "confirmed", false);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);


            ////// success case

            processInstance = runtimeService.StartProcessInstanceByKey("TransactionSubProcessTest");

            task = taskService.CreateTaskQuery()
                .First();
            taskService.SetVariable(task.Id, "confirmed", true);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestSimpleCaseTxCancelled()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // the task is present:
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // making the tx Assert.Fail:
            taskService.SetVariable(task.Id, "confirmed", false);
            taskService.Complete(task.Id);

            // we have no more compensate event subscriptions
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "compensate")
                .Count());

            // Assert that the compensation handlers have been invoked:
            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));
            Assert.AreEqual(1, runtimeService.GetVariable(processInstance.Id, "undoChargeCard"));

            // Signal compensation handler completion
            var compensationHandlerExecutions = collectExecutionsFor("undoBookHotel", "undoBookFlight", "undoChargeCard");
            foreach (var execution in compensationHandlerExecutions)
                runtimeService.Signal(execution.Id);

            // now the process instance execution is sitting in the 'afterCancellation' task
            // -> has left the transaction using the cancel boundary event
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterCancellation"));

            // if we have history, we check that the invocation of the compensation handlers is recorded in history.
            if (!processEngineConfiguration.History.Equals(processEngineConfiguration.History)) //HISTORY_NONE))
            {
                Assert.AreEqual(1,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookFlight")
                        .Count());

                Assert.AreEqual(5,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                        .Count());

                Assert.AreEqual(1,
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoChargeCard")
                        .Count());
            }

            // end the process instance
            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }


        [Test]
        [Deployment]
        public virtual void TestSimpleCaseTxSuccessful()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // the task is present:
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // making the tx succeed:
            taskService.SetVariable(task.Id, "confirmed", true);
            taskService.Complete(task.Id);

            // now the process instance execution is sitting in the 'afterSuccess' task
            // -> has left the transaction using the "normal" sequence flow
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterSuccess"));

            // there is a compensate event subscription for the transaction under the process instance
            var eventSubscriptionEntity =
                (EventSubscriptionEntity)
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "tx" && c.ExecutionId == processInstance.Id)
                    .First();

            // there is an event-scope execution associated with the event-subscription:
            Assert.NotNull(eventSubscriptionEntity.Configuration);
            var eventScopeExecution =
                runtimeService.CreateExecutionQuery(c => c.Id == eventSubscriptionEntity.Configuration)
                    .First();
            Assert.NotNull(eventScopeExecution);

            // there is a compensate event subscription for the miBody of 'bookHotel' Activity
            var miBodyEventSubscriptionEntity =
                (EventSubscriptionEntity)
                runtimeService.CreateEventSubscriptionQuery(
                        c =>
                            c.EventType == "compensate" &&
                            c.ActivityId == "bookHotel" + BpmnParse.MultiInstanceBodyIdSuffix &&
                            c.ExecutionId == eventScopeExecution.Id)
                    .First();
            Assert.NotNull(miBodyEventSubscriptionEntity);
            var miBodyEventScopeExecutionId = miBodyEventSubscriptionEntity.Configuration;

            // we still have compensate event subscriptions for the compensation handlers, only now they are part of the event scope
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c =>
                            c.EventType == "compensate" && c.ActivityId == "undoBookHotel" &&
                            c.ExecutionId == miBodyEventScopeExecutionId)
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c =>
                            c.EventType == "compensate" && c.ActivityId == "undoBookFlight" &&
                            c.ExecutionId == eventScopeExecution.Id)
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c =>
                            c.EventType == "compensate" && c.ActivityId == "undoChargeCard" &&
                            c.ExecutionId == eventScopeExecution.Id)
                    .Count());

            // Assert that the compensation handlers have not been invoked:
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "undoChargeCard"));

            // end the process instance
            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }

        //ORIGINAL LINE: Deployment(new string[]{"resources/bpmn/subprocess/transaction/TransactionSubProcessTest.TestWaitstateCompensationHandler.bpmn20.xml"}) public void TestWaitstateCompensationHandler()

        [Test]
        [Deployment("resources/bpmn/subprocess/transaction/TransactionSubProcessTest.TestWaitstateCompensationHandler.bpmn20.xml")]
        public virtual void TestWaitstateCompensationHandler()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");

            // after the process is started, we have compensate event subscriptions:
            Assert.AreEqual(5,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookHotel")
                    .Count());
            Assert.AreEqual(1,
                runtimeService.CreateEventSubscriptionQuery(
                        c => c.EventType == "compensate" && c.ActivityId == "undoBookFlight")
                    .Count());

            // the task is present:
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // making the tx Assert.Fail:
            taskService.SetVariable(task.Id, "confirmed", false);
            taskService.Complete(task.Id);

            // now there are two IUser task instances (the compensation handlers):IList<IQueryable<ITask>>

            var undoBookHotel = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "undoBookHotel")
                .ToList();
            var undoBookFlight = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "undoBookFlight")
                .ToList();

            Assert.AreEqual(5, undoBookHotel.Count);
            Assert.AreEqual(1, undoBookFlight.Count);
            var rootActivityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            var undoBookHotelInstances = GetInstancesForActivityId(rootActivityInstance, "undoBookHotel");
            var undoBookFlightInstances = GetInstancesForActivityId(rootActivityInstance, "undoBookFlight");
            Assert.AreEqual(5, undoBookHotelInstances.Count);
            Assert.AreEqual(1, undoBookFlightInstances.Count);

            //待修改
            Assert.That(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.Id)
                            .BeginScope("tx")
                            .Activity("failure")
                            .Activity("undoBookHotel")
                            .Activity("undoBookHotel")
                            .Activity("undoBookHotel")
                            .Activity("undoBookHotel")
                            .Activity("undoBookHotel")
                            .Activity("undoBookFlight")
                            .Done() != null);

            foreach (var t in undoBookHotel)
                taskService.Complete(t.Id);
            taskService.Complete(undoBookFlight[0].Id);

            // now the process instance execution is sitting in the 'afterCancellation' task
            // -> has left the transaction using the cancel boundary event
            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.True(activeActivityIds.Contains("afterCancellation"));

            // we have no more compensate event subscriptions
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "compensate")
                .Count());

            // end the process instance
            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }
        
    }
}