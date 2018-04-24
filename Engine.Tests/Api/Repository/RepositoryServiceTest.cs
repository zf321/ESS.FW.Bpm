using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Deployer;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;
//using ESS.FW.Bpm.Engine.Tests.Bpmn.TaskListener.Util;


namespace Engine.Tests.Api.Repository
{
    /// <summary>
	/// 
	/// 
	/// 
	/// </summary>
	public class RepositoryServiceTest : PluggableProcessEngineTestCase
	{

	  private const string Namespace = "xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL'";
	  private static readonly string TargetNamespace = "targetNamespace='" + BpmnParse.CamundaBpmnExtensionsNs + "'";

        [TearDown]
	  public void tearDown()
	  {
		ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.Execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly RepositoryServiceTest _outerInstance;

		  public CommandAnonymousInnerClass(RepositoryServiceTest outerInstance)
		  {
			  this._outerInstance = outerInstance;
		  }

		  public virtual object Execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }


	  private void CheckDeployedBytes(System.IO.Stream deployedResource, byte[] utf8Bytes)
	  {
		byte[] deployedBytes = new byte[utf8Bytes.Length];
		deployedResource.Read(deployedBytes, 0, deployedBytes.Length);

		for (int i = 0; i < utf8Bytes.Length; i++)
		{
		  Assert.AreEqual(utf8Bytes[i], deployedBytes[i]);
		}
	  }


	  public virtual void TestUtf8DeploymentMethod()
	  {
		//given utf8 charset
		Encoding utf8Charset = Encoding.UTF8;
        Encoding defaultCharset = processEngineConfiguration.DefaultCharset;
		processEngineConfiguration.DefaultCharset = utf8Charset;

		//and model instance with umlauts
		string umlautsString = "äöüÄÖÜß";
		string resourceName = "deployment.bpmn";
		IBpmnModelInstance instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("umlautsProcess").StartEvent(umlautsString).Done();
		string instanceAsString = ESS.FW.Bpm.Model.Bpmn.Bpmn.ConvertToString(instance);

		//when instance is deployed via AddString method
		IDeployment deployment = repositoryService.CreateDeployment().AddString(resourceName, instanceAsString).Deploy();

		//then bytes are saved in utf-8 format
		System.IO.Stream inputStream = repositoryService.GetResourceAsStream(deployment.Id, resourceName);
		byte[] utf8Bytes = instanceAsString.GetBytes(utf8Charset);
		CheckDeployedBytes(inputStream, utf8Bytes);
		repositoryService.DeleteDeployment(deployment.Id);


		//when model instance is deployed via AddModelInstance method
		deployment = repositoryService.CreateDeployment().AddModelInstance(resourceName, instance).Deploy();

		//then also the bytes are saved in utf-8 format
		inputStream = repositoryService.GetResourceAsStream(deployment.Id, resourceName);
		CheckDeployedBytes(inputStream, utf8Bytes);

		repositoryService.DeleteDeployment(deployment.Id);
		processEngineConfiguration.DefaultCharset = defaultCharset;
	  }


        [Test]
        public virtual void TestStartProcessInstanceById()
	  {
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.AreEqual(1, processDefinitions.Count);

		IProcessDefinition processDefinition = processDefinitions[0];
		Assert.AreEqual("oneTaskProcess", processDefinition.Key);
		Assert.NotNull(processDefinition.Id);
	  }


        [Test]
        public virtual void TestFindProcessDefinitionById()
        {
            IList<IProcessDefinition> definitions = repositoryService.CreateProcessDefinitionQuery()
                .ToList();
		Assert.AreEqual(1, definitions.Count);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.ProcessDefinition ==definitions[0]).First();
		runtimeService.StartProcessInstanceByKey("oneTaskProcess");
		Assert.NotNull(processDefinition);
		Assert.AreEqual("oneTaskProcess", processDefinition.Key);
		Assert.AreEqual("The One ITask Process", processDefinition.Name);

		processDefinition = repositoryService.GetProcessDefinition(definitions[0].Id);
		Assert.AreEqual("This is a process for testing purposes", processDefinition.Description);
	  }


        [Test]
        public virtual void TestDeleteDeploymentWithRunningInstances()
	  {
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.AreEqual(1, processDefinitions.Count);
		IProcessDefinition processDefinition = processDefinitions[0];

		runtimeService.StartProcessInstanceById(processDefinition.Id);

		// Try to Delete the deployment
		try
		{
		  repositoryService.DeleteDeployment(processDefinition.DeploymentId);
		  Assert.Fail("Exception expected");
		}
		catch (ProcessEngineException pee)
		{
		  // Exception expected when deleting deployment with running process
		 // Assert(pee.Message.Contains("Deletion of process definition without cascading failed."));
		}
	  }

