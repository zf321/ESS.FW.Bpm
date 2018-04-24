using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class TaskAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal const string CASE_KEY = "oneTaskCase";
        protected internal const string DEMO_ASSIGNEE_PROCESS_KEY = "demoAssigneeProcess";
        protected internal const string CANDIDATE_USERS_PROCESS_KEY = "candidateUsersProcess";
        protected internal const string CANDIDATE_GROUPS_PROCESS_KEY = "candidateGroupsProcess";
        protected internal const string INVALID_PERMISSION = "invalidPermission";
        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn", "resources/api/authorization/oneTaskProcess.bpmn20.xml", "resources/api/authorization/candidateUsersProcess.bpmn20.xml", "resources/api/authorization/candidateGroupsProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // task query ///////////////////////////////////////////////////////

        public virtual void testSimpleQueryWithTaskInsideProcessWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnOneTaskProcess()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithTaskInsideProcessWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            disableAuthorization();
            string taskId = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey==PROCESS_KEY).First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnOneTaskProcess()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
            StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testQueryWithTaskInsideCaseWithoutAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithStandaloneTaskWithoutAuthorization()
        {
            // given
            string taskId = "NewTask";
            createTask(taskId);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 0);

            deleteTask(taskId, true);
        }

        public virtual void testQueryWithStandaloneTaskWithReadPermissionOnTask()
        {
            // given
            string taskId = "NewTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // then
            //verifyQueryResults(query, 1);

            deleteTask(taskId, true);
        }

        // new task /////////////////////////////////////////////////////////////

        public virtual void testNewTaskWithoutAuthorization()
        {
            // given

            try
            {
                // when
                taskService.NewTask();
                Assert.Fail("Exception expected: It should not be possible to create a new task.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'Task'", e.Message);
            }
        }

        public virtual void testNewTask()
        {
            // given
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            ITask task = taskService.NewTask();

            // then
            Assert.NotNull(task);
        }

        // save task (insert) //////////////////////////////////////////////////////////

        public virtual void testSaveTaskInsertWithoutAuthorization()
        {
            // given
            TaskEntity task = TaskEntity.Create();

            try
            {
                // when
                taskService.SaveTask(task);
                Assert.Fail("Exception expected: It should not be possible to save a task.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'Task'", e.Message);
            }
        }

        public virtual void testSaveTaskInsert()
        {
            // given
            TaskEntity task = TaskEntity.Create();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            taskService.SaveTask(task);

            // then
            task = (TaskEntity)selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            string taskId = task.Id;
            deleteTask(taskId, true);
        }

        public virtual void testSaveAndUpdateTaskWithTaskAssignPermission()
        {
            // given
            TaskEntity task = TaskEntity.Create();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.TaskAssign);

            // when
            taskService.SaveTask(task);

            task.Delegate("demoNew");

            taskService.SaveTask(task);

            // then
            task = (TaskEntity)selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demoNew", task.Assignee);

            string taskId = task.Id;
            deleteTask(taskId, true);
        }

        // save (standalone) task (update) //////////////////////////////////////////////////////////

        public virtual void testSaveStandaloneTaskUpdateWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            ITask task = selectSingleTask();

            try
            {
                // when
                taskService.SaveTask(task);
                Assert.Fail("Exception expected: It should not be possible to save a task.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign'", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testSaveStandaloneTaskUpdate()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        // save (process) task (update) //////////////////////////////////////////////////////////

        public virtual void testSaveProcessTaskUpdateWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();

            try
            {
                // when
                taskService.SaveTask(task);
                Assert.Fail("Exception expected: It should not be possible to save a task.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(task.Id, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSaveProcessTaskUpdateWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, task.Id, userId, Permissions.Update);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, task.Id, userId, Permissions.TaskAssign);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testSaveProcessTaskUpdateWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testSaveProcessTaskUpdateWithUpdateTasksPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // save (case) task (update) //////////////////////////////////////////////////////////

        public virtual void testSaveCaseTaskUpdate()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            ITask task = selectSingleTask();
            task.Assignee = "demo";

            // when
            taskService.SaveTask(task);

            // then
            task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // Delete task ///////////////////////////////////////////////////////////////////////

        public virtual void testDeleteTaskWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.DeleteTask(taskId);
                Assert.Fail("Exception expected: It should not be possible to Delete a task.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Delete' permission on resource 'myTask' of type 'Task'", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testDeleteTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Delete);

            // when
            taskService.DeleteTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);

            // triggers a db clean up
            deleteTask(taskId, true);
        }

        // Delete tasks ///////////////////////////////////////////////////////////////////////

        public virtual void testDeleteTasksWithoutAuthorization()
        {
            // given
            string firstTaskId = "myTask1";
            createTask(firstTaskId);
            string secondTaskId = "myTask2";
            createTask(secondTaskId);

            try
            {
                // when
                taskService.DeleteTasks(new List<string>() { firstTaskId, secondTaskId});
                Assert.Fail("Exception expected: It should not be possible to Delete tasks.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Delete' permission on resource 'myTask1' of type 'Task'", e.Message);
            }

            deleteTask(firstTaskId, true);
            deleteTask(secondTaskId, true);
        }

        public virtual void testDeleteTasksWithDeletePermissionOnFirstTask()
        {
            // given
            string firstTaskId = "myTask1";
            createTask(firstTaskId);
            createGrantAuthorization(Resources.Task, firstTaskId, userId, Permissions.Delete);

            string secondTaskId = "myTask2";
            createTask(secondTaskId);

            try
            {
                // when
                taskService.DeleteTasks(new List<string>() { firstTaskId, secondTaskId });
                Assert.Fail("Exception expected: It should not be possible to Delete tasks.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Delete' permission on resource 'myTask2' of type 'Task'", e.Message);
            }

            deleteTask(firstTaskId, true);
            deleteTask(secondTaskId, true);
        }

        public virtual void testDeleteTasks()
        {
            // given
            string firstTaskId = "myTask1";
            createTask(firstTaskId);
            string secondTaskId = "myTask2";
            createTask(secondTaskId);

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Delete);

            // when
            taskService.DeleteTasks(new List<string>() { firstTaskId, secondTaskId });

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);

            // triggers a db clean up
            deleteTask(firstTaskId, true);
            deleteTask(secondTaskId, true);
        }

        // set assignee on standalone task /////////////////////////////////////////////

        public virtual void testStandaloneTaskSetAssigneeWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetAssignee(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to set an assignee");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssignee()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssigneeWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        // set assignee on process task /////////////////////////////////////////////

        public virtual void testProcessTaskSetAssigneeWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetAssignee(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to set an assignee");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetAssigneeWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssigneeWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssigneeWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskSetAssignee()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // set assignee on case task /////////////////////////////////////////////

        public virtual void testCaseTaskSetAssignee()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // set owner on standalone task /////////////////////////////////////////////

        public virtual void testStandaloneTaskSetOwnerWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetOwner(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to set an owner");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetOwner()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetOwnerWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);

            deleteTask(taskId, true);
        }

        // set owner on process task /////////////////////////////////////////////

        public virtual void testProcessTaskSetOwnerWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetOwner(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to set an owner");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetOwnerWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwner()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        public virtual void testProcessTaskSetOwnerWithTaskAssignPermission()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        // set owner on case task /////////////////////////////////////////////

        public virtual void testCaseTaskSetOwner()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Owner);
        }

        // add candidate user ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskAddCandidateUserWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.AddCandidateUser(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to add a candidate user");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddCandidateUser()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        // add candidate user ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskAddCandidateUserWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.AddCandidateUser(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to add a candidate user");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskAddCandidateUserWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionRevokeOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createRevokeAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            try
            {
                // when
                taskService.AddCandidateUser(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to add an user identity link");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }
        }

        public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithGrantTaskAssignAndRevokeUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createRevokeAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithTaskAssignPersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateUser()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add candidate user ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskAddCandidateUser()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add candidate group ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskAddCandidateGroupWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.AddCandidateGroup(taskId, "accounting");
                Assert.Fail("Exception expected: It should not be possible to add a candidate group");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddCandidateGroup()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddCandidateGroupWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        // add candidate group ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskAddCandidateGroupWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.AddCandidateGroup(taskId, "accounting");
                Assert.Fail("Exception expected: It should not be possible to add a candidate group");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskAddCandidateGroupWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroup()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermission()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionRevoked()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createRevokeAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add candidate group ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskAddCandidateGroup()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.AddCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add user identity link ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskAddUserIdentityLinkWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to add an user identity link");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddUserIdentityLink()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddUserIdentityLinkWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);


            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        // add user identity link ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskAddUserIdentityLinkWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to add an user identity link");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddUserIdentityLink()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add user identity link ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskAddUserIdentityLink()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.AddUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("demo", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add group identity link ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskAddGroupIdentityLinkWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to add a group identity link");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddGroupIdentityLink()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

            deleteTask(taskId, true);
        }

        // add group identity link ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskAddGroupIdentityLinkWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to add a group identity link");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddGroupIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        public virtual void testProcessTaskAddGroupIdentityLink()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // add group identity link ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskAddGroupIdentityLink()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.AddGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.AreEqual(1, linksForTask.Count);

            IIdentityLink identityLink = linksForTask[0];
            Assert.NotNull(identityLink);

            Assert.AreEqual("accounting", identityLink.GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
        }

        // Delete candidate user ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskDeleteCandidateUserWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.DeleteCandidateUser(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to Delete a candidate user");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteCandidateUser()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteCandidateUserWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        // Delete candidate user ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskDeleteCandidateUserWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.DeleteCandidateUser(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to Delete a candidate user");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskDeleteCandidateUserWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUserWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUserWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateUser()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete candidate user ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskDeleteCandidateUser()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete candidate group ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskDeleteCandidateGroupWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateGroup(taskId, "accounting");

            try
            {
                // when
                taskService.DeleteCandidateGroup(taskId, "accounting");
                Assert.Fail("Exception expected: It should not be possible to Delete a candidate group");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteCandidateGroup()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteCandidateGroupWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        // Delete candidate group ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskDeleteCandidateGroupWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            try
            {
                // when
                taskService.DeleteCandidateGroup(taskId, "accounting");
                Assert.Fail("Exception expected: It should not be possible to Delete a candidate group");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteCandidateGroup()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete candidate group ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskDeleteCandidateGroup()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            // when
            taskService.DeleteCandidateGroup(taskId, "accounting");

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete user identity link ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskDeleteUserIdentityLinkWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to Delete an user identity link");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteUserIdentityLink()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteUserIdentityLinkWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        // Delete user identity link ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskDeleteUserIdentityLinkWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to Delete an user identity link");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLink()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPermission()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete user identity link ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskDeleteUserIdentityLink()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            // when
            taskService.DeleteUserIdentityLink(taskId, "demo", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete group identity link ((standalone) task) /////////////////////////////////////////////

        public virtual void testStandaloneTaskDeleteGroupIdentityLinkWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateGroup(taskId, "accounting");

            try
            {
                // when
                taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to Delete a group identity link");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDeleteGroupIdentityLink()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        // Delete group identity link ((process) task) /////////////////////////////////////////////

        public virtual void testProcessTaskDeleteGroupIdentityLinkWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            try
            {
                // when
                taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);
                Assert.Fail("Exception expected: It should not be possible to Delete a group identity link");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        public virtual void testProcessTaskDeleteGroupIdentityLink()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // Delete group identity link ((case) task) /////////////////////////////////////////////

        public virtual void testCaseTaskDeleteGroupIdentityLink()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateGroup(taskId, "accounting");

            // when
            taskService.DeleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.Candidate);

            // then
            disableAuthorization();
            IList<IIdentityLink> linksForTask = taskService.GetIdentityLinksForTask(taskId);
            enableAuthorization();

            Assert.NotNull(linksForTask);
            Assert.True(linksForTask.Count == 0);
        }

        // get identity links ((standalone) task) ////////////////////////////////////////////////

        public virtual void testStandaloneTaskGetIdentityLinksWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.GetIdentityLinksForTask(taskId);
                Assert.Fail("Exception expected: It should not be possible to get identity links");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Read' permission on resource 'myTask' of type 'Task'", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetIdentityLinks()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);

            deleteTask(taskId, true);
        }

        // get identity links ((process) task) ////////////////////////////////////////////////

        public virtual void testProcessTaskGetIdentityLinksWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            try
            {
                // when
                taskService.GetIdentityLinksForTask(taskId);
                Assert.Fail("Exception expected: It should not be possible to get the identity links");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetIdentityLinksWithReadPersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);
        }

        public virtual void testProcessTaskGetIdentityLinksWithReadPersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);
        }

        public virtual void testProcessTaskGetIdentityLinksWithReadTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);
        }

        public virtual void testProcessTaskGetIdentityLinks()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);
        }

        // get identity links ((case) task) ////////////////////////////////////////////////

        public virtual void testCaseTaskGetIdentityLinks()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            addCandidateUser(taskId, "demo");

            // when
            IList<IIdentityLink> identityLinksForTask = taskService.GetIdentityLinksForTask(taskId);

            // then
            Assert.NotNull(identityLinksForTask);
            Assert.IsFalse(identityLinksForTask.Count == 0);
        }

        // claim (standalone) task ////////////////////////////////////////////////////////////

        public virtual void testStandaloneTaskClaimTaskWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.Claim(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to claim the task.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions:", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskClaimTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskClaimTaskWithTaskWorkPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId,  Permissions.TaskWork);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskClaimTaskWithRevokeTaskWorkPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createRevokeAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            try
            {
                // when
                taskService.Claim(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to complete a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskWork", e.Message);
            }

            deleteTask(taskId, true);
        }

        // claim (process) task ////////////////////////////////////////////////////////////

        public virtual void testProcessTaskClaimTaskWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.Claim(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to claim the task");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskClaimTaskWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithGrantTaskWorkAndRevokeUpdatePermissionsOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);
            createRevokeAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithRevokeTaskWorkPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createRevokeAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            try
            {
                // when
                taskService.Complete(taskId);
                Assert.Fail("Exception expected: It should not be possible to complete a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskWork", e.Message);
            }

        }

        public virtual void testProcessTaskClaimTaskWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithUpdateTasksPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskWork);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

        }

        public virtual void testProcessTaskClaimTaskWithRevokeTaskWorkPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createRevokeAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskWork);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            try
            {
                // when
                taskService.Complete(taskId);
                Assert.Fail("Exception expected: It should not be possible to complete a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskWork", e.Message);
            }

        }

        public virtual void testProcessTaskClaimTask()
        {
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // claim (case) task ////////////////////////////////////////////////////////////

        public virtual void testCaseTaskClaimTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.Claim(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // complete (standalone) task ////////////////////////////////////////////////////////////

        public virtual void testStandaloneTaskCompleteTaskWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.Complete(taskId);
                Assert.Fail("Exception expected: It should not be possible to complete a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskWork", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskCompleteTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);

            if (!processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelNone))
            {
                historyService.DeleteHistoricTaskInstance(taskId);
            }
        }

        public virtual void testStandaloneTaskCompleteWithTaskWorkPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);

            if (!processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelNone))
            {
                historyService.DeleteHistoricTaskInstance(taskId);
            }
        }

        // complete (process) task ////////////////////////////////////////////////////////////

        public virtual void testProcessTaskCompleteTaskWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.Complete(taskId);
                Assert.Fail("Exception expected: It should not be possible to complete a task");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskCompleteTaskWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskCompleteTaskWithTaskWorkPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskWork);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskCompleteTaskWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskCompleteTaskWithUpdateTasksPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskCompleteTaskWithTaskWorkPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskWork);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        public virtual void testProcessTaskCompleteTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        // complete (case) task ////////////////////////////////////////////////////////////

        public virtual void testCaseTaskCompleteTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.Complete(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.IsNull(task);
        }

        // delegate (standalone) task ///////////////////////////////////////////////////////

        public virtual void testStandaloneTaskDelegateTaskWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.DelegateTask(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to delegate a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDelegateTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskDelegateTaskWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);

            deleteTask(taskId, true);
        }

        // delegate (process) task ///////////////////////////////////////////////////////////

        public virtual void testProcessTaskDelegateTaskWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.DelegateTask(taskId, "demo");
                Assert.Fail("Exception expected: It should not be possible to delegate a task");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskDelegateTaskWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithUpdateTasksPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        public virtual void testProcessTaskDelegateTaskWithTaskAssignPermission()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // delegate (case) task /////////////////////////////////////////////////////////////////

        public virtual void testCaseTaskDelegateTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("demo", task.Assignee);
        }

        // resolve (standalone) task ///////////////////////////////////////////////////////

        public virtual void testStandaloneTaskResolveTaskWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.ResolveTask(taskId);
                Assert.Fail("Exception expected: It should not be possible to resolve a task");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskWork", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskResolveTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);

            deleteTask(taskId, true);
        }

        // delegate (process) task ///////////////////////////////////////////////////////////

        public virtual void testProcessTaskResolveTaskWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.ResolveTask(taskId);
                Assert.Fail("Exception expected: It should not be possible to resolve a task");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskResolveTaskWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);
        }

        public virtual void testProcessTaskResolveTaskWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);
        }

        public virtual void testProcessTaskResolveTaskWithUpdateTasksPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);
        }

        public virtual void testProcessTaskResolveTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);
        }

        // delegate (case) task /////////////////////////////////////////////////////////////////

        public virtual void testCaseTaskResolveTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, userId);
            delegateTask(taskId, "demo");

            // when
            taskService.ResolveTask(taskId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(userId, task.Assignee);
        }

        // set priority on standalone task /////////////////////////////////////////////

        public virtual void testStandaloneTaskSetPriorityWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetPriority(taskId, 80);
                Assert.Fail("Exception expected: It should not be possible to set a priority");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have one of the following permissions: 'Permissions.TaskAssign'", e.Message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetPriority()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetPriorityWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);

            deleteTask(taskId, true);
        }

        // set priority on process task /////////////////////////////////////////////

        public virtual void testProcessTaskSetPriorityWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetPriority(taskId, 80);
                Assert.Fail("Exception expected: It should not be possible to set a priority");
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
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetPriorityWithUpdatePersmissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithUpdatePersmissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.TaskAssign);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithUpdateTasksPersmissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriority()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        public virtual void testProcessTaskSetPriorityWithTaskAssignPermission()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.TaskAssign);

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        // set priority on case task /////////////////////////////////////////////

        public virtual void testCaseTaskSetPriority()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetPriority(taskId, 80);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual(80, task.Priority);
        }

        // get sub tasks ((standalone) task) ////////////////////////////////////

        public virtual void testStandaloneTaskGetSubTasksWithoutAuthorization()
        {
            // given
            string parentTaskId = "parentTaskId";
            createTask(parentTaskId);

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.True(subTasks.Count == 0);

            deleteTask(parentTaskId, true);
        }

        public virtual void testStandaloneTaskGetSubTasksWithReadPermissionOnSub1()
        {
            // given
            string parentTaskId = "parentTaskId";
            createTask(parentTaskId);

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, "sub1", userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(1, subTasks.Count);

            Assert.AreEqual("sub1", subTasks[0].Id);

            deleteTask(parentTaskId, true);
        }

        public virtual void testStandaloneTaskGetSubTasks()
        {
            // given
            string parentTaskId = "parentTaskId";
            createTask(parentTaskId);

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(2, subTasks.Count);

            deleteTask(parentTaskId, true);
        }

        // get sub tasks ((process) task) ////////////////////////////////////

        public virtual void testProcessTaskGetSubTasksWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.True(subTasks.Count == 0);
        }

        public virtual void testProcessTaskGetSubTasksWithReadPermissionOnSub1()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, "sub1", userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(1, subTasks.Count);

            Assert.AreEqual("sub1", subTasks[0].Id);
        }

        public virtual void testProcessTaskGetSubTasks()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(2, subTasks.Count);
        }

        // get sub tasks ((case) task) ////////////////////////////////////

        public virtual void testCaseTaskGetSubTasksWithoutAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.True(subTasks.Count == 0);
        }

        public virtual void testCaseTaskGetSubTasksWithReadPermissionOnSub1()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, "sub1", userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(1, subTasks.Count);

            Assert.AreEqual("sub1", subTasks[0].Id);
        }

        public virtual void testCaseTaskGetSubTasks()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string parentTaskId = selectSingleTask().Id;

            disableAuthorization();
            ITask sub1 = taskService.NewTask("sub1");
            sub1.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub1);

            ITask sub2 = taskService.NewTask("sub2");
            sub2.ParentTaskId = parentTaskId;
            taskService.SaveTask(sub2);
            enableAuthorization();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<ITask> subTasks = taskService.GetSubTasks(parentTaskId);

            // then
            Assert.IsFalse(subTasks.Count == 0);
            Assert.AreEqual(2, subTasks.Count);
        }

        // clear authorization ((standalone) task) ////////////////////////

        public virtual void testStandaloneTaskClearAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == taskId).First();
            enableAuthorization();
            Assert.NotNull(authorization);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == taskId).First();
            enableAuthorization();

            Assert.IsNull(authorization);

            deleteTask(taskId, true);
        }

        // clear authorization ((process) task) ////////////////////////

        public virtual void testProcessTaskClearAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == taskId).First();
            enableAuthorization();
            Assert.NotNull(authorization);

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == taskId).First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // set assignee -> an authorization is available (standalone task) /////////////////////////////////////////

        public virtual void testStandaloneTaskSetAssigneeCreateNewAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssigneeUpdateAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssigneeToNullAuthorizationStillAvailable()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set assignee to demo -> an authorization for demo is available
            taskService.SetAssignee(taskId, "demo");

            // when
            taskService.SetAssignee(taskId, null);

            // then
            // authorization for demo is still available
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testQueryStandaloneTaskSetAssignee()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set assignee to demo -> an authorization for demo is available
            taskService.SetAssignee(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId});
            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssigneeOutsideCommandContextInsert()
        {
            // given
            string taskId = "myTask";
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            ITask task = taskService.NewTask(taskId);
            task.Assignee = "demo";

            // when
            taskService.SaveTask(task);

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetAssigneeOutsideCommandContextSave()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            ITask task = selectSingleTask();

            task.Assignee = "demo";

            // when
            taskService.SaveTask(task);

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        // set assignee -> an authorization is available (process task) /////////////////////////////////////////

        public virtual void testProcessTaskSetAssigneeCreateNewAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testProcessTaskSetAssigneeUpdateAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testProcessTaskSetAssigneeToNullAuthorizationStillAvailable()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set assignee to demo -> an authorization for demo is available
            taskService.SetAssignee(taskId, "demo");

            // when
            taskService.SetAssignee(taskId, null);

            // then
            // authorization for demo is still available
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testQueryProcessTaskSetAssignee()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set assignee to demo -> an authorization for demo is available
            taskService.SetAssignee(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId});
        }

        public virtual void testProcessTaskAssignee()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, DEMO_ASSIGNEE_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.StartProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

            // then
            // an authorization for demo has been created
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            // demo is able to retrieve the task
            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
        }

        // set assignee -> should not create an authorization (case task) /////////////////////////////////////////

        public virtual void testCaseTaskSetAssigneeNoAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // set owner -> an authorization is available (standalone task) /////////////////////////////////////////

        public virtual void testStandaloneTaskSetOwnerCreateNewAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetOwnerUpdateAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testQueryStandaloneTaskSetOwner()
        {
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set owner to demo -> an authorization for demo is available
            taskService.SetOwner(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetOwnerOutsideCommandContextInsert()
        {
            // given
            string taskId = "myTask";
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            ITask task = taskService.NewTask(taskId);
            task.Owner = "demo";

            // when
            taskService.SaveTask(task);

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetOwnerOutsideCommandContextSave()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            ITask task = selectSingleTask();

            task.Owner = "demo";

            // when
            taskService.SaveTask(task);

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        // set owner -> an authorization is available (process task) /////////////////////////////////////////

        public virtual void testProcessTaskSetOwnerCreateNewAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testProcessTaskSetOwnerUpdateAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testQueryProcessTaskSetOwner()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // set owner to demo -> an authorization for demo is available
            taskService.SetOwner(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
        }

        // set owner -> should not create an authorization  (case task) /////////////////////////////////

        public virtual void testCaseTaskSetOwnerNoAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // add candidate user -> an authorization is available (standalone task) /////////////////

        public virtual void testStandaloneTaskAddCandidateUserCreateNewAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddCandidateUserUpdateAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testQueryStandaloneTaskAddCandidateUser()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // add candidate user -> an authorization for demo is available
            taskService.AddCandidateUser(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
            deleteTask(taskId, true);
        }

        public virtual void testQueryStandaloneTaskAddCandidateUserWithTaskAssignPermission()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.TaskAssign);

            // add candidate user -> an authorization for demo is available
            taskService.AddCandidateUser(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
            deleteTask(taskId, true);
        }

        // add candidate user -> an authorization is available (process task) ////////////////////

        public virtual void testProcessTaskAddCandidateUserCreateNewAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testProcessTaskAddCandidateUserUpdateAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testQueryProcessTaskAddCandidateUser()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // add candidate user -> an authorization for demo is available
            taskService.AddCandidateUser(taskId, "demo");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
        }

        public virtual void testProcessTaskCandidateUsers()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, CANDIDATE_USERS_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.StartProcessInstanceByKey(CANDIDATE_USERS_PROCESS_KEY);

            // then
            // an authorization for demo has been created
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            // an authorization for test has been created
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "test"&& c.ResourceId==taskId).First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            // demo is able to retrieve the task
            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", null);

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            // test is able to retrieve the task
            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);
        }

        // add candidate user -> should not create an authorization  (case task) /////////////////////////////////

        public virtual void testCaseTaskAddCandidateUserNoAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "demo").First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // add candidate group -> an authorization is available (standalone task) /////////////////

        public virtual void testStandaloneTaskAddCandidateGroupCreateNewAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "management").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskAddCandidateGroupUpdateAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "management").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            deleteTask(taskId, true);
        }

        public virtual void testQueryStandaloneTaskAddCandidateGroup()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // add candidate group -> an authorization for group management is available
            taskService.AddCandidateGroup(taskId, "management");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", new List<string> { "management" });

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
            deleteTask(taskId, true);
        }

        // add candidate group -> an authorization is available (process task) ////////////////////

        public virtual void testProcessTaskAddCandidateGroupCreateNewAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "management").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testProcessTaskAddCandidateGroupUpdateAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            createGrantAuthorization(Resources.Task, taskId, "demo", Permissions.Delete);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "management").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));
        }

        public virtual void testQueryProcessTaskAddCandidateGroup()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // add candidate group -> an authorization for group management is available
            taskService.AddCandidateGroup(taskId, "management");

            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", new List<string> { "management" });

            // when
            ITask task = taskService.CreateTaskQuery().First();

            // then
            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });
        }

        public virtual void testProcessTaskCandidateGroups()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, CANDIDATE_GROUPS_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.StartProcessInstanceByKey(CANDIDATE_GROUPS_PROCESS_KEY);

            // then
            // an authorization for management has been created
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "management").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            // an authorization for accounting has been created
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "accounting").First();
            enableAuthorization();

            Assert.NotNull(authorization);
            Assert.AreEqual(Resources.Task, authorization.ResourceType);
            Assert.AreEqual(taskId, authorization.ResourceId);
            Assert.True(authorization.IsPermissionGranted(Permissions.Read));
            Assert.True(authorization.IsPermissionGranted(DefaultTaskPermissionForUser));

            // management is able to retrieve the task
            identityService.ClearAuthentication();
            identityService.SetAuthentication("demo", new List<string> { "management" });

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);

            // accounting is able to retrieve the task
            identityService.ClearAuthentication();
            identityService.SetAuthentication(userId, new List<string> { groupId });

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual(taskId, task.Id);
        }

        // add candidate group -> should not create an authorization (case task) /////////////////////////////////

        public virtual void testCaseTaskAddCandidateGroupNoAuthorization()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId== "management").First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // TaskService#GetVariable() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariableWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariable(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariableWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariableWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);

            deleteTask(taskId, true);
        }

        // TaskService#GetVariable() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariableWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariable(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariableWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // TaskService#GetVariable() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariable()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // TaskService#getVariableLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariableLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariableLocal(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariableLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);

            deleteTask(taskId, true);
        }

        // TaskService#getVariableLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariableLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariableLocal(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariableLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableLocalWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testProcessTaskGetVariableLocalWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariableLocal(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // TaskService#GetVariable() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariableLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            object variable = taskService.GetVariable(taskId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // TaskService#getVariableTyped() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariableTypedWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariableTypedWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariableTypedWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);

            deleteTask(taskId, true);
        }

        // TaskService#getVariableTyped() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariableTypedWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariableTypedWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableTypedWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableTypedWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableTypedWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // TaskService#getVariableTyped() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariableTyped()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            ITypedValue typedValue = taskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // TaskService#getVariableLocalTyped() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariableLocalTypedWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariableLocalTypedWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariableLocalTypedWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);

            deleteTask(taskId, true);
        }

        // TaskService#getVariableLocalTyped() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariableLocalTypedWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariableLocalTypedWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableLocalTypedWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableLocalTypedWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testProcessTaskGetVariableLocalTypedWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // TaskService#getVariableLocalTyped() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariableLocalTyped()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            ITypedValue typedValue = taskService.GetVariableLocalTyped<ITypedValue>(taskId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // TaskService#getVariables() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariables(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariables(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariables() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesLocal(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariablesLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesLocal(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesTyped() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesTypedWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesTyped(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesTypedWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesTypedWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariablesTyped() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesTypedWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesTyped(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesTypedWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesTyped() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesTyped()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocalTyped() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesLocalTypedWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesLocalTyped(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesLocalTypedWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesLocalTypedWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariablesLocalTyped() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesLocalTypedWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesLocalTyped(taskId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesLocalTypedWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalTypedWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalTypedWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocalTyped() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesLocalTyped()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariables() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesByNameWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesByNameWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesByNameWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesByNameWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesByNameWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesByNameWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesByNameWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesByNameWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariables() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesByName()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            IDictionary<string, object> variables = taskService.GetVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesLocalByNameWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesLocalByNameWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesLocalByNameWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariablesLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesLocalByNameWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesLocalByNameWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalByNameWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalByNameWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalByNameWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesLocalByName()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesTyped() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesTypedByNameWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesTypedByNameWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesTypedByNameWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariables(taskId, Variables);
            enableAuthorization();

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesTypedByNameWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesTypedByNameWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedByNameWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedByNameWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesTypedByNameWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariables() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesTypedByName()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            IVariableMap variables = taskService.GetVariablesTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(variables.Any());
            Assert.AreEqual(1, variables.Count());

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskGetVariablesLocalTypedByNameWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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

        public virtual void testStandaloneTaskGetVariablesLocalTypedByNameWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskGetVariablesLocalTypedByNameWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);

            deleteTask(taskId, true);
        }

        // TaskService#getVariablesLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskGetVariablesLocalTypedByNameWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskGetVariablesLocalTypedByNameWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalTypedByNameWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Read);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalTypedByNameWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testProcessTaskGetVariablesLocalTypedByNameWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadTask);

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskGetVariablesLocalTypedByName()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariablesLocal(taskId, Variables);
            enableAuthorization();

            // when
            IDictionary<string, object> variables = taskService.GetVariablesLocalTyped(taskId, new List<string> { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // TaskService#SetVariable() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskSetVariableWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskSetVariableWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetVariableWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#SetVariable() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskSetVariableWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetVariableWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#SetVariable() (case task) /////////////////////////////////////

        public virtual void testCaseTaskSetVariable()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariableLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskSetVariableLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskSetVariableLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#setVariableLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskSetVariableLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetVariableLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableLocalWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariableLocalWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariableLocal() (case task) /////////////////////////////////////

        public virtual void testCaseTaskSetVariableLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariables() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskSetVariablesWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetVariables(taskId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskSetVariablesWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetVariablesWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#setVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskSetVariablesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetVariables(taskId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetVariablesWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariables() (case task) /////////////////////////////////////

        public virtual void testCaseTaskSetVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetVariables(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskSetVariablesLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.SetVariablesLocal(taskId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskSetVariablesLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskSetVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#setVariableLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskSetVariablesLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.SetVariablesLocal(taskId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskSetVariablesLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesLocalWithReadTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testProcessTaskSetVariablesLocalWithReadTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#setVariablesLocal() (case task) /////////////////////////////////////

        public virtual void testCaseTaskSetVariablesLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            // when
            taskService.SetVariablesLocal(taskId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // TaskService#removeVariable() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskRemoveVariableWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.RemoveVariable(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskRemoveVariableWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskRemoveVariableWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#removeVariable() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskRemoveVariableWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.RemoveVariable(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskRemoveVariableWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariable() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskRemoveVariable()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            taskService.RemoveVariable(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariableLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskRemoveVariableLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskRemoveVariableLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskRemoveVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#removeVariableLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskRemoveVariableLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskRemoveVariableLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariableLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariableLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskRemoveVariableLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariableLocal(taskId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariables() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskRemoveVariablesWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskRemoveVariablesWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskRemoveVariablesWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#removeVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskRemoveVariablesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskRemoveVariablesWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY, Variables);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariables() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskRemoveVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY, Variables);
            string taskId = selectSingleTask().Id;

            // when
            taskService.RemoveVariables(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskRemoveVariablesLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when
                taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
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

        public virtual void testStandaloneTaskRemoveVariablesLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskRemoveVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskService#removeVariablesLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskRemoveVariablesLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
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
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskRemoveVariablesLocalWithReadPermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskRemoveVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskService#removeVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskRemoveVariablesLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;

            disableAuthorization();
            taskService.SetVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
            enableAuthorization();

            // when
            taskService.RemoveVariablesLocal(taskId, new List<string> { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskServiceImpl#updateVariablesLocal() (standalone task) ////////////////////////////////////////////

        public virtual void testStandaloneTaskUpdateVariablesLocalWithoutAuthorization()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            try
            {
                // when (1)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (1)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (2)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (2)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (3)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (3)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
            }

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskUpdateVariablesLocalWithReadPermissionOnTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        public virtual void testStandaloneTaskUpdateVariablesLocalWithReadPermissionOnAnyTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // TaskServiceImpl#updateVariablesLocal() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskUpdateVariablesLocalWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when (1)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (1)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (2)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (2)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (3)
                ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (3)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskUpdateVariablesLocalWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesLocalWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesLocalWithUpdateTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesLocalWithUpdateTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskServiceImpl#updateVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskUpdateVariablesLocal()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariablesLocal(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskServiceImpl#updateVariables() (process task) ////////////////////////////////////////////

        public virtual void testProcessTaskUpdateVariablesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when (1)
                ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (1)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (2)
                ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (2)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (3)
                ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (3)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(taskId, message);
                AssertTextPresent(Resources.Task.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateTask.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testProcessTaskUpdateVariablesWithUpdatePermissionOnTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesWithUpdatePermissionOnAnyTask()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesWithUpdateTaskPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateTask);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testProcessTaskUpdateVariablesWithUpdateTaskPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateTask);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // TaskServiceImpl#updateVariablesLocal() (case task) ////////////////////////////////////////////

        public virtual void testCaseTaskUpdateVariables()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = selectSingleTask().Id;
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, null, new List<string> { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((TaskServiceImpl)taskService).UpdateVariables<object>(taskId, Variables, new List<string> { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testStandaloneTaskSaveWithGenericResourceIdOwner()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);

            ITask task = taskService.NewTask();
            task.Owner = "*";

            try
            {
                taskService.SaveTask(task);
                Assert.Fail("it should not be possible to save a task with the generic resource id *");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot create default authorization for owner *: " + "id cannot be *. * is a reserved identifier", e.Message);
            }
        }

        public virtual void testStandaloneTaskSaveWithGenericResourceIdOwnerTaskServiceApi()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Update);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.SetOwner(task.Id, "*");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot create default authorization for owner *: " + "id cannot be *. * is a reserved identifier", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveWithGenericResourceIdAssignee()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create);

            ITask task = taskService.NewTask();
            task.Assignee = "*";

            try
            {
                taskService.SaveTask(task);
                Assert.Fail("it should not be possible to save a task with the generic resource id *");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier", e.Message);
            }
        }

        public virtual void testStandaloneTaskSaveWithGenericResourceIdAssigneeTaskServiceApi()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Update);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.SetAssignee(task.Id, "*");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveIdentityLinkWithGenericUserId()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Update);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.AddUserIdentityLink(task.Id, "*", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot grant default authorization for identity link to user *: " + "id cannot be *. * is a reserved identifier.", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveIdentityLinkWithGenericGroupId()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Update);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.AddGroupIdentityLink(task.Id, "*", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot grant default authorization for identity link to group *: " + "id cannot be *. * is a reserved identifier.", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveIdentityLinkWithGenericGroupIdAndTaskAssignPermission()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.TaskAssign);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.AddGroupIdentityLink(task.Id, "*", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot grant default authorization for identity link to group *: " + "id cannot be *. * is a reserved identifier.", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveIdentityLinkWithGenericTaskId()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Update);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.AddUserIdentityLink("*", "aUserId", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot find task with id *", e.Message);
            }

            try
            {
                taskService.AddGroupIdentityLink("*", "aGroupId", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot find task with id *", e.Message);
            }

            deleteTask(task.Id, true);
        }

        public virtual void testStandaloneTaskSaveIdentityLinkWithGenericTaskIdAndTaskAssignPermission()
        {
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.TaskAssign);

            ITask task = taskService.NewTask();
            taskService.SaveTask(task);

            try
            {
                taskService.AddUserIdentityLink("*", "aUserId", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot find task with id *", e.Message);
            }

            try
            {
                taskService.AddGroupIdentityLink("*", "aGroupId", "someLink");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot find task with id *", e.Message);
            }

            deleteTask(task.Id, true);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testSetGenericResourceIdAssignee()
        public virtual void testSetGenericResourceIdAssignee()
        {
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                runtimeService.StartProcessInstanceByKey("genericResourceIdAssignmentProcess");
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier.", e.Message);
            }
        }

        public virtual void testAssignSameAssigneeAndOwnerToTask()
        {

            // given
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.All);

            // when
            ITask NewTask = taskService.NewTask();
            NewTask.Assignee = "Horst";
            NewTask.Owner = "Horst";

            // then
            System.Exception ex = null;
            try
            {
                taskService.SaveTask(NewTask);
            }
            catch (System.Exception)
            {
                Assert.Fail("Setting same assignee and owner to user should not Assert.Fail!");
            }

            taskService.DeleteTask(NewTask.Id, true);
        }

        public virtual void testPermissionsOnAssignSameAssigneeAndOwnerToTask()
        {

            try
            {
                // given
                createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.Create, Permissions.Delete, Permissions.Read);
                // Todo: DefaultAuthorizationProvider    
                //processEngineConfiguration.ResourceAuthorizationProvider = new MyExtendedPermissionDefaultAuthorizationProvider();

                // when
                ITask NewTask = taskService.NewTask();
                NewTask.Assignee = "Horst";
                NewTask.Owner = "Horst";
                taskService.SaveTask(NewTask);

                // then
                IAuthorization auth = authorizationService.CreateAuthorizationQuery(c=>c.UserId == "Horst").First();
                Assert.True(auth.IsPermissionGranted(Permissions.DeleteHistory));

                taskService.DeleteTask(NewTask.Id, true);

            }
            finally
            {
                // Todo: DefaultAuthorizationProvider            
                //processEngineConfiguration.ResourceAuthorizationProvider = new DefaultAuthorizationProvider();
            }


        }

        [Deployment]
        public virtual void testAssignSameAssigneeAndOwnerToProcess()
        {
            //given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.All);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.All);

            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            IList<IAuthorization> auths = authorizationService.CreateAuthorizationQuery()
                //.UserIdIn("horst")
                .ToList();
            Assert.True(auths.Count == 1);

        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testAssignSameUserToProcessTwice()
        public virtual void testAssignSameUserToProcessTwice()
        {
            //given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.All);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.All);

            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            IList<IAuthorization> auths = authorizationService.CreateAuthorizationQuery()
                //.UserIdIn("hans")
                .ToList();
            Assert.True(auths.Count == 1);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Deployment public void testAssignSameGroupToProcessTwice()
        public virtual void testAssignSameGroupToProcessTwice()
        {
            //given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.All);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.All);

            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            IList<IAuthorization> auths = authorizationService.CreateAuthorizationQuery()
                //.GroupIdIn("abc")
                .ToList();
            Assert.True(auths.Count == 1);
        }


        // helper ////////////////////////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<ITask> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual void verifyQueryResults(IQueryable<IVariableInstance> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}