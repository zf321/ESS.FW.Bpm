using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ActivateJobTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testActivationById_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobById(null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testMultipleActivationByProcessDefinitionKey_shouldActivateJob()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, true);

            // when
            // the job will be suspended
            managementService.ActivateJobByProcessDefinitionKey(key);

            // then
            // the job should be activated
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivateByProcessInstanceId_shouldActivateJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            var processInstance = runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the job definition
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activate
            managementService.ActivateJobByProcessInstanceId(processInstance.Id);

            // then
            // the job should be suspended
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(job.Id, suspendedJob.Id);
            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            Assert.IsFalse(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldActivateJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activated
            managementService.ActivateJobById(job.Id);

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(job.Id, activeJob.Id);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activated
            managementService.UpdateJobSuspensionState()
                .ByJobId(job.Id)
                .Activate();

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByJobDefinitionId_shouldActivateJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the job definition
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activated
            managementService.ActivateJobByJobDefinitionId(jobDefinition.Id);

            // then
            // the job should be activated
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(job.Id, activeJob.Id);
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByJobDefinitionIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // the job will be activated
            managementService.UpdateJobSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .Activate();

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionId_shouldActivateJob()
        {
            // given
            // a deployed process definition
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activated
            managementService.ActivateJobByProcessDefinitionId(processDefinition.Id);

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(job.Id, activeJob.Id);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when
            // the job will be activated
            managementService.UpdateJobSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Activate();

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldActivateJob()
        {
            // given
            // a deployed process definition
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.True(job.Suspended);

            // when
            // the job will be activated
            managementService.ActivateJobByProcessDefinitionKey(processDefinition.Key);

            // then
            // the job should be suspended
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(job.Id, activeJob.Id);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKeyUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // when
            // the job will be activated
            managementService.UpdateJobSuspensionState()
                .ByProcessDefinitionKey("suspensionProcess")
                .Activate();

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessInstanceIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            var processInstance = runtimeService.StartProcessInstanceByKey("suspensionProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("Assert.Fail", true));

            // suspended job definitions and corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // when
            // the job will be activated
            managementService.UpdateJobSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Activate();

            // then
            // the job should be active
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }
    }
}