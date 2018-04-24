using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricDecisionInstanceStatisticsQueryTest
    {
        private readonly bool InstanceFieldsInitialized;

        public HistoricDecisionInstanceStatisticsQueryTest()
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
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        protected internal const string DISH_DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";
        protected internal const string SCORE_DRG_DMN = "resources/dmn/deployment/drdScore.Dmn11.xml";

        protected internal const string NON_EXISTING = "fake";
        protected internal const string DISH_DECISION = "dish-decision";
        protected internal const string TEMPERATURE = "temperature";
        protected internal const string DAY_TYPE = "dayType";
        protected internal const string WEEKEND = "Weekend";

        protected internal IDecisionService decisionService;
        protected internal IRepositoryService repositoryService;
        protected internal IHistoryService historyService;

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public readonly ExpectedException thrown = ExpectedException.None();

        [SetUp]
        public virtual void setUp()
        {
            decisionService = engineRule.DecisionService;
            repositoryService = engineRule.RepositoryService;
            historyService = engineRule.HistoryService;
            testRule.Deploy(DISH_DRG_DMN);
        }

        [Test]
        public virtual void testStatisticForRootDecisionEvaluation()
        {
            //when
            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 11)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();

            var statisticsQuery =
                historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

            //then
            Assert.That(statisticsQuery.Count(), Is.EqualTo(3L));
            Assert.That(statisticsQuery
                .Count(), Is.EqualTo(3));
            Assert.That(statisticsQuery
                .First().Evaluations, Is.EqualTo(2));
            Assert.That(statisticsQuery
                .First().DecisionDefinitionKey, Is.Not.EqualTo(null));
        }

        [Test]
        public virtual void testStatisticForRootDecisionWithInstanceConstraintEvaluation()
        {
            //when
            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 11)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();


            var decisionInstanceId = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionRequirementsDefinitionId == decisionRequirementsDefinition.Id)
                //.RootDecisionInstancesOnly()
                
                .First().Id;

            var query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id)
                //.DecisionInstanceId(decisionInstanceId)
                ;

            //then
            Assert.That(query.Count(), Is.EqualTo(3L));
            Assert.That(query
                .Count(), Is.EqualTo(3));
            Assert.That(query
                .First().Evaluations, Is.EqualTo(1));
            Assert.That(query
                .First().DecisionDefinitionKey, Is.Not.EqualTo(null));
        }

        [Test]
        public virtual void testStatisticForRootDecisionWithFakeInstanceConstraintEvaluation()
        {
            //when
            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();

            var query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id)
                //.DecisionInstanceId(NON_EXISTING)
                ;

            //then
            Assert.That(query.Count(), Is.EqualTo(0L));
            Assert.That(query
                .Count(), Is.EqualTo(0));
        }

        [Test]
        public virtual void testStatisticForRootDecisionWithNullInstanceConstraintEvaluation()
        {
            //when
            decisionService.EvaluateDecisionTableByKey(DISH_DECISION)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21)
                    .PutValue(DAY_TYPE, WEEKEND))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();
            //when
            var query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id)
                //.DecisionInstanceId(null)
                ;

            //then
            try
            {
                query.Count();
            }
            catch (NullValueException)
            {
                //expected
            }

            try
            {
                query
                    .ToList();
            }
            catch (NullValueException)
            {
                //expected
            }
        }

        [Test]
        public virtual void testStatisticForChildDecisionEvaluation()
        {
            //when
            decisionService.EvaluateDecisionTableByKey("season")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();

            var statisticsQuery =
                historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

            //then
            Assert.That(statisticsQuery.Count(), Is.EqualTo(1L));
            Assert.That(statisticsQuery
                .Count(), Is.EqualTo(1));
            Assert.That(statisticsQuery
                .First().Evaluations, Is.EqualTo(1));
            Assert.That(statisticsQuery
                .First().DecisionDefinitionKey, Is.Not.EqualTo(null));
        }

        [Test]
        public virtual void testStatisticConstrainedToOneDRD()
        {
            //given
            testRule.Deploy(SCORE_DRG_DMN);

            //when
            decisionService.EvaluateDecisionTableByKey("score-decision")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("input", "john"))
                .Evaluate();

            decisionService.EvaluateDecisionTableByKey("season")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(TEMPERATURE, 21))
                .Evaluate();

            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Name =="Score")
                .First();

            var statisticsQuery =
                historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

            //then
            Assert.That(statisticsQuery.Count(), Is.EqualTo(1L));
            Assert.That(statisticsQuery
                .Count(), Is.EqualTo(1));
            Assert.That(statisticsQuery
                .First().Evaluations, Is.EqualTo(1));
            Assert.That(statisticsQuery
                .First().DecisionDefinitionKey, Is.Not.EqualTo(null));
        }

        [Test]
        public virtual void testStatisticDoesNotExistForFakeId()
        {
            Assert.That(historyService.CreateHistoricDecisionInstanceStatisticsQuery(NON_EXISTING)
                .Count(), Is.EqualTo(0L), "available statistics Count of fake");

            Assert.That(historyService.CreateHistoricDecisionInstanceStatisticsQuery(NON_EXISTING)
                
                .Count(), Is.EqualTo(0), "available statistics elements of fake");
        }

        [Test]
        //[ExpectedException(typeof(NullValueException))]
        public virtual void testStatisticThrowsExceptionOnNullConstraintsCount()
        {
            //expect
            //thrown.Expect(typeof(NullValueException));
            historyService.CreateHistoricDecisionInstanceStatisticsQuery(null)
                .Count();
        }

        [Test]
        //[ExpectedException(typeof(NullValueException))]
        public virtual void testStatisticThrowsExceptionOnNullConstraintsList()
        {
            //expect
            //thrown.Expect(typeof(NullValueException));
            historyService.CreateHistoricDecisionInstanceStatisticsQuery(null)
                
                .ToList();
        }

        [Test]
        public virtual void testStatisticForNotEvaluatedDRD()
        {
            //when
            var decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .First();

            var statisticsQuery =
                historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

            //then
            Assert.That(statisticsQuery.Count(), Is.EqualTo(0L), "available statistics Count");
            Assert.That(statisticsQuery
                .Count(), Is.EqualTo(0), "available statistics elements");
        }
    }
}