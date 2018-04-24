using System.Linq;
using Engine.Tests.Api.Delegate;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{


    /// <summary>
	/// Tests if a <seealso cref="IDelegateExecution"/> has the correct tenant-id. The
	/// Assertions are checked inside the service tasks.
	/// </summary>
	public class MultiTenancyDelegateExecutionTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

        [Test]
        public virtual void testSingleExecution()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		DeploymentForTenant("tenant1", ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).EndEvent().Done());

		AssertingJavaDelegate.AddAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }

        [Test]
        public virtual void testConcurrentExecution()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		//DeploymentForTenant("tenant1", Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ParallelGateway("fork").ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).ParallelGateway("join").EndEvent().MoveToNode("fork").ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).connectTo("join").Done());

		AssertingJavaDelegate.AddAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }

        [Test]
        public virtual void testEmbeddedSubprocess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		//DeploymentForTenant("tenant1", Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().SubProcess().EmbeddedSubProcess().StartEvent().ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).EndEvent().SubProcessDone().EndEvent().Done());

		AssertingJavaDelegate.AddAsserts(hasTenantId("tenant1"));

		startProcessInstance(PROCESS_DEFINITION_KEY);
	  }


        protected internal virtual void startProcessInstance(string processDefinitionKey)
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==processDefinitionKey)/*.LatestVersion()*/.First();

		runtimeService.StartProcessInstanceById(processDefinition.Id);
	  }

    [TearDown]
	  protected internal void tearDown()
	  {
		AssertingJavaDelegate.Clear();
		base.TearDown();
	  }


	  protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasTenantId(string expectedTenantId)
	  {
		return new DelegateExecutionAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass : AssertingJavaDelegate.DelegateExecutionAsserter
	  {
		  private string expectedTenantId;

		  public DelegateExecutionAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public virtual void DoAssert(IDelegateExecution execution)
		  {
			Assert.That(execution.TenantId, Is.EqualTo(expectedTenantId));
		  }
	  }

	}

}