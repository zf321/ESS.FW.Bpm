using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;


namespace Engine.Tests.Api.Repository
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ProcessDefinitionSuspensionTest : PluggableProcessEngineTestCase
	{

        [TearDown]
	  public void tearDown()
	  {
		ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.Execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly ProcessDefinitionSuspensionTest outerInstance;

		  public CommandAnonymousInnerClass(ProcessDefinitionSuspensionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual object Execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testProcessDefinitionActiveByDefault()
	   [Test]   public virtual void testProcessDefinitionActiveByDefault()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		Assert.IsFalse(processDefinition.Suspended);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testSuspendActivateProcessDefinitionById()
	   [Test]   public virtual void testSuspendActivateProcessDefinitionById()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		// suspend
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);
		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		// activate
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id);
		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testSuspendActivateProcessDefinitionByKey()
	   [Test]   public virtual void testSuspendActivateProcessDefinitionByKey()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		//suspend
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key);
		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		//activate
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key);
		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testActivateAlreadyActiveProcessDefinition()
	   [Test]   public virtual void testActivateAlreadyActiveProcessDefinition()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		try
		{
		  repositoryService.ActivateProcessDefinitionById(processDefinition.Id);
		  processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		  Assert.IsFalse(processDefinition.Suspended);
		}
		catch (System.Exception)
		{
		  Assert.Fail("Should be successful");
		}

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testSuspendAlreadySuspendedProcessDefinition()
	   [Test]   public virtual void testSuspendAlreadySuspendedProcessDefinition()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		try
		{
		  repositoryService.SuspendProcessDefinitionById(processDefinition.Id);
		  processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		  Assert.True(processDefinition.Suspended);
		}
		catch (System.Exception)
		{
		  Assert.Fail("Should be successful");
		}

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "resources/api/repository/processOne.bpmn20.xml", "resources/api/repository/processTwo.bpmn20.xml" }) public void testQueryForActiveDefinitions()
	   [Test]   public virtual void testQueryForActiveDefinitions()
	  {

		// default = all definitions
		IList<IProcessDefinition> processDefinitionList = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.AreEqual(2, processDefinitionList.Count);

		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		IProcessDefinition processDefinition = processDefinitionList[0];
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "resources/api/repository/processOne.bpmn20.xml", "resources/api/repository/processTwo.bpmn20.xml" }) public void testQueryForSuspendedDefinitions()
	   [Test]   public virtual void testQueryForSuspendedDefinitions()
	  {

		// default = all definitions
		IList<IProcessDefinition> processDefinitionList = repositoryService.CreateProcessDefinitionQuery().ToList();
		Assert.AreEqual(2, processDefinitionList.Count);

		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		IProcessDefinition processDefinition = processDefinitionList[0];
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testStartProcessInstanceForSuspendedProcessDefinition()
	   [Test]   public virtual void testStartProcessInstanceForSuspendedProcessDefinition()
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		// By id
		try
		{
		  runtimeService.StartProcessInstanceById(processDefinition.Id);
		  Assert.Fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}

		// By Key
		try
		{
		  runtimeService.StartProcessInstanceByKey(processDefinition.Key);
		  Assert.Fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testContinueProcessAfterProcessDefinitionSuspend()
	   [Test]   public virtual void testContinueProcessAfterProcessDefinitionSuspend()
	  {

		// Start Process Instance
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		runtimeService.StartProcessInstanceByKey(processDefinition.Key);

		// Verify one task is created
		ITask task = taskService.CreateTaskQuery().First();
		Assert.NotNull(task);
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

		// Suspend process definition
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		// Process should be able to continue
		taskService.Complete(task.Id);
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testSuspendProcessInstancesDuringProcessDefinitionSuspend()
	   [Test]   public virtual void testSuspendProcessInstancesDuringProcessDefinitionSuspend()
	  {

		int nrOfProcessInstances = 9;

		// Fire up a few processes for the deployed process definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		for (int i = 0; i < nrOfProcessInstances; i++)
		{
		  runtimeService.StartProcessInstanceByKey(processDefinition.Key);
		}
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Suspend process definitions and include process instances
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// Verify all process instances are also suspended
		foreach (IProcessInstance processInstance in runtimeService.CreateProcessInstanceQuery().ToList())
		{
		  Assert.True(processInstance.IsSuspended);
		}

		// Verify all process instances can't be continued
		foreach (ITask task in taskService.CreateTaskQuery().ToList())
		{
		  try
		  {
			Assert.True(task.Suspended);
			taskService.Complete(task.Id);
			Assert.Fail("A suspended task shouldn't be able to be continued");
		  }
		  catch (SuspendedEntityInteractionException e)
		  {
			AssertTextPresentIgnoreCase("is suspended", e.Message);
		  }
		}
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Activate the process definition again
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// Verify that all process instances can be completed
		foreach (ITask task in taskService.CreateTaskQuery().ToList())
		{
		  taskService.Complete(task.Id);
		}
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testSubmitStartFormAfterProcessDefinitionSuspend()
	   [Test]   public virtual void testSubmitStartFormAfterProcessDefinitionSuspend()
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		try
		{
		  formService.SubmitStartFormData(processDefinition.Id, new Dictionary<string, string>());
		  Assert.Fail();
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}

		try
		{
		  formService.SubmitStartFormData(processDefinition.Id, "someKey", new Dictionary<string, string>());
		  Assert.Fail();
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @IDeployment public void testJobIsExecutedOnProcessDefinitionSuspend()
	   [Test]   public virtual void testJobIsExecutedOnProcessDefinitionSuspend()
	  {

		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;

		// Suspending the process definition should not stop the execution of jobs
		// Added this test because in previous implementations, this was the case.
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);
		Assert.AreEqual(1, managementService.CreateJobQuery().Count());

		// The jobs should simply be executed
		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);
		Assert.AreEqual(0, managementService.CreateJobQuery().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDelayedSuspendProcessDefinition()
	   [Test]   public virtual void testDelayedSuspendProcessDefinition()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		// Suspend process definition in one week from now
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, false, new DateTime(oneWeekFromStartTime));

		// Verify we can just start process instances
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// execute job
		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		// Try to start process instance. It should Assert.Fail now.
		try
		{
		  runtimeService.StartProcessInstanceById(processDefinition.Id);
		  Assert.Fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("suspended", e.Message);
		}
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Activate again
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id);
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDelayedSuspendProcessDefinitionIncludingProcessInstances()
	   [Test]   public virtual void testDelayedSuspendProcessDefinitionIncludingProcessInstances()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		// Start some process instances
		int nrOfProcessInstances = 30;
		for (int i = 0; i < nrOfProcessInstances; i++)
		{
		  runtimeService.StartProcessInstanceById(processDefinition.Id);
		}

		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery().Count());//.Suspended().Count());
		Assert.AreEqual(nrOfProcessInstances, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Suspend process definition in one week from now
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, new DateTime(oneWeekFromStartTime));

		// Verify we can start process instances
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		nrOfProcessInstances = nrOfProcessInstances + 1;
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());

		// execute job
		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		// Try to start process instance. It should Assert.Fail now.
		try
		{
		  runtimeService.StartProcessInstanceById(processDefinition.Id);
		  Assert.Fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("suspended", e.Message);
		}
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(nrOfProcessInstances, taskService.CreateTaskQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Activate again
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, true, DateTime.Now);
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(nrOfProcessInstances, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, taskService.CreateTaskQuery().Count());//.Suspended().Count());
		Assert.AreEqual(nrOfProcessInstances, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDelayedActivateProcessDefinition()
	   [Test]   public virtual void testDelayedActivateProcessDefinition()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		// Try to start process instance. It should Assert.Fail now.
		try
		{
		  runtimeService.StartProcessInstanceById(processDefinition.Id);
		  Assert.Fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("suspended", e.Message);
		}
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Activate in a day from now
		long oneDayFromStart = startTime.Ticks + (24 * 60 * 60 * 1000);
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, false, new DateTime(oneDayFromStart));

		// execute job
		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		// Starting a process instance should now succeed
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
	  }

	   [Test]   public virtual void testSuspendMultipleProcessDefinitionsByKey()
	  {

		// Deploy three processes
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/runtime/oneTaskProcess.bpmn20.xml").Deploy();
		}
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Suspend all process definitions with same key
		repositoryService.SuspendProcessDefinitionByKey("oneTaskProcess");
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Activate again
		repositoryService.ActivateProcessDefinitionByKey("oneTaskProcess");
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Start process instance
		runtimeService.StartProcessInstanceByKey("oneTaskProcess");

		// And suspend again, cascading to process instances
		repositoryService.SuspendProcessDefinitionByKey("oneTaskProcess", true, DateTime.Now);
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());
		Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testDelayedSuspendMultipleProcessDefinitionsByKey()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// Deploy five versions of the same process
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/runtime/oneTaskProcess.bpmn20.xml").Deploy();
		}
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		// Start process instance
		runtimeService.StartProcessInstanceByKey("oneTaskProcess");

		// Suspend all process definitions with same key in 2 hours from now
		repositoryService.SuspendProcessDefinitionByKey("oneTaskProcess", true, new DateTime(startTime.Ticks + (2 * hourInMs)));
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// execute job
		IJob job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());

		// Activate again in 5 hourse from now
		repositoryService.ActivateProcessDefinitionByKey("oneTaskProcess", true, new DateTime(startTime.Ticks + (5 * hourInMs)));
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());//.Suspended().Count());

		// execute job
		job = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(job.Id);

		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery().Count());
		Assert.AreEqual(nrOfProcessDefinitions, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());
		Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendById_shouldSuspendJobDefinitionAndRetainJob()
	   [Test]   public virtual void testSuspendById_shouldSuspendJobDefinitionAndRetainJob()
	  {
		// given

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendByKey_shouldSuspendJobDefinitionAndRetainJob()
	   [Test]   public virtual void testSuspendByKey_shouldSuspendJobDefinitionAndRetainJob()
	  {
		// given

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	   [Test]   public virtual void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, false, DateTime.Now);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	   [Test]   public virtual void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key, false, DateTime.Now);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	   [Test]   public virtual void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be suspended too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	   [Test]   public virtual void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key, true, DateTime.Now);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be suspended too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	   [Test]   public virtual void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// the job is still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(2, jobQuery.Count());//.Active().Count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still active
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	   [Test]   public virtual void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// the job is still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(2, jobQuery.Count());//.Active().Count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still active
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	   [Test]   public virtual void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// the job is still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(2, jobQuery.Count());//.Active().Count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still active
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	   [Test]   public virtual void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.SuspendProcessDefinitionByKey(processDefinition.Key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// the job is still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(2, jobQuery.Count());//.Active().Count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still active
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

	   [Test]   public virtual void testMultipleSuspendByKey_shouldSuspendJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key);

		// then
		// the job definitions should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Active().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, false, DateTime.Now);

		// then
		// the job definitions should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Active().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		// then
		// the job definitions should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be suspended too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testDelayedMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to suspend the process definition delayed
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definitions should be still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());

		// ..and the corresponding jobs should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(6, jobQuery.Count());//.Active().Count());

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definitions should be suspended..
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be still active
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Active().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testDelayedMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// when
		// the process definition will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to suspend the process definition delayed
		IJob timerToSuspendProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToSuspendProcessDefinition);

		// the job definitions should be still active
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());

		// ..and the corresponding jobs should be still active
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(6, jobQuery.Count());//.Active().Count());

		// when
		// execute job
		managementService.ExecuteJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definitions should be suspended..
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be suspended too
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationById_shouldActivateJobDefinitionAndRetainJob()
	   [Test]   public virtual void testActivationById_shouldActivateJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id);

		// then
		// the job definition should be active..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// ..and the corresponding job should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationByKey_shouldActivateJobDefinitionAndRetainJob()
	   [Test]   public virtual void testActivationByKey_shouldActivateJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key);

		// then
		// the job definition should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// ..and the corresponding job should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationByIdAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	   [Test]   public virtual void testActivationByIdAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, false, DateTime.Now);

		// then
		// the job definition should be suspended..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// ..and the corresponding job should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationByKeyAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	   [Test]   public virtual void testActivationByKeyAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key, false, DateTime.Now);

		// then
		// the job definition should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// ..and the corresponding job should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	   [Test]   public virtual void testActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// then
		// the job definition should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// ..and the corresponding job should be activated too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	   [Test]   public virtual void testActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key, true, DateTime.Now);

		// then
		// the job definition should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.IsFalse(suspendedJobDefinition.Suspended);

		// ..and the corresponding job should be activated too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	   [Test]   public virtual void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count()); // the timer job is active

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be active
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activeJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
		Assert.IsFalse(activeJobDefinition.Suspended);

		// the job is still suspended
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	   [Test]   public virtual void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count()); // the timer job is active

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be suspended
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// the job is still suspended
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());

		IJob job = jobQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.True(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	   [Test]   public virtual void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.ActivateProcessDefinitionById(processDefinition.Id, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count()); // the timer job is active

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be activated
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// the job is activated
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn"}) public void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	   [Test]   public virtual void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["Assert.Fail"] = true;
		runtimeService.StartProcessInstanceById(processDefinition.Id, @params as IDictionary<string, ITypedValue>);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(1, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.ActivateProcessDefinitionByKey(processDefinition.Key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Suspended().Count());

		IJobDefinition suspendedJobDefinition = jobDefinitionQuery.First();//.Suspended().First();

		Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
		Assert.True(suspendedJobDefinition.Suspended);

		// the job is still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(1, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count()); // the timer job is active

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be activated
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobDefinitionQuery.Count());//.Active().Count());

		IJobDefinition activatedJobDefinition = jobDefinitionQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, activatedJobDefinition.Id);
		Assert.IsFalse(activatedJobDefinition.Suspended);

		// the job is activated too
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		IJob job = jobQuery.First();//.Active().First();

		Assert.AreEqual(jobDefinition.Id, job.JobDefinitionId);
		Assert.IsFalse(job.Suspended);
	  }

	   [Test]   public virtual void testMultipleActivationByKey_shouldActivateJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(key);

		// then
		// the job definitions should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());

		// ..and the corresponding jobs should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(key, false, DateTime.Now);

		// then
		// the job definitions should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndJob()
	  {

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(key, true, DateTime.Now);

		// then
		// the job definitions should be activated..
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be activated too
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(5, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testDelayedMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to activate the process definition delayed
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definitions should be still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());

		// ..and the corresponding jobs should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definitions should be activated..
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be still suspended
		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(0, jobQuery.Count());//.Active().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	   [Test]   public virtual void testDelayedMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/ProcessDefinitionSuspensionTest.TestWithOneAsyncServiceTask.bpmn").Deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["Assert.Fail"] = true;
		  runtimeService.StartProcessInstanceByKey(key, @params as IDictionary<string, ITypedValue>);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.SuspendProcessDefinitionByKey(key, true, DateTime.Now);

		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, repositoryService.CreateProcessDefinitionQuery().Count());//.Suspended().Count());

		Assert.AreEqual(0, managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
		Assert.AreEqual(5, managementService.CreateJobDefinitionQuery().Count());//.Suspended().Count());

		// when
		// the process definition will be activated
		repositoryService.ActivateProcessDefinitionByKey(key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to activate the process definition delayed
		IJob timerToActivateProcessDefinition = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.NotNull(timerToActivateProcessDefinition);

		// the job definitions should be still suspended
		IQueryable<IJobDefinition> jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Suspended().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Active().Count());

		// ..and the corresponding jobs should be still suspended
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

		Assert.AreEqual(5, jobQuery.Count());//.Suspended().Count());
		Assert.AreEqual(1, jobQuery.Count());//.Active().Count());

		// when
		// execute job
		managementService.ExecuteJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definitions should be activated..
		Assert.AreEqual(5, jobDefinitionQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobDefinitionQuery.Count());//.Suspended().Count());

		// ..and the corresponding jobs should be activated too
		Assert.AreEqual(5, jobQuery.Count());//.Active().Count());
		Assert.AreEqual(0, jobQuery.Count());//.Suspended().Count());

		// Clean DB
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }


