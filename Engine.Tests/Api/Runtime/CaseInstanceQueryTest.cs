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
//    public class CaseInstanceQueryTest : PluggableProcessEngineTestCase
//    {
//        private static readonly string CASE_DEFINITION_KEY = "oneTaskCase";
//        private static readonly string CASE_DEFINITION_KEY_2 = "oneTaskCase2";

//        private IList<string> caseInstanceIds;

//        /// <summary>
//        ///     Setup starts 4 case instances of oneTaskCase
//        ///     and 1 instance of oneTaskCase2
//        /// </summary>
//        [SetUp]
//        protected internal virtual void setUp()
//        {
//            //base.SetUp();
//            repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/cmmn/oneTaskCase.cmmn")
//                .AddClasspathResource("resources/api/cmmn/oneTaskCase2.cmmn")
//                .Deploy();

//            caseInstanceIds = new List<string>();
//            for (var i = 0; i < 4; i++)
//            {
//                var idd = caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                    .BusinessKey(i.ToString())
//                    .Create()
//                    .Id;

//                caseInstanceIds.Add(idd);
//            }
//            var id = caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY_2)
//                .BusinessKey("1")
//                .Create()
//                .Id;

//            caseInstanceIds.Add(id);
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

//        private void verifyQueryResults(ICaseInstanceQuery query, int countExpected)
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

//        private void verifySingleResultFails(ICaseInstanceQuery query)
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
//        public virtual void testCaseInstanceProperties()
//        {
//            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                .First()
//                .Id;

//            var caseInstance = caseService.CreateCaseInstanceQuery()
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2)
//                .First();

//            Assert.NotNull(caseInstance.Id);
//            Assert.AreEqual(caseInstance.Id, caseInstance.CaseInstanceId);
//            Assert.AreEqual("1", caseInstance.BusinessKey);
//            Assert.AreEqual(caseDefinitionId, caseInstance.CaseDefinitionId);
//            Assert.AreEqual("CasePlanModel_1", caseInstance.ActivityId);
//            Assert.IsNull(caseInstance.ActivityName);
//            Assert.IsNull(caseInstance.ParentId);
//            Assert.True(caseInstance.Active);
//            Assert.IsFalse(caseInstance.Enabled);
//        }

//        [Test]
//        public virtual void testQueryWithoutQueryParameter()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            //verifyQueryResults(query, 5);
//        }

//        [Test]
//        public virtual void testQueryByCaseDefinitionKey()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            query.CaseDefinitionKey(CASE_DEFINITION_KEY_2);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseDefinitionKey()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            query.CaseDefinitionId(caseDefinitionId);

//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseDefinitionId()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

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
//        public virtual void testQueryByActive()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode);

//            //verifyQueryResults(query, 5);
//        }

//        [Test]
//        public virtual void testQueryByCompleted()
//        {
//            foreach (var deployment in repositoryService.CreateDeploymentQuery()
//                
//                .ToList())
//                repositoryService.DeleteDeployment(deployment.Id, true);

//            repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/cmmn/oneTaskCaseWithManualActivation.cmmn")
//                .Deploy();

//            for (var i = 0; i < 4; i++)
//            {
//                var id = caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                    .BusinessKey(i.ToString())
//                    .Create()
//                    .Id;

//                caseInstanceIds.Add(id);
//            }

//            var executions = caseService.CreateCaseExecutionQuery()
//                .ActivityId("PI_HumanTask_1")
//                
//                .ToList();

//            foreach (var caseExecution in executions)
//                caseService.WithCaseExecution(caseExecution.Id)
//                    .Disable();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.Completed();

//            //verifyQueryResults(query, 4);
//        }
//        [Test]
//        [Deployment(new string[] {"resources/api/runtime/CaseInstanceQueryTest.TestQueryByTerminated.cmmn"}) ]
//        public virtual void testQueryByTerminated()
//        {
//            var caseInstanceId = caseService.WithCaseDefinitionByKey("termination")
//                .Create()
//                .Id;

//            var caseExecutionId = caseService.CreateCaseExecutionQuery()
//                .ActivityId("PI_HumanTask_1")
//                .CaseInstanceId(caseInstanceId)
//                .First()
//                .Id;

