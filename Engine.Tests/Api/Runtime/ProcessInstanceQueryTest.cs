//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration.Models;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Engine.Variable;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Runtime
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class ProcessInstanceQueryTest
//    {
//        [SetUp]
//        public virtual void initServices()
//        {
//            runtimeService = engineRule.RuntimeService;
//            repositoryService = engineRule.RepositoryService;
//            managementService = engineRule.ManagementService;
//            caseService = engineRule.CaseService;
//        }


//        /// <summary>
//        ///     Setup starts 4 process instances of oneTaskProcess
//        ///     and 1 instance of otherOneTaskProcess
//        /// </summary>
//        [SetUp]
//        public virtual void deployTestProcesses()
//        {
//            var deployment = engineRule.RepositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/runtime/oneTaskProcess.bpmn20.xml")
//                .AddClasspathResource("resources/api/runtime/otherOneTaskProcess.bpmn20.xml")
//                .Deploy();

//            engineRule.ManageDeployment(deployment);

//            var runtimeService = engineRule.RuntimeService;
//            processInstanceIds = new List<string>();
//            for (var i = 0; i < 4; i++)
//                processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "")
//                    .Id);
//            processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY_2, "businessKey_123")
//                .Id);
//        }

//        public static readonly IBpmnModelInstance FORK_JOIN_SUB_PROCESS_MODEL = ProcessModels.NewModel()
//            .StartEvent()
//            .SubProcess("subProcess")
//            ////.EmbeddedSubProcess()
//            //.StartEvent()
//            .ParallelGateway("fork")
//            .UserTask("userTask1")
//            .Name("completeMe")
//            .ParallelGateway("join")
//            .EndEvent()
//            //.MoveToNode("fork")
//            .UserTask("userTask2")
//            //.ConnectTo("join")
//            .SubProcessDone()
//            .EndEvent()
//            .Done();

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
//        //public RuleChain ruleChain;

//        private static readonly string PROCESS_DEFINITION_KEY = "oneTaskProcess";
//        private static readonly string PROCESS_DEFINITION_KEY_2 = "otherOneTaskProcess";
//        protected internal ICaseService caseService;

//        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//        private readonly bool InstanceFieldsInitialized;
//        protected internal IManagementService managementService;

//        protected internal IList<string> processInstanceIds;
//        protected internal IRepositoryService repositoryService;

//        protected internal IRuntimeService runtimeService;
//        protected internal ProcessEngineTestRule testHelper;

//        public ProcessInstanceQueryTest()
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
//            ////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
//        }


//        [Test]
//        public virtual void testQueryNoSpecificsList()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();
//            Assert.AreEqual(5, query.Count());
//            Assert.AreEqual(5, query
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryNoSpecificsSingleResult()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();
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
//        public virtual void testQueryByProcessDefinitionKeySingleResult()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY_2);
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.NotNull(query.First());
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionKey()
//        {
//            Assert.IsNull(runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey("invalid")
//                .First());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey("invalid")
//                
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionKeyMultipleResults()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY);
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query
//                .Count());

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
//        public virtual void testQueryByProcessInstanceId()
//        {
//            foreach (var ProcessInstanceId in processInstanceIds)
//            {
//                Assert.NotNull(runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId ==processInstanceId)
//                    .First());
//                Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId ==processInstanceId)
//                    
//                    .Count());
//            }
//        }

//        [Test]
//        public virtual void testQueryByBusinessKeyAndProcessDefinitionKey()
//        {
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("0", PROCESS_DEFINITION_KEY)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("1", PROCESS_DEFINITION_KEY)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("2", PROCESS_DEFINITION_KEY)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("3", PROCESS_DEFINITION_KEY)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("businessKey_123", PROCESS_DEFINITION_KEY_2)
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByBusinessKey()
//        {
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("0")
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("1")
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("businessKey_123")
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByBusinessKeyLike()
//        {
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .ProcessInstanceBusinessKeyLike("business%")
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .ProcessInstanceBusinessKeyLike("%sinessKey\\_123")
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .ProcessInstanceBusinessKeyLike("%siness%")
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidBusinessKey()
//        {
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceBusinessKey("invalid")
//                .Count());

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .SetProcessInstanceBusinessKey(null)
//                    .Count();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessInstanceId()
//        {
//            Assert.IsNull(runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceId("I do not exist")
//                .First());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceId("I do not exist")
//                
//                .Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"}) ]
//        public virtual void testQueryBySuperProcessInstanceId()
//        {
//            var superProcessInstance = runtimeService.StartProcessInstanceByKey("subProcessQueryTest");

//            var query = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(superProcessInstance.Id);
//            var subProcessInstance = query.First();
//            Assert.NotNull(subProcessInstance);
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidSuperProcessInstanceId()
//        {
//            Assert.IsNull(runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId("invalid")
//                .First());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId("invalid")
//                
//                .Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"})]
//        public virtual void testQueryBySubProcessInstanceId()
//        {
//            var superProcessInstance = runtimeService.StartProcessInstanceByKey("subProcessQueryTest");

