using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricDetailAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
        protected internal const string CASE_KEY = "oneTaskCase";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/messageStartEventProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic variable update query (standalone task) /////////////////////////////////////////////

        public virtual void testQueryAfterStandaloneTaskVariableUpdates()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 1);

            deleteTask(taskId, true);
        }

        // historic variable update query (process task) /////////////////////////////////////////////

        public virtual void testSimpleVariableUpdateQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleVariableUpdateQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleVariableUpdateQueryMultiple()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleVariableUpdateQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 1);
        }

        // historic variable update query (multiple process instances) ///////////////////////////////////////////

        public virtual void testVariableUpdateQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testVariableUpdateQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testVariableUpdateQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 7);
        }

        // historic variable update query (case variables) /////////////////////////////////////////////

        public virtual void testQueryAfterCaseVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 1);
        }

        // historic variable update query (mixed) ////////////////////////////////////

        public virtual void testMixedQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            disableAuthorization();
            taskService.SetVariables("one", Variables);
            taskService.SetVariables("two", Variables);
            taskService.SetVariables("three", Variables);
            taskService.SetVariables("four", Variables);
            taskService.SetVariables("five", Variables);
            enableAuthorization();

            createCaseInstanceByKey(CASE_KEY, Variables);
            createCaseInstanceByKey(CASE_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 7);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            disableAuthorization();
            taskService.SetVariables("one", Variables);
            taskService.SetVariables("two", Variables);
            taskService.SetVariables("three", Variables);
            taskService.SetVariables("four", Variables);
            taskService.SetVariables("five", Variables);
            enableAuthorization();

            createCaseInstanceByKey(CASE_KEY, Variables);
            createCaseInstanceByKey(CASE_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 10);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            disableAuthorization();
            taskService.SetVariables("one", Variables);
            taskService.SetVariables("two", Variables);
            taskService.SetVariables("three", Variables);
            taskService.SetVariables("four", Variables);
            taskService.SetVariables("five", Variables);
            enableAuthorization();

            createCaseInstanceByKey(CASE_KEY, Variables);
            createCaseInstanceByKey(CASE_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/;

            // then
            //verifyQueryResults(query, 14);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        // historic form field query //////////////////////////////////////////////////////

        public virtual void testSimpleFormFieldQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleFormFieldQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleFormFieldQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic variable update query (multiple process instances) ///////////////////////////////////////////

        public virtual void testFormFieldQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testFormFieldQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testFormFieldQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

            // then
            //verifyQueryResults(query, 7);
        }

        // historic detail query (variable update + form field) //////////

        public virtual void testDetailQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testDetailQueryWithReadHistoryOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testDetailQueryWithReadHistoryOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        // Delete deployment (cascade = false)

        public virtual void testQueryAfterDeletingDeployment()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY);
            taskId = selectSingleTask().Id;
            disableAuthorization();
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);
            enableAuthorization();

            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            StartProcessInstanceByKey(PROCESS_KEY, Variables);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }
            enableAuthorization();

            disableAuthorization();
            repositoryService.DeleteDeployment(deploymentId);
            enableAuthorization();

            // when
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

            // then
            //verifyQueryResults(query, 7);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery()
                .ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricDetail> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}