//            caseService.WithCaseExecution(caseExecutionId)
//                .Complete();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.Terminated();

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByCaseInstanceBusinessKey()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            query.CaseInstanceBusinessKey("1");

//            //verifyQueryResults(query, 2);
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseInstanceBusinessKey()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

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
//            var query = caseService.CreateCaseInstanceQuery();

//            query.CaseInstanceBusinessKey("0")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKey("1")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKey("2")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKey("3")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY);

//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKey("1")
//                .CaseDefinitionKey(CASE_DEFINITION_KEY_2);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByCaseInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            foreach (var caseInstanceId in caseInstanceIds)
//            {
//                query.CaseInstanceId(caseInstanceId);

//                //verifyQueryResults(query, 1);
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidCaseInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

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
//        public virtual void testQueryByNullVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aNullValue", null)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("aNullValue", null);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("aBooleanValue", true);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            query//.VariableValueEquals("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueNotEquals("aStringValue", "abd");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueNotEquals("aBooleanValue", false);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueNotEquals("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueNotEquals("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueNotEquals("aDateValue", before);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueNotEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThan("aStringValue", "ab");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueGreaterThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThan("aShortValue", (short) 122);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThan("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueGreaterThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aStringValue", "ab");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueGreaterThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aShortValue", (short) 122);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueGreaterThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aLongValue", (long) 788);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            var before = new DateTime(now.Ticks - 100000);

//            query.VariableValueGreaterThanOrEqual("aDateValue", before);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueGreaterThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueGreaterThanOrEqual("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueLessThan("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThan("aStringValue", "abd");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueLessThan("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThan("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThan("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueLessThan()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aStringValue", "abd");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByBooleanVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aBooleanValue", true)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueLessThanOrEqual("aBooleanValue", false)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByShortVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aShortValue", (short) 123)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aShortValue", (short) 124);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aShortValue", (short) 123);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByIntegerVariableValueLessThanOrEquals()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("anIntegerValue", 456)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByLongVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aLongValue", (long) 789)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aLongValue", (long) 790);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            var after = new DateTime(now.Ticks + 100000);