//            var subProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(superProcessInstance.Id)
//                .First();
//            Assert.NotNull(subProcessInstance);
//            Assert.AreEqual(superProcessInstance.Id, runtimeService.CreateProcessInstanceQuery()
//                /*.SetSubProcessInstanceId(subProcessInstance.Id)*/
//                .First()
//                .Id);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSubProcessInstanceId()
//        {
//            Assert.IsNull(runtimeService.CreateProcessInstanceQuery()
//                .SetSubProcessInstanceId("invalid")
//                .First());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .SetSubProcessInstanceId("invalid")
//                
//                .Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "resources/api/runtime/nestedSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"}) ]
//        public virtual void testQueryBySuperProcessInstanceIdNested()
//        {
//            var superProcessInstance = runtimeService.StartProcessInstanceByKey("nestedSubProcessQueryTest");

//            var subProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(superProcessInstance.Id)
//                .First();
//            Assert.NotNull(subProcessInstance);

//            var nestedSubProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(subProcessInstance.Id)
//                .First();
//            Assert.NotNull(nestedSubProcessInstance);
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "resources/api/runtime/nestedSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"}) ]
//        public virtual void testQueryBySubProcessInstanceIdNested()
//        {
//            var superProcessInstance = runtimeService.StartProcessInstanceByKey("nestedSubProcessQueryTest");

//            var subProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(superProcessInstance.Id)
//                .First();
//            Assert.AreEqual(superProcessInstance.Id, runtimeService.CreateProcessInstanceQuery()
//                /*.SetSubProcessInstanceId(subProcessInstance.Id)*/
//                .First()
//                .Id);

//            var nestedSubProcessInstance = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperProcessInstanceId(subProcessInstance.Id)
//                .First();
//            Assert.AreEqual(subProcessInstance.Id, runtimeService.CreateProcessInstanceQuery()
//                .SetSubProcessInstanceId(nestedSubProcessInstance.Id)
//                .First()
//                .Id);
//        }

//        [Test]
//        public virtual void testQueryPaging()
//        {
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                .Count());
//            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                /*.ListPage(0, 2)*/
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                .ListPage(1, 3)
//                .Count());
//        }

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            var processInstances = runtimeService.CreateProcessInstanceQuery()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
//                
//                .ToList();
//            Assert.AreEqual(5, processInstances.Count);
//            ////verifySorting(processInstances, processInstanceByProcessInstanceId());

//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
//                
//                .ToList();
//            Assert.AreEqual(5, processInstances.Count);
//            //verifySorting(processInstances, processInstanceByProcessDefinitionId());

//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                //.OrderByBusinessKey()
//                /*.Asc()*/
//                
//                .ToList();
//            Assert.AreEqual(5, processInstances.Count);
//            //verifySorting(processInstances, processInstanceByBusinessKey());

//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
//                
//                .Count());

//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                //.OrderByBusinessKey()
//                /*.Desc()*/
//                
//                .Count());

//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
//                
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryInvalidSorting()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    /*.OrderByProcessDefinitionId()*/
//                    
//                    .ToList(); // asc - Desc not called -> exception
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryStringVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["stringVar"] = "abcdef";
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["stringVar"] = "abcdef";
//            vars["stringVar2"] = "ghijkl";
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["stringVar"] = "azerty";
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Test EQUAL on single string variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("stringVar", "abcdef");
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Test EQUAL on two string variables, should result in single match
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("stringVar", "abcdef")
//                //.VariableValueEquals("stringVar2", "ghijkl");
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Test NOT_EQUAL, should return only 1 resultInstance
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("stringVar", "abcdef")
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN, should return only matching 'azerty'
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("stringVar", "abcdef")
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("stringVar", "z")
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test GREATER_THAN_OR_EQUAL, should return 3 results
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("stringVar", "abcdef")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("stringVar", "z")
//                .Count());

//            // Test LESS_THAN, should return 2 results
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("stringVar", "abcdeg")
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);
//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("stringVar", "abcdef")
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("stringVar", "z")
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("stringVar", "abcdef")
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);
//            expecedIds = new[] {processInstance1.Id, processInstance2.Id};
//            ids = new List<string> {processInstances[0].Id, processInstances[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("stringVar", "z")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("stringVar", "aa")
//                .Count());

//            // Test LIKE
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLike("stringVar", "azert%")
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLike("stringVar", "%y")
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLike("stringVar", "%zer%")
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLike("stringVar", "a%")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLike("stringVar", "%x%")
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }
//        [Test][Deployment("resources/api/oneTaskProcess.bpmn20.xml") ]
//        public virtual void testQueryLongVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["longVar"] = 12345L;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["longVar"] = 12345L;
//            vars["longVar2"] = 67890L;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["longVar"] = 55555L;
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Query on single long variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("longVar", 12345L);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Query on two long variables, should result in single match
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("longVar", 12345L)
//                //.VariableValueEquals("longVar2", 67890L);
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Query with unexisting variable value
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("longVar", 999L)
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test NOT_EQUALS
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                /*.VariableValueNotEquals("longVar", 12345L)*/
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("longVar", 44444L)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("longVar", 55555L)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("longVar", 1L)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 44444L)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 55555L)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 1L)
//                .Count());

