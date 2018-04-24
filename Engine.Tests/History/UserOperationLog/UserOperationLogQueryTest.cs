//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Mail;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.JobExecutor;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.History.UserOperationLog
//{


//    /// <summary>
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class UserOperationLogQueryTest : AbstractUserOperationLogTest
//	{

//	  protected internal const string OneTaskProcess = "resources/history/oneTaskProcess.bpmn20.xml";
//	  protected internal const string ONE_TASK_CASE = "resources/api/cmmn/oneTaskCase.cmmn";

//	  private IProcessInstance process;
//	  private ITask userTask;
//	  private IExecution execution;
//	  private string processTaskId;

//	  // normalize timestamps for databases which do not provide millisecond presision.
//	  private DateTime today = new DateTime((ClockUtil.CurrentTime.TimeOfDay.Ticks / 1000) * 1000);
//	  private DateTime tomorrow = new DateTime(((ClockUtil.CurrentTime.TimeOfDay.Ticks + 86400000) / 1000) * 1000);
//	  private DateTime yesterday = new DateTime(((ClockUtil.CurrentTime.TimeOfDay.Ticks - 86400000) / 1000) * 1000);

//        [TearDown]
//        protected internal void tearDown()
//	  {
//		base.TearDown();

//		if (userTask != null)
//		{
//		  historyService.DeleteHistoricTaskInstance(userTask.Id);
//		}
//	  }

//        [Test][Deployment( OneTaskProcess)]
//        public virtual void testQuery()
//	  {
//		createLogEntries();

//		// expect: all entries can be fetched
//		Assert.AreEqual(17, query().Count());

//		// entity type
//		Assert.AreEqual(11, query(c=>c.EntityType== EntityTypes.Task).Count());
//		Assert.AreEqual(4, query(c=>c.EntityType== EntityTypes.IdentityLink).Count());
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.Attachment).Count());
//		Assert.AreEqual(0, query(c=>c.EntityType== "unknown entity type").Count());

//		// operation type
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeCreate).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeSetPriority).Count());
//		Assert.AreEqual(4, query().OperationType(UserOperationLogEntryFields.OperationTypeUpdate).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeAddUserLink).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeDeleteUserLink).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeAddGroupLink).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeDeleteGroupLink).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeAddAttachment).Count());
//		Assert.AreEqual(1, query().OperationType(UserOperationLogEntryFields.OperationTypeDeleteAttachment).Count());

//		// process and execution reference
//		Assert.AreEqual(11, query(c=>c.ProcessDefinitionId ==process.ProcessDefinitionId).Count());
//		Assert.AreEqual(11, query(c=>c.ProcessInstanceId == process.Id).Count());
//		Assert.AreEqual(11, query(c=>c.Id == execution.Id).Count());

//		// task reference
//		Assert.AreEqual(11, query(c=>c.Id == processTaskId).Count());
//		Assert.AreEqual(6, query(c=>c.Id == userTask.Id).Count());

//		// IUser reference
//		Assert.AreEqual(11, query(c=>c.Id == "icke").Count()); // not includes the create operation called by the process
//		Assert.AreEqual(6, query(c=>c.Id == "er").Count());

//		// operation ID
//		IQueryable<IUserOperationLogEntry> updates = query().OperationType(UserOperationLogEntryFields.OperationTypeUpdate);
//		string updateOperationId = updates.First().OperationId;
//		Assert.AreEqual(updates.Count(), query().OperationId(updateOperationId).Count());

//		// changed properties
//		Assert.AreEqual(3, query().Property(TaskEntity.ASSIGNEE).Count());
//		Assert.AreEqual(2, query().Property(TaskEntity.OWNER).Count());

//		// ascending order results by time
//	      IList<IUserOperationLogEntry> ascLog = query()
//	          /*.OrderByTimestamp()*/
//	          /*.Asc()*/
//	          .ToList();
//		for (int i = 0; i < 4; i++)
//		{
//		  Assert.True(yesterday.Ticks <= ascLog[i].Timestamp.TimeOfDay.Ticks);
//		}
//		for (int i = 4; i < 12; i++)
//		{
//		  Assert.True(today.Ticks <= ascLog[i].Timestamp.TimeOfDay.Ticks);
//		}
//		for (int i = 12; i < 16; i++)
//		{
//		  Assert.True(tomorrow.Ticks <= ascLog[i].Timestamp.TimeOfDay.Ticks);
//		}

