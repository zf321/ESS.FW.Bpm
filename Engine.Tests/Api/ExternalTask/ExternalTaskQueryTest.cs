//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.ExternalTask
//{
    
//    public class ExternalTaskQueryTest : PluggableProcessEngineTestCase
//    {

//        protected internal const string WORKER_ID = "aWorkerId";
//        protected internal const string TOPIC_NAME = "externalTaskTopic";
//        protected internal const string ERROR_MESSAGE = "error";

//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: protected void setUp() throws Exception
//        protected internal virtual void setUp()
//        {
//            ClockUtil.CurrentTime = DateTime.Now;
//        }

//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: protected void tearDown() throws Exception
//        protected internal virtual void tearDown()
//        {
//            ClockUtil.Reset();
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSingleResult()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            Externaltask.IExternalTask externalTask = externalTaskService.CreateExternalTaskQuery().First();

//            // then
//            Assert.NotNull(externalTask.Id);

//            Assert.AreEqual(processInstance.Id, externalTask.ProcessInstanceId);
//            Assert.AreEqual("externalTask", externalTask.ActivityId);
//            Assert.NotNull(externalTask.ActivityInstanceId);
//            Assert.NotNull(externalTask.ExecutionId);
//            Assert.AreEqual(processInstance.ProcessDefinitionId, externalTask.ProcessDefinitionId);
//            Assert.AreEqual("oneExternalTaskProcess", externalTask.ProcessDefinitionKey);
//            Assert.AreEqual(TOPIC_NAME, externalTask.TopicName);
//            Assert.IsNull(externalTask.WorkerId);
//            Assert.IsNull(externalTask.LockExpirationTime);
//            Assert.IsFalse(externalTask.Suspended);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testList()
//        {
//            startInstancesByKey("oneExternalTaskProcess", 5);
//            Assert.AreEqual(5, externalTaskService.CreateExternalTaskQuery().Count());
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCount()
//        {
//            startInstancesByKey("oneExternalTaskProcess", 5);
//            Assert.AreEqual(5, externalTaskService.CreateExternalTaskQuery().Count());
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByLockState()
//        {
//            // given
//            startInstancesByKey("oneExternalTaskProcess", 5);
//            lockInstances(TOPIC_NAME, 10000L, 3, WORKER_ID);

//            // when
//            IList<Externaltask.IExternalTask> lockedTasks = externalTaskService.CreateExternalTaskQuery().Locked().ToList();
//            IList<Externaltask.IExternalTask> nonLockedTasks = externalTaskService.CreateExternalTaskQuery().NotLocked().ToList();

//            // then
//            Assert.AreEqual(3, lockedTasks.Count);
//            foreach (Externaltask.IExternalTask task in lockedTasks)
//            {
//                Assert.NotNull(task.LockExpirationTime);
//            }

//            Assert.AreEqual(2, nonLockedTasks.Count);
//            foreach (Externaltask.IExternalTask task in nonLockedTasks)
//            {
//                Assert.IsNull(task.LockExpirationTime);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            // given
//            IDeployment secondDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml").Deploy();

//            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery(null).ToList();

//            startInstancesById(processDefinitions[0].Id, 3);
//            startInstancesById(processDefinitions[1].Id, 2);

//            // when
//            IList<Externaltask.IExternalTask> definition1Tasks = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessDefinitionId == processDefinitions[0].Id).ToList();
//            IList<Externaltask.IExternalTask> definition2Tasks = externalTaskService.CreateExternalTaskQuery(c => c.ProcessDefinitionId == processDefinitions[1].Id).ToList();

//            // then
//            Assert.AreEqual(3, definition1Tasks.Count);
//            foreach (Externaltask.IExternalTask task in definition1Tasks)
//            {
//                Assert.AreEqual(processDefinitions[0].Id, task.ProcessDefinitionId);
//            }

//            Assert.AreEqual(2, definition2Tasks.Count);
//            foreach (Externaltask.IExternalTask task in definition2Tasks)
//            {
//                Assert.AreEqual(processDefinitions[1].Id, task.ProcessDefinitionId);
//            }