//            // Test LESS_THAN
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("longVar", 55555L)
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("longVar", 12345L)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("longVar", 66666L)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("longVar", 55555L)
//                
//                .ToList();
//            Assert.AreEqual(3, processInstances.Count);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("longVar", 12344L)
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryDoubleVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["doubleVar"] = 12345.6789;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["doubleVar"] = 12345.6789;
//            vars["doubleVar2"] = 9876.54321;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["doubleVar"] = 55555.5555;
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Query on single double variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("doubleVar", 12345.6789);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Query on two double variables, should result in single value
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("doubleVar", 12345.6789)
//                //.VariableValueEquals("doubleVar2", 9876.54321);
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Query with unexisting variable value
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("doubleVar", 9999.99)
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test NOT_EQUALS
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("doubleVar", 12345.6789)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("doubleVar", 44444.4444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("doubleVar", 55555.5555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("doubleVar", 1.234)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 44444.4444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 55555.5555)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 1.234)
//                .Count());

//            // Test LESS_THAN
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("doubleVar", 55555.5555)
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("doubleVar", 12345.6789)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("doubleVar", 66666.6666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("doubleVar", 55555.5555)
//                
//                .ToList();
//            Assert.AreEqual(3, processInstances.Count);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("doubleVar", 12344.6789)
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryIntegerVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["integerVar"] = 12345;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["integerVar"] = 12345;
//            vars["integerVar2"] = 67890;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["integerVar"] = 55555;
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Query on single integer variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("integerVar", 12345);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Query on two integer variables, should result in single value
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("integerVar", 12345)
//                //.VariableValueEquals("integerVar2", 67890);
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Query with unexisting variable value
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("integerVar", 9999)
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test NOT_EQUALS
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("integerVar", 12345)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("integerVar", 44444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("integerVar", 55555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("integerVar", 1)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 44444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 55555)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 1)
//                .Count());

//            // Test LESS_THAN
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("integerVar", 55555)
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("integerVar", 12345)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("integerVar", 66666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("integerVar", 55555)
//                
//                .ToList();
//            Assert.AreEqual(3, processInstances.Count);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("integerVar", 12344)
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryShortVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            short shortVar = 1234;
//            vars["shortVar"] = shortVar;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            short shortVar2 = 6789;
//            vars = new Dictionary<string, object>();
//            vars["shortVar"] = shortVar;
//            vars["shortVar2"] = shortVar2;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["shortVar"] = (short) 5555;
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Query on single short variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("shortVar", shortVar);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Query on two short variables, should result in single value
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("shortVar", shortVar)
//                //.VariableValueEquals("shortVar2", shortVar2);
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Query with unexisting variable value
//            short unexistingValue = 9999;
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("shortVar", unexistingValue)
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test NOT_EQUALS
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("shortVar", (short) 1234)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("shortVar", (short) 4444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("shortVar", (short) 5555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("shortVar", (short) 1)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 4444)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 5555)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 1)
//                .Count());

//            // Test LESS_THAN
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("shortVar", (short) 5555)
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("shortVar", (short) 1234)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("shortVar", (short) 6666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("shortVar", (short) 5555)
//                
//                .ToList();
//            Assert.AreEqual(3, processInstances.Count);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("shortVar", (short) 1233)
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryDateVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            var date1 = new DateTime();
//            vars["dateVar"] = date1;

//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            var date2 = new DateTime();
//            vars = new Dictionary<string, object>();
//            vars["dateVar"] = date1;
//            vars["dateVar2"] = date2;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            var nextYear = new DateTime();
//            nextYear.AddYears(1);
//            vars = new Dictionary<string, object>();
//            vars["dateVar"] = nextYear;
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            var nextMonth = new DateTime();
//            nextMonth.AddMonths(1);

//            var twoYearsLater = new DateTime();
//            twoYearsLater.AddYears(2);

//            var oneYearAgo = new DateTime();
//            oneYearAgo.AddYears(-1);

//            // Query on single short variable, should result in 2 matches
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("dateVar", date1);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(2, processInstances.Count);

//            // Query on two short variables, should result in single value
//            query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("dateVar", date1)
//                //.VariableValueEquals("dateVar2", date2);
//            var resultInstance = query.First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

//            // Query with unexisting variable value
//            var unexistingDate = DateTime.Parse("01/01/1989 12:00:00");
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("dateVar", unexistingDate)
//                .First();
//            Assert.IsNull(resultInstance);

//            // Test NOT_EQUALS
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("dateVar", date1)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            // Test GREATER_THAN
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("dateVar", nextMonth)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("dateVar", nextYear)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("dateVar", oneYearAgo)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", nextMonth)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            resultInstance = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", nextYear)
//                .First();
//            Assert.NotNull(resultInstance);
//            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", oneYearAgo)
//                .Count());

//            // Test LESS_THAN
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("dateVar", nextYear)
//                
//                .ToList();
//            Assert.AreEqual(2, processInstances.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string>(new[] {processInstances[0].Id, processInstances[1].Id});
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("dateVar", date1)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("dateVar", twoYearsLater)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            processInstances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("dateVar", nextYear)
//                
//                .ToList();
//            Assert.AreEqual(3, processInstances.Count);

//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("dateVar", oneYearAgo)
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testBooleanVariable()
//        {
//            // TEST EQUALS
//            var vars = new Dictionary<string, object>();
//            vars["booleanVar"] = true;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["booleanVar"] = false;
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            var instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("booleanVar", true)
//                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance1.Id, instances[0].Id);

//            instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("booleanVar", false)
//                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance2.Id, instances[0].Id);

//            // TEST NOT_EQUALS
//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("booleanVar", true)
//                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance2.Id, instances[0].Id);

//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("booleanVar", false)
//                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance1.Id, instances[0].Id);

//            // Test unsupported operations
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThan("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'greater than' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThanOrEqual("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'greater than or equal' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThan("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("Booleans and null cannot be used in 'less than' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThanOrEqual("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'less than or equal' condition"));
//            }

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryVariablesUpdatedToNullValue()
//        {
//            // Start process instance with different types of variables
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["longVar"] = 928374L;
//            variables["shortVar"] = (short) 123;
//            variables["integerVar"] = 1234;
//            variables["stringVar"] = "coca-cola";
//            variables["dateVar"] = DateTime.Now;
//            variables["booleanVar"] = true;
//            variables["nullVar"] = null;
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("longVar", null)
//                //.VariableValueEquals("shortVar", null)
//                //.VariableValueEquals("integerVar", null)
//                //.VariableValueEquals("stringVar", null)
//                //.VariableValueEquals("booleanVar", null)
//                //.VariableValueEquals("dateVar", null);

//            var notQuery = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("longVar", null)
//                .VariableValueNotEquals("shortVar", null)
//                .VariableValueNotEquals("integerVar", null)
//                .VariableValueNotEquals("stringVar", null)
//                .VariableValueNotEquals("booleanVar", null)
//                .VariableValueNotEquals("dateVar", null);

//            Assert.IsNull(query.First());
//            Assert.NotNull(notQuery.First());

//            // Set all existing variables values to null
//            runtimeService.SetVariable(processInstance.Id, "longVar", null);
//            runtimeService.SetVariable(processInstance.Id, "shortVar", null);
//            runtimeService.SetVariable(processInstance.Id, "integerVar", null);
//            runtimeService.SetVariable(processInstance.Id, "stringVar", null);
//            runtimeService.SetVariable(processInstance.Id, "dateVar", null);
//            runtimeService.SetVariable(processInstance.Id, "nullVar", null);
//            runtimeService.SetVariable(processInstance.Id, "booleanVar", null);

//            IExecution queryResult = query.First();
//            Assert.NotNull(queryResult);
//            Assert.AreEqual(processInstance.Id, queryResult.Id);
//            Assert.IsNull(notQuery.First());
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryNullVariable()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["nullVar"] = null;
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["nullVar"] = "notnull";
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["nullVarLong"] = "notnull";
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["nullVarDouble"] = "notnull";
//            var processInstance4 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            vars = new Dictionary<string, object>();
//            vars["nullVarByte"] = "testbytes".GetBytes();
//            var processInstance5 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            // Query on null value, should return one value
//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("nullVar", null);
//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(1, processInstances.Count);
//            Assert.AreEqual(processInstance1.Id, processInstances[0].Id);

//            // Test NOT_EQUALS null
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("nullVar", null)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("nullVarLong", null)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("nullVarDouble", null)
//                .Count());
//            // When a byte-array refrence is present, the variable is not considered null
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("nullVarByte", null)
//                .Count());

//            // All other variable queries with null should throw exception
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThan("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'greater than' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThanOrEqual("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'greater than or equal' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThan("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("Booleans and null cannot be used in 'less than' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThanOrEqual("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message,
//                    Does.Contain("Booleans and null cannot be used in 'less than or equal' condition"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLike("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("Booleans and null cannot be used in 'like' condition"));
//            }

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance4.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance5.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryInvalidTypes()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["bytesVar"] = "test".GetBytes();
//            vars["serializableVar"] = new DummySerializable();

//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    //.VariableValueEquals("bytesVar", "test".GetBytes())
//                    
//                    .ToList();
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("Variables of type ByteArray cannot be used to query"));
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    //.VariableValueEquals("serializableVar", new DummySerializable())
//                    
//                    .ToList();
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("Object values cannot be used to query"));
//            }

//            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//        }

//        [Test]
//        public virtual void testQueryVariablesNullNameArgument()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    //.VariableValueEquals(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueNotEquals(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThan(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThanOrEqual(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThan(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThanOrEqual(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLike(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                Assert.That(ae.Message, Does.Contain("name is null"));
//            }
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryAllVariableTypes()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["nullVar"] = null;
//            vars["stringVar"] = "string";
//            vars["longVar"] = 10L;
//            vars["doubleVar"] = 1.2;
//            vars["integerVar"] = 1234;
//            vars["booleanVar"] = true;
//            vars["shortVar"] = (short) 123;

//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            var query = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("nullVar", null)
//                //.VariableValueEquals("stringVar", "string")
//                //.VariableValueEquals("longVar", 10L)
//                //.VariableValueEquals("doubleVar", 1.2)
//                //.VariableValueEquals("integerVar", 1234)
//                //.VariableValueEquals("booleanVar", true)
//                //.VariableValueEquals("shortVar", (short) 123);

//            var processInstances = query
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(1, processInstances.Count);
//            Assert.AreEqual(processInstance.Id, processInstances[0].Id);

//            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testClashingValues()
//        {
//            IDictionary<string, object> vars = new Dictionary<string, object>();
//            vars["var"] = 1234L;

//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);

//            IDictionary<string, object> vars2 = new Dictionary<string, object>();
//            vars2["var"] = 1234;

//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars2);

//            var foundInstances = runtimeService.CreateProcessInstanceQuery()
//                //.SetProcessDefinitionKey("oneTaskProcess")
//                //.VariableValueEquals("var", 1234L)
//                
//                .ToList();

//            Assert.AreEqual(1, foundInstances.Count);
//            Assert.AreEqual(processInstance.Id, foundInstances[0].Id);

//            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceIds()
//        {
//            ISet<string> processInstanceIds = new HashSet<string>(this.processInstanceIds);

//            // start an instance that will not be part of the query
//            runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY_2, "2");

//            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
//                .SetProcessInstanceIds(processInstanceIds);
//            Assert.AreEqual(5, processInstanceQuery.Count());

//            var processInstances = processInstanceQuery
//                .ToList();
//            Assert.NotNull(processInstances);
//            Assert.AreEqual(5, processInstances.Count);

//            foreach (var processInstance in processInstances)
//                Assert.True(processInstanceIds.Contains(processInstance.Id));
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceIdsEmpty()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .SetProcessInstanceIds(new HashSet<string>());
//                Assert.Fail("ProcessEngineException expected");
//            }
//            catch (ProcessEngineException re)
//            {
//                Assert.That(re.Message, Does.Contain("Set of process instance ids is empty"));
//            }
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceIdsNull()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .SetProcessInstanceIds(null);
//                Assert.Fail("ProcessEngineException expected");
//            }
//            catch (ProcessEngineException re)
//            {
//                Assert.That(re.Message, Does.Contain("Set of process instance ids is null"));
//            }
//        }

//        [Test]
//        public virtual void testQueryByActive()
//        {
//            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(5, processInstanceQuery.Active()
//                .Count());

//            repositoryService.SuspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY);

//            Assert.AreEqual(5, processInstanceQuery.Active()
//                .Count());

//            repositoryService.SuspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY, true, DateTime.MaxValue);

//            Assert.AreEqual(1, processInstanceQuery.Active()
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryBySuspended()
//        {
//            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(0, processInstanceQuery.Suspended()
//                .Count());

//            repositoryService.SuspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY);

//            Assert.AreEqual(0, processInstanceQuery.Suspended()
//                .Count());

//            repositoryService.SuspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY, true, DateTime.MaxValue);

//            Assert.AreEqual(4, processInstanceQuery.Suspended()
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQuery()
//        {
//            var tablePrefix = engineRule.ProcessEngineConfiguration.DatabaseTablePrefix;
//            // just test that the query will be constructed and executed, details are tested in the TaskQueryTest
//            Assert.AreEqual(tablePrefix + "ACT_RU_EXECUTION", managementService.GetTableName(typeof(IProcessInstance)));

//            var piCount = runtimeService.CreateProcessInstanceQuery()
//                .Count();

//            Assert.AreEqual(piCount, runtimeService.CreateNativeProcessInstanceQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IProcessInstance)))
//                
//                .Count);
//            Assert.AreEqual(piCount, runtimeService.CreateNativeProcessInstanceQuery()
//                .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IProcessInstance)))
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQueryPaging()
//        {
//            Assert.AreEqual(5, runtimeService.CreateNativeProcessInstanceQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IProcessInstance)))
//                .ListPage(0, 5)
//                .Count);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentId()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentId(incident.Id)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentId()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(0, query.IncidentId("invalid")
//                .Count());

//            try
//            {
//                query.IncidentId(null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentType()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentType(incident.IncidentType)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentType()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(0, query.IncidentType("invalid")
//                .Count());

//            try
//            {
//                query.IncidentType(null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessage()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentMessage(incident.IncidentMessage)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentMessage()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(0, query.IncidentMessage("invalid")
//                .Count());

//            try
//            {
//                query.IncidentMessage(null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessageLike()
//        {
//            runtimeService.StartProcessInstanceByKey("failingProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("\\_exception%")
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentMessageLike()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.AreEqual(0, query.Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""invalid")
//                .Count());

//            try
//            {
//                query.Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentIdInSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentId(incident.Id)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//            Assert.AreEqual(processInstance.Id, processInstanceList[0].Id);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentTypeInSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentType(incident.IncidentType)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//            Assert.AreEqual(processInstance.Id, processInstanceList[0].Id);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessageInSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .IncidentMessage(incident.IncidentMessage)
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//            Assert.AreEqual(processInstance.Id, processInstanceList[0].Id);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessageLikeInSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            testHelper.ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
//                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var processInstanceList = runtimeService.CreateProcessInstanceQuery()
//                .Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("exception%")
//                
//                .ToList();

//            Assert.AreEqual(1, processInstanceList.Count);
//            Assert.AreEqual(processInstance.Id, processInstanceList[0].Id);
//        }

//        [Test][Deployment(new string[] { "resources/api/cmmn/oneProcessTaskCase.cmmn", "resources/api/oneTaskProcess.bpmn20.xml" }) ]
//        public virtual void testQueryByCaseInstanceId()
//        {
//            var caseInstanceId = caseService.WithCaseDefinitionByKey("oneProcessTaskCase")
//                .Create()
//                .Id;

//            var query = runtimeService.CreateProcessInstanceQuery();

//            query.SetCaseInstanceId(caseInstanceId);

//            Assert.AreEqual(1, query.Count());

//            var result = query
//                .ToList();
//            Assert.AreEqual(1, result.Count);

//            var processInstance = result[0];
//            Assert.AreEqual(caseInstanceId, processInstance.CaseInstanceId);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseInstanceId()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            query.SetCaseInstanceId("invalid");

//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                query.SetCaseInstanceId(null);
//                Assert.Fail("The passed case instance should not be null.");
//            }
//            catch (Exception)
//            {
//            }
//        }
//        [Test][Deployment(new string[] { "resources/api/runtime/superCase.cmmn", "resources/api/runtime/superProcessWithCallActivityInsideSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" })]
//        public virtual void testQueryByCaseInstanceIdHierarchy()
//        {
//            var caseInstanceId = caseService.WithCaseDefinitionByKey("oneProcessTaskCase")
//                .BusinessKey("aBusinessKey")
//                .Create()
//                .Id;

//            var query = runtimeService.CreateProcessInstanceQuery();

//            query.SetCaseInstanceId(caseInstanceId);

//            Assert.AreEqual(2, query.Count());

//            var result = query
//                .ToList();
//            Assert.AreEqual(2, result.Count);

//            var firstProcessInstance = result[0];
//            Assert.AreEqual(caseInstanceId, firstProcessInstance.CaseInstanceId);

//            var secondProcessInstance = result[1];
//            Assert.AreEqual(caseInstanceId, secondProcessInstance.CaseInstanceId);
//        }

//        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml") ]
//        public virtual void testProcessVariableValueEqualsNumber()
//        {
//            // long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123L));

//            // non-matching long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 12345L));

//            // short
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", (short) 123));

//            // double
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123.0d));

//            // integer
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123));