        [Test]
        public virtual void TestDeleteDeploymentSkipCustomListeners()
	  {
		IDeploymentBuilder deploymentBuilder = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeleteProcessInstanceSkipCustomListeners.bpmn20.xml");

		string deploymentId = deploymentBuilder.Deploy().Id;

		runtimeService.StartProcessInstanceByKey("testProcess");

		repositoryService.DeleteDeployment(deploymentId, true, false);
		Assert.AreEqual(1, TestExecutionListener.CollectedEvents.Count);
		TestExecutionListener.Reset();

		deploymentId = deploymentBuilder.Deploy().Id;

		runtimeService.StartProcessInstanceByKey("testProcess");

		repositoryService.DeleteDeployment(deploymentId, true, true);
		Assert.True(TestExecutionListener.CollectedEvents.Count == 0);
		TestExecutionListener.Reset();

	  }

        [Test]
        public virtual void TestDeleteDeploymentSkipCustomTaskListeners()
	  {
		IDeploymentBuilder deploymentBuilder = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeleteProcessInstanceSkipCustomTaskListeners.bpmn20.xml");

		string deploymentId = deploymentBuilder.Deploy().Id;

		runtimeService.StartProcessInstanceByKey("testProcess");
            // var task =ITaskListener
        //    Bpm.Engine.Tests.Bpmn.task.
       // RecorderTaskListener.RecordedEvents.Clear();

		//repositoryService.DeleteDeployment(deploymentId, true, false);
		//Assert.AreEqual(1, RecorderTaskListener.RecordedEvents.Count);
		//RecorderTaskListener.clear();

		//deploymentId = deploymentBuilder.Deploy().Id;

		//runtimeService.StartProcessInstanceByKey("testProcess");

		//repositoryService.DeleteDeployment(deploymentId, true, true);
		//Assert.True(RecorderTaskListener.RecordedEvents.Count == 0);
		//RecorderTaskListener.clear();
	  }

        [Test]
        public virtual void TestDeleteDeploymentSkipIoMappings()
	  {
		IDeploymentBuilder deploymentBuilder = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeleteDeploymentSkipIoMappings.bpmn20.xml");

		string deploymentId = deploymentBuilder.Deploy().Id;
		runtimeService.StartProcessInstanceByKey("ioMappingProcess");

		// Try to Delete the deployment
		try
		{
		  repositoryService.DeleteDeployment(deploymentId, true, false);//true;
		}
		catch (System.Exception e)
		{
		  throw new ProcessEngineException("Exception is not expected when deleting deployment with running process", e);
		}
	  }

        [Test]
        public virtual void TestDeleteDeploymentWithoutSkipIoMappings()
	  {
		IDeploymentBuilder deploymentBuilder = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeleteDeploymentSkipIoMappings.bpmn20.xml");

		string deploymentId = deploymentBuilder.Deploy().Id;
		runtimeService.StartProcessInstanceByKey("ioMappingProcess");

		// Try to Delete the deployment
		try
		{
		  repositoryService.DeleteDeployment(deploymentId, true, false);//, false
                Assert.Fail("Exception expected");
		}
		catch (System.Exception e)
		{
		  // Exception expected when deleting deployment with running process
		  // Assert (e.GetMessage().Contains("Exception when output mapping is executed"));
		  AssertTextPresent("Exception when output mapping is executed", e.Message);
		}

		repositoryService.DeleteDeployment(deploymentId, true, false);//, true
        }

        [Test]
        public virtual void TestDeleteDeploymentNullDeploymentId()
	  {
		try
		{
		  repositoryService.DeleteDeployment(null);
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("deploymentId is null", ae.Message);
		}
	  }

        [Test]
        public virtual void TestDeleteDeploymentCascadeNullDeploymentId()
	  {
		try
		{
		  repositoryService.DeleteDeployment(null, true);
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("deploymentId is null", ae.Message);
		}
	  }


        [Test]
        public virtual void TestDeleteDeploymentCascadeWithRunningInstances()
	  {
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.AreEqual(1, processDefinitions.Count);
		IProcessDefinition processDefinition = processDefinitions[0];

		runtimeService.StartProcessInstanceById(processDefinition.Id);

		// Try to Delete the deployment, no exception should be thrown
		repositoryService.DeleteDeployment(processDefinition.DeploymentId, true);
	  }


