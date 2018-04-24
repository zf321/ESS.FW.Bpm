using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class IncidentTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testShouldCreateOneIncident()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(incident);

            Assert.NotNull(incident.Id);
            Assert.NotNull(incident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(AlwaysFailingDelegate.Message, incident.IncidentMessage);
            Assert.AreEqual(processInstance.Id, incident.ExecutionId);
            Assert.AreEqual("theServiceTask", incident.ActivityId);
            Assert.AreEqual(processInstance.Id, incident.ProcessInstanceId);
            Assert.AreEqual(processInstance.ProcessDefinitionId, incident.ProcessDefinitionId);
            Assert.AreEqual(incident.Id, incident.CauseIncidentId);
            Assert.AreEqual(incident.Id, incident.RootCauseIncidentId);

            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(job);

            Assert.AreEqual(job.Id, incident.Configuration);
            Assert.AreEqual(job.JobDefinitionId, incident.JobDefinitionId);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testShouldCreateOneIncidentAfterSetRetries()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            var incidents = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();

            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 1);

            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(job);

            // set job retries to 1 -> should Assert.Fail again and a second incident should be created
            managementService.SetJobRetries(job.Id, 1);

            ExecuteAvailableJobs();

            incidents = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();

            // There is still one incident
            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 1);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testShouldCreateOneIncidentAfterExecuteJob()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            var incidents = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();

            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 1);

            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(job);

            // set job retries to 1 -> should Assert.Fail again and a second incident should be created
            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail("Exception was expected.");
            }
            catch (ProcessEngineException)
            {
                // exception expected
            }

            incidents = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();

            // There is still one incident
            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 1);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncidentForNestedExecution.bpmn")]
        public virtual void testShouldCreateOneIncidentForNestedExecution()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcessWithNestedExecutions");

            ExecuteAvailableJobs();

            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(incident);

            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(job);

            var executionIdOfNestedFailingExecution = job.ExecutionId;

            Assert.IsFalse(processInstance.Id == executionIdOfNestedFailingExecution);

            Assert.NotNull(incident.Id);
            Assert.NotNull(incident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, incident.IncidentType);
            Assert.AreEqual(AlwaysFailingDelegate.Message, incident.IncidentMessage);
            Assert.AreEqual(executionIdOfNestedFailingExecution, incident.ExecutionId);
            Assert.AreEqual("theServiceTask", incident.ActivityId);
            Assert.AreEqual(processInstance.Id, incident.ProcessInstanceId);
            Assert.AreEqual(incident.Id, incident.CauseIncidentId);
            Assert.AreEqual(incident.Id, incident.RootCauseIncidentId);
            Assert.AreEqual(job.Id, incident.Configuration);
            Assert.AreEqual(job.JobDefinitionId, incident.JobDefinitionId);
        }

        [Test][Deployment( new []{"resources/api/mgmt/IncidentTest.TestShouldCreateRecursiveIncidents.bpmn", "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn"})]
        public virtual void testShouldCreateRecursiveIncidents()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("callFailingProcess");

            ExecuteAvailableJobs();

            var incidents = runtimeService.CreateIncidentQuery()
                .ToList();
            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 2);

            var failingProcessDef = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "failingProcess")
                .First();
            var failingProcess = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == failingProcessDef.Id)
                //.SetProcessDefinitionKey("failingProcess")
                .First();
            Assert.NotNull(failingProcess);

            var callProcessDef = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "callFailingProcess")
                .First();
            var callProcess = runtimeService.CreateProcessInstanceQuery(c => c.ProcessDefinitionId == callProcessDef.Id)
                //.SetProcessDefinitionKey("callFailingProcess")
                .First();
            Assert.NotNull(callProcess);

            // Root cause incident
            var causeIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==failingProcess.ProcessDefinitionId)
                .First();
            Assert.NotNull(causeIncident);

            var job = managementService.CreateJobQuery(c=>c.ExecutionId ==causeIncident.ExecutionId)
                .First();
            Assert.NotNull(job);

            Assert.NotNull(causeIncident.Id);
            Assert.NotNull(causeIncident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, causeIncident.IncidentType);
            Assert.AreEqual(AlwaysFailingDelegate.Message, causeIncident.IncidentMessage);
            Assert.AreEqual(job.ExecutionId, causeIncident.ExecutionId);
            Assert.AreEqual("theServiceTask", causeIncident.ActivityId);
            Assert.AreEqual(failingProcess.Id, causeIncident.ProcessInstanceId);
            Assert.AreEqual(causeIncident.Id, causeIncident.CauseIncidentId);
            Assert.AreEqual(causeIncident.Id, causeIncident.RootCauseIncidentId);
            Assert.AreEqual(job.Id, causeIncident.Configuration);
            Assert.AreEqual(job.JobDefinitionId, causeIncident.JobDefinitionId);

            // Recursive created incident
            var recursiveCreatedIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==callProcess.ProcessDefinitionId)
                .First();
            Assert.NotNull(recursiveCreatedIncident);

            var theCallActivityExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "theCallActivity")
                .First();
            Assert.NotNull(theCallActivityExecution);

            Assert.NotNull(recursiveCreatedIncident.Id);
            Assert.NotNull(recursiveCreatedIncident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, recursiveCreatedIncident.IncidentType);
            Assert.IsNull(recursiveCreatedIncident.IncidentMessage);
            Assert.AreEqual(theCallActivityExecution.Id, recursiveCreatedIncident.ExecutionId);
            Assert.AreEqual("theCallActivity", recursiveCreatedIncident.ActivityId);
            Assert.AreEqual(processInstance.Id, recursiveCreatedIncident.ProcessInstanceId);
            Assert.AreEqual(causeIncident.Id, recursiveCreatedIncident.CauseIncidentId);
            Assert.AreEqual(causeIncident.Id, recursiveCreatedIncident.RootCauseIncidentId);
            Assert.IsNull(recursiveCreatedIncident.Configuration);
            Assert.IsNull(recursiveCreatedIncident.JobDefinitionId);
        }

        [Test][Deployment( new []{"resources/api/mgmt/IncidentTest.TestShouldCreateRecursiveIncidentsForNestedCallActivity.bpmn", "resources/api/mgmt/IncidentTest.TestShouldCreateRecursiveIncidents.bpmn", "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn"}) ]
        public virtual void testShouldCreateRecursiveIncidentsForNestedCallActivity()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("callingFailingCallActivity");

            ExecuteAvailableJobs();

            var incidents = runtimeService.CreateIncidentQuery()
                
                .ToList();
            Assert.IsFalse(incidents.Count == 0);
            Assert.True(incidents.Count == 3);

            // Root Cause IIncident

            var failingProcessDef = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "failingProcess")
                .First();
            var failingProcess = runtimeService.CreateProcessInstanceQuery(c => c.ProcessDefinitionId == failingProcessDef.Id)
                //.SetProcessDefinitionKey("failingProcess")
                .First();
            Assert.NotNull(failingProcess);

            var rootCauseIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==failingProcess.ProcessDefinitionId)
                .First();
            Assert.NotNull(rootCauseIncident);

            var job = managementService.CreateJobQuery(c=>c.ExecutionId ==rootCauseIncident.ExecutionId)
                .First();
            Assert.NotNull(job);

            Assert.NotNull(rootCauseIncident.Id);
            Assert.NotNull(rootCauseIncident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, rootCauseIncident.IncidentType);
            Assert.AreEqual(AlwaysFailingDelegate.Message, rootCauseIncident.IncidentMessage);
            Assert.AreEqual(job.ExecutionId, rootCauseIncident.ExecutionId);
            Assert.AreEqual("theServiceTask", rootCauseIncident.ActivityId);
            Assert.AreEqual(failingProcess.Id, rootCauseIncident.ProcessInstanceId);
            Assert.AreEqual(rootCauseIncident.Id, rootCauseIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseIncident.Id, rootCauseIncident.RootCauseIncidentId);
            Assert.AreEqual(job.Id, rootCauseIncident.Configuration);
            Assert.AreEqual(job.JobDefinitionId, rootCauseIncident.JobDefinitionId);

            // Cause IIncident
            var callProcessDef = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "callFailingProcess")
                .First();
            var callFailingProcess = runtimeService.CreateProcessInstanceQuery(c => c.ProcessDefinitionId == callProcessDef.Id)
                //.SetProcessDefinitionKey("callFailingProcess")
                .First();
            Assert.NotNull(callFailingProcess);

            var causeIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==callFailingProcess.ProcessDefinitionId)
                .First();
            Assert.NotNull(causeIncident);

            var theCallActivityExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "theCallActivity")
                .First();
            Assert.NotNull(theCallActivityExecution);

            Assert.NotNull(causeIncident.Id);
            Assert.NotNull(causeIncident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, causeIncident.IncidentType);
            Assert.IsNull(causeIncident.IncidentMessage);
            Assert.AreEqual(theCallActivityExecution.Id, causeIncident.ExecutionId);
            Assert.AreEqual("theCallActivity", causeIncident.ActivityId);
            Assert.AreEqual(callFailingProcess.Id, causeIncident.ProcessInstanceId);
            Assert.AreEqual(rootCauseIncident.Id, causeIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseIncident.Id, causeIncident.RootCauseIncidentId);
            Assert.IsNull(causeIncident.Configuration);
            Assert.IsNull(causeIncident.JobDefinitionId);

            // Top level incident of the startet process (recursive created incident for super super process instance)
            var topLevelIncident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==processInstance.ProcessDefinitionId)
                .First();
            Assert.NotNull(topLevelIncident);

            var theCallingCallActivity = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "theCallingCallActivity")
                .First();
            Assert.NotNull(theCallingCallActivity);

            Assert.NotNull(topLevelIncident.Id);
            Assert.NotNull(topLevelIncident.IncidentTimestamp);
            Assert.AreEqual(IncidentFields.FailedJobHandlerType, topLevelIncident.IncidentType);
            Assert.IsNull(topLevelIncident.IncidentMessage);
            Assert.AreEqual(theCallingCallActivity.Id, topLevelIncident.ExecutionId);
            Assert.AreEqual("theCallingCallActivity", topLevelIncident.ActivityId);
            Assert.AreEqual(processInstance.Id, topLevelIncident.ProcessInstanceId);
            Assert.AreEqual(causeIncident.Id, topLevelIncident.CauseIncidentId);
            Assert.AreEqual(rootCauseIncident.Id, topLevelIncident.RootCauseIncidentId);
            Assert.IsNull(topLevelIncident.Configuration);
            Assert.IsNull(topLevelIncident.JobDefinitionId);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testShouldDeleteIncidentAfterJobHasBeenDeleted()
        {
            // start failing process
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            // get the job
            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(job);

            // there exists one incident to failed
            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(incident);

            // Delete the job
            managementService.DeleteJob(job.Id);

            // the incident has been deleted too.
            incident = runtimeService.CreateIncidentQuery(c=> c.Id==incident.Id)
                .FirstOrDefault();
            Assert.IsNull(incident);
        }

        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldDeleteIncidentAfterJobWasSuccessfully.bpmn")]
        public virtual void testShouldDeleteIncidentAfterJobWasSuccessfully()
        {
            // Start process instance
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["fail"] = true;
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcessWithUserTask", parameters);

            ExecuteAvailableJobs();

            // job exists
            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .FirstOrDefault();
            Assert.NotNull(job);

            // incident was created
            var incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(incident);

            // set execution variable from "true" to "false"
            runtimeService.SetVariable(processInstance.Id, "fail", false);

            // set retries of failed job to 1, with the change of the Assert.Fail variable the job
            // will be executed successfully
            managementService.SetJobRetries(job.Id, 1);

            ExecuteAvailableJobs();

            // Update process instance
            processInstance = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessInstanceId == processInstance.Id)
                .FirstOrDefault();
            Assert.True(processInstance is ExecutionEntity);

            // should stay in the user task
            var exec = (ExecutionEntity) processInstance;
            Assert.AreEqual("theUserTask", exec.ActivityId);

            // there does not exist any incident anymore
            incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .FirstOrDefault();
            Assert.IsNull(incident);
        }

        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldCreateIncidentOnFailedStartTimerEvent.bpmn") ]
        public virtual void testShouldCreateIncidentOnFailedStartTimerEvent()
        {
            // After process start, there should be timer created
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            var job = jobQuery.First();
            var jobId = job.Id;

            while (0 != job.Retries)
            {
                try
                {
                    managementService.ExecuteJob(jobId);
                    Assert.Fail();
                }
                catch (System.Exception)
                {
                    // expected
                }
                job = jobQuery.FirstOrDefault(c => c.Id == jobId);
            }

            // job exists
            job = jobQuery.First();
            Assert.NotNull(job);

            Assert.AreEqual(0, job.Retries);

            // incident was created
            var incident = runtimeService.CreateIncidentQuery(c=> c.Configuration==job.Id)
                .First();
            Assert.NotNull(incident);

            // manually Delete job for timer start event
            managementService.DeleteJob(job.Id);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testDoNotCreateNewIncident()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            var query = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId== processInstance.Id);
            var incident = query.FirstOrDefault();
            Assert.NotNull(incident);

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // set retries to 1 by job definition id
            managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

            // the incident still exists
            var tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);

            // execute the available job (should Assert.Fail again)
            ExecuteAvailableJobs();

            // the incident still exists and there
            // should be not a new incident
            Assert.AreEqual(1, query.Count());
            tmp = query.First();
            Assert.AreEqual(incident.Id, tmp.Id);
        }

        [Test]
        [Deployment]
        public virtual void testIncidentUpdateAfterCompaction()
        {
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            ExecuteAvailableJobs();

            var incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);
            Assert.AreNotSame(processInstanceId, incident.ExecutionId);

            runtimeService.CorrelateMessage("Message");

            incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);

            // incident updated with new execution id after execution tree is compacted
            Assert.AreEqual(processInstanceId, incident.ExecutionId);
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testDoNotSetNegativeRetries()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            ExecuteAvailableJobs();

            // it exists a job with 0 retries and an incident
            var job = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(0, job.Retries);

            Assert.AreEqual(1, runtimeService.CreateIncidentQuery()
                .Count());

            // it should not be possible to set negative retries
            var jobEntity = (JobEntity) job;
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, jobEntity));

            Assert.AreEqual(0, job.Retries);

            // retries should still be 0 after execution this job again
            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }

            job = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(0, job.Retries);

            // also no new incident was created
            Assert.AreEqual(1, runtimeService.CreateIncidentQuery()
                .Count());

            // it should not be possible to set the retries to a negative number with the management service
            try
            {
                managementService.SetJobRetries(job.Id, -200);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }

            try
            {
                managementService.SetJobRetriesByJobDefinitionId(job.JobDefinitionId, -300);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly IncidentTest outerInstance;

            private readonly JobEntity jobEntity;

            public CommandAnonymousInnerClass(IncidentTest outerInstance, JobEntity jobEntity)
            {
                this.outerInstance = outerInstance;
                this.jobEntity = jobEntity;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                jobEntity.Retries = -100;
                return null;
            }
        }

        [Test]
        [Deployment]
        public virtual void testActivityIdProperty()
        {
            ExecuteAvailableJobs();

            var incident = runtimeService.CreateIncidentQuery()
                .First();

            Assert.NotNull(incident);

            Assert.NotNull(incident.ActivityId);
            Assert.AreEqual("theStart", incident.ActivityId);
            Assert.IsNull(incident.ProcessInstanceId);
            Assert.IsNull(incident.ExecutionId);
        }

        public virtual void testBoundaryEventIncidentActivityId()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .UserTask("userTask")
                .EndEvent()
                .MoveToActivity("userTask")
                .BoundaryEvent("boundaryEvent")
                .TimerWithDuration("PT5S")
                .EndEvent()
                .Done());

            // given
            runtimeService.StartProcessInstanceByKey("process");
            var timerJob = managementService.CreateJobQuery()
                .First();

            // when creating an incident
            managementService.SetJobRetries(timerJob.Id, 0);

            // then
            var incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);
            Assert.AreEqual("boundaryEvent", incident.ActivityId);
        }
    }
}