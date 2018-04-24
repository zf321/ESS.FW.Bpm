using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorBatchTest
    {
        [SetUp]
        public void SetUpEngineRule()
        {
            try
            {
                if (EngineRule.ProcessEngine == null)
                    EngineRule.InitializeProcessEngine();

                EngineRule.InitializeServices();

                EngineRule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [SetUp]
        public void SetUpTestRule()
        {
            try
            {
                MigrationRule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        [SetUp]
        public virtual void ReplaceJobExecutor()
        {
            var processEngineConfiguration = EngineRule.ProcessEngineConfiguration;
            DefaultJobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor = new CountingJobExecutor(this);
            processEngineConfiguration.SetJobExecutor(jobExecutor);
        }

        [SetUp]
        public virtual void SaveBatchJobsPerSeed()
        {
            DefaultBatchJobsPerSeed = EngineRule.ProcessEngineConfiguration.BatchJobsPerSeed;
        }

        [TearDown]
        public void TearDownEngineRule()
        {
            EngineRule.Finished();
        }

        [TearDown]
        public void TearDownTestRule()
        {
            MigrationRule.Finished();
        }

        [TearDown]
        public virtual void ResetJobExecutor()
        {
            EngineRule.ProcessEngineConfiguration.SetJobExecutor(DefaultJobExecutor);
        }

        [TearDown]
        public virtual void ResetBatchJobsPerSeed()
        {
            EngineRule.ProcessEngineConfiguration.BatchJobsPerSeed = DefaultBatchJobsPerSeed;
        }

        [TearDown]
        public virtual void RemoveBatches()
        {
            Helper.RemoveAllRunningAndHistoricBatches();
        }

        private readonly bool _instanceFieldsInitialized;

        public JobExecutorBatchTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            MigrationRule = new MigrationTestRule(EngineRule);
            Helper = new BatchMigrationHelper(EngineRule, MigrationRule);
            ////ruleChain = RuleChain.outerRule(EngineRule).around(MigrationRule);
        }


        protected internal ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule MigrationRule;
        protected internal BatchMigrationHelper Helper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule);
        ////public RuleChain ruleChain;
        public CountingJobExecutor jobExecutor;
        protected internal ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor DefaultJobExecutor;
        protected internal int DefaultBatchJobsPerSeed;

        [Test]
        [Ignore("迁移暂时没用到")]
        public virtual void TestJobExecutorHintedOnBatchCreation()
        {
            // given
            jobExecutor.StartRecord();

            // when a batch is created
            Helper.MigrateProcessInstancesAsync(2);

            // then the job executor is hinted for the seed job
            Assert.AreEqual(1, jobExecutor.JobsAdded);
        }

        [Test]
        [Ignore("迁移暂时没用到")]
        public virtual void TestJobExecutorHintedSeedJobExecution()
        {
            // reduce number of batch jobs per seed to not have to create a lot of instances
            EngineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

            // given
            var batch = Helper.MigrateProcessInstancesAsync(13);
            jobExecutor.StartRecord();

            // when the seed job is Executed
            Helper.ExecuteSeedJob(batch);

            // then the job executor is hinted for the seed job and 10 execution jobs
            Assert.AreEqual(11, jobExecutor.JobsAdded);
        }

        [Test]
        [Ignore("迁移暂时没用到")]
        public virtual void TestJobExecutorHintedSeedJobCompletion()
        {
            // given
            var batch = Helper.MigrateProcessInstancesAsync(3);
            jobExecutor.StartRecord();

            // when the seed job is Executed
            Helper.ExecuteSeedJob(batch);

            // then the job executor is hinted for the monitor job and 3 execution jobs
            Assert.AreEqual(4, jobExecutor.JobsAdded);
        }

        public class CountingJobExecutor : ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor
        {
            private readonly JobExecutorBatchTest _outerInstance;
            public long jobsAdded;


            public bool Record;

            public CountingJobExecutor(JobExecutorBatchTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public override bool IsActive
            {
                get { return true; }
            }

            public virtual long JobsAdded
            {
                get { return jobsAdded; }
            }

            protected override void StartExecutingJobs()
            {
                // do nothing
            }

            protected override void StopExecutingJobs()
            {
                // do nothing
            }

            public override void ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
            {
                // do nothing
            }

            public virtual void StartRecord()
            {
                ResetJobsAdded();
                Record = true;
            }

            public virtual void JobWasAdded()
            {
                if (Record)
                    jobsAdded++;
            }

            public virtual void ResetJobsAdded()
            {
                jobsAdded = 0;
            }
        }
    }
}