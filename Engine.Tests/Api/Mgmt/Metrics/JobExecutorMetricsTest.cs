using Engine.Tests.JobExecutor;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorMetricsTest : AbstractMetricsTest
    {
        protected internal ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor;
        //protected internal ThreadPoolExecutor jobThreadPoolExecutor;
        [SetUp]
        protected internal void setUp()
        {
            //base.SetUp();
            jobExecutor = processEngineConfiguration.JobExecutor;
        }
        [TearDown]
        protected internal void tearDown()
        {
            TearDown();
            processEngineConfiguration.SetJobExecutor(jobExecutor);
        }

        [Test][Deployment( "resources/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
        public virtual void testJobAcquisitionMetricReporting()
        {
            // given
            for (var i = 0; i < 3; i++)
                runtimeService.StartProcessInstanceByKey("asyncServiceTaskProcess");

            // when
            WaitForJobExecutorToProcessAllJobs(5000);
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // then
            var acquisitionAttempts = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquisitionAttempt)
                .Sum();
            Assert.True(acquisitionAttempts >= 1);

            var acquiredJobs = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquiredSuccess)
                .Sum();
            Assert.AreEqual(3, acquiredJobs);
        }

        [Test]
        [Deployment( "resources/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml") ]
        public virtual void testCompetingJobAcquisitionMetricReporting()
        {
            // given
            for (var i = 0; i < 3; i++)
                runtimeService.StartProcessInstanceByKey("asyncServiceTaskProcess");

            // replace job executor
            var jobExecutor1 = new ControllableJobExecutor((ProcessEngineImpl) ProcessEngine);
            processEngineConfiguration.SetJobExecutor(jobExecutor1);
            var jobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl) ProcessEngine);

            var jobAcquisitionThread1 = jobExecutor1.AcquisitionThreadControl;
            var jobAcquisitionThread2 = jobExecutor2.AcquisitionThreadControl;

            // when both executors are waiting to finish acquisition
            jobExecutor1.Start();
            jobAcquisitionThread1.WaitForSync(); // wait before starting acquisition
            jobAcquisitionThread1.MakeContinueAndWaitForSync(); // wait before finishing acquisition

            jobExecutor2.Start();
            jobAcquisitionThread2.WaitForSync(); // wait before starting acquisition
            jobAcquisitionThread2.MakeContinueAndWaitForSync(); // wait before finishing acquisition

            // thread 1 is able to acquire all jobs
            jobAcquisitionThread1.MakeContinueAndWaitForSync();
            // thread 2 cannot acquire any jobs since they have been locked (and executed) by thread1 meanwhile
            jobAcquisitionThread2.MakeContinueAndWaitForSync();

            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // then
            var acquisitionAttempts = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquisitionAttempt)
                .Sum();
            // each job executor twice (since the controllable thread always waits when already acquiring jobs)
            Assert.AreEqual(2 + 2, acquisitionAttempts);

            var acquiredJobs = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquiredSuccess)
                .Sum();
            Assert.AreEqual(3, acquiredJobs);

            var acquiredJobsFailure = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquiredFailure)
                .Sum();
            Assert.AreEqual(3, acquiredJobsFailure);

            // cleanup
            jobExecutor1.Shutdown();
            jobExecutor2.Shutdown();

            processEngineConfiguration.DbMetricsReporter.ReportNow();
        }

        [Test][Deployment( "resources/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
        public virtual void testJobExecutionMetricReporting()
        {
            // given
            for (var i = 0; i < 3; i++)
                runtimeService.StartProcessInstanceByKey("asyncServiceTaskProcess");
            for (var i = 0; i < 2; i++)
                runtimeService.StartProcessInstanceByKey("asyncServiceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("Assert.Fail", true));

            // when
            WaitForJobExecutorToProcessAllJobs(5000);

            // then
            var jobsSuccessful = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobSuccessful)
                .Sum();
            Assert.AreEqual(3, jobsSuccessful);

            var jobsFailed = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobFailed)
                .Sum();
            // 2 jobs * 3 tries
            Assert.AreEqual(6, jobsFailed);

            var jobCandidatesForAcquisition = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquiredSuccess)
                .Sum();
            Assert.AreEqual(3 + 6, jobCandidatesForAcquisition);
        }

        [Test][Deployment]
        public virtual void testJobExecutionMetricExclusiveFollowUp()
        {
            // given
            for (var i = 0; i < 3; i++)
                runtimeService.StartProcessInstanceByKey("exclusiveServiceTasksProcess");

            // when
            WaitForJobExecutorToProcessAllJobs(5000);

            // then
            var jobsSuccessful = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobSuccessful)
                .Sum();
            Assert.AreEqual(6, jobsSuccessful);

            var jobsFailed = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobFailed)
                .Sum();
            Assert.AreEqual(0, jobsFailed);

            // the respective follow-up jobs are exclusive and have been executed right away without
            // acquisition
            var jobCandidatesForAcquisition = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobAcquiredSuccess)
                .Sum();
            Assert.AreEqual(3, jobCandidatesForAcquisition);

            var exclusiveFollowupJobs = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobLockedExclusive)
                .Sum();
            Assert.AreEqual(3, exclusiveFollowupJobs);
        }

        [Test]
        [Deployment("resources/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
        public virtual void testJobRejectedExecutionMetricReporting()
        {
            // replace job executor with one that rejects all jobs
            var rejectingExecutor = new RejectingJobExecutor();
            processEngineConfiguration.SetJobExecutor(rejectingExecutor);
            rejectingExecutor.RegisterProcessEngine((ProcessEngineImpl) ProcessEngine);

            // given three jobs
            for (var i = 0; i < 3; i++)
                runtimeService.StartProcessInstanceByKey("asyncServiceTaskProcess");

            // when executing the jobs
            WaitForJobExecutorToProcessAllJobs(5000L);

            // then all of them were rejected by the job executor which is reflected by the metric
            var numRejectedJobs = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.JobExecutionRejected)
                .Sum();

            Assert.AreEqual(3, numRejectedJobs);
        }

        public class RejectingJobExecutor : DefaultJobExecutor
        {
            //public RejectingJobExecutor()
            //{
            //  BlockingQueue<ThreadStart> threadPoolQueue = new ArrayBlockingQueue<ThreadStart>(queueSize);
            //  threadPoolExecutor = new ThreadPoolExecutorAnonymousInnerClass(this, corePoolSize, maxPoolSize, TimeUnit.MILLISECONDS, threadPoolQueue);
            //  threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.AbortPolicy();

            //  rejectedJobsHandler = new CallerRunsRejectedJobsHandler();
            //}

            //private class ThreadPoolExecutorAnonymousInnerClass : ThreadPoolExecutor
            //{
            //	private readonly RejectingJobExecutor outerInstance;

            //	public ThreadPoolExecutorAnonymousInnerClass(RejectingJobExecutor outerInstance, UnknownType corePoolSize, UnknownType maxPoolSize, UnknownType MILLISECONDS, BlockingQueue<ThreadStart> threadPoolQueue) : base(corePoolSize, maxPoolSize, 0L, MILLISECONDS, threadPoolQueue)
            //	{
            //		this.outerInstance = outerInstance;
            //	}


            //	public override void execute(ThreadStart command)
            //	{
            //	  throw new RejectedExecutionException();
            //	}
            //}
        }
    }
}