//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Runtime
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class CaseExecutionQueryTest : PluggableProcessEngineTestCase
//    {
//        private static readonly string CASE_DEFINITION_KEY = "oneTaskCase";
//        private static readonly string CASE_DEFINITION_KEY_2 = "twoTaskCase";

//        /// <summary>
//        ///     Setup starts 4 case instances of oneTaskCase
//        ///     and 1 instance of twoTaskCase
//        /// </summary>
//        [SetUp]
//        protected internal virtual void setUp()
//        {
//            //base.SetUp();
//            repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/cmmn/oneTaskCase.cmmn")
//                .AddClasspathResource("resources/api/cmmn/twoTaskCase.cmmn")
//                .Deploy();

//            for (var i = 0; i < 4; i++)
//                caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                    .BusinessKey(i + "")
//                    .Create();
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY_2)
//                .BusinessKey("1")
//                .Create();
//        }
//        [TearDown]
//        protected internal virtual void tearDown()
//        {
//            foreach (var deployment in repositoryService.CreateDeploymentQuery()
//                
//                .ToList())
//                repositoryService.DeleteDeployment(deployment.Id, true);
//            TearDown();
//        }

//        private void verifyQueryResults(ICaseExecutionQuery query, int countExpected)
//        {
//            Assert.AreEqual(countExpected, query
//                .Count());
//            Assert.AreEqual(countExpected, query.Count());

//            if (countExpected == 1)
//                Assert.NotNull(query.First());
//            else if (countExpected > 1)
//                verifySingleResultFails(query);
//            else if (countExpected == 0)
//                Assert.IsNull(query.First());
//        }

//        protected internal virtual void verifyQueryWithOrdering(ICaseExecutionQuery query, int countExpected,
//            TestOrderingUtil.NullTolerantComparator<ICaseExecution> expectedOrdering)
//        {
//            //verifyQueryResults(query, countExpected);
//            TestOrderingUtil.verifySorting(query
//                .ToList();, expectedOrdering);
//        }

//        private void verifySingleResultFails(ICaseExecutionQuery query)
//        {
//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryWithoutQueryParameter()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            //verifyQueryResults(query, 11);
//        }
//        [Test]
//        public virtual void testQueryByCaseDefinitionKey()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 8);

//            query.CaseDefinitionKey(CASE_DEFINITION_KEY_2);

//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseDefinitionKey()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseDefinitionKey("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionKey(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByCaseDefinitionId()
//        {
//            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY)
//                .First()
//                .Id;

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseDefinitionId(caseDefinitionId);

//            //verifyQueryResults(query, 8);

//            caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                .First()
//                .Id;

//            query.CaseDefinitionId(caseDefinitionId);

//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseDefinitionId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseDefinitionId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByCaseInstaceId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            var caseInstances = caseService.CreateCaseInstanceQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY)
//                
//                .ToList();

//            foreach (var caseInstance in caseInstances)
//            {
//                query.CaseInstanceId(caseInstance.Id);

//                //verifyQueryResults(query, 2);
//            }

//            var instance = caseService.CreateCaseInstanceQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                .First();

//            query.CaseInstanceId(instance.Id);

//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseInstanceId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByCaseInstanceBusinessKey()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceBusinessKey("0");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseInstanceBusinessKey()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceBusinessKey("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseInstanceBusinessKey(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByCaseInstanceBusinessKeyAndCaseDefinitionKey()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceBusinessKey("0")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 2);

//            query.CaseInstanceBusinessKey("1")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 2);

//            query.CaseInstanceBusinessKey("2")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 2);

//            query.CaseInstanceBusinessKey("3")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 2);

//            query.CaseInstanceBusinessKey("1")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2);

//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByCaseExecutionId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            var executions = caseService.CreateCaseExecutionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                
//                .ToList();

//            foreach (var execution in executions)
//            {
//                query.CaseExecutionId(execution.Id);

//                //verifyQueryResults(query, 1);
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseExecutionId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseExecutionId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseExecutionId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByActivityId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.ActivityId("PI_HumanTask_1");

//            //verifyQueryResults(query, 5);

//            query.ActivityId("PI_HumanTask_2");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.ActivityId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.ActivityId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneMilestoneCase.cmmn"}) ]
//        public virtual void testQueryByAvailable()
//        {
//            caseService.WithCaseDefinitionByKey("oneMilestoneCase")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.Available();

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByEnabled()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.Enabled();

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByActive()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode);

//            //verifyQueryResults(query, 9);
//        }

//        [Test]
//        public virtual void testQueryByDisabled()
//        {
//            var caseExecutions = caseService.CreateCaseExecutionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                .ActivityId("PI_HumanTask_1")
//                
//                .ToList();

