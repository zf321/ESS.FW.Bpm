//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Runtime
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class ExecutionQueryTest : PluggableProcessEngineTestCase
//    {
//        private static readonly string CONCURRENT_PROCESS_KEY = "concurrent";
//        private static readonly string SEQUENTIAL_PROCESS_KEY = "oneTaskProcess";

//        private IList<string> concurrentProcessInstanceIds;
//        private IList<string> sequentialProcessInstanceIds;

//        [SetUp]
//        protected internal virtual void setUp()
//        {
//            //base.SetUp();
//            repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/runtime/oneTaskProcess.bpmn20.xml")
//                .AddClasspathResource("resources/api/runtime/concurrentExecution.bpmn20.xml")
//                .Deploy();

//            concurrentProcessInstanceIds = new List<string>();
//            sequentialProcessInstanceIds = new List<string>();

//            for (var i = 0; i < 4; i++)
//                concurrentProcessInstanceIds.Add(
//                    runtimeService.StartProcessInstanceByKey(CONCURRENT_PROCESS_KEY, "BUSINESS-KEY-" + i)
//                        .Id);
//            sequentialProcessInstanceIds.Add(runtimeService.StartProcessInstanceByKey(SEQUENTIAL_PROCESS_KEY)
//                .Id);
//        }
//        [TearDown]
//        protected internal virtual void tearDown()
//        {
//            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
//                .ToList())
//                repositoryService.DeleteDeployment(deployment.Id, true);
//            TearDown();
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionKey()
//        {
//            // Concurrent process with 3 executions for each process instance
//            Assert.AreEqual(12, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
                
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(SEQUENTIAL_PROCESS_KEY)
                
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionKey()
//        {
//            var query = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            foreach (var ProcessInstanceId in concurrentProcessInstanceIds)
//            {
//                var query = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==ProcessInstanceId);
//                Assert.AreEqual(3, query
//                    .Count());
//                Assert.AreEqual(3, query.Count());
//            }
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == sequentialProcessInstanceIds[0])
                
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessInstanceId()
//        {
//            var query = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        public virtual void testQueryExecutionId()
//        {
//            var execution = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(SEQUENTIAL_PROCESS_KEY)
//                .First();
//            Assert.NotNull(runtimeService.CreateExecutionQuery()
//                .ExecutionId(execution.Id));
//        }

//        [Test]
//        public virtual void testQueryByInvalidExecutionId()
//        {
//            var query = runtimeService.CreateExecutionQuery()
//                .ExecutionId("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByActivityId()
//        {
//            var query = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "receivePayment");
//            Assert.AreEqual(4, query
//                .Count());
//            Assert.AreEqual(4, query.Count());

//            try
//            {
//                Assert.IsNull(query.First());
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        public virtual void testQueryPaging()
//        {
//            Assert.AreEqual(13, runtimeService.CreateExecutionQuery()
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ListPage(0, 4)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ListPage(2, 1)
//                .Count());
//            Assert.AreEqual(10, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ListPage(1, 10)
//                .Count());
//            Assert.AreEqual(12, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ListPage(0, 20)
//                .Count());
//        }

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            // 13 executions: 3 for each concurrent, 1 for the sequential
//            var executions = runtimeService.CreateExecutionQuery()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, executionByProcessInstanceId());

//            executions = runtimeService.CreateExecutionQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, executionByProcessDefinitionId());

//            executions = runtimeService.CreateExecutionQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, executionByProcessDefinitionKey(processEngine));

//            executions = runtimeService.CreateExecutionQuery()
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, inverted(executionByProcessInstanceId()));

//            executions = runtimeService.CreateExecutionQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, inverted(executionByProcessDefinitionId()));

//            executions = runtimeService.CreateExecutionQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Desc()*/
                
//                .ToList();
//            Assert.AreEqual(13, executions.Count);
//            //verifySorting(executions, inverted(executionByProcessDefinitionKey(processEngine)));

//            executions = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
                
//                .ToList();
//            Assert.AreEqual(12, executions.Count);
//            //verifySorting(executions, executionByProcessDefinitionId());

//            executions = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
                
//                .ToList();
//            Assert.AreEqual(12, executions.Count);
//            //verifySorting(executions, executionByProcessDefinitionId());

//            executions = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
                
