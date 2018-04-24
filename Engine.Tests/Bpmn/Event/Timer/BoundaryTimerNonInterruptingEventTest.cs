using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class BoundaryTimerNonInterruptingEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testMultipleTimersOnUserTask()
        {
            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            // After process start, there should be 3 timers created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimersOnUserTask");
            ITask task1 = taskService.CreateTaskQuery().First();
            Assert.AreEqual("First Task", task1.Name);

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            IList<IJob> jobs = jobQuery.ToList();
            Assert.AreEqual(2, jobs.Count);

            // After setting the clock to time '1 hour and 5 seconds', the first timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
            WaitForJobExecutorToProcessAllJobs(5000L);

            // we still have one timer more to fire
            Assert.AreEqual(1L, jobQuery.Count());

            // and we are still in the first state, but in the second state as well!
            Assert.AreEqual(2L, taskService.CreateTaskQuery().Count());
            IList<ITask> taskList = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Desc()*/.ToList();
            Assert.AreEqual("First Task", taskList[0].Name);
            Assert.AreEqual("Escalation Task 1", taskList[1].Name);

            // complete the task and end the forked execution
            taskService.Complete(taskList[1].Id);

            // but we still have the original executions
            Assert.AreEqual(1L, taskService.CreateTaskQuery().Count());
            Assert.AreEqual("First Task", taskService.CreateTaskQuery().First().Name);

            // After setting the clock to time '2 hour and 5 seconds', the second timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((2 * 60 * 60 * 1000) + 5000));
            WaitForJobExecutorToProcessAllJobs(5000L);

            // no more timers to fire
            Assert.AreEqual(0L, jobQuery.Count());

            // and we are still in the first state, but in the next escalation state as well
            Assert.AreEqual(2L, taskService.CreateTaskQuery().Count());
            taskList = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Desc()*/.ToList();
            Assert.AreEqual("First Task", taskList[0].Name);
            Assert.AreEqual("Escalation Task 2", taskList[1].Name);

            // This time we end the main task
            taskService.Complete(taskList[0].Id);

            // but we still have the escalation task
            Assert.AreEqual(1L, taskService.CreateTaskQuery().Count());
            ITask escalationTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalation Task 2", escalationTask.Name);

            taskService.Complete(escalationTask.Id);

            // now we are really done :-)
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnMiUserTask()
        {

            // After process start, there should be 1 timer created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimersOnUserTask");
            IList<ITask> taskList = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(5, taskList.Count);
            foreach (ITask task in taskList)
            {
                Assert.AreEqual("First Task", task.Name);
            }

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(job);

            // execute the timer
            managementService.ExecuteJob(job.Id);

            // now there are 6 tasks
            taskList = taskService.CreateTaskEntityQuery().OrderBy(m=>m.NameWithoutCascade)/*.OrderByTaskName()*//*.Asc()*/.ToList().Cast<ITask>().ToList();
            Assert.AreEqual(6, taskList.Count);

            // first task is the escalation task
            ITask escalationTask = taskList.ElementAt(0);
            Assert.AreEqual("Escalation Task 1", escalationTask.Name);
            taskList.RemoveAt(0);
            // complete it
            taskService.Complete(escalationTask.Id);

            // now complete the remaining tasks
            foreach (ITask task in taskList)
            {
                taskService.Complete(task.Id);
            }

            // process instance is ended
            AssertProcessEnded(pi.Id);

        }

        [Test]
        [Deployment]
        public virtual void testJoin()
        {
            // After process start, there should be 3 timers created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testJoin");
            ITask task1 = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Main Task", task1.Name);

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            // we now have both tasks
            Assert.AreEqual(2L, taskService.CreateTaskQuery().Count());

            // end the first
            taskService.Complete(task1.Id);

            // we now have one task left
            Assert.AreEqual(1L, taskService.CreateTaskQuery().Count());
            ITask task2 = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Escalation Task", task2.Name);

            // complete the task, the parallel gateway should fire
            taskService.Complete(task2.Id);

            // and the process has ended
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnConcurrentMiTasks()
        {

            // After process start, there should be 1 timer created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("timerOnConcurrentMiTasks");
            IList<ITask> taskList = taskService.CreateTaskEntityQuery().OrderByDescending(m=>m.NameWithoutCascade)/*.OrderByTaskName()*//*.Desc()*/.ToList().Cast<ITask>().ToList();
            Assert.AreEqual(6, taskList.Count);
            ITask secondTask = taskList.ElementAt(0);
            Assert.AreEqual("Second Task", secondTask.Name);
            taskList.RemoveAt(0);
            
            foreach (ITask task in taskList)
            {
                Assert.AreEqual("First Task", task.Name);
            }

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(job);

            // execute the timer
            managementService.ExecuteJob(job.Id);

            // now there are 7 tasks
            taskList = taskService.CreateTaskEntityQuery().OrderBy(m=>m.NameWithoutCascade)/*.OrderByTaskName()*//*.Asc()*/.ToList().Cast<ITask>().ToList();
            Assert.AreEqual(7, taskList.Count);

            // first task is the escalation task
            ITask escalationTask = taskList.ElementAt(0);
            Assert.AreEqual("Escalation Task 1", escalationTask.Name);
            taskList.RemoveAt(0);
            // complete it
            taskService.Complete(escalationTask.Id);

            // now complete the remaining tasks
            foreach (ITask task in taskList)
            {
                taskService.Complete(task.Id);
            }

            // process instance is ended
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnConcurrentTasks()
        {
            string procId = runtimeService.StartProcessInstanceByKey("nonInterruptingOnConcurrentTasks").Id;
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);
            Assert.AreEqual(3, taskService.CreateTaskQuery().Count());

            // Complete task that was reached by non interrupting timer
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask").First();
            taskService.Complete(task.Id);
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            // Complete other tasks
            foreach (ITask t in taskService.CreateTaskQuery().ToList())
            {
                taskService.Complete(t.Id);
            }
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerOnConcurrentTasks.bpmn20.xml" })]
        public virtual void testTimerOnConcurrentTasks2()
        {
            string procId = runtimeService.StartProcessInstanceByKey("nonInterruptingOnConcurrentTasks").Id;
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);
            Assert.AreEqual(3, taskService.CreateTaskQuery().Count());

            // Complete 2 tasks that will trigger the join
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="firstTask").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="secondTask").First();
            taskService.Complete(task.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            // Finally, complete the task that was created due to the timer
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testTimerWithCycle()
        {
            var id = runtimeService.StartProcessInstanceByKey("nonInterruptingCycle").Id;
            IQueryable<ITask> tq = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask");
            Assert.AreEqual(0, tq.Count());
            moveByHours(1);
            Assert.AreEqual(1, tq.Count());
            moveByHours(1);
            Assert.AreEqual(2, tq.Count());

            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            taskService.Complete(task.Id);

            moveByHours(1);
            Assert.AreEqual(2, tq.Count());
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnEmbeddedSubprocess()        
        {
            string id = runtimeService.StartProcessInstanceByKey("nonInterruptingTimerOnEmbeddedSubprocess").Id;

            IQueryable<ITask> tq = taskService.CreateTaskQuery(c=>c.AssigneeWithoutCascade== "kermit");

            Assert.AreEqual(1, tq.Count());

            // Simulate timer
            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);

            tq = taskService.CreateTaskQuery(c=>c.AssigneeWithoutCascade == "kermit");

            Assert.AreEqual(2, tq.Count());

            IList<ITask> tasks = tq.ToList();

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            AssertProcessEnded(id);
        }

        [Test]
        [Deployment]
        public virtual void testReceiveTaskWithBoundaryTimer()
        {
            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["timeCycle"] = "R/PT1H";

            // After process start, there should be a timer created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingCycle", variables);

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            IList<IJob> jobs = jobQuery.ToList();
            Assert.AreEqual(1, jobs.Count);

            // The IExecution Query should work normally and find executions in state "task"
            IList<IExecution> executions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task").ToList();
            Assert.AreEqual(1, executions.Count);
            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(executions[0].Id);
            Assert.AreEqual(1, activeActivityIds.Count);
            Assert.AreEqual("task", activeActivityIds[0]);

            runtimeService.Signal(executions[0].Id);

            //    // After setting the clock to time '1 hour and 5 seconds', the second timer should fire
            //    ClockUtil.setCurrentTime(new Date(startTime.getTime() + ((60 * 60 * 1000) + 5000)));
            //    waitForJobExecutorToProcessAllJobs(5000L);
            //    Assert.AreEqual(0L, jobQuery.Count());

            // which means the process has ended
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTimerOnConcurrentSubprocess()
        {
            string procId = runtimeService.StartProcessInstanceByKey("testTimerOnConcurrentSubprocess").Id;
            Assert.AreEqual(4, taskService.CreateTaskQuery().Count());

            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);
            Assert.AreEqual(5, taskService.CreateTaskQuery().Count());

            // Complete 4 tasks that will trigger the join
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub1task1").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub1task2").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub2task1").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub2task2").First();
            taskService.Complete(task.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            // Finally, complete the task that was created due to the timer
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask").First();
            taskService.Complete(task.Id);

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerOnConcurrentSubprocess.bpmn20.xml" })]
        public virtual void testTimerOnConcurrentSubprocess2()
        {
            string procId = runtimeService.StartProcessInstanceByKey("testTimerOnConcurrentSubprocess").Id;
            Assert.AreEqual(4, taskService.CreateTaskQuery().Count());

            IJob timer = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(timer.Id);
            Assert.AreEqual(5, taskService.CreateTaskQuery().Count());

            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub1task1").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub1task2").First();
            taskService.Complete(task.Id);

            // complete the task that was created due to the timer
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask").First();
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub2task1").First();
            taskService.Complete(task.Id);
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="sub2task2").First();
            taskService.Complete(task.Id);
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

            AssertProcessEnded(procId);
        }

        
        private void moveByHours(int hours)
        {
            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + ((hours * 60 * 1000 * 60) + 5000));
            ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor.Start();
            Thread.Sleep(1000);
            jobExecutor.Shutdown();
        }

        [Test]
        [Deployment]
        public virtual void testMultipleOutgoingSequenceFlows()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(3, taskQuery.Count());

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
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask1").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask2").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask1").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask2").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);

            // Case 2: fire outer tasks first

            pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimer");

            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask1").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="timerFiredTask2").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask1").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask2").First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingTimer");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(10, taskQuery.Count());

            IList<ITask> tasks = taskQuery.ToList();

            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerWithCycle.bpmn20.xml" })]
        public virtual void testTimeCycle()
        {
            // given
            runtimeService.StartProcessInstanceByKey("nonInterruptingCycle");

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            string jobId = jobQuery.First().Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(1, jobQuery.Count());

            string anotherJobId = jobQuery.First().Id;
            Assert.IsFalse(jobId.Equals(anotherJobId));
        }

        [Test]
        [Deployment]
        public virtual void testFailingTimeCycle()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<IJob> failedJobQuery = managementService.CreateJobQuery();
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(1, jobQuery.Count());

            string jobId = jobQuery.First().Id;
            failedJobQuery.Where(c=>c.Id==jobId);

            // when (1)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            IJob failedJob = failedJobQuery.First();
            Assert.AreEqual(2, failedJob.Retries);

            // a new timer job has been created
            Assert.AreEqual(2, jobQuery.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.Retries == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c=>c.Retries > 0).Count());

            // when (2)
            try
            {
                managementService.ExecuteJob(jobId);
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            failedJob = failedJobQuery.First();
            Assert.AreEqual(1, failedJob.Retries);

            // there are still two jobs
            Assert.AreEqual(2, jobQuery.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.Retries == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c=>c.Retries > 0).Count());
        }

    }

}