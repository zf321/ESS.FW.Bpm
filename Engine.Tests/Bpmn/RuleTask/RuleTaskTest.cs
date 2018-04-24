using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.RuleTask
{

	/// <summary>
	/// 
	/// </summary>
	public class RuleTaskTest : PluggableProcessEngineTestCase
	{

        [Test]
        [Deployment]
	  public virtual void testJavaDelegate()
        {
        //var query = commandContext.DecisionDefinitionManager.FindLatestDecisionDefinitionByKey("");
             
        Event.End.DummyServiceTask.wasExecuted = true;
		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("businessRuleTaskJavaDelegate");

		AssertProcessEnded(processInstance.Id);
		Assert.True(Event.End.DummyServiceTask.wasExecuted);
	  }
	}
}