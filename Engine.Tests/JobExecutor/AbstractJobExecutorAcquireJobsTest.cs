using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public abstract class AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public void SetUpEngineRule()
        {
            try
            {
                if (Rule.ProcessEngine == null)
                    Rule.InitializeProcessEngine();

                Rule.InitializeServices();

                Rule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [SetUp]
        public virtual void InitServices()
        {
            RuntimeService = Rule.RuntimeService;
            ManagementService = Rule.ManagementService;
        }

        [SetUp]
        public virtual void SaveProcessEngineConfiguration()
        {
            Configuration = (ProcessEngineConfigurationImpl)Rule.ProcessEngine.ProcessEngineConfiguration;
            _jobExecutorAcquireByDueDate = Configuration.JobExecutorAcquireByDueDate;
            _jobExecutorAcquireByPriority = Configuration.JobExecutorAcquireByPriority;
            _jobExecutorPreferTimerJobs = Configuration.JobExecutorPreferTimerJobs;
        }

        [SetUp]
        public virtual void SetClock()
        {
            ClockTestUtil.SetClockToDateWithoutMilliseconds();
        }

        [TearDown]
        public void TearDownEngineRule()
        {
            Rule.Finished();
        }

        [TearDown]
        public virtual void RestoreProcessEngineConfiguration()
        {
            Configuration.SetJobExecutorAcquireByDueDate(_jobExecutorAcquireByDueDate);
            Configuration.JobExecutorAcquireByPriority = _jobExecutorAcquireByPriority;
            Configuration.SetJobExecutorPreferTimerJobs(_jobExecutorPreferTimerJobs);
        }

        [TearDown]
        public virtual void ResetClock()
        {
            ClockUtil.Reset();
        }

        private bool _jobExecutorAcquireByDueDate;
        private bool _jobExecutorAcquireByPriority;
        private bool _jobExecutorPreferTimerJobs;

        protected internal ProcessEngineConfigurationImpl Configuration;

        protected internal IManagementService ManagementService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule Rule = new ProvidedProcessEngineRule();
        protected internal IRuntimeService RuntimeService;

        protected internal virtual IList<JobEntity> FindAcquirableJobs()
        {
            return Configuration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        protected internal virtual string StartProcess(string processDefinitionKey, string activity)
        {
            return
                RuntimeService.CreateProcessInstanceByKey(processDefinitionKey)
                    .StartBeforeActivity(activity)
                    .ExecuteWithVariablesInReturn()
                    .Id;
        }

        protected internal virtual void StartProcess(string processDefinitionKey, string activity, int times)
        {
            for (var i = 0; i < times; i++)
                StartProcess(processDefinitionKey, activity);
        }

        private class CommandAnonymousInnerClass : ICommand<IList<JobEntity>>
        {
            private readonly AbstractJobExecutorAcquireJobsTest _outerInstance;

            public CommandAnonymousInnerClass(AbstractJobExecutorAcquireJobsTest outerInstance)
            {
                _outerInstance = outerInstance;
            }


            public IList<JobEntity> Execute(CommandContext commandContext)
            {
                return commandContext.JobManager.FindNextJobsToExecute(new Page(0, 100));
            }
        }
    }
}