//            // cleanup
//            repositoryService.DeleteDeployment(secondDeployment.Id, true);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/parallelExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByActivityId()
//        {
//            // given
//            startInstancesByKey("parallelExternalTaskProcess", 3);

//            // when
//            IList<Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery(c=>c.ActivityId == "externalTask2").ToList();

//            // then
//            Assert.AreEqual(3, tasks.Count);
//            foreach (Externaltask.IExternalTask task in tasks)
//            {
//                Assert.AreEqual("externalTask2", task.ActivityId);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/parallelExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByActivityIdIn()
//        {
//            // given
//            startInstancesByKey("parallelExternalTaskProcess", 3);

//            IList<string> activityIds = new string[] { "externalTask1", "externalTask2" };

//            // when
//            IList<Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery(c=>c.ActivityId ==(string[])activityIds.ToArray()).ToList();

//            // then
//            Assert.AreEqual(6, tasks.Count);
//            foreach (Externaltask.IExternalTask task in tasks)
//            {
//                Assert.True(activityIds.Contains(task.ActivityId));
//            }
//        }

//        public virtual void testFailQueryByActivityIdInNull()
//        {
//            try
//            {
//                externalTaskService.CreateExternalTaskQuery(c=>c.ActivityId ==(string)null);
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException)
//            {
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/parallelExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByTopicName()
//        {
//            // given
//            startInstancesByKey("parallelExternalTaskProcess", 3);

//            // when
//            IList<Externaltask.IExternalTask> topic1Tasks = externalTaskService.CreateExternalTaskQuery().TopicName("topic1").ToList();

//            // then
//            Assert.AreEqual(3, topic1Tasks.Count);
//            foreach (Externaltask.IExternalTask task in topic1Tasks)
//            {
//                Assert.AreEqual("topic1", task.TopicName);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            // given
//            IList<IProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 3);

//            // when
//            Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == processInstances[0].Id).First();

//            // then
//            Assert.NotNull(task);
//            Assert.AreEqual(processInstances[0].Id, task.ProcessInstanceId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByExecutionId()
//        {
//            // given
//            IList<IProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 3);

//            IProcessInstance firstInstance = processInstances[0];

//            IActivityInstance externalTaskActivityInstance = runtimeService.GetActivityInstance(firstInstance.Id).GetActivityInstances("externalTask")[0];
//            string executionId = externalTaskActivityInstance.ExecutionIds[0];

//            // when
//            Externaltask.IExternalTask externalTask = externalTaskService.CreateExternalTaskQuery(c=>c.Id == executionId).First();

//            // then
//            Assert.NotNull(externalTask);
//            Assert.AreEqual(executionId, externalTask.ExecutionId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByWorkerId()
//        {
//            // given
//            startInstancesByKey("oneExternalTaskProcess", 10);
//            lockInstances(TOPIC_NAME, 10000L, 3, "worker1");
//            lockInstances(TOPIC_NAME, 10000L, 4, "worker2");

//            // when
//            IList<Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery(c=>c.WorkerId =="worker1").ToList();

//            // then
//            Assert.AreEqual(3, tasks.Count);
//            foreach (Externaltask.IExternalTask task in tasks)
//            {
//                Assert.AreEqual("worker1", task.WorkerId);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByLockExpirationTime()
//        {
//            // given
//            startInstancesByKey("oneExternalTaskProcess", 10);
//            lockInstances(TOPIC_NAME, 5000L, 3, WORKER_ID);
//            lockInstances(TOPIC_NAME, 10000L, 4, WORKER_ID);

//            // when
//            DateTime lockDate = new DateTime(ClockUtil.CurrentTime.Millisecond + 7000L);
//            IList<Externaltask.IExternalTask> lockedExpirationBeforeTasks = externalTaskService.CreateExternalTaskQuery().LockExpirationBefore(lockDate).ToList();

//            IList<Externaltask.IExternalTask> lockedExpirationAfterTasks = externalTaskService.CreateExternalTaskQuery().LockExpirationAfter(lockDate).ToList();