//		// descending order results by time
//	      IList<IUserOperationLogEntry> descLog = query()
//	          /*.OrderByTimestamp()*/
//	          /*.Desc()*/
//	          .ToList();
//		for (int i = 0; i < 4; i++)
//		{
//		  Assert.True(tomorrow.Ticks <= descLog[i].Timestamp.TimeOfDay.Ticks);
//		}
//		for (int i = 4; i < 11; i++)
//		{
//		  Assert.True(today.Ticks <= descLog[i].Timestamp.TimeOfDay.Ticks);
//		}
//		for (int i = 11; i < 15; i++)
//		{
//		  Assert.True(yesterday.Ticks <= descLog[i].Timestamp.TimeOfDay.Ticks);
//		}

//		// filter by time, created yesterday
//		Assert.AreEqual(4, query().BeforeTimestamp(today).Count());
//		// filter by time, created today and before
//		Assert.AreEqual(12, query().BeforeTimestamp(tomorrow).Count());
//		// filter by time, created today and later
//		Assert.AreEqual(13, query().AfterTimestamp(yesterday).Count());
//		// filter by time, created tomorrow
//		Assert.AreEqual(5, query().AfterTimestamp(today).Count());
//		Assert.AreEqual(0, query().AfterTimestamp(today).BeforeTimestamp(yesterday).Count());
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryWithBackwardCompatibility()
//	  {
//		createLogEntries();

//		// expect: all entries can be fetched
//		Assert.AreEqual(17, query().Count());

//		// entity type
//		Assert.AreEqual(11, query(c=>c.EntityType== EntityTypes.Task).Count());
//		Assert.AreEqual(4, query(c=>c.EntityType== EntityTypes.IdentityLink).Count());
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.Attachment).Count());
//		Assert.AreEqual(0, query(c=>c.EntityType== "unknown entity type").Count());
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessInstanceOperationsById()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SuspendProcessInstanceById(process.Id);
//		runtimeService.ActivateProcessInstanceById(process.Id);

//		runtimeService.DeleteProcessInstance(process.Id, "a Delete reason");

//		// then
//		Assert.AreEqual(3, query(c=>c.EntityType== EntityTypes.ProcessInstance).Count());

//		IUserOperationLogEntry deleteEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).Where(c=>c.ProcessInstanceId==process.Id).OperationType(UserOperationLogEntryFields.OperationTypeDelete).First();

//		Assert.NotNull(deleteEntry);
//		Assert.AreEqual(process.Id, deleteEntry.ProcessInstanceId);
//		Assert.NotNull(deleteEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", deleteEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, deleteEntry.DeploymentId);

//		  IUserOperationLogEntry suspendEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).Where(c=>c.ProcessInstanceId==process.Id).OperationType(UserOperationLogEntryFields.OperationTypeSuspend).First();

//		Assert.NotNull(suspendEntry);
//		Assert.AreEqual(process.Id, suspendEntry.ProcessInstanceId);
//		Assert.NotNull(suspendEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", suspendEntry.ProcessDefinitionKey);

//		Assert.AreEqual("suspensionState", suspendEntry.Property);
//		Assert.AreEqual("suspended", suspendEntry.NewValue);
//		Assert.IsNull(suspendEntry.OrgValue);

//		IUserOperationLogEntry activateEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).Where(c=>c.ProcessInstanceId==process.Id).OperationType(UserOperationLogEntryFields.OperationTypeActivate).First();

//		Assert.NotNull(activateEntry);
//		Assert.AreEqual(process.Id, activateEntry.ProcessInstanceId);
//		Assert.NotNull(activateEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", activateEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, activateEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateEntry.Property);
//		Assert.AreEqual("active", activateEntry.NewValue);
//		Assert.IsNull(activateEntry.OrgValue);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessInstanceOperationsByProcessDefinitionId()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SuspendProcessInstanceByProcessDefinitionId(process.ProcessDefinitionId);
//		runtimeService.ActivateProcessInstanceByProcessDefinitionId(process.ProcessDefinitionId);

//		// then
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.ProcessInstance).Count());

//		IUserOperationLogEntry suspendEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeSuspend).First();

