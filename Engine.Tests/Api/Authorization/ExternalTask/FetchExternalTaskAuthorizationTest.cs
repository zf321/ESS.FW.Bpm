using ESS.FW.Bpm.Engine.Authorization;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class FetchExternalTaskAuthorizationTest : AuthorizationTest
    {

        public const string WORKER_ID = "workerId";
        public const long LOCK_TIME = 10000L;

        protected internal string deploymentId;

        protected internal string instance1Id;
        protected internal string instance2Id;

        [SetUp]
        protected internal override void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml").Id;

            instance1Id = StartProcessInstanceByKey("oneExternalTaskProcess").Id;
            instance2Id = StartProcessInstanceByKey("twoExternalTaskProcess").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        public virtual void testFetchWithoutAuthorization()
        {
            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(0, tasks.Count);
        }

        public virtual void testFetchWithReadOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(0, tasks.Count);
        }

        public virtual void testFetchWithUpdateOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(0, tasks.Count);
        }

        public virtual void testFetchWithReadAndUpdateOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read, Permissions.Update);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(1, tasks.Count);
            ////Assert.AreEqual(instance1Id, tasks[0].ProcessInstanceId);
        }

        public virtual void testFetchWithReadInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.ReadInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(0, tasks.Count);
        }

        public virtual void testFetchWithUpdateInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.UpdateInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            ////Assert.AreEqual(0, tasks.Count);
        }

        public virtual void testFetchWithReadAndUpdateInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.ReadInstance, Permissions.UpdateInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(1, tasks.Count);
            //Assert.AreEqual(instance1Id, tasks[0].ProcessInstanceId);
        }

        public virtual void testFetchWithReadOnProcessInstanceAndUpdateInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.UpdateInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(1, tasks.Count);
            //Assert.AreEqual(instance1Id, tasks[0].ProcessInstanceId);
        }

        public virtual void testFetchWithUpdateOnProcessInstanceAndReadInstanceOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Update);
            createGrantAuthorization(Resources.ProcessDefinition, "oneExternalTaskProcess", userId, Permissions.ReadInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(1, tasks.Count);
            //Assert.AreEqual(instance1Id, tasks[0].ProcessInstanceId);
        }

        public virtual void testFetchWithReadAndUpdateOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read, Permissions.Update);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testQueryWithReadAndUpdateInstanceOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance, Permissions.UpdateInstance);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(2, tasks.Count);
        }

        public virtual void testQueryWithReadProcessInstanceAndUpdateInstanceOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            createGrantAuthorization(Resources.ProcessInstance, instance1Id, userId, Permissions.Read);

            // when
            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID).Topic("externalTaskTopic", LOCK_TIME).Execute();

            // then
            //Assert.AreEqual(1, tasks.Count);
            //Assert.AreEqual(instance1Id, tasks[0].ProcessInstanceId);
        }

    }

}