using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Dmn.BusinessRuleTask
{
    [TestFixture]
    public class DmnBusinessRuleTaskTest
    {
        private readonly bool InstanceFieldsInitialized;

        public DmnBusinessRuleTaskTest()
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
            ////ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        public const string DECISION_PROCESS =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml";

        public const string DECISION_PROCESS_EXPRESSION =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRefExpression.bpmn20.xml";

        public const string DECISION_PROCESS_COMPOSITEEXPRESSION =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRefCompositeExpression.bpmn20.xml";

        public const string DECISION_PROCESS_LATEST =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRefLatestBinding.bpmn20.xml";

        public const string DECISION_PROCESS_DEPLOYMENT =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRefDeploymentBinding.bpmn20.xml";

        public const string DECISION_PROCESS_VERSION =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRefVersionBinding.bpmn20.xml";

        public const string DECISION_OKAY_DMN =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionOkay.dmn11.xml";

        public const string DECISION_NOT_OKAY_DMN =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionNotOkay.dmn11.xml";

        public const string DECISION_POJO_DMN =
            "resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestPojo.dmn11.xml";

        public const string DECISION_LITERAL_EXPRESSION_DMN =
            "resources/dmn/deployment/DecisionWithLiteralExpression.dmn";

        public const string DRD_DISH_RESOURCE = "resources/dmn/deployment/drdDish.dmn11.xml";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

        protected internal IRuntimeService runtimeService;

        [SetUp]
        public virtual void Init()
        {
            testRule.Starting();
            //runtimeService = engineRule.RuntimeService;
            runtimeService = engineRule.ProcessEngine.RuntimeService;
        }
        [TearDown]
        public virtual void TearDown()
        {
            testRule.Finished();
        }
        [Test]
        [Deployment(new []{DECISION_PROCESS, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN }) ]
        public virtual void decisionRef()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            Assert.AreEqual("okay", getDecisionResult(processInstance));

            processInstance = startExpressionProcess("testDecision", 1);
            Assert.AreEqual("okay", getDecisionResult(processInstance));
        }
        [Test]
        [Deployment( DECISION_PROCESS)]
        //[ExpectedException(typeof(DecisionDefinitionNotFoundException))]
        public virtual void noDecisionFound()
        {
            //thrown.Expect(typeof(DecisionDefinitionNotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key 'testDecision'");

            runtimeService.StartProcessInstanceByKey("testProcess");
        }

        [Test][Deployment( DECISION_PROCESS_EXPRESSION) ]
        //[ExpectedException(typeof(DecisionDefinitionNotFoundException))]
        public virtual void noDecisionFoundRefByExpression()
        {
            //thrown.Expect(typeof(DecisionDefinitionNotFoundException));
            //thrown.ExpectMessage("no decision definition deployed with key = 'testDecision', version = '1' and tenant-id 'null");

            startExpressionProcess("testDecision", 1);
        }

        [Test]
        [Deployment(new []{DECISION_PROCESS_LATEST, DECISION_OKAY_DMN })]
        public virtual void decisionRefLatestBinding()
        {
            testRule.Deploy(DECISION_NOT_OKAY_DMN);

            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            Assert.AreEqual("not okay", getDecisionResult(processInstance));
        }

        [Test]
        [Deployment(new []{DECISION_PROCESS_DEPLOYMENT, DECISION_OKAY_DMN }) ]
        public virtual void decisionRefDeploymentBinding()
        {
            testRule.Deploy(DECISION_NOT_OKAY_DMN);

            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            Assert.AreEqual("okay", getDecisionResult(processInstance));
        }

        [Test]
        [Deployment(new []{DECISION_PROCESS_VERSION, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN })]
        public virtual void decisionRefVersionBinding()
        {
            testRule.Deploy(DECISION_NOT_OKAY_DMN);
            testRule.Deploy(DECISION_OKAY_DMN);

            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            Assert.AreEqual("not okay", getDecisionResult(processInstance));

            processInstance = startExpressionProcess("testDecision", 2);
            Assert.AreEqual("not okay", getDecisionResult(processInstance));
        }

        [Test][Deployment(new [] {DECISION_PROCESS, DECISION_POJO_DMN}) ]
        public virtual void testPojo()
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("pojo", new TestPojo("okay", 13.37));
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            Assert.AreEqual("okay", getDecisionResult(processInstance));
        }

        [Test]
        [Deployment( DECISION_LITERAL_EXPRESSION_DMN)]
        public virtual void evaluateDecisionWithLiteralExpression()
        {
            testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .BusinessRuleTask()
                .CamundaDecisionRef("decisionLiteralExpression")
                .CamundaResultVariable("result")
                .CamundaMapDecisionResult("singleEntry")
                .EndEvent()
                .CamundaAsyncBefore()
                .Done());

            var processInstance = runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("a", 2)
                .PutValue("b", 3));

            Assert.AreEqual(5, getDecisionResult(processInstance));
        }

        [Test][Deployment( DRD_DISH_RESOURCE) ]
        public virtual void evaluateDecisionWithRequiredDecisions()
        {
            testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .BusinessRuleTask()
                .CamundaDecisionRef("dish-decision")
                .CamundaResultVariable("result")
                .CamundaMapDecisionResult("singleEntry")
                .EndEvent()
                .CamundaAsyncBefore()
                .Done());

            var processInstance = runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("temperature", 32)
                .PutValue("dayType", "Weekend"));

            Assert.AreEqual("Light salad", getDecisionResult(processInstance));
        }

        [Test][Deployment(new []{DECISION_PROCESS_COMPOSITEEXPRESSION, DECISION_OKAY_DMN}) ]
        public virtual void decisionRefWithCompositeExpression()
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("version", 1);
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcessCompositeExpression", variables);

            Assert.AreEqual("okay", getDecisionResult(processInstance));
        }

        protected internal virtual IProcessInstance startExpressionProcess(object decisionKey, object version)
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("decision", decisionKey)
                .PutValue("version", version);
            return runtimeService.StartProcessInstanceByKey("testProcessExpression", variables);
        }

        protected internal virtual object getDecisionResult(IProcessInstance processInstance)
        {
            // the single entry of the single result of the decision result is stored as process variable
            var test= runtimeService.GetVariable(processInstance.Id, "result");
            return test;
        }
    }
}