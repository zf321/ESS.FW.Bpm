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
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) 

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricProcessInstanceManagerProcessInstancesForCleanupTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void Init()
        {
            _runtimeService = EngineRule.RuntimeService;
            _historyService = EngineRule.HistoryService;
        }

        private readonly bool _instanceFieldsInitialized;

        public HistoricProcessInstanceManagerProcessInstancesForCleanupTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            TestRule = new ProcessEngineTestRule(EngineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        protected internal const string OneTaskProcess = "oneTaskProcess";
        protected internal const string TwoTasksProcess = "twoTasksProcess";

        public ProcessEngineRule EngineRule = new ProcessEngineRule(true);
        public ProcessEngineTestRule TestRule;

        private IHistoryService _historyService;
        private IRuntimeService _runtimeService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public int processDefiniotion1TTL;
        public int ProcessDefiniotion1TTL;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public int processDefiniotion2TTL;
        public int ProcessDefiniotion2TTL;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(2) public int processInstancesOfProcess1Count;
        public int ProcessInstancesOfProcess1Count;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(3) public int processInstancesOfProcess2Count;
        public int ProcessInstancesOfProcess2Count;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(4) public int daysPassedAfterProcessEnd;
        public int DaysPassedAfterProcessEnd;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(5) public int batchSize;
        public int BatchSize;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(6) public int resultCount;
        public int ResultCount;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios()
        public static ICollection<object[]> Scenarios()
        {
            return new[]
            {
                new object[] {3, 5, 3, 7, 4, 50, 3},
                new object[] {3, 5, 3, 7, 2, 50, 0},
                new object[] {3, 5, 3, 7, 6, 50, 10},
                new object[] {3, 5, 3, 7, 6, 4, 4}
            };
            //not enough time has passed
            //all historic process instances are old enough to be cleaned up
            //batchSize will reduce the result
        }

        [Test][Deployment( new []{ "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/twoTasksProcess.bpmn20.xml" })]
        public virtual void TestFindHistoricProcessInstanceIdsForCleanup()
        {
            EngineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
            //start processes
            var ids = PrepareHistoricProcesses(OneTaskProcess, ProcessInstancesOfProcess1Count);
            ((List<string>) ids).AddRange(PrepareHistoricProcesses(TwoTasksProcess, ProcessInstancesOfProcess2Count));

            _runtimeService.DeleteProcessInstances(ids, null, true, true);

            //some days passed
            ClockUtil.CurrentTime = DateTime.Now.AddDays(DaysPassedAfterProcessEnd);

            EngineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoricProcessInstanceManagerProcessInstancesForCleanupTest _outerInstance;

            public CommandAnonymousInnerClass(HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public object Execute(CommandContext commandContext)
            {
                //given
                //set different TTL for two process definition
                _outerInstance.UpdateTimeToLive(commandContext, OneTaskProcess, _outerInstance.ProcessDefiniotion1TTL);
                _outerInstance.UpdateTimeToLive(commandContext, TwoTasksProcess, _outerInstance.ProcessDefiniotion2TTL);
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly HistoricProcessInstanceManagerProcessInstancesForCleanupTest _outerInstance;

            public CommandAnonymousInnerClass2(
                HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public object Execute(CommandContext commandContext)
            {
                //when
                IList<string> historicProcessInstanceIdsForCleanup =
                    commandContext.HistoricProcessInstanceManager.FindHistoricProcessInstanceIdsForCleanup(
                        _outerInstance.BatchSize);

                //then
                Assert.AreEqual(_outerInstance.ResultCount, historicProcessInstanceIdsForCleanup.Count);

                if (_outerInstance.ResultCount > 0)
                {
                    var historicProcessInstances = _outerInstance._historyService.CreateHistoricProcessInstanceQuery()
                        //.ProcessInstanceIds(new HashSet<string>(historicProcessInstanceIdsForCleanup))
                        
                        .ToList();

                    foreach (var historicProcessInstance in historicProcessInstances)
                    {
                        Assert.NotNull(historicProcessInstance.EndTime);
                        var processDefinitions =
                            _outerInstance.EngineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Id ==historicProcessInstance.ProcessDefinitionId)
                                
                                .ToList();
                        Assert.AreEqual(1, processDefinitions.Count);
                        var processDefinition = (ProcessDefinitionEntity) processDefinitions[0];
                        Assert.True(historicProcessInstance.EndTime <
                                    ClockUtil.CurrentTime.AddDays(processDefinition.HistoryTimeToLive));
                    }
                }

                return null;
            }
        }

        private void UpdateTimeToLive(CommandContext commandContext, string businessKey, int timeToLive)
        {
            var processDefinitions = EngineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==businessKey)
                
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
                var processInstance = _runtimeService.StartProcessInstanceByKey(businessKey);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }
    }
}