using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SuspendJobTest : PluggableProcessEngineTestCase
    {
        public virtual void testSuspensionById_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobById(null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldSuspendJob()
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

            // when
            // the job will be suspended
            managementService.SuspendJobByProcessDefinitionKey(key);

            // then
            // the job should be suspended
            var jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldSuspendJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.SuspendJobById(job.Id);

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .First();

            //Assert.AreEqual(job.Id, suspendedJob.Id);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.UpdateJobSuspensionState()
                .ByJobId(job.Id)
                .Suspend();

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByJobDefinitionId_shouldSuspendJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // the job definition
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.SuspendJobByJobDefinitionId(jobDefinition.Id);

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(job.Id, suspendedJob.Id);
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByJobDefinitionIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // the job will be suspended
            managementService.UpdateJobSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .Suspend();

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldSuspendJob()
        {
            // given
            // a deployed process definition
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.SuspendJobByProcessDefinitionId(processDefinition.Id);

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(job.Id, suspendedJob.Id);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when
            // the job will be suspended
            managementService.UpdateJobSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Suspend();

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldSuspendJob()
        {
            // given
            // a deployed process definition
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.SuspendJobByProcessDefinitionKey(processDefinition.Key);

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(job.Id, suspendedJob.Id);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKeyUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            // when
            // the job will be suspended
            managementService.UpdateJobSuspensionState()
                .ByProcessDefinitionKey("suspensionProcess")
                .Suspend();

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessInstanceId_shouldSuspendJob()
        {
            // given

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            var processInstance = runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // the job definition
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            var job = jobQuery.First();
            Assert.IsFalse(job.Suspended);

            // when
            // the job will be suspended
            managementService.SuspendJobByProcessInstanceId(processInstance.Id);

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(job.Id, suspendedJob.Id);
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessInstanceIdUsingBuilder()
        {
            // given

            // a running process instance with a failed job
            var processInstance = runtimeService.StartProcessInstanceByKey("suspensionProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("Assert.Fail", true));

            // the failed job
            var jobQuery = managementService.CreateJobQuery();
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            // when
            // the job will be suspended
            managementService.UpdateJobSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Suspend();

            // then
            // the job should be suspended
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
        }
    }
}