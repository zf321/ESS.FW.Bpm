using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary> 
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    [TestFixture]
    public class HistoryCleanupOnEngineStartTest
    {
        private readonly bool InstanceFieldsInitialized;

        public HistoryCleanupOnEngineStartTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            engineRule = new ProvidedProcessEngineRule(bootstrapRule);
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
        }


        protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";

        protected internal ProcessEngineBootstrapRule bootstrapRule =
            new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                configuration.HistoryCleanupBatchWindowStartTime = "23:00";
                configuration.HistoryCleanupBatchWindowEndTime = "01:00";
                return configuration;
            }
        }

        protected internal ProvidedProcessEngineRule engineRule;
        public ProcessEngineTestRule testRule;

        private IHistoryService historyService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void init()
        {
            historyService = engineRule.ProcessEngine.HistoryService;
        }

        [TearDown]
        public virtual void clearDatabase()
        {
            var processEngineConfiguration =
                (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoryCleanupOnEngineStartTest outerInstance;

            public CommandAnonymousInnerClass(HistoryCleanupOnEngineStartTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobs = outerInstance.engineRule.ProcessEngine.ManagementService.CreateJobQuery()
                    
                    .ToList();
                if (jobs.Count > 0)
                {
                    Assert.AreEqual(1, jobs.Count);
                    var jobId = jobs[0].Id;
                    commandContext.JobManager.DeleteJob((JobEntity) jobs[0]);
                    commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(jobId);
                }

                return null;
            }
        }


        [Test]
        public virtual void testHistoryCleanupJob()
        {
            var historyCleanupJob = historyService.FindHistoryCleanupJob();
            Assert.NotNull(historyCleanupJob);
            var historyCleanupBatchWindowStartTime =
                ((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration)
                .HistoryCleanupBatchWindowStartTimeAsDate;
            Assert.AreEqual(
                HistoryCleanupHelper.GetNextRunWithinBatchWindow(ClockUtil.CurrentTime,
                    historyCleanupBatchWindowStartTime), historyCleanupJob.Duedate);
        }
    }
}