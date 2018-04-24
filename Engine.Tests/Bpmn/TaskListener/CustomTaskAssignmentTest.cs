using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.TaskListener
{
	/// <summary>
	/// 
	///  <falko.Menge@camunda.com>
	/// 
	/// </summary>
	public class CustomTaskAssignmentTest : PluggableProcessEngineTestCase
	{

        [SetUp]
        protected new internal void RunBare()// setUp()
        {
		base.RunBare();

		identityService.SaveUser(identityService.NewUser("kermit"));
		identityService.SaveUser(identityService.NewUser("fozzie"));
		identityService.SaveUser(identityService.NewUser("gonzo"));

		identityService.SaveGroup(identityService.NewGroup("management"));

		identityService.CreateMembership("kermit", "management");
	  }

        [TearDown]
        protected new internal  void TearDown()
	  {
		identityService.DeleteUser("kermit");
		identityService.DeleteUser("fozzie");
		identityService.DeleteUser("gonzo");
		identityService.DeleteGroup("management");
		base.TearDown();
	  }
        
	  public virtual void testCandidateGroupAssignment()
	  {
		runtimeService.StartProcessInstanceByKey("customTaskAssignment");
		//Assert.AreEqual(1, taskService.CreateTaskQuery().TaskCandidateGroup("management").Count());
		//Assert.AreEqual(1, taskService.CreateTaskQuery().TaskCandidateUser("kermit").Count());
		//Assert.AreEqual(0, taskService.CreateTaskQuery().TaskCandidateUser("fozzie").Count());
	  }
        
	  public virtual void testCandidateUserAssignment()
	  {
		runtimeService.StartProcessInstanceByKey("customTaskAssignment");
		//Assert.AreEqual(1, taskService.CreateTaskQuery().TaskCandidateUser("kermit").Count());
		//Assert.AreEqual(1, taskService.CreateTaskQuery().TaskCandidateUser("fozzie").Count());
		//Assert.AreEqual(0, taskService.CreateTaskQuery().TaskCandidateUser("gonzo").Count());
	  }
        
	  public virtual void testAssigneeAssignment()
	  {
		runtimeService.StartProcessInstanceByKey("setAssigneeInListener");
		Assert.NotNull(taskService.CreateTaskQuery(c=>c.Assignee == "kermit").First());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "fozzie").Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "gonzo").Count());
	  }


	  public virtual void testOverwriteExistingAssignments()
	  {
		runtimeService.StartProcessInstanceByKey("overrideAssigneeInListener");
		Assert.NotNull(taskService.CreateTaskQuery(c=>c.Assignee == "kermit").First());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "fozzie").Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "gonzo").Count());
	  }
        
	  public virtual void testOverwriteExistingAssignmentsFromVariable()
	  {
		// prepare variables
		IDictionary<string, string> assigneeMappingTable = new Dictionary<string, string>();
		assigneeMappingTable["fozzie"] = "gonzo";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["assigneeMappingTable"] = assigneeMappingTable;

		// start process instance
		runtimeService.StartProcessInstanceByKey("customTaskAssignment", variables);

		// check task lists
		Assert.NotNull(taskService.CreateTaskQuery(c=>c.Assignee == "gonzo").First());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "fozzie").Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.Assignee == "kermit").Count());
	  }
        
	  public virtual void testReleaseTask()
	  {
		runtimeService.StartProcessInstanceByKey("releaseTaskProcess");

		ITask task = taskService.CreateTaskQuery(c=>c.Assignee == "fozzie").First();
		Assert.NotNull(task);
		string taskId = task.Id;

		// Set assignee to null
		taskService.SetAssignee(taskId, null);

		task = taskService.CreateTaskQuery(c=>c.Assignee == "fozzie").First();
		Assert.IsNull(task);

		task = taskService.CreateTaskQuery(c=>c.Id == taskId).First();
		Assert.NotNull(task);
		Assert.IsNull(task.Assignee);
	  }

	}

}