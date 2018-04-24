using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Xml.instance;
using NUnit.Framework;


namespace Engine.Tests.Api.Repository
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class RedeploymentTest : PluggableProcessEngineTestCase
    {

        public const string DEPLOYMENT_NAME = "my-deployment";
        public const string PROCESS_KEY = "process";
        public const string PROCESS_1_KEY = "process-1";
        public const string PROCESS_2_KEY = "process-2";
        public const string PROCESS_3_KEY = "process-3";
        public const string RESOURCE_NAME = "path/to/my/process.bpmn";
        public const string RESOURCE_1_NAME = "path/to/my/process1.bpmn";
        public const string RESOURCE_2_NAME = "path/to/my/process2.bpmn";
        public const string RESOURCE_3_NAME = "path/to/my/process3.bpmn";

        [Test]
        public virtual void testRedeployInvalidDeployment()
        {

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources("not-existing").Deploy();
                Assert.Fail("It should not be able to re-deploy an unexisting deployment");
            }
            catch (NotFoundException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById("not-existing", "an-id").Deploy();
                Assert.Fail("It should not be able to re-deploy an unexisting deployment");
            }
            catch (NotFoundException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById("not-existing", new List<string>() { "an-id" }).Deploy();
                Assert.Fail("It should not be able to re-deploy an unexisting deployment");
            }
            catch (NotFoundException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName("not-existing", "a-name").Deploy();
                Assert.Fail("It should not be able to re-deploy an unexisting deployment");
            }
            catch (NotFoundException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName("not-existing", new List<string>() { "a-name" }).Deploy();
                Assert.Fail("It should not be able to re-deploy an unexisting deployment");
            }
            catch (NotFoundException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testNotValidDeploymentId()
        {
            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(null);
                Assert.Fail("It should not be possible to pass a null deployment id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(null, "an-id");
                Assert.Fail("It should not be possible to pass a null deployment id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById(null, new List<string>() { "an-id" });
                Assert.Fail("It should not be possible to pass a null deployment id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(null, "a-name");
                Assert.Fail("It should not be possible to pass a null deployment id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName(null, new List<string>() { "a-name" });
                Assert.Fail("It should not be possible to pass a null deployment id");
            }
            catch (NotValidException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testRedeployUnexistingDeploymentResource()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            try
            {
                // when
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(deployment.Id, "not-existing-resource.bpmn").Deploy();
                Assert.Fail("It should not be possible to re-deploy a not existing deployment resource");
            }
            catch (NotFoundException)
            {
                // then
                // expected
            }

            try
            {
                // when
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName(deployment.Id, new List<string>() { "not-existing-resource.bpmn" }).Deploy();
                Assert.Fail("It should not be possible to re-deploy a not existing deployment resource");
            }
            catch (NotFoundException)
            {
                // then
                // expected
            }

            try
            {
                // when
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment.Id, "not-existing-resource-id").Deploy();
                Assert.Fail("It should not be possible to re-deploy a not existing deployment resource");
            }
            catch (NotFoundException)
            {
                // then
                // expected
            }

            try
            {
                // when
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById(deployment.Id, new List<string>() { "not-existing-resource-id" }).Deploy();
                Assert.Fail("It should not be possible to re-deploy a not existing deployment resource");
            }
            catch (NotFoundException)
            {
                // then
                // expected
            }

            deleteDeployments(deployment);
        }

        [Test]
        public virtual void testNotValidResource()
        {
            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById("an-id", null);
                Assert.Fail("It should not be possible to pass a null resource id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById("an-id", null);
                Assert.Fail("It should not be possible to pass a null resource id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById("an-id", new List<string>() { });
                Assert.Fail("It should not be possible to pass a null resource id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById("an-id", new List<string>());
                Assert.Fail("It should not be possible to pass a null resource id");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName("an-id", null);
                Assert.Fail("It should not be possible to pass a null resource name");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName("an-id", null);
                Assert.Fail("It should not be possible to pass a null resource name");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName("an-id", new List<string>() { });
                Assert.Fail("It should not be possible to pass a null resource name");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName("an-id", new List<string>());
                Assert.Fail("It should not be possible to pass a null resource name");
            }
            catch (NotValidException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testRedeployNewDeployment()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c => c.Name == DEPLOYMENT_NAME);

            Assert.NotNull(deployment1.Id);
            //verifyQueryResults(query, 1);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).Deploy();

            // then
            Assert.NotNull(deployment2);
            Assert.NotNull(deployment2.Id);
            Assert.IsFalse(deployment1.Id.Equals(deployment2.Id));

            //verifyQueryResults(query, 2);

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testFailingDeploymentName()
        {
            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).SetNameFromDeployment("a-deployment-id");
                Assert.Fail("Cannot set name() and nameFromDeployment().");
            }
            catch (NotValidException)
            {
                // expected
            }

            try
            {
                repositoryService.CreateDeployment().SetNameFromDeployment("a-deployment-id").Name(DEPLOYMENT_NAME);
                Assert.Fail("Cannot set name() and nameFromDeployment().");
            }
            catch (NotValidException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testRedeployDeploymentName()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            Assert.AreEqual(DEPLOYMENT_NAME, deployment1.Name);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().SetNameFromDeployment(deployment1.Id).AddDeploymentResources(deployment1.Id).Deploy();

            // then
            Assert.NotNull(deployment2);
            Assert.AreEqual(deployment1.Name, deployment2.Name);

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeployDeploymentDifferentName()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            Assert.AreEqual(DEPLOYMENT_NAME, deployment1.Name);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().Name("my-another-deployment").AddDeploymentResources(deployment1.Id).Deploy();

            // then
            Assert.NotNull(deployment2);
            Assert.IsFalse(deployment1.Name.Equals(deployment2.Name));

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeployDeploymentSourcePropertyNotSet()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).Source("my-deployment-source").AddModelInstance(RESOURCE_NAME, model).Deploy();

            Assert.AreEqual("my-deployment-source", deployment1.Source);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).Deploy();

            // then
            Assert.NotNull(deployment2);
            Assert.IsNull(deployment2.Source);

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeploySetDeploymentSourceProperty()
        {
            // given
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).Source("my-deployment-source").AddModelInstance(RESOURCE_NAME, model).Deploy();

            Assert.AreEqual("my-deployment-source", deployment1.Source);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).Source("my-another-deployment-source").Deploy();

            // then
            Assert.NotNull(deployment2);
            Assert.AreEqual("my-another-deployment-source", deployment2.Source);

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeployDeploymentResource()
        {
            // given

            // first deployment
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

            // second deployment
            model = createProcessWithUserTask(PROCESS_KEY);
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).Deploy();

            IResource resource2 = getResourceByName(deployment2.Id, RESOURCE_NAME);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).Deploy();

            // then
            IResource resource3 = getResourceByName(deployment3.Id, RESOURCE_NAME);
            Assert.NotNull(resource3);

            // id
            Assert.NotNull(resource3.Id);
            Assert.IsFalse(resource1.Id.Equals(resource3.Id));

            // deployment id
            Assert.AreEqual(deployment3.Id, resource3.DeploymentId);

            // name
            Assert.AreEqual(resource1.Name, resource3.Name);

            // bytes
            byte[] bytes1 = ((ResourceEntity)resource1).Bytes;
            byte[] bytes2 = ((ResourceEntity)resource2).Bytes;
            byte[] bytes3 = ((ResourceEntity)resource3).Bytes;
            Assert.True(bytes1.SequenceEqual(bytes3));
            Assert.IsFalse(bytes2.SequenceEqual(bytes3));

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployAllDeploymentResources()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // second deployment
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model2).AddModelInstance(RESOURCE_2_NAME, model1).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 3);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployOneDeploymentResourcesByName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployMultipleDeploymentResourcesByName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
            IBpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 1);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
            model3 = createProcessWithUserTask(PROCESS_3_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 2);

            // when (1)
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).AddDeploymentResourceByName(deployment1.Id, RESOURCE_3_NAME).Deploy();

            // then (1)
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 3);

            // when (2)
            IDeployment deployment4 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesByName(deployment2.Id, new List<string>() { RESOURCE_1_NAME, RESOURCE_3_NAME }).Deploy();

            // then (2)
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 4);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 4);

            deleteDeployments(deployment1, deployment2, deployment3, deployment4);
        }

        [Test]
        public virtual void testRedeployOneAndMultipleDeploymentResourcesByName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
            IBpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 1);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
            model3 = createProcessWithUserTask(PROCESS_3_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).AddDeploymentResourcesByName(deployment1.Id, new List<string>() { RESOURCE_2_NAME, RESOURCE_3_NAME }).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 3);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testSameDeploymentResourceByName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).AddDeploymentResourcesByName(deployment1.Id, new List<string>() { RESOURCE_1_NAME }).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployOneDeploymentResourcesById()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            IResource resource = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource.Id).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployMultipleDeploymentResourcesById()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
            IBpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 1);

            IResource resource11 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
            IResource resource13 = getResourceByName(deployment1.Id, RESOURCE_3_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
            model3 = createProcessWithUserTask(PROCESS_3_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 2);

            IResource resource21 = getResourceByName(deployment2.Id, RESOURCE_1_NAME);
            IResource resource23 = getResourceByName(deployment2.Id, RESOURCE_3_NAME);

            // when (1)
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource11.Id).AddDeploymentResourceById(deployment1.Id, resource13.Id).Deploy();

            // then (1)
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 3);

            // when (2)
            IDeployment deployment4 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourcesById(deployment2.Id, new List<string>() { resource21.Id, resource23.Id }).Deploy();

            // then (2)
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 4);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 4);

            deleteDeployments(deployment1, deployment2, deployment3, deployment4);
        }

        [Test]
        public virtual void testRedeployOneAndMultipleDeploymentResourcesById()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
            IBpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 1);

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
            IResource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);
            IResource resource3 = getResourceByName(deployment1.Id, RESOURCE_3_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
            model3 = createProcessWithUserTask(PROCESS_3_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).AddDeploymentResourcesById(deployment1.Id, new List<string>() { resource2.Id, resource3.Id }).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 3);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeploySameDeploymentResourceById()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).AddDeploymentResourcesById(deployment1.Id, new List<string>() { resource1.Id }).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployDeploymentResourceByIdAndName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
            IResource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).AddDeploymentResourceByName(deployment1.Id, resource2.Name).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 3);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployDeploymentResourceByIdAndNameMultiple()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IBpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
            IResource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);

            // second deployment
            model1 = createProcessWithScriptTask(PROCESS_1_KEY);
            model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().AddDeploymentResourcesById(deployment1.Id, new List<string>() { resource1.Id }).AddDeploymentResourcesByName(deployment1.Id, new List<string>() { resource2.Id }).Deploy();

            // then
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 3);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 3);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeployments()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment1.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment2.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResources(deployment1.Id).AddDeploymentResources(deployment2.Id).Deploy();

            Assert.AreEqual(2, repositoryService.GetDeploymentResources(deployment3.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeploymentsById()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment1.Id).Count);
            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment2.Id).Count);
            IResource resource2 = getResourceByName(deployment2.Id, RESOURCE_2_NAME);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResourceById(deployment1.Id, resource1.Id).AddDeploymentResourceById(deployment2.Id, resource2.Id).Deploy();

            Assert.AreEqual(2, repositoryService.GetDeploymentResources(deployment3.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeploymentsByName()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment1.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment2.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).AddDeploymentResourceByName(deployment2.Id, RESOURCE_2_NAME).Deploy();

            Assert.AreEqual(2, repositoryService.GetDeploymentResources(deployment3.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeploymentsByNameAndId()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment1.Id).Count);
            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment2.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // when
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResourceById(deployment1.Id, resource1.Id).AddDeploymentResourceByName(deployment2.Id, RESOURCE_2_NAME).Deploy();

            Assert.AreEqual(2, repositoryService.GetDeploymentResources(deployment3.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeploymentsAddsNewSource()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment1.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_2_NAME, model2).Deploy();

            Assert.AreEqual(1, repositoryService.GetDeploymentResources(deployment2.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 1);

            // when
            IBpmnModelInstance model3 = createProcessWithUserTask(PROCESS_3_KEY);
            IDeployment deployment3 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResources(deployment1.Id).AddDeploymentResources(deployment2.Id).AddModelInstance(RESOURCE_3_NAME, model3).Deploy();

            Assert.AreEqual(3, repositoryService.GetDeploymentResources(deployment3.Id).Count);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_2_KEY), 2);
            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_3_KEY), 1);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testRedeployFormDifferentDeploymentsSameResourceName()
        {
            // given
            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            // second deployment
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_1_NAME, model2).Deploy();

            // when
            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-3").AddDeploymentResources(deployment1.Id).AddDeploymentResources(deployment2.Id).Deploy();
                Assert.Fail("It should not be possible to deploy different resources with same name.");
            }
            catch (NotValidException)
            {
                // expected
            }

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeployAndAddNewResourceWithSameName()
        {
            // given
            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-1").AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            // when
            IBpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

            try
            {
                repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME + "-2").AddModelInstance(RESOURCE_1_NAME, model2).AddDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).Deploy();
                Assert.Fail("It should not be possible to deploy different resources with same name.");
            }
            catch (NotValidException)
            {
                // expected
            }

            deleteDeployments(deployment1);
        }

        [Test]
        public virtual void testRedeployEnableDuplcateChecking()
        {
            // given
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            IBpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
            IDeployment deployment1 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_1_NAME, model1).Deploy();

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            // when
            IDeployment deployment2 = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddDeploymentResources(deployment1.Id).EnableDuplicateFiltering(true).Deploy();

            Assert.AreEqual(deployment1.Id, deployment2.Id);

            //verifyQueryResults(query.Where(c=>c.Key == PROCESS_1_KEY), 1);

            deleteDeployments(deployment1);
        }

        [Test]
        public virtual void testSimpleProcessApplicationDeployment()
        {
            // given
            EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
            var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

            // when
            var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).Deploy() as IProcessApplicationDeployment;

            // then
            // registration was performed:
            IProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
            Assert.AreEqual(1, deploymentIds.Count);
            Assert.True(deploymentIds.Contains(deployment2.Id));

            deleteDeployments(deployment1, deployment2);
        }

        [Test]
        public virtual void testRedeployProcessApplicationDeploymentResumePreviousVersions()
        {
            // given
            EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

            // first deployment
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
            var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

            // second deployment
            model = createProcessWithUserTask(PROCESS_KEY);
            var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

            // when
            var deployment3 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).Deploy() as IProcessApplicationDeployment;

            // then
            // old deployments was resumed
            IProcessApplicationRegistration registration = deployment3.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
            Assert.AreEqual(3, deploymentIds.Count);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        [Test]
        public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentName()
        {
            // given
            EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

            // first deployment
            IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
            //  IProcessApplicationDeployment deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();
            var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

            IResource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

            // second deployment
            model = createProcessWithUserTask(PROCESS_KEY);
            var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

            // when
            var deployment3 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().ResumePreviousVersionsBy(ResumePreviousBy.ResumeByDeploymentName).Name(DEPLOYMENT_NAME).AddDeploymentResourceById(deployment1.Id, resource1.Id).Deploy() as IProcessApplicationDeployment;

            // then
            // old deployment was resumed
            IProcessApplicationRegistration registration = deployment3.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
            Assert.AreEqual(3, deploymentIds.Count);

            deleteDeployments(deployment1, deployment2, deployment3);
        }

        // helper ///////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQuery<IModelElementInstance> query, int countExpected)
        {
            Assert.AreEqual(countExpected, query.Count());
        }

        protected internal virtual void verifyQueryResults(IQueryable<IDeployment> query, int countExpected)
        {
            Assert.AreEqual(countExpected, query.Count());
        }

        protected internal virtual void verifyQueryResults(IQueryable<IProcessDefinition> query, int countExpected)
        {
            Assert.AreEqual(countExpected, query.Count());
        }
        protected internal virtual IResource getResourceByName(string deploymentId, string resourceName)
        {
            IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);

            foreach (IResource resource in resources)
            {
                if (resource.Name.Equals(resourceName))
                {
                    return resource;
                }
            }

            return null;
        }

        protected internal virtual void deleteDeployments(params IDeployment[] deployments)
        {
            foreach (IDeployment deployment in deployments)
            {
                repositoryService.DeleteDeployment(deployment.Id, true);
            }
        }

        protected internal virtual IBpmnModelInstance createProcessWithServiceTask(string key)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).StartEvent().ServiceTask().CamundaExpression("${true}").EndEvent().Done();
        }

        protected internal virtual IBpmnModelInstance createProcessWithUserTask(string key)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).StartEvent().UserTask().EndEvent().Done();
        }

        protected internal virtual IBpmnModelInstance createProcessWithReceiveTask(string key)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).StartEvent().ReceiveTask().EndEvent().Done();
        }

        protected internal virtual IBpmnModelInstance createProcessWithScriptTask(string key)
        {
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).StartEvent().ScriptTask();
            start.ScriptFormat("javascript").ScriptText("return true");
            return start.UserTask().EndEvent().Done();
        }

    }

}