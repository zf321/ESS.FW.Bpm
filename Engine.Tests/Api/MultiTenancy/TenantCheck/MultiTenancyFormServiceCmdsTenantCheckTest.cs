using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{

	public class MultiTenancyFormServiceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyFormServiceCmdsTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "formKeyProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal ITaskService taskService;

	  protected internal IFormService formService;

	  protected internal IRuntimeService runtimeService;

	  protected internal IIdentityService identityService;

	  protected internal IRepositoryService repositoryService;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

      [SetUp]
	  public virtual void init()
	  {

		taskService = engineRule.TaskService;

		formService = engineRule.FormService;

		identityService = engineRule.IdentityService;

		runtimeService = engineRule.RuntimeService;

		repositoryService = engineRule.RepositoryService;

		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

	  }

	  
	   [Test]   public virtual void testGetStartFormWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		IStartFormData startFormData = formService.GetStartFormData(instance.ProcessDefinitionId);

		// then
		Assert.NotNull(startFormData);
		Assert.AreEqual("aStartFormKey",startFormData.FormKey);
	  }


	   [Test]   public virtual void testGetStartFormWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition '" + instance.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.GetStartFormData(instance.ProcessDefinitionId);

	  }


	   [Test]   public virtual void testGetStartFormWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IStartFormData startFormData = formService.GetStartFormData(instance.ProcessDefinitionId);

		// then
		Assert.NotNull(startFormData);
		Assert.AreEqual("aStartFormKey",startFormData.FormKey);

	  }

	  // GetRenderedStartForm
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormWithAuthenticatedTenant()
	   [Test]   public virtual void testGetRenderedStartFormWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		Assert.NotNull(formService.GetRenderedStartForm(processDefinitionId, "juel"));
	  }


	   [Test]   public virtual void testGetRenderedStartFormWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.GetRenderedStartForm(processDefinitionId, "juel");
	  }


	   [Test]   public virtual void testGetRenderedStartFormWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		Assert.NotNull(formService.GetRenderedStartForm(processDefinitionId, "juel"));
	  }

	  // submitStartForm
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithAuthenticatedTenant()
	   [Test]   public virtual void testSubmitStartFormWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		Assert.NotNull(formService.SubmitStartForm(processDefinitionId, properties));
	  }


	   [Test]   public virtual void testSubmitStartFormWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot create an instance of the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.SubmitStartForm(processDefinitionId, properties);

	  }


	   [Test]   public virtual void testSubmitStartFormWithDisabledTenantcheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/request.Form");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		Assert.NotNull(formService.SubmitStartForm(processDefinitionId, properties));

	  }

	  // getStartFormKey
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormKeyWithAuthenticatedTenant()
	   [Test]   public virtual void testGetStartFormKeyWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		Assert.AreEqual("aStartFormKey", formService.GetStartFormKey(processDefinitionId));

	  }


	   [Test]   public virtual void testGetStartFormKeyWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition '" + processDefinitionId + "' because it belongs to no authenticated tenant.");
		formService.GetStartFormKey(processDefinitionId);

	  }


	   [Test]   public virtual void testGetStartFormKeyWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionId;

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual("aStartFormKey", formService.GetStartFormKey(processDefinitionId));

	  }

	  
	   [Test]   public virtual void testGetTaskFormWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		string taskId = taskService.CreateTaskQuery().First().Id;

		ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

		// then
		Assert.NotNull(taskFormData);
		Assert.AreEqual("aTaskFormKey", taskFormData.FormKey);
	  }


	   [Test]   public virtual void testGetTaskFormWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		string taskId = taskService.CreateTaskQuery().First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.GetTaskFormData(taskId);

	  }


	   [Test]   public virtual void testGetTaskFormWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		string taskId = taskService.CreateTaskQuery().First().Id;

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

		// then
		Assert.NotNull(taskFormData);
		Assert.AreEqual("aTaskFormKey", taskFormData.FormKey);

	  }

	  
	   [Test]   public virtual void testSubmitTaskFormWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		runtimeService.StartProcessInstanceById(processDefinitionId);

		Assert.AreEqual(taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).Count(), 1);

		string taskId = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		formService.SubmitTaskForm(taskId, null);

		// task gets completed on execution of submitTaskForm
		Assert.AreEqual(taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).Count(), 0);
	  }


	   [Test]   public virtual void testSubmitTaskFormWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		runtimeService.StartProcessInstanceById(processDefinitionId);

		string taskId = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).First().Id;

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot work on task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.SubmitTaskForm(taskId, null);
	  }


	   [Test]   public virtual void testSubmitTaskFormWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		runtimeService.StartProcessInstanceById(processDefinitionId);

		string taskId = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).First().Id;

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		formService.SubmitTaskForm(taskId, null);

		// task gets completed on execution of submitTaskForm
		Assert.AreEqual(taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processDefinitionId).Count(), 0);
	  }

	  
	   [Test]   public virtual void testGetRenderedTaskFormWithAuthenticatedTenant()
	  {

		// deploy tenants
		//testRule.DeployForTenant(TENANT_ONE, "resources/api/form/FormsProcess.bpmn20.xml", "resources/api/form/task.Form").Id;

		string procDefId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		//formService.SubmitStartForm(procDefId, properties).Id;

		string taskId = taskService.CreateTaskQuery().First().Id;
		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual("Mike is speaking in room 5b", formService.GetRenderedTaskForm(taskId, "juel"));
	  }


	   [Test]   public virtual void testGetRenderedTaskFormWithNoAuthenticatedTenant()
	  {

		// deploy tenants
		//testRule.DeployForTenant(TENANT_ONE, "resources/api/form/FormsProcess.bpmn20.xml", "resources/api/form/task.Form").Id;

		string procDefId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		formService.SubmitStartForm(procDefId, properties);

		string taskId = taskService.CreateTaskQuery().First().Id;
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");

		// when
		formService.GetRenderedTaskForm(taskId, "juel");
	  }


	   [Test]   public virtual void testGetRenderedTaskFormWithDisabledTenantCheck()
	  {

		// deploy tenants
		//testRule.DeployForTenant(TENANT_ONE, "resources/api/form/FormsProcess.bpmn20.xml", "resources/api/form/task.Form").Id;

		string procDefId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		formService.SubmitStartForm(procDefId, properties);

		string taskId = taskService.CreateTaskQuery().First().Id;
		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual("Mike is speaking in room 5b", formService.GetRenderedTaskForm(taskId, "juel"));
	  }

	  // getTaskFormKey
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithAuthenticatedTenant()
	   [Test]   public virtual void testGetTaskFormKeyWithAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		ITask task = taskService.CreateTaskQuery().First();

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		Assert.AreEqual("aTaskFormKey", formService.GetTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey));

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithNoAuthenticatedTenant()
	   [Test]   public virtual void testGetTaskFormKeyWithNoAuthenticatedTenant()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		ITask task = taskService.CreateTaskQuery().First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition '" + task.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		formService.GetTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyWithDisabledTenantCheck()
	   [Test]   public virtual void testGetTaskFormKeyWithDisabledTenantCheck()
	  {

		testRule.DeployForTenant(TENANT_ONE, "resources/api/authorization/formKeyProcess.bpmn20.xml");

		runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		ITask task = taskService.CreateTaskQuery().First();

		identityService.SetAuthentication("aUserId", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		formService.GetTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);
		// then
		Assert.AreEqual("aTaskFormKey", formService.GetTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey));
	  }
	}

}