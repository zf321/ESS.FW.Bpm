using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class DeploymentStatisticsQueryTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment(new []{"resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml"}) ]
        public virtual void testDeploymentStatisticsQueryWithFailedJobs()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            var result = statistics[0];
            Assert.AreEqual(1, result.FailedJobs);
        }

        [Test][Deployment(new []{"resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml"}) ]
        public virtual void testDeploymentStatisticsQueryWithIncidents()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.IsFalse(statistics.Count == 0);
            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            var incidentStatistics = result.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);
        }

        [Test]
        [Deployment(new[] { "resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml" })]
        public virtual void testDeploymentStatisticsQueryWithIncidentType()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /////*.IncludeIncidentsForType("failedJob")*/
                
                .ToList();

            Assert.IsFalse(statistics.Count == 0);
            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            var incidentStatistics = result.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);
        }

        [Test]
        [Deployment(new[] { "resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml" })]
        public virtual void testDeploymentStatisticsQueryWithInvalidIncidentType()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                //.IncludeIncidentsForType("invalid")
                
                .ToList();

            Assert.IsFalse(statistics.Count == 0);
            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            var incidentStatistics = result.IncidentStatistics;
            Assert.True(incidentStatistics.Count == 0);
        }

        [Test]
        [Deployment(new[] { "resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestStatisticsQueryWithFailedJobs.bpmn20.xml" })]
        public virtual void testDeploymentStatisticsQueryWithIncidentsAndFailedJobs()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ExampleProcess", parameters);

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeIncidents()*/
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.IsFalse(statistics.Count == 0);
            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            Assert.AreEqual(1, result.FailedJobs);

            var incidentStatistics = result.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(1, incident.IncidentCount);
        }

        [Test][Deployment(  "resources/api/mgmt/StatisticsTest.TestCallActivityWithIncidentsWithoutFailedJobs.bpmn20.xml") ]
        public virtual void testDeploymentStatisticsQueryWithTwoIncidentsAndOneFailedJobs()
        {
            runtimeService.StartProcessInstanceByKey("callExampleSubProcess");

            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeIncidents()*/
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.IsFalse(statistics.Count == 0);
            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            // has one failed job
            Assert.AreEqual(1, result.FailedJobs);

            var incidentStatistics = result.IncidentStatistics;
            Assert.IsFalse(incidentStatistics.Count == 0);
            Assert.AreEqual(1, incidentStatistics.Count);

            var incident = incidentStatistics[0];
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(2, incident.IncidentCount); // ..but two incidents
        }

        [Test][Deployment( new []{"resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml", "resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml"}) ]
        public virtual void testDeploymentStatisticsQueryWithoutRunningInstances()
        {
            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(0, result.Instances);
            Assert.AreEqual(0, result.FailedJobs);
        }

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml") ]
        public virtual void testQueryByIncidentsWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
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

        [Test][Deployment(  "resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")]
        public virtual void testQueryByIncidentTypeWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
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

        [Test][Deployment( "resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")]
        public virtual void testQueryByFailedJobsWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];

            // there is no running instance
            Assert.AreEqual(0, result.Instances);
            // but there is one failed timer job
            Assert.AreEqual(1, result.FailedJobs);
        }

        [Test] [Deployment("resources/api/mgmt/StatisticsTest.TestFailedTimerStartEvent.bpmn20.xml")] 
        public virtual void testQueryByFailedJobsAndIncidentsWithFailedTimerStartEvent()
        {
            ExecuteAvailableJobs();

            var statistics = managementService.CreateDeploymentStatisticsQuery()
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
        public virtual void testDeploymentStatisticsQuery()
        {
            var deploymentName = "my deployment";

            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml")
                .Name(deploymentName)
                .Deploy();
            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ParGatewayExampleProcess");

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);

            var result = statistics[0];
            Assert.AreEqual(2, result.Instances);
            Assert.AreEqual(0, result.FailedJobs);

            Assert.AreEqual(deployment.Id, result.Id);
            Assert.AreEqual(deploymentName, result.Name);

            // only compare time on second level (i.E. drop milliseconds)
            var cal1 = new DateTime();
            cal1 = new DateTime(deployment.DeploymentTime.Ticks);
            //cal1.Set(DateTime.MILLISECOND, 0);

            var cal2 = new DateTime();
            cal2 = new DateTime(result.DeploymentTime.Ticks);
            //cal2.Set(DateTime.MILLISECOND, 0);

            Assert.True(cal1.Equals(cal2));

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testDeploymentStatisticsQueryCountAndPaging()
        {
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml")
                .Deploy();

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ParGatewayExampleProcess");

            var anotherDeployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestMultiInstanceStatisticsQuery.bpmn20.xml")
                .AddClasspathResource("resources/api/mgmt/StatisticsTest.TestParallelGatewayStatisticsQuery.bpmn20.xml")
                .Deploy();

            runtimeService.StartProcessInstanceByKey("MIExampleProcess");
            runtimeService.StartProcessInstanceByKey("ParGatewayExampleProcess");

            var Count = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                .Count();

            Assert.AreEqual(2, Count);

            var statistics = managementService.CreateDeploymentStatisticsQuery()
                /*.IncludeFailedJobs()*/
                /*.ListPage(0, 1)*/
                .ToList();
            Assert.AreEqual(1, statistics.Count);

            repositoryService.DeleteDeployment(deployment.Id, true);
            repositoryService.DeleteDeployment(anotherDeployment.Id, true);
        }
    }
}