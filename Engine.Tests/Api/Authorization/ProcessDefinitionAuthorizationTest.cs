using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ProcessDefinitionAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
        protected internal const string TWO_TASKS_PROCESS_KEY = "twoTasksProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/twoTasksProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            // given user is not authorized to read any process definition

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
        {
            // given
            // given user gets read permission on any process definition
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            // given user gets read permission on any process definition
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadPermissionOnOneTaskProcess()
        {
            // given
            // given user gets read permission on "oneTaskProcess" process definition
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessDefinition definition = query.First();
            Assert.NotNull(definition);
            Assert.AreEqual(ONE_TASK_PROCESS_KEY, definition.Key);
        }

        public virtual void testQueryWithRevokedReadPermission()
        {
            // given
            // given user gets all permissions on any process definition
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.All);

            IAuthorization authorization = createRevokeAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY);
            authorization.UserId = userId;
            authorization.RemovePermission(Permissions.Read);
            saveAuthorization(authorization);

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessDefinition definition = query.First();
            Assert.NotNull(definition);
            Assert.AreEqual(TWO_TASKS_PROCESS_KEY, definition.Key);
        }

        public virtual void testQueryWithGroupAuthorizationRevokedReadPermission()
        {
            // given
            // given user gets all permissions on any process definition
            IAuthorization authorization = createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any);
            authorization.GroupId = groupId;
            authorization.AddPermission(Permissions.All);
            saveAuthorization(authorization);

            authorization = createRevokeAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY);
            authorization.GroupId = groupId;
            authorization.RemovePermission(Permissions.Read);
            saveAuthorization(authorization);

            // when
            IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessDefinition definition = query.First();
            Assert.NotNull(definition);
            Assert.AreEqual(TWO_TASKS_PROCESS_KEY, definition.Key);
        }

        // get process definition /////////////////////////////////////////////////////

        public virtual void testGetProcessDefinitionWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.GetProcessDefinition(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IProcessDefinition definition = repositoryService.GetProcessDefinition(processDefinitionId);

            // then
            Assert.NotNull(definition);
        }

        // get deployed process definition /////////////////////////////////////////////////////

        public virtual void testGetDeployedProcessDefinitionWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                ((RepositoryServiceImpl)repositoryService).GetDeployedProcessDefinition(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetDeployedProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IReadOnlyProcessDefinition definition = ((RepositoryServiceImpl)repositoryService).GetDeployedProcessDefinition(processDefinitionId);

            // then
            Assert.NotNull(definition);
        }

        // get process diagram /////////////////////////////////////////////////////

        public virtual void testGetProcessDiagramWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.GetProcessDiagram(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the process diagram");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetProcessDiagram()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            System.IO.Stream stream = repositoryService.GetProcessDiagram(processDefinitionId);

            // then
            // no process diagram deployed
            Assert.IsNull(stream);
        }

        // get process model /////////////////////////////////////////////////////

        public virtual void testGetProcessModelWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.GetProcessModel(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the process model");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetProcessModel()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            System.IO.Stream stream = repositoryService.GetProcessModel(processDefinitionId);

            // then
            Assert.NotNull(stream);
        }

        // get bpmn model instance /////////////////////////////////////////////////////

        public virtual void testGetBpmnModelInstanceWithoutAuthorizations()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.GetBpmnModelInstance(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the bpmn model instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetBpmnModelInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            IBpmnModelInstance modelInstance = repositoryService.GetBpmnModelInstance(processDefinitionId);

            // then
            Assert.NotNull(modelInstance);
        }

        // get process diagram layout /////////////////////////////////////////////////

        public virtual void testGetProcessDiagramLayoutWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.GetProcessDiagramLayout(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to get the process diagram layout");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetProcessDiagramLayout()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Read);

            // when
            DiagramLayout diagramLayout = repositoryService.GetProcessDiagramLayout(processDefinitionId);

            // then
            // no process diagram deployed
            Assert.IsNull(diagramLayout);
        }

        // suspend process definition by id ///////////////////////////////////////////

        public virtual void testSuspendProcessDefinitionByIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionById(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionById()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.SuspendProcessDefinitionById(processDefinitionId);

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);
        }

        // activate process definition by id ///////////////////////////////////////////

        public virtual void testActivateProcessDefinitionByIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionById(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionById()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.ActivateProcessDefinitionById(processDefinitionId);

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);
        }

        // suspend process definition by id including instances ///////////////////////////////////////////

        public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            repositoryService.SuspendProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessDefinitionByIdIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            // when
            repositoryService.SuspendProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        // activate process definition by id including instances ///////////////////////////////////////////

        public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.ActivateProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessDefinitionByIdIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            string processDefinitionId = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessDefinitionById(processDefinitionId);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            repositoryService.ActivateProcessDefinitionById(processDefinitionId, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        // suspend process definition by key ///////////////////////////////////////////

        public virtual void testSuspendProcessDefinitionByKeyWithoutAuthorization()
        {
            // given

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionByKey()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);
        }

        // activate process definition by id ///////////////////////////////////////////

        public virtual void testActivateProcessDefinitionByKeyWithoutAuthorization()
        {
            // given
            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionByKey()
        {
            // given
            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);
        }

        // suspend process definition by key including instances ///////////////////////////////////////////

        public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithoutAuthorization()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to suspend the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessDefinitionByKeyIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            // when
            repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.True(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        // activate process definition by key including instances ///////////////////////////////////////////

        public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));
                Assert.Fail("Exception expected: It should not be possible to activate the process definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);

            // when
            repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessDefinitionByKeyIncludingInstancesWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            repositoryService.ActivateProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Parse(null));

            // then
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.IsFalse(definition.Suspended);

            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        
        public virtual void testProcessDefinitionUpdateTimeToLive()
        {

            // given
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.Update);
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

            // when
            // Todo: IRepositoryService.UpdateProcessDefinitionHistoryTimeToLive(..)
            //repositoryService.updateProcessDefinitionHistoryTimeToLive(definition.Id, 6);

            // then
            definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            Assert.AreEqual(6, definition.HistoryTimeToLive);

        }

        public virtual void testDecisionDefinitionUpdateTimeToLiveWithoutAuthorizations()
        {
            //given
            IProcessDefinition definition = selectProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            try
            {
                //when
                // Todo: IRepositoryService.UpdateProcessDefinitionHistoryTimeToLive(..)
                //repositoryService.UpdateProcessDefinitionHistoryTimeToLive(definition.Id, 6);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

        }

        // helper /////////////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IProcessDefinition> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}