//            // untyped null (should not match)
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess" /*, Collections.singletonMap("var", null)*/);

//            // typed null (should not match)
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", "123"));

//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());
//        }

//        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml") ]
//        public virtual void testProcessVariableValueNumberComparison()
//        {
//            // long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess"
//                /*,new Dictionary<string, ITypedValue>(){{"var", 123}}*/);

//            // non-matching long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess"
//                //    //new Dictionary<string, ITypedValue>(){{"var", 12345L}}
//            );

//            // short
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", (short) 123}};

//            // double
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123.0d}};

//            // integer
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123};

//            // untyped null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess" /*, Collections.singletonMap("var", null)*/);

//            // typed null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", "123"));

//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThan("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueGreaterThanOrEqual("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThan("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
//                .VariableValueLessThanOrEqual("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneProcessTaskCase.cmmn"})]
//        public virtual void testQueryBySuperCaseInstanceId()
//        {
//            var superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneProcessTaskCase")
//                .Id;

//            var query = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperCaseInstanceId(superCaseInstanceId);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());

//            var subProcessInstance = query.First();
//            Assert.NotNull(subProcessInstance);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSuperCaseInstanceId()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.IsNull(query.SetSuperProcessInstanceId("invalid")
//                .First());
//            Assert.AreEqual(0, query.SetSuperProcessInstanceId("invalid")
//                
//                .Count());