//                .ToList();
//            Assert.AreEqual(12, executions.Count);
//            //verifySorting(executions,
//            //    hierarchical(executionByProcessDefinitionKey(processEngine), inverted(executionByProcessInstanceId())));
//        }

//        [Test]
//        public virtual void testQueryInvalidSorting()
//        {
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    //.OrderByProcessDefinitionKey()
                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByBusinessKey()
//        {
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ProcessInstanceBusinessKey("BUSINESS-KEY-1")
                
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ProcessInstanceBusinessKey("BUSINESS-KEY-2")
                
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey(CONCURRENT_PROCESS_KEY)
//                .ProcessInstanceBusinessKey("NON-EXISTING")
                
//                .Count());
//        }

//        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml") ]
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("stringVar", "abcdef");
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Test EQUAL on two string variables, should result in single match
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("stringVar", "abcdef")
//                //.VariableValueEquals("stringVar2", "ghijkl");
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Test NOT_EQUAL, should return only 1 execution
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("stringVar", "abcdef")
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN, should return only matching 'azerty'
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("stringVar", "abcdef")
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("stringVar", "z")
//                .First();
//            Assert.IsNull(execution);

//            // Test GREATER_THAN_OR_EQUAL, should return 3 results
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("stringVar", "abcdef")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("stringVar", "z")
//                .Count());

//            // Test LESS_THAN, should return 2 results
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("stringVar", "abcdeg")
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);
//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("stringVar", "abcdef")
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("stringVar", "z")
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("stringVar", "abcdef")
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);
//            expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("stringVar", "z")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("stringVar", "aa")
//                .Count());

//            // Test LIKE
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueLike("stringVar", "azert%")
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueLike("stringVar", "%y")
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueLike("stringVar", "%zer%")
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLike("stringVar", "a%")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLike("stringVar", "%x%")
//                .Count());

//            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance3.Id, "test");
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("longVar", 12345L);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Query on two long variables, should result in single match
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("longVar", 12345L)
//                //.VariableValueEquals("longVar2", 67890L);
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Query with unexisting variable value
//            execution = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("longVar", 999L)
//                .First();
//            Assert.IsNull(execution);

//            // Test NOT_EQUALS
//            execution = runtimeService.CreateExecutionQuery()
//                /*.VariableValueNotEquals("longVar", 12345L)*/
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("longVar", 44444L)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("longVar", 55555L)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("longVar", 1L)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 44444L)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 55555L)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("longVar", 1L)
//                .Count());

//            // Test LESS_THAN
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("longVar", 55555L)
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("longVar", 12345L)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("longVar", 66666L)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("longVar", 55555L)
                
//                .ToList();
//            Assert.AreEqual(3, executions.Count);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("doubleVar", 12345.6789);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Query on two double variables, should result in single value
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("doubleVar", 12345.6789)
//                //.VariableValueEquals("doubleVar2", 9876.54321);
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Query with unexisting variable value
//            execution = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("doubleVar", 9999.99)
//                .First();
//            Assert.IsNull(execution);