//		Assert.NotNull(suspendEntry);
//		Assert.AreEqual(process.ProcessDefinitionId, suspendEntry.ProcessDefinitionId);
//		Assert.IsNull(suspendEntry.ProcessInstanceId);
//		Assert.AreEqual("oneTaskProcess", suspendEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, suspendEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendEntry.Property);
//		Assert.AreEqual("suspended", suspendEntry.NewValue);
//		Assert.IsNull(suspendEntry.OrgValue);

//		IUserOperationLogEntry activateEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeActivate).First();

//		Assert.NotNull(activateEntry);
//		Assert.IsNull(activateEntry.ProcessInstanceId);
//		Assert.AreEqual("oneTaskProcess", activateEntry.ProcessDefinitionKey);
//		Assert.AreEqual(process.ProcessDefinitionId, activateEntry.ProcessDefinitionId);
//		Assert.AreEqual(DeploymentId, activateEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateEntry.Property);
//		Assert.AreEqual("active", activateEntry.NewValue);
//		Assert.IsNull(activateEntry.OrgValue);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessInstanceOperationsByProcessDefinitionKey()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SuspendProcessInstanceByProcessDefinitionKey("oneTaskProcess");
//		runtimeService.ActivateProcessInstanceByProcessDefinitionKey("oneTaskProcess");

//		// then
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.ProcessInstance).Count());

//		IUserOperationLogEntry suspendEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).ProcessDefinitionKey("oneTaskProcess").OperationType(UserOperationLogEntryFields.OperationTypeSuspend).First();

//		Assert.NotNull(suspendEntry);
//		Assert.IsNull(suspendEntry.ProcessInstanceId);
//		Assert.IsNull(suspendEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", suspendEntry.ProcessDefinitionKey);
//		Assert.IsNull(suspendEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendEntry.Property);
//		Assert.AreEqual("suspended", suspendEntry.NewValue);
//		Assert.IsNull(suspendEntry.OrgValue);

//		IUserOperationLogEntry activateEntry = query(c=>c.EntityType== EntityTypes.ProcessInstance).ProcessDefinitionKey("oneTaskProcess").OperationType(UserOperationLogEntryFields.OperationTypeActivate).First();

//		Assert.NotNull(activateEntry);
//		Assert.IsNull(activateEntry.ProcessInstanceId);
//		Assert.IsNull(activateEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", activateEntry.ProcessDefinitionKey);
//		Assert.IsNull(activateEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateEntry.Property);
//		Assert.AreEqual("active", activateEntry.NewValue);
//		Assert.IsNull(activateEntry.OrgValue);
//	  }

//        /// <summary>
//        /// CAM-1930: add Assertions for additional op log entries here
//        /// </summary>
//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessDefinitionOperationsById()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		repositoryService.SuspendProcessDefinitionById(process.ProcessDefinitionId, true,DateTime.MaxValue);
//		repositoryService.ActivateProcessDefinitionById(process.ProcessDefinitionId, true, DateTime.MaxValue);

//		// then
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.ProcessDefinition).Count());

//		// Process Definition Suspension
//		IUserOperationLogEntry suspendDefinitionEntry = query(c=>c.EntityType== EntityTypes.ProcessDefinition).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition).First();

//		Assert.NotNull(suspendDefinitionEntry);
//		Assert.AreEqual(process.ProcessDefinitionId, suspendDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", suspendDefinitionEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, suspendDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendDefinitionEntry.Property);
//		Assert.AreEqual("suspended", suspendDefinitionEntry.NewValue);
//		Assert.IsNull(suspendDefinitionEntry.OrgValue);

//		IUserOperationLogEntry activateDefinitionEntry = query(c=>c.EntityType== EntityTypes.ProcessDefinition).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition ).First();

//		Assert.NotNull(activateDefinitionEntry);
//		Assert.AreEqual(process.ProcessDefinitionId, activateDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", activateDefinitionEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, activateDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateDefinitionEntry.Property);
//		Assert.AreEqual("active", activateDefinitionEntry.NewValue);
//		Assert.IsNull(activateDefinitionEntry.OrgValue);

//	  }

//        /// <summary>
//        /// CAM-1930: add Assertions for additional op log entries here
//        /// </summary>
//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessDefinitionOperationsByKey()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		repositoryService.SuspendProcessDefinitionByKey("oneTaskProcess", true, DateTime.MaxValue);
//		repositoryService.ActivateProcessDefinitionByKey("oneTaskProcess", true, DateTime.MaxValue);