//            // then
//            Assert.AreEqual(3, lockedExpirationBeforeTasks.Count);
//            foreach (Externaltask.IExternalTask task in lockedExpirationBeforeTasks)
//            {
//                Assert.NotNull(task.LockExpirationTime);
//                Assert.True(task.LockExpirationTime.Ticks < lockDate.Ticks);
//            }

//            Assert.AreEqual(4, lockedExpirationAfterTasks.Count);
//            foreach (Externaltask.IExternalTask task in lockedExpirationAfterTasks)
//            {
//                Assert.NotNull(task.LockExpirationTime);
//                Assert.True(task.LockExpirationTime.Ticks > lockDate.Ticks);
//            }
//        }

//        public virtual void testQueryWithNullValues()
//        {
//            try
//            {
//                externalTaskService.CreateExternalTaskQuery().ExternalTaskId(null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("externalTaskId is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery(c=>c.ActivityId == null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("activityId is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery(c => c.ExecutionId == null)
//                    .ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("executionId is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery().LockExpirationAfter(DateTime.Parse(null)).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("lockExpirationAfter is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery().LockExpirationBefore(DateTime.Parse(null)).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("lockExpirationBefore is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery(c=>c.ProcessDefinitionId ==null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("processDefinitionId is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("ProcessInstanceId is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery().TopicName(null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("topicName is null", e.Message);
//            }

//            try
//            {
//                externalTaskService.CreateExternalTaskQuery().WorkerId(null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("workerId is null", e.Message);
//            }
//        }

//        // Todo: TestOrderingUtil many methods
//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQuerySorting()
//        {

//            startInstancesByKey("oneExternalTaskProcess", 5);
//            startInstancesByKey("twoExternalTaskProcess", 5);

//            lockInstances(TOPIC_NAME, 5000L, 5, WORKER_ID);
//            lockInstances(TOPIC_NAME, 10000L, 5, WORKER_ID);

//            // asc
//            IList<Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery()//.OrderById()/*.Asc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.externalTaskById());

//            tasks = externalTaskService.CreateExternalTaskQuery()//.OrderByProcessInstanceId()/*.Asc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.externalTaskByProcessInstanceId());

//            tasks = externalTaskService.CreateExternalTaskQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.externalTaskByProcessDefinitionId());

//            tasks = externalTaskService.CreateExternalTaskQuery()//.OrderByProcessDefinitionKey()/*.Asc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.externalTaskByProcessDefinitionKey());

//            tasks = externalTaskService.CreateExternalTaskQuery().OrderByLockExpirationTime()/*.Asc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.externalTaskByLockExpirationTime());

//            // Desc
//            tasks = externalTaskService.CreateExternalTaskQuery()//.OrderById()/*.Desc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskById()));

//            tasks = externalTaskService.CreateExternalTaskQuery()//.OrderByProcessInstanceId()/*.Desc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskByProcessInstanceId()));

//            tasks = externalTaskService.CreateExternalTaskQuery()/*.OrderByProcessDefinitionId()*//*.Desc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskByProcessDefinitionId()));

//            tasks = externalTaskService.CreateExternalTaskQuery()//.OrderByProcessDefinitionKey()/*.Desc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskByProcessDefinitionKey()));

//            tasks = externalTaskService.CreateExternalTaskQuery().OrderByLockExpirationTime()/*.Desc()*/.ToList();
//            Assert.AreEqual(10, tasks.Count);
//            //TestOrderingUtil.verifySorting(tasks, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskByLockExpirationTime()));
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryBySuspensionState()
//        {
//            // given
//            startInstancesByKey("oneExternalTaskProcess", 5);
//            suspendInstances(3);

//            // when
//            IList<Externaltask.IExternalTask> suspendedTasks = externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode).ToList();
//            IList<Externaltask.IExternalTask> activeTasks = externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).ToList();

//            // then
//            Assert.AreEqual(3, suspendedTasks.Count);
//            foreach (Externaltask.IExternalTask task in suspendedTasks)
//            {
//                Assert.True(task.Suspended);
//            }

//            Assert.AreEqual(2, activeTasks.Count);
//            foreach (Externaltask.IExternalTask task in activeTasks)
//            {
//                Assert.IsFalse(task.Suspended);
//                Assert.IsFalse(suspendedTasks.Contains(task));
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByRetries()
//        {
//            // given
//            startInstancesByKey("oneExternalTaskProcess", 5);

