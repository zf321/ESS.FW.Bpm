using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Repository
{
    
    
	/// 
	/// <summary>
	/// 
	/// </summary>
	public class DeleteProcessDefinitionTest
	{


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();



	  protected internal IRepositoryService repositoryService;
	  protected internal IRuntimeService runtimeService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal IDeployment deployment;
        public const string DEPLOYMENT_NAME = "my-deployment";

        [SetUp]
        public virtual void initServices()
	  {
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		processEngineConfiguration = (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;
	  }

        [TearDown]
	  public virtual void cleanUp()
	  {
		if (deployment != null)
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		  deployment = null;
		}
	  }

        //[ExpectedException(typeof(NullValueException))]
	   [Test]
        public virtual void testDeleteProcessDefinitionNullId()
	  {
		// declare expected exception
	//	thrown.Expect(typeof(NullValueException));
	//	thrown.ExpectMessage("processDefinitionId is null");

		repositoryService.DeleteProcessDefinition(null);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessDefinition()
	   [Test]   public virtual void testDeleteNonExistingProcessDefinition()
	  {
		// declare expected exception
		//thrown.Expect(typeof(NotFoundException));
		//thrown.ExpectMessage("No process definition found with id 'notexist': processDefinition is null");

		repositoryService.DeleteProcessDefinition("notexist");
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinition()
	   [Test]   public virtual void testDeleteProcessDefinition()
	  {
		// given deployment with two process definitions in one xml model file
		deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/repository/twoProcesses.bpmn20.xml").Deploy();
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();

		//when a process definition is been deleted
		repositoryService.DeleteProcessDefinition(processDefinitions[0].Id);

		//then only one process definition should remain
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionWithProcessInstance()
	   [Test]   public virtual void testDeleteProcessDefinitionWithProcessInstance()
	  {
		// given process definition and a process instance
		IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		deployment = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", bpmnModel).Deploy();
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process").First();
		runtimeService.CreateProcessInstanceByKey("process").ExecuteWithVariablesInReturn();

		//when the corresponding process definition is deleted from the deployment
		try
		{
		  repositoryService.DeleteProcessDefinition(processDefinition.Id);
		  Assert.Fail("Should Assert.Fail, since there exists a process instance");
		}
		catch (ProcessEngineException pee)
		{
		  // then Exception is expected, the deletion should Assert.Fail since there exist a process instance
		  // and the cascade flag is per default false
		  Assert.True(pee.Message.Contains("Deletion of process definition without cascading failed."));
		}
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionCascade()
	   [Test]   public virtual void testDeleteProcessDefinitionCascade()
	  {
		// given process definition and a process instance
		IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		deployment = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", bpmnModel).Deploy();
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process").First();
		runtimeService.CreateProcessInstanceByKey("process").ExecuteWithVariablesInReturn();

		//when the corresponding process definition is cascading deleted from the deployment
		repositoryService.DeleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and no definition
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());
		if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelActivity.Id)
		{
		  Assert.AreEqual(0, engineRule.HistoryService.CreateHistoricActivityInstanceQuery().Count());
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionClearsCache()
	   [Test]   public virtual void testDeleteProcessDefinitionClearsCache()
	  {
		// given process definition and a process instance
		IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		deployment = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", bpmnModel).Deploy();
		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process").First().Id;

		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// ensure definitions and models are part of the cache
		Assert.NotNull(deploymentCache.ProcessDefinitionCache.Get(processDefinitionId));
		Assert.NotNull(deploymentCache.BpmnModelInstanceCache.Get(processDefinitionId));

		repositoryService.DeleteProcessDefinition(processDefinitionId, true);

		// then the definitions and models are removed from the cache
		Assert.IsNull(deploymentCache.ProcessDefinitionCache.Get(processDefinitionId));
		Assert.IsNull(deploymentCache.BpmnModelInstanceCache.Get(processDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionAndRefillDeploymentCache()
	   [Test]   public virtual void testDeleteProcessDefinitionAndRefillDeploymentCache()
	  {
		// given a deployment with two process definitions in one xml model file
		deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/repository/twoProcesses.bpmn20.xml").Deploy();
		IProcessDefinition processDefinitionOne = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one").First();
		IProcessDefinition processDefinitionTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "two").First();

		string idOne = processDefinitionOne.Id;
		//one is deleted from the deployment
		repositoryService.DeleteProcessDefinition(idOne);

		//when clearing the deployment cache
		processEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();

		//then creating process instance from the existing process definition
		IProcessInstanceWithVariables procInst = runtimeService.CreateProcessInstanceByKey("two").ExecuteWithVariablesInReturn();
		Assert.NotNull(procInst);
		Assert.True(procInst.ProcessDefinitionId.Contains("two"));

		//should refill the cache
		ICache<string, ProcessDefinitionEntity> cache = processEngineConfiguration.DeploymentCache.ProcessDefinitionCache;
		Assert.NotNull(cache.Get(processDefinitionTwo.Id));
		//The deleted process definition should not be recreated after the cache is refilled
		Assert.IsNull(cache.Get(processDefinitionOne.Id));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionAndRedeploy()
	   [Test]   public virtual void testDeleteProcessDefinitionAndRedeploy()
	  {
		// given a deployment with two process definitions in one xml model file
		deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/repository/twoProcesses.bpmn20.xml").Deploy();

		IProcessDefinition processDefinitionOne = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one").First();

		//one is deleted from the deployment
		repositoryService.DeleteProcessDefinition(processDefinitionOne.Id);

		//when the process definition is redeployed
		IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment.Id).Deploy();

		//then there should exist three process definitions
		//two of the redeployment and the remaining one
		Assert.AreEqual(3, repositoryService.CreateProcessDefinitionQuery().Count());

		//clean up
		repositoryService.DeleteDeployment(deployment2.Id, true);
	  }
	}

}