//		// then
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.ProcessDefinition).Count());

//		IUserOperationLogEntry suspendDefinitionEntry = query(c=>c.EntityType== EntityTypes.ProcessDefinition).ProcessDefinitionKey("oneTaskProcess").OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition).First();

//		Assert.NotNull(suspendDefinitionEntry);
//		Assert.IsNull(suspendDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", suspendDefinitionEntry.ProcessDefinitionKey);
//		Assert.IsNull(suspendDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendDefinitionEntry.Property);
//		Assert.AreEqual("suspended", suspendDefinitionEntry.NewValue);
//		Assert.IsNull(suspendDefinitionEntry.OrgValue);

//		IUserOperationLogEntry activateDefinitionEntry = query(c=>c.EntityType== EntityTypes.ProcessDefinition).ProcessDefinitionKey("oneTaskProcess").OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition).First();

//		Assert.NotNull(activateDefinitionEntry);
//		Assert.IsNull(activateDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual("oneTaskProcess", activateDefinitionEntry.ProcessDefinitionKey);
//		Assert.IsNull(activateDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateDefinitionEntry.Property);
//		Assert.AreEqual("active", activateDefinitionEntry.NewValue);
//		Assert.IsNull(activateDefinitionEntry.OrgValue);
//	  }

//        [Test]
//        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
//        public virtual void testQueryJobOperations()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("process");

//		// when
//		managementService.SuspendJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
//		managementService.ActivateJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
//		managementService.SuspendJobByProcessInstanceId(process.Id);
//		managementService.ActivateJobByProcessInstanceId(process.Id);

//		// then
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.JobDefinition).Count());
//		Assert.AreEqual(2, query(c=>c.EntityType== EntityTypes.Job).Count());

//		// active job definition
//		IUserOperationLogEntry activeJobDefinitionEntry = query(c=>c.EntityType== EntityTypes.JobDefinition).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeActivateJobDefinition ).First();

//		Assert.NotNull(activeJobDefinitionEntry);
//		Assert.AreEqual(process.ProcessDefinitionId, activeJobDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual(DeploymentId, activeJobDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activeJobDefinitionEntry.Property);
//		Assert.AreEqual("active", activeJobDefinitionEntry.NewValue);
//		Assert.IsNull(activeJobDefinitionEntry.OrgValue);

//		// active job
//		IUserOperationLogEntry activateJobIdEntry = query(c=>c.EntityType== EntityTypes.Job).Where(c=>c.ProcessInstanceId==process.ProcessInstanceId).OperationType(UserOperationLogEntryFields.OperationTypeActivateJob).First();

//		Assert.NotNull(activateJobIdEntry);
//		Assert.AreEqual(process.ProcessInstanceId, activateJobIdEntry.ProcessInstanceId);
//		Assert.AreEqual(DeploymentId, activateJobIdEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", activateJobIdEntry.Property);
//		Assert.AreEqual("active", activateJobIdEntry.NewValue);
//		Assert.IsNull(activateJobIdEntry.OrgValue);

//		// suspended job definition
//		IUserOperationLogEntry suspendJobDefinitionEntry = query(c=>c.EntityType== EntityTypes.JobDefinition).ProcessDefinitionId(process.ProcessDefinitionId).OperationType(UserOperationLogEntryFields.OperationTypeSuspendJobDefinition).First();

//		Assert.NotNull(suspendJobDefinitionEntry);
//		Assert.AreEqual(process.ProcessDefinitionId, suspendJobDefinitionEntry.ProcessDefinitionId);
//		Assert.AreEqual(DeploymentId, suspendJobDefinitionEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendJobDefinitionEntry.Property);
//		Assert.AreEqual("suspended", suspendJobDefinitionEntry.NewValue);
//		Assert.IsNull(suspendJobDefinitionEntry.OrgValue);

//		// suspended job
//		IUserOperationLogEntry suspendedJobEntry = query(c=>c.EntityType== EntityTypes.Job).Where(c=>c.ProcessInstanceId==process.ProcessInstanceId).OperationType(UserOperationLogEntryFields.OperationTypeSuspendJob).First();

//		Assert.NotNull(suspendedJobEntry);
//		Assert.AreEqual(process.ProcessInstanceId, suspendedJobEntry.ProcessInstanceId);
//		Assert.AreEqual(DeploymentId, suspendedJobEntry.DeploymentId);

