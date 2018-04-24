using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class ProcessDefinitionStatisticsQueryTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml") ]
        public virtual void testMultiInstanceProcessDefinitionStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("MIExampleProcess");

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(1, result.Instances);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestSubprocessStatisticsQuery.bpmn20.xml")]
        public virtual void testSubprocessProcessDefinitionStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(1, result.Instances);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml")]
        public virtual void testCallActivityProcessDefinitionStatisticsQuery()
        {
            runtimeService.StartProcessInstanceByKey("callExampleSubProcess");

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            foreach (var result in statistics)
                if (result.Key.Equals("ExampleProcess"))
                {
                    Assert.AreEqual(1, result.Instances);
                    Assert.AreEqual(1, result.FailedJobs);
                }
                else if (result.Key.Equals("callExampleSubProcess"))
                {
                    Assert.AreEqual(1, result.Instances);
                    Assert.AreEqual(0, result.FailedJobs);
                }
                else
                {
                    Assert.Fail(result + " was not expected.");
                }
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryForMultipleVersions()
        {
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")
                .Deploy();

            var definitions = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                
                .ToList();

            foreach (var definition in definitions)
                runtimeService.StartProcessInstanceById(definition.Id);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(0, definitionResult.FailedJobs);

            Assert.True(!definitionResult.IncidentStatistics.Any());

            definitionResult = statistics[1];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(0, definitionResult.FailedJobs);

            Assert.True(!definitionResult.IncidentStatistics.Any());

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryForMultipleVersionsWithFailedJobsAndIncidents()
        {
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")
                .Deploy();

            var definitions = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                
                .ToList();

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            foreach (var definition in definitions)
            {
                //runtimeService.StartProcessInstanceById(definition.Id, parameters);
            }

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(1, definitionResult.FailedJobs);

            var incidentStatistics = definitionResult.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];

            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);

            definitionResult = statistics[1];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(1, definitionResult.FailedJobs);

            incidentStatistics = definitionResult.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            incident = incidentStatistics[0];

            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryForMultipleVersionsWithIncidentType()
        {
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")
                .Deploy();

            var definitions = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                
                .ToList();

            foreach (var definition in definitions)
                runtimeService.StartProcessInstanceById(definition.Id);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                ///*.IncludeIncidentsForType("failedJob")*/
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(0, definitionResult.FailedJobs);

            Assert.True(!definitionResult.IncidentStatistics.Any());

            definitionResult = statistics[1];
            Assert.AreEqual(1, definitionResult.Instances);
            Assert.AreEqual(0, definitionResult.FailedJobs);

            Assert.True(!definitionResult.IncidentStatistics.Any());

            repositoryService.DeleteDeployment(deployment.Id, true);
        }
        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryPagination()
        {
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestStatisticsQuery.bpmn20.xml")
                .Deploy();

            var definitions = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="ExampleProcess")
                
                .ToList();

            foreach (var definition in definitions)
                runtimeService.StartProcessInstanceById(definition.Id);

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.ListPage(0, 1)*/
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") ]
        public virtual void testProcessDefinitionStatisticsQueryWithIncidentsWithoutFailedJobs()
        {
            runtimeService.StartProcessInstanceByKey("callExampleSubProcess");

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeIncidents()*/
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(2, statistics.Count);

            IProcessDefinitionStatistics callExampleSubProcessStaticstics = null;
            IProcessDefinitionStatistics exampleSubProcessStaticstics = null;

            foreach (var current in statistics)
                if (current.Key.Equals("callExampleSubProcess"))
                    callExampleSubProcessStaticstics = current;
                else if (current.Key.Equals("ExampleProcess"))
                    exampleSubProcessStaticstics = current;
                else
                    Assert.Fail(current.Key + " was not expected.");

            Assert.NotNull(callExampleSubProcessStaticstics);
            Assert.NotNull(exampleSubProcessStaticstics);

            // "super" process definition
            Assert.AreEqual(1, callExampleSubProcessStaticstics.Instances);
            Assert.AreEqual(0, callExampleSubProcessStaticstics.FailedJobs);

            Assert.IsFalse(!callExampleSubProcessStaticstics.IncidentStatistics.Any());
            Assert.AreEqual(1, callExampleSubProcessStaticstics.IncidentStatistics.Count());

            var incidentStatistics = callExampleSubProcessStaticstics.IncidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistics.IncidentType);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);

            // "called" process definition
            Assert.AreEqual(1, exampleSubProcessStaticstics.Instances);
            Assert.AreEqual(1, exampleSubProcessStaticstics.FailedJobs);

            Assert.IsFalse(!exampleSubProcessStaticstics.IncidentStatistics.Any());
            Assert.AreEqual(1, exampleSubProcessStaticstics.IncidentStatistics.Count());

            incidentStatistics = exampleSubProcessStaticstics.IncidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistics.IncidentType);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml") ]
        public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeIncidents()*/
                
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
        public virtual void testQueryByIncidentTypeWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
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
            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
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
            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
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
        public virtual void testProcessDefinitionStatisticsProperties()
        {
            var resourceName =
                "resources/api/mgmt/ProcessDefinitionStatisticsQueryTest.TestProcessDefinitionStatisticsProperties.bpmn20.xml";
            var deploymentId = DeploymentForTenant("tenant1", resourceName);

            var processDefinitionStatistics = managementService.CreateProcessDefinitionStatisticsQuery()
                .First();

            Assert.AreEqual("testProcess", processDefinitionStatistics.Key);
            Assert.AreEqual("process name", processDefinitionStatistics.Name);
            Assert.AreEqual("Examples", processDefinitionStatistics.Category);
            Assert.AreEqual(null, processDefinitionStatistics.Description); // it is not parsed for the statistics query
            Assert.AreEqual("tenant1", processDefinitionStatistics.TenantId);
            Assert.AreEqual("v0.1.0", processDefinitionStatistics.VersionTag);
            Assert.AreEqual(deploymentId, processDefinitionStatistics.DeploymentId);
            Assert.AreEqual(resourceName, processDefinitionStatistics.ResourceName);
            Assert.AreEqual(null, processDefinitionStatistics.DiagramResourceName);
            Assert.AreEqual(1, processDefinitionStatistics.Version);
            Assert.AreEqual(0, processDefinitionStatistics.Instances);
            Assert.AreEqual(0, processDefinitionStatistics.FailedJobs);
            Assert.True(!processDefinitionStatistics.IncidentStatistics.Any());
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryCount()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            ExecuteAvailableJobs();

            var Count = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                .Count();

            Assert.AreEqual(1, Count);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithFailedJobs()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(2, definitionResult.Instances);
            Assert.AreEqual(1, definitionResult.FailedJobs);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithIncidents()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(2, definitionResult.Instances);

            Assert.IsFalse(!definitionResult.IncidentStatistics.Any());
            Assert.AreEqual(1, definitionResult.IncidentStatistics.Count);

            var incidentStatistics = definitionResult.IncidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistics.IncidentType);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithIncidentsAndFailedJobs()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeIncidents()*/
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(2, definitionResult.Instances);
            Assert.AreEqual(1, definitionResult.FailedJobs);

            Assert.IsFalse(!definitionResult.IncidentStatistics.Any());
            Assert.AreEqual(1, definitionResult.IncidentStatistics.Count);

            var incidentStatistics = definitionResult.IncidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistics.IncidentType);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithIncidentType()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                ///*.IncludeIncidentsForType("failedJob")*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(2, definitionResult.Instances);

            Assert.IsFalse(!definitionResult.IncidentStatistics.Any());
            Assert.AreEqual(1, definitionResult.IncidentStatistics.Count());

            var incidentStatistics = definitionResult.IncidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incidentStatistics.IncidentType);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithInvalidIncidentType()
        {
            runtimeService.StartProcessInstanceByKey("ExampleProcess");

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                //.IncludeIncidentsForType("invalid")
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(2, definitionResult.Instances);

            Assert.True(definitionResult.IncidentStatistics.Any());
        }

        [Test]
        [Deployment("resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml")]
        public virtual void testProcessDefinitionStatisticsQueryWithoutRunningInstances()
        {
            var statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var definitionResult = statistics[0];
            Assert.AreEqual(0, definitionResult.Instances);
            Assert.AreEqual(0, definitionResult.FailedJobs);

            statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.True(!definitionResult.IncidentStatistics.Any());
        }
    }
}