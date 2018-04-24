//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Runtime
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class IncidentQueryTest
//    {
//        [SetUp]
//        public virtual void initServices()
//        {
//            runtimeService = engineRule.RuntimeService;
//            managementService = engineRule.ManagementService;
//        }

//        /// <summary>
//        ///     Setup starts 4 process instances of oneFailingServiceTaskProcess.
//        /// </summary>
//        [SetUp]
//        public virtual void startProcessInstances()
//        {
//            testHelper.Deploy(FAILING_SERVICE_TASK_MODEL);

//            processInstanceIds = new List<string>();
//            for (var i = 0; i < 4; i++)
//                processInstanceIds.Add(
//                    engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "")
//                        .Id);

//            testHelper.ExecuteAvailableJobs();
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
//        private readonly bool InstanceFieldsInitialized;
//        protected internal IManagementService managementService;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
//        //public RuleChain chain;

//        private IList<string> processInstanceIds;

//        protected internal IRuntimeService runtimeService;
//        public ProcessEngineTestRule testHelper;

//        public IncidentQueryTest()
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
//        [Test]
//        public virtual void testQuery()
//        {
//            var query = runtimeService.CreateIncidentQuery();
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByIncidentType()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.IncidentType==IncidentFields.FailedJobHandlerType);
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentType()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.IncidentType=="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByIncidentMessage()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.IncidentMessage=="Expected_exception.");
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentMessage()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.IncidentMessage=="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            var processDefinitionId = engineRule.RepositoryService.CreateProcessDefinitionQuery()
//                .First()
//                .Id;

//            var query = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==processDefinitionId);
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId=="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstanceIds[0]);

//            Assert.AreEqual(1, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(1, incidents.Count);

//            var incident = query.First();
//            Assert.NotNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessInstanceId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== "invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByIncidentId()
//        {
//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstanceIds[0])
//                .First();
//            Assert.NotNull(incident);

//            var query = runtimeService.CreateIncidentQuery(c=> c.Id==incident.Id);

//            Assert.AreEqual(1, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(1, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.Id=="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByExecutionId()
//        {
//            var execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstanceIds[0])
//                .First();
//            Assert.NotNull(execution);

//            var query = runtimeService.CreateIncidentQuery(c=>c.ExecutionId ==execution.Id);

//            Assert.AreEqual(1, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(1, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidExecutionId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=>c.ExecutionId =="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByActivityId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.ActivityId =="theServiceTask");
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.ActivityId =="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByConfiguration()
//        {
//            var jobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstanceIds[0])
//                .First()
//                .Id;

//            var query = runtimeService.CreateIncidentQuery(c=> c.Configuration==jobId);
//            Assert.AreEqual(1, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(1, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidConfiguration()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.Configuration=="invalid");

//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);

//            var incident = query.First();
//            Assert.IsNull(incident);
//        }

//        [Test]
//        public virtual void testQueryByCauseIncidentIdEqualsNull()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.CauseIncidentId==null);
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCauseIncidentId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=> c.CauseIncidentId=="invalid");
//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);
//            Assert.AreEqual(0, incidents.Count);
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/IncidentQueryTest.TestQueryByCauseIncidentId.bpmn"}) ]
//        public virtual void testQueryByCauseIncidentId()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("callFailingProcess");

//            testHelper.ExecuteAvailableJobs();

//            var subProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                /*.SetSuperProcessInstanceId(processInstance.Id)*/
//                .First();
//            Assert.NotNull(subProcessInstance);

//            var causeIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== subProcessInstance.Id)
//                .First();
//            Assert.NotNull(causeIncident);

//            var query = runtimeService.CreateIncidentQuery(c=> c.CauseIncidentId==causeIncident.Id);
//            Assert.AreEqual(2, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(2, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByRootCauseIncidentIdEqualsNull()
//        {
//            var query = runtimeService.CreateIncidentQuery()
//                .RootCauseIncidentId(null);
//            Assert.AreEqual(4, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(4, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByRootInvalidCauseIncidentId()
//        {
//            var query = runtimeService.CreateIncidentQuery()
//                .RootCauseIncidentId("invalid");
//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.True(incidents.Count == 0);
//            Assert.AreEqual(0, incidents.Count);
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/IncidentQueryTest.TestQueryByRootCauseIncidentId.bpmn", "resources/api/runtime/IncidentQueryTest.TestQueryByCauseIncidentId.bpmn"})]
//        public virtual void testQueryByRootCauseIncidentId()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("callFailingCallActivity");

//            testHelper.ExecuteAvailableJobs();

//            var subProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                /*.SetSuperProcessInstanceId(processInstance.Id)*/
//                .First();
//            Assert.NotNull(subProcessInstance);

//            var failingSubProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(subProcessInstance.Id)
//                .First();
//            Assert.NotNull(subProcessInstance);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== failingSubProcessInstance.Id)
//                .First();
//            Assert.NotNull(incident);

//            var query = runtimeService.CreateIncidentQuery()
//                .RootCauseIncidentId(incident.Id);
//            Assert.AreEqual(3, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.IsFalse(incidents.Count == 0);
//            Assert.AreEqual(3, incidents.Count);

//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//                // Exception is expected
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
//                .Id;
//            var jobDefinitionId2 = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId == processDefinitionId2)
//                .First()
//                .Id;

//            var query = runtimeService.CreateIncidentQuery()
//                //.JobDefinitionIdIn(jobDefinitionId1, jobDefinitionId2)
//                ;

//            Assert.AreEqual(2, query
//                .Count());
//            Assert.AreEqual(2, query.Count());

//            query = runtimeService.CreateIncidentQuery()
//                //.JobDefinitionIdIn(jobDefinitionId1)
//                ;

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());

//            query = runtimeService.CreateIncidentQuery()
//                //.JobDefinitionIdIn(jobDefinitionId2)
//                ;

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByUnknownJobDefinitionId()
//        {
//            var query = runtimeService.CreateIncidentQuery(c=>c.JobDefinitionId == "unknown");
//            Assert.AreEqual(0, query.Count());

//            var incidents = query
//                .ToList();
//            Assert.AreEqual(0, incidents.Count);
//        }

//        [Test]
//        public virtual void testQueryByNullJobDefinitionId()
//        {
//            try
//            {
//                runtimeService.CreateIncidentQuery(c=>c.JobDefinitionId == (string) null);
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
//                runtimeService.CreateIncidentQuery(c=>c.JobDefinitionId == null);
//                Assert.Fail("Should Assert.Fail");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("jobDefinitionIds is null"));
//            }
//        }

//        [Test]
//        public virtual void testQueryPaging()
//        {
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.ListPage(0, 4)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateIncidentQuery()
//                //.ListPage(2, 1)
//                .Count());
//            Assert.AreEqual(2, runtimeService.CreateIncidentQuery()
//                //.ListPage(1, 2)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateIncidentQuery()
//                //.ListPage(1, 4)
//                .Count());
//        }

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                .OrderByIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                .OrderByIncidentTimestamp()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByIncidentType()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByExecutionId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                ///*.OrderByActivityId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByCauseIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByRootCauseIncidentId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByConfiguration()
//                /*.Asc()*/
                
//                .Count());

//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                .OrderByIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                .OrderByIncidentTimestamp()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByIncidentType()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByExecutionId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                /*.OrderByActivityId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByCauseIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByRootCauseIncidentId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateIncidentQuery()
//                //.OrderByConfiguration()
//                /*.Desc()*/
                
//                .Count());
//        }
//    }
//}