//            try
//            {
//                query.SetSuperCaseInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NullValueException)
//            {
//                // expected
//            }
//        }

//        [Test][Deployment(new string[] { "resources/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "resources/api/cmmn/oneTaskCase.cmmn" }) ]
//        public virtual void testQueryBySubCaseInstanceId()
//        {
//            var superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest")
//                .Id;

//            var subCaseInstanceId = caseService.CreateCaseInstanceQuery()
//                .SuperProcessInstanceId(superProcessInstanceId)
//                .First()
//                .Id;

//            var query = runtimeService.CreateProcessInstanceQuery()
//                .SetSubCaseInstanceId(subCaseInstanceId);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());

//            var superProcessInstance = query.First();
//            Assert.NotNull(superProcessInstance);
//            Assert.AreEqual(superProcessInstanceId, superProcessInstance.Id);
//        }

//        [Test][Deployment(new string[] { "resources/api/runtime/superProcessWithCaseCallActivityInsideSubProcess.bpmn20.xml", "resources/api/cmmn/oneTaskCase.cmmn" }) ]
//        public virtual void testQueryBySubCaseInstanceIdNested()
//        {
//            var superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest")
//                .Id;

//            var subCaseInstanceId = caseService.CreateCaseInstanceQuery()
//                .SuperProcessInstanceId(superProcessInstanceId)
//                .First()
//                .Id;

