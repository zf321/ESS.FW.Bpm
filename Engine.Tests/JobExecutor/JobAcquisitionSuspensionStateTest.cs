using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     <para>This testcase verifies that jobs inserted without suspension state are active by default</para>
    ///     @author Christian Lipphardt
    /// </summary>
    [TestFixture]
    public class JobAcquisitionSuspensionStateTest : PluggableProcessEngineTestCase
    {
        protected internal ICommandExecutor commandExecutor;
        protected internal string JobId;
        [SetUp]
        protected internal virtual void SetUp()
        {
            commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
        }
        [TearDown]
        protected internal virtual void tearDown()
        {
            if (!ReferenceEquals(JobId, null))
                commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly JobAcquisitionSuspensionStateTest _outerInstance;

            public CommandAnonymousInnerClass(JobAcquisitionSuspensionStateTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.persistence.Entity.JobEntity newTimer = commandContext.GetJobManager().FindJobById(jobId);
                var newTimer = commandContext.JobManager.FindJobById(_outerInstance.JobId);
                newTimer.Delete();
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(newTimer.Id);
                return null;
            }
        }

        [Test]
        public virtual void TestJobAcquisitionForJobsWithoutSuspensionStateSet()
        {
            const string processInstanceId = "1";
            const string myCustomTimerEntity = "myCustomTimerEntity";
            const string jobId = "2";

            // we insert a timer job without specifying a suspension state
            commandExecutor.Execute(new CommandAnonymousInnerClass2(this, processInstanceId, myCustomTimerEntity, jobId));

            // it is picked up by the acquisition queries
            commandExecutor.Execute(new CommandAnonymousInnerClass3(this, processInstanceId, myCustomTimerEntity));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly JobAcquisitionSuspensionStateTest _outerInstance;
            private string _jobId;
            private string _myCustomTimerEntity;

            private string _processInstanceId;

            public CommandAnonymousInnerClass2(JobAcquisitionSuspensionStateTest outerInstance, string processInstanceId,
                string myCustomTimerEntity, string jobId)
            {
                _outerInstance = outerInstance;
                _processInstanceId = processInstanceId;
                _myCustomTimerEntity = myCustomTimerEntity;
                _jobId = jobId;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                JobEntity job = new TimerEntity
                {
                    Id = _jobId,
                    Revision = 1,
                    Retries = 3,
                    ProcessInstanceId = _processInstanceId,
                    Exclusive = true,
                    JobHandlerType = TimerStartEventJobHandler.TYPE,
                    JobHandlerConfigurationRaw = _myCustomTimerEntity
                };

                var context = _outerInstance.runtimeService.GetDbContext();
                context.Set<JobEntity>().Add(job);
                int result = context.SaveChanges();
                Assert.AreEqual(1,result);
                _outerInstance.JobId = _jobId;

                
                return null;
            }
        }

        private class CommandAnonymousInnerClass3 : ICommand<object>
        {
            private readonly JobAcquisitionSuspensionStateTest _outerInstance;
            private readonly string _myCustomTimerEntity;

            private readonly string _processInstanceId;

            public CommandAnonymousInnerClass3(JobAcquisitionSuspensionStateTest outerInstance, string processInstanceId,
                string myCustomTimerEntity)
            {
                _outerInstance = outerInstance;
                _processInstanceId = processInstanceId;
                _myCustomTimerEntity = myCustomTimerEntity;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobManager = commandContext.JobManager;

                var executableJobs = jobManager.FindNextJobsToExecute(new Page(0, 1));

                Assert.AreEqual(1, executableJobs.Count);
                Assert.AreEqual(_myCustomTimerEntity, executableJobs[0].JobHandlerConfigurationRaw);
                Assert.AreEqual(SuspensionStateFields.Active.StateCode, executableJobs[0].SuspensionState);

                executableJobs = jobManager.FindJobsByProcessInstanceId(_processInstanceId);
                Assert.AreEqual(1, executableJobs.Count);
                Assert.AreEqual(_myCustomTimerEntity, executableJobs[0].JobHandlerConfigurationRaw);
                Assert.AreEqual(SuspensionStateFields.Active.StateCode, executableJobs[0].SuspensionState);
                return null;
            }
        }
    }
}