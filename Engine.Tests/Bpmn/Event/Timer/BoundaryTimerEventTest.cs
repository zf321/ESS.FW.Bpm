using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class BoundaryTimerEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testMultipleTimersOnUserTask()
        {
            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            // After process start, there should be 3 Timers created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("multipleTimersOnUserTask");
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            IList<IJob> jobs = jobQuery.ToList();
            Assert.AreEqual(3, jobs.Count);

            // After setting the clock to time '1 hour and 5 seconds', the second timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
            WaitForJobExecutorToProcessAllJobs(5000L);
            Assert.AreEqual(0L, jobQuery.Count());

            // which means that the third task is reached
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Third ITask", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnNestingOfSubprocesses()
        {

            runtimeService.StartProcessInstanceByKey("timerOnNestedSubprocesses");
            IList<ITask> tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Inner subprocess task 1", tasks[0].Name);
            Assert.AreEqual("Inner subprocess task 2", tasks[1].Name);

            IJob timer = managementService.CreateJobQuery()/*.Timers()*/.First();
            managementService.ExecuteJob(timer.Id);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task outside subprocess", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testExpressionOnTimer()
        {
            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["duration"] = "PT1H";

            // After process start, there should be a timer created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testExpressionOnTimer", variables);

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            IList<IJob> jobs = jobQuery.ToList();
            Assert.AreEqual(1, jobs.Count);

            // After setting the clock to time '1 hour and 5 seconds', the second timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
            WaitForJobExecutorToProcessAllJobs(5000L);
            Assert.AreEqual(0L, jobQuery.Count());

            // which means the process has ended
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTimerInSingleTransactionProcess()
        {
            // make sure that if a PI completes in single transaction, JobEntities associated with the execution are deleted.
            // broken before 5.10, see ACT-1133
            runtimeService.StartProcessInstanceByKey("timerOnSubprocesses");
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testRepeatingTimerWithCancelActivity()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("repeatingTimerAndCallActivity");
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            // Firing job should cancel the IUser task, destroy the scope,
            // re-enter the task and recreate the task. A new timer should also be created.
            // This didn't happen before 5.11 (new jobs kept being created). See ACT-1427
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testMultipleOutgoingSequenceFlows()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("interruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IList<ITask> tasks = taskQuery.ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleOutgoingSequenceFlowsOnSubprocess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("interruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IList<ITask> tasks = taskQuery.ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("interruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IList<ITask> tasks = taskQuery.ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingTimerDuration()
        {

            // Start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("escalationExample");

            // There should be one task, with a timer : first line support
            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.AreEqual("First line support", task.Name);

            // Manually execute the job
            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);

            // The timer has fired, and the second task (secondlinesupport) now exists
            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.AreEqual("Handle escalated issue", task.Name);
        }

    }

}