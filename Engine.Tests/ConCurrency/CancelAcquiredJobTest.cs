using System;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace Engine.Tests.ConCurrency
{
    
	public class CancelAcquiredJobTest : PluggableProcessEngineTestCase
    {
        [Deployment]
        public virtual void testBothJobsAcquiredAtSameTime()
        {

            runtimeService.StartProcessInstanceByKey("testProcess");

            // move clock by 20 seconds -> both jobs are acquirable:
            //ClockUtil.CurrentTime = new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis() + (20 * 1000));
            ClockUtil.CurrentTime = DateTime.Now.AddSeconds(20);
            WaitForJobExecutorToProcessAllJobs(6000);

        }



    }

}