//            var query = runtimeService.CreateProcessInstanceQuery()
//                .SetSubCaseInstanceId(subCaseInstanceId);

//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());

//            var superProcessInstance = query.First();
//            Assert.NotNull(superProcessInstance);
//            Assert.AreEqual(superProcessInstanceId, superProcessInstance.Id);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSubCaseInstanceId()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery();

//            Assert.IsNull(query.SetSubProcessInstanceId("invalid")
//                .First());
//            Assert.AreEqual(0, query.SetSubProcessInstanceId("invalid")
//                
//                .Count());

//            try
//            {
//                query.SetSubCaseInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NullValueException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryNullValue()
//        {
//            // typed null
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess",
//                Variable.Variables.CreateVariables()
//                    .PutValueTyped("var", Variable.Variables.StringValue(null)));

//            // untyped null
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess",
//                Variable.Variables.CreateVariables()
//                    .PutValueTyped("var", null));

//            // non-null String value
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess",
//                Variable.Variables.CreateVariables()
//                    .PutValue("var", "a String Value"));

//            var processInstance4 = runtimeService.StartProcessInstanceByKey("oneTaskProcess",
//                Variable.Variables.CreateVariables()
//                    .PutValue("var", "another String Value"));

//            // (1) query for untyped null: should return typed and untyped null (notEquals: the opposite)
//            var instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", null)
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance1.Id, processInstance2.Id));
//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("var", null)
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance3.Id, processInstance4.Id));