//		Assert.AreEqual("suspensionState", suspendedJobEntry.Property);
//		Assert.AreEqual("suspended", suspendedJobEntry.NewValue);
//		Assert.IsNull(suspendedJobEntry.OrgValue);
//	  }
//        [Test][Deployment(   "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedServiceTask.bpmn20.xml" ) ]
//	  public virtual void testQueryJobRetryOperationsById()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("failedServiceTask");
//		IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == process.ProcessInstanceId).First();

//		managementService.SetJobRetries(job.Id, 10);

//		// then
//		Assert.AreEqual(1, query(c=>c.EntityType== EntityTypes.Job).OperationType(UserOperationLogEntryFields.OperationTypeSetJobRetries).Count());

//		IUserOperationLogEntry jobRetryEntry = query(c=>c.EntityType== EntityTypes.Job).JobId(job.Id).OperationType(UserOperationLogEntryFields.OperationTypeSetJobRetries).First();

//		Assert.NotNull(jobRetryEntry);
//		Assert.AreEqual(job.Id, jobRetryEntry.JobId);

//		Assert.AreEqual("3", jobRetryEntry.OrgValue);
//		Assert.AreEqual("10", jobRetryEntry.NewValue);
//		Assert.AreEqual("retries", jobRetryEntry.Property);
//		Assert.AreEqual(job.JobDefinitionId, jobRetryEntry.JobDefinitionId);
//		Assert.AreEqual(job.ProcessInstanceId, jobRetryEntry.ProcessInstanceId);
//		Assert.AreEqual(job.ProcessDefinitionKey, jobRetryEntry.ProcessDefinitionKey);
//		Assert.AreEqual(job.ProcessDefinitionId, jobRetryEntry.ProcessDefinitionId);
//		Assert.AreEqual(DeploymentId, jobRetryEntry.DeploymentId);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryJobDefinitionOperationWithDelayedJobDefinition()
//	  {
//		// given
//		// a running process instance
//		IProcessInstance process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// with a process definition id
//		string processDefinitionId = process.ProcessDefinitionId;

//		// ..Which will be suspended with the corresponding jobs
//		managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

//		// one week from now
//		ClockUtil.CurrentTime = today;
//		long oneWeekFromStartTime = today.Ticks + (7 * 24 * 60 * 60 * 1000);

//		// when
//		// activate the job definition
//		managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, false, new DateTime(oneWeekFromStartTime));

//		// then
//		// there is a IUser log entry for the activation
//		long? jobDefinitionEntryCount = query(c=>c.EntityType== EntityTypes.JobDefinition).OperationType(UserOperationLogEntryFields.OperationTypeActivateJobDefinition).ProcessDefinitionId(processDefinitionId).Count();

//		Assert.AreEqual(1, jobDefinitionEntryCount.Value);

//		// there exists a job for the delayed activation execution
//		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

//		IJob delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).First();
//		Assert.NotNull(delayedActivationJob);

//		// execute job
//		managementService.ExecuteJob(delayedActivationJob.Id);

//		jobDefinitionEntryCount = query(c=>c.EntityType== EntityTypes.JobDefinition).OperationType(UserOperationLogEntryFields.OperationTypeActivateJobDefinition).ProcessDefinitionId(processDefinitionId).Count();

//		Assert.AreEqual(1, jobDefinitionEntryCount.Value);

//		// Clean up db
//		ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
//		commandExecutor.Execute(new CommandAnonymousInnerClass(this));
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<object>
//	  {
//		  private readonly UserOperationLogQueryTest outerInstance;

//		  public CommandAnonymousInnerClass(UserOperationLogQueryTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerActivateJobDefinitionHandler.TYPE);
//			return null;
//		  }
//	  }
//        [Test][Deployment( "resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn") ]
//	  public virtual void testQueryProcessDefinitionOperationWithDelayedProcessDefinition()
//	  {
//		// given
//		ClockUtil.CurrentTime = today;
//		const long hourInMs = 60 * 60 * 1000;

//		string key = "oneFailingServiceTaskProcess";

//		// a running process instance with a failed service task
//		IDictionary<string, object> @params = new Dictionary<string, object>();
//		@params["Assert.Fail"] = true;
//		runtimeService.StartProcessInstanceByKey(key, @params);

//		// when
//		// the process definition will be suspended
//		repositoryService.SuspendProcessDefinitionByKey(key, false, new DateTime(today.Ticks + (2 * hourInMs)));

