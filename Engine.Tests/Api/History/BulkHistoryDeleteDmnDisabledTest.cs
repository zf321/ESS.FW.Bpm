using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BulkHistoryDeleteDmnDisabledTest
    {
        private readonly bool InstanceFieldsInitialized;

        public BulkHistoryDeleteDmnDisabledTest()
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

        [Test]
        public virtual void bulkHistoryDeleteWithDisabledDmn()
        {
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("someProcess")
                .StartEvent()
                .UserTask("userTask")
                .EndEvent()
                .Done();
            testRule.Deploy(model);
            var ids = prepareHistoricProcesses("someProcess");
            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="someProcess")
                .Count());
        }

        private IList<string> prepareHistoricProcesses(string businessKey)
        {
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < 5; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey(businessKey);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }
    }
}