//            // (2) query for typed null: should return typed null only (notEquals: the opposite)
//            instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", Variable.Variables.StringValue(null))
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance1.Id));
//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("var", Variable.Variables.StringValue(null))
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance2.Id, processInstance3.Id, processInstance4.Id));

//            // (3) query for typed value: should return typed value only (notEquals: the opposite)
//            instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("var", "a String Value")
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance3.Id));
//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("var", "a String Value")
//                
//                .ToList();
//            verifyResultContainsExactly(instances, asSet(processInstance1.Id, processInstance2.Id, processInstance4.Id));
//        }

//        [Test]
//        public virtual void testQueryByDeploymentId()
//        {
//            // given
//            var firstDeploymentId = repositoryService.CreateDeploymentQuery()
//                .First()
//                .Id;

//            // make a second deployment and start an instance
//            var secondDeployment = repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/runtime/oneTaskProcess.bpmn20.xml")
//                .Deploy();

//            var secondProcessInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // when
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .SetDeploymentId(firstDeploymentId);

//            // then the instance belonging to the second deployment is not returned
//            Assert.AreEqual(5, query.Count());

//            var instances = query
//                .ToList();
//            Assert.AreEqual(5, instances.Count);

//            foreach (var returnedInstance in instances)
//                Assert.True(!returnedInstance.Id.Equals(secondProcessInstance.Id));

//            // cleanup
//            repositoryService.DeleteDeployment(secondDeployment.Id, true);
//        }

//        [Test]
//        public virtual void testQueryByInvalidDeploymentId()
//        {
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
//                .SetDeploymentId("invalid")
//                .Count());

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .SetDeploymentId(null)
//                    .Count();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullActivityId()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .ActivityIdIn((string) null);
//                Assert.Fail("exception expected");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("activity ids contains null value"));
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullActivityIds()
//        {
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .ActivityIdIn(null);
//                Assert.Fail("exception expected");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("activity ids is null"));
//            }
//        }

//        [Test]
//        public virtual void testQueryByUnknownActivityId()
//        {
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("unknown");

//            AssertNoProcessInstancesReturned(query);
//        }

//        [Test]
//        public virtual void testQueryByLeafActivityId()
//        {
//            // given
//            var oneTaskDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
//            var gatewaySubProcessDefinition = testHelper.DeployAndGetDefinition(FORK_JOIN_SUB_PROCESS_MODEL);

//            // when
//            var oneTaskInstance1 = runtimeService.StartProcessInstanceById(oneTaskDefinition.Id);
//            var oneTaskInstance2 = runtimeService.StartProcessInstanceById(oneTaskDefinition.Id);
//            var gatewaySubProcessInstance1 = runtimeService.StartProcessInstanceById(gatewaySubProcessDefinition.Id);
//            var gatewaySubProcessInstance2 = runtimeService.StartProcessInstanceById(gatewaySubProcessDefinition.Id);

//            var task = engineRule.TaskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==gatewaySubProcessInstance2.Id)
//                .TaskName("completeMe")
//                .First();
//            engineRule.TaskService.Complete(task.Id);

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("userTask");
//            AssertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("userTask1", "userTask2");
//            AssertReturnedProcessInstances(query, gatewaySubProcessInstance1, gatewaySubProcessInstance2);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("userTask", "userTask1");
//            AssertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2, gatewaySubProcessInstance1);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("userTask", "userTask1", "userTask2");
//            AssertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2, gatewaySubProcessInstance1,
//                gatewaySubProcessInstance2);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("join");
//            AssertReturnedProcessInstances(query, gatewaySubProcessInstance2);
//        }

//        [Test]
//        public virtual void testQueryByNonLeafActivityId()
//        {
//            // given
//            var processDefinition = testHelper.DeployAndGetDefinition(FORK_JOIN_SUB_PROCESS_MODEL);

//            // when
//            runtimeService.StartProcessInstanceById(processDefinition.Id);

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("subProcess", "fork");
//            AssertNoProcessInstancesReturned(query);
//        }

