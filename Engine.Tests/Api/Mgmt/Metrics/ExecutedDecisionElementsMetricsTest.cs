using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{

    [TestFixture]
    public class ExecutedDecisionElementsMetricsTest : AbstractMetricsTest
	{

	  public const string DMN_FILE = "resources/api/mgmt/metrics/ExecutedDecisionElementsTest.Dmn11.xml";
	  public static IVariableMap VARIABLES = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "").PutValue("sum", 100);

	  protected internal override void clearMetrics()
	  {
		base.clearMetrics();
		processEngineConfiguration.DmnEngineConfiguration.EngineMetricCollector.ClearExecutedDecisionElements();
	  }

        [Test]
        public virtual void testBusinessRuleTask()
	  {
		IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().BusinessRuleTask("task").EndEvent().Done();

		IBusinessRuleTask task = (IBusinessRuleTask)modelInstance.GetModelElementById/*<IBusinessRuleTask>*/("task");
		task.CamundaDecisionRef = "decision";

		DeploymentId = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).AddClasspathResource(DMN_FILE).Deploy().Id;

		Assert.AreEqual(0l, ExecutedDecisionElements);
		Assert.AreEqual(0l, ExecutedDecisionElementsFromDmnEngine);

		runtimeService.StartProcessInstanceByKey("testProcess", VARIABLES);

		Assert.AreEqual(16l, ExecutedDecisionElements);
		Assert.AreEqual(16l, ExecutedDecisionElementsFromDmnEngine);

		processEngineConfiguration.DbMetricsReporter.ReportNow();

		Assert.AreEqual(16l, ExecutedDecisionElements);
		Assert.AreEqual(16l, ExecutedDecisionElementsFromDmnEngine);
	  }

	  protected internal virtual long ExecutedDecisionElements
	  {
		  get
		  {
			return managementService.CreateMetricsQuery().Name(ESS.FW.Bpm.Engine.Management.Metrics.ExecutedDecisionElements).Sum();
		  }
	  }

	  protected internal virtual long ExecutedDecisionElementsFromDmnEngine
	  {
		  get
		  {
			return processEngineConfiguration.DmnEngineConfiguration.EngineMetricCollector.ExecutedDecisionElements;
		  }
	  }

	}

}