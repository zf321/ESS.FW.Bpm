//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.History.Dmn
//{




//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]

//    [TestFixture]
//    public class HistoricDecisionInstanceQueryTest : PluggableProcessEngineTestCase
//	{

//	  protected internal const string DECISION_CASE = "resources/history/HistoricDecisionInstanceTest.CaseWithDecisionTask.cmmn";
//	  protected internal const string DECISION_PROCESS = "resources/history/HistoricDecisionInstanceTest.ProcessWithBusinessRuleTask.bpmn20.xml";

//	  protected internal const string DECISION_SINGLE_OUTPUT_DMN = "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml";
//	  protected internal const string DECISION_NO_INPUT_DMN = "resources/history/HistoricDecisionInstanceTest.NoInput.Dmn11.xml";

//	  protected internal const string DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

//	  protected internal const string DECISION_DEFINITION_KEY = "testDecision";
//	  protected internal const string DISH_DECISION = "dish-decision";

//      [Test][Deployment(new [] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//	  public virtual void testQueryIncludeInputsForNonExistingDecision()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery()/*.IncludeInputs()*/;
//		Assert.That(query.First(), Is.EqualTo(null));

//		startProcessInstanceAndEvaluateDecision();

//		Assert.That(query.DecisionInstanceId("nonExisting").First(), Is.EqualTo(null));
//	  }

//        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryIncludeOutputs()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		try
//		{
//		    var historicDecisionOutputInstances = query.First().Outputs;
//		    Assert.Fail("expected exception: output not fetched");
//		}
//		catch (ProcessEngineException)
//		{
//		  // should throw exception if output is not fetched
//		}

//		Assert.That(query/*.IncludeOutputs()*/.First().Outputs.Count, Is.EqualTo(1));
//	  }

//        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryIncludeOutputsForNonExistingDecision()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery()/*.IncludeOutputs()*/;
//		Assert.That(query.First(), Is.EqualTo(null));

//		startProcessInstanceAndEvaluateDecision();

//		Assert.That(query.DecisionInstanceId("nonExisting").First(), Is.EqualTo(null));
//        }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryIncludeInputsNoInput()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query/*.IncludeInputs()*/.First().Inputs.Count, Is.EqualTo(0));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryIncludeOutputsNoInput()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query/*.IncludeOutputs()*/.First().Outputs.Count, Is.EqualTo(0));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryPaging()
//	  {

//		startProcessInstanceAndEvaluateDecision();
//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query/*.ListPage(0, 2)*/.Count, Is.EqualTo(2));
//		Assert.That(query/*.ListPage(1, 1)*/.Count, Is.EqualTo(1));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQuerySortByEvaluationTime()
//	  {

//		startProcessInstanceAndEvaluateDecision();
//		waitASignificantAmountOfTime();
//		startProcessInstanceAndEvaluateDecision();

//	      IList<IHistoricDecisionInstance> orderAsc = historyService.CreateHistoricDecisionInstanceQuery()
//	          .OrderByEvaluationTime()
//	          /*.Asc()*/
//	          .ToList();
//		Assert.That(orderAsc[0].EvaluationTime<(orderAsc[1].EvaluationTime), Is.EqualTo(true));

//	      IList<IHistoricDecisionInstance> orderDesc = historyService.CreateHistoricDecisionInstanceQuery()
//	          .OrderByEvaluationTime()
//	          /*.Desc()*/
//	          .ToList();
//		Assert.That(orderDesc[0].EvaluationTime>(orderDesc[1].EvaluationTime), Is.EqualTo(true));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByDecisionInstanceId()
//	  {
//		IProcessInstance pi1 = startProcessInstanceAndEvaluateDecision();
//		IProcessInstance pi2 = startProcessInstanceAndEvaluateDecision();

//		string decisionInstanceId1 = historyService.CreateHistoricDecisionInstanceQuery(c=>c.ProcessInstanceId == pi1.Id).First().Id;
//		string decisionInstanceId2 = historyService.CreateHistoricDecisionInstanceQuery(c=>c.ProcessInstanceId == pi2.Id).First().Id;

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionInstanceId(decisionInstanceId1).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionInstanceId(decisionInstanceId2).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionInstanceId("unknown").Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByDecisionInstanceIds()
//	  {
//		IProcessInstance pi1 = startProcessInstanceAndEvaluateDecision();
//		IProcessInstance pi2 = startProcessInstanceAndEvaluateDecision();

