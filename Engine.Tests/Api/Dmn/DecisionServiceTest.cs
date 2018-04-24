using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;
using IDmnDecisionTableResult = ESS.FW.Bpm.Engine.IDmnDecisionTableResult;

namespace Engine.Tests.Api.Dmn
{


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DecisionServiceTest
    {
        private bool InstanceFieldsInitialized = false;

        public DecisionServiceTest()
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


        protected internal const string DMN_DECISION_TABLE = "resources/api/dmn/Example.Dmn";
        protected internal const string DMN_DECISION_TABLE_V2 = "resources/api/dmn/Example_v2.Dmn";

        protected internal const string DMN_DECISION_LITERAL_EXPRESSION = "resources/api/dmn/DecisionWithLiteralExpression.Dmn";
        protected internal const string DMN_DECISION_LITERAL_EXPRESSION_V2 = "resources/api/dmn/DecisionWithLiteralExpression_v2.Dmn";

        protected internal const string DRD_DISH_DECISION_TABLE = "resources/dmn/deployment/drdDish.Dmn11.xml";

        protected internal const string DECISION_DEFINITION_KEY = "decision";

        protected internal const string RESULT_OF_FIRST_VERSION = "ok";
        protected internal const string RESULT_OF_SECOND_VERSION = "notok";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

        protected internal IDecisionService decisionService;
        protected internal IRepositoryService repositoryService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void init()
        [SetUp]
        public virtual void init()
        {
            decisionService = engineRule.ProcessEngine.DecisionService;
            repositoryService = engineRule.ProcessEngine.RepositoryService;
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableById()
        {
            IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableById(decisionDefinition.Id, createVariables());

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableByKey()
        {
            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableByKeyAndLatestVersion()
        {
            testRule.Deploy(DMN_DECISION_TABLE_V2);

            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableByKeyAndVersion()
        {
            testRule.Deploy(DMN_DECISION_TABLE_V2);

            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, 1, createVariables());

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableByKeyAndNullVersion()
        {
            testRule.Deploy(DMN_DECISION_TABLE_V2);

            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, null, createVariables());

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
        }

        [Test]
        public virtual void evaluateDecisionTableByNullId()
        {

            //thrown.Expect(typeof(NotValidException));
            //thrown.ExpectMessage("either decision definition id or key must be set");

            decisionService.EvaluateDecisionTableById(null, null);
            Assert.Fail("either decision definition id or key must be set");
        }

        [Test]
        public virtual void evaluateDecisionTableByNonExistingId()
        {
            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no deployed decision definition found with id 'unknown'");

            decisionService.EvaluateDecisionTableById("unknown", null);
            Assert.Fail("no deployed decision definition found with id 'unknown'");
        }

        [Test]
        public virtual void evaluateDecisionTableByNullKey()
        {
            //thrown.Expect(typeof(NotValidException));
            //thrown.ExpectMessage("either decision definition id or key must be set");

            decisionService.EvaluateDecisionTableByKey(null, null);
            Assert.Fail("either decision definition id or key must be set");
        }

        [Test]
        public virtual void evaluateDecisionTableByNonExistingKey()
        {
            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key 'unknown'");

            decisionService.EvaluateDecisionTableByKey("unknown", null);
            Assert.Fail("no decision definition deployed with key 'unknown'");
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_TABLE })]
        public virtual void evaluateDecisionTableByKeyWithNonExistingVersion()
        {
            IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key = 'decision' and version = '42'");

            decisionService.EvaluateDecisionTableByKeyAndVersion(decisionDefinition.Key, 42, null);

            Assert.Fail("no decision definition deployed with key = 'decision' and version = '42'");
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionById()
        {
            IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

            IDmnDecisionResult decisionResult = decisionService.EvaluateDecisionById(decisionDefinition.Id).Variables(createVariables()).Evaluate();

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionByKey()
        {
            IDmnDecisionResult decisionResult = decisionService.EvaluateDecisionByKey(DECISION_DEFINITION_KEY).Variables(createVariables()).Evaluate();

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionByKeyAndLatestVersion()
        {
            testRule.Deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

            IDmnDecisionResult decisionResult = decisionService.EvaluateDecisionByKey(DECISION_DEFINITION_KEY).Variables(createVariables()).Evaluate();

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionByKeyAndVersion()
        {
            testRule.Deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

            IDmnDecisionResult decisionResult = decisionService.EvaluateDecisionByKey(DECISION_DEFINITION_KEY).Version(1).Variables(createVariables()).Evaluate();

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionByKeyAndNullVersion()
        {
            testRule.Deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

            IDmnDecisionResult decisionResult = decisionService.EvaluateDecisionByKey(DECISION_DEFINITION_KEY).Version(null).Variables(createVariables()).Evaluate();

            AssertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
        }

        [Test]
        public virtual void evaluateDecisionByNullId()
        {
            //thrown.Expect(typeof(NotValidException));
            //thrown.ExpectMessage("either decision definition id or key must be set");

            decisionService.EvaluateDecisionById(null).Evaluate();
            Assert.Fail("either decision definition id or key must be set");
        }

        [Test]
        public virtual void evaluateDecisionByNonExistingId()
        {
            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no deployed decision definition found with id 'unknown'");

            decisionService.EvaluateDecisionById("unknown").Evaluate();
            Assert.Fail("no deployed decision definition found with id 'unknown'");
        }

        [Test]
        public virtual void evaluateDecisionByNullKey()
        {
            //thrown.Expect(typeof(NotValidException));
            //thrown.ExpectMessage("either decision definition id or key must be set");

            decisionService.EvaluateDecisionByKey(null).Evaluate();
            Assert.Fail("either decision definition id or key must be set");
        }

        [Test]
        public virtual void evaluateDecisionByNonExistingKey()
        {
            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key 'unknown'");

            decisionService.EvaluateDecisionByKey("unknown").Evaluate();
            Assert.Fail("no decision definition deployed with key 'unknown'");
        }

        [Test]
        [Deployment(new string[] { DMN_DECISION_LITERAL_EXPRESSION })]
        public virtual void evaluateDecisionByKeyWithNonExistingVersion()
        {
            IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

            //thrown.Expect(typeof(NotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key = 'decision' and version = '42'");

            decisionService.EvaluateDecisionByKey(decisionDefinition.Key).Version(42).Evaluate();
            Assert.Fail("no decision definition deployed with key = 'decision' and version = '42'");
        }

        [Test]
        [Deployment(new string[] { DRD_DISH_DECISION_TABLE })]
        public virtual void evaluateDecisionWithRequiredDecisions()
        {

            IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey("dish-decision", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("temperature", 32).PutValue("dayType", "Weekend"));

            AssertThatDecisionHasResult(decisionResult, "Light salad");
        }

        protected internal virtual IVariableMap createVariables()
        {
            return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "silver").PutValue("sum", 723);
        }

        protected internal virtual void AssertThatDecisionHasResult(IDmnDecisionTableResult decisionResult, object expectedValue)
        {
            //Assert.That(decisionResult, Is.EqualTo(notNullValue()));
            Assert.NotNull(decisionResult);
            Assert.That(decisionResult.Count, Is.EqualTo(1));
            //string value = decisionResult.SingleResult.FirstEntry;
            string value = decisionResult.GetSingleResult().getFirstEntry<string>();
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        protected internal virtual void AssertThatDecisionHasResult(IDmnDecisionResult decisionResult, object expectedValue)
        {
            //Assert.That(decisionResult, Is.EqualTo(notNullValue()));
            Assert.That(decisionResult, Is.Not.Null);
            Assert.That(decisionResult.Count, Is.EqualTo(1));
            string value = (string)decisionResult.GetSingleEntry().GetFirstEntry();
            //string value = decisionResult.SingleResult.FirstEntry;
            Assert.That(value, Is.EqualTo(expectedValue));
        }

    }

}