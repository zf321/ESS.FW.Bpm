using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    [TestFixture]
    public class ActivityStatisticsAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

        protected internal string deploymentId;
        
        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // without any authorization

        public virtual void testQueryWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.CreateActivityStatisticsQuery(processDefinitionId).ToList();
                Assert.Fail("Exception expected: It should not be possible to execute the activity statistics query");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        // including instances //////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingInstancesWithoutAuthorizationOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IActivityStatistics> query = managementService.CreateActivityStatisticsQuery(processDefinitionId);

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryIncludingInstancesWithReadPermissionOnOneProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            disableAuthorization();
            string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId).First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(1, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        public virtual void testQueryIncludingInstancesWithMany()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            disableAuthorization();
            string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId).First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(1, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        public virtual void testQueryIncludingInstancesWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId).First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        public virtual void testQueryIncludingInstancesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read, Permissions.ReadInstance);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId).First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        // including failed jobs //////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingFailedJobsWithoutAuthorizationOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeFailedJobs()*/.First();

            // then
            Assert.IsNull(statistics);
        }

        public virtual void testQueryIncludingFailedJobsWithReadPermissionOnOneProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            disableAuthorization();
            string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(1, statistics.Instances);
            Assert.AreEqual(1, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        public virtual void testQueryIncludingFailedJobsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(3, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read, Permissions.ReadInstance);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(3, statistics.FailedJobs);
            Assert.True(!statistics.IncidentStatistics.Any());
        }

        // including incidents //////////////////////////////////////////////////////////////

        public virtual void testQueryIncludingIncidentsWithoutAuthorizationOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*/.First();

            // then
            Assert.IsNull(statistics);
        }

        public virtual void testQueryIncludingIncidentsWithReadPermissionOnOneProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            disableAuthorization();
            string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(1, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        public virtual void testQueryIncludingIncidentsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(3, incidentStatistics.IncidentCount);
        }

        public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read, Permissions.ReadInstance);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(0, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(3, incidentStatistics.IncidentCount);
        }

        // including incidents and failed jobs //////////////////////////////////////////////////////////

        public virtual void testQueryIncludingIncidentsAndFailedJobsWithoutAuthorizationOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*//*.IncludeFailedJobs()*/.First();

            // then
            Assert.IsNull(statistics);
        }

        public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadPermissionOnOneProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            disableAuthorization();
            string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*//*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(1, statistics.Instances);
            Assert.AreEqual(1, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(1, incidentStatistics.IncidentCount);
        }

        public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*//*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(3, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(3, incidentStatistics.IncidentCount);
        }

        public virtual void testQueryIncludingIncidentsAndFailedJobsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.Read, Permissions.ReadInstance);

            // when
            IActivityStatistics statistics = managementService.CreateActivityStatisticsQuery(processDefinitionId)/*.IncludeIncidents()*//*.IncludeFailedJobs()*/.First();

            // then
            Assert.NotNull(statistics);
            Assert.AreEqual("scriptTask", statistics.Id);
            Assert.AreEqual(3, statistics.Instances);
            Assert.AreEqual(3, statistics.FailedJobs);
            Assert.IsFalse(!statistics.IncidentStatistics.Any());
            IIncidentStatistics incidentStatistics = statistics.IncidentStatistics.ElementAt(0);
            Assert.AreEqual(3, incidentStatistics.IncidentCount);
        }

        // helper ///////////////////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IActivityStatistics> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}