//            query.VariableValueLessThanOrEqual("aDateValue", after);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByDoubleVariableValueLessThanOrEqual()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aDoubleValue", 1.5)
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLessThanOrEqual("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

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

//            var query = caseService.CreateCaseInstanceQuery();

//            try
//            {
//                query.VariableValueLike("aNullValue", null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByStringVariableValueLike()
//        {
//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("aStringValue", "abc")
//                .Create();

//            var query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLike("aStringValue", "ab%");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLike("aStringValue", "%bc");

//            //verifyQueryResults(query, 1);

//            query = caseService.CreateCaseInstanceQuery();

//            query.VariableValueLike("aStringValue", "%b%");

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            var query = caseService.CreateCaseInstanceQuery();

//            // asc
//            query//.OrderByCaseDefinitionId()
//                /*.Asc()*/;
//            //verifyQueryResults(query, 5);

//            query = caseService.CreateCaseInstanceQuery();

//            query.OrderByCaseDefinitionKey()
//                /*.Asc()*/;
//            //verifyQueryResults(query, 5);

//            query = caseService.CreateCaseInstanceQuery();

//            query//.OrderByCaseInstanceId()
//                /*.Asc()*/;
//            //verifyQueryResults(query, 5);

//            // Desc

//            query = caseService.CreateCaseInstanceQuery();

//            query//.OrderByCaseDefinitionId()
//                /*.Desc()*/;
//            //verifyQueryResults(query, 5);

//            query = caseService.CreateCaseInstanceQuery();

//            query.OrderByCaseDefinitionKey()
//                /*.Desc()*/;
//            //verifyQueryResults(query, 5);

//            query = caseService.CreateCaseInstanceQuery();

//            query//.OrderByCaseInstanceId()
//                /*.Desc()*/;
//            //verifyQueryResults(query, 5);

//            query = caseService.CreateCaseInstanceQuery();
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
//                .SetVariable("var", Variable.Variables.LongValue(0))
//                .Create();

//            caseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY)
//                .SetVariable("var", "123")
//                .Create();

//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());

//            Assert.AreEqual(1, caseService.CreateCaseInstanceQuery()
//                //.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                .Count());

//            // other operators
//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                .VariableValueNotEquals("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(1, caseService.CreateCaseInstanceQuery()
//                .VariableValueGreaterThan("var", Engine.Variable.Variables.NumberValue(123L))
//                .Count());
//            Assert.AreEqual(5, caseService.CreateCaseInstanceQuery()
//                .VariableValueGreaterThanOrEqual("var", Engine.Variable.Variables.NumberValue(123.0m))
//                .Count());
//            Assert.AreEqual(0, caseService.CreateCaseInstanceQuery()
//                .VariableValueLessThan("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//            Assert.AreEqual(4, caseService.CreateCaseInstanceQuery()
//                .VariableValueLessThanOrEqual("var", Engine.Variable.Variables.NumberValue(123))
//                .Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml"}) ]
//        public virtual void testQueryBySuperProcessInstanceId()
//        {
//            var superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest")
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .SuperProcessInstanceId(superProcessInstanceId);

//            //verifyQueryResults(query, 1);
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/superProcessWithCaseCallActivityInsideSubProcess.bpmn20.xml"}) ]
//        public virtual void testQueryBySuperProcessInstanceIdNested()
//        {
//            var superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest")
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .SuperProcessInstanceId(superProcessInstanceId);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSuperProcessInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery()
//                .SuperProcessInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.SuperProcessInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//            }
//        }

//        [Test][Deployment(new string[] { "resources/api/cmmn/oneProcessTaskCase.cmmn", "resources/api/runtime/oneTaskProcess.bpmn20.xml"}) ]
//        public virtual void testQueryBySubProcessInstanceId()
//        {
//            var superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneProcessTaskCase")
//                .Id;

//            var subProcessInstanceId = runtimeService.CreateProcessInstanceQuery()
//                .SetSuperCaseInstanceId(superCaseInstanceId)
//                .First()
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .SubProcessInstanceId(subProcessInstanceId);

//            //verifyQueryResults(query, 1);

//            var caseInstance = query.First();
//            Assert.AreEqual(superCaseInstanceId, caseInstance.Id);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSubProcessInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery()
//                .SubProcessInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.SubProcessInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//                // expected
//            }
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneCaseTaskCase.cmmn"}) ]
//        public virtual void testQueryBySuperCaseInstanceId()
//        {
//            var superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneCaseTaskCase")
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .SuperCaseInstanceId(superCaseInstanceId);

//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSuperCaseInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery()
//                .SuperCaseInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.SuperCaseInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//                // expected
//            }
//        }
//        [Test]
//        [Deployment(new string[] {"resources/api/cmmn/oneCaseTaskCase.cmmn"}) ]
//        public virtual void testQueryBySubCaseInstanceId()
//        {
//            var superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneCaseTaskCase")
//                .Id;

//            var subCaseInstanceId = caseService.CreateCaseInstanceQuery()
//                .SuperCaseInstanceId(superCaseInstanceId)
//                .First()
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .SubCaseInstanceId(subCaseInstanceId);

//            //verifyQueryResults(query, 1);

//            var caseInstance = query.First();
//            Assert.AreEqual(superCaseInstanceId, caseInstance.Id);
//        }

//        [Test]
//        public virtual void testQueryByInvalidSubCaseInstanceId()
//        {
//            var query = caseService.CreateCaseInstanceQuery()
//                .SubCaseInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.SubCaseInstanceId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByDeploymentId()
//        {
//            var deploymentId = repositoryService.CreateDeploymentQuery()
//                .First()
//                .Id;

//            var query = caseService.CreateCaseInstanceQuery()
//                .DeploymentId(deploymentId);

//            //verifyQueryResults(query, 5);
//        }

//        [Test]
//        public virtual void testQueryByInvalidDeploymentId()
//        {
//            var query = caseService.CreateCaseInstanceQuery()
//                .DeploymentId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.DeploymentId(null);
//                Assert.Fail();
//            }
//            catch (NotValidException)
//            {
//                // expected
//            }
//        }
//    }
//}