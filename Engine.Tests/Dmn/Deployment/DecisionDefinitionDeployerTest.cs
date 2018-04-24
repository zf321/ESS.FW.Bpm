using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Dmn.impl;
using ESS.FW.Bpm.Model.Dmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Dmn.Deployment
{
    [TestFixture]
    public class DecisionDefinitionDeployerTest
    {
        [SetUp]
        public virtual void initServices()
        {
            testRule.Starting();
            repositoryService = engineRule.ProcessEngine.RepositoryService;
        }
        [TearDown]
        public  void Finished()
        {
            testRule.Finished();
        }
        private readonly bool InstanceFieldsInitialized;

        public DecisionDefinitionDeployerTest()
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


        protected internal const string DMN_CHECK_ORDER_RESOURCE =
            "resources/dmn/deployment/DecisionDefinitionDeployerTest.testDmnDeployment.dmn11.xml";

        protected internal const string DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX =
            "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDmnDeployment.dmn";

        protected internal const string DMN_SCORE_RESOURCE = "resources/dmn/deployment/dmnScore.dmn11.xml";

        protected internal const string DMN_DECISION_LITERAL_EXPRESSION =
            "resources/dmn/deployment/DecisionWithLiteralExpression.Dmn";

        protected internal const string DRD_SCORE_RESOURCE = "resources/dmn/deployment/drdScore.dmn11.xml";
        protected internal const string DRD_SCORE_V2_RESOURCE = "resources/dmn/deployment/drdScore_v2.dmn11.xml";
        protected internal const string DRD_DISH_RESOURCE = "resources/dmn/deployment/drdDish.dmn11.xml";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

        protected internal IRepositoryService repositoryService;

        [Test]
        public virtual void dmnDeploymentWithDmnSuffix()
        {
            var deploymentId = testRule.Deploy(DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX)
                .Id;

            // there should be one deployment
            var deploymentQuery = repositoryService.CreateDeploymentQuery();

            //Assert.AreEqual(1, deploymentQuery.Count());
            Assert.IsNotNull(deploymentQuery.SingleOrDefault(c=>c.Id == deploymentId));

            // there should be one case definition
            var query = repositoryService.CreateDecisionDefinitionQuery();
            Assert.AreEqual(1, query.Count());

            var decisionDefinition = query.First();

            Assert.True(decisionDefinition.Id.StartsWith("decision:1:"));
            Assert.AreEqual("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
            Assert.AreEqual("CheckOrder", decisionDefinition.Name);
            Assert.AreEqual("decision", decisionDefinition.Key);
            Assert.AreEqual(1, decisionDefinition.Version);
            Assert.AreEqual(DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX, decisionDefinition.ResourceName);
            Assert.AreEqual(deploymentId, decisionDefinition.DeploymentId);
            Assert.IsNull(decisionDefinition.DiagramResourceName);
        }

        [Test]
        public virtual void dmnDeploymentWithDecisionLiteralExpression()
        {
            var deploymentId = testRule.Deploy(DMN_DECISION_LITERAL_EXPRESSION)
                .Id;

            // there should be decision deployment
            var deploymentQuery = repositoryService.CreateDeploymentQuery();
            
            //Assert.AreEqual(1, deploymentQuery.Count());
            Assert.IsNotNull(deploymentQuery.SingleOrDefault(c => c.Id == deploymentId));

            // there should be one decision definition
            var query = repositoryService.CreateDecisionDefinitionQuery();
            Assert.AreEqual(1, query.Count());

            var decisionDefinition = query.First();

            Assert.True(decisionDefinition.Id.StartsWith("decisionLiteralExpression:1:"));
            Assert.AreEqual("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
            Assert.AreEqual("decisionLiteralExpression", decisionDefinition.Key);
            Assert.AreEqual("Decision with Literal Expression", decisionDefinition.Name);
            Assert.AreEqual(1, decisionDefinition.Version);
            Assert.AreEqual(DMN_DECISION_LITERAL_EXPRESSION, decisionDefinition.ResourceName);
            Assert.AreEqual(deploymentId, decisionDefinition.DeploymentId);
            Assert.IsNull(decisionDefinition.DiagramResourceName);
        }

        [Test]
        [Deployment]
        public virtual void longDecisionDefinitionKey()
        {
            var decisionDefinition = repositoryService.CreateDecisionDefinitionQuery()
                .First();

            Assert.IsFalse(decisionDefinition.Id.StartsWith("o123456789"));
            Assert.AreEqual("o123456789o123456789o123456789o123456789o123456789o123456789o123456789",
                decisionDefinition.Key);
        }

        [Test]//TODO 自定义异常
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void duplicateIdInDeployment()
        {
            var resourceName1 =
                "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDuplicateIdInDeployment.Dmn11.xml";
            var resourceName2 =
                "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDuplicateIdInDeployment2.Dmn11.xml";

            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("duplicateDecision");

            repositoryService.CreateDeployment()
                .AddClasspathResource(resourceName1)
                .AddClasspathResource(resourceName2)
                .Name("duplicateIds")
                .Deploy();
        }

        [Test]
        [Deployment(new[] { "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDecisionDiagramResource.dmn11.xml", "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDecisionDiagramResource.png" })]
        public virtual void getDecisionDiagramResource()
        {
            var resourcePrefix = "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestDecisionDiagramResource";

            var decisionDefinition = repositoryService.CreateDecisionDefinitionQuery()
                .First();

            Assert.AreEqual(resourcePrefix + ".dmn11.xml", decisionDefinition.ResourceName);
            Assert.AreEqual("decision", decisionDefinition.Key);

            var diagramResourceName = decisionDefinition.DiagramResourceName;
            Assert.AreEqual(resourcePrefix + ".png", diagramResourceName);

            var diagramStream = repositoryService.GetResourceAsStream(decisionDefinition.DeploymentId,
                diagramResourceName);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final byte[] diagramBytes = org.camunda.bpm.Engine.impl.util.IoUtil.ReadInputStream(diagramStream, "diagram stream");
            var diagramBytes = IoUtil.ReadInputStream(diagramStream, "diagram stream");
            Assert.AreEqual(2540, diagramBytes.Length);
        }

        [Test]//注意大小写Decision1=>decision1
        [Deployment(new[] { "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestMultipleDecisionDiagramResource.dmn11.xml", "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestMultipleDecisionDiagramResource.decision1.png", "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestMultipleDecisionDiagramResource.decision2.png", "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestMultipleDecisionDiagramResource.decision3.png" })]
        public virtual void multipleDiagramResourcesProvided()
        {
            var resourcePrefix =
                "resources/dmn/deployment/DecisionDefinitionDeployerTest.TestMultipleDecisionDiagramResource.";

            var decisionDefinitionQuery = repositoryService.CreateDecisionDefinitionQuery().ToList();
            Assert.AreEqual(3, decisionDefinitionQuery.Count());

            foreach (var decisionDefinition in decisionDefinitionQuery
                .ToList())
                Assert.AreEqual(resourcePrefix + decisionDefinition.Key + ".png", decisionDefinition.DiagramResourceName);
        }

        [Test]
        public virtual void drdDeployment()
        {
            var deploymentId = testRule.Deploy(DRD_SCORE_RESOURCE)
                .Id;

            // there should be one decision requirements definition
            var query = repositoryService.CreateDecisionRequirementsDefinitionQuery();
            Assert.AreEqual(1, query.Count());

            var decisionRequirementsDefinition = query.First();

            Assert.True(decisionRequirementsDefinition.Id.StartsWith("score:1:"));
            Assert.AreEqual("score", decisionRequirementsDefinition.Key);
            Assert.AreEqual("Score", decisionRequirementsDefinition.Name);
            Assert.AreEqual("test-drd-1", decisionRequirementsDefinition.Category);
            Assert.AreEqual(1, decisionRequirementsDefinition.Version);
            Assert.AreEqual(DRD_SCORE_RESOURCE, decisionRequirementsDefinition.ResourceName);
            Assert.AreEqual(deploymentId, decisionRequirementsDefinition.DeploymentId);
            Assert.IsNull(decisionRequirementsDefinition.DiagramResourceName);

            // both decisions should have a reference to the decision requirements definition
            var decisions = repositoryService.CreateDecisionDefinitionQuery()
                ////.OrderByDecisionDefinitionKey()
                /*.Asc()*/

                .ToList().OrderBy(m=>m.Key).ToList();
            Assert.AreEqual(2, decisions.Count);

            var firstDecision = decisions[0];
            Assert.AreEqual("score-decision", firstDecision.Key);
            Assert.AreEqual(decisionRequirementsDefinition.Id, firstDecision.DecisionRequirementsDefinitionId);
            Assert.AreEqual("score", firstDecision.DecisionRequirementsDefinitionKey);

            var secondDecision = decisions[1];
            Assert.AreEqual("score-result", secondDecision.Key);
            Assert.AreEqual(decisionRequirementsDefinition.Id, secondDecision.DecisionRequirementsDefinitionId);
            Assert.AreEqual("score", secondDecision.DecisionRequirementsDefinitionKey);
        }

        [Test]
        [Deployment(DMN_CHECK_ORDER_RESOURCE)]
        public virtual void noDrdForSingleDecisionDeployment()
        {
            // when the DMN file contains only a single decision definition
            Assert.AreEqual(1, repositoryService.CreateDecisionDefinitionQuery()
                .Count());

            // then no decision requirements definition should be created
            Assert.AreEqual(0, repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .Count());
            // and the decision should not be linked to a decision requirements definition
            var decisionDefinition = repositoryService.CreateDecisionDefinitionQuery()
                .FirstOrDefault();
            Assert.IsNull(decisionDefinition.DecisionRequirementsDefinitionId);
            Assert.IsNull(decisionDefinition.DecisionRequirementsDefinitionKey);
        }

        [Test]
        [Deployment(new[] { DRD_SCORE_RESOURCE, DRD_DISH_RESOURCE })]
        public virtual void multipleDrdDeployment()
        {
            // there should be two decision requirements definitions
            var decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery()
                //.OrderByDecisionRequirementsDefinitionCategory()
                /*.Asc()*/

                .ToList().OrderBy(m=>m.Category).ToList();

            Assert.AreEqual(2, decisionRequirementsDefinitions.Count);
            Assert.AreEqual("score", decisionRequirementsDefinitions[0].Key);
            Assert.AreEqual("dish", decisionRequirementsDefinitions[1].Key);

            // the decisions should have a reference to the decision requirements definition
            var decisions = repositoryService.CreateDecisionDefinitionQuery()
                //.OrderByDecisionDefinitionCategory()
                /*.Asc()*/

                .ToList().OrderBy(m=>m.Category).ToList();
            Assert.AreEqual(5, decisions.Count);
            Assert.AreEqual(decisionRequirementsDefinitions[0].Id, decisions[0].DecisionRequirementsDefinitionId);
            Assert.AreEqual(decisionRequirementsDefinitions[0].Id, decisions[1].DecisionRequirementsDefinitionId);
            Assert.AreEqual(decisionRequirementsDefinitions[1].Id, decisions[2].DecisionRequirementsDefinitionId);
            Assert.AreEqual(decisionRequirementsDefinitions[1].Id, decisions[3].DecisionRequirementsDefinitionId);
            Assert.AreEqual(decisionRequirementsDefinitions[1].Id, decisions[4].DecisionRequirementsDefinitionId);
        }

        [Test]//TODO 自定义异常
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void duplicateDrdIdInDeployment()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("definitions");

            repositoryService.CreateDeployment()
                .AddClasspathResource(DRD_SCORE_RESOURCE)
                .AddClasspathResource(DRD_SCORE_V2_RESOURCE)
                .Name("duplicateIds")
                .Deploy();
        }

        [Test]
        public virtual void deployMultipleDecisionsWithSameDrdId()
        {
            // when deploying two decision with the same drd id `definitions`
            testRule.Deploy(DMN_SCORE_RESOURCE, DMN_CHECK_ORDER_RESOURCE);

            // then create two decision definitions and
            // ignore the duplicated drd id since no drd is created
            Assert.AreEqual(2, repositoryService.CreateDecisionDefinitionQuery()
                .Count());
            Assert.AreEqual(0, repositoryService.CreateDecisionRequirementsDefinitionQuery()
                .Count());
        }

        [Test]
        public virtual void deployDecisionIndependentFromDrd()
        {
            var deploymentIdDecision = testRule.Deploy(DMN_SCORE_RESOURCE)
                .Id;
            var deploymentIdDrd = testRule.Deploy(DRD_SCORE_RESOURCE)
                .Id;

            // there should be one decision requirements definition
            var query = repositoryService.CreateDecisionRequirementsDefinitionQuery();
            Assert.AreEqual(1, query.Count());

            var decisionRequirementsDefinition = query.First();
            Assert.AreEqual(1, decisionRequirementsDefinition.Version);
            Assert.AreEqual(deploymentIdDrd, decisionRequirementsDefinition.DeploymentId);

            // and two deployed decisions with different versions
            var decisions = repositoryService.CreateDecisionDefinitionQuery(c => c.Key == "score-decision")
                //.OrderByDecisionDefinitionVersion()
                /*.Asc()*/

                .ToList().OrderBy(m=>m.Version).ToList();

            Assert.AreEqual(2, decisions.Count);

            var firstDecision = decisions[0];
            Assert.AreEqual(1, firstDecision.Version);
            Assert.AreEqual(deploymentIdDecision, firstDecision.DeploymentId);
            Assert.IsNull(firstDecision.DecisionRequirementsDefinitionId);

            var secondDecision = decisions[1];
            Assert.AreEqual(2, secondDecision.Version);
            Assert.AreEqual(deploymentIdDrd, secondDecision.DeploymentId);
            Assert.AreEqual(decisionRequirementsDefinition.Id, secondDecision.DecisionRequirementsDefinitionId);
        }

        [Test]
        public virtual void testDeployDmnModelInstance()
        {
            // given
            var dmnModelInstance = createDmnModelInstance();

            // when
            testRule.Deploy(repositoryService.CreateDeployment()
                .AddModelInstance("foo.Dmn", dmnModelInstance));

            // then
            Assert.NotNull(repositoryService.CreateDecisionDefinitionQuery(c => c.ResourceName == "foo.Dmn")
                .First());
        }

        [Test]
        public virtual void testDeployDmnModelInstanceNegativeHistoryTimeToLive()
        {
            // given
            var dmnModelInstance = createDmnModelInstanceNegativeHistoryTimeToLive();

            try
            {
                testRule.Deploy(repositoryService.CreateDeployment()
                    .AddModelInstance("foo.Dmn", dmnModelInstance));
                Assert.Fail("Exception for negative time to live value is expected.");
            }
            catch (ProcessEngineException ex)
            {
                Assert.True(ex.InnerException.Message.Contains("greater than or equal to 0"));
            }
        }

        protected internal static IDmnModelInstance createDmnModelInstanceNegativeHistoryTimeToLive()
        {
            var modelInstance = ESS.FW.Bpm.Model.Dmn.Dmn.CreateEmptyModel();
            var definitions = modelInstance.NewInstance<IDefinitions>(typeof(IDefinitions));
            definitions.Id = DmnModelConstants.DmnElementDefinitions;
            definitions.Name = DmnModelConstants.DmnElementDefinitions;
            definitions.Namespace = DmnModelConstants.CamundaNs;
            modelInstance.Definitions = definitions;

            var decision = modelInstance.NewInstance<IDecision>(typeof(IDecision));
            decision.Id = "Decision-1";
            decision.Name = "foo";
            decision.CamundaHistoryTimeToLive = -5;
            modelInstance.Definitions.AddChildElement(decision);

            return modelInstance;
        }

        protected internal static IDmnModelInstance createDmnModelInstance()
        {
            var modelInstance = ESS.FW.Bpm.Model.Dmn.Dmn.CreateEmptyModel();
            var definitions = modelInstance.NewInstance<IDefinitions>(typeof(IDefinitions));
            definitions.Id = DmnModelConstants.DmnElementDefinitions;
            definitions.Name = DmnModelConstants.DmnElementDefinitions;
            definitions.Namespace = DmnModelConstants.CamundaNs;
            modelInstance.Definitions = definitions;

            var decision = modelInstance.NewInstance<IDecision>(typeof(IDecision));
            decision.Id = "Decision-1";
            decision.Name = "foo";
            decision.CamundaHistoryTimeToLive = 5;
            modelInstance.Definitions.AddChildElement(decision);

            var decisionTable = modelInstance.NewInstance<IDecisionTable>(typeof(IDecisionTable));
            decisionTable.Id = DmnModelConstants.DmnElementDecisionTable;
            decisionTable.HitPolicy = HitPolicy.First;
            decision.AddChildElement(decisionTable);

            var input = modelInstance.NewInstance<IInput>(typeof(IInput));
            input.Id = "Input-1";
            input.Label = "Input";
            decisionTable.AddChildElement(input);

            var inputExpression = modelInstance.NewInstance<IInputExpression>(typeof(IInputExpression));
            inputExpression.Id = "InputExpression-1";
            var inputExpressionText = modelInstance.NewInstance<IText>(typeof(IText));
            inputExpressionText.TextContent = "input";
            inputExpression.Text = inputExpressionText;
            inputExpression.TypeRef = "string";
            input.InputExpression = inputExpression;

            var output = modelInstance.NewInstance<IOutput>(typeof(IOutput));
            output.Name = "output";
            output.Label = "Output";
            output.TypeRef = "string";
            decisionTable.AddChildElement(output);

            return modelInstance;
        }

        [Test]
        public virtual void testDeployAndGetDecisionDefinition()
        {
            // given decision model
            var dmnModelInstance = createDmnModelInstance();

            // when decision model is deployed
            var deploymentBuilder = repositoryService.CreateDeployment()
                .AddModelInstance("foo.Dmn", dmnModelInstance);
            var deployment = testRule.Deploy(deploymentBuilder);

            // then deployment contains definition
            var deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
            Assert.AreEqual(1, deployedDecisionDefinitions.Count);
            Assert.IsNull(deployment.DeployedDecisionRequirementsDefinitions);
            Assert.IsNull(deployment.DeployedProcessDefinitions);
            Assert.IsNull(deployment.DeployedCaseDefinitions);

            // and persisted definition are equal to deployed definition
            var persistedDecisionDef = repositoryService.CreateDecisionDefinitionQuery(c => c.ResourceName == "foo.Dmn")
                .First();
            Assert.AreEqual(persistedDecisionDef.Id, deployedDecisionDefinitions[0].Id);
        }

        [Test]//TODO Builder模式
        public virtual void testDeployEmptyDecisionDefinition()
        {
            // given empty decision model
            var modelInstance = ESS.FW.Bpm.Model.Dmn.Dmn.CreateEmptyModel();
            var definitions = modelInstance.NewInstance<IDefinitions>(typeof(IDefinitions));
            definitions.Id = DmnModelConstants.DmnElementDefinitions;
            definitions.Name = DmnModelConstants.DmnElementDefinitions;
            definitions.Namespace = DmnModelConstants.CamundaNs;
            modelInstance.Definitions = definitions;

            // when decision model is deployed
            var deploymentBuilder = repositoryService.CreateDeployment()
                .AddModelInstance("foo.Dmn", modelInstance);
            var deployment = testRule.Deploy(deploymentBuilder);

            // then deployment contains no definitions
            Assert.IsNull(deployment.DeployedDecisionDefinitions);
            Assert.IsNull(deployment.DeployedDecisionRequirementsDefinitions);

            // and there are no persisted definitions
            Assert.IsNull(repositoryService.CreateDecisionDefinitionQuery(c => c.ResourceName == "foo.Dmn").SingleOrDefault());
        }


        [Test]
        public virtual void testDeployAndGetDRDDefinition()
        {
            // when decision requirement graph is deployed
            var deployment = testRule.Deploy(DRD_SCORE_RESOURCE);

            // then deployment contains definitions
            var deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
            Assert.AreEqual(2, deployedDecisionDefinitions.Count);

            var deployedDecisionRequirementsDefinitions = deployment.DeployedDecisionRequirementsDefinitions;
            Assert.AreEqual(1, deployedDecisionRequirementsDefinitions.Count);

            Assert.IsNull(deployment.DeployedProcessDefinitions);
            Assert.IsNull(deployment.DeployedCaseDefinitions);

            // and persisted definitions are equal to deployed definitions
            var persistedDecisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery(c => c.ResourceName == DRD_SCORE_RESOURCE)
                .First();
            Assert.AreEqual(persistedDecisionRequirementsDefinition.Id, deployedDecisionRequirementsDefinitions[0].Id);

            var persistedDecisionDefinitions = repositoryService.CreateDecisionDefinitionQuery(c => c.ResourceName == DRD_SCORE_RESOURCE)

                .ToList();
            Assert.AreEqual(deployedDecisionDefinitions.Count, persistedDecisionDefinitions.Count);
        }

        [Test]
        public virtual void dmnDeployment()
        {
            var deploymentId = testRule.Deploy(DMN_CHECK_ORDER_RESOURCE)
                .Id;

            // there should be decision deployment
            var deploymentQuery = repositoryService.CreateDeploymentQuery();

            //Assert.AreEqual(1, deploymentQuery.Count());
            Assert.IsNotNull(deploymentQuery.SingleOrDefault(c => c.Id == deploymentId));

            // there should be one decision definition
            var query = repositoryService.CreateDecisionDefinitionQuery();
            Assert.AreEqual(1, query.Count());

            var decisionDefinition = query.First();

            Assert.True(decisionDefinition.Id.StartsWith("decision:1:"));
            Assert.AreEqual("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
            Assert.AreEqual("CheckOrder", decisionDefinition.Name);
            Assert.AreEqual("decision", decisionDefinition.Key);
            Assert.AreEqual(1, decisionDefinition.Version);
            Assert.AreEqual(DMN_CHECK_ORDER_RESOURCE, decisionDefinition.ResourceName);
            Assert.AreEqual(deploymentId, decisionDefinition.DeploymentId);
            Assert.IsNull(decisionDefinition.DiagramResourceName);
        }
    }
}