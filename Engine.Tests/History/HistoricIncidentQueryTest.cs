//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.History
//{
//    /// <summary>
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    [TestFixture]
//    public class HistoricIncidentQueryTest
//    {
//        [SetUp]
//        public virtual void initServices()
//        {
//            runtimeService = engineRule.RuntimeService;
//            managementService = engineRule.ManagementService;
//            historyService = engineRule.HistoryService;
//        }

//        private readonly bool InstanceFieldsInitialized;

//        public HistoricIncidentQueryTest()
//        {
//            if (!InstanceFieldsInitialized)
//            {
//                InitializeInstanceFields();
//                InstanceFieldsInitialized = true;
//            }
//        }

//        private void InitializeInstanceFields()
//        {
//            testHelper = new ProcessEngineTestRule(engineRule);
//            //chain = RuleChain.outerRule(engineRule).around(testHelper);
//        }


//        public static string PROCESS_DEFINITION_KEY = "oneFailingServiceTaskProcess";
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        public static IBpmnModelInstance FAILING_SERVICE_TASK_MODEL =
//            Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY)
//                .StartEvent("start")
//                .ServiceTask("task")
//                .CamundaAsyncBefore()
//                .CamundaClass(typeof(FailingDelegate).FullName)
//                .EndEvent("end")
//                .Done();

//        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//        public ProcessEngineTestRule testHelper;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
//        //public RuleChain chain;


//        protected internal IRuntimeService runtimeService;
//        protected internal IManagementService managementService;
//        protected internal IHistoryService historyService;

//        [Test]
//        public virtual void testQueryByInvalidIncidentType()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.IncidentType("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.IncidentType("invalid")
//                .Count());

//            try
//            {
//                query.IncidentType(null);
//                Assert.Fail("It was possible to set a null value as incidentType.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentMessage()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.IncidentMessage("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.IncidentMessage("invalid")
//                .Count());

//            try
//            {
//                query.IncidentMessage(null);
//                Assert.Fail("It was possible to set a null value as incidentMessage.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.Where(c=>c.ProcessDefinitionId=="invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.Where(c=>c.ProcessDefinitionId=="invalid")
//                .Count());

//            try
//            {
//                query.Where(c=>c.ProcessDefinitionId==null);
//                Assert.Fail("It was possible to set a null value as processDefinitionId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessInstanceId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.Where(c=>c.ProcessInstanceId=="invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.Where(c=>c.ProcessInstanceId=="invalid")
//                .Count());

//            try
//            {
//                query.Where(c=>c.ProcessInstanceId==null);
//                Assert.Fail("It was possible to set a null value as ProcessInstanceId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidExecutionId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.ExecutionId("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.ExecutionId("invalid")
//                .Count());

//            try
//            {
//                query.ExecutionId(null);
//                Assert.Fail("It was possible to set a null value as executionId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.ActivityId("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.ActivityId("invalid")
//                .Count());

//            try
//            {
//                query.ActivityId(null);
//                Assert.Fail("It was possible to set a null value as activityId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test][Deployment(new []{"resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml", "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"})]
//        public virtual void testQueryByCauseIncidentId()
//        {
//            startProcessInstance("process");

//            var ProcessInstanceId = runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                .First()
//                .Id;

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
//                .First();

//            var query = historyService.CreateHistoricIncidentQuery()
//                .CauseIncidentId(incident.Id);

//            Assert.AreEqual(2, query
//                .Count());
//            Assert.AreEqual(2, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidCauseIncidentId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.CauseIncidentId("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.CauseIncidentId("invalid")
//                .Count());

//            try
//            {
//                query.CauseIncidentId(null);
//                Assert.Fail("It was possible to set a null value as causeIncidentId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test][Deployment(new []{"resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml", "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"}) ]
//        public virtual void testQueryByRootCauseIncidentId()
//        {
//            startProcessInstance("process");

//            var ProcessInstanceId = runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                .First()
//                .Id;

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
//                .First();

//            var query = historyService.CreateHistoricIncidentQuery()
//                .RootCauseIncidentId(incident.Id);

//            Assert.AreEqual(2, query
//                .Count());
//            Assert.AreEqual(2, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidRootCauseIncidentId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.RootCauseIncidentId("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.RootCauseIncidentId("invalid")
//                .Count());

//            try
//            {
//                query.RootCauseIncidentId(null);
//                Assert.Fail("It was possible to set a null value as rootCauseIncidentId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidConfigurationId()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.Configuration("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.Configuration("invalid")
//                .Count());

//            try
//            {
//                query.Configuration(null);
//                Assert.Fail("It was possible to set a null value as configuration.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidOpen()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            try
//            {
//                query.Where(c=>c.Open);
//                Assert.Fail("It was possible to set a the open flag twice.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidResolved()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            try
//            {
//                query.Where(c=>c.Resolved)
//                    .Resolved();
//                Assert.Fail("It was possible to set a the resolved flag twice.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidDeleted()
//        {
//            var query = historyService.CreateHistoricIncidentQuery();

//            try
//            {
//                query.Deleted()
//                    .Deleted();
//                Assert.Fail("It was possible to set a the deleted flag twice.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByJobDefinitionId()
//        {
//            var processDefinitionId1 = testHelper.DeployAndGetDefinition(FAILING_SERVICE_TASK_MODEL)
//                .Id;
//            var processDefinitionId2 = testHelper.DeployAndGetDefinition(FAILING_SERVICE_TASK_MODEL)
//                .Id;

//            runtimeService.StartProcessInstanceById(processDefinitionId1);
//            runtimeService.StartProcessInstanceById(processDefinitionId2);
//            testHelper.ExecuteAvailableJobs();

