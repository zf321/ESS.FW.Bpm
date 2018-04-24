using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
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
    public class ProcessInstanceAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
        protected internal const string MESSAGE_BOUNDARY_PROCESS_KEY = "messageBoundaryProcess";
        protected internal const string SIGNAL_BOUNDARY_PROCESS_KEY = "signalBoundaryProcess";
        protected internal const string SIGNAL_START_PROCESS_KEY = "signalStartProcess";
        protected internal const string THROW_WARNING_SIGNAL_PROCESS_KEY = "throwWarningSignalProcess";
        protected internal const string THROW_ALERT_SIGNAL_PROCESS_KEY = "throwAlertSignalProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/messageStartEventProcess.bpmn20.xml", "resources/api/authorization/messageBoundaryEventProcess.bpmn20.xml", "resources/api/authorization/signalBoundaryEventProcess.bpmn20.xml", "resources/api/authorization/signalStartEventProcess.bpmn20.xml", "resources/api/authorization/throwWarningSignalEventProcess.bpmn20.xml", "resources/api/authorization/throwAlertSignalEventProcess.bpmn20.xml").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // process instance query //////////////////////////////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        // process instance query (multiple process instances) ////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        // start process instance by key //////////////////////////////////////////////

        public virtual void testStartProcessInstanceByKeyWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            try
            {
                // when
                runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByKeyWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByKeyWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByKey()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // start process instance by id //////////////////////////////////////////////

        public virtual void testStartProcessInstanceByIdWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceById(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByIdWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceById(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByIdWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceById(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceById()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            // when
            runtimeService.StartProcessInstanceById(processDefinitionId);

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testStartProcessInstanceAtActivitiesByKey()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.CreateProcessInstanceByKey(PROCESS_KEY).StartBeforeActivity("theTask").Execute();

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testStartProcessInstanceAtActivitiesByKeyWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            try
            {
                // when
                runtimeService.CreateProcessInstanceByKey(PROCESS_KEY).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceAtActivitiesByKeyWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.CreateProcessInstanceByKey(PROCESS_KEY).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceAtActivitiesByKeyWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.CreateProcessInstanceByKey(PROCESS_KEY).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceAtActivitiesById()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            // when
            runtimeService.CreateProcessInstanceById(processDefinitionId).StartBeforeActivity("theTask").Execute();

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testStartProcessInstanceAtActivitiesByIdWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CreateProcessInstanceById(processDefinitionId).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceAtActivitiesByIdWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CreateProcessInstanceById(processDefinitionId).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'oneTaskProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceAtActivitiesByIdWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.CreateInstance);

            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CreateProcessInstanceById(processDefinitionId).StartBeforeActivity("theTask").Execute();
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        // start process instance by message //////////////////////////////////////////////

        public virtual void testStartProcessInstanceByMessageWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessageWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessageWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessage()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            // when
            runtimeService.StartProcessInstanceByMessage("startInvoiceMessage");

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // start process instance by message and process definition id /////////////////////////////

        public virtual void testStartProcessInstanceByMessageAndProcDefIdWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessageAndProcDefIdWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessageAndProcDefIdWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to start a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByMessageAndProcDefId()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            string processDefinitionId = selectProcessDefinitionByKey(MESSAGE_START_PROCESS_KEY).Id;

            // when
            runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("startInvoiceMessage", processDefinitionId);

            // then
            disableAuthorization();
             IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // Delete process instance /////////////////////////////

        public virtual void testDeleteProcessInstanceWithoutAuthorization()
        {
            // given
            // no authorization to Delete a process instance

            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
                Assert.Fail("Exception expected: It should not be possible to Delete a process instance");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Delete.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.DeleteInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testDeleteProcessInstanceWithDeletePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Delete);

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            disableAuthorization();
            AssertProcessEnded(ProcessInstanceId);
            enableAuthorization();
        }

        public virtual void testDeleteProcessInstanceWithDeletePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Delete);

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            disableAuthorization();
            AssertProcessEnded(ProcessInstanceId);
            enableAuthorization();
        }

        public virtual void testDeleteProcessInstanceWithDeleteInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.DeleteInstance);

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            disableAuthorization();
            AssertProcessEnded(ProcessInstanceId);
            enableAuthorization();
        }

        public virtual void testDeleteProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Delete);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.DeleteInstance);

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            disableAuthorization();
            AssertProcessEnded(ProcessInstanceId);
            enableAuthorization();
        }

        // get active activity ids ///////////////////////////////////

        public virtual void testGetActiveActivityIdsWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.GetActiveActivityIds(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to retrieve active ativity ids");
            }
            catch (AuthorizationException)
            {

                // then
                //      String message = e.GetMessage();
                //      AssertTextPresent(userId, message);
                //      AssertTextPresent(Permissions.Read.GetName(), message);
                //      AssertTextPresent(ProcessInstanceId, message);
                //      AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetActiveActivityIdsWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IList<string> activityIds = runtimeService.GetActiveActivityIds(ProcessInstanceId);

            // then
            Assert.NotNull(activityIds);
            Assert.IsFalse(activityIds.Count == 0);
        }

        public virtual void testGetActiveActivityIdsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IList<string> activityIds = runtimeService.GetActiveActivityIds(ProcessInstanceId);

            // then
            Assert.NotNull(activityIds);
            Assert.IsFalse(activityIds.Count == 0);
        }

        public virtual void testGetActiveActivityIdsWithReadInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IList<string> activityIds = runtimeService.GetActiveActivityIds(ProcessInstanceId);

            // then
            Assert.NotNull(activityIds);
            Assert.IsFalse(activityIds.Count == 0);
        }

        public virtual void testGetActiveActivityIds()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IList<string> activityIds = runtimeService.GetActiveActivityIds(ProcessInstanceId);

            // then
            Assert.NotNull(activityIds);
            Assert.IsFalse(activityIds.Count == 0);
        }

        // get activity instance ///////////////////////////////////////////

        public virtual void testGetActivityInstanceWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to retrieve ativity instances");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetActivityInstanceWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            Assert.NotNull(activityInstance);
        }

        public virtual void testGetActivityInstanceWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            Assert.NotNull(activityInstance);
        }

        public virtual void testGetActivityInstanceWithReadInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            Assert.NotNull(activityInstance);
        }

        public virtual void testGetActivityInstanceIds()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            Assert.NotNull(activityInstance);
        }

        // signal execution ///////////////////////////////////////////

        public virtual void testSignalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.Signal(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to signal an execution");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSignalWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.Signal(ProcessInstanceId);

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        public virtual void testSignalWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.Signal(ProcessInstanceId);

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        public virtual void testSignalWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.Signal(ProcessInstanceId);

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        public virtual void testSignal()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            Assert.NotNull(activityInstance);
        }

        // signal event received //////////////////////////////////////

        public virtual void testSignalEventReceivedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SignalEventReceived("alert");
                Assert.Fail("Exception expected: It should not be possible to trigger a signal event");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSignalEventReceivedWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SignalEventReceived("alert");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SignalEventReceived("alert");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SignalEventReceived("alert");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceived()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SignalEventReceived("alert");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedTwoExecutionsShouldFail()
        {
            // given
            string firstProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            string secondProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, firstProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.SignalEventReceived("alert");
                Assert.Fail("Exception expected: It should not be possible to trigger a signal event");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(secondProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSignalEventReceivedTwoExecutionsShouldSuccess()
        {
            // given
            string firstProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            string secondProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, firstProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, secondProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SignalEventReceived("alert");

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.IsFalse(tasks.Count == 0);
            foreach (ITask task in tasks)
            {
                Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
            }
            enableAuthorization();
        }

        // signal event received by execution id //////////////////////////////////////

        public virtual void testSignalEventReceivedByExecutionIdWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            string executionId = selectSingleTask().ExecutionId;

            try
            {
                // when
                runtimeService.SignalEventReceived("alert", executionId);
                Assert.Fail("Exception expected: It should not be possible to trigger a signal event");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSignalEventReceivedByExecutionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.SignalEventReceived("alert", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedByExecutionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.SignalEventReceived("alert", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedByExecutionIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.SignalEventReceived("alert", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testSignalEventReceivedByExecutionId()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.SignalEventReceived("alert", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testStartProcessInstanceBySignalEventReceivedWithoutAuthorization()
        {
            // given
            // no authorization to start a process instance

            try
            {
                // when
                runtimeService.SignalEventReceived("warning");
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceBySignalEventReceivedWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.SignalEventReceived("warning");
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'signalStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceBySignalEventReceived()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            // when
            runtimeService.SignalEventReceived("warning");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("task", task.TaskDefinitionKey);
        }

        /// <summary>
        /// currently the ThrowSignalEventActivityBehavior does not check authorization
        /// </summary>
        public virtual void FAILING_testStartProcessInstanceByThrowSignalEventWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, THROW_WARNING_SIGNAL_PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.StartProcessInstanceByKey(THROW_WARNING_SIGNAL_PROCESS_KEY);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'signalStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testStartProcessInstanceByThrowSignalEvent()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_START_PROCESS_KEY, userId, Permissions.CreateInstance);
            createGrantAuthorization(Resources.ProcessDefinition, THROW_WARNING_SIGNAL_PROCESS_KEY, userId, Permissions.CreateInstance);

            // when
            runtimeService.StartProcessInstanceByKey(THROW_WARNING_SIGNAL_PROCESS_KEY);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("task", task.TaskDefinitionKey);
        }

        /// <summary>
        /// currently the ThrowSignalEventActivityBehavior does not check authorization
        /// </summary>
        public virtual void FAILING_testThrowSignalEventWithoutAuthorization()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, THROW_ALERT_SIGNAL_PROCESS_KEY, userId, Permissions.CreateInstance);

            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.StartProcessInstanceByKey(THROW_ALERT_SIGNAL_PROCESS_KEY);

                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(SIGNAL_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testThrowSignalEvent()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, THROW_ALERT_SIGNAL_PROCESS_KEY, userId, Permissions.CreateInstance);

            string ProcessInstanceId = StartProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, SIGNAL_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.StartProcessInstanceByKey(THROW_ALERT_SIGNAL_PROCESS_KEY);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        // message event received /////////////////////////////////////

        public virtual void testMessageEventReceivedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            string executionId = selectSingleTask().ExecutionId;

            try
            {
                // when
                runtimeService.MessageEventReceived("boundaryInvoiceMessage", executionId);
                Assert.Fail("Exception expected: It should not be possible to trigger a message event");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testMessageEventReceivedByExecutionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.MessageEventReceived("boundaryInvoiceMessage", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testMessageEventReceivedByExecutionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.MessageEventReceived("boundaryInvoiceMessage", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testMessageEventReceivedByExecutionIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.MessageEventReceived("boundaryInvoiceMessage", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testMessageEventReceivedByExecutionId()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            string executionId = selectSingleTask().ExecutionId;

            // when
            runtimeService.MessageEventReceived("boundaryInvoiceMessage", executionId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        // correlate message (correlates to an execution) /////////////

        public virtual void testCorrelateMessageExecutionWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CorrelateMessage("boundaryInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testCorrelateMessageExecutionWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.CorrelateMessage("boundaryInvoiceMessage");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateMessageExecutionWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.CorrelateMessage("boundaryInvoiceMessage");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateMessageExecutionWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CorrelateMessage("boundaryInvoiceMessage");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateMessageExecution()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CorrelateMessage("boundaryInvoiceMessage");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        // correlate message (correlates to a process definition) /////////////

        public virtual void testCorrelateMessageProcessDefinitionWithoutAuthorization()
        {
            // given

            try
            {
                // when
                runtimeService.CorrelateMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testCorrelateMessageProcessDefinitionWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.CorrelateMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testCorrelateMessageProcessDefinitionWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.CorrelateMessage("startInvoiceMessage");
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testCorrelateMessageProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            // when
            runtimeService.CorrelateMessage("startInvoiceMessage");

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("task", task.TaskDefinitionKey);
        }

        // correlate all (correlates to executions) ///////////////////

        public virtual void testCorrelateAllExecutionWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testCorrelateAllExecutionWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateAllExecutionWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateAllExecutionWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateAllExecution()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testCorrelateAllTwoExecutionsShouldFail()
        {
            // given
            string firstProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            string secondProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, firstProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();
                Assert.Fail("Exception expected: It should not be possible to trigger a signal event");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(secondProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testCorrelateAllTwoExecutionsShouldSuccess()
        {
            // given
            string firstProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            string secondProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, firstProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, secondProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.CreateMessageCorrelation("boundaryInvoiceMessage").CorrelateAll();

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.IsFalse(tasks.Count == 0);
            foreach (ITask task in tasks)
            {
                Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
            }
            enableAuthorization();
        }

        // correlate all (correlates to a process definition) /////////////

        public virtual void testCorrelateAllProcessDefinitionWithoutAuthorization()
        {
            // given

            try
            {
                // when
                runtimeService.CreateMessageCorrelation("startInvoiceMessage").CorrelateAll();
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {
                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testCorrelateAllProcessDefinitionWithCreatePermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);

            try
            {
                // when
                runtimeService.CreateMessageCorrelation("startInvoiceMessage").CorrelateAll();
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.CreateInstance' permission on resource 'messageStartProcess' of type 'ProcessDefinition'", e.Message);
            }
        }

        public virtual void testCorrelateAllProcessDefinitionWithCreateInstancesPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            try
            {
                // when
                runtimeService.CreateMessageCorrelation("startInvoiceMessage").CorrelateAll();
                Assert.Fail("Exception expected: It should not be possible to correlate a message.");
            }
            catch (AuthorizationException e)
            {

                // then
                AssertTextPresent("The user with id 'test' does not have 'Permissions.Create' permission on resource 'IProcessInstance'", e.Message);
            }
        }

        public virtual void testCorrelateAllProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Create);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_START_PROCESS_KEY, userId, Permissions.CreateInstance);

            // when
            runtimeService.CreateMessageCorrelation("startInvoiceMessage").CorrelateAll();

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("task", task.TaskDefinitionKey);
        }

        // suspend process instance by id /////////////////////////////

        public virtual void testSuspendProcessInstanceByIdWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SuspendProcessInstanceById(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessInstanceByIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SuspendProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SuspendProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceById()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        // activate process instance by id /////////////////////////////

        public virtual void testActivateProcessInstanceByIdWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);

            try
            {
                // when
                runtimeService.ActivateProcessInstanceById(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be posssible to activate a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessInstanceByIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.ActivateProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.ActivateProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceById()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceById(ProcessInstanceId);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        // suspend process instance by process definition id /////////////////////////////

        public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = StartProcessInstanceByKey(PROCESS_KEY).ProcessDefinitionId;

            try
            {
                // when
                runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string processDefinitionId = instance.ProcessDefinitionId;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string processDefinitionId = instance.ProcessDefinitionId;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionId()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        // activate process instance by process definition id /////////////////////////////

        public virtual void testActivateProcessInstanceByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            suspendProcessInstanceById(ProcessInstanceId);

            try
            {
                // when
                runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionIdWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionId()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            string processDefinitionId = instance.ProcessDefinitionId;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinitionId);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        // suspend process instance by process definition key /////////////////////////////

        public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            try
            {
                // when
                runtimeService.SuspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.SuspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            IProcessInstance instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        public virtual void testSuspendProcessInstanceByProcessDefinitionKey()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            instance = selectSingleProcessInstance();
            Assert.True(instance.IsSuspended);
        }

        // activate process instance by process definition key /////////////////////////////

        public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithoutAuthorization()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            suspendProcessInstanceById(ProcessInstanceId);

            try
            {
                // when
                runtimeService.ActivateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                runtimeService.ActivateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be posssible to suspend a process instance.");
            }
            catch (AuthorizationException e)
            {

                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithUpdateInstancesPermissionOnProcessDefinition()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        public virtual void testActivateProcessInstanceByProcessDefinitionKey()
        {
            // given
            IProcessInstance instance = StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = instance.Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(PROCESS_KEY);

            // then
            instance = selectSingleProcessInstance();
            Assert.IsFalse(instance.IsSuspended);
        }

        // modify process instance /////////////////////////////////////

        public virtual void testModifyProcessInstanceWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.CreateProcessInstanceModification(ProcessInstanceId).StartBeforeActivity("taskAfterBoundaryEvent").Execute();
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testModifyProcessInstanceWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).StartBeforeActivity("taskAfterBoundaryEvent").Execute();

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            enableAuthorization();

            Assert.IsFalse(tasks.Count == 0);
            Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testModifyProcessInstanceWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).StartBeforeActivity("taskAfterBoundaryEvent").Execute();

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            enableAuthorization();

            Assert.IsFalse(tasks.Count == 0);
            Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testModifyProcessInstanceWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).StartBeforeActivity("taskAfterBoundaryEvent").Execute();

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            enableAuthorization();

            Assert.IsFalse(tasks.Count == 0);
            Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testModifyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).StartBeforeActivity("taskAfterBoundaryEvent").Execute();

            // then
            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            enableAuthorization();

            Assert.IsFalse(tasks.Count == 0);
            Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testDeleteProcessInstanceByModifyingWithoutDeleteAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            try
            {
                // when
                runtimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Delete.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.DeleteInstance.ToString(), message);
                AssertTextPresent(MESSAGE_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testDeleteProcessInstanceByModifyingWithoutDeletePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Delete);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        public virtual void testDeleteProcessInstanceByModifyingWithoutDeletePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Delete);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        public virtual void testDeleteProcessInstanceByModifyingWithoutDeleteInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(MESSAGE_BOUNDARY_PROCESS_KEY).Id;
            IAuthorization authorization = createGrantAuthorization(Resources.ProcessDefinition, MESSAGE_BOUNDARY_PROCESS_KEY);
            authorization.UserId = userId;
            authorization.AddPermission(Permissions.UpdateInstance);
            authorization.AddPermission(Permissions.DeleteInstance);
            saveAuthorization(authorization);

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();

            // then
            AssertProcessEnded(ProcessInstanceId);
        }

        // clear process instance authorization ////////////////////////

        public virtual void testClearProcessInstanceAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.All);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.All);

            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == ProcessInstanceId).First();
            enableAuthorization();
            Assert.NotNull(authorization);

            string taskId = selectSingleTask().Id;

            // when
            taskService.Complete(taskId);

            // then
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == ProcessInstanceId).First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        public virtual void testDeleteProcessInstanceClearAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.All);

            disableAuthorization();
            IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == ProcessInstanceId).First();
            enableAuthorization();
            Assert.NotNull(authorization);

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            disableAuthorization();
            authorization = authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == ProcessInstanceId).First();
            enableAuthorization();

            Assert.IsNull(authorization);
        }

        // RuntimeService#GetVariable() ////////////////////////////////////////////

        public virtual void testGetVariableWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariable(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariableWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            object variable = runtimeService.GetVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            object variable = runtimeService.GetVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            object variable = runtimeService.GetVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            object variable = runtimeService.GetVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // RuntimeService#getVariableLocal() ////////////////////////////////////////////

        public virtual void testGetVariableLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariableLocal(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariableLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            object variable = runtimeService.GetVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            object variable = runtimeService.GetVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            object variable = runtimeService.GetVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        public virtual void testGetVariableLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            object variable = runtimeService.GetVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.AreEqual(VARIABLE_VALUE, variable);
        }

        // RuntimeService#getVariableTyped() ////////////////////////////////////////////

        public virtual void testGetVariableTypedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when                
                runtimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariableTypedWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            ITypedValue typedValue = runtimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableTypedWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            ITypedValue typedValue = runtimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableTypedWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            ITypedValue typedValue = runtimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableTypedWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            ITypedValue typedValue = runtimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // RuntimeService#getVariableLocalTyped() ////////////////////////////////////////////

        public virtual void testGetVariableLocalTypedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariableLocalTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariableLocalTypedWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            ITypedValue typedValue = runtimeService.GetVariableLocalTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableLocalTypedWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            ITypedValue typedValue = runtimeService.GetVariableLocalTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableLocalTypedWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            ITypedValue typedValue = runtimeService.GetVariableLocalTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        public virtual void testGetVariableLocalTypedWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            ITypedValue typedValue = runtimeService.GetVariableLocalTyped<ITypedValue>(ProcessInstanceId, VARIABLE_NAME);

            // then
            Assert.NotNull(typedValue);
            Assert.AreEqual(VARIABLE_VALUE, typedValue.Value);
        }

        // RuntimeService#getVariables() ////////////////////////////////////////////

        public virtual void testGetVariablesWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariables(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesLocal() ////////////////////////////////////////////

        public virtual void testGetVariablesLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesLocal(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesTyped() ////////////////////////////////////////////

        public virtual void testGetVariablesTypedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesTyped(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesTypedWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesLocalTyped() ////////////////////////////////////////////

        public virtual void testGetVariablesLocalTypedWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesLocalTyped(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesLocalTypedWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariables() ////////////////////////////////////////////

        public virtual void testGetVariablesByNameWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesByNameWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesByNameWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesByNameWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesByNameWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesLocal() ////////////////////////////////////////////

        public virtual void testGetVariablesLocalByNameWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesLocalByNameWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalByNameWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalByNameWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalByNameWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IDictionary<string, object> variables = runtimeService.GetVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            Assert.NotNull(variables);
            Assert.IsFalse(variables.Count == 0);
            Assert.AreEqual(1, variables.Count);

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesTyped() ////////////////////////////////////////////

        public virtual void testGetVariablesTypedByNameWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesTypedByNameWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedByNameWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedByNameWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesTypedByNameWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#getVariablesLocalTyped() ////////////////////////////////////////////

        public virtual void testGetVariablesLocalTypedByNameWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.GetVariablesLocalTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);
                Assert.Fail("Exception expected: It should not be to retrieve the variable instances");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetVariablesLocalTypedByNameWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedByNameWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedByNameWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        public virtual void testGetVariablesLocalTypedByNameWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IVariableMap variables = runtimeService.GetVariablesLocalTyped(ProcessInstanceId, new List<string>() { VARIABLE_NAME }, false);

            // then
            Assert.NotNull(variables);
            Assert.IsTrue(DynamicQueryable.Any(variables));
            Assert.AreEqual(1, DynamicQueryable.Count(variables));

            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        // RuntimeService#SetVariable() ////////////////////////////////////////////

        public virtual void testSetVariableWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SetVariable(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetVariableWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SetVariable(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SetVariable(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariable(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariable(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // RuntimeService#setVariableLocal() ////////////////////////////////////////////

        public virtual void testSetVariableLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SetVariableLocal(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetVariableLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SetVariableLocal(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SetVariableLocal(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariableLocal(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariableLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariableLocal(ProcessInstanceId, VARIABLE_NAME, VARIABLE_VALUE);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // RuntimeService#setVariables() ////////////////////////////////////////////

        public virtual void testSetVariablesWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SetVariables(ProcessInstanceId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetVariablesWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SetVariables(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SetVariables(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariables(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariables(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // RuntimeService#setVariablesLocal() ////////////////////////////////////////////

        public virtual void testSetVariablesLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when
                runtimeService.SetVariablesLocal(ProcessInstanceId, Variables);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetVariablesLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.SetVariablesLocal(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.SetVariablesLocal(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariablesLocal(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        public virtual void testSetVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.SetVariablesLocal(ProcessInstanceId, Variables);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 1);
            enableAuthorization();
        }

        // RuntimeService#removeVariable() ////////////////////////////////////////////

        public virtual void testRemoveVariableWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.RemoveVariable(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testRemoveVariableWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariable(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // RuntimeService#removeVariableLocal() ////////////////////////////////////////////

        public virtual void testRemoveVariableLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.RemoveVariableLocal(ProcessInstanceId, VARIABLE_NAME);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testRemoveVariableLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariableLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariableLocal(ProcessInstanceId, VARIABLE_NAME);

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // RuntimeService#removeVariables() ////////////////////////////////////////////

        public virtual void testRemoveVariablesWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.RemoveVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testRemoveVariablesWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariables(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // RuntimeService#removeVariablesLocal() ////////////////////////////////////////////

        public virtual void testRemoveVariablesLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;

            try
            {
                // when
                runtimeService.RemoveVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testRemoveVariablesLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            runtimeService.RemoveVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testRemoveVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY, Variables).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            runtimeService.RemoveVariablesLocal(ProcessInstanceId, new List<string>() { VARIABLE_NAME });

            // then
            disableAuthorization();
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // RuntimeServiceImpl#updateVariables() ////////////////////////////////////////////

        public virtual void testUpdateVariablesWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when (1)
                ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, null);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (1)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (2)
                ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (2)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (3)
                ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (3)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testUpdateVariablesWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariables(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // RuntimeServiceImpl#updateVariablesLocal() ////////////////////////////////////////////

        public virtual void testUpdateVariablesLocalWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            try
            {
                // when (1)
                ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, null);
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (1)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (2)
                ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (2)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }

            try
            {
                // when (3)
                ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });
                Assert.Fail("Exception expected: It should not be to set a variable");
            }
            catch (AuthorizationException e)
            {
                // then (3)
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testUpdateVariablesLocalWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesLocalWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesLocalWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.UpdateInstance);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testUpdateVariablesLocalWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery();

            // when (1)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, null);

            // then (1)
            disableAuthorization();
            //verifyQueryResults(query, 1);
            enableAuthorization();

            // when (2)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, null, new List<string>() { VARIABLE_NAME });

            // then (2)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();

            // when (3)
            ((RuntimeServiceImpl)runtimeService).UpdateVariablesLocal(ProcessInstanceId, Variables, new List<string>() { VARIABLE_NAME });

            // then (3)
            disableAuthorization();
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        // helper /////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults( IQueryable<IProcessInstance> query, int countExpected)
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