using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobDefinitionPriorityTest : PluggableProcessEngineTestCase
    {
        protected internal const long EXPECTED_DEFAULT_PRIORITY = 0;

        [Test][Deployment( "resources/api/mgmt/jobPrioProcess.bpmn20.xml") ]
        public virtual void testSetJobDefinitionPriorityOverridesBpmnPriority()
        {
            // given a process instance with a job with default priority and a corresponding job definition
            var instance = runtimeService.CreateProcessInstanceByKey("jobPrioProcess")
                .StartBeforeActivity("task2")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            var jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == job.JobDefinitionId)
                .First();

            // when I set the job definition's priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 62);

            // then the job definition's priority value has changed
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.AreEqual(62, (long) updatedDefinition.OverridingJobPriority);

            // the existing job's priority is still the value as given in the BPMN XML
            var updatedExistingJob = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(5, updatedExistingJob.Priority);

            // and a new job of that definition receives the updated priority
            // meaning that the updated priority overrides the priority specified in the BPMN XML
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("task2")
                .Execute();

            var newJob = getJobThatIsNot(updatedExistingJob);
            Assert.AreEqual(62, newJob.Priority);
        }

        [Test][Deployment( "resources/api/mgmt/jobPrioProcess.bpmn20.xml") ]
        public virtual void testSetJobDefinitionPriorityWithCascadeOverridesBpmnPriority()
        {
            // given a process instance with a job with default priority and a corresponding job definition
            var instance = runtimeService.CreateProcessInstanceByKey("jobPrioProcess")
                .StartBeforeActivity("task2")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            var jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == job.JobDefinitionId)
                .First();

            // when I set the job definition's priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 72, true);

            // then the job definition's priority value has changed
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.AreEqual(72, (long) updatedDefinition.OverridingJobPriority);

            // the existing job's priority has changed as well
            var updatedExistingJob = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(72, updatedExistingJob.Priority);

            // and a new job of that definition receives the updated priority
            // meaning that the updated priority overrides the priority specified in the BPMN XML
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("task2")
                .Execute();

            var newJob = getJobThatIsNot(updatedExistingJob);
            Assert.AreEqual(72, newJob.Priority);
        }

        [Test][Deployment(  "resources/api/mgmt/jobPrioProcess.bpmn20.xml") ]
        public virtual void testRedeployOverridesSetJobDefinitionPriority()
        {
            // given a process instance with a job with default priority and a corresponding job definition
            runtimeService.CreateProcessInstanceByKey("jobPrioProcess")
                .StartBeforeActivity("task2")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            var jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == job.JobDefinitionId)
                .First();

            // when I set the job definition's priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 72, true);

            // then the job definition's priority value has changed
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.AreEqual(72, (long) updatedDefinition.OverridingJobPriority);

            // the existing job's priority has changed as well
            var updatedExistingJob = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(72, updatedExistingJob.Priority);

            // if the process definition is redeployed
            var secondDeploymentId = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/mgmt/jobPrioProcess.bpmn20.xml")
                .Deploy()
                .Id;

            // then a new job will have the priority from the BPMN xml
            var secondInstance = runtimeService.CreateProcessInstanceByKey("jobPrioProcess")
                .StartBeforeActivity("task2")
                .Execute();

            var newJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== secondInstance.Id)
                .First();
            Assert.AreEqual(5, newJob.Priority);

            repositoryService.DeleteDeployment(secondDeploymentId, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/jobPrioProcess.bpmn20.xml")]
        public virtual void testGetJobDefinitionDefaultPriority()
        {
            // with a process with job definitions deployed
            // then the definitions have a default null priority, meaning that they don't override the
            // value in the BPMN XML
            var jobDefinitions = managementService.CreateJobDefinitionQuery()
                
                .ToList();
            Assert.AreEqual(4, jobDefinitions.Count);

            Assert.IsNull(jobDefinitions[0].OverridingJobPriority);
            Assert.IsNull(jobDefinitions[1].OverridingJobPriority);
            Assert.IsNull(jobDefinitions[2].OverridingJobPriority);
            Assert.IsNull(jobDefinitions[3].OverridingJobPriority);
        }

        [Test]
        public virtual void testSetNonExistingJobDefinitionPriority()
        {
            try
            {
                managementService.SetOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId", 42);
                Assert.Fail("should not succeed");
            }
            catch (NotFoundException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist",
                    e.Message);
            }

            try
            {
                managementService.SetOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId", 42, true);
                Assert.Fail("should not succeed");
            }
            catch (NotFoundException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist",
                    e.Message);
            }
        }

        [Test]
        public virtual void testResetNonExistingJobDefinitionPriority()
        {
            try
            {
                managementService.ClearOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId");
                Assert.Fail("should not succeed");
            }
            catch (NotFoundException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist",
                    e.Message);
            }
        }

        [Test]
        public virtual void testSetNullJobDefinitionPriority()
        {
            try
            {
                managementService.SetOverridingJobPriorityForJobDefinition(null, 42);
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
            }

            try
            {
                managementService.SetOverridingJobPriorityForJobDefinition(null, 42, true);
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
            }
        }

        [Test]
        public virtual void testResetNullJobDefinitionPriority()
        {
            try
            {
                managementService.ClearOverridingJobPriorityForJobDefinition(null);
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
            }
        }

        protected internal virtual IJob getJobThatIsNot(IJob other)
        {
            var jobs = managementService.CreateJobQuery()
                
                .ToList();
            Assert.AreEqual(2, jobs.Count);

            if (jobs[0].Id.Equals(other.Id))
                return jobs[1];
            if (jobs[1].Id.Equals(other.Id))
                return jobs[0];
            throw new ProcessEngineException("IJob with id " + other.Id + " does not exist anymore");
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testResetJobDefinitionPriority()
        {
            // given a job definition
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when I set a priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

            // and I reset the priority
            managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

            // then the job definition priority is still null
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.IsNull(updatedDefinition.OverridingJobPriority);

            // and a new job instance does not receive the intermittently set priority
            runtimeService.CreateProcessInstanceByKey("asyncTaskProcess")
                .StartBeforeActivity("task")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(EXPECTED_DEFAULT_PRIORITY, job.Priority);
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testResetJobDefinitionPriorityWhenPriorityIsNull()
        {
            // given a job definition with null priority
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();
            Assert.IsNull(jobDefinition.OverridingJobPriority);

            // when I set a priority
            managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

            // then the priority remains unchanged
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.IsNull(updatedDefinition.OverridingJobPriority);
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetJobDefinitionPriority()
        {
            // given a process instance with a job with default priority and a corresponding job definition
            var instance = runtimeService.CreateProcessInstanceByKey("asyncTaskProcess")
                .StartBeforeActivity("task")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            var jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == job.JobDefinitionId)
                .First();

            // when I set the job definition's priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

            // then the job definition's priority value has changed
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.AreEqual(42, (long) updatedDefinition.OverridingJobPriority);

            // the existing job's priority has not changed
            var updatedExistingJob = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(job.Priority, updatedExistingJob.Priority);

            // and a new job of that definition receives the updated priority
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("task")
                .Execute();

            var newJob = getJobThatIsNot(updatedExistingJob);
            Assert.AreEqual(42, newJob.Priority);
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetJobDefinitionPriorityToExtremeValues()
        {
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // it is possible to set the max long value
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, long.MaxValue);
            jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();
            Assert.AreEqual(long.MaxValue, (long) jobDefinition.OverridingJobPriority);

            // it is possible to set the min long value
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, long.MinValue + 1);
                // +1 for informix
            jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();
            Assert.AreEqual(long.MinValue + 1, (long) jobDefinition.OverridingJobPriority);
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetJobDefinitionPriorityWithCascade()
        {
            // given a process instance with a job with default priority and a corresponding job definition
            var instance = runtimeService.CreateProcessInstanceByKey("asyncTaskProcess")
                .StartBeforeActivity("task")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();
            var jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == job.JobDefinitionId)
                .First();

            // when I set the job definition's priority
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 52, true);

            // then the job definition's priority value has changed
            var updatedDefinition = managementService.CreateJobDefinitionQuery(c=>c.Id == jobDefinition.Id)
                .First();
            Assert.AreEqual(52, (long) updatedDefinition.OverridingJobPriority);

            // the existing job's priority has changed as well
            var updatedExistingJob = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(52, updatedExistingJob.Priority);

            // and a new job of that definition receives the updated priority
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("task")
                .Execute();

            var newJob = getJobThatIsNot(updatedExistingJob);
            Assert.AreEqual(52, newJob.Priority);
        }
    }
}