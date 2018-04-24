using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Exclusive
{
    [TestFixture]
    public class ExclusiveTimerEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testCatchingTimerEvent()
        {

            // Set the clock fixed
            DateTime startTime = DateTime.Now;

            // After process start, there should be 3 timers created
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("exclusiveTimers");
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.Id);
            Assert.AreEqual(3, jobQuery.Count());

            // After setting the clock to time '50minutes and 5 seconds', the timers should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((50 * 60 * 1000) + 5000));
            WaitForJobExecutorToProcessAllJobs(5000L);

            Assert.AreEqual(0, jobQuery.Count());
            AssertProcessEnded(pi.ProcessInstanceId);


        }


    }
}