//		// then
//		// there exists a timer job to suspend the process definition delayed
//		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery()/*.Timers()*/.First();
//		Assert.NotNull(timerToSuspendProcessDefinition);

//		// there is a IUser log entry for the activation
//		long? processDefinitionEntryCount = query(c=>c.EntityType== EntityTypes.ProcessDefinition).OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition).ProcessDefinitionKey(key).Count();

//		Assert.AreEqual(1, processDefinitionEntryCount.Value);

//		// when
//		// execute job
//		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

//		// then
//		// there is a IUser log entry for the activation
//		processDefinitionEntryCount = query(c=>c.EntityType== EntityTypes.ProcessDefinition).OperationType(UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition).ProcessDefinitionKey(key).Count();

//		Assert.AreEqual(1, processDefinitionEntryCount.Value);

//		// clean up op log
//		ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
//		commandExecutor.Execute(new CommandAnonymousInnerClass2(this));
//	  }

//	  private class CommandAnonymousInnerClass2 : ICommand<object>
//	  {
//		  private readonly UserOperationLogQueryTest outerInstance;

//		  public CommandAnonymousInnerClass2(UserOperationLogQueryTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
//			return null;
//		  }
//	  }

//        // ----- PROCESS INSTANCE MODIFICATION -----

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryProcessInstanceModificationOperation()
//	  {
//		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		string ProcessInstanceId = processInstance.Id;

//		IProcessDefinition definition = repositoryService.CreateProcessDefinitionQuery().First();

//		runtimeService.CreateProcessInstanceModification(processInstance.Id).StartBeforeActivity("theTask").Execute();

//		IQueryable<IUserOperationLogEntry> logQuery = query(c=>c.EntityType== EntityTypes.ProcessInstance).OperationType(UserOperationLogEntryFields.OperationTypeModifyProcessInstance);

//		Assert.AreEqual(1, logQuery.Count());
//		IUserOperationLogEntry logEntry = logQuery.First();

//		Assert.AreEqual(ProcessInstanceId, logEntry.ProcessInstanceId);
//		Assert.AreEqual(processInstance.ProcessDefinitionId, logEntry.ProcessDefinitionId);
//		Assert.AreEqual(definition.Key, logEntry.ProcessDefinitionKey);
//		Assert.AreEqual(DeploymentId, logEntry.DeploymentId);
//		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeModifyProcessInstance, logEntry.OperationType);
//		Assert.AreEqual(EntityTypes.ProcessInstance, logEntry.EntityType);
//		Assert.IsNull(logEntry.Property);
//		Assert.IsNull(logEntry.OrgValue);
//		Assert.IsNull(logEntry.NewValue);
//	  }

//        // ----- ADD VARIABLES -----

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddExecutionVariableOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SetVariable(process.Id, "testVariable1", "THIS IS TESTVARIABLE!!!");

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeSetVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddExecutionVariablesMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SetVariables(process.Id, createMapForVariableAddition());

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeSetVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddExecutionVariablesSingleAndMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.SetVariable(process.Id, "testVariable3", "foo");
//		runtimeService.SetVariables(process.Id, createMapForVariableAddition());
//		runtimeService.SetVariable(process.Id, "testVariable4", "bar");

//		// then
//		verifyVariableOperationAsserts(3, UserOperationLogEntryFields.OperationTypeSetVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddTaskVariableOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.SetVariable(processTaskId, "testVariable1", "THIS IS TESTVARIABLE!!!");

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeSetVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddTaskVariablesMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.SetVariables(processTaskId, createMapForVariableAddition());

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeSetVariable);
//        }
//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryAddTaskVariablesSingleAndMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.SetVariable(processTaskId, "testVariable3", "foo");
//		taskService.SetVariables(processTaskId, createMapForVariableAddition());
//		taskService.SetVariable(processTaskId, "testVariable4", "bar");

//		// then
//		verifyVariableOperationAsserts(3, UserOperationLogEntryFields.OperationTypeSetVariable);
//	  }

//        // ----- PATCH VARIABLES -----

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryPatchExecutionVariablesOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		((RuntimeServiceImpl) runtimeService).UpdateVariables(process.Id, createMapForVariableAddition(), createCollectionForVariableDeletion());

