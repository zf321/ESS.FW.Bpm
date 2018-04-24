//using System.Linq;
//using ESS.FW.Bpm.Engine.Impl.JobExecutor;
//using ESS.FW.Bpm.Engine.Management;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Mgmt
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class JobDefinitionQueryTest : PluggableProcessEngineTestCase
//    {
//        [Test]
//        public virtual void testQueryInvalidSortingUsage()
//        {
//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .OrderByJobDefinitionId()
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("call asc() or Desc() after using orderByXX()", e.Message);
//            }

//            try
//            {
//                managementService.CreateJobQuery()
//                    /*.Asc()*/;
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("You should call any of the orderBy methods first before specifying a direction",
//                    e.Message);
//            }
//        }

//        // Test Helpers ////////////////////////////////////////////////////////

//        private void verifyQueryResults(IQueryable<IJobDefinition> query, int countExpected)
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

//        private void verifySingleResultFails(IQueryable<IJobDefinition> query)
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
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByActive()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .Active();
//            //verifyQueryResults(query, 4);

//            // suspend first one
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(AsyncContinuationJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only three active job definitions left
//            //verifyQueryResults(query, 3);

//            // Suspend second one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerStartEventJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only two active job definitions left
//            //verifyQueryResults(query, 2);

//            // suspend third one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerCatchIntermediateEventJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only two active job definitions left
//            //verifyQueryResults(query, 1);

//            // suspend fourth one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerExecuteNestedActivityJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // no one is active
//            //verifyQueryResults(query, 0);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByActivityId()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .ActivityIdIn("ServiceTask_1");
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobDefinitionQuery()
//                .ActivityIdIn("ServiceTask_1", "BoundaryEvent_1");
//            //verifyQueryResults(query, 2);

//            query = managementService.CreateJobDefinitionQuery()
//                .ActivityIdIn("ServiceTask_1", "BoundaryEvent_1", "StartEvent_1");
//            //verifyQueryResults(query, 3);

//            query = managementService.CreateJobDefinitionQuery()
//                .ActivityIdIn("ServiceTask_1", "BoundaryEvent_1", "StartEvent_1", "IntermediateCatchEvent_1");
//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .ActivityIdIn("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .ActivityIdIn(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .ActivityIdIn((string) null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidDefinitionId()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .ProcessDefinitionId("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .ProcessDefinitionId(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidDefinitionKey()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .ProcessDefinitionKey("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .ProcessDefinitionKey(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidJobConfiguration()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .JobConfiguration("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .JobConfiguration(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidJobDefinitionId()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .JobDefinitionId("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .JobDefinitionId(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByInvalidJobType()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .JobType("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobDefinitionQuery()
//                    .JobType(null);
//                Assert.Fail("A ProcessEngineExcpetion was expected.");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByJobDefinitionId()
//        {
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerStartEventJobHandler.TYPE)
//                .First();

//            var query = managementService.CreateJobDefinitionQuery()
//                .JobDefinitionId(jobDefinition.Id);

//            //verifyQueryResults(query, 1);

//            Assert.AreEqual(jobDefinition.Id, query.First()
//                .Id);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByJobType()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .JobType(AsyncContinuationJobHandler.TYPE);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerStartEventJobHandler.TYPE);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerCatchIntermediateEventJobHandler.TYPE);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerExecuteNestedActivityJobHandler.TYPE);
//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByNoCriteria()
//        {
//            var query = managementService.CreateJobDefinitionQuery();
//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
//                .First();

//            var query = managementService.CreateJobDefinitionQuery()
//                .ProcessDefinitionId(processDefinition.Id);
//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryByProcessDefinitionKey()
//        {
//            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
//                .First();

//            var query = managementService.CreateJobDefinitionQuery()
//                .ProcessDefinitionKey(processDefinition.Key);
//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryBySuspended()
//        {
//            var query = managementService.CreateJobDefinitionQuery()
//                .Suspended();
//            //verifyQueryResults(query, 0);

//            // suspend first one
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(AsyncContinuationJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only one is suspended
//            //verifyQueryResults(query, 1);

//            // Suspend second one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerStartEventJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only two are suspended
//            //verifyQueryResults(query, 2);

//            // suspend third one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerCatchIntermediateEventJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // only three are suspended
//            //verifyQueryResults(query, 3);

//            // suspend fourth one
//            jobDefinition = managementService.CreateJobDefinitionQuery()
//                .JobType(TimerExecuteNestedActivityJobHandler.TYPE)
//                .First();
//            managementService.SuspendJobDefinitionById(jobDefinition.Id);

//            // all are suspended
//            //verifyQueryResults(query, 4);
//        }

//        // Pagination //////////////////////////////////////////////////////////

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryPaging()
//        {
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                .ListPage(0, 4)
//                .Count());
//            Assert.AreEqual(1, managementService.CreateJobDefinitionQuery()
//                .ListPage(2, 1)
//                .Count());
//            Assert.AreEqual(2, managementService.CreateJobDefinitionQuery()
//                .ListPage(1, 2)
//                .Count());
//            Assert.AreEqual(3, managementService.CreateJobDefinitionQuery()
//                .ListPage(1, 4)
//                .Count());
//        }

//        // Sorting /////////////////////////////////////////////////////////////

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQuerySorting()
//        {
//            // asc
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByActivityId()*/
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByJobConfiguration()*/
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                .OrderByJobDefinitionId()
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                .OrderByJobType()
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
//                
//                .Count());

//            // Desc
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByActivityId()*/
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByJobConfiguration()*/
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                .OrderByJobDefinitionId()
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                .OrderByJobType()
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
//                
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobDefinitionQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Desc()*/
//                
//                .Count());
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobDefinitionQueryTest.TestBase.bpmn")]
//        public virtual void testQueryWithOverridingJobPriority()
//        {
//            // given
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                /*.ListPage(0, 1)*/
//                .First();
//            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

//            // when
//            var queriedDefinition = managementService.CreateJobDefinitionQuery()
//                .WithOverridingJobPriority()
//                .First();

//            // then
//            Assert.NotNull(queriedDefinition);
//            Assert.AreEqual(jobDefinition.Id, queriedDefinition.Id);
//            Assert.AreEqual(42L, (long) queriedDefinition.OverridingJobPriority);

//            // and
//            Assert.AreEqual(1, managementService.CreateJobDefinitionQuery()
//                .WithOverridingJobPriority()
//                .Count());
//        }
//    }
//}