//            var jobDefinitionId1 = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId == processDefinitionId1)
//                .First()
//                .JobDefinitionId;
//            var jobDefinitionId2 = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId == processDefinitionId2)
//                .First()
//                .JobDefinitionId;

//            var query = historyService.CreateHistoricIncidentQuery()
//                .JobDefinitionIdIn(jobDefinitionId1, jobDefinitionId2);

//            Assert.AreEqual(2, query
//                .Count());
//            Assert.AreEqual(2, query.Count());

//            query = historyService.CreateHistoricIncidentQuery()
//                .JobDefinitionIdIn(jobDefinitionId1);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());

//            query = historyService.CreateHistoricIncidentQuery()
//                .JobDefinitionIdIn(jobDefinitionId2);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByUnknownJobDefinitionId()
//        {
//            var processDefinitionId = testHelper.DeployAndGetDefinition(FAILING_SERVICE_TASK_MODEL)
//                .Id;

//            runtimeService.StartProcessInstanceById(processDefinitionId);
//            testHelper.ExecuteAvailableJobs();

//            var query = historyService.CreateHistoricIncidentQuery()
//                .JobDefinitionIdIn("unknown");

//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByNullJobDefinitionId()
//        {
//            try
//            {
//                historyService.CreateHistoricIncidentQuery()
//                    .JobDefinitionIdIn((string) null);
//                Assert.Fail("Should Assert.Fail");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("jobDefinitionIds contains null value"));
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullJobDefinitionIds()
//        {
//            try
//            {
//                historyService.CreateHistoricIncidentQuery()
//                    .JobDefinitionIdIn(null);
//                Assert.Fail("Should Assert.Fail");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("jobDefinitionIds is null"));
//            }
//        }

//        protected internal virtual void startProcessInstance(string key)
//        {
//            startProcessInstances(key, 1);
//        }

//        protected internal virtual void startProcessInstances(string key, int numberOfInstances)
//        {
//            for (var i = 0; i < numberOfInstances; i++)
//                runtimeService.StartProcessInstanceByKey(key);

//            testHelper.ExecuteAvailableJobs();
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByActivityId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var query = historyService.CreateHistoricIncidentQuery(c=> c.ActivityId =="theServiceTask");

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByConfiguration()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var configuration = managementService.CreateJobQuery()
//                .First()
//                .Id;

//            var query = historyService.CreateHistoricIncidentQuery()
//                .Configuration(configuration);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByDeleted()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var ProcessInstanceId = runtimeService.CreateProcessInstanceQuery()
//                .First()
//                .Id;
//            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

//            var query = historyService.CreateHistoricIncidentQuery()
//                .Deleted();

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByExecutionId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var pi = runtimeService.CreateProcessInstanceQuery()
//                .First();

//            var query = historyService.CreateHistoricIncidentQuery()
//                .ExecutionId(pi.Id);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByIncidentId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var incidentId = historyService.CreateHistoricIncidentQuery()
//                .First()
//                .Id;

//            var query = historyService.CreateHistoricIncidentQuery()
//                .IncidentId(incidentId);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessage()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var query = historyService.CreateHistoricIncidentQuery()
//                .IncidentMessage(FailingDelegate.EXCEPTION_MESSAGE);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByIncidentType()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var query = historyService.CreateHistoricIncidentQuery()
//                .IncidentType(IncidentFields.FailedJobHandlerType);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByInvalidIncidentId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(0, query.IncidentId("invalid")
                
//                .Count());
//            Assert.AreEqual(0, query.IncidentId("invalid")
//                .Count());

//            try
//            {
//                query.IncidentId(null);
//                Assert.Fail("It was possible to set a null value as incidentId.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByOpen()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var query = historyService.CreateHistoricIncidentQuery()
//                .Open();

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var pi = runtimeService.CreateProcessInstanceQuery()
//                .First();

//            var query = historyService.CreateHistoricIncidentQuery()
//                .ProcessDefinitionId(pi.ProcessDefinitionId);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var pi = runtimeService.CreateProcessInstanceQuery()
//                .First();

//            var query = historyService.CreateHistoricIncidentQuery(c=>c.ProcessInstanceId== pi.Id);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryByResolved()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY);

//            var jobId = managementService.CreateJobQuery()
//                .First()
//                .Id;
//            managementService.SetJobRetries(jobId, 1);

//            var query = historyService.CreateHistoricIncidentQuery()
//                .Resolved();

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQueryPaging()
//        {
//            startProcessInstances(PROCESS_DEFINITION_KEY, 4);

//            var query = historyService.CreateHistoricIncidentQuery();

//            Assert.AreEqual(4, query.ListPage(0, 4)
//                .Count());
//            Assert.AreEqual(1, query.ListPage(2, 1)
//                .Count());
//            Assert.AreEqual(2, query.ListPage(1, 2)
//                .Count());
//            Assert.AreEqual(3, query.ListPage(1, 4)
//                .Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
//        public virtual void testQuerySorting()
//        {
//            startProcessInstances(PROCESS_DEFINITION_KEY, 4);

//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByCreateTime()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByEndTime()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByIncidentType()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByExecutionId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                /*.OrderByActivityId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByCauseIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByRootCauseIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByConfiguration()
//                /*.Asc()*/
                
//                .Count());
//            //Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery().OrderByIncidentState()/*.Asc()*/.Count());

//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByCreateTime()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                .OrderByEndTime()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByIncidentType()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByExecutionId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                /*.OrderByActivityId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByCauseIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByRootCauseIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery()
//                //.OrderByConfiguration()
//                /*.Desc()*/
                
//                .Count());
//            //Assert.AreEqual(4, historyService.CreateHistoricIncidentQuery().OrderByIncidentState()/*.Desc()*/.Count());
//        }
//    }
//}