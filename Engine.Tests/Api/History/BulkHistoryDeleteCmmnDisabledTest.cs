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
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity) ]
    [TestFixture]
    public class BulkHistoryDeleteCmmnDisabledTest
    {
        private readonly bool InstanceFieldsInitialized;

        public BulkHistoryDeleteCmmnDisabledTest()
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
                configuration.CmmnEnabled = false;
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

            var historicProcessInstances = historyService.CreateHistoricProcessInstanceQuery()
                
                .ToList();
            foreach (var historicProcessInstance in historicProcessInstances)
                historyService.DeleteHistoricProcessInstance(historicProcessInstance.Id);

            var historicDecisionInstances = historyService.CreateHistoricDecisionInstanceQuery()
                
                .ToList();
            foreach (var historicDecisionInstance in historicDecisionInstances)
                historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly BulkHistoryDeleteCmmnDisabledTest outerInstance;

            public CommandAnonymousInnerClass(BulkHistoryDeleteCmmnDisabledTest outerInstance)
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

                commandContext.MeterLogManager.DeleteAll();

                return null;
            }
        }
        [Test][Deployment( new [] { "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/dmn/Example.Dmn" }) ]
        public virtual void historyCleanUpWithDisabledCmmn()
        {
            // given
            prepareHistoricProcesses(5);
            prepareHistoricDecisions(5);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = historyService.CleanUpHistoryAsync(true)
                .Id;

            engineRule.ManagementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricDecisionInstanceQuery()
                .Count());
        }

        private void prepareHistoricProcesses(int instanceCount)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < instanceCount; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
                processInstanceIds.Add(processInstance.Id);
            }
            var processDefinitions = engineRule.RepositoryService.CreateProcessDefinitionQuery()
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            engineRule.RepositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, 5);
            ClockUtil.CurrentTime = DateTime.Now.AddDays(-6);
            runtimeService.DeleteProcessInstances(processInstanceIds, null, true, true);
            ClockUtil.CurrentTime = oldCurrentTime;
        }

        private void prepareHistoricDecisions(int instanceCount)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            var decisionDefinitions = engineRule.RepositoryService.CreateDecisionDefinitionQuery(c=>c.Key =="decision")
                
                .ToList();
            Assert.AreEqual(1, decisionDefinitions.Count);
            engineRule.RepositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, 5);

            ClockUtil.CurrentTime = DateTime.Now.AddDays(-6);
            for (var i = 0; i < instanceCount; i++)
                engineRule.DecisionService.EvaluateDecisionByKey("decision")
                    .Variables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("status", "silver")
                        .PutValue("sum", 723))
                    .Evaluate();
            ClockUtil.CurrentTime = oldCurrentTime;
        }
    }
}