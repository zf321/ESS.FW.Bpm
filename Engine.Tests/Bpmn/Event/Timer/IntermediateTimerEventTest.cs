using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class IntermediateTimerEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testCatchingTimerEvent()
        {

            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            // After process start, there should be timer created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("intermediateTimerEventExample");
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            Assert.AreEqual(1, jobQuery.Count());

            // After setting the clock to time '50minutes and 5 seconds', the second timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.AddMinutes(50).AddSeconds(5).Ticks);
            WaitForJobExecutorToProcessAllJobs(5000L);

            jobQuery = managementService.CreateJobQuery(c => c.ProcessInstanceId == pi.Id);
            Assert.AreEqual(0, jobQuery.Count());
            AssertProcessEnded(pi.ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testExpression()
        {
            // Set the clock fixed
            Dictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["dueDate"] = DateTime.Now;

            Dictionary<string, object> variables2 = new Dictionary<string, object>();
            //variables2["dueDate"] = (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(DateTime.Now);
            variables2["dueDate"] = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");


            // After process start, there should be timer created
            IProcessInstance pi1 = runtimeService.StartProcessInstanceByKey("intermediateTimerEventExample", variables1);
            IProcessInstance pi2 = runtimeService.StartProcessInstanceByKey("intermediateTimerEventExample", variables2);

            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi1.Id).Count());
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi2.Id).Count());

            // After setting the clock to one second in the future the timers should fire
            IList<IJob> jobs = managementService.CreateJobQuery()/*.Executable()*/.ToList();
            Assert.AreEqual(2, jobs.Count);
            foreach (IJob job in jobs)
            {
                managementService.ExecuteJob(job.Id);
            }

            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi1.Id).Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi2.Id).Count());

            AssertProcessEnded(pi1.ProcessInstanceId);
            AssertProcessEnded(pi2.ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testTimeCycle()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            IQueryable<IJob> query = managementService.CreateJobQuery();
            Assert.AreEqual(1, query.Count());

            string jobId = query.First().Id;
            managementService.ExecuteJob(jobId);

            Assert.AreEqual(0, query.Count());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

            AssertProcessEnded(ProcessInstanceId);
        }

    }
}