using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricProcessInstanceManagerTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
        }

        private readonly bool InstanceFieldsInitialized;

        public HistoricProcessInstanceManagerTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        protected internal const string OneTaskProcess = "oneTaskProcess";
        protected internal const string TWO_TASKS_PROCESS = "twoTasksProcess";

        public ProcessEngineRule engineRule = new ProcessEngineRule(true);
        public ProcessEngineTestRule testRule;

        private IRuntimeService runtimeService;

        [Test]
        [Deployment(new[] { "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/twoTasksProcess.bpmn20.xml" })]
        public virtual void testCountHistoricProcessInstanceIdsForCleanup()
        {
            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
            //start processes
            var ids = PrepareHistoricProcesses(OneTaskProcess, 35);
            ((List<string>) ids).AddRange(PrepareHistoricProcesses(TWO_TASKS_PROCESS, 65));

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //4 days passed
            ClockUtil.CurrentTime = DateTime.Now.AddDays(4);

            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoricProcessInstanceManagerTest outerInstance;

            public CommandAnonymousInnerClass(HistoricProcessInstanceManagerTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public object Execute(CommandContext commandContext)
            {
                //given
                //set different TTL for two process definition
                outerInstance.UpdateTimeToLive(commandContext, OneTaskProcess, 3);
                outerInstance.UpdateTimeToLive(commandContext, TWO_TASKS_PROCESS, 5);
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly HistoricProcessInstanceManagerTest outerInstance;

            public CommandAnonymousInnerClass2(HistoricProcessInstanceManagerTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public object Execute(CommandContext commandContext)
            {
                //when
                var count =
                    commandContext.HistoricProcessInstanceManager.FindHistoricProcessInstanceIdsForCleanupCount();

                //then
                Assert.AreEqual(35L, count);

                return null;
            }
        }

        private void UpdateTimeToLive(CommandContext commandContext, string businessKey, int timeToLive)
        {
            var processDefinitions = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==businessKey)
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            var processDefinition1 = (ProcessDefinitionEntity) processDefinitions[0];
            processDefinition1.HistoryTimeToLive = timeToLive;
            commandContext.ProcessDefinitionManager.Update(processDefinition1);
        }

        private IList<string> PrepareHistoricProcesses(string businessKey, int? processInstanceCount)
        {
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < processInstanceCount; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey(businessKey);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }
    }
}