using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Dmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Dmn
{


    /// <summary>
    /// 
    /// </summary>
    public class DecisionDefinitionAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "testProcess";
        protected internal const string DECISION_DEFINITION_KEY = "sampleDecision";

        [SetUp]
        public void setUp()
        {
            DeploymentId = createDeployment(null, "resources/api/authorization/singleDecision.Dmn11.xml", "resources/api/authorization/anotherDecision.Dmn11.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(DeploymentId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given user is not authorized to read any decision definition

            // when
            IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnAnyDecisionDefinition()
        {
            // given user gets read permission on any decision definition
            createGrantAuthorization(Resources.DecisionDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadPermissionOnOneDecisionDefinition()
        {
            // given user gets read permission on the decision definition
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);

            // when
            IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

            // then
            //verifyQueryResults(query, 1);

            IDecisionDefinition definition = query.First();
            Assert.NotNull(definition);
            Assert.AreEqual(DECISION_DEFINITION_KEY, definition.Key);
        }

        public virtual void testQueryWithMultiple()
        {
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);
            createGrantAuthorization(Resources.DecisionDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testGetDecisionDefinitionWithoutAuthorizations()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            try
            {
                // when
                repositoryService.GetDecisionDefinition(decisionDefinitionId);
                Assert.Fail("Exception expected");

            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(DECISION_DEFINITION_KEY, message);
                AssertTextPresent(Resources.DecisionDefinition.ToString(), message);
            }
        }

        public virtual void testGetDecisionDefinition()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);

            // when
            IDecisionDefinition decisionDefinition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

            // then
            Assert.NotNull(decisionDefinition);
        }

        public virtual void testGetDecisionDiagramWithoutAuthorizations()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            try
            {
                // when
                repositoryService.GetDecisionDiagram(decisionDefinitionId);
                Assert.Fail("Exception expected");

            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(DECISION_DEFINITION_KEY, message);
                AssertTextPresent(Resources.DecisionDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetDecisionDiagram()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);

            // when
            System.IO.Stream stream = repositoryService.GetDecisionDiagram(decisionDefinitionId);

            // then
            // no decision diagram deployed
            Assert.IsNull(stream);
        }

        public virtual void testGetDecisionModelWithoutAuthorizations()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            try
            {
                // when
                repositoryService.GetDecisionModel(decisionDefinitionId);
                Assert.Fail("Exception expected");

            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(DECISION_DEFINITION_KEY, message);
                AssertTextPresent(Resources.DecisionDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetDecisionModel()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);

            // when
            System.IO.Stream stream = repositoryService.GetDecisionModel(decisionDefinitionId);

            // then
            Assert.NotNull(stream);
        }

        public virtual void testGetDmnModelInstanceWithoutAuthorizations()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

            try
            {
                // when
                repositoryService.GetDmnModelInstance(decisionDefinitionId);
                Assert.Fail("Exception expected");

            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(DECISION_DEFINITION_KEY, message);
                AssertTextPresent(Resources.DecisionDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetDmnModelInstance()
        {
            // given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Read);

            // when
            IDmnModelInstance modelInstance = repositoryService.GetDmnModelInstance(decisionDefinitionId);

            // then
            Assert.NotNull(modelInstance);
        }

        public virtual void testDecisionDefinitionUpdateTimeToLive()
        {
            //given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            createGrantAuthorization(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, userId, Permissions.Update);

            //when
            // Todo: IRepositoryService.UpdateDecisionDefinitionHistoryTimeToLive(..)
            //repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

            //then
            Assert.AreEqual(6, selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).HistoryTimeToLive);

        }

        public virtual void testDecisionDefinitionUpdateTimeToLiveWithoutAuthorizations()
        {
            //given
            string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
            try
            {
                //when
                // Todo: IRepositoryService.UpdateDecisionDefinitionHistoryTimeToLive(..)
                //repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);
                Assert.Fail("Exception expected");

            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(DECISION_DEFINITION_KEY, message);
                AssertTextPresent(Resources.DecisionDefinition.ToString()/*.ResourceName()*/, message);
            }

        }

    }

}