//            // Test NOT_EQUALS
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("doubleVar", 12345.6789)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("doubleVar", 44444.4444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("doubleVar", 55555.5555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("doubleVar", 1.234)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 44444.4444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 55555.5555)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("doubleVar", 1.234)
//                .Count());

//            // Test LESS_THAN
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("doubleVar", 55555.5555)
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("doubleVar", 12345.6789)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("doubleVar", 66666.6666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("doubleVar", 55555.5555)
                
//                .ToList();
//            Assert.AreEqual(3, executions.Count);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("integerVar", 12345);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Query on two integer variables, should result in single value
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("integerVar", 12345)
//                //.VariableValueEquals("integerVar2", 67890);
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Query with unexisting variable value
//            execution = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("integerVar", 9999)
//                .First();
//            Assert.IsNull(execution);

//            // Test NOT_EQUALS
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("integerVar", 12345)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("integerVar", 44444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("integerVar", 55555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("integerVar", 1)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 44444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 55555)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("integerVar", 1)
//                .Count());

//            // Test LESS_THAN
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("integerVar", 55555)
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("integerVar", 12345)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("integerVar", 66666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("integerVar", 55555)
                
//                .ToList();
//            Assert.AreEqual(3, executions.Count);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("shortVar", shortVar);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Query on two short variables, should result in single value
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("shortVar", shortVar)
//                //.VariableValueEquals("shortVar2", shortVar2);
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Query with unexisting variable value
//            short unexistingValue = 9999;
//            execution = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("shortVar", unexistingValue)
//                .First();
//            Assert.IsNull(execution);

//            // Test NOT_EQUALS
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("shortVar", (short) 1234)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("shortVar", (short) 4444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("shortVar", (short) 5555)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("shortVar", (short) 1)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 4444)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 5555)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("shortVar", (short) 1)
//                .Count());

//            // Test LESS_THAN
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("shortVar", (short) 5555)
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("shortVar", (short) 1234)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("shortVar", (short) 6666)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("shortVar", (short) 5555)
                
//                .ToList();
//            Assert.AreEqual(3, executions.Count);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("dateVar", date1);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(2, executions.Count);

//            // Query on two short variables, should result in single value
//            query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("dateVar", date1)
//                //.VariableValueEquals("dateVar2", date2);
//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance2.Id, execution.Id);

//            // Query with unexisting variable value
//            var unexistingDate = DateTime.Parse("01/01/1989 12:00:00");
//            execution = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("dateVar", unexistingDate)
//                .First();
//            Assert.IsNull(execution);

//            // Test NOT_EQUALS
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("dateVar", date1)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            // Test GREATER_THAN
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("dateVar", nextMonth)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("dateVar", nextYear)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThan("dateVar", oneYearAgo)
//                .Count());

//            // Test GREATER_THAN_OR_EQUAL
//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", nextMonth)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            execution = runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", nextYear)
//                .First();
//            Assert.NotNull(execution);
//            Assert.AreEqual(processInstance3.Id, execution.Id);

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueGreaterThanOrEqual("dateVar", oneYearAgo)
//                .Count());

//            // Test LESS_THAN
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("dateVar", nextYear)
                
//                .ToList();
//            Assert.AreEqual(2, executions.Count);

//            IList<string> expecedIds = new List<string> {processInstance1.Id, processInstance2.Id};
//            IList<string> ids = new List<string> {executions[0].Id, executions[1].Id};
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//            //ids.RemoveAll(expecedIds);
//            Assert.True(ids.Count == 0);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("dateVar", date1)
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery()
//                .VariableValueLessThan("dateVar", twoYearsLater)
//                .Count());

//            // Test LESS_THAN_OR_EQUAL
//            executions = runtimeService.CreateExecutionQuery()
//                .VariableValueLessThanOrEqual("dateVar", nextYear)
                
//                .ToList();
//            Assert.AreEqual(3, executions.Count);

//            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
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
                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance1.Id, instances[0].Id);

//            instances = runtimeService.CreateProcessInstanceQuery()
//                //.VariableValueEquals("booleanVar", false)
                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance2.Id, instances[0].Id);

//            // TEST NOT_EQUALS
//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("booleanVar", true)
                
//                .ToList();

//            Assert.NotNull(instances);
//            Assert.AreEqual(1, instances.Count);
//            Assert.AreEqual(processInstance2.Id, instances[0].Id);

//            instances = runtimeService.CreateProcessInstanceQuery()
//                .VariableValueNotEquals("booleanVar", false)
                
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
//                AssertTextPresent("Booleans and null cannot be used in 'greater than' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueGreaterThanOrEqual("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'greater than or equal' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThan("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'less than' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateProcessInstanceQuery()
//                    .VariableValueLessThanOrEqual("booleanVar", true);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'less than or equal' condition", ae.Message);
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
//            variables["booleanVar"] = true;
//            variables["dateVar"] = DateTime.Now;
//            variables["nullVar"] = null;
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("longVar", null)
//                //.VariableValueEquals("shortVar", null)
//                //.VariableValueEquals("integerVar", null)
//                //.VariableValueEquals("stringVar", null)
//                //.VariableValueEquals("booleanVar", null)
//                //.VariableValueEquals("dateVar", null);

//            var notQuery = runtimeService.CreateExecutionQuery()
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
//            runtimeService.SetVariable(processInstance.Id, "booleanVar", null);
//            runtimeService.SetVariable(processInstance.Id, "dateVar", null);
//            runtimeService.SetVariable(processInstance.Id, "nullVar", null);

//            var queryResult = query.First();
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
//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("nullVar", null);
//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(1, executions.Count);
//            Assert.AreEqual(processInstance1.Id, executions[0].Id);

//            // Test NOT_EQUALS null
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("nullVar", null)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("nullVarLong", null)
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("nullVarDouble", null)
//                .Count());
//            // When a byte-array refrence is present, the variable is not considered null
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                .VariableValueNotEquals("nullVarByte", null)
//                .Count());

//            // All other variable queries with null should throw exception
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueGreaterThan("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'greater than' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueGreaterThanOrEqual("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'greater than or equal' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLessThan("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'less than' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLessThanOrEqual("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'less than or equal' condition", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLike("nullVar", null);
//                Assert.Fail("Excetion expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Booleans and null cannot be used in 'like' condition", ae.Message);
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
//                runtimeService.CreateExecutionQuery()
//                    //.VariableValueEquals("bytesVar", "test".GetBytes())
                    
//                    .ToList();
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Variables of type ByteArray cannot be used to query", ae.Message);
//            }

//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    //.VariableValueEquals("serializableVar", new DummySerializable())
                    
//                    .ToList();
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Object values cannot be used to query", ae.Message);
//            }

//            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//        }

//        [Test]
//        public virtual void testQueryVariablesNullNameArgument()
//        {
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    //.VariableValueEquals(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueNotEquals(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueGreaterThan(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueGreaterThanOrEqual(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLessThan(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLessThanOrEqual(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateExecutionQuery()
//                    .VariableValueLike(null, "value");
//                Assert.Fail("Expected exception");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("name is null", ae.Message);
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

//            var query = runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("nullVar", null)
//                //.VariableValueEquals("stringVar", "string")
//                //.VariableValueEquals("longVar", 10L)
//                //.VariableValueEquals("doubleVar", 1.2)
//                //.VariableValueEquals("integerVar", 1234)
//                //.VariableValueEquals("booleanVar", true)
//                //.VariableValueEquals("shortVar", (short) 123);

//            var executions = query
//                .ToList();
//            Assert.NotNull(executions);
//            Assert.AreEqual(1, executions.Count);
//            Assert.AreEqual(processInstance.Id, executions[0].Id);

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

//            var executions = runtimeService.CreateExecutionQuery()
//                .ProcessDefinitionKey("oneTaskProcess")
//                //.VariableValueEquals("var", 1234L)
                
//                .ToList();

//            Assert.AreEqual(1, executions.Count);
//            Assert.AreEqual(processInstance.Id, executions[0].ProcessInstanceId);

//            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryBySignalSubscriptionName()
//        {
//            runtimeService.StartProcessInstanceByKey("catchSignal");

//            // it finds subscribed instances
//            var execution = runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("alert")
//                .First();
//            Assert.NotNull(execution);

//            // test query for nonexisting subscription
//            execution = runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("nonExisitng")
//                .First();
//            Assert.IsNull(execution);

//            // it finds more than one
//            runtimeService.StartProcessInstanceByKey("catchSignal");
//            Assert.AreEqual(2, runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("alert")
//                .Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryBySignalSubscriptionNameBoundary()
//        {
//            runtimeService.StartProcessInstanceByKey("signalProces");

//            // it finds subscribed instances
//            var execution = runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("Test signal")
//                .First();
//            Assert.NotNull(execution);

//            // test query for nonexisting subscription
//            execution = runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("nonExisitng")
//                .First();
//            Assert.IsNull(execution);

//            // it finds more than one
//            runtimeService.StartProcessInstanceByKey("signalProces");
//            Assert.AreEqual(2, runtimeService.CreateExecutionQuery()
//                .SignalEventSubscription("Test signal")
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQuery()
//        {
//            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//            // just test that the query will be constructed and executed, details are tested in the TaskQueryTest
//            Assert.AreEqual(tablePrefix + "ACT_RU_EXECUTION", managementService.GetTableName(typeof(IExecution)));

//            var executionCount = runtimeService.CreateExecutionQuery()
//                .Count();

//            Assert.AreEqual(executionCount, runtimeService.CreateNativeExecutionQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IExecution)))
                
//                .Count);
//            Assert.AreEqual(executionCount, runtimeService.CreateNativeExecutionQuery()
//                .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IExecution)))
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQueryPaging()
//        {
//            Assert.AreEqual(5, runtimeService.CreateNativeExecutionQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IExecution)))
//                .ListPage(1, 5)
//                .Count);
//            Assert.AreEqual(1, runtimeService.CreateNativeExecutionQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IExecution)))
//                .ListPage(2, 1)
//                .Count);
//        }
//        [Test][Deployment("resources/api/runtime/concurrentExecution.bpmn20.xml") ]
//        public virtual void testExecutionQueryWithProcessVariable()
//        {
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["x"] = "parent";
//            var pi = runtimeService.StartProcessInstanceByKey("concurrent", variables);

//            var concurrentExecutions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
                
//                .ToList();
//            Assert.AreEqual(3, concurrentExecutions.Count);
//            foreach (var execution in concurrentExecutions)
//                if (!((ExecutionEntity) execution).IsProcessInstanceExecution)
//                    runtimeService.SetVariableLocal(execution.Id, "x", "child");

//            Assert.AreEqual(2, runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
//                //.VariableValueEquals("x", "child")
//                .Count());
//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
//                //.VariableValueEquals("x", "parent")
//                .Count());

//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
//                //.ProcessVariableValueEquals("x", "parent")
//                .Count());
//            Assert.AreEqual(3, runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
//                .ProcessVariableValueNotEquals("x", "xxx")
//                .Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/concurrentExecution.bpmn20.xml")]
//        public virtual void testExecutionQueryForSuspendedExecutions()
//        {
//            var suspendedExecutions = runtimeService.CreateExecutionQuery()
//                .Suspended()
                
//                .ToList();
//            Assert.AreEqual(suspendedExecutions.Count, 0);

//            foreach (var instanceId in concurrentProcessInstanceIds)
//                runtimeService.SuspendProcessInstanceById(instanceId);

//            suspendedExecutions = runtimeService.CreateExecutionQuery()
//                .Suspended()
                
//                .ToList();
//            Assert.AreEqual(12, suspendedExecutions.Count);

//            var activeExecutions = runtimeService.CreateExecutionQuery()
//                .Active()
                
//                .ToList();
//            Assert.AreEqual(1, activeExecutions.Count);

//            foreach (var activeExecution in activeExecutions)
//                Assert.AreEqual(activeExecution.ProcessInstanceId, sequentialProcessInstanceIds[0]);
//        }

//        [Test][Deployment("resources/api/runtime/failingProcessCreateOneIncident.bpmn20.xml") ]
//        public virtual void testQueryByIncidentId()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentId(incident.Id)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//        }

//        public virtual void testQueryByInvalidIncidentId()
//        {
//            var query = repositoryService.CreateProcessDefinitionQuery();

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

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentType(incident.IncidentType)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//        }

//        public virtual void testQueryByInvalidIncidentType()
//        {
//            var query = repositoryService.CreateProcessDefinitionQuery();

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

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentMessage(incident.IncidentMessage)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//        }

//        public virtual void testQueryByInvalidIncidentMessage()
//        {
//            var query = repositoryService.CreateProcessDefinitionQuery();

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

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var executionList = runtimeService.CreateExecutionQuery(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("\\_exception%")))
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//        }

//        [Test]
//        public virtual void testQueryByInvalidIncidentMessageLike()
//        {
//            var query = repositoryService.CreateProcessDefinitionQuery();

//            Assert.AreEqual(0, query.IncidentMessageLike("invalid")
//                .Count());

//            try
//            {
//                query.IncidentMessageLike(null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test][Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml") ]
//        public virtual void testQueryByIncidentIdSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentId(incident.Id)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//            // execution id of subprocess != process instance id
//            Assert.AreNotSame(processInstance.Id, executionList[0].Id);
//        }

//        [Test][Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentTypeInSubprocess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentType(incident.IncidentType)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//            // execution id of subprocess != process instance id
//            Assert.AreNotSame(processInstance.Id, executionList[0].Id);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessageInSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .IncidentMessage(incident.IncidentMessage)
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//            // execution id of subprocess != process instance id
//            Assert.AreNotSame(processInstance.Id, executionList[0].Id);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml")]
//        public virtual void testQueryByIncidentMessageLikeSubProcess()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("failingSubProcess");

//            ExecuteAvailableJobs();

//            var incidentList = runtimeService.CreateIncidentQuery()
                
//                .ToList();
//            Assert.AreEqual(1, incidentList.Count);

//            runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();

//            var executionList = runtimeService.CreateExecutionQuery()
//                .Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("exception%")
                
//                .ToList();

//            Assert.AreEqual(1, executionList.Count);
//            // execution id of subprocess != process instance id
//            Assert.AreNotSame(processInstance.Id, executionList[0].Id);
//        }

//        [Test][Deployment(new string[]{"resources/api/runtime/oneTaskProcess.bpmn20.xml", "resources/api/runtime/oneMessageCatchProcess.bpmn20.xml"})]
//        public virtual void testQueryForExecutionsWithMessageEventSubscriptions()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var instance1 = runtimeService.StartProcessInstanceByKey("oneMessageCatchProcess");
//            var instance2 = runtimeService.StartProcessInstanceByKey("oneMessageCatchProcess");

//            var executions = runtimeService.CreateExecutionQuery()
//                .MessageEventSubscription()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
                
//                .ToList();

//            Assert.AreEqual(2, executions.Count);
//            if (instance1.Id.CompareTo(instance2.Id) < 0)
//            {
//                Assert.AreEqual(instance1.Id, executions[0].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, executions[1].ProcessInstanceId);
//            }
//            else
//            {
//                Assert.AreEqual(instance2.Id, executions[0].ProcessInstanceId);
//                Assert.AreEqual(instance1.Id, executions[1].ProcessInstanceId);
//            }
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneMessageCatchProcess.bpmn20.xml")]
//        public virtual void testQueryForExecutionsWithMessageEventSubscriptionsOverlappingFilters()
//        {
//            var instance = runtimeService.StartProcessInstanceByKey("oneMessageCatchProcess");

//            var execution = runtimeService.CreateExecutionQuery()
//                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
//                .MessageEventSubscription()
//                .First();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);

//            runtimeService.CreateExecutionQuery()
//                .MessageEventSubscription()
//                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                
//                .ToList();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/twoBoundaryEventSubscriptions.bpmn20.xml")]
//        public virtual void testQueryForExecutionsWithMultipleSubscriptions()
//        {
//            // given two message event subscriptions
//            var instance = runtimeService.StartProcessInstanceByKey("process");

//            var subscriptions = runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId== instance.Id)
                
//                .ToList();
//            Assert.AreEqual(2, subscriptions.Count);
//            Assert.AreEqual(subscriptions[0].ExecutionId, subscriptions[1].ExecutionId);

//            // should return the execution once (not twice)
//            var execution = runtimeService.CreateExecutionQuery()
//                .MessageEventSubscription()
//                .First();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);

//            // should return the execution once
//            execution = runtimeService.CreateExecutionQuery()
//                /*.MessageEventSubscriptionName("messageName_1")*/
//                .First();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);

//            // should return the execution once
//            execution = runtimeService.CreateExecutionQuery()
//                /*.MessageEventSubscriptionName("messageName_2")*/
//                .First();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);

//            // should return the execution once
//            execution = runtimeService.CreateExecutionQuery()
//                /*.MessageEventSubscriptionName("messageName_1")*/
//                /*.MessageEventSubscriptionName("messageName_2")*/
//                .First();

//            Assert.NotNull(execution);
//            Assert.AreEqual(instance.Id, execution.ProcessInstanceId);

//            // should not return the execution
//            execution = runtimeService.CreateExecutionQuery()
//                /*.MessageEventSubscriptionName("messageName_1")*/
//                /*.MessageEventSubscriptionName("messageName_2")*/
//                //.MessageEventSubscriptionName("another")
//                .First();

//            Assert.IsNull(execution);
//        }
//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testProcessVariableValueEqualsNumber()
//        {
//            // long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", 123L}};

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
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", null));

//            // typed null (should not match)
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", "123"));

//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0M))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());

//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0M))
//                .Count());
//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(1, runtimeService.CreateExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void testProcessVariableValueNumberComparison()
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

//            // untyped null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", null));

//            // typed null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            //new Dictionary<string, ITypedValue>(){{"var", "123"));

//            Assert.AreEqual(4, runtimeService.CreateExecutionQuery()
//                .ProcessVariableValueNotEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//        }
//    }
//}