//		// then
//	   verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeModifyVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryPatchTaskVariablesOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		((TaskServiceImpl) taskService).UpdateVariablesLocal(processTaskId, createMapForVariableAddition(), createCollectionForVariableDeletion());

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeModifyVariable);
//	  }

//        // ----- REMOVE VARIABLES -----

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveExecutionVariableOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.RemoveVariable(process.Id, "testVariable1");

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveExecutionVariablesMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.RemoveVariables(process.Id, createCollectionForVariableDeletion());

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveExecutionVariablesSingleAndMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		// when
//		runtimeService.RemoveVariable(process.Id, "testVariable1");
//		runtimeService.RemoveVariables(process.Id, createCollectionForVariableDeletion());
//		runtimeService.RemoveVariable(process.Id, "testVariable2");

//		// then
//		verifyVariableOperationAsserts(3, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//        }
//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveTaskVariableOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.RemoveVariable(processTaskId, "testVariable1");

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveTaskVariablesMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.RemoveVariables(processTaskId, createCollectionForVariableDeletion());

//		// then
//		verifyVariableOperationAsserts(1, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//	  }

//        [Test]
//        [Deployment(OneTaskProcess)]
//        public virtual void testQueryRemoveTaskVariablesSingleAndMapOperation()
//	  {
//		// given
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// when
//		taskService.RemoveVariable(processTaskId, "testVariable3");
//		taskService.RemoveVariables(processTaskId, createCollectionForVariableDeletion());
//		taskService.RemoveVariable(processTaskId, "testVariable4");

//		// then
//		verifyVariableOperationAsserts(3, UserOperationLogEntryFields.OperationTypeRemoveVariable);
//	  }

//	  // --------------- CMMN --------------------
//      [Test][Deployment(ONE_TASK_CASE) ]
//	  public virtual void testQueryByCaseDefinitionId()
//	  {
//		// given:
//		// a deployed case definition
//		//string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;

//		// an active case instance
//		//caseService.WithCaseDefinition(caseDefinitionId).Create();

//		ITask task = taskService.CreateTaskQuery().First();

//		Assert.NotNull(task);

//		// when
//		taskService.SetAssignee(task.Id, "demo");

//		// then

//		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.CaseDefinitionId== caseDefinitionId);

//		//verifyQueryResults(query, 1);
//	  }
//        [Test]
//        [Deployment(ONE_TASK_CASE)]
//        public virtual void testQueryByCaseInstanceId()
//	  {
//		// given:
//		// a deployed case definition
//		string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;

//		// an active case instance
//		string caseInstanceId = caseService.WithCaseDefinition(caseDefinitionId).Create().Id;

//		ITask task = taskService.CreateTaskQuery().First();

//		Assert.NotNull(task);

//		// when
//		taskService.SetAssignee(task.Id, "demo");

//		// then

//		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.CaseInstanceId ==caseInstanceId);

//		//verifyQueryResults(query, 1);
//	  }
//        [Test]
//        [Deployment(ONE_TASK_CASE)]
//        public virtual void testQueryByCaseExecutionId()
//	  {
//		// given:
//		// a deployed case definition
//		//string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;

//		// an active case instance
//		//caseService.WithCaseDefinition(caseDefinitionId).Create();

//		//string caseExecutionId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;

//		ITask task = taskService.CreateTaskQuery().First();

//		Assert.NotNull(task);

//		// when
//		taskService.SetAssignee(task.Id, "demo");

//		// then

//		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery().CaseExecutionId(caseExecutionId);

//		//verifyQueryResults(query, 1);
//	  }

//        [Test]
//        public virtual void testQueryByDeploymentId()
//	  {
//		// given
//		string deploymentId = repositoryService.CreateDeployment().AddClasspathResource(OneTaskProcess).Deploy().Id;

//		// when
//		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=> c.DeploymentId == DeploymentId);

//		// then
//		//verifyQueryResults(query, 1);

//		repositoryService.DeleteDeployment(DeploymentId, true);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidDeploymentId()
//	  {
//		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=> c.DeploymentId == "invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DeploymentId(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // expected
//		}
//	  }

//	  private void verifyQueryResults(IQueryable<IUserOperationLogEntry> query, int countExpected)
//	  {
//		Assert.AreEqual(countExpected, query.Count());
//		Assert.AreEqual(countExpected, query.Count());

