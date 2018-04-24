using System.Diagnostics;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;


namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// </summary>
    public class CompetingJobAcquisitionTest : PluggableProcessEngineTestCase
    {

        ////private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal Thread testThread = Thread.CurrentThread;
        internal static ControllableThread activeThread;
        internal static string jobId;

        public class JobAcquisitionThread : ControllableThread
        {
            private readonly CompetingJobAcquisitionTest outerInstance;

            public JobAcquisitionThread(CompetingJobAcquisitionTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            internal OptimisticLockingException exception;
            internal AcquiredJobs jobs;

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }
            public virtual void run()
            {
                try
                {
                    ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor = outerInstance.processEngineConfiguration.JobExecutor;
                    jobs = outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<AcquiredJobs>(activeThread, new AcquireJobsCmd(jobExecutor)));

                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment]
        public virtual void testCompetingJobAcquisitions()
        {
            runtimeService.StartProcessInstanceByKey("CompetingJobAcquisitionProcess");

            Debug.WriteLine("test thread starts thread one");
            JobAcquisitionThread threadOne = new JobAcquisitionThread(this);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread two");
            JobAcquisitionThread threadTwo = new JobAcquisitionThread(this);
            threadTwo.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.IsNull(threadOne.exception);
            // the job was acquired
            Assert.AreEqual(1, threadOne.jobs.Size());

            Debug.WriteLine("test thread notifies thread 2");
            threadTwo.proceedAndWaitTillDone();
            // the acquisition did NOT Assert.Fail
            Assert.IsNull(threadTwo.exception);
            // but the job was not acquired
            Assert.AreEqual(0, threadTwo.jobs.Size());

        }

    }

}