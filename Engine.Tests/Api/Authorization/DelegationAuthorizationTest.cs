using System;
using System.Linq;
using Engine.Tests.Api.Authorization.Service;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    [TestFixture]
    public class DelegationAuthorizationTest : AuthorizationTest
    {

        public const string DEFAULT_PROCESS_KEY = "process";

        [SetUp]
        protected internal void setUp()
        {
            base.setUp();
            MyDelegationService.clearProperties();
            processEngineConfiguration.SetAuthorizationEnabledForCustomCode(false);
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTask()
        {
            // given
            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myDelegate", new ExecuteQueryDelegate());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myDelegate", new ExecuteCommandDelegate());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myDelegate", new ExecuteQueryDelegate());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myDelegate", new ExecuteCommandDelegate());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTask()
        {
            // given
            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myBehavior", new MyServiceTaskActivityBehaviorExecuteQuery());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myBehavior", new MyServiceTaskActivityBehaviorExecuteCommand());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testSignallableActivityBehaviorAsClass()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 4);
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.Signal(ProcessInstanceId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testSignallableActivityBehaviorAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("activityBehavior", new MyServiceTaskActivityBehaviorExecuteQuery());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 4);
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.Signal(ProcessInstanceId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTask()
        {
            // given
            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteQueryListener());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteCommandListener());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteQueryListener());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteCommandListener());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesCommandAfterUserCompletesTask()
        {
            // given
            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteQueryTaskListener());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteCommandTaskListener());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesQueryAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteQueryTaskListener());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerExecutesCommandAfterUserCompletesTaskAsExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myListener", new ExecuteCommandTaskListener());

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testTaskAssigneeExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myTaskService", new MyTaskService());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testScriptTaskExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            ITask task = selectAnyTask();

            string taskId = task.Id;
            string ProcessInstanceId = task.ProcessInstanceId;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(5l, variableCount.Value);

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptTaskExecutesCommandAfterUserCompletesTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IVariableInstance variableUser = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId)/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptExecutionListenerExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            ITask task = selectAnyTask();

            string taskId = task.Id;
            string ProcessInstanceId = task.ProcessInstanceId;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(5l, variableCount.Value);

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptExecutionListenerExecutesCommandAfterUserCompletesTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IVariableInstance variableUser = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId)/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptTaskListenerExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            ITask task = selectAnyTask();

            string taskId = task.Id;
            string ProcessInstanceId = task.ProcessInstanceId;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(5l, variableCount.Value);

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptTaskListenerExecutesCommandAfterUserCompletesTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IVariableInstance variableUser = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId)/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptConditionExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            ITask task = selectAnyTask();

            string taskId = task.Id;
            string ProcessInstanceId = task.ProcessInstanceId;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(5l, variableCount.Value);

            enableAuthorization();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testScriptConditionExecutesCommandAfterUserCompletesTask()
        public virtual void testScriptConditionExecutesCommandAfterUserCompletesTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IVariableInstance variableUser = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId)/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptIoMappingExecutesQueryAfterUserCompletesTask()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            ITask task = selectAnyTask();

            string taskId = task.Id;
            string ProcessInstanceId = task.ProcessInstanceId;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(5l, variableCount.Value);

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testScriptIoMappingExecutesCommandAfterUserCompletesTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();

            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

            IVariableInstance variableUser = query/*.VariableName("userId")*/.First();
            Assert.NotNull(variableUser);
            Assert.AreEqual(userId, variableUser.Value);

            IVariableInstance variableCount = query/*.VariableName("Count")*/.First();
            Assert.NotNull(variableCount);
            Assert.AreEqual(1l, variableCount.Value);

            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery().Count());

            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testCustomStartFormHandlerExecutesQuery()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string processDefinitionId = selectProcessDefinitionByKey(DEFAULT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, DEFAULT_PROCESS_KEY, userId, Permissions.Read);

            // when
            IStartFormData startFormData = formService.GetStartFormData(processDefinitionId);

            // then
            Assert.NotNull(startFormData);

            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testCustomTaskFormHandlerExecutesQuery()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            Assert.NotNull(taskFormData);

            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment(new string[] { "resources/api/authorization/DelegationAuthorizationTest.TestCustomStartFormHandlerExecutesQuery.bpmn20.xml" })]
        public virtual void testSubmitCustomStartFormHandlerExecutesQuery()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string processDefinitionId = selectProcessDefinitionByKey(DEFAULT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, DEFAULT_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            formService.SubmitStartForm(processDefinitionId, null);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment(new string[] { "resources/api/authorization/DelegationAuthorizationTest.TestCustomTaskFormHandlerExecutesQuery.bpmn20.xml" })]
        public virtual void testSubmitCustomTaskFormHandlerExecutesQuery()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testCustomFormFieldValidator()
        {
            // given
            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment]
        public virtual void testCustomFormFieldValidatorAsDelegateExpression()
        {
            // given
            processEngineConfiguration.Beans.Add("myValidator", new MyFormFieldValidator());

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment(new string[] { "resources/api/authorization/DelegationAuthorizationTest.TestJavaDelegateExecutesQueryAfterUserCompletesTask.bpmn20.xml" })]
        public virtual void testPerformAuthorizationCheckByExecutingQuery()
        {
            // given
            processEngineConfiguration.AuthorizationEnabledForCustomCode = true;

            startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
            string taskId = selectAnyTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            Assert.AreEqual(Convert.ToInt64(0), MyDelegationService.INSTANCES_COUNT);
        }

        [Test]
        [Deployment(new string[] { "resources/api/authorization/DelegationAuthorizationTest.TestJavaDelegateExecutesCommandAfterUserCompletesTask.bpmn20.xml" })]
        public virtual void testPerformAuthorizationCheckByExecutingCommand()
        {
            // given
            processEngineConfiguration.AuthorizationEnabledForCustomCode = true;

            StartProcessInstanceByKey(DEFAULT_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            try
            {
                // when
                taskService.Complete(taskId);
                Assert.Fail("Exception expected: It should not be possible to execute the command inside JavaDelegate");
            }
            catch (AuthorizationException)
            {
            }

            // then
            Assert.NotNull(MyDelegationService.CURRENT_AUTHENTICATION);
            Assert.AreEqual(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

            disableAuthorization();
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            enableAuthorization();
        }

        [Test]
        [Deployment]
        public virtual void testTaskListenerOnCreateAssignsTask()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when (1)
            taskService.Complete(taskId);

            // then (1)
            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            // when (2)
            taskService.Complete(task.Id);

            // then (2)
            AssertProcessEnded(ProcessInstanceId);
        }

        // helper /////////////////////////////////////////////////////////////////////////

        protected internal virtual void startProcessInstancesByKey(string key, int Count)
        {
            for (int i = 0; i < Count; i++)
            {
                StartProcessInstanceByKey(key);
            }
        }

        protected internal virtual ITask selectAnyTask()
        {
            disableAuthorization();
            ITask task = taskService.CreateTaskQuery().First();
            enableAuthorization();
            return task;
        }

    }

}