        [Test]
        public virtual void TestDeleteDeploymentClearsCache()
	  {

		// fetch definition ids
		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;
		string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;
		// fetch CMMN model to be placed to in the cache
		//repositoryService.GetCmmnModelInstance(caseDefinitionId);

		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// ensure definitions and models are part of the cache
		Assert.NotNull(deploymentCache.ProcessDefinitionCache.Get(processDefinitionId));
		Assert.NotNull(deploymentCache.BpmnModelInstanceCache.Get(processDefinitionId));
		//Assert.NotNull(deploymentCache.CaseDefinitionCache.Get(caseDefinitionId));
		//Assert.NotNull(deploymentCache.CmmnModelInstanceCache.Get(caseDefinitionId));

		// when the deployment is deleted
		repositoryService.DeleteDeployment(DeploymentId, true);

		// then the definitions and models are removed from the cache
		Assert.IsNull(deploymentCache.ProcessDefinitionCache.Get(processDefinitionId));
		Assert.IsNull(deploymentCache.BpmnModelInstanceCache.Get(processDefinitionId));
		//Assert.IsNull(deploymentCache.CaseDefinitionCache.Get(caseDefinitionId));
		//Assert.IsNull(deploymentCache.CmmnModelInstanceCache.Get(caseDefinitionId));
	  }

        [Test]
        public virtual void TestFindDeploymentResourceNamesNullDeploymentId()
	  {
		try
		{
		  repositoryService.GetDeploymentResourceNames(null);
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("deploymentId is null", ae.Message);
		}
	  }

        [Test]
        public virtual void TestFindDeploymentResourcesNullDeploymentId()
	  {
		try
		{
		  repositoryService.GetDeploymentResources(null);
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("deploymentId is null", e.Message);
		}
	  }

        [Test]
        public virtual void TestDeploymentWithDelayedProcessDefinitionActivation()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		DateTime inThreeDays = new DateTime(startTime.Ticks + (3 * 24 * 60 * 60 * 1000));

		// Deploy process, but activate after three days
		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/oneTaskProcess.bpmn20.xml").AddClasspathResource("resources/api/twoTasksProcess.bpmn20.xml").ActivateProcessDefinitionsOn(inThreeDays).Deploy();

		Assert.AreEqual(1, repositoryService.CreateDeploymentQuery().Count());
		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
           // .Suspended()
            .Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Shouldn't be able to start a process instance
		try
		{
		  runtimeService.StartProcessInstanceByKey("oneTaskProcess");
		  Assert.Fail();
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresentIgnoreCase("suspended", e.Message);
		}

		IList<IJob> jobs = managementService.CreateJobQuery().ToList();
		managementService.ExecuteJob(jobs[0].Id);
		managementService.ExecuteJob(jobs[1].Id);

