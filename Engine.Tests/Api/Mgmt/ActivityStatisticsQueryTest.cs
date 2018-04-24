using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class ActivityStatisticsQueryTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryWithIncidentsWithoutFailedJobs()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("callExampleSubProcess");

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                /*.IncludeIncidents()*/
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];

            Assert.AreEqual("callSubProcess", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs); // has no failed jobs

            var incidentStatistics = activityResult.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount); //.. but has one incident
        }

        [Test][Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQuery.bpmn20.xml")]
        public virtual void testActivityStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];
            Assert.AreEqual(1, activityResult.Instances);
            Assert.AreEqual("theTask", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs);
            Assert.True(!activityResult.IncidentStatistics.Any());
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQuery.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryCount()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var Count = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                .Count();

            Assert.AreEqual(1, Count);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQuery.bpmn20.xml")]
        public virtual void testManyInstancesActivityStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];
            Assert.AreEqual(3, activityResult.Instances);
            Assert.AreEqual("theTask", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs);
            Assert.True(!activityResult.IncidentStatistics.Any());
        }

        [Test][Deployment(  "resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml") ]
        public virtual void testParallelMultiInstanceActivityStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="MIExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];
            Assert.AreEqual(3, activityResult.Instances);
            Assert.AreEqual("theTask", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs);
            Assert.True(!activityResult.IncidentStatistics.Any());
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestSubprocessStatisticsQuery.bpmn20.xml")]
        public virtual void testSubprocessActivityStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(1, result.Instances);
            Assert.AreEqual("subProcessTask", result.Id);
        }

        [Test][Deployment( new []{ "resources/api/mgmt/StatisticsTest.TestCallActivityStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml"}) ]
        public virtual void testCallActivityActivityStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("callExampleSubProcess");

            ExecuteAvailableJobs();

            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(1, result.Instances);
            Assert.AreEqual(0, result.FailedJobs);
            Assert.True(!result.IncidentStatistics.Any());

            var callSubProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="callExampleSubProcess")
                .First();

            var callSubProcessStatistics = managementService.CreateActivityStatisticsQuery(callSubProcessDefinition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, callSubProcessStatistics.Count);

            result = callSubProcessStatistics[0];
            Assert.AreEqual(1, result.Instances);
            Assert.AreEqual(0, result.FailedJobs);
            Assert.True(!result.IncidentStatistics.Any());
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestActivityStatisticsQueryWithIntermediateTimer.bpmn20.xml") ]
        public virtual void testActivityStatisticsQueryWithIntermediateTimer()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];
            Assert.AreEqual(1, activityResult.Instances);
            Assert.AreEqual("theTimer", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs);
            Assert.True(!activityResult.IncidentStatistics.Any());
        }

        [Test]
        public virtual void testNullProcessDefinitionParameter()
        {
            try
            {
                managementService.CreateActivityStatisticsQuery(null)
                    
                    .ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
                // expected
            }
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml") ]
        public virtual void testActivityStatisticsQueryPagination()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ParGatewayExampleProcess")
                .First();

            runtimeService.StartProcessInstanceById(definition.Id);

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                /*.ListPage(0, 1)*/
                .ToList();

            Assert.AreEqual(1, statistics.Count);
        }

        [Test][Deployment("resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml") ]
        public virtual void testParallelGatewayActivityStatisticsQuery()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ParGatewayExampleProcess")
                .First();

            runtimeService.StartProcessInstanceById(definition.Id);

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            foreach (var result in statistics)
                Assert.AreEqual(1, result.Instances);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestNonInterruptingBoundaryEventStatisticsQuery.bpmn20.xml")]
        public virtual void testNonInterruptingBoundaryEventActivityStatisticsQuery()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var boundaryJob = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(boundaryJob.Id);

            // when
            var activityStatistics =
                managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                    
                    .ToList();

            // then
            Assert.AreEqual(2, activityStatistics.Count);

            var userTaskStatistics = getStatistics(activityStatistics, "task");
            Assert.NotNull(userTaskStatistics);
            Assert.AreEqual("task", userTaskStatistics.Id);
            Assert.AreEqual(1, userTaskStatistics.Instances);

            var afterBoundaryStatistics = getStatistics(activityStatistics, "afterBoundaryTask");
            Assert.NotNull(afterBoundaryStatistics);
            Assert.AreEqual("afterBoundaryTask", afterBoundaryStatistics.Id);
            Assert.AreEqual(1, afterBoundaryStatistics.Instances);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestAsyncInterruptingEventSubProcessStatisticsQuery.bpmn20.xml") ]
        public virtual void testAsyncInterruptingEventSubProcessActivityStatisticsQuery()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            runtimeService.CorrelateMessage("Message");

            // when
            var activityStatistics =
                managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                    .First();

            // then
            Assert.NotNull(activityStatistics);
            Assert.AreEqual("eventSubprocess", activityStatistics.Id);
            Assert.AreEqual(1, activityStatistics.Instances);
        }

        protected internal virtual IActivityStatistics getStatistics(IList<IActivityStatistics> activityStatistics,
            string activityId)
        {
            foreach (var statistics in activityStatistics)
                if (activityId.Equals(statistics.Id))
                    return statistics;

            return null;
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml") ]
        public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process")
                .First();

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            Assert.AreEqual("theStart", result.Id);

            // there is no running activity instance
            Assert.AreEqual(0, result.Instances);

            var incidentStatistics = result.IncidentStatistics;

            // but there is one incident for the failed timer job
            Assert.AreEqual(1, incidentStatistics.Count);

            var incidentStatistic = incidentStatistics[0];
            Assert.AreEqual(1, incidentStatistic.IncidentCount);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistic.IncidentType);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")]
        public virtual void testQueryByIncidentTypeWithFailedTimerStartEvent()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process")
                .First();

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                //.IncludeIncidentsForType(IncidentFields.FailedJobHandlerType)
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            // there is no running instance
            Assert.AreEqual(0, result.Instances);

            var incidentStatistics = result.IncidentStatistics;

            // but there is one incident for the failed timer job
            Assert.AreEqual(1, incidentStatistics.Count);

            var incidentStatistic = incidentStatistics[0];
            Assert.AreEqual(1, incidentStatistic.IncidentCount);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistic.IncidentType);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")]
        public virtual void testQueryByFailedJobsWithFailedTimerStartEvent()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process")
                .First();

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            // there is no running instance
            Assert.AreEqual(0, result.Instances);
            // but there is one failed timer job
            Assert.AreEqual(1, result.FailedJobs);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")]
        public virtual void testQueryByFailedJobsAndIncidentsWithFailedTimerStartEvent()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process")
                .First();

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            // there is no running instance
            Assert.AreEqual(0, result.Instances);
            // but there is one failed timer job
            Assert.AreEqual(1, result.FailedJobs);

            var incidentStatistics = result.IncidentStatistics;

            // and there is one incident for the failed timer job
            Assert.AreEqual(1, incidentStatistics.Count);

            var incidentStatistic = incidentStatistics[0];
            Assert.AreEqual(1, incidentStatistic.IncidentCount);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistic.IncidentType);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQuery.bpmn20.xml")]
        public virtual void FAILING_testActivityStatisticsQueryWithNoInstances()
        {
            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);
            var result = statistics[0];
            Assert.AreEqual("theTask", result.Id);
            Assert.AreEqual(0, result.Instances);
            Assert.AreEqual(0, result.FailedJobs);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryWithIncidents()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            var processInstance = runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];

            var incidentStatistics = activityResult.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryWithIncidentType()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            var processInstance = runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                ///*.IncludeIncidentsForType("failedJob")*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];

            var incidentStatistics = activityResult.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryWithInvalidIncidentType()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            var processInstance = runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                //.IncludeIncidentsForType("invalid")
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];

            var incidentStatistics = activityResult.IncidentStatistics;
            Assert.True(incidentStatistics.Count == 0);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testActivityStatisticsQueryWithoutFailedJobs()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                .First();

            var statistics = managementService.CreateActivityStatisticsQuery(definition.Id)
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var activityResult = statistics[0];
            Assert.AreEqual(1, activityResult.Instances);
            Assert.AreEqual("theServiceTask", activityResult.Id);
            Assert.AreEqual(0, activityResult.FailedJobs);
        }
    }
}