//            foreach (var caseExecution in caseExecutions)
//                caseService.WithCaseExecution(caseExecution.Id)
//                    .Disable();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.Disabled();

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aNullValue", null);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aBooleanValue", true);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aLongValue", (long) 789);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueEquals()
//        {
//            var now = DateTime.Now;
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query//.VariableValueEquals("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueEquals()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query//.VariableValueEquals("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableValueEquals()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query//.VariableValueEquals("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aStringValue", "abd");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aBooleanValue", false);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aLongValue", (long) 790);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueNotEquals()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var before = new DateTime(now.Ticks - 100000);

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aDateValue", before);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueNotEquals("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueNotEquals()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueNotEquals("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableValueNotEquals()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueNotEquals("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThan("aStringValue", "ab");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThan("aShortValue", (short) 122);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThan("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThan("aLongValue", (long) 788);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueGreaterThan()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var before = new DateTime(now.Ticks - 100000);

//            query.VariableValueGreaterThan("aDateValue", before);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThan("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueGreaterThan()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableGreaterThan()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aStringValue", "ab");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aShortValue", (short) 122);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueGreaterThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aLongValue", (long) 788);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aLongValue", (long) 789);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueGreaterThanOrEqual()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var before = new DateTime(now.Ticks - 100000);

//            query.VariableValueGreaterThanOrEqual("aDateValue", before);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueGreaterThanOrEqual("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueGreaterThanOrEqual()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableGreaterThanOrEqual()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThan("aStringValue", "abd");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThan("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThan("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThan("aLongValue", (long) 790);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueLessThan()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var after = new DateTime(now.Ticks + 100000);

//            query.VariableValueLessThan("aDateValue", after);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThan("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueLessThan()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThan("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableLessThan()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThan("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aStringValue", "abd");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueLessThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aLongValue", (long) 790);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aLongValue", (long) 789);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDateVariableValueLessThanOrEqual()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var after = new DateTime(now.Ticks + 100000);

//            query.VariableValueLessThanOrEqual("aDateValue", after);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLessThanOrEqual("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayVariableValueLessThanOrEqual()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableVariableLessThanOrEqual()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullVariableValueLike()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.VariableValueLike("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLike()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLike("aStringValue", "ab%");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLike("aStringValue", "%bc");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseExecutionQuery();

//            query.VariableValueLike("aStringValue", "%b%");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aNullValue", null);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aBooleanValue", true);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aShortValue", (short) 123);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aLongValue", (long) 789);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueEquals()
//        {
//            var now = DateTime.Now;
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aDateValue", now);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueEquals("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueEquals()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueEquals("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableValueEquals()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueEquals("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aStringValue", "abd");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aBooleanValue", false);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aShortValue", (short) 124);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("anIntegerValue", 457);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aLongValue", (long) 790);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueNotEquals()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var before = new DateTime(now.Ticks - 100000);

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aDateValue", before);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueNotEquals("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueNotEquals()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueNotEquals("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableValueNotEquals()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueNotEquals("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThan("aStringValue", "ab");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThan("aShortValue", (short) 122);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThan("anIntegerValue", 455);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThan("aLongValue", (long) 788);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueGreaterThan()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var before = new DateTime(now.Ticks - 100000);

//            query.CaseInstanceVariableValueGreaterThan("aDateValue", before);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThan("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThan()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableGreaterThan()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aStringValue", "ab");

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aShortValue", (short) 122);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("anIntegerValue", 455);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aLongValue", (long) 788);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aLongValue", (long) 789);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var before = new DateTime(now.Ticks - 100000);

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aDateValue", before);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEqual("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEqual("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableGreaterThanOrEqual()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEqual("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThan("aStringValue", "abd");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThan("aShortValue", (short) 124);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThan("anIntegerValue", 457);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThan("aLongValue", (long) 790);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueLessThan()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var after = new DateTime(now.Ticks + 100000);

//            query.CaseInstanceVariableValueLessThan("aDateValue", after);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThan("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThan()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableLessThan()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aStringValue", "abd");

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aShortValue", (short) 124);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueLessThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("anIntegerValue", 457);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByLongCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aLongValue", (long) 790);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aLongValue", (long) 789);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDateCaseInstanceVariableValueLessThanOrEqual()
//        {
//            var now = DateTime.Now;

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDateValue", now)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            var after = new DateTime(now.Ticks + 100000);

//            query.CaseInstanceVariableValueLessThanOrEqual("aDateValue", after);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLessThanOrEqual("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThanOrEqual()
//        {
//            var bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aByteArrayValue", bytes)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEqual("aByteArrayValue", bytes)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryBySerializableCaseInstanceVariableLessThanOrEqual()
//        {
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aSerializableValue", serializable)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEqual("aSerializableValue", serializable)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNullCaseInstanceVariableValueLike()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLike("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringCaseInstanceVariableValueLike()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "ab%");

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "%bc");