//            IList<ILockedExternalTask> tasks = lockInstances(TOPIC_NAME, 10000L, 3, WORKER_ID);
//            //failInstances(tasks.SubList(0, 2), ERROR_MESSAGE, 0, 5000L); // two tasks have no retries left
//            failInstances(tasks.Take(2).ToList();, ERROR_MESSAGE, 0, 5000L);
//            /*failInstances(tasks.SubList(2, 3), ERROR_MESSAGE, 4, 5000L);*/ // one task has retries left
//            failInstances(tasks.Skip(2).Take(3).ToList();, ERROR_MESSAGE, 4, 5000L);

//            // when
//            IList<Externaltask.IExternalTask> tasksWithRetries = externalTaskService.CreateExternalTaskQuery(c=>c.Retries > 0).ToList();
//            IList<Externaltask.IExternalTask> tasksWithoutRetries = externalTaskService.CreateExternalTaskQuery(c=>c.Retries == 0).ToList();

//            // then
//            Assert.AreEqual(3, tasksWithRetries.Count);
//            foreach (Externaltask.IExternalTask task in tasksWithRetries)
//            {
//                Assert.True(task.Retries == null || task.Retries > 0);
//            }

//            Assert.AreEqual(2, tasksWithoutRetries.Count);
//            foreach (Externaltask.IExternalTask task in tasksWithoutRetries)
//            {
//                Assert.True(task.Retries == 0);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryById()
//        {
//            // given
//            IList<IProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 2);
//            IList<Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery().ToList();

//            IProcessInstance firstInstance = processInstances[0];
//            Externaltask.IExternalTask firstTask = tasks[0];
//            if (!firstTask.ProcessInstanceId.Equals(firstInstance.Id))
//            {
//                firstTask = tasks[1];
//            }

//            // when
//            Externaltask.IExternalTask resultTask = externalTaskService.CreateExternalTaskQuery().ExternalTaskId(firstTask.Id).First();

//            // then
//            Assert.AreEqual(firstTask.Id, resultTask.Id);
//        }

//        protected internal virtual IList<IProcessInstance> startInstancesByKey(string processDefinitionKey, int number)
//        {
//            IList<IProcessInstance> processInstances = new List<IProcessInstance>();
//            for (int i = 0; i < number; i++)
//            {
//                processInstances.Add(runtimeService.StartProcessInstanceByKey(processDefinitionKey));
//            }

//            return processInstances;
//        }

//        protected internal virtual IList<IProcessInstance> startInstancesById(string processDefinitionId, int number)
//        {
//            IList<IProcessInstance> processInstances = new List<IProcessInstance>();
//            for (int i = 0; i < number; i++)
//            {
//                processInstances.Add(runtimeService.StartProcessInstanceById(processDefinitionId));
//            }

//            return processInstances;
//        }

//        protected internal virtual void suspendInstances(int number)
//        {
//            IList<IProcessInstance> processInstances = runtimeService.CreateProcessInstanceQuery().ListPage(0, number).ToList();

//            foreach (IProcessInstance processInstance in processInstances)
//            {
//                runtimeService.SuspendProcessInstanceById(processInstance.Id);
//            }
//        }

//        protected internal virtual IList<ILockedExternalTask> lockInstances(string topic, long duration, int number, string workerId)
//        {
//            return externalTaskService.FetchAndLock(number, workerId).Topic(topic, duration).Execute();
//        }

//        protected internal virtual void failInstances(IList<ILockedExternalTask> tasks, string errorMessage, int retries, long retryTimeout)
//        {
//            this.failInstances(tasks, errorMessage, null, retries, retryTimeout);
//        }

//        protected internal virtual void failInstances(IList<ILockedExternalTask> tasks, string errorMessage, string errorDetails, int retries, long retryTimeout)
//        {
//            foreach (ILockedExternalTask task in tasks)
//            {
//                externalTaskService.HandleFailure(task.Id, task.WorkerId, errorMessage, errorDetails, retries, retryTimeout);
//            }
//        }

//    }

//}