//		if (countExpected == 1)
//		{
//		  Assert.NotNull(query.First());
//		}
//		else if (countExpected > 1)
//		{
//		  verifySingleResultFails(query);
//		}
//		else if (countExpected == 0)
//		{
//		  Assert.IsNull(query.First());
//		}
//	  }

//	  private void verifySingleResultFails(IQueryable<IUserOperationLogEntry> query)
//	  {
//		try
//		{
//		  query.First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	  private IDictionary<string, object> createMapForVariableAddition()
//	  {
//		IDictionary<string, object> variables = new Dictionary<string, object>();
//		variables["testVariable1"] = "THIS IS TESTVARIABLE!!!";
//		variables["testVariable2"] = "OVER 9000!";

//		return variables;
//	  }

//	  private ICollection<string> createCollectionForVariableDeletion()
//	  {
//		ICollection<string> variables = new List<string>();
//		variables.Add("testVariable3");
//		variables.Add("testVariable4");

//		return variables;
//	  }

//	  private void verifyVariableOperationAsserts(int countAssertValue, string operationType)
//	  {
//		IQueryable<IUserOperationLogEntry> logQuery = query(c=>c.EntityType== EntityTypes.Variable).OperationType(operationType);
//		Assert.AreEqual(countAssertValue, logQuery.Count());

//		if (countAssertValue > 1)
//		{
//		    IList<IUserOperationLogEntry> logEntryList = logQuery.ToList();

//		  foreach (IUserOperationLogEntry logEntry in logEntryList)
//		  {
//			Assert.AreEqual(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
//			Assert.AreEqual(process.ProcessInstanceId, logEntry.ProcessInstanceId);
//			Assert.AreEqual(DeploymentId, logEntry.DeploymentId);
//		  }
//		}
//		else
//		{
//		  IUserOperationLogEntry logEntry = logQuery.First();
//		  Assert.AreEqual(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
//		  Assert.AreEqual(process.ProcessInstanceId, logEntry.ProcessInstanceId);
//		  Assert.AreEqual(DeploymentId, logEntry.DeploymentId);
//		}
//	  }

//	  private IQueryable<IUserOperationLogEntry> query()
//	  {
//		return historyService.CreateUserOperationLogQuery();
//	  }

//	  /// <summary>
//	  /// start process and operate on userTask to create some log entries for the query tests
//	  /// </summary>
//	  private void createLogEntries()
//	  {
//		ClockUtil.CurrentTime = yesterday;

//		// create a process with a userTask and work with it
//		process = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		execution = ProcessEngine.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==process.Id).First();
//		processTaskId = taskService.CreateTaskQuery().First().Id;

//		// IUser "icke" works on the process userTask
//		identityService.AuthenticatedUserId = "icke";

//		// create and remove some links
//		taskService.AddCandidateUser(processTaskId, "er");
//		taskService.DeleteCandidateUser(processTaskId, "er");
//		taskService.AddCandidateGroup(processTaskId, "wir");
//		taskService.DeleteCandidateGroup(processTaskId, "wir");

//		// assign and reassign the userTask
//		ClockUtil.CurrentTime = today;
//		taskService.SetOwner(processTaskId, "icke");
//		taskService.Claim(processTaskId, "icke");
//		taskService.SetAssignee(processTaskId, "er");

//		// change priority of task
//		taskService.SetPriority(processTaskId, 10);

//		// add and Delete an attachment
//		IAttachment attachment = taskService.CreateAttachment("image/ico", processTaskId, process.Id, "favicon.ico", "favicon", "http://camunda.com/favicon.ico");
//		taskService.DeleteAttachment(attachment.Id);

//		// complete the userTask to finish the process
//		taskService.Complete(processTaskId);
//		AssertProcessEnded(process.Id);

//		// IUser "er" works on the process userTask
//		identityService.AuthenticatedUserId = "er";

//		// create a standalone userTask
//		userTask = taskService.NewTask();
//		userTask.Name = "to do";
//		taskService.SaveTask(userTask);

//		// change some properties manually to create an update event
//		ClockUtil.CurrentTime = tomorrow;
//		userTask.Description = "Desc";
//		userTask.Owner = "icke";
//		userTask.Assignee = "er";
//		userTask.DueDate = DateTime.Now;
//		taskService.SaveTask(userTask);

//		// complete the userTask
//		taskService.Complete(userTask.Id);
//	  }

//	}

//}