//		string decisionInstanceId1 = historyService.CreateHistoricDecisionInstanceQuery(c=>c.ProcessInstanceId == pi1.Id).First().Id;
//		string decisionInstanceId2 = historyService.CreateHistoricDecisionInstanceQuery(c=>c.ProcessInstanceId == pi2.Id).First().Id;

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionInstanceIdIn(decisionInstanceId1).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionInstanceIdIn(decisionInstanceId2).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionInstanceIdIn(decisionInstanceId1, decisionInstanceId2).Count(), Is.EqualTo(2L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByDecisionDefinitionId()
//	  {
//		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionDefinitionId(decisionDefinitionId).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionDefinitionId("other id").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment(new [] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN }) ]
//        public virtual void testQueryByDecisionDefinitionIdIn()
//	  {
//		//given
//		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY).First().Id;
//		string decisionDefinitionId2 = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DISH_DECISION).First().Id;

//		//when
//		startProcessInstanceAndEvaluateDecision();
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		//then
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionDefinitionIdIn(decisionDefinitionId, decisionDefinitionId2).Count(), Is.EqualTo(2L));
//		Assert.That(query.DecisionDefinitionIdIn("other id", "anotherFake").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new []{DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN}) ]
//        public virtual void testQueryByInvalidDecisionDefinitionIdIn()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		try
//		{
//		  query.DecisionDefinitionIdIn("aFake", null).Count();
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  //expected
//		}
//	  }

//        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN })]
//        public virtual void testQueryByDecisionDefinitionKeyIn()
//	  {

//		//when
//		startProcessInstanceAndEvaluateDecision();
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		//then
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionDefinitionKeyIn(DISH_DECISION, DECISION_DEFINITION_KEY).Count(), Is.EqualTo(2L));
//		Assert.That(query.DecisionDefinitionKeyIn("other id", "anotherFake").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new []{DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN, DRG_DMN}) ]
//        public virtual void testQueryByInvalidDecisionDefinitionKeyIn()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		try
//		{
//		  query.DecisionDefinitionKeyIn("aFake", null).Count();
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  //expected
//		}
//	  }

//        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByDecisionDefinitionKey()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionDefinitionKey(DECISION_DEFINITION_KEY).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionDefinitionKey("other key").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new string[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByDecisionDefinitionName()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionDefinitionName("sample decision").Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionDefinitionName("other name").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new string[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByProcessDefinitionKey()
//	  {
//		string processDefinitionKey = repositoryService.CreateProcessDefinitionQuery().First().Key;

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Where(c=>c.ProcessDefinitionKey== processDefinitionKey).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.ProcessDefinitionKey== "other process").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new [] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByProcessDefinitionId()
//	  {
//		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Where(c=>c.ProcessDefinitionId==processDefinitionId).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.ProcessDefinitionId=="other process").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new string[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByProcessInstanceId()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		string ProcessInstanceId = runtimeService.CreateProcessInstanceQuery().First().Id;

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Where(c=>c.ProcessInstanceId==ProcessInstanceId).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.ProcessInstanceId=="other process").Count(), Is.EqualTo(0L));
//	  }

//        [Test][Deployment( new []{ DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByActivityId()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.ActivityIdIn("task").Count(), Is.EqualTo(1L));
//		Assert.That(query.ActivityIdIn("other activity").Count(), Is.EqualTo(0L));
//		Assert.That(query.ActivityIdIn("task", "other activity").Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByActivityInstanceId()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		string activityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "task").First().Id;

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		Assert.That(query.ActivityInstanceIdIn(activityInstanceId).Count(), Is.EqualTo(1L));
//		Assert.That(query.ActivityInstanceIdIn("other activity").Count(), Is.EqualTo(0L));
//		Assert.That(query.ActivityInstanceIdIn(activityInstanceId, "other activity").Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByEvaluatedBefore()
//	  {
//		DateTime beforeEvaluated = new DateTime(1441612000);
//		DateTime evaluated = new DateTime(1441613000);
//		DateTime afterEvaluated = new DateTime(1441614000);

