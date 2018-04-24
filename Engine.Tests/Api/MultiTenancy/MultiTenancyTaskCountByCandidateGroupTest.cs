using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    
    

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class MultiTenancyTaskCountByCandidateGroupTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskCountByCandidateGroupTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			//ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}


	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;




	  protected internal ITaskService taskService;
	  protected internal IIdentityService identityService;
	  protected internal IAuthorizationService authorizationService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string userId = "aUser";
	  protected internal string groupId = "aGroup";
	  protected internal string tenantId = "aTenant";
	  protected internal string anotherTenantId = "anotherTenant";

	  protected internal IList<string> taskIds = new List<string>();

        [SetUp]
	  public virtual void setUp()
	  {
		taskService = processEngineRule.TaskService;
		identityService = processEngineRule.IdentityService;
		authorizationService = processEngineRule.IAuthorizationService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

		createTask(groupId, tenantId);
		createTask(groupId, anotherTenantId);
		createTask(groupId, anotherTenantId);

		processEngineConfiguration.SetTenantCheckEnabled(true);
	  }

    [TearDown]
	  public virtual void cleanUp()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);

		foreach (string taskId in taskIds)
		{
		  taskService.DeleteTask(taskId, true);
		}
	  }


        [Test]
        public virtual void shouldOnlyShowTenantSpecificTasks()
	  {
		// given

		identityService.SetAuthentication(userId, null, new List<string>() { tenantId});

		// when
		IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();

		// then
		Assert.AreEqual(1, results.Count);
	  }

	  protected internal virtual void createTask(string groupId, string tenantId)
	  {
		ITask task = taskService.NewTask();
		task.TenantId = tenantId;
		taskService.SaveTask(task);

		if (!string.ReferenceEquals(groupId, null))
		{
		  taskService.AddCandidateGroup(task.Id, groupId);
		  taskIds.Add(task.Id);
		}
	  }
	}

}