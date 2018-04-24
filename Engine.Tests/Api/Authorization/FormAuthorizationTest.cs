using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class FormAuthorizationTest : AuthorizationTest
    {

        protected internal const string FORM_KEY_PROCESS_KEY = "formKeyProcess";
        protected internal const string RENDERED_FORM_PROCESS_KEY = "renderedFormProcess";
        protected internal const string CASE_KEY = "oneTaskCase";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/formKeyProcess.bpmn20.xml", "resources/api/authorization/renderedFormProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // get start form data ///////////////////////////////////////////

        public virtual void testGetStartFormDataWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;

            try
            {
                // when
                formService.GetStartFormData(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get start form data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetStartFormData()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.Read);

            // when
            IStartFormData startFormData = formService.GetStartFormData(processDefinitionId);

            // then
            Assert.NotNull(startFormData);
            Assert.AreEqual("aStartFormKey", startFormData.FormKey);
        }

        // get rendered start form /////////////////////////////////////

        public virtual void testGetRenderedStartFormWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;

            try
            {
                // when
                formService.GetRenderedStartForm(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get start form data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetRenderedStartForm()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.Read);

            // when
            object renderedStartForm = formService.GetRenderedStartForm(processDefinitionId);

            // then
            Assert.NotNull(renderedStartForm);
        }

        // get start form variables //////////////////////////////////

        public virtual void testGetStartFormVariablesWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;

            try
            {
                // when
                formService.GetStartFormVariables(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get start form data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetStartFormVariables()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(RENDERED_FORM_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.Read);

            // when
            IVariableMap variables = formService.GetStartFormVariables(processDefinitionId);

            // then
            Assert.NotNull(variables);
            Assert.AreEqual(1, variables.Count);
        }

        // submit start form /////////////////////////////////////////

        public virtual void testSubmitStartFormWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;

            try
            {
                // when
                formService.SubmitStartForm(processDefinitionId, null);
                Assert.Fail("Exception expected: It should not possible to submit a start form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Create.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSubmitStartFormWithCreatePermissionOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                formService.SubmitStartForm(processDefinitionId, null);
                Assert.Fail("Exception expected: It should not possible to submit a start form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.CreateInstance.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSubmitStartFormWithCreateInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                formService.SubmitStartForm(processDefinitionId, null);
                Assert.Fail("Exception expected: It should not possible to submit a start form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Create.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSubmitStartForm()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            IProcessInstance instance = formService.SubmitStartForm(processDefinitionId, null);

            // then
            Assert.NotNull(instance);
        }

        // get task form data (standalone task) /////////////////////////////////

        public virtual void testStandaloneTaskGetTaskFormDataWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                formService.GetTaskFormData(taskId);
                Assert.Fail("Exception expected: It should not possible to get task form data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetTaskFormData()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            // Standalone task, no ITaskFormData available
            Assert.IsNull(taskFormData);

            deleteTask(taskId, true);
        }

        // get task form data (process task) /////////////////////////////////

        public virtual void testProcessTaskGetTaskFormDataWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                formService.GetTaskFormData(taskId);
                Assert.Fail("Exception expected: It should not possible to get task form data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadTask.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetTaskFormDataWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            Assert.NotNull(taskFormData);
        }

        public virtual void testProcessTaskGetTaskFormDataWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            Assert.NotNull(taskFormData);
        }

        public virtual void testProcessTaskGetTaskFormData()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            Assert.NotNull(taskFormData);
        }

        // get task form data (case task) /////////////////////////////////

        public virtual void testCaseTaskGetTaskFormData()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            ITaskFormData taskFormData = formService.GetTaskFormData(taskId);

            // then
            Assert.NotNull(taskFormData);
        }

        // get rendered task form (standalone task) //////////////////

        public virtual void testStandaloneTaskGetTaskRenderedFormWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                formService.GetRenderedTaskForm(taskId);
                Assert.Fail("Exception expected: It should not possible to get rendered task form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetTaskRenderedForm()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            try
            {
                // when
                // Standalone task, no ITaskFormData available
                formService.GetRenderedTaskForm(taskId);
            }
            catch (NullValueException)
            {
            }

            deleteTask(taskId, true);
        }

        // get rendered task form (process task) /////////////////////////////////

        public virtual void testProcessTaskGetRenderedTaskFormWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                formService.GetRenderedTaskForm(taskId);
                Assert.Fail("Exception expected: It should not possible to get rendered task form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadTask.ToString(), message);
                AssertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetRenderedTaskFormWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            object taskForm = formService.GetRenderedTaskForm(taskId);

            // then
            Assert.NotNull(taskForm);
        }

        public virtual void testProcessTaskGetRenderedTaskFormWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            object taskForm = formService.GetRenderedTaskForm(taskId);

            // then
            Assert.NotNull(taskForm);
        }

        public virtual void testProcessTaskGetRenderedTaskForm()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            object taskForm = formService.GetRenderedTaskForm(taskId);

            // then
            Assert.NotNull(taskForm);
        }

        // get rendered task form (case task) /////////////////////////////////

        public virtual void testCaseTaskGetRenderedTaskForm()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            object taskForm = formService.GetRenderedTaskForm(taskId);

            // then
            Assert.IsNull(taskForm);
        }

        // get task form variables (standalone task) ////////////////////////

        public virtual void testStandaloneTaskGetTaskFormVariablesWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                formService.GetTaskFormVariables(taskId);
                Assert.Fail("Exception expected: It should not possible to get task form variables");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetTaskFormVariables()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IVariableMap variables = formService.GetTaskFormVariables(taskId);

            // then
            Assert.NotNull(variables);

            deleteTask(taskId, true);
        }

        // get task form variables (process task) /////////////////////////////////

        public virtual void testProcessTaskGetTaskFormVariablesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                formService.GetTaskFormVariables(taskId);
                Assert.Fail("Exception expected: It should not possible to get task form variables");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadTask.ToString(), message);
                AssertTextPresent(RENDERED_FORM_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetTaskFormVariablesWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IVariableMap variables = formService.GetTaskFormVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.AreEqual(1, variables.Count);
        }

        public virtual void testProcessTaskGetTaskFormVariablesWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = formService.GetTaskFormVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.AreEqual(1, variables.Count);
        }

        public virtual void testProcessTaskGetTaskFormVariables()
        {
            // given
            StartProcessInstanceByKey(RENDERED_FORM_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, RENDERED_FORM_PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = formService.GetTaskFormVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.AreEqual(1, variables.Count);
        }

        // get task form variables (case task) /////////////////////////////////

        public virtual void testCaseTaskGetTaskFormVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            IVariableMap variables = formService.GetTaskFormVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.AreEqual(0, variables.Count);
        }

        // submit task form (standalone task) ////////////////////////////////

        public virtual void testStandaloneTaskSubmitTaskFormWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                formService.SubmitTaskForm(taskId, null);
                Assert.Fail("Exception expected: It should not possible to submit a task form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSubmitTaskForm()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);

            deleteTask(taskId, true);
        }

        // submit task form (process task) ////////////////////////////////

        public virtual void testProcessTaskSubmitTaskFormWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                formService.SubmitTaskForm(taskId, null);
                Assert.Fail("Exception expected: It should not possible to submit a task form");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSubmitTaskFormWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskSubmitTaskFormWithUpdateTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskSubmitTaskForm()
        {
            // given
            StartProcessInstanceByKey(FORM_KEY_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        // submit task form (case task) ////////////////////////////////

        public virtual void testCaseTaskSubmitTaskForm()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            formService.SubmitTaskForm(taskId, null);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        // get start form key ////////////////////////////////////////

        public virtual void testGetStartFormKeyWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;

            try
            {
                // when
                formService.GetStartFormKey(processDefinitionId);
                Assert.Fail("Exception expected: It should not possible to get a start form key");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetStartFormKey()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.Read);

            // when
            string formKey = formService.GetStartFormKey(processDefinitionId);

            // then
            Assert.AreEqual("aStartFormKey", formKey);
        }

        // get task form key ////////////////////////////////////////

        public virtual void testGetTaskFormKeyWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;

            try
            {
                // when
                formService.GetTaskFormKey(processDefinitionId, "task");
                Assert.Fail("Exception expected: It should not possible to get a task form key");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(FORM_KEY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetTaskFormKey()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(FORM_KEY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, FORM_KEY_PROCESS_KEY, userId, Permissions.Read);

            // when
            string formKey = formService.GetTaskFormKey(processDefinitionId, "task");

            // then
            Assert.AreEqual("aTaskFormKey", formKey);
        }

    }

}