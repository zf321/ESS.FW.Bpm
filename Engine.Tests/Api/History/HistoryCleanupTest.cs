using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Dmn.BusinessRuleTask;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoryCleanupTest
    {
        private readonly bool _instanceFieldsInitialized;

        public HistoryCleanupTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            EngineRule = new ProvidedProcessEngineRule(BootstrapRule);
            TestRule = new ProcessEngineTestRule(EngineRule);
            ////ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
        }


        private const int ProcessInstancesCount = 3;
        private const int DecisionsInProcessInstances = 3;
        private const int DecisionInstancesCount = 10;
        private const int CaseInstancesCount = 4;
        private const int HistoryTimeToLive = 5;
        private const int DaysInThePast = -6;
        protected internal const string OneTaskProcess = "oneTaskProcess";
        protected internal const string Decision = "decision";
        protected internal const string OneTaskCase = "case";

        protected internal ProcessEngineBootstrapRule BootstrapRule =
            new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                configuration.HistoryCleanupBatchSize = 20;
                configuration.HistoryCleanupBatchThreshold = 10;
                return configuration;
            }
        }

        protected internal ProvidedProcessEngineRule EngineRule;
        public ProcessEngineTestRule TestRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        ////public ExpectedException thrown = ExpectedException.None();

        private IHistoryService _historyService;
        private IRuntimeService _runtimeService;
        private IManagementService _managementService;
        private ICaseService _caseService;
        private IRepositoryService _repositoryService;
        private ProcessEngineConfigurationImpl _processEngineConfiguration;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
        ////public RuleChain ruleChain;

        [SetUp]
        public virtual void Init()
        {
            _runtimeService = EngineRule.RuntimeService;
            _historyService = EngineRule.HistoryService;
            _managementService = EngineRule.ManagementService;
            _caseService = EngineRule.CaseService;
            _repositoryService = EngineRule.RepositoryService;
            _processEngineConfiguration = EngineRule.ProcessEngineConfiguration;
            TestRule.Deploy("resources/api/oneTaskProcess.bpmn20.xml", "resources/api/dmn/Example.Dmn",
                "resources/api/cmmn/oneTaskCaseWithHistoryTimeToLive.cmmn");
        }

        [TearDown]
        public virtual void ClearDatabase()
        {
            //reset configuration changes
            var defaultStartTime = _processEngineConfiguration.HistoryCleanupBatchWindowStartTime;
            var defaultEndTime = _processEngineConfiguration.HistoryCleanupBatchWindowEndTime;
            var defaultBatchSize = _processEngineConfiguration.HistoryCleanupBatchSize;

            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = defaultStartTime;
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = defaultEndTime;
            _processEngineConfiguration.HistoryCleanupBatchSize = defaultBatchSize;

            _processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));

            var historicProcessInstances = _historyService.CreateHistoricProcessInstanceQuery()
                
                .ToList();
            foreach (var historicProcessInstance in historicProcessInstances)
                _historyService.DeleteHistoricProcessInstance(historicProcessInstance.Id);

            var historicDecisionInstances = _historyService.CreateHistoricDecisionInstanceQuery()
                
                .ToList();
            foreach (var historicDecisionInstance in historicDecisionInstances)
                _historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

            var historicCaseInstances = _historyService.CreateHistoricCaseInstanceQuery()
                
                .ToList();
            foreach (var historicCaseInstance in historicCaseInstances)
                _historyService.DeleteHistoricCaseInstance(historicCaseInstance.Id);

            ClearMetrics();
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoryCleanupTest _outerInstance;

            public CommandAnonymousInnerClass(HistoryCleanupTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobs = _outerInstance._managementService.CreateJobQuery()
                    
                    .ToList();
                if (jobs.Count > 0)
                {
                    Assert.AreEqual(1, jobs.Count);
                    var jobId = jobs[0].Id;
                    commandContext.JobManager.DeleteJob((JobEntity) jobs[0]);
                    commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(jobId);
                }

                var historicIncidents = _outerInstance._historyService.CreateHistoricIncidentQuery()
                    
                    .ToList();
                foreach (var historicIncident in historicIncidents)
                    commandContext.HistoricIncidentManager.Delete((HistoricIncidentEntity) historicIncident);

                commandContext.MeterLogManager.DeleteAll();

                return null;
            }
        }

        protected internal virtual void ClearMetrics()
        {
            var meters = _processEngineConfiguration.MetricsRegistry.Meters.Values;
            foreach (var meter in meters)
            {
                var meterAndClear = meter.AndClear;
            }
            _managementService.DeleteMetrics(null);
        }

        [Test]
        public virtual void TestHistoryCleanupManualRun()
        {
            //given
            PrepareData(15);

            ClockUtil.CurrentTime = DateTime.Now;
            //when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            AssertResult(0);
        }

        [Test][Deployment( new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" }) ]
        public virtual void TestHistoryCleanupOnlyDecisionInstancesRemoved()
        {
            // given
            PrepareInstances(null, HistoryTimeToLive, null);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(ProcessInstancesCount, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricDecisionInstanceQuery()
                .Count());
            Assert.AreEqual(CaseInstancesCount, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        [Test][Deployment(new []{ "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn"}) ]
        public virtual void TestHistoryCleanupOnlyProcessInstancesRemoved()
        {
            // given
            PrepareInstances(HistoryTimeToLive, null, null);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(0, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(DecisionInstancesCount + DecisionsInProcessInstances,
                _historyService.CreateHistoricDecisionInstanceQuery()
                    .Count());
            Assert.AreEqual(CaseInstancesCount, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        [Test][Deployment(new[ ]{ "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupOnlyCaseInstancesRemoved()
        {
            // given
            PrepareInstances(null, null, HistoryTimeToLive);

            ClockUtil.CurrentTime = DateTime.Now;

            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(ProcessInstancesCount, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(DecisionInstancesCount + DecisionsInProcessInstances,
                _historyService.CreateHistoricDecisionInstanceQuery()
                    .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }
        
        [Test]
        [Deployment(new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupOnlyDecisionInstancesNotRemoved()
        {
            // given
            PrepareInstances(HistoryTimeToLive, null, HistoryTimeToLive);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(0, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(DecisionInstancesCount + DecisionsInProcessInstances,
                _historyService.CreateHistoricDecisionInstanceQuery()
                    .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupOnlyProcessInstancesNotRemoved()
        {
            // given
            PrepareInstances(null, HistoryTimeToLive, HistoryTimeToLive);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(ProcessInstancesCount, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricDecisionInstanceQuery()
                .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupOnlyCaseInstancesNotRemoved()
        {
            // given
            PrepareInstances(HistoryTimeToLive, HistoryTimeToLive, null);

            ClockUtil.CurrentTime = DateTime.Now;

            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(0, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(0, _historyService.CreateHistoricDecisionInstanceQuery()
                .Count());
            Assert.AreEqual(CaseInstancesCount, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupEverythingRemoved()
        {
            // given
            PrepareInstances(HistoryTimeToLive, HistoryTimeToLive, HistoryTimeToLive);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            AssertResult(0);
        }

        [Test]
        [Deployment(new[] { "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml", "resources/api/authorization/oneTaskCase.cmmn" })]
        public virtual void TestHistoryCleanupNothingRemoved()
        {
            // given
            PrepareInstances(null, null, null);

            ClockUtil.CurrentTime = DateTime.Now;
            // when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(ProcessInstancesCount, _historyService.CreateHistoricProcessInstanceQuery()
                .Count());
            Assert.AreEqual(DecisionInstancesCount + DecisionsInProcessInstances,
                _historyService.CreateHistoricDecisionInstanceQuery()
                    .Count());
            Assert.AreEqual(CaseInstancesCount, _historyService.CreateHistoricCaseInstanceQuery()
                .Count());
        }

        private void PrepareInstances(int? processInstanceTimeToLive, int? decisionTimeToLive, int? caseTimeToLive)
        {
            //update time to live
            var processDefinitions = _repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="testProcess")
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            _repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id,
                processInstanceTimeToLive);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<DecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== "testDecision").ToList();
            var decisionDefinitions = _repositoryService.CreateDecisionDefinitionQuery(c=>c.Key =="testDecision")
                
                .ToList();
            Assert.AreEqual(1, decisionDefinitions.Count);
            _repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, decisionTimeToLive);

            var caseDefinitions = _repositoryService.CreateCaseDefinitionQuery()
                //.CaseDefinitionKey("oneTaskCase")
                
                .ToList();
            Assert.AreEqual(1, caseDefinitions.Count);
            _repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, caseTimeToLive);

            var oldCurrentTime = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = DateTime.Now.AddDays(DaysInThePast);

            //create 3 process instances
            IList<string> processInstanceIds = new List<string>();
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("pojo", new TestPojo("okay", 13.37));
            for (var i = 0; i < ProcessInstancesCount; i++)
            {
                var processInstance = _runtimeService.StartProcessInstanceByKey("testProcess", variables);
                processInstanceIds.Add(processInstance.Id);
            }
            _runtimeService.DeleteProcessInstances(processInstanceIds, null, true, true);

            //+10 standalone decisions
            for (var i = 0; i < DecisionInstancesCount; i++)
                EngineRule.DecisionService.EvaluateDecisionByKey("testDecision")
                    .Variables(variables)
                    .Evaluate();

            // create 4 process instances
            for (var i = 0; i < CaseInstancesCount; i++)
            {
                var caseInstance = _caseService.CreateCaseInstanceByKey("oneTaskCase",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("pojo", new TestPojo("okay", 13.37 + i)));
                _caseService.TerminateCaseExecution(caseInstance.Id);
                _caseService.CloseCaseInstance(caseInstance.Id);
            }

            ClockUtil.CurrentTime = oldCurrentTime;
        }

        [Test]
        public virtual void TestHistoryCleanupWithinBatchWindow()
        {
            //given
            PrepareData(15);

            //we're within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(HistoryTimeToLive)
                .ToString("HH:mm");
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync(false)
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            AssertResult(0);
        }

        [Test]
        public virtual void TestHistoryCleanupJobNullTtl()
        {
            //given
            RemoveHistoryTimeToLive();

            PrepareData(15);

            ClockUtil.CurrentTime = DateTime.Now;
            //when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            AssertResult(15);
        }

        private void RemoveHistoryTimeToLive()
        {
            var processDefinitions = _repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==OneTaskProcess)
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            _repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<DecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION).ToList();
            var decisionDefinitions = _repositoryService.CreateDecisionDefinitionQuery(c=>c.Key ==Decision)
                
                .ToList();
            Assert.AreEqual(1, decisionDefinitions.Count);
            _repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<CaseDefinition> caseDefinitions = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey(ONE_TASK_CASE).ToList();
            var caseDefinitions = _repositoryService.CreateCaseDefinitionQuery()
                //.CaseDefinitionKey(OneTaskCase)
                
                .ToList();
            Assert.AreEqual(1, caseDefinitions.Count);
            _repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, null);
        }

        [Test][Deployment( "resources/api/twoTasksProcess.bpmn20.xml" )]
        public virtual void TestHistoryCleanupJobDefaultTtl()
        {
            //given
            PrepareBpmnData(15, "twoTasksProcess");

            ClockUtil.CurrentTime = DateTime.Now;
            //when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            AssertResult(15);
        }

        [Test]
        public virtual void TestFindHistoryCleanupJob()
        {
            //given
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            //when
            var historyCleanupJob = _historyService.FindHistoryCleanupJob();

            //then
            Assert.NotNull(historyCleanupJob);
            Assert.AreEqual(jobId, historyCleanupJob.Id);
        }

        [Test]
        public virtual void TestRescheduleForNever()
        {
            //given

            //force creation of job
            _historyService.CleanUpHistoryAsync(true);
            var historyCleanupJob = (JobEntity) _historyService.FindHistoryCleanupJob();
            Assert.NotNull(historyCleanupJob);
            Assert.NotNull(historyCleanupJob.Duedate);

            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = null;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = null;
            _processEngineConfiguration.InitHistoryCleanup();

            ClockUtil.CurrentTime = DateTime.Now;

            //when
            _historyService.CleanUpHistoryAsync(false);

            //then
            historyCleanupJob = (JobEntity) _historyService.FindHistoryCleanupJob();
            Assert.AreEqual(SuspensionStateFields.Suspended.StateCode, historyCleanupJob.SuspensionState);
            Assert.IsNull(historyCleanupJob.Duedate);
        }

        [Test]
        public virtual void TestHistoryCleanupJobResolveIncident()
        {
            //given
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;
            ImitateFailedJob(jobId);

            //when
            //call to cleanup history means that incident was resolved
            jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            //then
            var jobEntity = GetJobEntity(jobId);
            Assert.AreEqual(null, jobEntity.ExceptionByteArrayId);
            Assert.AreEqual(null, jobEntity.ExceptionMessage);
        }
        
        private void ImitateFailedJob(string jobId)
        {
            _processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, jobId));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly HistoryCleanupTest _outerInstance;

            private readonly string _jobId;

            public CommandAnonymousInnerClass2(HistoryCleanupTest outerInstance, string jobId)
            {
                _outerInstance = outerInstance;
                _jobId = jobId;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobEntity = _outerInstance.GetJobEntity(_jobId);
                jobEntity.Retries = 0;
                jobEntity.ExceptionMessage = "Something bad happened";
                jobEntity.ExceptionStacktrace =
                    ExceptionUtil.GetExceptionStacktrace(new System.Exception("Something bad happened"));
                return null;
            }
        }

        [Test]
        public virtual void TestLessThanThresholdManualRun()
        {
            //given
            PrepareData(5);

            ClockUtil.CurrentTime = DateTime.Now;
            //when
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            Assert.AreEqual(0, _historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==OneTaskProcess)
                .Count());

            var jobEntity = GetJobEntity(jobId);
            Assert.AreEqual(SuspensionStateFields.Suspended.StateCode, jobEntity.SuspensionState);
        }

        [Test]
        public virtual void TestNotEnoughTimeToDeleteEverything()
        {
            //given
            //we have something to cleanup
            PrepareData(40);
            //we call history cleanup within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(HistoryTimeToLive)
                .ToString("HH:mm"); //now.AddHours(HistoryTimeToLive).ToString("HH:mm");
            _processEngineConfiguration.InitHistoryCleanup();
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            //job is executed once within batch window
            _managementService.ExecuteJob(jobId);

            //when
            //time passed -> outside batch window
            ClockUtil.CurrentTime = now.AddHours(6);
            //the job is called for the second time
            _managementService.ExecuteJob(jobId);

            //then
            //second execution was not able to Delete rest data
            AssertResult(20);
        }

        [Test]
        public virtual void TestManualRunDoesNotRespectBatchWindow()
        {
            //given
            //we have something to cleanup
            var processInstanceCount = 40;
            PrepareData(processInstanceCount);

            //we call history cleanup outside batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.AddHours(1)
                .ToString("HH:mm"); //(new SimpleDateFormat("HH:mm")).Format(DateUtils.AddHours(now, 1)); //now + 1 hour
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(HistoryTimeToLive)
                .ToString("HH:mm"); //now + 5 hours
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            //job is executed before batch window start
            var jobId = _historyService.CleanUpHistoryAsync(true)
                .Id;
            _managementService.ExecuteJob(jobId);

            //the job is called for the second time after batch window end
            ClockUtil.CurrentTime = now.AddHours(6); //now + 6 hours
            _managementService.ExecuteJob(jobId);

            //then
            AssertResult(0);
        }


        [Test]
        public virtual void TestLessThanThresholdWithinBatchWindow()
        {
            //given
            PrepareData(5);

            //we're within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(HistoryTimeToLive)
                .ToString("HH:mm");
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;

            _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till current time + delay
            var nextRun = GetNextRunWithDelay(ClockUtil.CurrentTime, 0);
            Assert.True(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
            var nextRunMax = ClockUtil.CurrentTime.AddSeconds(HistoryCleanupJobHandlerConfiguration.MaxDelay);
            Assert.True(jobEntity.Duedate < nextRunMax);

            //countEmptyRuns incremented
            Assert.AreEqual(1, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        private DateTime GetNextRunWithDelay(DateTime date, int countEmptyRuns)
        {
            //ignore milliseconds because MySQL does not support them, and it's not important for test
            var result = new DateTime();
                //DateUtils.SetMilliseconds(DateUtils.AddSeconds(date, Math.Min((int)(Math.Pow(2.0, countEmptyRuns) * HistoryCleanupJobHandlerConfiguration.StartDelay), HistoryCleanupJobHandlerConfiguration.MaxDelay)), 0);
            return result;
        }

        private JobEntity GetJobEntity(string jobId)
        {
            return (JobEntity) _managementService.CreateJobQuery(c=>c.Id == jobId)
                
                .First();
        }

        [Test]
        public virtual void TestLessThanThresholdWithinBatchWindowAgain()
        {
            //given
            PrepareData(5);

            //we're within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(1)
                .ToString("HH:mm"); //(new SimpleDateFormat("HH:mm")).Format(DateUtils.AddHours(now, 1));
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            for (var i = 1; i <= 6; i++)
                _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till current time + (2 power Count)*delay
            var nextRun = GetNextRunWithDelay(ClockUtil.CurrentTime, HistoryTimeToLive);
            Assert.True(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
            var nextRunMax = ClockUtil.CurrentTime.AddSeconds(HistoryCleanupJobHandlerConfiguration.MaxDelay);
            Assert.True(jobEntity.Duedate < nextRunMax);

            //countEmptyRuns incremented
            Assert.AreEqual(6, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        [Test]
        public virtual void TestLessThanThresholdWithinBatchWindowMaxDelayReached()
        {
            //given
            PrepareData(5);

            //we're within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddHours(2)
                .ToString("HH:mm"); //(new SimpleDateFormat("HH:mm")).Format(DateUtils.AddHours(now, 2));
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            for (var i = 1; i <= 11; i++)
                _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till current time + max delay
            var nextRun = GetNextRunWithDelay(ClockUtil.CurrentTime, 10);
            Assert.True(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
            Assert.True(jobEntity.Duedate < GetNextRunWithinBatchWindow(now));

            //countEmptyRuns incremented
            Assert.AreEqual(11, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        [Test]
        public virtual void TestLessThanThresholdCloseToBatchWindowEndTime()
        {
            //given
            PrepareData(5);

            //we're within batch window
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = now.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(now);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = now.AddMinutes(30)
                .ToString("HH:mm"); // (new SimpleDateFormat("HH:mm")).Format(DateUtils.AddMinutes(now, 30));
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            for (var i = 1; i <= 9; i++)
                _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till next batch window start time
            var nextRun = GetNextRunWithinBatchWindow(ClockUtil.CurrentTime);
            Assert.True(jobEntity.Duedate.Equals(nextRun));

            //countEmptyRuns canceled
            Assert.AreEqual(0, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        [Test]
        public virtual void TestLessThanThresholdOutsideBatchWindow()
        {
            //given
            PrepareData(5);

            //we're outside batch window
            var twoHoursAgo = DateTime.Now;
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = twoHoursAgo.ToString("HH:mm");
                //(new SimpleDateFormat("HH:mm")).Format(twoHoursAgo);
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = twoHoursAgo.AddHours(1)
                .ToString("HH:mm"); //(new SimpleDateFormat("HH:mm")).Format(DateUtils.AddHours(twoHoursAgo, 1));

            _processEngineConfiguration.InitHistoryCleanup();
            ClockUtil.CurrentTime = twoHoursAgo.AddHours(2); // DateUtils.AddHours(twoHoursAgo, 2);

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            for (var i = 1; i <= 3; i++)
                _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till next batch window start
            var nextRun = GetNextRunWithinBatchWindow(ClockUtil.CurrentTime);
            Assert.True(jobEntity.Duedate.Equals(nextRun));

            //countEmptyRuns canceled
            Assert.AreEqual(0, configuration.CountEmptyRuns);

            //nothing was removed
            AssertResult(5);
        }

        [Test]
        public virtual void TestLessThanThresholdOutsideBatchWindowAfterMidnight()
        {
            //given
            PrepareData(5);

            //we're outside batch window, batch window passes midnight
            var date = DateTime.Now;
            ClockUtil.CurrentTime = new DateTime(date.Year, date.Month, date.Day, 1, 10, 0);
                //DateUtils.SetMinutes(DateUtils.SetHours(date, 1), 10); //01:10
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till next batch window start
            var nextRun = GetNextRunWithinBatchWindow(ClockUtil.CurrentTime);
            Assert.True(jobEntity.Duedate.Equals(nextRun));
            Assert.True(nextRun > ClockUtil.CurrentTime);

            //countEmptyRuns canceled
            Assert.AreEqual(0, configuration.CountEmptyRuns);

            //nothing was removed
            AssertResult(5);
        }

        [Test]
        public virtual void TestLessThanThresholdOutsideBatchWindowBeforeMidnight()
        {
            //given
            PrepareData(5);

            //we're outside batch window, batch window passes midnight
            var date = DateTime.Now;
            ClockUtil.CurrentTime = new DateTime(date.Year, date.Month, date.Day, 22, 10, 0);
                //DateUtils.SetMinutes(DateUtils.SetHours(date, 22), 10); //22:10
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;
            _managementService.ExecuteJob(jobId);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till next batch window start
            var nextRun = GetNextRunWithinBatchWindow(ClockUtil.CurrentTime);
            Assert.True(jobEntity.Duedate.Equals(nextRun));
            Assert.True(nextRun > ClockUtil.CurrentTime);

            //countEmptyRuns cancelled
            Assert.AreEqual(0, configuration.CountEmptyRuns);

            //nothing was removed
            AssertResult(5);
        }

        [Test]
        public virtual void TestLessThanThresholdWithinBatchWindowBeforeMidnight()
        {
            //given
            PrepareData(5);

            //we're within batch window, but batch window passes midnight
            var date = DateTime.Now;
            ClockUtil.CurrentTime = new DateTime(date.Year, date.Month, date.Day, 23, 10, 0);
                //DateUtils.SetMinutes(DateUtils.SetHours(date, 23), 10); //23:10
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;

            ExecuteJobHelper.ExecuteJob(jobId, _processEngineConfiguration.CommandExecutorTxRequired);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till current time + delay
            var nextRun = GetNextRunWithDelay(ClockUtil.CurrentTime, 0);
            Assert.True(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
            var nextRunMax = ClockUtil.CurrentTime.AddSeconds(HistoryCleanupJobHandlerConfiguration.MaxDelay);
            Assert.True(jobEntity.Duedate < nextRunMax);

            //countEmptyRuns incremented
            Assert.AreEqual(1, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        [Test]
        public virtual void TestLessThanThresholdWithinBatchWindowAfterMidnight()
        {
            //given
            PrepareData(5);

            //we're within batch window, but batch window passes midnight
            var date = DateTime.Now;
            ClockUtil.CurrentTime = new DateTime(date.Year, date.Month, date.Day, 0, 10, 0);
                // DateUtils.SetMinutes(DateUtils.SetHours(date, 0), 10); //00:10
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
            _processEngineConfiguration.InitHistoryCleanup();

            //when
            var jobId = _historyService.CleanUpHistoryAsync()
                .Id;

            ExecuteJobHelper.ExecuteJob(jobId, _processEngineConfiguration.CommandExecutorTxRequired);

            //then
            var jobEntity = GetJobEntity(jobId);
            var configuration = GetConfiguration(jobEntity);

            //job rescheduled till current time + delay
            var nextRun = GetNextRunWithDelay(ClockUtil.CurrentTime, 0);
            Assert.True(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
            var nextRunMax = ClockUtil.CurrentTime.AddSeconds(HistoryCleanupJobHandlerConfiguration.MaxDelay);
            Assert.True(jobEntity.Duedate < nextRunMax);

            //countEmptyRuns incremented
            Assert.AreEqual(1, configuration.CountEmptyRuns);

            //data is still removed
            AssertResult(0);
        }

        [Test]
        public virtual void TestConfiguration()
        {
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00+0200";
            _processEngineConfiguration.InitHistoryCleanup();
            var c = DateTime.Now; //(TimeZone.GetTimeZone("GMT+2:00"));
            var startTime = _processEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
            c = new DateTime(startTime.Ticks);
            Assert.AreEqual(23, c.Hour);
            Assert.AreEqual(0, c.Minute);
            Assert.AreEqual(0, c.Second);

            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.InitHistoryCleanup();
            c = new DateTime();
            startTime = _processEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
            c = new DateTime(startTime.Ticks);
            Assert.AreEqual(23, c.Hour);
            Assert.AreEqual(0, c.Minute);
            Assert.AreEqual(0, c.Second);

            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:35-0800";
            _processEngineConfiguration.InitHistoryCleanup();
            c = DateTime.Now; //DateTime.GetInstance(TimeZone.GetTimeZone("GMT-8:00"));
            var endTime = _processEngineConfiguration.HistoryCleanupBatchWindowEndTimeAsDate;
            c = new DateTime(endTime.Ticks);
            Assert.AreEqual(1, c.Hour);
            Assert.AreEqual(35, c.Minute);
            Assert.AreEqual(0, c.Second);

            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:35";
            _processEngineConfiguration.InitHistoryCleanup();
            c = new DateTime();
            endTime = _processEngineConfiguration.HistoryCleanupBatchWindowEndTimeAsDate;
            c = new DateTime(endTime.Ticks);
            Assert.AreEqual(1, c.Hour);
            Assert.AreEqual(35, c.Minute);
            Assert.AreEqual(0, c.Second);

            _processEngineConfiguration.HistoryCleanupBatchSize = 500;
            _processEngineConfiguration.InitHistoryCleanup();
            Assert.AreEqual(_processEngineConfiguration.HistoryCleanupBatchSize, 500);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void TestConfigurationFailureWrongStartTime()
        {
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";

            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("historyCleanupBatchWindowStartTime");

            _processEngineConfiguration.InitHistoryCleanup();
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void TestConfigurationFailureWrongEndTime()
        {
            _processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
            _processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "wrongValue";

            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("historyCleanupBatchWindowEndTime");

            _processEngineConfiguration.InitHistoryCleanup();
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void TestConfigurationFailureWrongBatchSize()
        {
            _processEngineConfiguration.HistoryCleanupBatchSize = 501;

            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("historyCleanupBatchSize");

            _processEngineConfiguration.InitHistoryCleanup();
        }

        private DateTime GetNextRunWithinBatchWindow(DateTime currentTime)
        {
            var batchWindowStartTime = _processEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
            return GetNextRunWithinBatchWindow(currentTime, batchWindowStartTime);
        }

        public virtual DateTime GetNextRunWithinBatchWindow(DateTime date, DateTime batchWindowStartTime)
        {
            var todayPossibleRun = UpdateTime(date, batchWindowStartTime);
            if (todayPossibleRun > date)
                return todayPossibleRun;
            return todayPossibleRun.AddDays(1);
        }

        private DateTime UpdateTime(DateTime now, DateTime newTime)
        {
            var result = now;

            var newTimeCalendar = new DateTime();
            newTimeCalendar = new DateTime(newTime.Ticks);

            //result = DateUtils.SetHours(result, newTimeCalendar.Hour);
            //result = DateUtils.SetMinutes(result, newTimeCalendar.Minute);
            //result = DateUtils.SetSeconds(result, newTimeCalendar.Second);
            //result = DateUtils.SetMilliseconds(result, newTimeCalendar.Millisecond);
            return result;
        }

        private HistoryCleanupJobHandlerConfiguration GetConfiguration(JobEntity jobEntity)
        {
            var jobHandlerConfigurationRaw = jobEntity.JobHandlerConfigurationRaw;
            return HistoryCleanupJobHandlerConfiguration.FromJson(new JObject(jobHandlerConfigurationRaw));
        }


        private void PrepareData(int instanceCount)
        {
            var createdInstances = instanceCount / 3;
            PrepareBpmnData(createdInstances, OneTaskProcess);
            PrepareDmnData(createdInstances);
            PrepareCmmnData(instanceCount - 2 * createdInstances);
        }

        private void PrepareBpmnData(int instanceCount, string businesskey)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = DateTime.Now.AddDays(DaysInThePast);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses(businesskey, getVariables(), instanceCount);
            var ids = PrepareHistoricProcesses(businesskey, Variables, instanceCount);
            _runtimeService.DeleteProcessInstances(ids, null, true, true);
            ClockUtil.CurrentTime = oldCurrentTime;
        }

        private void PrepareDmnData(int instanceCount)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = DateTime.Now.AddDays(DaysInThePast);
            for (var i = 0; i < instanceCount; i++)
                EngineRule.DecisionService.EvaluateDecisionByKey(Decision)
                    .Variables(DmnVariables)
                    .Evaluate();
            ClockUtil.CurrentTime = oldCurrentTime;
        }

        private void PrepareCmmnData(int instanceCount)
        {
            var oldCurrentTime = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = DateTime.Now.AddDays(DaysInThePast);

            for (var i = 0; i < instanceCount; i++)
            {
                var caseInstance = _caseService.CreateCaseInstanceByKey(OneTaskCase);
                _caseService.TerminateCaseExecution(caseInstance.Id);
                _caseService.CloseCaseInstance(caseInstance.Id);
            }
            ClockUtil.CurrentTime = oldCurrentTime;
        }

        private IList<string> PrepareHistoricProcesses(string businessKey, IVariableMap variables,
            int? processInstanceCount)
        {
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < processInstanceCount; i++)
            {
                var processInstance = _runtimeService.StartProcessInstanceByKey(businessKey, variables);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }

        private IVariableMap Variables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("aVariableName", "aVariableValue")
                    .PutValue("anotherVariableName", "anotherVariableValue");
            }
        }

        protected internal virtual IVariableMap DmnVariables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("status", "silver")
                    .PutValue("sum", 723);
            }
        }

        private void AssertResult(long expectedInstanceCount)
        {
            var count = _historyService.CreateHistoricProcessInstanceQuery()
                            .Count() + _historyService.CreateHistoricDecisionInstanceQuery()
                            .Count() + _historyService.CreateHistoricCaseInstanceQuery()
                            .Count();
            Assert.AreEqual(expectedInstanceCount, count);
        }
    }
}