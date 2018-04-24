using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ExternalTaskQueryAuthorizationTest : AuthorizationTest
    {

        protected internal string deploymentId;

        protected internal string instance1Id;
        protected internal string instance2Id;

        [SetUp]
        protected internal override void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml").Id;

            instance1Id = StartProcessInstanceByKey("oneExternalTaskProcess").Id;
            instance2Id = StartProcessInstanceByKey("twoExternalTaskProcess").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 1);
            Assert.AreEqual(instance1Id, query.First().ProcessInstanceId);
        }

        public virtual void testQueryWithReadOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.ReadInstance);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 1);
            Assert.AreEqual(instance1Id, query.First().ProcessInstanceId);
        }

        public virtual void testQueryWithReadInstanceOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadInstanceWithMultiple()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.ReadInstance);
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

            // then
            ////verifyQueryResults(query, 2);
        }
    }

}