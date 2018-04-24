using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class JobDefinitionAuthorizationTest : AuthorizationTest
    {

        protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
        protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/timerStartEventProcess.bpmn20.xml", "resources/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // job definition query ///////////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given

            // when
            IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Read);

            // when
            IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        // suspend job definition by id ///////////////////////////////

        public virtual void testSuspendByIdWithoutAuthorization()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SuspendJobDefinitionById(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendByIdWithUpdatePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            // when
            managementService.SuspendJobDefinitionById(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        public virtual void testSuspendByIdWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            // when
            managementService.SuspendJobDefinitionById(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        // activate job definition by id ///////////////////////////////

        public virtual void testActivateByIdWithoutAuthorization()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionById(jobDefinitionId);

            try
            {
                // when
                managementService.ActivateJobDefinitionById(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateByIdWithUpdatePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionById(jobDefinitionId);

            // when
            managementService.ActivateJobDefinitionById(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        public virtual void testActivateByIdWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionById(jobDefinitionId);

            // when
            managementService.ActivateJobDefinitionById(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        // suspend job definition by id (including jobs) ///////////////////////////////

        public virtual void testSuspendIncludingJobsByIdWithoutAuthorization()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionById(jobDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionById(jobDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByIdWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByIdWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job definition by id (including jobs) ///////////////////////////////

        public virtual void testActivateIncludingJobsByIdWithoutAuthorization()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            suspendJobDefinitionIncludingJobsById(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionById(jobDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsById(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionById(jobDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsById(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByIdWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsById(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByIdWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsById(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionById(jobDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // suspend job definition by process definition id ///////////////////////////////

        public virtual void testSuspendByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendByProcessDefinitionIdWithUpdatePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        public virtual void testSuspendByProcessDefinitionIdWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        // activate job definition by process definition id ///////////////////////////////

        public virtual void testActivateByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateByProcessDefinitionIdWithUpdatePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        public virtual void testActivateByProcessDefinitionIdWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            SuspendJobDefinitionByProcessDefinitionId(processDefinitionId);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        // suspend job definition by process definition id (including jobs) ///////////////////////////////

        public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job definition by id (including jobs) ///////////////////////////////

        public virtual void testActivateIncludingJobsByProcessDefinitionIdWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinitionId, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // suspend job definition by process definition key ///////////////////////////////

        public virtual void testSuspendByProcessDefinitionKeyWithoutAuthorization()
        {
            // given

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendByProcessDefinitionKeyWithUpdatePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        public virtual void testSuspendByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);
        }

        // activate job definition by process definition key ///////////////////////////////

        public virtual void testActivateByProcessDefinitionKeyWithoutAuthorization()
        {
            // given
            SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateByProcessDefinitionKeyWithUpdatePermissionOnProcessDefinition()
        {
            // given
            SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        public virtual void testActivateByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);
        }

        // suspend job definition by process definition key (including jobs) ///////////////////////////////

        public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.True(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job definition by id (including jobs) ///////////////////////////////

        public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);
                Assert.Fail("Exception expected: It should not be possible to suspend a job definition");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateIncludingJobsByProcessDefinitionKeyWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobDefinitionIncludingJobsByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY, true);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            Assert.NotNull(jobDefinition);
            Assert.IsFalse(jobDefinition.Suspended);

            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // helper /////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IJobDefinition> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual ESS.FW.Bpm.Engine.Management.IJobDefinition selectJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey==processDefinitionKey).First();
            enableAuthorization();
            return jobDefinition;
        }

        protected internal virtual ESS.FW.Bpm.Engine.Runtime.IJob selectJobByProcessInstanceId(string ProcessInstanceId)
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
            enableAuthorization();
            return job;
        }

    }

}