//		ClockUtil.CurrentTime = evaluated;
//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		Assert.That(query.EvaluatedBefore(afterEvaluated).Count(), Is.EqualTo(1L));
//		Assert.That(query.EvaluatedBefore(evaluated).Count(), Is.EqualTo(1L));
//		Assert.That(query.EvaluatedBefore(beforeEvaluated).Count(), Is.EqualTo(0L));

//		ClockUtil.Reset();
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByEvaluatedAfter()
//	  {
//		DateTime beforeEvaluated = new DateTime(1441612000);
//		DateTime evaluated = new DateTime(1441613000);
//		DateTime afterEvaluated = new DateTime(1441614000);

//		ClockUtil.CurrentTime = evaluated;
//		startProcessInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		Assert.That(query.EvaluatedAfter(beforeEvaluated).Count(), Is.EqualTo(1L));
//		Assert.That(query.EvaluatedAfter(evaluated).Count(), Is.EqualTo(1L));
//		Assert.That(query.EvaluatedAfter(afterEvaluated).Count(), Is.EqualTo(0L));

//		ClockUtil.Reset();
//	  }

//        [Test][Deployment(new [] { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByCaseDefinitionKey()
//	  {
//		createCaseInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseDefinitionKey("case").Count(), Is.EqualTo(1L));
//	  }

//	  public virtual void testQueryByInvalidCaseDefinitionKey()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseDefinitionKey("invalid").Count(), Is.EqualTo(0L));

//		try
//		{
//		  query.CaseDefinitionKey(null);
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByCaseDefinitionId()
//	  {
//		ICaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseDefinitionId(caseInstance.CaseDefinitionId).Count(), Is.EqualTo(1L));
//	  }

//	  public virtual void testQueryByInvalidCaseDefinitionId()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseDefinitionId("invalid").Count(), Is.EqualTo(0L));

//		try
//		{
//		  query.CaseDefinitionId(null);
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByCaseInstanceId()
//	  {
//		ICaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseInstanceId(caseInstance.Id).Count(), Is.EqualTo(1L));
//	  }

//	  public virtual void testQueryByInvalidCaseInstanceId()
//	  {
//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.CaseInstanceId("invalid").Count(), Is.EqualTo(0L));

//		try
//		{
//		  query.CaseInstanceId(null);
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test][Deployment( new [] { DECISION_SINGLE_OUTPUT_DMN }) ]
//        public virtual void testQueryByUserId()
//	  {
//		evaluateDecisionWithAuthenticatedUser("demo");

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Where(c=>c.UserId=="demo").Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testQueryByInvalidUserId()
//	  {
//		evaluateDecisionWithAuthenticatedUser("demo");

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Where(c=>c.UserId=="dem1").Count(), Is.EqualTo(0L));

//		try
//		{
//		  query.Where(c=>c.UserId==null);
//		  Assert.Fail("exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test][Deployment(  DRG_DMN ) ]
//        public virtual void testQueryByRootDecisionInstanceId()
//	  {
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
//		Assert.That(query.Count(), Is.EqualTo(3L));

//		string rootDecisionInstanceId = query.DecisionDefinitionKey(DISH_DECISION).First().Id;
//		string requiredDecisionInstanceId1 = query.DecisionDefinitionKey("season").First().Id;
//		string requiredDecisionInstanceId2 = query.DecisionDefinitionKey("guestCount").First().Id;

//		query = historyService.CreateHistoricDecisionInstanceQuery();
//		Assert.That(query.RootDecisionInstanceId(rootDecisionInstanceId).Count(), Is.EqualTo(3L));
//		Assert.That(query.RootDecisionInstanceId(requiredDecisionInstanceId1).Count(), Is.EqualTo(0L));
//		Assert.That(query.RootDecisionInstanceId(requiredDecisionInstanceId2).Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        [Deployment(DRG_DMN)]
//        public virtual void testQueryByRootDecisionInstancesOnly()
//	  {
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.Count(), Is.EqualTo(3L));
//		Assert.That(query.RootDecisionInstancesOnly().Count(), Is.EqualTo(1L));
//		Assert.That(query.RootDecisionInstancesOnly().First().DecisionDefinitionKey, Is.EqualTo(DISH_DECISION));
//	  }