//        [Test]
//        public virtual void testQueryByAsyncBeforeActivityId()
//        {
//            // given
//            var testProcess = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
//                .StartEvent("start")
//                .CamundaAsyncBefore()
//                .SubProcess("subProcess")
//                .CamundaAsyncBefore()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask("task")
//                .CamundaAsyncBefore()
//                .CamundaExpression("${true}")
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent("end")
//                .CamundaAsyncBefore()
//                .Done());

//            // when
//            var instanceBeforeStart = runtimeService.StartProcessInstanceById(testProcess.Id);
//            var instanceBeforeSubProcess = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceBeforeSubProcess);
//            var instanceBeforeTask = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceBeforeTask);
//            executeJobForProcessInstance(instanceBeforeTask);
//            var instanceBeforeEnd = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceBeforeEnd);
//            executeJobForProcessInstance(instanceBeforeEnd);
//            executeJobForProcessInstance(instanceBeforeEnd);

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("start");
//            AssertReturnedProcessInstances(query, instanceBeforeStart);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("subProcess");
//            AssertReturnedProcessInstances(query, instanceBeforeSubProcess);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("task");
//            AssertReturnedProcessInstances(query, instanceBeforeTask);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("end");
//            AssertReturnedProcessInstances(query, instanceBeforeEnd);
//        }

//        [Test]
//        public virtual void testQueryByAsyncAfterActivityId()
//        {
//            // given
//            var testProcess = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
//                .StartEvent("start")
//                .CamundaAsyncAfter()
//                .SubProcess("subProcess")
//                .CamundaAsyncAfter()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask("task")
//                .CamundaAsyncAfter()
//                .CamundaExpression("${true}")
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent("end")
//                .CamundaAsyncAfter()
//                .Done());

//            // when
//            var instanceAfterStart = runtimeService.StartProcessInstanceById(testProcess.Id);
//            var instanceAfterTask = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceAfterTask);
//            var instanceAfterSubProcess = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceAfterSubProcess);
//            executeJobForProcessInstance(instanceAfterSubProcess);
//            var instanceAfterEnd = runtimeService.StartProcessInstanceById(testProcess.Id);
//            executeJobForProcessInstance(instanceAfterEnd);
//            executeJobForProcessInstance(instanceAfterEnd);
//            executeJobForProcessInstance(instanceAfterEnd);

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("start");
//            AssertReturnedProcessInstances(query, instanceAfterStart);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("task");
//            AssertReturnedProcessInstances(query, instanceAfterTask);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("subProcess");
//            AssertReturnedProcessInstances(query, instanceAfterSubProcess);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("end");
//            AssertReturnedProcessInstances(query, instanceAfterEnd);
//        }

//        [Test]
//        public virtual void testQueryByActivityIdBeforeCompensation()
//        {
//            // given
//            var testProcess =
//                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

//            // when
//            runtimeService.StartProcessInstanceById(testProcess.Id);
//            testHelper.CompleteTask("userTask1");

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("subProcess");
//            AssertNoProcessInstancesReturned(query);
//        }

//        [Test]
//        public virtual void testQueryByActivityIdDuringCompensation()
//        {
//            // given
//            var testProcess =
//                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

//            // when
//            var processInstance = runtimeService.StartProcessInstanceById(testProcess.Id);
//            testHelper.CompleteTask("userTask1");
//            testHelper.CompleteTask("userTask2");

//            // then
//            var query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("subProcess");
//            AssertReturnedProcessInstances(query, processInstance);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("compensationEvent");
//            AssertReturnedProcessInstances(query, processInstance);

//            query = runtimeService.CreateProcessInstanceQuery()
//                .ActivityIdIn("compensationHandler");
//            AssertReturnedProcessInstances(query, processInstance);
//        }

//        protected internal virtual void executeJobForProcessInstance(IProcessInstance processInstance)
//        {
//            var job = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();
//            managementService.ExecuteJob(job.Id);
//        }

//        protected internal virtual ISet<T> asSet<T>(params T[] elements)
//        {
//            return new HashSet<T>(elements);
//        }

//        protected internal virtual void AssertNoProcessInstancesReturned( IQueryable<IProcessInstance> query)
//        {
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query
//                .Count());
//        }

//        protected internal virtual void AssertReturnedProcessInstances( IQueryable<IProcessInstance> query,
//            params IProcessInstance[] processInstances)
//        {
//            var expectedSize = processInstances.Length;
//            Assert.AreEqual(expectedSize, query.Count());
//            Assert.AreEqual(expectedSize, query
//                .Count());

//            verifyResultContainsExactly(query
//                .ToList();, collectProcessInstanceIds(processInstances));
//        }

//        protected internal virtual void verifyResultContainsExactly(IList<IProcessInstance> instances,
//            ISet<string> processInstanceIds)
//        {
//            var retrievedInstanceIds = collectProcessInstanceIds(instances);
//            Assert.AreEqual(processInstanceIds, retrievedInstanceIds);
//        }

//        protected internal virtual ISet<string> collectProcessInstanceIds(IList<IProcessInstance> instances)
//        {
//            ISet<string> retrievedInstanceIds = new HashSet<string>();
//            foreach (var instance in instances)
//                retrievedInstanceIds.Add(instance.Id);
//            return retrievedInstanceIds;
//        }
//    }
//}