//            //verifyQueryResults(query, 2);

//            query = caseService.CreateCaseExecutionQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "%b%");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testCaseVariableValueEqualsNumber()
//        {
//            // long
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", 123L)
//                .Create();

//            // non-matching long
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", 12345L)
//                .Create();

//            // short
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", (short) 123)
//                .Create();

//            // double
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", 123.0d)
//                .Create();

//            // integer
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", 123)
//                .Create();

//            // untyped null (should not match)
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", null)
//                .Create();

//            // typed null (should not match)
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", Variable.Variables.LongValue(null))
//                .Create();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", "123")
//                .Create();

//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(1, caseService.CreateCaseExecutionQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());

//            // other operators
//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                .VariableValueNotEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(1, caseService.CreateCaseExecutionQuery()
//                .VariableValueGreaterThan("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(5, caseService.CreateCaseExecutionQuery()
//                .VariableValueGreaterThanOrEqual("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(0, caseService.CreateCaseExecutionQuery()
//                .VariableValueLessThan("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseExecutionQuery()
//                .VariableValueLessThanOrEqual("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            // two executions per case instance match the query
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(2, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());

//            // other operators
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueNotEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(2, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueGreaterThan("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(10, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueGreaterThanOrEqual("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(0, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueLessThan("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(8, caseService.CreateCaseExecutionQuery()
//                .CaseInstanceVariableValueLessThanOrEqual("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//        }


//        [Test]
//        public virtual void testQuerySorting()
//        {
//            var query = caseService.CreateCaseExecutionQuery();

//            // asc
//            query//.OrderByCaseDefinitionId()
//                /*.Asc()*/;
//            //verifyQueryWithOrdering(query, 11, caseExecutionByDefinitionId());

//            query = caseService.CreateCaseExecutionQuery();

//            query.OrderByCaseDefinitionKey()
//                /*.Asc()*/;
//            //verifyQueryWithOrdering(query, 11, caseExecutionByDefinitionKey(processEngine));

//            query = caseService.CreateCaseExecutionQuery();

//            query//.OrderByCaseExecutionId()
//                /*.Asc()*/;
//            //verifyQueryWithOrdering(query, 11, caseExecutionById());


//            // Desc

//            query = caseService.CreateCaseExecutionQuery();

//            query//.OrderByCaseDefinitionId()
//                /*.Desc()*/;
//            //verifyQueryWithOrdering(query, 11, inverted(caseExecutionByDefinitionId()));

//            query = caseService.CreateCaseExecutionQuery();

//            query.OrderByCaseDefinitionKey()
//                /*.Desc()*/;
//            ////verifyQueryWithOrdering(query, 11, inverted(caseExecutionByDefinitionKey(processEngine)));

//            query = caseService.CreateCaseExecutionQuery();

//            query//.OrderByCaseExecutionId()
//                /*.Desc()*/;
//            //verifyQueryWithOrdering(query, 11, inverted(caseExecutionById()));

//            query = caseService.CreateCaseExecutionQuery();
//        }

//        [Test]
//        public virtual void testCaseExecutionProperties()
//        {
//            // given
//            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY)
//                .First()
//                .Id;

//            var caseInstanceId = caseService.WithCaseDefinition(caseDefinitionId)
//                .Create()
//                .Id;

//            // when
//            var task = caseService.CreateCaseExecutionQuery()
//                .CaseInstanceId(caseInstanceId)
//                .ActivityId("PI_HumanTask_1")
//                .First();

//            // then
//            Assert.AreEqual("PI_HumanTask_1", task.ActivityId);
//            Assert.AreEqual("A HumanTask", task.ActivityName);
//            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
//            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
//            Assert.AreEqual(caseInstanceId, task.ParentId);
//            Assert.AreEqual("humanTask", task.ActivityType);
//            Assert.NotNull(task.ActivityDescription);
//            Assert.NotNull(task.Id);
//        }
//        [Test]
//        [Deployment("resources/cmmn/required/RequiredRuleTest.TestVariableBasedRule.cmmn")]
//        public virtual void testQueryByRequired()
//        {
//            caseService.CreateCaseInstanceByKey("case" /*, new Dictionary<string, ITypedValue>(){{"required", true)*/);

//            var query = caseService.CreateCaseExecutionQuery()
//                .Required();

//            //verifyQueryResults(query, 1);

//            var execution = query.First();
//            Assert.NotNull(execution);
//            Assert.True(execution.Required);
//        }
//    }
//}