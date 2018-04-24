using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{


	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class TaskVariablesTest : PluggableProcessEngineTestCase
	{

        [Test]
        public virtual void testStandaloneTaskVariables()
	  {
		ITask task = taskService.NewTask();
		task.Name = "gonzoTask";
		taskService.SaveTask(task);

		string taskId = task.Id;
		taskService.SetVariable(taskId, "instrument", "trumpet");
		Assert.AreEqual("trumpet", taskService.GetVariable(taskId, "instrument"));

		taskService.DeleteTask(taskId, true);
	  }

[Test]
	  public virtual void testTaskExecutionVariables()
	  {
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.CreateTaskQuery().First().Id;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		Assert.AreEqual(expectedVariables, runtimeService.GetVariables(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, taskService.GetVariables(taskId));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocal(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, taskService.GetVariablesLocal(taskId));

		runtimeService.SetVariable(ProcessInstanceId, "instrument", "trumpet");

		expectedVariables = new Dictionary<string, object>();
		Assert.AreEqual(expectedVariables, taskService.GetVariablesLocal(taskId));
		expectedVariables["instrument"] = "trumpet";
		Assert.AreEqual(expectedVariables, runtimeService.GetVariables(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, taskService.GetVariables(taskId));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocal(ProcessInstanceId));

		taskService.SetVariable(taskId, "player", "gonzo");

		expectedVariables = new Dictionary<string, object>();
		Assert.AreEqual(expectedVariables, taskService.GetVariablesLocal(taskId));
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		Assert.AreEqual(expectedVariables, runtimeService.GetVariables(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, taskService.GetVariables(taskId));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocal(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocal(ProcessInstanceId, null));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocalTyped(ProcessInstanceId, null, true));

		taskService.SetVariableLocal(taskId, "budget", "unlimited");

		expectedVariables = new Dictionary<string, object>();
		expectedVariables["budget"] = "unlimited";
		Assert.AreEqual(expectedVariables, taskService.GetVariablesLocal(taskId));
		Assert.AreEqual(expectedVariables, taskService.GetVariablesLocalTyped(taskId, true));
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		Assert.AreEqual(expectedVariables, taskService.GetVariables(taskId));
		Assert.AreEqual(expectedVariables, taskService.GetVariablesTyped(taskId, true));

		Assert.AreEqual(expectedVariables, taskService.GetVariables(taskId, null));
		Assert.AreEqual(expectedVariables, taskService.GetVariablesTyped(taskId, null, true));

		expectedVariables = new Dictionary<string, object>();
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		Assert.AreEqual(expectedVariables, runtimeService.GetVariables(ProcessInstanceId));
		Assert.AreEqual(expectedVariables, runtimeService.GetVariablesLocal(ProcessInstanceId));


		// typed variable API

		List<string> serializableValue = new List<string>();
		serializableValue.Add("1");
		serializableValue.Add("2");
		//taskService.SetVariable(taskId, "objectVariable", objectValue(serializableValue).Create());

		List<string> serializableValueLocal = new List<string>();
		serializableValueLocal.Add("3");
		serializableValueLocal.Add("4");
		//taskService.SetVariableLocal(taskId, "objectVariableLocal", objectValue(serializableValueLocal).Create());

		object value = taskService.GetVariable(taskId, "objectVariable");
		Assert.AreEqual(serializableValue, value);

		object valueLocal = taskService.GetVariableLocal(taskId, "objectVariableLocal");
		Assert.AreEqual(serializableValueLocal, valueLocal);

		IObjectValue typedValue = taskService.GetVariableTyped<IObjectValue>(taskId, "objectVariable");
		Assert.AreEqual(serializableValue, typedValue.Value);

		IObjectValue serializedValue = taskService.GetVariableTyped<IObjectValue>(taskId, "objectVariable", false);
		Assert.IsFalse(serializedValue.IsDeserialized);

		IObjectValue typedValueLocal = taskService.GetVariableLocalTyped<IObjectValue>(taskId, "objectVariableLocal");
		Assert.AreEqual(serializableValueLocal, typedValueLocal.Value);

		IObjectValue serializedValueLocal = taskService.GetVariableLocalTyped<IObjectValue>(taskId, "objectVariableLocal", false);
		Assert.IsFalse(serializedValueLocal.IsDeserialized);

		try
		{
           IObjectValue val = taskService.GetVariableTyped<IObjectValue>(taskId, "objectVariable");
		  Assert.Fail("expected exception");
		}
		catch (System.InvalidCastException)
		{
		  //happy path
		}

	  }
	}

}