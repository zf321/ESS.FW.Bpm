using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ProcessDefinitionStatisticsAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // without running instances //////////////////////////////////////////////////////////

        public virtual void testQueryWithoutAuthorizations()
        {
            // given

            // when
            IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnOneTaskProcess()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessDefinitionStatistics statistics = query.First();
            Assert.AreEqual(ONE_TASK_PROCESS_KEY, statistics.Key);
            Assert.AreEqual(0, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.True(!DynamicQueryable.Any(statistics.IncidentStatistics));
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

            // then
            //verifyQueryResults(query, 2);

            IList<IProcessDefinitionStatistics> statistics = query.ToList();
                ;foreach (IProcessDefinitionStatistics result in statistics)
            {
                verifyStatisticsResult(result, 0, 0, 0);
            }
        }

        // including instances //////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingInstancesWithoutProcessInstanceAuthorizations()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<IProcessDefinitionStatistics> statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                .ToList();

            // then
            Assert.AreEqual(2, statistics.Count);

            IProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
            verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

            IProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
            verifyStatisticsResult(oneIncidentProcessStatistics, 3, 0, 0);
        }

        // including failed jobs ////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingFailedJobsWithoutProcessInstanceAuthorizations()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<IProcessDefinitionStatistics> statistics =
                    managementService.CreateProcessDefinitionStatisticsQuery() /*.IncludeFailedJobs()*/
                        .ToList();

                // then
                ;
            ;Assert.AreEqual(2, statistics.Count);

            IProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
            verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

            IProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
            verifyStatisticsResult(oneIncidentProcessStatistics, 3, 3, 0);
        }

        // including incidents //////////////////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingIncidentsWithoutProcessInstanceAuthorizations()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<IProcessDefinitionStatistics> statistics = managementService.CreateProcessDefinitionStatisticsQuery()
                    /*.IncludeIncidents()*/
                    .ToList();
                ;
                // then
                ;Assert.AreEqual(2, statistics.Count);

            IProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
            verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

            IProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
            verifyStatisticsResult(oneIncidentProcessStatistics, 3, 0, 3);
        }

        // including incidents and failed jobs ///////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingIncidentsAndFailedJobsWithoutProcessInstanceAuthorizations()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<IProcessDefinitionStatistics> statistics =
                managementService.CreateProcessDefinitionStatisticsQuery() /*.IncludeIncidents()*//*.IncludeFailedJobs()*/
                    .ToList();

            // then
            Assert.AreEqual(2, statistics.Count);

            IProcessDefinitionStatistics oneTaskProcessStatistics = getStatisticsByKey(statistics, ONE_TASK_PROCESS_KEY);
            verifyStatisticsResult(oneTaskProcessStatistics, 2, 0, 0);

            IProcessDefinitionStatistics oneIncidentProcessStatistics = getStatisticsByKey(statistics, ONE_INCIDENT_PROCESS_KEY);
            verifyStatisticsResult(oneIncidentProcessStatistics, 3, 3, 3);
        }

        // helper ///////////////////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IProcessDefinitionStatistics> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual void verifyStatisticsResult(IProcessDefinitionStatistics statistics, int instances, int failedJobs, int incidents)
        {
            Assert.AreEqual(instances, statistics.Instances, "Instances");
            Assert.AreEqual(failedJobs, statistics.FailedJobs, "Failed Jobs");

            IList<IIncidentStatistics> incidentStatistics = statistics.IncidentStatistics;
            if (incidents == 0)
            {
                Assert.True(incidentStatistics.Count == 0, "Incidents supposed to be empty");
            }
            else
            {
                // the test does have only one type of incidents
                Assert.AreEqual(incidents, incidentStatistics[0].IncidentCount, "Incidents");
            }
        }

        protected internal virtual IProcessDefinitionStatistics getStatisticsByKey(IList<IProcessDefinitionStatistics> statistics, string key)
        {
            foreach (IProcessDefinitionStatistics result in statistics)
            {
                if (key.Equals(result.Key))
                {
                    return result;
                }
            }
            Assert.Fail("No statistics found for key '" + key + "'.");
            return null;
        }
    }

}