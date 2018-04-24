using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	public class MultiTenancyJobCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "exceptionInJobExecution";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal IProcessInstance processInstance;

	  protected internal IManagementService managementService;

	  protected internal IIdentityService identityService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

	  protected internal static readonly IBpmnModelInstance BPMN_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("exceptionInJobExecution").StartEvent().UserTask("aUserTask").BoundaryEvent("timerEvent").TimerWithDuration("PT4H").ServiceTask().CamundaExpression("${failing}").EndEvent().Done();

	  internal IBpmnModelInstance BPMN_NO_FAIL_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("noFail").StartEvent().UserTask("aUserTask").BoundaryEvent("timerEvent").TimerWithDuration("PT4H").EndEvent().Done();

    [SetUp]
	  public virtual void init()
	  {

		managementService = engineRule.ManagementService;

		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, BPMN_NO_FAIL_PROCESS);

		processInstance = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);
	  }

	  // set jobRetries
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithAuthenticatedTenant()
	   [Test]   public virtual void testSetJobRetriesWithAuthenticatedTenant()
	  {

		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.SetJobRetries(timerJob.Id, 5);

		Assert.AreEqual(5, managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First().Retries);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetJobRetriesWithNoAuthenticatedTenant()
	  {

		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");
		managementService.SetJobRetries(timerJob.Id, 5);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithDisabledTenantCheck()
	   [Test]   public virtual void testSetJobRetriesWithDisabledTenantCheck()
	  {

		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.SetJobRetries(timerJob.Id, 5);

		// then
		Assert.AreEqual(5, managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First().Retries);

	  }

	  // set Jobretries based on job definition
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithAuthenticatedTenant()
	   [Test]   public virtual void testSetJobRetriesDefinitionWithAuthenticatedTenant()
	  {

		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.SetJobRetries(jobId, 0);

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		// sets the retries for failed jobs - That's the reason why job retries are made 0 in the above step
		managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

		// then
		Assert.AreEqual(1, selectJobByProcessInstanceId(processInstance.Id).Retries);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetJobRetriesDefinitionWithNoAuthenticatedTenant()
	  {

		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.SetJobRetries(jobId, 0);
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");
		// when
		managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithDisabledTenantCheck()
	   [Test]   public virtual void testSetJobRetriesDefinitionWithDisabledTenantCheck()
	  {

		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.SetJobRetries(jobId, 0);
		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 1);
		// then
		Assert.AreEqual(1, selectJobByProcessInstanceId(processInstance.Id).Retries);

	  }

	  // set JobDueDate
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithAuthenticatedTenant()
	   [Test]   public virtual void testSetJobDueDateWithAuthenticatedTenant()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.Duedate <DateTime.Now).Count());

		//DateTime cal = new DateTime();
		var cal = DateTime.Now;
		cal.AddDays(-3);

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.SetJobDuedate(timerJob.Id, cal);

		// then
		Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Duedate <DateTime.Now).Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetJobDueDateWithNoAuthenticatedTenant()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");
		// when
		managementService.SetJobDuedate(timerJob.Id, DateTime.Now);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithDisabledTenantCheck()
	   [Test]   public virtual void testSetJobDueDateWithDisabledTenantCheck()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
            
		var cal =DateTime.Now;
		cal.AddDays(-3);

		managementService.SetJobDuedate(timerJob.Id, cal);
		// then
		Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Duedate <DateTime.Now).Count());

	  }

	  // set jobPriority test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithAuthenticatedTenant()
	   [Test]   public virtual void testSetJobPriorityWithAuthenticatedTenant()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.SetJobPriority(timerJob.Id, 5);

		// then
		Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Priority >=5).Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetJobPriorityWithNoAuthenticatedTenant()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");

		// when
		managementService.SetJobPriority(timerJob.Id, 5);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithDisabledTenantCheck()
	   [Test]   public virtual void testSetJobPriorityWithDisabledTenantCheck()
	  {
		IJob timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.SetJobPriority(timerJob.Id, 5);
		// then
		Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Priority >=5).Count());
	  }

	  // setOverridingJobPriorityForJobDefinition without cascade
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithAuthenticatedTenant()
	   [Test]   public virtual void testSetOverridingJobPriorityWithAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();
		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, Is.EqualTo(1701L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetOverridingJobPriorityWithNoAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");
		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithDisabledTenantCheck()
	   [Test]   public virtual void testSetOverridingJobPriorityWithDisabledTenantCheck()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);
		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, Is.EqualTo(1701L));
	  }

	  // setOverridingJobPriority with cascade
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndAuthenticatedTenant()
	   [Test]   public virtual void testSetOverridingJobPriorityWithCascadeAndAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();
		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);

		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, Is.EqualTo(1701L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndNoAuthenticatedTenant()
	   [Test]   public virtual void testSetOverridingJobPriorityWithCascadeAndNoAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndDisabledTenantCheck()
	   [Test]   public virtual void testSetOverridingJobPriorityWithCascadeAndDisabledTenantCheck()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);
		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, Is.EqualTo(1701L));
	  }

	  // clearOverridingJobPriorityForJobDefinition
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithAuthenticatedTenant()
	   [Test]   public virtual void testClearOverridingJobPriorityWithAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, Is.EqualTo(1701L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, null);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithNoAuthenticatedTenant()
	   [Test]   public virtual void testClearOverridingJobPriorityWithNoAuthenticatedTenant()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithDisabledTenantCheck()
	   [Test]   public virtual void testClearOverridingJobPriorityWithDisabledTenantCheck()
	  {
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().ToList().First();

		managementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		managementService.ClearOverridingJobPriorityForJobDefinition(jobDefinition.Id);
		// then
		Assert.That(managementService.CreateJobDefinitionQuery(c=>c.Id==jobDefinition.Id).First().OverridingJobPriority, null);
	  }

	  // getJobExceptionStackTrace
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithAuthenticatedTenant()
	   [Test]   public virtual void testGetJobExceptionStackTraceWithAuthenticatedTenant()
	  {

		string ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.ExecuteAvailableJobs();

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		Assert.That(managementService.GetJobExceptionStacktrace(timerJobId)!=null);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithNoAuthenticatedTenant()
	   [Test]   public virtual void testGetJobExceptionStackTraceWithNoAuthenticatedTenant()
	  {

		string ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.ExecuteAvailableJobs();

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the job '" + timerJobId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.GetJobExceptionStacktrace(timerJobId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithDisabledTenantCheck()
	   [Test]   public virtual void testGetJobExceptionStackTraceWithDisabledTenantCheck()
	  {

		string ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.ExecuteAvailableJobs();

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		managementService.GetJobExceptionStacktrace(timerJobId);
		Assert.That(managementService.GetJobExceptionStacktrace(timerJobId)!=null);
	  }

	  // deleteJobs
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithAuthenticatedTenant()
	   [Test]   public virtual void testDeleteJobWithAuthenticatedTenant()
	  {
		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.DeleteJob(timerJobId);

		// then
		Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithNoAuthenticatedTenant()
	   [Test]   public virtual void testDeleteJobWithNoAuthenticatedTenant()
	  {
		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the job '" + timerJobId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.DeleteJob(timerJobId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithDisabledTenantCheck()
	   [Test]   public virtual void testDeleteJobWithDisabledTenantCheck()
	  {
		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.DeleteJob(timerJobId);

		// then
		Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
	  }

	  //executeJobs
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithAuthenticatedTenant()
	   [Test]   public virtual void testExecuteJobWithAuthenticatedTenant()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey("noFail").Id;

		IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==noFailProcessInstanceId);

		Assert.AreEqual(1, taskQuery.Count());

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == noFailProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		managementService.ExecuteJob(timerJobId);

		// then
		Assert.AreEqual(0, taskQuery.Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithNoAuthenticatedTenant()
	   [Test]   public virtual void testExecuteJobWithNoAuthenticatedTenant()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey("noFail").Id;

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == noFailProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the job '" + timerJobId + "' because it belongs to no authenticated tenant.");
		managementService.ExecuteJob(timerJobId);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithDisabledTenantCheck()
	   [Test]   public virtual void testExecuteJobWithDisabledTenantCheck()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey("noFail").Id;

		string timerJobId = managementService.CreateJobQuery(c=>c.ProcessInstanceId == noFailProcessInstanceId).First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		managementService.ExecuteJob(timerJobId);

		IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==noFailProcessInstanceId);

		// then
		Assert.AreEqual(0, taskQuery.Count());
	  }

	  protected internal virtual IJob selectJobByProcessInstanceId(string ProcessInstanceId)
	  {
		IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
		return job;
	  }
	}

}