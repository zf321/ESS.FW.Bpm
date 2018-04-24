using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History.Dmn
{



    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class)

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricDecisionInstanceDecisionServiceEvaluationTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDecisionInstanceDecisionServiceEvaluationTest()
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
		}


	  protected internal const string DECISION_PROCESS_WITH_DECISION_SERVICE = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideDelegation.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_START_LISTENER = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideStartListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_END_LISTENER = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideEndListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_TAKE_LISTENER = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideTakeListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_INSIDE_EXPRESSION = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideExpression.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_INSIDE_DELEGATE_EXPRESSION = "resources/history/HistoricDecisionInstanceTest.TestDecisionEvaluatedWithDecisionServiceInsideDelegateExpression.bpmn20.xml";

	  protected internal const string DECISION_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml";

	  protected internal const string DECISION_DEFINITION_KEY = "testDecision";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return (new object[][]
		{
			new object[] {DECISION_PROCESS_WITH_DECISION_SERVICE, "task"},
			new object[] {DECISION_PROCESS_WITH_START_LISTENER, "task"},
			new object[] {DECISION_PROCESS_WITH_END_LISTENER, "task"},
			new object[] {DECISION_PROCESS_INSIDE_EXPRESSION, "task"},
			new object[] {DECISION_PROCESS_INSIDE_DELEGATE_EXPRESSION, "task"},
			new object[] {DECISION_PROCESS_WITH_TAKE_LISTENER, "start"}
		});
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String process;
	  public string process;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public String activityId;
	  public string activityId;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public util.ProcessEngineTestRule testRule = new util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testRule;

	  protected internal IRuntimeService runtimeService;
	  protected internal IRepositoryService repositoryService;
	  protected internal IHistoryService historyService;

      [SetUp]
	  public virtual void init()
	  {
		//testRule.Deploy(DECISION_DMN, process);

		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
	  }

      [Test]
	  public virtual void evaluateDecisionWithDecisionService()
	  {

		runtimeService.StartProcessInstanceByKey("testProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("input1", null).PutValue("myBean", new DecisionServiceDelegate()));

		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Id ==processInstance.ProcessDefinitionId).First();
		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		// references to process instance should be set since the decision is evaluated while executing a process instance
		Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(processDefinition.Key));
		Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(processDefinition.Id));
		Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(processInstance.Id));
		Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo(activityId));
		Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
	  }

	}

}