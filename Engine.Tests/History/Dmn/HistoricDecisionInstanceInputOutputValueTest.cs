using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History.Dmn
{


    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class)

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricDecisionInstanceInputOutputValueTest
	{

	  protected internal const string DECISION_PROCESS = "resources/history/HistoricDecisionInstanceTest.ProcessWithBusinessRuleTask.bpmn20.xml";
	  protected internal const string DECISION_SINGLE_OUTPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: input({0}) = {1}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return (new object[][]
		{
			new object[] {"string", "a"},
			new object[] {"long", 1L},
			new object[] {"double", 2.5},
			new object[] {"bytes", "object".GetBytes()},
			//new object[] {"object", new JavaSerializable("foo")},
			//new object[] {"object", Collections.singletonList(new JavaSerializable("bar"))}
		});
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String valueType;
	  public string valueType;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public Object inputValue;
	  public object inputValue;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

        [Test][Deployment( new []{DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void decisionInputInstanceValue()
	  {

		startProcessInstanceAndEvaluateDecision(inputValue);

		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery()/*.IncludeInputs()*/.First();
		IList<IHistoricDecisionInputInstance> inputInstances = historicDecisionInstance.Inputs;
		Assert.That(inputInstances.Count, Is.EqualTo(1));

		IHistoricDecisionInputInstance inputInstance = inputInstances[0];
		Assert.That(inputInstance.TypeName, Is.EqualTo(valueType));
		Assert.That(inputInstance.Value, Is.EqualTo(inputValue));
	  }

        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
        public virtual void decisionOutputInstanceValue()
	  {

		startProcessInstanceAndEvaluateDecision(inputValue);

		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery()/*.IncludeOutputs()*/.First();
		IList<IHistoricDecisionOutputInstance> outputInstances = historicDecisionInstance.Outputs;
		Assert.That(outputInstances.Count, Is.EqualTo(1));

		IHistoricDecisionOutputInstance outputInstance = outputInstances[0];
		Assert.That(outputInstance.TypeName, Is.EqualTo(valueType));
		Assert.That(outputInstance.Value, Is.EqualTo(inputValue));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceAndEvaluateDecision(object input)
	  {
		return engineRule.RuntimeService.StartProcessInstanceByKey("testProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("input1", input));
	  }

	}

}