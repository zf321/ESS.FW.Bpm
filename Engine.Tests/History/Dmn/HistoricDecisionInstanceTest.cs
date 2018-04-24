using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;


namespace Engine.Tests.History.Dmn
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricDecisionInstanceTest : PluggableProcessEngineTestCase
	{

	  public const string DECISION_CASE = "resources/history/HistoricDecisionInstanceTest.CaseWithDecisionTask.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE = "resources/history/HistoricDecisionInstanceTest.TestCaseDecisionEvaluatedWithDecisionServiceInsideDelegate.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_RULE = "resources/history/HistoricDecisionInstanceTest.TestManualActivationRuleEvaluatesDecision.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_IF_PART = "resources/history/HistoricDecisionInstanceTest.TestIfPartEvaluatesDecision.cmmn";

	  public const string DECISION_PROCESS = "resources/history/HistoricDecisionInstanceTest.ProcessWithBusinessRuleTask.bpmn20.xml";

	  public const string DECISION_SINGLE_OUTPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml";
	  public const string DECISION_MULTIPLE_OUTPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionMultipleOutput.Dmn11.xml";
	  public const string DECISION_COMPOUND_OUTPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionCompoundOutput.Dmn11.xml";
	  public const string DECISION_MULTIPLE_INPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionMultipleInput.Dmn11.xml";
	  public const string DECISION_COLLECT_SUM_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionCollectSum.Dmn11.xml";
	  public const string DECISION_RETURNS_TRUE = "resources/history/HistoricDecisionInstanceTest.ReturnsTrue.Dmn11.xml";

	  public const string DECISION_LITERAL_EXPRESSION_DMN = "resources/api/dmn/DecisionWithLiteralExpression.Dmn";

	  public const string DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

	  public const string DECISION_DEFINITION_KEY = "testDecision";


        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDecisionInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Id ==processInstance.ProcessDefinitionId).First();
		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key==DECISION_DEFINITION_KEY).First().Id;
		string activityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "task").First().Id;

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

            Assert.That(historicDecisionInstance, Is.Not.Null);
            Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(processDefinition.Key));
		Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(processDefinition.Id));

		Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(processInstance.Id));

		Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(null));

		Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(null));

		Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo("task"));
		Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(activityInstanceId));

		Assert.That(historicDecisionInstance.RootDecisionInstanceId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.DecisionRequirementsDefinitionId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.DecisionRequirementsDefinitionKey, Is.EqualTo(null));

            Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
        }

        [Test][Deployment( new []{ DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testCaseDecisionInstanceProperties()
	  {

		ICaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		//ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery(c=>c.CaseDefinitionId== caseInstance.CaseDefinitionId).First();

		//string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

		//string activityInstanceId = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_DecisionTask_1").First().Id;

		//IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

  //          Assert.That(historicDecisionInstance, Is.Not.Null);
  //          Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		//Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		//Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		//Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(null));

		//Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(caseDefinition.Key));
		//Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(caseDefinition.Id));
		//Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(caseInstance.Id));

		//Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo("PI_DecisionTask_1"));
		//Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(activityInstanceId));

  //          Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
        }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDecisionInputInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*/*.IncludeInputs()*/.First();
		IList<IHistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		//Assert.That(inputs, Is.Not.Null);
		Assert.That(inputs.Count, Is.EqualTo(1));

		IHistoricDecisionInputInstance input = inputs[0];
		Assert.That(input.DecisionInstanceId, Is.EqualTo(historicDecisionInstance.Id));
		Assert.That(input.ClauseId, Is.EqualTo("in"));
		Assert.That(input.ClauseName, Is.EqualTo("input"));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testMultipleDecisionInstances()
	  {

		startProcessInstanceAndEvaluateDecision("a");
		waitASignificantAmountOfTime();
		startProcessInstanceAndEvaluateDecision("b");

		IList<IHistoricDecisionInstance> historicDecisionInstances = historyService.CreateHistoricDecisionInstanceQuery()/*/*.IncludeInputs().OrderByEvaluationTime()/*.Asc()*/.ToList();
		Assert.That(historicDecisionInstances.Count, Is.EqualTo(2));

		IList<IHistoricDecisionInputInstance> inputsOfFirstDecision = historicDecisionInstances[0].Inputs;
		Assert.That(inputsOfFirstDecision.Count, Is.EqualTo(1));
		Assert.That(inputsOfFirstDecision[0].Value, Is.EqualTo((object) "a"));

		IList<IHistoricDecisionInputInstance> inputsOfSecondDecision = historicDecisionInstances[1].Inputs;
		Assert.That(inputsOfSecondDecision.Count, Is.EqualTo(1));
		Assert.That(inputsOfSecondDecision[0].Value, Is.EqualTo((object) "b"));
	  }

        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_MULTIPLE_INPUT_DMN}) ]
        public virtual void testMultipleDecisionInputInstances()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "a";
		variables["input2"] = 1;
		runtimeService.StartProcessInstanceByKey("testProcess", variables);

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*/*.IncludeInputs()*/.First();
		IList<IHistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		Assert.That(inputs.Count, Is.EqualTo(2));

		Assert.That(inputs[0].Value, Is.EqualTo((object) "a"));
		Assert.That(inputs[1].Value, Is.EqualTo((object) 1));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDisableDecisionInputInstanceByteValue()
	  {

		byte[] bytes = "object".GetBytes();
		startProcessInstanceAndEvaluateDecision(bytes);

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*/*.IncludeInputs()*//*.DisableBinaryFetching()*/.First();
		IList<IHistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		Assert.That(inputs.Count, Is.EqualTo(1));

		IHistoricDecisionInputInstance input = inputs[0];
		Assert.That(input.TypeName, Is.EqualTo("bytes"));
		Assert.That(input.Value, Is.EqualTo(null));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDecisionOutputInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*.IncludeOutputs()*/.First();
		IList<IHistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
            Assert.That(outputs, Is.Not.Null);
            Assert.That(outputs.Count, Is.EqualTo(1));

		IHistoricDecisionOutputInstance output = outputs[0];
		Assert.That(output.DecisionInstanceId, Is.EqualTo(historicDecisionInstance.Id));
		Assert.That(output.ClauseId, Is.EqualTo("out"));
		Assert.That(output.ClauseName, Is.EqualTo("output"));

		Assert.That(output.RuleId, Is.EqualTo("rule"));
		Assert.That(output.RuleOrder, Is.EqualTo(1));

		Assert.That(output.VariableName, Is.EqualTo("result"));
	  }

        [Test][Deployment( new [] { DECISION_PROCESS, DECISION_MULTIPLE_OUTPUT_DMN })]
        public virtual void testMultipleDecisionOutputInstances()
	  {

		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*.IncludeOutputs()*/.First();
		IList<IHistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		Assert.That(outputs.Count, Is.EqualTo(2));

		IHistoricDecisionOutputInstance firstOutput = outputs[0];
		Assert.That(firstOutput.ClauseId, Is.EqualTo("out1"));
		Assert.That(firstOutput.RuleId, Is.EqualTo("rule1"));
		Assert.That(firstOutput.RuleOrder, Is.EqualTo(1));
		Assert.That(firstOutput.VariableName, Is.EqualTo("result1"));
		Assert.That(firstOutput.Value, Is.EqualTo((object) "okay"));

		IHistoricDecisionOutputInstance secondOutput = outputs[1];
		Assert.That(secondOutput.ClauseId, Is.EqualTo("out1"));
		Assert.That(secondOutput.RuleId, Is.EqualTo("rule2"));
		Assert.That(secondOutput.RuleOrder, Is.EqualTo(2));
		Assert.That(secondOutput.VariableName, Is.EqualTo("result1"));
		Assert.That(secondOutput.Value, Is.EqualTo((object) "not okay"));
	  }

        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_COMPOUND_OUTPUT_DMN }) ]
        public virtual void testCompoundDecisionOutputInstances()
	  {

		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery()/*.IncludeOutputs()*/.First();
		IList<IHistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		Assert.That(outputs.Count, Is.EqualTo(2));

		IHistoricDecisionOutputInstance firstOutput = outputs[0];
		Assert.That(firstOutput.ClauseId, Is.EqualTo("out1"));
		Assert.That(firstOutput.RuleId, Is.EqualTo("rule1"));
		Assert.That(firstOutput.RuleOrder, Is.EqualTo(1));
		Assert.That(firstOutput.VariableName, Is.EqualTo("result1"));
		Assert.That(firstOutput.Value, Is.EqualTo((object) "okay"));

		IHistoricDecisionOutputInstance secondOutput = outputs[1];
		Assert.That(secondOutput.ClauseId, Is.EqualTo("out2"));
		Assert.That(secondOutput.RuleId, Is.EqualTo("rule1"));
		Assert.That(secondOutput.RuleOrder, Is.EqualTo(1));
		Assert.That(secondOutput.VariableName, Is.EqualTo("result2"));
		Assert.That(secondOutput.Value, Is.EqualTo((object) "not okay"));
	  }

        [Test][Deployment( new [] { DECISION_PROCESS, DECISION_COLLECT_SUM_DMN }) ]
        public virtual void testCollectResultValue()
	  {

		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance.CollectResultValue, Is.Not.Null);
		Assert.That(historicDecisionInstance.CollectResultValue, Is.EqualTo(3.0));
	  }

        [Test][Deployment(DECISION_LITERAL_EXPRESSION_DMN)]
        public virtual void testDecisionInstancePropertiesOfDecisionLiteralExpression()
	  {
		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

		decisionService.EvaluateDecisionByKey("decision").Variables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("sum", 2205)).Evaluate();

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery()/*/*.IncludeInputs()*//*.IncludeOutputs()*/;
		Assert.That(query.Count(), Is.EqualTo(1L));

		IHistoricDecisionInstance historicDecisionInstance = query.First();

		Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinition.Id));
		Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo("decision"));
		Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("Decision with Literal Expression"));
            Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);

            Assert.That(historicDecisionInstance.Inputs.Count, Is.EqualTo(0));

		IList<IHistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		Assert.That(outputs.Count, Is.EqualTo(1));

		IHistoricDecisionOutputInstance output = outputs[0];
		Assert.That(output.VariableName, Is.EqualTo("result"));
		Assert.That(output.TypeName, Is.EqualTo("string"));
		Assert.That((string) output.Value, Is.EqualTo("ok"));

		Assert.That(output.ClauseId, Is.EqualTo(null));
		Assert.That(output.ClauseName, Is.EqualTo(null));
		Assert.That(output.RuleId, Is.EqualTo(null));
		Assert.That(output.RuleOrder, Is.EqualTo(null));
	  }

        [Test][Deployment( DRG_DMN)]
        public virtual void testDecisionInstancePropertiesOfDrdDecision()
	  {

		decisionService.EvaluateDecisionTableByKey("dish-decision").SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

		IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));

		//IHistoricDecisionInstance rootHistoricDecisionInstance = query.DecisionDefinitionKey("dish-decision").First();
		//IHistoricDecisionInstance requiredHistoricDecisionInstance1 = query.DecisionDefinitionKey("season").First();
		//IHistoricDecisionInstance requiredHistoricDecisionInstance2 = query.DecisionDefinitionKey("guestCount").First();

		//Assert.That(rootHistoricDecisionInstance.RootDecisionInstanceId, Is.EqualTo(null));
		//Assert.That(rootHistoricDecisionInstance.DecisionRequirementsDefinitionId, Is.EqualTo(decisionRequirementsDefinition.Id));
		//Assert.That(rootHistoricDecisionInstance.DecisionRequirementsDefinitionKey, Is.EqualTo(decisionRequirementsDefinition.Key));

		//Assert.That(requiredHistoricDecisionInstance1.RootDecisionInstanceId, Is.EqualTo(rootHistoricDecisionInstance.Id));
		//Assert.That(requiredHistoricDecisionInstance1.DecisionRequirementsDefinitionId, Is.EqualTo(decisionRequirementsDefinition.Id));
		//Assert.That(requiredHistoricDecisionInstance1.DecisionRequirementsDefinitionKey, Is.EqualTo(decisionRequirementsDefinition.Key));

		//Assert.That(requiredHistoricDecisionInstance2.RootDecisionInstanceId, Is.EqualTo(rootHistoricDecisionInstance.Id));
		//Assert.That(requiredHistoricDecisionInstance2.DecisionRequirementsDefinitionId, Is.EqualTo(decisionRequirementsDefinition.Id));
		//Assert.That(requiredHistoricDecisionInstance2.DecisionRequirementsDefinitionKey, Is.EqualTo(decisionRequirementsDefinition.Key));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDeleteHistoricDecisionInstances()
	  {
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionDefinitionKey== DECISION_DEFINITION_KEY);

		startProcessInstanceAndEvaluateDecision();

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();
		historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinition.Id);

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDeleteHistoricDecisionInstanceByInstanceId()
	  {

		// given
		startProcessInstanceAndEvaluateDecision();
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionDefinitionKey== DECISION_DEFINITION_KEY);

		Assert.That(query.Count(), Is.EqualTo(1L));
		IHistoricDecisionInstance historicDecisionInstance = query/*/*.IncludeInputs()*//*.IncludeOutputs()*/.First();

		// when
		historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		// then
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	  public virtual void testDeleteHistoricDecisionInstanceByUndeployment()
	  {
		string firstDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(DECISION_PROCESS).AddClasspathResource(DECISION_SINGLE_OUTPUT_DMN).Deploy().Id;

		startProcessInstanceAndEvaluateDecision();

		string secondDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(DECISION_PROCESS).AddClasspathResource(DECISION_MULTIPLE_OUTPUT_DMN).Deploy().Id;

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));

		repositoryService.DeleteDeployment(secondDeploymentId, true);
		Assert.That(query.Count(), Is.EqualTo(1L));

		repositoryService.DeleteDeployment(firstDeploymentId, true);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test][Deployment(  DECISION_SINGLE_OUTPUT_DMN ) ]
        public virtual void testDecisionEvaluatedWithDecisionService()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "test";
		decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
		// references to process instance should be null since the decision is not evaluated while executing a process instance
		Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(null));
		// the IUser should be null since no IUser was authenticated during evaluation
		Assert.That(historicDecisionInstance.UserId, Is.EqualTo(null));
	  }

        [Test][Deployment(  DECISION_SINGLE_OUTPUT_DMN ) ]
        public virtual void testDecisionEvaluatedWithAuthenticatedUser()
	  {
		identityService.AuthenticatedUserId = "demo";
		IVariableMap variables = Variables.PutValue("input1", "test");
		decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		// the IUser should be set since the decision was evaluated with the decision service
		Assert.That(historicDecisionInstance.UserId, Is.EqualTo("demo"));
	  }

        [Test]
        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDecisionEvaluatedWithAuthenticatedUserFromProcess()
	  {
		identityService.AuthenticatedUserId = "demo";
		startProcessInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		// the IUser should be null since the decision was evaluated by the process
		Assert.That(historicDecisionInstance.UserId, Is.EqualTo(null));
	  }

        [Test][Deployment( new []{ DECISION_CASE_WITH_DECISION_SERVICE, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testDecisionEvaluatedWithAuthenticatedUserFromCase()
	  {
		identityService.AuthenticatedUserId = "demo";
		createCaseInstanceAndEvaluateDecision();

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		// the IUser should be null since decision was evaluated by the case
		Assert.That(historicDecisionInstance.UserId, Is.EqualTo(null));
	  }

        [Test][Deployment( new []{ DECISION_CASE_WITH_DECISION_SERVICE, DECISION_SINGLE_OUTPUT_DMN })]
        public virtual void testCaseDecisionEvaluatedWithDecisionServiceInsideDelegate()
	  {

		ICaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		//ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery(c=>c.CaseDefinitionId== caseInstance.CaseDefinitionId).First();

		//string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

		string activityInstanceId = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First().Id;

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		//Assert.That(historicDecisionInstance, Is.Not.Null);
		//Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		//Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		//Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		//// references to case instance should be set since the decision is evaluated while executing a case instance
		//Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(caseDefinition.Key));
		//Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(caseDefinition.Id));
		//Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(caseInstance.Id));
		//Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo("PI_HumanTask_1"));
		//Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(activityInstanceId));
		//Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
	  }

        [Test][Deployment( new string[] { DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_RULE, DECISION_RETURNS_TRUE })]
        public virtual void testManualActivationRuleEvaluatesDecision()
	  {

		ICaseInstance caseInstance = caseService.WithCaseDefinitionByKey("case").SetVariable("input1", null).SetVariable("myBean", new DecisionServiceDelegate()).Create();

		//ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery(c=>c.CaseDefinitionId== caseInstance.CaseDefinitionId).First();

		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.DecisionRequirementsDefinitionKey== DECISION_DEFINITION_KEY).First().Id;

		string activityInstanceId = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First().Id;

		IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		Assert.That(historicDecisionInstance, Is.Not.Null);
		Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		// references to case instance should be set since the decision is evaluated while executing a case instance
		Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(null));
		Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(caseDefinition.Key));
		//Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(caseDefinition.Id));
		Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(caseInstance.Id));
		Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo("PI_HumanTask_1"));
		Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(activityInstanceId));
		Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
	  }

        [Test][Deployment( new string[]{ DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_IF_PART, DECISION_RETURNS_TRUE }) ]
        public virtual void testIfPartEvaluatesDecision()
	  {

		ICaseInstance caseInstance = caseService.WithCaseDefinitionByKey("case").SetVariable("input1", null).SetVariable("myBean", new DecisionServiceDelegate()).Create();

		string humanTask1 = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;
		caseService.CompleteCaseExecution(humanTask1);

		//ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery(c=>c.CaseDefinitionId== caseInstance.CaseDefinitionId).First();

		//string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

		//string activityInstanceId = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First().Id;

		//IHistoricDecisionInstance historicDecisionInstance = historyService.CreateHistoricDecisionInstanceQuery().First();

		////Assert.That(historicDecisionInstance, Is.Not.Null);
		//Assert.That(historicDecisionInstance.DecisionDefinitionId, Is.EqualTo(decisionDefinitionId));
		//Assert.That(historicDecisionInstance.DecisionDefinitionKey, Is.EqualTo(DECISION_DEFINITION_KEY));
		//Assert.That(historicDecisionInstance.DecisionDefinitionName, Is.EqualTo("sample decision"));

		//// references to case instance should be set since the decision is evaluated while executing a case instance
		//Assert.That(historicDecisionInstance.ProcessDefinitionKey, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessDefinitionId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.ProcessInstanceId, Is.EqualTo(null));
		//Assert.That(historicDecisionInstance.CaseDefinitionKey, Is.EqualTo(caseDefinition.Key));
		//Assert.That(historicDecisionInstance.CaseDefinitionId, Is.EqualTo(caseDefinition.Id));
		//Assert.That(historicDecisionInstance.CaseInstanceId, Is.EqualTo(caseInstance.Id));
		//Assert.That(historicDecisionInstance.ActivityId, Is.EqualTo("PI_HumanTask_1"));
		//Assert.That(historicDecisionInstance.ActivityInstanceId, Is.EqualTo(activityInstanceId));
		//Assert.That(historicDecisionInstance.EvaluationTime, Is.Not.Null);
	  }

	  public virtual void testTableNames()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		Assert.That(managementService.GetTableName(typeof(IHistoricDecisionInstance)), Is.EqualTo(tablePrefix + "ACT_HI_DECINST"));

		Assert.That(managementService.GetTableName(typeof(HistoricDecisionInstanceEntity)), Is.EqualTo(tablePrefix + "ACT_HI_DECINST"));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceAndEvaluateDecision()
	  {
		return startProcessInstanceAndEvaluateDecision(null);
	  }

	  protected internal virtual IProcessInstance startProcessInstanceAndEvaluateDecision(object input)
	  {
		return runtimeService.StartProcessInstanceByKey("testProcess", getVariables(input));
	  }

	  protected internal virtual ICaseInstance createCaseInstanceAndEvaluateDecision()
	  {
		return caseService.WithCaseDefinitionByKey("case").SetVariables(getVariables("test")).Create();
	  }

	  protected internal virtual IVariableMap getVariables(object input)
	  {
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
		variables.PutValue("input1", input);
		return variables;
	  }

	  /// <summary>
	  /// Use between two rule evaluations to ensure the expected order by evaluation time.
	  /// </summary>
	  protected internal virtual void waitASignificantAmountOfTime()
	  {
		DateTime now = new DateTime(ClockUtil.CurrentTime.Ticks);
		ClockUtil.CurrentTime = now.AddSeconds(10);
	  }

	}

}