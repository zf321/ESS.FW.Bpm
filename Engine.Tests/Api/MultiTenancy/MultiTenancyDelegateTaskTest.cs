using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;
using AssertingTaskListener = Engine.Tests.Api.Delegate.AssertingTaskListener;

namespace Engine.Tests.Api.MultiTenancy
{
    

    /// <summary>
	/// Tests if a <seealso cref="DelegateTask"/> has the correct tenant-id. The
	/// Assertions are checked inside the task listener.
	/// </summary>
	public class MultiTenancyDelegateTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string BPMN = "resources/api/multitenancy/taskListener.bpmn";

	  public virtual void testSingleExecutionWithUserTask()
	  {
		DeploymentForTenant("tenant1", BPMN);

		AssertingTaskListener.addAsserts(hasTenantId("tenant1"));

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		runtimeService.StartProcessInstanceById(processDefinition.Id);
	  }


	  protected internal static AssertingTaskListener.DelegateTaskAsserter hasTenantId(string expectedTenantId)
	  {
		return new DelegateTaskAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateTaskAsserterAnonymousInnerClass : Delegate.AssertingTaskListener.DelegateTaskAsserter
	  {
		  private string expectedTenantId;

		  public DelegateTaskAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public void doAssert(IDelegateTask task)
		  {
			Assert.That(task.TenantId, Is.EqualTo(expectedTenantId));
		  }
        }

    [TearDown]
	  protected internal void tearDown()
	  {
		AssertingTaskListener.clear();
		base.TearDown();
	  }

	}

}