//        [Test]
//        [Deployment(DRG_DMN)]
//        public virtual void testQueryByDecisionRequirementsDefinitionId()
//	  {
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionRequirementsDefinitionId("notExisting").Count(), Is.EqualTo(0L));
//		Assert.That(query.DecisionRequirementsDefinitionId(decisionRequirementsDefinition.Id).Count(), Is.EqualTo(3L));
//	  }

//        [Test]
//        [Deployment(DRG_DMN)]
//        public virtual void testQueryByDecisionRequirementsDefinitionKey()
//	  {
//		decisionService.EvaluateDecisionTableByKey(DISH_DECISION).SetVariables(Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

//		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

//		Assert.That(query.DecisionRequirementsDefinitionKey("notExisting").Count(), Is.EqualTo(0L));
//		Assert.That(query.DecisionRequirementsDefinitionKey("dish").Count(), Is.EqualTo(3L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testNativeQuery()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

//		IQueryable<INativeHistoricDecisionInstance> nativeQuery = historyService.CreateNativeHistoricDecisionInstanceQuery().Sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST");

//		Assert.That(nativeQuery.Count, Is.EqualTo(1));

//		IQueryable<INativeHistoricDecisionInstance> nativeQueryWithParameter = historyService.CreateNativeHistoricDecisionInstanceQuery().Sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST H WHERE H.DEC_DEF_KEY_ = #{decisionDefinitionKey}");

//		Assert.That(nativeQueryWithParameter.Parameter("decisionDefinitionKey", DECISION_DEFINITION_KEY).Count, Is.EqualTo(1));
//		Assert.That(nativeQueryWithParameter.Parameter("decisionDefinitionKey", "other decision").Count, Is.EqualTo(0));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testNativeCountQuery()
//	  {

//		startProcessInstanceAndEvaluateDecision();

//		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

//		IQueryable<INativeHistoricDecisionInstance> nativeQuery = historyService.CreateNativeHistoricDecisionInstanceQuery().Sql("SELECT Count(*) FROM " + tablePrefix + "ACT_HI_DECINST");

//		Assert.That(nativeQuery.Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        [Deployment(new[] { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
//        public virtual void testNativeQueryPaging()
//	  {

//		startProcessInstanceAndEvaluateDecision();
//		startProcessInstanceAndEvaluateDecision();

//		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

//		IQueryable<INativeHistoricDecisionInstance> nativeQuery = historyService.CreateNativeHistoricDecisionInstanceQuery().Sql("SELECT * FROM " + tablePrefix + "ACT_HI_DECINST");

//		Assert.That(nativeQuery/*.ListPage(0, 2)*/.Count, Is.EqualTo(2));
//		Assert.That(nativeQuery/*.ListPage(1, 1)*/.Count, Is.EqualTo(1));
//	  }

//	  protected internal virtual IProcessInstance startProcessInstanceAndEvaluateDecision()
//	  {
//		return runtimeService.StartProcessInstanceByKey("testProcess", Variables);
//	  }

//	  protected internal virtual ICaseInstance createCaseInstanceAndEvaluateDecision()
//	  {
//		return caseService.WithCaseDefinitionByKey("case").SetVariables(Variables).Create();
//	  }

//	  protected internal virtual void evaluateDecisionWithAuthenticatedUser(string userId)
//	  {
//		identityService.AuthenticatedUserId = userId;
//		IVariableMap variables = Variables.PutValue("input1", "test");
//		decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);
//	  }

//	  protected internal virtual IVariableMap Variables
//	  {
//		  get
//		  {
//			IVariableMap variables = Variable.Variables.CreateVariables();
//			variables.Add("input1", "test");
//			return variables;
//		  }
//	  }

//	  /// <summary>
//	  /// Use between two rule evaluations to ensure the expected order by evaluation time.
//	  /// </summary>
//	  protected internal virtual void waitASignificantAmountOfTime()
//	  {
//		DateTime now = new DateTime(ClockUtil.CurrentTime.Ticks);
//		ClockUtil.CurrentTime = now.AddSeconds(10);
//	  }

//	}

//}