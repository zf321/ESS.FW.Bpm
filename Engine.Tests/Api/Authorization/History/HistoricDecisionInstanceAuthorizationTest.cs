using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricDecisionInstanceAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "testProcess";
        protected internal const string DECISION_DEFINITION_KEY = "testDecision";

        [SetUp]
        public void setUp()
        {
            DeploymentId = createDeployment(null, "resources/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml", "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(DeploymentId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            startProcessInstanceAndEvaluateDecision();

            // when
            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

            // then
            ////verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnDecisionDefinition()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithReadPermissionOnAnyDecisionDefinition()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            createGrantAuthorization(Resources.DecisionDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            createGrantAuthorization(Resources.DecisionDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testDeleteHistoricDecisionInstanceWithoutAuthorization()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            try
            {
                // when
                historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);
                Assert.Fail("expect authorization exception");
            }
            catch (AuthorizationException e)
            {
                // then
                Assert.That(e.Message, Is.EqualTo("The user with id 'test' does not have 'Permissions.DeleteHistory' permission on resource 'testDecision' of type 'DecisionDefinition'."));
            }
        }

        public virtual void testDeleteHistoricDecisionInstanceWithDeleteHistoryPermissionOnDecisionDefinition()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            createGrantAuthorization(Resources.DecisionDefinition, AuthorizationFields.Any, userId, Permissions.DeleteHistory);
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;


            // when
            historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

            // then
            disableAuthorization();
            Assert.That(historyService.CreateHistoricDecisionInstanceQuery().Count(), Is.EqualTo(0L));
            enableAuthorization();
        }

        public virtual void testDeleteHistoricDecisionInstanceWithDeleteHistoryPermissionOnAnyDecisionDefinition()
        {
            // given
            startProcessInstanceAndEvaluateDecision();
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.DeleteHistory);
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            // when
            historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

            // then
            disableAuthorization();
            Assert.That(historyService.CreateHistoricDecisionInstanceQuery().Count(), Is.EqualTo(0L));
            enableAuthorization();
        }

        public virtual void testDeleteHistoricDecisionInstanceByInstanceIdWithoutAuthorization()
        {

            // given
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.ReadHistory);
            startProcessInstanceAndEvaluateDecision();

            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
            IHistoricDecisionInstance historicDecisionInstance = query/*/*.IncludeInputs()*//*.IncludeOutputs()*/.First();

            try
            {
                // when
                historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
                Assert.Fail("expect authorization exception");
            }
            catch (AuthorizationException e)
            {
                // then
                Assert.That(e.Message, Is.EqualTo("The user with id 'test' does not have 'Permissions.DeleteHistory' permission on resource 'testDecision' of type 'DecisionDefinition'."));
            }
        }

        public virtual void testDeleteHistoricDecisionInstanceByInstanceIdWithDeleteHistoryPermissionOnDecisionDefinition()
        {

            // given
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.DeleteHistory, Permissions.ReadHistory);
            startProcessInstanceAndEvaluateDecision();

            IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
            ////verifyQueryResults(query, 1);
            IHistoricDecisionInstance historicDecisionInstance = query/*/*.IncludeInputs()*//*.IncludeOutputs()*/.First();

            // when
            historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

            // then
            ////verifyQueryResults(query, 0);
        }

        protected internal virtual void startProcessInstanceAndEvaluateDecision()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["input1"] = null;
            StartProcessInstanceByKey(PROCESS_KEY, variables);
        }

    }

}