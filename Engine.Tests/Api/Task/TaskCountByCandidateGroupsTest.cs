using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class TaskCountByCandidateGroupsTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskCountByCandidateGroupsTest()
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

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  //public RuleChain ruleChain;


	  protected internal ITaskService taskService;
	  protected internal IIdentityService identityService;
	  protected internal IAuthorizationService authorizationService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string userId = "user";
	  protected internal IList<string> tasks = new List<string>();
	  protected internal IList<string> tenants =new List<string>(){"tenant1", "tenant2" };
	  protected internal IList<string> groups = new List<string>() { "aGroupId", "anotherGroupId" };


        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
	  {
		taskService = processEngineRule.TaskService;
		identityService = processEngineRule.IdentityService;
		authorizationService = processEngineRule.IAuthorizationService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

		createTask(groups[0], tenants[0]);
		createTask(groups[0], tenants[1]);
		createTask(groups[1], tenants[1]);
		createTask(null, tenants[1]);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (string taskId in tasks)
		{
		  taskService.DeleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTaskCountsByGroup()
	  public virtual void shouldReturnTaskCountsByGroup()
	  {
		// when
		IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();

		// then
		Assert.AreEqual(3, results.Count);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProvideTaskCountForEachGroup()
	  public virtual void shouldProvideTaskCountForEachGroup()
	  {
		// when
		IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();

		// then
		foreach (ITaskCountByCandidateGroupResult result in results)
		{
		  checkResultCount(result, null, 1);
		  checkResultCount(result, groups[0], 2);
		  checkResultCount(result, groups[1], 1);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProvideGroupNameForEachGroup()
	  public virtual void shouldProvideGroupNameForEachGroup()
	  {
		// when
		IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();

		// then
		foreach (ITaskCountByCandidateGroupResult result in results)
		{
		  Assert.True(checkResultName(result));
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchCountOfTasksWithoutAssignee()
	  public virtual void shouldFetchCountOfTasksWithoutAssignee()
	  {
		// given
		IUser user = identityService.NewUser(userId);
		identityService.SaveUser(user);

		// when
		taskService.DelegateTask(tasks[2], userId);
		IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();

		identityService.DeleteUser(userId);

		// then
		Assert.AreEqual(2, results.Count);
	  }

	  protected internal virtual void createTask(string groupId, string tenantId)
	  {
		ITask task = taskService.NewTask();
		task.TenantId = tenantId;
		taskService.SaveTask(task);

		if (!string.ReferenceEquals(groupId, null))
		{
		  taskService.AddCandidateGroup(task.Id, groupId);
		}

		tasks.Add(task.Id);
	  }

	  protected internal virtual void checkResultCount(ITaskCountByCandidateGroupResult result, string expectedResultName, int expectedResultCount)
	  {
		if ((string.ReferenceEquals(expectedResultName, null) && result.GroupName == null) || (result.GroupName != null && result.GroupName.Equals(expectedResultName)))
		{
		  Assert.AreEqual(expectedResultCount, result.TaskCount);
		}
	  }

	  protected internal virtual bool checkResultName(ITaskCountByCandidateGroupResult result)
	  {
		return result.GroupName == null || result.GroupName.Equals(groups[0]) || result.GroupName.Equals(groups[1]);
	  }
	}

}