		Assert.AreEqual(1, repositoryService.CreateDeploymentQuery().Count());
		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery()
           // .Suspended()
            .Count());
		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Should be able to start process instance
		runtimeService.StartProcessInstanceByKey("oneTaskProcess");
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

		// Cleanup
		repositoryService.DeleteDeployment(deployment.Id, true);
	  }

        [Test]
        public virtual void TestDeploymentWithDelayedProcessDefinitionAndJobDefinitionActivation()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		DateTime inThreeDays = new DateTime(startTime.Ticks + (3 * 24 * 60 * 60 * 1000));

		// Deploy process, but activate after three days
		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/oneAsyncTask.bpmn").ActivateProcessDefinitionsOn(inThreeDays).Deploy();

		Assert.AreEqual(1, repositoryService.CreateDeploymentQuery().Count());

		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery()
            //.Suspended()
            .Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery()
            //.Suspended()
            .Count());
		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Shouldn't be able to start a process instance
		try
		{
		  runtimeService.StartProcessInstanceByKey("oneTaskProcess");
		  Assert.Fail();
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresentIgnoreCase("suspended", e.Message);
		}

		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		Assert.AreEqual(1, repositoryService.CreateDeploymentQuery().Count());

		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery()
            //.Suspended()
            .Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());
		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery()
            //.Suspended()
            .Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Should be able to start process instance
		runtimeService.StartProcessInstanceByKey("oneTaskProcess");
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

		// Cleanup
		repositoryService.DeleteDeployment(deployment.Id, true);
	  }


        [Test]
        public virtual void TestGetResourceAsStreamUnexistingResourceInExistingDeployment()
	  {
		// Get hold of the deployment id
		IDeployment deployment = repositoryService.CreateDeploymentQuery().First();

		try
		{
		  repositoryService.GetResourceAsStream(deployment.Id, "resources/api/unexistingProcess.bpmn.xml");
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("no resource found with name", ae.Message);
		}
	  }


        [Test]
        public virtual void TestGetResourceAsStreamUnexistingDeployment()
	  {

		try
		{
		  repositoryService.GetResourceAsStream("unexistingdeployment", "resources/api/unexistingProcess.bpmn.xml");
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("no resource found with name", ae.Message);
		}
	  }


        [Test]
        public virtual void TestGetResourceAsStreamNullArguments()
	  {
		try
		{
		  repositoryService.GetResourceAsStream(null, "resource");
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("deploymentId is null", ae.Message);
		}

		try
		{
		  repositoryService.GetResourceAsStream("deployment", null);
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  AssertTextPresent("resourceName is null", ae.Message);
		}
	  }


        [Test]
        public virtual void TestGetCaseDefinition()
	  {
		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

		ICaseDefinition caseDefinition = query.First();
		string caseDefinitionId = caseDefinition.Id;

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinitionId);

		Assert.NotNull(definition);
		Assert.AreEqual(caseDefinitionId, definition.Id);
	  }

        [Test]
        public virtual void TestGetCaseDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.GetCaseDefinition("invalid");
		}
		catch (NotFoundException e)
		{
		  AssertTextPresent("no deployed case definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetCaseDefinition(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("caseDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetCaseModel()
	  {
		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

		ICaseDefinition caseDefinition = query.First();
		string caseDefinitionId = caseDefinition.Id;

		System.IO.Stream caseModel = repositoryService.GetCaseModel(caseDefinitionId);

		Assert.NotNull(caseModel);

		byte[] readInputStream = IoUtil.ReadInputStream(caseModel, "caseModel");
		string model = StringHelperClass.NewString(readInputStream, "UTF-8");

		Assert.True(model.Contains("<case id=\"one\" name=\"One\">"));

		IoUtil.CloseSilently(caseModel);
	  }


        [Test]
        public virtual void TestGetCaseModelByInvalidId()
	  {
		try
		{
		  repositoryService.GetCaseModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("no deployed case definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetCaseModel(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("caseDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetDecisionDefinition()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

		IDecisionDefinition decisionDefinition = query.First();
		string decisionDefinitionId = decisionDefinition.Id;

		IDecisionDefinition definition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

		Assert.NotNull(definition);
		Assert.AreEqual(decisionDefinitionId, definition.Id);
	  }

        [Test]
        public virtual void TestGetDecisionDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.GetDecisionDefinition("invalid");
		  Assert.Fail();
		}
		catch (NotFoundException e)
		{
		  AssertTextPresent("no deployed decision definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetDecisionDefinition(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("decisionDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetDecisionRequirementsDefinition()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		IDecisionRequirementsDefinition decisionRequirementsDefinition = query.First();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		IDecisionRequirementsDefinition definition = repositoryService.GetDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		Assert.NotNull(definition);
		Assert.AreEqual(decisionRequirementsDefinitionId, definition.Id);
	  }

        [Test]
        public virtual void TestGetDecisionRequirementsDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.GetDecisionRequirementsDefinition("invalid");
		  Assert.Fail();
		}
		catch (System.Exception e)
		{
		  AssertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetDecisionRequirementsDefinition(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetDecisionModel()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

		IDecisionDefinition decisionDefinition = query.First();
		string decisionDefinitionId = decisionDefinition.Id;

		System.IO.Stream decisionModel = repositoryService.GetDecisionModel(decisionDefinitionId);

		Assert.NotNull(decisionModel);

		byte[] readInputStream = IoUtil.ReadInputStream(decisionModel, "decisionModel");
		string model = StringHelperClass.NewString(readInputStream, "UTF-8");

		Assert.True(model.Contains("<decision id=\"one\" name=\"One\">"));

		IoUtil.CloseSilently(decisionModel);
	  }


        [Test]
        public virtual void TestGetDecisionModelByInvalidId()
	  {
		try
		{
		  repositoryService.GetDecisionModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("no deployed decision definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetDecisionModel(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("decisionDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetDecisionRequirementsModel()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		IDecisionRequirementsDefinition decisionRequirementsDefinition = query.First();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		System.IO.Stream decisionRequirementsModel = repositoryService.GetDecisionRequirementsModel(decisionRequirementsDefinitionId);

		Assert.NotNull(decisionRequirementsModel);

		byte[] readInputStream = IoUtil.ReadInputStream(decisionRequirementsModel, "decisionRequirementsModel");
		string model = StringHelperClass.NewString(readInputStream, "UTF-8");

		Assert.True(model.Contains("<definitions id=\"dish\" name=\"Dish\" namespace=\"test-drg\""));
		IoUtil.CloseSilently(decisionRequirementsModel);
	  }


        [Test]
        public virtual void TestGetDecisionRequirementsModelByInvalidId()
	  {
		try
		{
		  repositoryService.GetDecisionRequirementsModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetDecisionRequirementsModel(null);
		  Assert.Fail();
		}
		catch (NotValidException e)
		{
		  AssertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }


        [Test]
        public virtual void TestGetDecisionRequirementsDiagram()
	  {

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		IDecisionRequirementsDefinition decisionRequirementsDefinition = query.First();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		System.IO.Stream actualDrd = repositoryService.GetDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		Assert.NotNull(actualDrd);
	  }


        [Test]
        public virtual void TestGetDecisionRequirementsDiagramByInvalidId()
	  {
		try
		{
		  repositoryService.GetDecisionRequirementsDiagram("invalid");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.GetDecisionRequirementsDiagram(null);
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }

        [Test]
        public virtual void TestDeployRevisedProcessAfterDeleteOnOtherProcessEngine()
	  {

		// Setup both process engines
		IProcessEngine processEngine1 = (new StandaloneProcessEngineConfiguration()).SetProcessEngineName("reboot-test-schema").SetDatabaseSchemaUpdate(processEngineConfiguration.DatabaseSchemaUpdate).SetJdbcUrl("jdbc:h2:mem:activiti-process-cache-test;DB_CLOSE_DELAY=1000").SetJobExecutorActivate(false).BuildProcessEngine();
		IRepositoryService repositoryService1 = processEngine1.RepositoryService;

		IProcessEngine processEngine2 = (new StandaloneProcessEngineConfiguration()).SetProcessEngineName("reboot-test").SetDatabaseSchemaUpdate(processEngineConfiguration.DatabaseSchemaUpdate).SetJdbcUrl("jdbc:h2:mem:activiti-process-cache-test;DB_CLOSE_DELAY=1000").SetJobExecutorActivate(false).BuildProcessEngine();
		IRepositoryService repositoryService2 = processEngine2.RepositoryService;
		IRuntimeService runtimeService2 = processEngine2.RuntimeService;
		ITaskService taskService2 = processEngine2.TaskService;

		// Deploy first version of process: start->originalTask->end on first process engine
		string deploymentId = repositoryService1.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeployRevisedProcessAfterDeleteOnOtherProcessEngine.v1.bpmn20.xml").Deploy().Id;

		// Start process instance on second engine
		string processDefinitionId = repositoryService2.CreateProcessDefinitionQuery().First().Id;
		runtimeService2.StartProcessInstanceById(processDefinitionId);
		ITask task = taskService2.CreateTaskQuery().First();
		Assert.AreEqual("original task", task.Name);

		// Delete the deployment on second process engine
		repositoryService2.DeleteDeployment(deploymentId, true);
		Assert.AreEqual(0, repositoryService2.CreateDeploymentQuery().Count());
		Assert.AreEqual(0, runtimeService2.CreateProcessInstanceQuery().Count());

		// deploy a revised version of the process: start->revisedTask->end on first process engine
		//
		// Before the bugfix, this would set the cache on the first process engine,
		// but the second process engine still has the original process definition in his cache.
		// Since there is a deployment Delete in between, the new generated process definition id is the same
		// as in the original deployment, making the second process engine using the old cached process definition.
		deploymentId = repositoryService1.CreateDeployment().AddClasspathResource("resources/api/repository/RepositoryServiceTest.TestDeployRevisedProcessAfterDeleteOnOtherProcessEngine.v2.bpmn20.xml").Deploy().Id;

		// Start process instance on second process engine -> must use revised process definition
		processDefinitionId = repositoryService2.CreateProcessDefinitionQuery().First().Id;
		runtimeService2.StartProcessInstanceByKey("oneTaskProcess");
		task = taskService2.CreateTaskQuery().First();
		Assert.AreEqual("revised task", task.Name);

		// cleanup
		repositoryService1.DeleteDeployment(deploymentId, true);
		processEngine1.Close();
		processEngine2.Close();
	  }

        [Test]
        public virtual void TestDeploymentPersistence()
	  {
		IDeployment deployment = repositoryService.CreateDeployment().Name("strings").AddString("resources/test/HelloWorld.string", "hello world").AddString("resources/test/TheAnswer.string", "42").Deploy();

		IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery().ToList();
		Assert.AreEqual(1, deployments.Count);
		deployment = deployments[0];

		Assert.AreEqual("strings", deployment.Name);
		Assert.NotNull(deployment.DeploymentTime);

		string deploymentId = deployment.Id;
		IList<string> resourceNames = repositoryService.GetDeploymentResourceNames(deploymentId);
		ISet<string> expectedResourceNames = new HashSet<string>();
		expectedResourceNames.Add("resources/test/HelloWorld.string");
		expectedResourceNames.Add("resources/test/TheAnswer.string");
		Assert.AreEqual(expectedResourceNames, new HashSet<string>(resourceNames));

		System.IO.Stream resourceStream = repositoryService.GetResourceAsStream(deploymentId, "resources/test/HelloWorld.string");
		Assert.True( "hello world".GetBytes().Equals(IoUtil.ReadInputStream(resourceStream, "test")));

		resourceStream = repositoryService.GetResourceAsStream(deploymentId, "resources/test/TheAnswer.string");
		Assert.True("42".GetBytes().Equals(IoUtil.ReadInputStream(resourceStream, "test")));

		repositoryService.DeleteDeployment(deploymentId);
	  }

        [Test]
        public virtual void TestProcessDefinitionPersistence()
	  {
		string deploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/processOne.bpmn20.xml").AddClasspathResource("resources/api/repository/processTwo.bpmn20.xml").Deploy().Id;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();

		Assert.AreEqual(2, processDefinitions.Count);

		repositoryService.DeleteDeployment(deploymentId);
	  }


        [Test]
        public virtual void TestDecisionDefinitionUpdateTimeToLive()
	  {
		//given
		IDecisionDefinition decisionDefinition = FindOnlyDecisionDefinition();

		//when
		//repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 6);

		//then
		decisionDefinition = FindOnlyDecisionDefinition();
		//Assert.AreEqual(6, decisionDefinition.HistoryTimeToLive.intValue());

	  }


        [Test]
        public virtual void TestDecisionDefinitionUpdateTimeToLiveNull()
	  {
		//given
		IDecisionDefinition decisionDefinition = FindOnlyDecisionDefinition();

		//when
		//repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, null);

		//then
		decisionDefinition = (DecisionDefinitionEntity) repositoryService.GetDecisionDefinition(decisionDefinition.Id);
		//Assert.AreEqual(null, decisionDefinition.HistoryTimeToLive);

	  }


        [Test]
        public virtual void TestDecisionDefinitionUpdateTimeToLiveNegative()
	  {
		//given
		IDecisionDefinition decisionDefinition = FindOnlyDecisionDefinition();

		//when
		try
		{
		//  repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, -1);
		  Assert.Fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  Assert.True(ex.Message.Contains("greater than"));
		}

	  }


        [Test]
        public virtual void TestProcessDefinitionUpdateTimeToLive()
	  {
		//given
		IProcessDefinition processDefinition = FindOnlyProcessDefinition();

		//when
		//repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinition.Id, 6);

		//then
		processDefinition = FindOnlyProcessDefinition();
		//Assert.AreEqual(6, processDefinition.HistoryTimeToLive.intValue());

	  }


        [Test]
        public virtual void TestProcessDefinitionUpdateTimeToLiveNull()
	  {
		//given
		IProcessDefinition processDefinition = FindOnlyProcessDefinition();

		//when
		//repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinition.Id, null);

		//then
		processDefinition = FindOnlyProcessDefinition();
		//Assert.AreEqual(null, processDefinition.HistoryTimeToLive);

	  }


        [Test]
        public virtual void TestProcessDefinitionUpdateTimeToLiveNegative()
	  {
		//given
		IProcessDefinition processDefinition = FindOnlyProcessDefinition();

		//when
		try
		{
		 // repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinition.Id, -1);
		  Assert.Fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  Assert.True(ex.Message.Contains("greater than"));
		}

	  }


        [Test]
        public virtual void TestProcessDefinitionUpdateTimeToLiveUserOperationLog()
	  {
		//given
		IProcessDefinition processDefinition = FindOnlyProcessDefinition();
		//int? timeToLiveOrgValue = processDefinition.HistoryTimeToLive;
		ProcessEngine.IdentityService.AuthenticatedUserId = "userId";

		//when
		int? timeToLiveNewValue = 6;
		//repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinition.Id, timeToLiveNewValue);

		//then
		IList<IUserOperationLogEntry> opLogEntries = ProcessEngine.HistoryService.CreateUserOperationLogQuery().ToList();
		Assert.AreEqual(1, opLogEntries.Count);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.history.Event.UserOperationLogEntryEventEntity userOperationLogEntry = (org.Camunda.bpm.Engine.impl.history.Event.UserOperationLogEntryEventEntity)opLogEntries[0];
		UserOperationLogEntryEventEntity userOperationLogEntry = (UserOperationLogEntryEventEntity)opLogEntries[0];

		//Assert.AreEqual(UserOperationLogEntry.OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE, userOperationLogEntry.OperationType);
		Assert.AreEqual(processDefinition.Key, userOperationLogEntry.ProcessDefinitionKey);
		Assert.AreEqual(processDefinition.Id, userOperationLogEntry.ProcessDefinitionId);
		Assert.AreEqual("historyTimeToLive", userOperationLogEntry.Property);
		//Assert.AreEqual(timeToLiveOrgValue, Convert.ToInt32(userOperationLogEntry.OrgValue));
		Assert.AreEqual(timeToLiveNewValue, Convert.ToInt32(userOperationLogEntry.NewValue));

	  }


        [Test]
        public virtual void TestUpdateHistoryTimeToLive()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		ICaseDefinition caseDefinition = FindOnlyCaseDefinition();

		// when
		//repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, 6);

		// then
		caseDefinition = FindOnlyCaseDefinition();

		//Assert.AreEqual(6, caseDefinition.HistoryTimeToLive.intValue());
	  }

        public virtual void TestUpdateHistoryTimeToLiveNull()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		ICaseDefinition caseDefinition = FindOnlyCaseDefinition();

		// when
		//UpdateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, null);

		// then
		caseDefinition = FindOnlyCaseDefinition();

		//Assert.AreEqual(null, caseDefinition.HistoryTimeToLive);
	  }


        [Test]
        public virtual void TestUpdateHistoryTimeToLiveNegative()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		ICaseDefinition caseDefinition = FindOnlyCaseDefinition();

		// when
		try
		{
		 // repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, -1);
		  Assert.Fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  Assert.True(ex.Message.Contains("greater than"));
		}
	  }


        [Test]
        public virtual void TestUpdateHistoryTimeToLiveInCache()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		ICaseDefinition caseDefinition = FindOnlyCaseDefinition();

		// assume
		//Assert.IsNull(caseDefinition.HistoryTimeToLive);

		// when
		//repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, 10);

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinition.Id);
		//Assert.AreEqual(Convert.ToInt32(10), definition.HistoryTimeToLive);
	  }

	  private ICaseDefinition FindOnlyCaseDefinition()
	  {
		IList<ICaseDefinition> caseDefinitions = repositoryService.CreateCaseDefinitionQuery().ToList();
		Assert.NotNull(caseDefinitions);
		Assert.AreEqual(1, caseDefinitions.Count);
		return caseDefinitions[0];
	  }

	  private IProcessDefinition FindOnlyProcessDefinition()
	  {
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.NotNull(processDefinitions);
		Assert.AreEqual(1, processDefinitions.Count);
		return processDefinitions[0];
	  }

	  private IDecisionDefinition FindOnlyDecisionDefinition()
	  {
		IList<IDecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery().ToList();
		Assert.NotNull(decisionDefinitions);
		Assert.AreEqual(1, decisionDefinitions.Count);
		return decisionDefinitions[0];
	  }

        [Test]
        public virtual void TestProcessDefinitionIntrospection()
	  {
		string deploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/processOne.bpmn20.xml").Deploy().Id;

		string procDefId = repositoryService.CreateProcessDefinitionQuery().First().Id;
		IReadOnlyProcessDefinition processDefinition = ((RepositoryServiceImpl)repositoryService).GetDeployedProcessDefinition(procDefId);

		Assert.AreEqual(procDefId, processDefinition.Id);
		Assert.AreEqual("Process One", processDefinition.Name);
		Assert.AreEqual("the first process", processDefinition.GetProperty("documentation"));

		IPvmActivity start = processDefinition.FindActivity("start");
		Assert.NotNull(start);
		Assert.AreEqual("start", start.Id);
		Assert.AreEqual("S t a r t", start.GetProperty("name"));
		Assert.AreEqual("the start event", start.GetProperty("documentation"));
		Assert.AreEqual(new List<IPvmActivity>(), start.Activities);
		IList<IPvmTransition> outgoingTransitions = start.OutgoingTransitions;
		Assert.AreEqual(1, outgoingTransitions.Count);
		Assert.AreEqual("${a == b}", outgoingTransitions[0].GetProperty(BpmnParse.PropertynameConditionText));

		IPvmActivity end = processDefinition.FindActivity("end");
		Assert.NotNull(end);
		Assert.AreEqual("end", end.Id);

		IPvmTransition transition = outgoingTransitions[0];
		Assert.AreEqual("flow1", transition.Id);
		Assert.AreEqual("Flow One", transition.GetProperty("name"));
		Assert.AreEqual("The only transitions in the process", transition.GetProperty("documentation"));
		Assert.AreSame(start, transition.Source);
            Assert.AreSame(end, transition.Destination);

		repositoryService.DeleteDeployment(deploymentId);
	  }

        [Test]
        public virtual void TestProcessDefinitionQuery()
	  {
		string deployment1Id = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/processOne.bpmn20.xml").AddClasspathResource("resources/api/repository/processTwo.bpmn20.xml").Deploy().Id;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
               // /*.OrderByProcessDefinitionName()*//*.Asc()*/
               // .OrderByProcessDefinitionVersion()/*.Asc()*/
                .ToList();

		Assert.AreEqual(2, processDefinitions.Count);

		string deployment2Id = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/processOne.bpmn20.xml").AddClasspathResource("resources/api/repository/processTwo.bpmn20.xml").Deploy().Id;

		Assert.AreEqual(4, repositoryService.CreateProcessDefinitionQuery()
           // /*.OrderByProcessDefinitionName()*//*.Asc()*/
            .Count());
		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
            ///*.LatestVersion()*//*.OrderByProcessDefinitionName()*//*.Asc()*/
            .Count());

		DeleteDeployments(new List<string>(){deployment1Id, deployment2Id });
	  }

        [Test]
        public virtual void TestGetProcessDefinitions()
	  {
		IList<string> deploymentIds = new List<string>();
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='IDR' name='Insurance Damage Report 1' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='IDR' name='Insurance Damage Report 2' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='IDR' name='Insurance Damage Report 3' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='EN' name='Expense Note 1' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='EN' name='Expense Note 2' isExecutable='true' />" + "</definitions>")));

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
               // //.OrderByProcessDefinitionKey()/*.Asc()*/.OrderByProcessDefinitionVersion()/*.Desc()*/
              
                .ToList();

		Assert.NotNull(processDefinitions);

		Assert.AreEqual(5, processDefinitions.Count);

		IProcessDefinition processDefinition = processDefinitions[0];
		Assert.AreEqual("EN", processDefinition.Key);
		Assert.AreEqual("Expense Note 2", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("EN:2"));
		Assert.AreEqual(2, processDefinition.Version);

		processDefinition = processDefinitions[1];
		Assert.AreEqual("EN", processDefinition.Key);
		Assert.AreEqual("Expense Note 1", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("EN:1"));
		Assert.AreEqual(1, processDefinition.Version);

		processDefinition = processDefinitions[2];
		Assert.AreEqual("IDR", processDefinition.Key);
		Assert.AreEqual("Insurance Damage Report 3", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("IDR:3"));
		Assert.AreEqual(3, processDefinition.Version);

		processDefinition = processDefinitions[3];
		Assert.AreEqual("IDR", processDefinition.Key);
		Assert.AreEqual("Insurance Damage Report 2", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("IDR:2"));
		Assert.AreEqual(2, processDefinition.Version);

		processDefinition = processDefinitions[4];
		Assert.AreEqual("IDR", processDefinition.Key);
		Assert.AreEqual("Insurance Damage Report 1", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("IDR:1"));
		Assert.AreEqual(1, processDefinition.Version);

		DeleteDeployments(deploymentIds);
	  }

        [Test]
        public virtual void TestDeployIdenticalProcessDefinitions()
	  {
		IList<string> deploymentIds = new List<string>();
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='IDR' name='Insurance Damage Report' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(DeployProcessString(("<definitions " + Namespace + " " + TargetNamespace + ">" + "  <process id='IDR' name='Insurance Damage Report' isExecutable='true' />" + "</definitions>")));

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                ////.OrderByProcessDefinitionKey()/*.Asc()*/.OrderByProcessDefinitionVersion()/*.Desc()*/
                .ToList();

		Assert.NotNull(processDefinitions);
		Assert.AreEqual(2, processDefinitions.Count);

		IProcessDefinition processDefinition = processDefinitions[0];
		Assert.AreEqual("IDR", processDefinition.Key);
		Assert.AreEqual("Insurance Damage Report", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("IDR:2"));
		Assert.AreEqual(2, processDefinition.Version);

		processDefinition = processDefinitions[1];
		Assert.AreEqual("IDR", processDefinition.Key);
		Assert.AreEqual("Insurance Damage Report", processDefinition.Name);
		Assert.True(processDefinition.Id.StartsWith("IDR:1"));
		Assert.AreEqual(1, processDefinition.Version);

		DeleteDeployments(deploymentIds);
	  }

	  private string DeployProcessString(string processString)
	  {
		string resourceName = "xmlString." + BpmnDeployer.BpmnResourceSuffixes[0];
		return repositoryService.CreateDeployment().AddString(resourceName, processString).Deploy().Id;
	  }

	  private void DeleteDeployments(ICollection<string> deploymentIds)
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  repositoryService.DeleteDeployment(deploymentId);
		}
	  }

	}

}