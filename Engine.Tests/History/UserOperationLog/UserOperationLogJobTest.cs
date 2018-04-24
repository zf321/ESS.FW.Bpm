using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogJobTest : AbstractUserOperationLogTest
	{

        [Test]
        [Deployment("resources/history/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetJobPriority()
	  {
		// given a job
		runtimeService.StartProcessInstanceByKey("asyncTaskProcess");
		IJob job = managementService.CreateJobQuery().First();

		// when I set a job priority
		managementService.SetJobPriority(job.Id, 42);

		// then an op log entry is written
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery().First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.Job, userOperationLogEntry.EntityType);
		Assert.AreEqual(job.Id, userOperationLogEntry.JobId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeSetPriority, userOperationLogEntry.OperationType);

		Assert.AreEqual("priority", userOperationLogEntry.Property);
		Assert.AreEqual("42", userOperationLogEntry.NewValue);
		Assert.AreEqual("0", userOperationLogEntry.OrgValue);

		Assert.AreEqual(USER_ID, userOperationLogEntry.UserId);

		Assert.AreEqual(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		Assert.AreEqual(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		Assert.AreEqual(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		Assert.AreEqual(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		Assert.AreEqual(DeploymentId, userOperationLogEntry.DeploymentId);
	  }
	}

}