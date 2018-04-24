using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class EventSubscriptionAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
        protected internal const string SIGNAL_BOUNDARY_PROCESS_KEY = "signalBoundaryProcess";

        protected internal string deploymentId;
        
        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneMessageBoundaryEventProcess.bpmn20.xml", "resources/api/authorization/signalBoundaryEventProcess.bpmn20.xml").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);

            IEventSubscription eventSubscription = query.First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(ProcessInstanceId, eventSubscription.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);

            IEventSubscription eventSubscription = query.First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(ProcessInstanceId, eventSubscription.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);

            IEventSubscription eventSubscription = query.First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(ProcessInstanceId, eventSubscription.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);

            IEventSubscription eventSubscription = query.First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(ProcessInstanceId, eventSubscription.ProcessInstanceId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;

            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 1);

            IEventSubscription eventSubscription = query.First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(ProcessInstanceId, eventSubscription.ProcessInstanceId);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        protected internal virtual void verifyQueryResults(IQueryable<IEventSubscription> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}