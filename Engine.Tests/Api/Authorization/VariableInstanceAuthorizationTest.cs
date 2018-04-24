

//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Runtime;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Authorization
//{

//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class VariableInstanceAuthorizationTest : AuthorizationTest
//    {

//        protected internal const string PROCESS_KEY = "oneTaskProcess";
//        protected internal const string CASE_KEY = "oneTaskCase";

//        protected internal string deploymentId;

//        [SetUp]
//        public void setUp()
//        {
//            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn").Id;
//            base.setUp();
//        }

//        [TearDown]
//        public void tearDown()
//        {
//            base.TearDown();
//            DeleteDeployment(deploymentId);
//        }

//        public virtual void testProcessVariableQueryWithoutAuthorization()
//        {
//            // given
//            StartProcessInstanceByKey(PROCESS_KEY, Variables);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(null);

//            // then
//            //verifyQueryResults(query.Count(), 0);
//        }

//        public virtual void testCaseVariableQueryWithoutAuthorization()
//        {
//            // given
//            createCaseInstanceByKey(CASE_KEY, Variables);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testProcessLocalTaskVariableQueryWithoutAuthorization()
//        {
//            // given
//            StartProcessInstanceByKey(PROCESS_KEY);
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 0);
//        }

//        public virtual void testCaseLocalTaskVariableQueryWithoutAuthorization()
//        {
//            // given
//            createCaseInstanceByKey(CASE_KEY);
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testStandaloneTaskVariableQueryWithoutAuthorization()
//        {
//            // given
//            string taskId = "myTask";
//            createTask(taskId);
//            setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 0);

//            deleteTask(taskId, true);
//        }

//        public virtual void testProcessVariableQueryWithReadPermissionOnProcessInstance()
//        {
//            // given
//            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
//            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);

//            IVariableInstance variable = query.First();
//            Assert.NotNull(variable);
//            Assert.AreEqual(ProcessInstanceId, variable.ProcessInstanceId);
//        }

//        public virtual void testProcessVariableQueryWithReadInstancesPermissionOnOneTaskProcess()
//        {
//            // given
//            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
//            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);

//            IVariableInstance variable = query.First();
//            Assert.NotNull(variable);
//            Assert.AreEqual(ProcessInstanceId, variable.ProcessInstanceId);
//        }

//        public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnTask()
//        {
//            // given
//            StartProcessInstanceByKey(PROCESS_KEY);
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
//            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testProcessLocalTaskVariableQueryWithMultiple()
//        {
//            // given
//            StartProcessInstanceByKey(PROCESS_KEY);
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
//            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);
//            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnProcessInstance()
//        {
//            // given
//            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
//            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);

//            IVariableInstance variable = query.First();
//            Assert.NotNull(variable);
//            Assert.AreEqual(ProcessInstanceId, variable.ProcessInstanceId);
//        }

//        public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnOneProcessTask()
//        {
//            // given
//            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
//            string taskId = selectSingleTask().Id;
//            setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
//            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);

//            IVariableInstance variable = query.First();
//            Assert.NotNull(variable);
//            Assert.AreEqual(ProcessInstanceId, variable.ProcessInstanceId);
//        }

//        public virtual void testStandaloneTaskVariableQueryWithReadPermissionOnTask()
//        {
//            // given
//            string taskId = "myTask";
//            createTask(taskId);
//            setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
//            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

//            // when
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            //verifyQueryResults(query, 1);

//            deleteTask(taskId, true);
//        }

//        public virtual void testMixedVariables()
//        {
//            // given
//            string taskId = "myTask";
//            createTask(taskId);
//            setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

//            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).ProcessInstanceId;

//            createCaseInstanceByKey(CASE_KEY, Variables);

//            // when (1)
//            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

//            // then (1)
//            //verifyQueryResults(query, 1);

//            // when (2)
//            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

//            // then (2)
//            //verifyQueryResults(query, 2);

//            // when (3)
//            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

//            // then (3)
//            //verifyQueryResults(query, 3);

//            deleteTask(taskId, true);
//        }

//        // helper ////////////////////////////////////////////////////////////////

//        protected internal virtual void verifyQueryResults(IQueryable<IVariableInstance> query, int countExpected)
//        {
//            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
//            //verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
//        }

//    }

//}