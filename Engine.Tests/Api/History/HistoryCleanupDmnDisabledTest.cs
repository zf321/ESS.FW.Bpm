using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    [TestFixture]
    public class HistoryCleanupDmnDisabledTest
    {
        private readonly bool InstanceFieldsInitialized;

        public HistoryCleanupDmnDisabledTest()
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


        protected internal ProcessEngineBootstrapRule bootstrapRule =
            new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                configuration.DmnEnabled = false;
                return configuration;
            }
        }

        protected internal ProvidedProcessEngineRule engineRule;
        public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
        //public RuleChain ruleChain;

        private IRuntimeService runtimeService;
        private IHistoryService historyService;

        [SetUp]
        public virtual void createProcessEngine()
        {
            runtimeService = engineRule.RuntimeService;
            historyService = engineRule.HistoryService;
        }

        [TearDown]
        public virtual void clearDatabase()
        {
            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoryCleanupDmnDisabledTest outerInstance;

            public CommandAnonymousInnerClass(HistoryCleanupDmnDisabledTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobs = outerInstance.engineRule.ManagementService.CreateJobQuery()
                    
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

        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml") ]
        public virtual void historyCleanupWithDisabledDmn()
        {
            prepareHistoricProcesses("oneTaskProcess");

            ClockUtil.CurrentTime = DateTime.Now;
            //when
            var jobId = historyService.CleanUpHistoryAsync(true)
                .Id;

            engineRule.ManagementService.ExecuteJob(jobId);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess")
                .Count());
        }

        private void prepareHistoricProcesses(string businessKey)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = DateTime.Now.AddDays(-6);

            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < 5; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey(businessKey);
                processInstanceIds.Add(processInstance.Id);
            }
            runtimeService.DeleteProcessInstances(processInstanceIds, null, true, true);

            ClockUtil.CurrentTime = oldCurrentTime;
        }
    }
}