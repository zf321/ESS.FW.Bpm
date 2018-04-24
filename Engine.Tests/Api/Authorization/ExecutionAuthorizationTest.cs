using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{

    public class ExecutionAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
        protected internal const string MESSAGE_BOUNDARY_PROCESS_KEY = "messageBoundaryProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/messageBoundaryEventProcess.bpmn20.xml").Id;
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
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);

            IExecution execution = query.First();
            Assert.NotNull(execution);
            Assert.AreEqual(ProcessInstanceId, execution.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);

            IExecution execution = query.First();
            Assert.NotNull(execution);
            Assert.AreEqual(ProcessInstanceId, execution.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);

            IExecution execution = query.First();
            Assert.NotNull(execution);
            Assert.AreEqual(ProcessInstanceId, execution.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);

            IExecution execution = query.First();
            Assert.NotNull(execution);
            Assert.AreEqual(ProcessInstanceId, execution.ProcessInstanceId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;

            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 1);

            IExecution execution = query.First();
            Assert.NotNull(execution);
            Assert.AreEqual(ProcessInstanceId, execution.ProcessInstanceId);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 11);
        }

        public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IExecution> query = runtimeService.CreateExecutionQuery();

            // then
            //verifyQueryResults(query, 11);
        }

        protected internal virtual void verifyQueryResults(IQueryable<IExecution> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }
    }

}