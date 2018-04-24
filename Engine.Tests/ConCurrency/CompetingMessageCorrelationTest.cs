using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CompetingMessageCorrelationTest : ConcurrencyTestCase
    {
        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void testConcurrentCorrelationFailsWithOptimisticLockingException()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // both threads correlate
            thread1.MakeContinue();
            thread2.MakeContinue();

            thread1.WaitForSync();
            thread2.WaitForSync();

            // the service task was executed twice
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // the first thread ends its transcation
            thread1.WaitUntilDone();
            Assert.IsNull(thread1.Exception);

            ITask afterMessageTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

            // the second thread ends its transaction and fails with optimistic locking exception
            thread2.WaitUntilDone();
            Assert.True(thread2.Exception != null);
            Assert.True(thread2.Exception is OptimisticLockingException);
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void testConcurrentExclusiveCorrelation()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // thread one correlates and acquires the exclusive lock
            thread1.MakeContinue();
            thread1.WaitForSync();

            // the service task was executed once
            Assert.AreEqual(1, InvocationLogListener.Invocations);

            // thread two attempts to acquire the exclusive lock but can't since thread 1 hasn't released it yet
            thread2.MakeContinue();
            Thread.Sleep(2000);

            // let the first thread ends its transaction
            thread1.MakeContinue();
            Assert.IsNull(thread1.Exception);

            // thread 2 can't continue because the event subscription it tried to lock was deleted
            thread2.WaitForSync();
            Assert.True(thread2.Exception != null);
            Assert.True(thread2.Exception is ProcessEngineException);
            AssertTextPresent("does not have a subscription to a message event with name 'Message'", thread2.Exception.Message);

            // the first thread ended successfully without an exception
            thread1.Join();
            Assert.IsNull(thread1.Exception);

            // the follow-up task was reached
            ITask afterMessageTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

            // the service task was not executed a second time
            Assert.AreEqual(1, InvocationLogListener.Invocations);
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void testConcurrentExclusiveCorrelationToDifferentExecutions()
        {
            InvocationLogListener.reset();

            // given a process instance
            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("testProcess");
            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel to each of the two instances
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", instance1.Id, true));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", instance2.Id, true));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // thread one correlates and acquires the exclusive lock on the event subscription of instance1
            thread1.MakeContinue();
            thread1.WaitForSync();

            // the service task was executed once
            Assert.AreEqual(1, InvocationLogListener.Invocations);

            // thread two correlates and acquires the exclusive lock on the event subscription of instance2
            // depending on the database and locking used, this may block thread2
            thread2.MakeContinue();

            // thread 1 completes successfully
            thread1.WaitUntilDone();
            Assert.IsNull(thread1.Exception);

            // thread2 should be able to continue at least after thread1 has finished and released its lock
            thread2.WaitForSync();

            // the service task was executed the second time
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // thread 2 completes successfully
            thread2.WaitUntilDone();
            Assert.IsNull(thread2.Exception);

            // the follow-up task was reached in both instances
            Assert.AreEqual(2, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="afterMessageUserTask").Count());
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void FAILING_testConcurrentExclusiveCorrelationToDifferentExecutionsCase2()
        {
            InvocationLogListener.reset();

            // given a process instance
            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("testProcess");
            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel to each of the two instances
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", instance1.Id, true));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", instance2.Id, true));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // thread one correlates and acquires the exclusive lock on the event subscription of instance1
            thread1.MakeContinue();
            thread1.WaitForSync();

            // the service task was executed once
            Assert.AreEqual(1, InvocationLogListener.Invocations);

            // thread two correlates and acquires the exclusive lock on the event subscription of instance2
            thread2.MakeContinue();
            // FIXME: this does not return on sql server due to locking
            thread2.WaitForSync();

            // the service task was executed the second time
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // thread 2 completes successfully, even though it acquired its lock after thread 1
            thread2.WaitUntilDone();
            Assert.IsNull(thread2.Exception);

            // thread 1 completes successfully
            thread1.WaitUntilDone();
            Assert.IsNull(thread1.Exception);

            // the follow-up task was reached in both instances
            Assert.AreEqual(2, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="afterMessageUserTask").Count());
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void testConcurrentMixedCorrelation()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel (one exclusive, one non-exclusive)
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // thread one correlates and acquires the exclusive lock
            thread1.MakeContinue();
            thread1.WaitForSync();

            // thread two correlates since it does not need a pessimistic lock
            thread2.MakeContinue();
            thread2.WaitForSync();

            // the service task was executed twice
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // the first thread ends its transaction and releases the lock; the event subscription is now gone
            thread1.WaitUntilDone();
            Assert.IsNull(thread1.Exception);

            ITask afterMessageTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

            // thread two attempts to end its transaction and fails with optimistic locking
            thread2.MakeContinue();
            thread2.WaitForSync();

            Assert.True(thread2.Exception != null);
            Assert.True(thread2.Exception is OptimisticLockingException);
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml" })]
        public virtual void FAILING_testConcurrentMixedCorrelationCase2()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel (one exclusive, one non-exclusive)
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // thread one correlates and acquires no lock
            thread1.MakeContinue();
            thread1.WaitForSync();

            // thread two acquires a lock and succeeds because thread one hasn't acquired one
            thread2.MakeContinue();
            thread2.WaitForSync();

            // the service task was executed twice
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // thread one ends its transaction and blocks on flush when it attempts to Delete the event subscription
            thread1.MakeContinue();
            Thread.Sleep(5000);
            Assert.IsNull(thread1.Exception);

            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

            // thread 2 flushes successfully and releases the lock
            thread2.WaitUntilDone();
            Assert.IsNull(thread2.Exception);

            ITask afterMessageTask = taskService.CreateTaskQuery().First();
            Assert.NotNull(afterMessageTask);
            Assert.AreEqual(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

            // thread 1 flush fails with optimistic locking
            thread1.Join();
            Assert.True(thread1.Exception != null);
            Assert.True(thread1.Exception is OptimisticLockingException);
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.EventSubprocess.bpmn" })]
        public virtual void testEventSubprocess()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            // and two threads correlating in parallel
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("incoming", false));
            thread1.ReportInterrupts();
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("incoming", false));
            thread2.ReportInterrupts();

            // both threads open a transaction and wait before correlating the message
            thread1.WaitForSync();
            thread2.WaitForSync();

            // both threads correlate
            thread1.MakeContinue();
            thread2.MakeContinue();

            thread1.WaitForSync();
            thread2.WaitForSync();

            // the first thread ends its transaction
            thread1.WaitUntilDone();
            Assert.IsNull(thread1.Exception);

            // the second thread ends its transaction and fails with optimistic locking exception
            thread2.WaitUntilDone();
            Assert.True(thread2.Exception != null);
            Assert.True(thread2.Exception is OptimisticLockingException);
        }

        [Deployment]
        public virtual void testConcurrentMessageCorrelationAndTreeCompaction()
        {
            runtimeService.StartProcessInstanceByKey("process");

            // trigger non-interrupting boundary event and wait before flush
            ThreadControl correlateThread = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
            correlateThread.ReportInterrupts();

            // stop correlation right before the flush
            correlateThread.WaitForSync();
            correlateThread.MakeContinueAndWaitForSync();

            // trigger tree compaction
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            // flush correlation
            correlateThread.WaitUntilDone();

            // the correlation should not have succeeded
            System.Exception exception = correlateThread.Exception;
            Assert.NotNull(exception);
            Assert.True(exception is OptimisticLockingException);
        }

        [Deployment(new string[] { "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.TestConcurrentMessageCorrelationAndTreeCompaction.bpmn20.xml" })]
        public virtual void testConcurrentTreeCompactionAndMessageCorrelation()
        {
            runtimeService.StartProcessInstanceByKey("process");
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();

            // trigger tree compaction and wait before flush
            ThreadControl taskCompletionThread = ExecuteControllableCommand(new ControllableCompleteTaskCommand(tasks));
            taskCompletionThread.ReportInterrupts();

            // stop task completion right before flush
            taskCompletionThread.WaitForSync();

            // perform message correlation to non-interrupting boundary event
            // (i.E. adds another concurrent execution to the scope execution)
            runtimeService.CorrelateMessage("Message");

            // flush task completion and tree compaction
            taskCompletionThread.WaitUntilDone();

            // then it should not have succeeded
            System.Exception exception = taskCompletionThread.Exception;
            Assert.NotNull(exception);
            Assert.True(exception is OptimisticLockingException);
        }

        [Deployment]
        public virtual void testConcurrentMessageCorrelationTwiceAndTreeCompaction()
        {
            runtimeService.StartProcessInstanceByKey("process");

            // trigger non-interrupting boundary event 1 that ends in a none end event immediately
            runtimeService.CorrelateMessage("Message2");

            // trigger non-interrupting boundary event 2 and wait before flush
            ThreadControl correlateThread = ExecuteControllableCommand(new ControllableMessageCorrelationCommand("Message1", false));
            correlateThread.ReportInterrupts();

            // stop correlation right before the flush
            correlateThread.WaitForSync();
            correlateThread.MakeContinueAndWaitForSync();

            // trigger tree compaction
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            // flush correlation
            correlateThread.WaitUntilDone();

            // the correlation should not have succeeded
            System.Exception exception = correlateThread.Exception;
            Assert.NotNull(exception);
            Assert.True(exception is OptimisticLockingException);
        }

        [Test]
        [Deployment]
        public virtual void testConcurrentEndExecutionListener()
        {
            InvocationLogListener.reset();

            // given a process instance
            runtimeService.StartProcessInstanceByKey("testProcess");

            IList<IExecution> tasks = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("Message")*/.ToList();
            // two tasks waiting for the message
            Assert.AreEqual(2, tasks.Count);

            // start first thread and wait in the second execution end listener
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableMessageEventReceivedCommand(tasks[0].Id, "Message", true));
            thread1.ReportInterrupts();
            thread1.WaitForSync();

            // the counting execution listener was executed on task 1
            Assert.AreEqual(1, InvocationLogListener.Invocations);

            // start second thread and complete the task
            ThreadControl thread2 = ExecuteControllableCommand(new ControllableMessageEventReceivedCommand(tasks[1].Id, "Message", false));
            thread2.WaitForSync();
            thread2.WaitUntilDone();

            // the counting execution listener was executed on task 1 and 2
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // continue with thread 1
            thread1.MakeContinueAndWaitForSync();

            // the counting execution listener was not executed again
            Assert.AreEqual(2, InvocationLogListener.Invocations);

            // try to complete thread 1
            thread1.WaitUntilDone();

            // thread 1 was rolled back with an optimistic locking exception
            System.Exception exception = thread1.Exception;
            Assert.NotNull(exception);
            Assert.True(exception is OptimisticLockingException);

            // the execution listener was not executed again
            Assert.AreEqual(2, InvocationLogListener.Invocations);
        }

        public class InvocationLogListener : IJavaDelegate
        {

            //protected internal static AtomicInteger invocations = new AtomicInteger(0);

            //public static void reset()
            //{
            //    invocations.Set(0);
            //}

            //public void Execute(IBaseDelegateExecution execution)
            //{
            //    invocations.incrementAndGet();
            //}

            //public static int Invocations
            //{
            //    get
            //    {
            //        return invocations.Get();
            //    }
            //}

            protected internal static int invocations = 0;

            public static void reset()
            {
                invocations = 0;
            }

            public void Execute(IBaseDelegateExecution execution)
            {
                invocations++;
            }

            public static int Invocations
            {
                get { return invocations; }
            }
        }

        public class WaitingListener : IDelegateListener<IBaseDelegateExecution>
        {

            protected internal static ThreadControl monitor;

            public virtual void Notify(IDelegateExecution execution)
            {
                if (WaitingListener.Monitor != null)
                {
                    ThreadControl localMonitor = WaitingListener.Monitor;
                    WaitingListener.Monitor = null;
                    localMonitor.Sync();
                }
            }

            public void Notify(IBaseDelegateExecution instance)
            {
                if (WaitingListener.Monitor != null)
                {
                    ThreadControl localMonitor = WaitingListener.Monitor;
                    WaitingListener.Monitor = null;
                    localMonitor.Sync();
                }
            }

            public static ThreadControl Monitor
            {
                set
                {
                    monitor = value;
                }
                get { return monitor; }
            }
        }

        protected internal class ControllableMessageCorrelationCommand : ControllableCommand<object>
        {

            protected internal string messageName;
            protected internal bool exclusive;
            protected internal string ProcessInstanceId;

            public ControllableMessageCorrelationCommand(string messageName, bool exclusive)
            {
                this.messageName = messageName;
                this.exclusive = exclusive;
            }

            public ControllableMessageCorrelationCommand(string messageName, string ProcessInstanceId, bool exclusive) : this(messageName, exclusive)
            {
                this.ProcessInstanceId = ProcessInstanceId;
            }

            public override object Execute(CommandContext commandContext)
            {

                Monitor.Sync(); // thread will block here until makeContinue() is called form main thread

                MessageCorrelationBuilderImpl correlationBuilder = new MessageCorrelationBuilderImpl(commandContext, messageName);
                if (!string.ReferenceEquals(ProcessInstanceId, null))
                {
                    correlationBuilder.ProcessInstanceId(ProcessInstanceId);
                }

                if (exclusive)
                {
                    correlationBuilder.CorrelateExclusively();
                }
                else
                {
                    correlationBuilder.Correlate();
                }

                Monitor.Sync(); // thread will block here until waitUntilDone() is called form main thread

                return null;
            }

        }

        protected internal class ControllableMessageEventReceivedCommand : ControllableCommand<object>
        {

            protected internal readonly string executionId;
            protected internal readonly string messageName;
            protected internal readonly bool shouldWaitInListener;

            public ControllableMessageEventReceivedCommand(string executionId, string messageName, bool shouldWaitInListener)
            {
                this.executionId = executionId;
                this.messageName = messageName;
                this.shouldWaitInListener = shouldWaitInListener;
            }

            public override object Execute(CommandContext commandContext)
            {

                if (shouldWaitInListener)
                {
                    WaitingListener.Monitor = Monitor;
                }

                MessageEventReceivedCmd receivedCmd = new MessageEventReceivedCmd(messageName, executionId, null);

                receivedCmd.Execute(commandContext);

                Monitor.Sync();

                return null;
            }
        }

        public class ControllableCompleteTaskCommand : ControllableCommand<object>
        {

            protected internal IList<ITask> tasks;

            public ControllableCompleteTaskCommand(IList<ITask> tasks)
            {
                this.tasks = tasks;
            }

            public override object Execute(CommandContext commandContext)
            {

                foreach (ITask task in tasks)
                {
                    CompleteTaskCmd completeTaskCmd = new CompleteTaskCmd(task.Id, null);
                    completeTaskCmd.Execute(commandContext);
                }

                Monitor.Sync();

                return null;
            }

        }

    }

}