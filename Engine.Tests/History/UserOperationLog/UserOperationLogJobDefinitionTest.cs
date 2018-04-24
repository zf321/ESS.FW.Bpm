using System;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogJobDefinitionTest : AbstractUserOperationLogTest
	{

        [Test][Deployment("resources/history/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetOverridingPriority()
	  {
		// given a job definition
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// when I set a job priority
		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// then an op log entry is written
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery().First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.JobDefinition, userOperationLogEntry.EntityType);
		Assert.AreEqual(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeSetPriority, userOperationLogEntry.OperationType);

		Assert.AreEqual("overridingPriority", userOperationLogEntry.Property);
		Assert.AreEqual("42", userOperationLogEntry.NewValue);
		Assert.AreEqual(null, userOperationLogEntry.OrgValue);

		Assert.AreEqual(USER_ID, userOperationLogEntry.UserId);

		Assert.AreEqual(jobDefinition.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		Assert.AreEqual(jobDefinition.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		Assert.AreEqual(DeploymentId, userOperationLogEntry.DeploymentId);
	  }

        [Test]
        [Deployment("resources/history/asyncTaskProcess.bpmn20.xml")]
        public virtual void testOverwriteOverridingPriority()
	  {
		// given a job definition
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// with an overriding priority
	      ClockUtil.CurrentTime = DateTime.Now;// new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis());
		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// when I overwrite that priority
		ClockUtil.CurrentTime = DateTime.Now;// new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis() + 10000);
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 43);

		// then this is accessible via the op log
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery()/*.OrderByTimestamp()*//*.Desc()*//*.ListPage(0, 1)*/.First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.JobDefinition, userOperationLogEntry.EntityType);
		Assert.AreEqual(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeSetPriority, userOperationLogEntry.OperationType);

		Assert.AreEqual("overridingPriority", userOperationLogEntry.Property);
		Assert.AreEqual("43", userOperationLogEntry.NewValue);
		Assert.AreEqual("42", userOperationLogEntry.OrgValue);
	  }

        [Test]
        [Deployment("resources/history/asyncTaskProcess.bpmn20.xml")]
        public virtual void testClearOverridingPriority()
	  {
		// given a job definition
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// with an overriding priority
		ClockUtil.CurrentTime = DateTime.Now;// new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis());
            managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// when I clear that priority
		ClockUtil.CurrentTime = DateTime.Now;// new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis() + 10000);
            managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then this is accessible via the op log
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery()/*.OrderByTimestamp()*//*.Desc()*//*.ListPage(0, 1)*/.First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.JobDefinition, userOperationLogEntry.EntityType);
		Assert.AreEqual(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeSetPriority, userOperationLogEntry.OperationType);

		Assert.AreEqual("overridingPriority", userOperationLogEntry.Property);
		Assert.IsNull(userOperationLogEntry.NewValue);
		Assert.AreEqual("42", userOperationLogEntry.OrgValue);

		Assert.AreEqual(USER_ID, userOperationLogEntry.UserId);

		Assert.AreEqual(jobDefinition.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		Assert.AreEqual(jobDefinition.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		Assert.AreEqual(DeploymentId, userOperationLogEntry.DeploymentId);
	  }

        [Test]
        [Deployment("resources/history/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetOverridingPriorityCascadeToJobs()
	  {
		// given a job definition and job
		runtimeService.StartProcessInstanceByKey("asyncTaskProcess");
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();
		IJob job = managementService.CreateJobQuery().First();

		// when I set an overriding priority with cascade=true
		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42, true);

		// then there are two op log entries
		Assert.AreEqual(2, historyService.CreateUserOperationLogQuery().Count());

		// (1): One for the job definition priority
		IUserOperationLogEntry jobDefOpLogEntry = historyService.CreateUserOperationLogQuery(c=>c.EntityType== EntityTypes.JobDefinition).First();
		Assert.NotNull(jobDefOpLogEntry);

		// (2): and another one for the job priorities
		IUserOperationLogEntry jobOpLogEntry = historyService.CreateUserOperationLogQuery(c=>c.EntityType== EntityTypes.Job).First();
		Assert.NotNull(jobOpLogEntry);

		Assert.AreEqual("both entries should be part of the same operation", jobDefOpLogEntry.OperationId, jobOpLogEntry.OperationId);

		Assert.AreEqual(EntityTypes.Job, jobOpLogEntry.EntityType);
		Assert.IsNull("id should null because it is a bulk update operation", jobOpLogEntry.JobId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeSetPriority, jobOpLogEntry.OperationType);

		Assert.AreEqual("priority", jobOpLogEntry.Property);
		Assert.AreEqual("42", jobOpLogEntry.NewValue);
		Assert.IsNull("Original Value should be null because it is not known for bulk operations", jobOpLogEntry.OrgValue);

		Assert.AreEqual(USER_ID, jobOpLogEntry.UserId);

		// these properties should be there to narrow down the bulk update (like a SQL WHERE clasue)
		Assert.AreEqual(job.JobDefinitionId, jobOpLogEntry.JobDefinitionId);
		Assert.IsNull("an unspecified set of process instances was affected by the operation", jobOpLogEntry.ProcessInstanceId);
		Assert.AreEqual(job.ProcessDefinitionId, jobOpLogEntry.ProcessDefinitionId);
		Assert.AreEqual(job.ProcessDefinitionKey, jobOpLogEntry.ProcessDefinitionKey);
		Assert.AreEqual(DeploymentId, jobOpLogEntry.DeploymentId);
	  }

	}

}