using System.Linq;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricIncidentTest : PluggableProcessEngineTestCase
    {
        private static readonly string PROCESS_DEFINITION_KEY = "oneFailingServiceTaskProcess";

        protected internal virtual void startProcessInstance(string key)
        {
            startProcessInstances(key, 1);
        }

        protected internal virtual void startProcessInstances(string key, int numberOfInstances)
        {
            for (var i = 0; i < numberOfInstances; i++)
                runtimeService.StartProcessInstanceByKey(key);

            ExecuteAvailableJobs();
        }

        [Test]
        [Deployment]
        public virtual void testCreateHistoricIncidentForNestedExecution()
        {
            startProcessInstance("process");

            var execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "serviceTask")
                .First();
            Assert.NotNull(execution);

            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .FirstOrDefault();
            Assert.NotNull(historicIncident);

            Assert.AreEqual(execution.Id, historicIncident.ExecutionId);
            Assert.AreEqual("serviceTask", historicIncident.ActivityId);
        }


        [Test]
        [Deployment(
            new[]
            {
                "resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml",
                "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"
            })]
        public virtual void testCreateRecursiveHistoricIncidents()
        {
            startProcessInstance("process");

            var pi1 = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("process")
                .First();
            Assert.NotNull(pi1);

            var pi2 = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
                .First();
            Assert.NotNull(pi2);

            var query = historyService.CreateHistoricIncidentQuery();

            var rootCauseHistoricIncident = query.Where(c=>c.ProcessInstanceId==pi2.Id)
                .First();
            Assert.NotNull(rootCauseHistoricIncident);

            // cause and root cause id is equal to the id of the root incident
            Assert.AreEqual(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.RootCauseIncidentId);

            var historicIncident = query.Where(c=>c.ProcessInstanceId==pi1.Id)
                .First();
            Assert.NotNull(historicIncident);

            // cause and root cause id is equal to the id of the root incident
            Assert.AreEqual(rootCauseHistoricIncident.Id, historicIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseHistoricIncident.Id, historicIncident.RootCauseIncidentId);
        }

        [Test]
        [Deployment(
            new[]
            {
                "resources/history/HistoricIncidentTest.TestCreateRecursiveHistoricIncidentsForNestedCallActivities.bpmn20.xml",
                "resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml",
                "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"
            })]
        public virtual void testCreateRecursiveHistoricIncidentsForNestedCallActivities()
        {
            startProcessInstance("process1");

            var pi1 = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("process1")
                .First();
            Assert.NotNull(pi1);

            var pi2 = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("process")
                .First();
            Assert.NotNull(pi2);

            var pi3 = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
                .First();
            Assert.NotNull(pi3);

            var query = historyService.CreateHistoricIncidentQuery();

            var rootCauseHistoricIncident = query
                .First(c => c.ProcessInstanceId==pi3.Id);
            Assert.NotNull(rootCauseHistoricIncident);

            // cause and root cause id is equal to the id of the root incident
            Assert.AreEqual(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseHistoricIncident.Id, rootCauseHistoricIncident.RootCauseIncidentId);

            var causeHistoricIncident = query
                .First(c => c.ProcessInstanceId==pi2.Id);
            Assert.NotNull(causeHistoricIncident);

            // cause and root cause id is equal to the id of the root incident
            Assert.AreEqual(rootCauseHistoricIncident.Id, causeHistoricIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseHistoricIncident.Id, causeHistoricIncident.RootCauseIncidentId);

            var historicIncident = query
                .First(c => c.ProcessInstanceId==pi1.Id);
            Assert.NotNull(historicIncident);

            // cause and root cause id is equal to the id of the root incident
            Assert.AreEqual(causeHistoricIncident.Id, historicIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseHistoricIncident.Id, historicIncident.RootCauseIncidentId);
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testCreateSecondHistoricIncident()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;
            managementService.SetJobRetries(jobId, 1);

            ExecuteAvailableJobs();

            var query = historyService.CreateHistoricIncidentQuery();
            Assert.AreEqual(2, query.Count());

            // the first historic incident has been resolved
            Assert.AreEqual(1, query
                .Count(c => c.Resolved));

            query = historyService.CreateHistoricIncidentQuery();
            // a new historic incident exists which is open
            Assert.AreEqual(1, query
                .Count(c => c.Open));
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testDoNotCreateNewIncident()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var pi = runtimeService.CreateProcessInstanceQuery()
                .First();

            var query = historyService.CreateHistoricIncidentQuery(c=>c.ProcessInstanceId== pi.Id);
            var incident = query.First();
            Assert.NotNull(incident);

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // set retries to 1 by job definition id
            managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

            // the incident still exists
            var tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);
            Assert.IsNull(tmp.EndTime);
            Assert.True(tmp.Open);

            // execute the available job (should Assert.Fail again)
            ExecuteAvailableJobs();

            // the incident still exists and there
            // should be not a new incident
            Assert.AreEqual(1, query.Count());
            tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);
            Assert.IsNull(tmp.EndTime);
            Assert.True(tmp.Open);
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testPropertiesOfHistoricIncident()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);

            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .First();
            Assert.NotNull(historicIncident);

            Assert.AreEqual(incident.Id, historicIncident.Id);
            Assert.AreEqual(incident.IncidentTimestamp, historicIncident.CreateTime);
            Assert.IsNull(historicIncident.EndTime);
            Assert.AreEqual(incident.IncidentType, historicIncident.IncidentType);
            Assert.AreEqual(incident.IncidentMessage, historicIncident.IncidentMessage);
            Assert.AreEqual(incident.ExecutionId, historicIncident.ExecutionId);
            Assert.AreEqual(incident.ActivityId, historicIncident.ActivityId);
            Assert.AreEqual(incident.ProcessInstanceId, historicIncident.ProcessInstanceId);
            Assert.AreEqual(incident.ProcessDefinitionId, historicIncident.ProcessDefinitionId);
            Assert.AreEqual(PROCESS_DEFINITION_KEY, historicIncident.ProcessDefinitionKey);
            Assert.AreEqual(incident.CauseIncidentId, historicIncident.CauseIncidentId);
            Assert.AreEqual(incident.RootCauseIncidentId, historicIncident.RootCauseIncidentId);
            Assert.AreEqual(incident.Configuration, historicIncident.Configuration);
            Assert.AreEqual(incident.JobDefinitionId, historicIncident.JobDefinitionId);

            Assert.True(historicIncident.Open);
            Assert.IsFalse(historicIncident.Deleted);
            Assert.IsFalse(historicIncident.Resolved);
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSetHistoricIncidentToDeleted()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var ProcessInstanceId = runtimeService.CreateProcessInstanceQuery()
                .First()
                .Id;
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .First();
            Assert.NotNull(historicIncident);

            Assert.NotNull(historicIncident.EndTime);

            Assert.IsFalse(historicIncident.Open);
            Assert.True(historicIncident.Deleted);
            Assert.IsFalse(historicIncident.Resolved);
        }

        [Test]
        [Deployment(
            new[]
            {
                "resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml",
                "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"
            })]
        public virtual void testSetHistoricIncidentToDeletedRecursive()
        {
            startProcessInstance("process");

            var processInstanceId = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey(PROCESS_DEFINITION_KEY)
                .First()
                .Id;
            runtimeService.DeleteProcessInstance(processInstanceId, null);

            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                
                .ToList();

            foreach (var historicIncident in historicIncidents)
            {
                Assert.NotNull(historicIncident.EndTime);

                Assert.IsFalse(historicIncident.Open);
                Assert.True(historicIncident.Deleted);
                Assert.IsFalse(historicIncident.Resolved);
            }
        }


        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSetHistoricIncidentToResolved()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;
            managementService.SetJobRetries(jobId, 1);

            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .First();
            Assert.NotNull(historicIncident);

            Assert.NotNull(historicIncident.EndTime);

            Assert.IsFalse(historicIncident.Open);
            Assert.IsFalse(historicIncident.Deleted);
            Assert.True(historicIncident.Resolved);
        }

        [Test]
        [Deployment(
            new[]
            {
                "resources/history/HistoricIncidentQueryTest.TestQueryByCauseIncidentId.bpmn20.xml",
                "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml"
            })]
        public virtual void testSetHistoricIncidentToResolvedRecursive()
        {
            startProcessInstance("process");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;
            managementService.SetJobRetries(jobId, 1);

            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                
                .ToList();

            foreach (var historicIncident in historicIncidents)
            {
                Assert.NotNull(historicIncident.EndTime);

                Assert.IsFalse(historicIncident.Open);
                Assert.IsFalse(historicIncident.Deleted);
                Assert.True(historicIncident.Resolved);
            }
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSetRetriesByJobDefinitionIdResolveIncident()
        {
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var pi = runtimeService.CreateProcessInstanceQuery()
                .First();

            var query = historyService.CreateHistoricIncidentQuery(c=>c.ProcessInstanceId== pi.Id);
            var incident = query.First();
            Assert.NotNull(incident);

            runtimeService.SetVariable(pi.Id, "Assert.Fail", false);

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // set retries to 1 by job definition id
            managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

            // the incident still exists
            var tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);
            Assert.IsNull(tmp.EndTime);
            Assert.True(tmp.Open);

            // execute the available job (should Assert.Fail again)
            ExecuteAvailableJobs();

            // the incident still exists and there
            // should be not a new incident
            Assert.AreEqual(1, query.Count());
            tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);
            Assert.NotNull(tmp.EndTime);
            Assert.True(tmp.Resolved);

            AssertProcessEnded(pi.Id);
        }
    }
}