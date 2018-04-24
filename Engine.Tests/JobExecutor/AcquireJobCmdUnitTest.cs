using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NSubstitute;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class AcquireJobCmdUnitTest
    {
        protected internal const string ProcessInstanceId1 = "pi_1";
        protected internal const string ProcessInstanceId2 = "pi_2";

        protected internal const string JobId1 = "job_1";
        protected internal const string JobId2 = "job_2";

        protected internal AcquireJobsCmd acquireJobsCmd;
        protected internal IJobManager jobManager;
        protected internal CommandContext commandContext;


        [SetUp]
        public void InitCommand()
        {
            try
            {
                var jobExecutor = Substitute.For<ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor>();
                jobExecutor.MaxJobsPerAcquisition.Returns(3);
                jobExecutor.LockOwner.Returns("test");
                jobExecutor.LockTimeInMillis.Returns(5 * 60 * 1000);

                acquireJobsCmd = new AcquireJobsCmd(jobExecutor);

                commandContext = Substitute.For<CommandContext>();

                jobManager = Substitute.For<IJobManager>();
                commandContext.JobManager.Returns(jobManager);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        
        [Test]
        public virtual void NonExclusiveJobsSameInstance()
        {
            // given: two non-exclusive jobs for a different process instance
            var job1 = CreateNonExclusiveJob(JobId1, ProcessInstanceId1);
            var job2 = CreateNonExclusiveJob(JobId2, ProcessInstanceId1);

            // Todo: JobManager.FindNextJobsToExecute()复杂sql语句
            // when the job executor acquire new jobs
            jobManager.FindNextJobsToExecute(Arg.Any<Page>()).Returns(new List<JobEntity> {job1, job2});

            // then the job executor should acquire job1 and job 2 in different batches
            CheckThatAcquiredJobsInDifferentBatches();
        }

        [Test]
        public virtual void NonExclusiveDifferentInstance()
        {
            // given: two non-exclusive jobs for the same process instance
            var job1 = CreateNonExclusiveJob(JobId1, ProcessInstanceId1);
            var job2 = CreateNonExclusiveJob(JobId2, ProcessInstanceId2);

            // Todo: JobManager.FindNextJobsToExecute()复杂sql语句
            // when the job executor acquire new jobs
            jobManager.FindNextJobsToExecute(Arg.Any<Page>()).Returns(new List<JobEntity> {job1, job2});

            // then the job executor should acquire job1 and job 2 in different batches
            CheckThatAcquiredJobsInDifferentBatches();
        }

        [Test]
        public virtual void ExclusiveJobsSameInstance()
        {
            // given: two exclusive jobs for the same process instance
            var job1 = CreateExclusiveJob(JobId1, ProcessInstanceId1);
            var job2 = CreateExclusiveJob(JobId2, ProcessInstanceId1);

            // Todo: JobManager.FindNextJobsToExecute()复杂sql语句
            // when the job executor acquire new jobs
            jobManager.FindNextJobsToExecute(Arg.Any<Page>()).Returns(new List<JobEntity> {job1, job2});

            // then the job executor should acquire job1 and job 2 in one batch
            var acquiredJobs = acquireJobsCmd.Execute(commandContext);

            var jobIdBatches = acquiredJobs.JobIdBatches;
            Assert.That(jobIdBatches.Count, Is.EqualTo(1));
            Assert.That(jobIdBatches[0].Count, Is.EqualTo(2));
            //Assert.That(jobIdBatches[0], HasItems(JobId1, JobId2));
        }

        [Test]
        public virtual void ExclusiveJobsDifferentInstance()
        {
            // given: two exclusive jobs for a different process instance
            var job1 = CreateExclusiveJob(JobId1, ProcessInstanceId1);
            var job2 = CreateExclusiveJob(JobId2, ProcessInstanceId2);

            // when the job executor acquire new jobs
            jobManager.FindNextJobsToExecute(Arg.Any<Page>()).Returns(new List<JobEntity> {job1, job2});

            // then the job executor should acquire job1 and job 2 in different batches
            CheckThatAcquiredJobsInDifferentBatches();
        }

        protected internal virtual JobEntity CreateExclusiveJob(string id, string processInstanceId)
        {
            var job = CreateNonExclusiveJob(id, processInstanceId);
            job.Exclusive.Returns(true);
            return job;
        }

        protected internal virtual JobEntity CreateNonExclusiveJob(string id, string processInstanceId)
        {
            var job = Substitute.For<JobEntity>();
            job.Id.Returns(id);
            job.ProcessInstanceId.Returns(processInstanceId);
            return job;
        }

        protected internal virtual void CheckThatAcquiredJobsInDifferentBatches()
        {
            var acquiredJobs = acquireJobsCmd.Execute(commandContext);

            var jobIdBatches = acquiredJobs.JobIdBatches;
            Assert.That(jobIdBatches.Count, Is.EqualTo(2));
            Assert.That(jobIdBatches[0].Count, Is.EqualTo(1));
            Assert.That(jobIdBatches[1].Count, Is.EqualTo(1));
        }
    }
}