using NUnit.Framework;

namespace Engine.Tests.Bpmn.Property
{

	using PluggableProcessEngineTestCase = PluggableProcessEngineTestCase;

	/// <summary>
	/// 
	/// </summary>
	public class PropertyTest : PluggableProcessEngineTestCase
	{



        [Test]
        [Deployment]
        public virtual void testUserTaskSrcProperty()
	  {

		// DO NOT Permissions.Delete: WILL BE REACTIVATED SOON!
		// http://jira.codehaus.org/browse/ACT-88


	//    // Start the process -> waits in usertask
	//    Map<String, Object> vars = new HashMap<String, Object>();
	//    vars.put("inputVar", "test");
	//    IProcessInstance pi = deployer.GetProcessService().StartProcessInstanceByKey("testUserTaskSrcProperty", vars);
	//
	//    // 1 task should be active, and since the task is scoped 1 child execution
	//    // should exist
	//    Assert.NotNull(deployer.GetTaskService().CreateTaskQuery().First());
	//    List<IExecution> childExecutions = deployer.GetProcessService().FindChildExecutions(pi.GetId());
	//    Assert.AreEqual(1, childExecutions.Count());
	//
	//    // The scope at the task should be able to see the 'myVar' variable,
	//    // but the process instance shouldn't be able to see it
	//    IExecution childExecution = childExecutions[0];
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(childExecution.GetId(), "myVar"));
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "myVar"));
	//
	//    // The variable 'inputVar' should be visible for both
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(childExecution.GetId(), "inputVar"));
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(pi.GetId(), "inputVar"));
	//
	//    // Change the value of variable 'myVar' on the task scope
	//    deployer.GetProcessService().SetVariable(childExecution.GetId(), "myVar", "new_value");
	//    Assert.AreEqual("new_value", deployer.GetProcessService().GetVariable(childExecution.GetId(), "myVar"));
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(childExecution.GetId(), "inputVar"));
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "myVar"));
	//
	//    // When the task completes, the variable 'myVar' is destroyed
	//    specifier.GetTaskService().Complete(specifier.GetTaskService().CreateTaskQuery().First().GetId());
	//    for (IExecution execution : deployer.GetProcessService().FindChildExecutions(pi.GetId())) {
	//      Assert.IsNull(deployer.GetProcessService().GetVariable(execution.GetId(), "myVar"));
	//    }
	  }


        [Test]
        [Deployment]
        public virtual void testUserTaskSrcExprProperty()
	  {

	//    // Start the process -> waits in usertask
	//    final String address = "TestStreet 123 90210 Beverly-Hills";
	//    Order order = new Order(address);
	//    Map<String, Object> vars = new HashMap<String, Object>();
	//    vars.put("order", order);
	//    IProcessInstance pi = deployer.GetProcessService().StartProcessInstanceByKey("testUserTaskSrcExprProperty", vars);
	//
	//    // The execution at the task should be able to see the 'orderAddress'
	//    // variable,
	//    // but the process instance shouldn't be able to see it
	//    List<IExecution> childExecutions = deployer.GetProcessService().FindChildExecutions(pi.GetId());
	//    String childExecutionId = childExecutions[0].GetId();
	//    Assert.AreEqual(address, deployer.GetProcessService().GetVariable(childExecutionId, "orderAddress"));
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "orderAddress"));
	//
	//    // Completing the task removes the 'orderAddress' variable
	//    specifier.GetTaskService().Complete(specifier.GetTaskService().CreateTaskQuery().First().GetId());
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "orderAddress"));
	//    Assert.NotNull(deployer.GetProcessService().GetVariable(pi.GetId(), "order"));
	  }


        [Test]
        [Deployment]
        public virtual void testUserTaskDstProperty()
	  {

	//    IProcessInstance pi = deployer.GetProcessService().StartProcessInstanceByKey("testUserTaskDstProperty");
	//    List<IExecution> childExecutions = deployer.GetProcessService().FindChildExecutions(pi.GetId());
	//    String childExecutionId = childExecutions[0].GetId();
	//
	//    // The execution at the task should be able to see the 'taskVar' variable,
	//    Map<String, Object> vars = deployer.GetProcessService().GetVariables(childExecutionId);
	//    Assert.AreEqual(1, vars.Count());
	//    Assert.True(vars.containsKey("taskVar"));
	//
	//    // but the process instance shouldn't be able to see it
	//    Assert.True(deployer.GetProcessService().GetVariables(pi.GetId()).IsEmpty());
	//
	//    // Setting the 'taskVar' value and completing the task should push the value
	//    // into 'processVar'
	//    deployer.GetProcessService().SetVariable(childExecutionId, "taskVar", "myValue");
	//    specifier.GetTaskService().Complete(specifier.GetTaskService().CreateTaskQuery().First().GetId());
	//    vars = deployer.GetProcessService().GetVariables(pi.GetId());
	//    Assert.AreEqual(1, vars.Count());
	//    Assert.True(vars.containsKey("processVar"));
	  }


        [Test]
        [Deployment]
        public virtual void testUserTaskDstExprProperty()
	  {

	//    Order order = new Order();
	//    Map<String, Object> vars = new HashMap<String, Object>();
	//    vars.put("order", order);
	//    IProcessInstance pi = deployer.GetProcessService().StartProcessInstanceByKey("testUserTaskDstExprProperty", vars);
	//
	//    List<IExecution> childExecutions = deployer.GetProcessService().FindChildExecutions(pi.GetId());
	//    String childExecutionId = childExecutions[0].GetId();
	//
	//    // The execution at the task should be able to see the 'orderAddress'
	//    // variable,
	//    vars = deployer.GetProcessService().GetVariables(childExecutionId);
	//    Assert.AreEqual(1, vars.Count());
	//    Assert.True(vars.containsKey("orderAddress"));
	//
	//    // but the process instance shouldn't be able to see it
	//    vars = deployer.GetProcessService().GetVariables(pi.GetId());
	//    Assert.AreEqual(1, vars.Count());
	//    Assert.True(vars.containsKey("order"));
	//
	//    // Setting the 'orderAddress' value and completing the task should push the
	//    // value into order object
	//    deployer.GetProcessService().SetVariable(childExecutionId, "orderAddress", "testAddress");
	//    specifier.GetTaskService().Complete(specifier.GetTaskService().CreateTaskQuery().First().GetId());
	//    Assert.AreEqual(1, deployer.GetProcessService().GetVariables(pi.GetId()).Count());
	//
	//    Order orderAfterComplete = (Order) deployer.GetProcessService().GetVariable(pi.GetId(), "order");
	//    Assert.AreEqual("testAddress", orderAfterComplete.GetAddress());
	  }


        [Test]
        [Deployment]
        public virtual void testUserTaskLinkProperty()
	  {

	//    // Start the process -> waits in usertask
	//    Map<String, Object> vars = new HashMap<String, Object>();
	//    vars.put("inputVar", "test");
	//    IProcessInstance pi = deployer.GetProcessService().StartProcessInstanceByKey("testUserTaskLinkProperty", vars);
	//
	//    // Variable 'taskVar' should only be visible for the task scoped execution
	//    IExecution childExecution = deployer.GetProcessService().FindChildExecutions(pi.GetId())[0];
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(childExecution.GetId(), "taskVar"));
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(childExecution.GetId(), "inputVar"));
	//
	//    // Change the value of variable 'taskVar' on the task scope
	//    String taskScopedExecutionId = childExecution.GetId();
	//    deployer.GetProcessService().SetVariable(taskScopedExecutionId, "taskVar", "new_value");
	//    Assert.AreEqual("new_value", deployer.GetProcessService().GetVariable(taskScopedExecutionId, "taskVar"));
	//    Assert.AreEqual("test", deployer.GetProcessService().GetVariable(taskScopedExecutionId, "inputVar"));
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "taskVar"));
	//
	//    // Completing the task copies the value of 'taskVar' into 'inputVar'
	//    specifier.GetTaskService().Complete(specifier.GetTaskService().CreateTaskQuery().First().GetId());
	//    Assert.True(deployer.GetProcessService().FindChildExecutions(pi.GetId()).IsEmpty()); // second
	//                                                                          // task
	//                                                                          // is
	//                                                                          // not
	//                                                                          // scoped
	//    Assert.IsNull(deployer.GetProcessService().FindExecutionById(taskScopedExecutionId));
	//    Assert.IsNull(deployer.GetProcessService().GetVariable(pi.GetId(), "taskVar"));
	//    Assert.AreEqual("new_value", deployer.GetProcessService().GetVariable(pi.GetId(), "inputVar"));

	  }

	  // @Test public void testUserTaskLinkExprProperty() {
	  // deployProcessForThisTestMethod();
	  //    
	  // // Start the process -> waits in usertask
	  // Map<String, Object> address = new HashMap<String, Object>();
	  // address.put("Street", "Broadway");
	  // address.put("City", "New York");
	  //    
	  // Map<String, Object> variables = new HashMap<String, Object>();
	  // variables.put("address", address);
	  // IProcessInstance pi =
	  // processService.StartProcessInstanceByKey("testUserTaskLinkExprProperty",
	  // variables);
	  //    
	  // // Variable 'taskVar' should only be visible for the task scoped execution
	  // IExecution childExecution =
	  // processService.FindChildExecutions(pi.GetId())[0];
	  // Assert.AreEqual("test", processService.GetVariable(childExecution.GetId(),
	  // "taskVar"));
	  // Assert.AreEqual("test", processService.GetVariable(childExecution.GetId(),
	  // "inputVar"));
	  //    
	  // // Change the value of variable 'taskVar' on the task scope
	  // String taskScopedExecutionId = childExecution.GetId();
	  // processService.SetVariable(taskScopedExecutionId, "taskVar", "new_value");
	  // Assert.AreEqual("new_value", processService.GetVariable(taskScopedExecutionId,
	  // "taskVar"));
	  // Assert.AreEqual("test", processService.GetVariable(taskScopedExecutionId,
	  // "inputVar"));
	  // Assert.IsNull(processService.GetVariable(pi.GetId(), "taskVar"));
	  //    
	  // // Completing the task copies the value of 'taskVar' into 'inputVar'
	  // taskService.Complete(taskService.CreateTaskQuery().First().GetId());
	  // Assert.True(processService.FindChildExecutions(pi.GetId()).IsEmpty()); //
	  // second task is not scoped
	  // Assert.IsNull(processService.FindExecutionById(taskScopedExecutionId));
	  // Assert.IsNull(processService.GetVariable(pi.GetId(), "taskVar"));
	  // Assert.AreEqual("new_value", processService.GetVariable(pi.GetId(),
	  // "inputVar"));
	  // }

	}

}