//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"resources/api/repository/ProcessDefinitionSuspensionTest.TestSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"}) public void testSuspendStartTimerOnProcessDefinitionSuspensionByKey()
	   [Test]   public virtual void testSuspendStartTimerOnProcessDefinitionSuspensionByKey()
	  {
		IJob startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();

		Assert.IsFalse(startTimer.Suspended);

		// when
		repositoryService.SuspendProcessDefinitionByKey("process");

		// then

		// refresh job
		startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.True(startTimer.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"resources/api/repository/ProcessDefinitionSuspensionTest.TestSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"}) public void testSuspendStartTimerOnProcessDefinitionSuspensionById()
	   [Test]   public virtual void testSuspendStartTimerOnProcessDefinitionSuspensionById()
	  {
		IProcessDefinition pd = repositoryService.CreateProcessDefinitionQuery().First();

		IJob startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();

		Assert.IsFalse(startTimer.Suspended);

		// when
		repositoryService.SuspendProcessDefinitionById(pd.Id);

		// then

		// refresh job
		startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.True(startTimer.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"resources/api/repository/ProcessDefinitionSuspensionTest.TestSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"}) public void testActivateStartTimerOnProcessDefinitionSuspensionByKey()
	   [Test]   public virtual void testActivateStartTimerOnProcessDefinitionSuspensionByKey()
	  {
		repositoryService.SuspendProcessDefinitionByKey("process");

		IJob startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.True(startTimer.Suspended);

		// when
		repositoryService.ActivateProcessDefinitionByKey("process");
		// then

		// refresh job
		startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.IsFalse(startTimer.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"resources/api/repository/ProcessDefinitionSuspensionTest.TestSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"}) public void testActivateStartTimerOnProcessDefinitionSuspensionById()
	   [Test]   public virtual void testActivateStartTimerOnProcessDefinitionSuspensionById()
	  {
		IProcessDefinition pd = repositoryService.CreateProcessDefinitionQuery().First();
		repositoryService.SuspendProcessDefinitionById(pd.Id);

		IJob startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();

		Assert.True(startTimer.Suspended);

		// when
		repositoryService.ActivateProcessDefinitionById(pd.Id);

		// then

		// refresh job
		startTimer = managementService.CreateJobQuery().First();///*.Timers()*/.First();
		Assert.IsFalse(startTimer.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testStartBeforeActivityForSuspendProcessDefinition()
	   [Test]   public virtual void testStartBeforeActivityForSuspendProcessDefinition()
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		//start process instance
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();

		// Suspend process definition
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// try to start before activity for suspended processDefinition
		try
		{
		  runtimeService.CreateProcessInstanceModification(processInstance.Id).StartBeforeActivity("theTask").Execute();
		  Assert.Fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testStartAfterActivityForSuspendProcessDefinition()
	   [Test]   public virtual void testStartAfterActivityForSuspendProcessDefinition()
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		//start process instance
		runtimeService.StartProcessInstanceById(processDefinition.Id);
		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();

		// Suspend process definition
		repositoryService.SuspendProcessDefinitionById(processDefinition.Id, true, DateTime.Now);

		// try to start after activity for suspended processDefinition
		try
		{
		  runtimeService.CreateProcessInstanceModification(processInstance.Id).StartAfterActivity("theTask").Execute();
		  Assert.Fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  AssertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testSuspendAndActivateProcessDefinitionByIdUsingBuilder()
	   [Test]   public virtual void testSuspendAndActivateProcessDefinitionByIdUsingBuilder()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		// suspend
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).Suspend();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		// activate
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).Activate();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/repository/processOne.bpmn20.xml"}) public void testSuspendAndActivateProcessDefinitionByKeyUsingBuilder()
	   [Test]   public virtual void testSuspendAndActivateProcessDefinitionByKeyUsingBuilder()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		// suspend
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(processDefinition.Key).Suspend();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		// activate
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(processDefinition.Key).Activate();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDelayedSuspendProcessDefinitionUsingBuilder()
	   [Test]   public virtual void testDelayedSuspendProcessDefinitionUsingBuilder()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		// suspend process definition in one week from now
		long oneWeekFromStartTime = (DateTime.Now).Ticks + (7 * 24 * 60 * 60 * 1000);

		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).ExecutionDate(new DateTime(oneWeekFromStartTime)).Suspend();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		// execute the suspension job
		IJob job = managementService.CreateJobQuery().First();
		Assert.NotNull(job);
		managementService.ExecuteJob(job.Id);

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDelayedActivateProcessDefinitionUsingBuilder()
	   [Test]   public virtual void testDelayedActivateProcessDefinitionUsingBuilder()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		// suspend
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(processDefinition.Key).Suspend();

		// activate process definition in one week from now
		long oneWeekFromStartTime = (DateTime.Now).Ticks + (7 * 24 * 60 * 60 * 1000);

		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).ExecutionDate(new DateTime(oneWeekFromStartTime)).Activate();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		// execute the activation job
		IJob job = managementService.CreateJobQuery().First();
		Assert.NotNull(job);
		managementService.ExecuteJob(job.Id);

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testSuspendAndActivateProcessDefinitionIncludeInstancesUsingBuilder()
	   [Test]   public virtual void testSuspendAndActivateProcessDefinitionIncludeInstancesUsingBuilder()
	  {

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		IProcessInstance processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

		Assert.IsFalse(processDefinition.Suspended);
		Assert.IsFalse(processInstance.IsSuspended);

		// suspend
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).IncludeProcessInstances(true).Suspend();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.True(processDefinition.Suspended);

		processInstance = runtimeService.CreateProcessInstanceQuery().First();
		Assert.True(processInstance.IsSuspended);

		// activate
		repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).IncludeProcessInstances(true).Activate();

		processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.IsFalse(processDefinition.Suspended);

		processInstance = runtimeService.CreateProcessInstanceQuery().First();
		Assert.IsFalse(processInstance.IsSuspended);
	  }

	}

}