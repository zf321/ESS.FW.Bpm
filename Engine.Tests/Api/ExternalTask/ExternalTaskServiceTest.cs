//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.History.Impl;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.IExternalTask;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Engine.Variable;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Common.Extensions;
//using ESS.FW.Common.Utilities;
//using NUnit.Framework;
//using ESS.FW.Bpm.Engine.Task;

//namespace ESS.FW.Bpm.Engine.Tests.Api.ExternalTask
//{
//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    public class ExternalTaskServiceTest : PluggableProcessEngineTestCase
//    {

//        protected internal const string WORKER_ID = "aWorkerId";
//        protected internal const long LOCK_TIME = 10000L;
//        protected internal const string TOPIC_NAME = "externalTaskTopic";

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

//        public virtual void testFailOnMalformedpriorityInput()
//        {
//            try
//            {
//                repositoryService.CreateDeployment().AddClasspathResource("resources/api/externaltask/externalTaskInvalidPriority.bpmn20.xml").Deploy();
//                Assert.Fail("deploying a process with malformed priority should not succeed");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresentIgnoreCase("value 'NOTaNumber' for attribute 'taskPriority' " + "is not a valid number", e.Message);
//            }
//        }


//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetch()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            //IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            //// then
//            //Assert.AreEqual(1, externalTasks.Count);

//            //ILockedExternalTask task = externalTasks[0];
//            //Assert.NotNull(task.Id);
//            //Assert.AreEqual(processInstance.Id, task.ProcessInstanceId);
//            //Assert.AreEqual(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
//            //Assert.AreEqual("externalTask", task.ActivityId);
//            //Assert.AreEqual("oneExternalTaskProcess", task.ProcessDefinitionKey);
//            //Assert.AreEqual(TOPIC_NAME, task.TopicName);

//            //IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id).GetActivityInstances("externalTask")[0];

//            //Assert.AreEqual(activityInstance.Id, task.ActivityInstanceId);
//            //Assert.AreEqual(activityInstance.ExecutionIds[0], task.ExecutionId);

//            //AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

//            //Assert.AreEqual(WORKER_ID, task.WorkerId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml" })]
//        public virtual void testFetchWithPriority()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            Assert.AreEqual(1, externalTasks.Count);

//            ILockedExternalTask task = externalTasks[0];
//            Assert.NotNull(task.Id);
//            Assert.AreEqual(processInstance.Id, task.ProcessInstanceId);
//            Assert.AreEqual(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
//            Assert.AreEqual("externalTaskWithPrio", task.ActivityId);
//            Assert.AreEqual("twoExternalTaskWithPriorityProcess", task.ProcessDefinitionKey);
//            Assert.AreEqual(TOPIC_NAME, task.TopicName);
//            Assert.AreEqual(7, task.Priority);

//            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id).GetActivityInstances("externalTaskWithPrio")[0];

//            Assert.AreEqual(activityInstance.Id, task.ActivityInstanceId);
//            Assert.AreEqual(activityInstance.ExecutionIds[0], task.ExecutionId);

//            AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

//            Assert.AreEqual(WORKER_ID, task.WorkerId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
//        public virtual void testFetchProcessWithPriority()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(2, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(2, externalTasks.Count);

//            // then
//            //task with no prio gets prio defined by process
//            Assert.AreEqual(9, externalTasks[0].Priority);
//            //task with own prio overrides prio defined by process
//            Assert.AreEqual(7, externalTasks[1].Priority);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpressionProcess.bpmn20.xml" })]
//        public virtual void testFetchProcessWithPriorityExpression()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Engine.Variable.Variables.CreateVariables().PutValue("priority", 18));

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(2, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(2, externalTasks.Count);

//            // then
//            //task with no prio gets prio defined by process
//            Assert.AreEqual(18, externalTasks[0].Priority);
//            //task with own prio overrides prio defined by process
//            Assert.AreEqual(7, externalTasks[1].Priority);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
//        public virtual void testFetchWithPriorityExpression()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Engine.Variable.Variables.CreateVariables().PutValue("priority", 18));
//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            Assert.AreEqual(1, externalTasks.Count);

//            ILockedExternalTask task = externalTasks[0];
//            Assert.NotNull(task.Id);
//            Assert.AreEqual(processInstance.Id, task.ProcessInstanceId);
//            Assert.AreEqual(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
//            Assert.AreEqual("externalTaskWithPrio", task.ActivityId);
//            Assert.AreEqual("twoExternalTaskWithPriorityProcess", task.ProcessDefinitionKey);
//            Assert.AreEqual(TOPIC_NAME, task.TopicName);
//            Assert.AreEqual(18, task.Priority);

//            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id).GetActivityInstances("externalTaskWithPrio")[0];

//            Assert.AreEqual(activityInstance.Id, task.ActivityInstanceId);
//            Assert.AreEqual(activityInstance.ExecutionIds[0], task.ExecutionId);

//            AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

//            Assert.AreEqual(WORKER_ID, task.WorkerId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml" })]
//        public virtual void testFetchWithPriorityOrdering()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(2, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            Assert.AreEqual(2, externalTasks.Count);
//            Assert.True(externalTasks[0].Priority > externalTasks[1].Priority);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml" })]
//        public virtual void testFetchNextWithPriority()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then the task is locked
//            Assert.AreEqual(1, externalTasks.Count);

//            ILockedExternalTask task = externalTasks[0];
//            long firstPrio = task.Priority;
//            AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

//            // another task with next higher priority can be claimed
//            externalTasks = externalTaskService.FetchAndLock(1, "anotherWorkerId", true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(1, externalTasks.Count);
//            Assert.True(firstPrio >= externalTasks[0].Priority);

//            // the expiration time expires
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);

//            //first can be claimed
//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(1, externalTasks.Count);
//            Assert.AreEqual(firstPrio, externalTasks[0].Priority);
//        }

//        [Deployment]
//        public virtual void testFetchTopicSelection()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoTopicsProcess");

//            // when
//            IQueryable<Externaltask.IExternalTask> topic1Tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic1", LOCK_TIME)*/.Execute();

//            IQueryable<Externaltask.IExternalTask> topic2Tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic2", LOCK_TIME)*/.Execute();

//            // then
//            //Assert.AreEqual(1, topic1Tasks.Count);
//            //Assert.AreEqual("topic1", topic1Tasks[0].TopicName);

//            //Assert.AreEqual(1, topic2Tasks.Count);
//            //Assert.AreEqual("topic2", topic2Tasks[0].TopicName);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchWithoutTopicName()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            try
//            {
//                externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(null, LOCK_TIME)*/.Execute();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("topicName is null", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchNullWorkerId()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            try
//            {
//                externalTaskService.FetchAndLock(1, null)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("workerId is null", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchNegativeNumberOfTasks()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            try
//            {
//                externalTaskService.FetchAndLock(-1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("maxResults is not greater than or equal to 0", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchLessTasksThanExist()
//        {
//            // given
//            for (int i = 0; i < 10; i++)
//            {
//                runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            }

//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(5, externalTasks.Count);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchNegativeLockTime()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            try
//            {
//                externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, -1L)*/.Execute();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("lockTime is not greater than 0", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchZeroLockTime()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            try
//            {
//                externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME,0L)*/.Execute();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("lockTime is not greater than 0", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchNoTopics()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID).Execute();

//            // then
//            Assert.AreEqual(0, tasks.Count);
//        }

//        [Deployment]
//        public virtual void testFetchVariables()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", 42).PutValue("processVar2", 43));

//            // when
//            //IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables("processVar1", "subProcessVar", "taskVar").Execute();

//            // then
//            //ILockedExternalTask task = externalTasks[0];
//            //IVariableMap variables = task.Variables;
//            //Assert.AreEqual(3, variables.Count);

//            //Assert.AreEqual(42, variables.GetValue("processVar1", typeof(Int32)));
//            //Assert.AreEqual(44L, variables.GetValue("subProcessVar", typeof(Int64)));
//            //Assert.AreEqual(45L, variables.GetValue("taskVar", typeof(Int64)));

//        }

//        [Deployment(new string[] { "resources/api/externaltask/ExternalTaskServiceTest.TestFetchVariables.bpmn20.xml" })]
//        public virtual void testShouldNotFetchSerializedVariables()
//        {
//            // given
//            ExternalTaskCustomValue customValue = new ExternalTaskCustomValue();
//            customValue.TestValue = "value1";
//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", customValue));

//            // when
//            //IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables("processVar1").Execute();

//            // then
//            //ILockedExternalTask task = externalTasks[0];
//            //IVariableMap variables = task.Variables;
//            //Assert.AreEqual(1, variables.Count);

//            //try
//            //{
//            //    variables.GetValue("processVar1", typeof(object));
//            //    Assert.Fail("did not receive an exception although variable was serialized");
//            //}
//            //catch (System.InvalidOperationException e)
//            //{
//            //    Assert.AreEqual("Object is not deserialized.", e.Message);
//            //}
//        }

//        [Deployment(new string[] { "resources/api/externaltask/ExternalTaskServiceTest.TestFetchVariables.bpmn20.xml" })]
//        public virtual void testFetchSerializedVariables()
//        {
//            // given
//            ExternalTaskCustomValue customValue = new ExternalTaskCustomValue();
//            customValue.TestValue = "value1";
//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", customValue));

//            // when
//            //IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables("processVar1").EnableCustomObjectDeserialization().Execute();

//            // then
//            //ILockedExternalTask task = externalTasks[0];
//            //IVariableMap variables = task.Variables;
//            //Assert.AreEqual(1, variables.Count);

//            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            ////ORIGINAL LINE: final ExternalTaskCustomValue receivedCustomValue = (ExternalTaskCustomValue) variables.Get("processVar1");
//            //ExternalTaskCustomValue receivedCustomValue = (ExternalTaskCustomValue)variables.GetValue("processVar1", typeof(ExternalTaskCustomValue));
//            //Assert.NotNull(receivedCustomValue);
//            //Assert.NotNull(receivedCustomValue.TestValue);
//            //Assert.AreEqual("value1", receivedCustomValue.TestValue);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/ExternalTaskServiceTest.TestFetchVariables.bpmn20.xml" })]
//        public virtual void testFetchAllVariables()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", 42).PutValue("processVar2", 43));

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            ILockedExternalTask task = externalTasks[0];
//            verifyVariables(task);

//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", 42).PutValue("processVar2", 43));

//            //externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables((string[])null).Execute();

//            task = externalTasks[0];
//            verifyVariables(task);

//            runtimeService.StartProcessInstanceByKey("subProcessExternalTask", Engine.Variable.Variables.CreateVariables().PutValue("processVar1", 42).PutValue("processVar2", 43));

//            IList<string> list = null;
//            //externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables(list).Execute();

//            task = externalTasks[0];
//            verifyVariables(task);
//        }

//        private void verifyVariables(ILockedExternalTask task)
//        {
//            IVariableMap variables = task.Variables;
//            Assert.AreEqual(4, variables.Count);

//            Assert.AreEqual(42, variables.GetValue("processVar1", typeof(Int32)));
//            Assert.AreEqual(43, variables.GetValue("processVar2", typeof(Int32)));
//            Assert.AreEqual(44L, variables.GetValue("subProcessVar", typeof(Int64)));
//            Assert.AreEqual(45L, variables.GetValue("taskVar", typeof(Int64)));
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchNonExistingVariable()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Variables("nonExistingVariable").Execute();

//            //ILockedExternalTask task = tasks[0];

//            // then
//            //Assert.True(task.Variables.IsEmpty());
//        }

//        [Deployment]
//        public virtual void testFetchMultipleTopics()
//        {
//            // given a process instance with external tasks for topics "topic1", "topic2", and "topic3"
//            runtimeService.StartProcessInstanceByKey("parallelExternalTaskProcess");

//            // when fetching tasks for two topics
//            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic1", LOCK_TIME)*//*.Topic("topic2", LOCK_TIME * 2)*/.Execute();

//            // then those two tasks are locked
//            //Assert.AreEqual(2, tasks.Count);
//            //ILockedExternalTask topic1Task = "topic1".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];
//            //ILockedExternalTask topic2Task = "topic2".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];

//            //Assert.AreEqual("topic1", topic1Task.TopicName);
//            //AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), topic1Task.LockExpirationTime);

//            //Assert.AreEqual("topic2", topic2Task.TopicName);
//            //AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME * 2), topic2Task.LockExpirationTime);

//            //// and the third task can still be fetched
//            ////tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic1", LOCK_TIME)*//*.Topic("topic2", LOCK_TIME * 2)*//*.Topic("topic3", LOCK_TIME * 3)*/.Execute();

//            //Assert.AreEqual(1, tasks.Count);

//            //ILockedExternalTask topic3Task = tasks[0];
//            //Assert.AreEqual("topic3", topic3Task.TopicName);
//            //AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME * 3), topic3Task.LockExpirationTime);
//        }

//        [Deployment]
//        public virtual void testFetchMultipleTopicsWithVariables()
//        {
//            // given a process instance with external tasks for topics "topic1" and "topic2"
//            // both have local variables "var1" and "var2"
//            runtimeService.StartProcessInstanceByKey("parallelExternalTaskProcess", Engine.Variable.Variables.CreateVariables().PutValue("var1", 0).PutValue("var2", 0));

//            // when
//            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic1", LOCK_TIME)*//*.Variables("var1", "var2")*//*.Topic("topic2", LOCK_TIME)*//*.Variables("var1")*/.Execute();

//            //ILockedExternalTask topic1Task = "topic1".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];
//            //ILockedExternalTask topic2Task = "topic2".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];

//            //Assert.AreEqual("topic1", topic1Task.TopicName);
//            //Assert.AreEqual("topic2", topic2Task.TopicName);

//            //// then the correct variables have been fetched
//            //IVariableMap topic1Variables = topic1Task.Variables;
//            //Assert.AreEqual(2, topic1Variables.Count);
//            //Assert.AreEqual(1L, topic1Variables.GetValue("var1", typeof(Int64)));
//            //Assert.AreEqual(1L, topic1Variables.GetValue("var2", typeof(Int64)));

//            //IVariableMap topic2Variables = topic2Task.Variables;
//            //Assert.AreEqual(1, topic2Variables.Count);
//            //Assert.AreEqual(2L, topic2Variables.GetValue("var1", typeof(Int64)));

//        }

//        [Deployment(new string[] { "resources/api/externaltask/ExternalTaskServiceTest.TestFetchMultipleTopics.bpmn20.xml" })]
//        public virtual void testFetchMultipleTopicsMaxTasks()
//        {
//            // given
//            for (int i = 0; i < 10; i++)
//            {
//                runtimeService.StartProcessInstanceByKey("parallelExternalTaskProcess");
//            }

//            // when
//            //IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic("topic1", LOCK_TIME)*//*.Topic("topic2", LOCK_TIME)*/.Topic("topic3", LOCK_TIME).Execute();

//            // then 5 tasks were returned in total, not per topic
//            //Assert.AreEqual(5, tasks.Count);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchSuspendedTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when suspending the process instance
//            runtimeService.SuspendProcessInstanceById(processInstance.Id);

//            // then the external task cannot be fetched
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(0, externalTasks.Count);

//            // when activating the process instance
//            runtimeService.ActivateProcessInstanceById(processInstance.Id);

//            // then the task can be fetched
//            externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(1, externalTasks.Count);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testFetchAndLockWithInitialBuilder()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            //IExternalTaskQueryBuilder initialBuilder = externalTaskService.FetchAndLock(1, WORKER_ID);
//            //initialBuilder/*.Topic(TOPIC_NAME, LOCK_TIME)*/;

//            //// should execute regardless whether the initial builder is used or the builder returned by the
//            //// #topic invocation
//            //IList<ILockedExternalTask> tasks = initialBuilder.Execute();

//            //// then
//            //Assert.AreEqual(1, tasks.Count);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testComplete()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            externalTaskService.Complete(externalTasks[0].Id, WORKER_ID);

//            // then
//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, externalTasks.Count);

//            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
//            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("afterExternalTask").Done());
//        }


//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteWithVariables()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            externalTaskService.Complete(externalTasks[0].Id, WORKER_ID, Engine.Variable.Variables.CreateVariables().PutValue("var", 42));

//            // then
//            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
//            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("afterExternalTask").Done());


//            Assert.AreEqual(42, runtimeService.GetVariable(processInstance.Id, "var"));
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteWithWrongWorkerId()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then it is not possible to complete the task with a different worker id
//            try
//            {
//                externalTaskService.Complete(externalTasks[0].Id, "someCrazyWorkerId");
//                Assert.Fail("exception expected");
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent("cannot be completed by worker 'someCrazyWorkerId'. It is locked by worker '" + WORKER_ID + "'.", e.Message);
//            }
//        }

//        public virtual void testCompleteNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.Complete("nonExistingTaskId", WORKER_ID);
//                Assert.Fail("exception expected");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
//            }
//        }

//        public virtual void testCompleteNullTaskId()
//        {
//            try
//            {
//                externalTaskService.Complete(null, WORKER_ID);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("Cannot find external task with id " + null, e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteNullWorkerId()
//        {
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            try
//            {
//                externalTaskService.Complete(task.Id, null);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("workerId is null", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteSuspendedTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = externalTasks[0];

//            // when suspending the process instance
//            runtimeService.SuspendProcessInstanceById(processInstance.Id);

//            // then the external task cannot be completed
//            try
//            {
//                externalTaskService.Complete(task.Id, WORKER_ID);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("IExternalTask with id '" + task.Id + "' is suspended", e.Message);
//            }

//            AssertProcessNotEnded(processInstance.Id);

//            // when activating the process instance again
//            runtimeService.ActivateProcessInstanceById(processInstance.Id);

//            // then the task can be completed
//            externalTaskService.Complete(task.Id, WORKER_ID);

//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testLocking()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then the task is locked
//            Assert.AreEqual(1, externalTasks.Count);

//            ILockedExternalTask task = externalTasks[0];
//            AssertUtil.AssertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

//            // and cannot be retrieved by another query
//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, externalTasks.Count);

//            // unless the expiration time expires
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(1, externalTasks.Count);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteLockExpiredTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // and the lock expires without the task being reclaimed
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);
//            // then the task can successfully be completed
//            externalTaskService.Complete(externalTasks[0].Id, WORKER_ID);

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, externalTasks.Count);
//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testCompleteReclaimedLockExpiredTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // and the lock expires
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);

//            // and it is reclaimed by another worker
//            IList<ILockedExternalTask> reclaimedTasks = externalTaskService.FetchAndLock(1, "anotherWorkerId")/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then the first worker cannot complete the task
//            try
//            {
//                externalTaskService.Complete(externalTasks[0].Id, WORKER_ID);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("cannot be completed by worker '" + WORKER_ID + "'. It is locked by worker 'anotherWorkerId'.", e.Message);
//            }

//            // and the second worker can
//            externalTaskService.Complete(reclaimedTasks[0].Id, "anotherWorkerId");

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, externalTasks.Count);
//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testDeleteProcessInstance()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            runtimeService.DeleteProcessInstance(processInstance.Id, null);

//            // then
//            Assert.AreEqual(0, externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute().Count);
//            AssertProcessEnded(processInstance.Id);
//        }


//        [Deployment]
//        public virtual void testExternalTaskExecutionTreeExpansion()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("boundaryExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            ILockedExternalTask externalTask = tasks[0];

//            // when a non-interrupting boundary event is triggered meanwhile
//            // such that the execution tree is expanded
//            runtimeService.CorrelateMessage("Message");

//            // then the external task can still be completed
//            externalTaskService.Complete(externalTask.Id, WORKER_ID);

//            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
//            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("afterBoundaryTask").Done());


//            ITask afterBoundaryTask = taskService.CreateTaskQuery().First();
//            taskService.Complete(afterBoundaryTask.Id);

//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment]
//        public virtual void testExternalTaskExecutionTreeCompaction()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("concurrentExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            ILockedExternalTask externalTask = tasks[0];

//            ITask userTask = taskService.CreateTaskQuery().First();

//            // when the user task completes meanwhile, thereby trigger execution tree compaction
//            taskService.Complete(userTask.Id);

//            // then the external task can still be completed
//            externalTaskService.Complete(externalTask.Id, WORKER_ID);

//            tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, tasks.Count);

//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testUnlock()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = externalTasks[0];

//            // when unlocking the task
//            externalTaskService.Unlock(task.Id);

//            // then it can be acquired again
//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(1, externalTasks.Count);
//            ILockedExternalTask reAcquiredTask = externalTasks[0];
//            Assert.AreEqual(task.Id, reAcquiredTask.Id);
//        }

//        public virtual void testUnlockNullTaskId()
//        {
//            try
//            {
//                externalTaskService.Unlock(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("externalTaskId is null"));
//            }
//        }

//        public virtual void testUnlockNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.Unlock("nonExistingId");
//                Assert.Fail("expected exception");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("Cannot find external task with id nonExistingId", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailure()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            // when submitting a failure (after a simulated processing time of three seconds)
//            ClockUtil.CurrentTime = nowPlus(3000L);

//            string errorMessage = "errorMessage";
//            externalTaskService.HandleFailure(task.Id, WORKER_ID, errorMessage, 5, 3000L);

//            // then the task cannot be immediately acquired again
//            tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, tasks.Count);

//            // and no incident exists because there are still retries left
//            Assert.AreEqual(0, runtimeService.CreateIncidentQuery().Count());

//            // but when the retry time expires, the task is available again
//            ClockUtil.CurrentTime = nowPlus(4000L);

//            tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(1, tasks.Count);

//            // and the retries and error message are accessible
//            task = tasks[0];
//            Assert.AreEqual(errorMessage, task.ErrorMessage);
//            Assert.AreEqual(5, (int)task.Retries);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureWithErrorDetails()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            // when submitting a failure (after a simulated processing time of three seconds)
//            ClockUtil.CurrentTime = nowPlus(3000L);

//            string errorMessage;
//            string exceptionStackTrace;
//            try
//            {
//                //RuntimeSqlException cause = new RuntimeSqlException("test cause");
//                var cause = new Exception("test cause");
//                for (int i = 0; i < 10; i++)
//                {
//                    //cause = new RuntimeSqlException(cause);
//                    cause = new Exception("", cause);
//                }
//                throw cause;
//            }
//            catch (Exception e)
//            {
//                //exceptionStackTrace = ExceptionUtils.GetStackTrace(e);
//                exceptionStackTrace = e.StackTrace;
//                errorMessage = e.Message;
//                while (errorMessage.Length < 1000)
//                {
//                    errorMessage = errorMessage + ":" + e.Message;
//                }
//            }
//            Assert.That(exceptionStackTrace, Is.Not.Null);
//            //  make sure that stack trace is longer then errorMessage DB field length
//            Assert.That(exceptionStackTrace.Length, Is.GreaterThan(4000));
//            externalTaskService.HandleFailure(task.Id, WORKER_ID, errorMessage, exceptionStackTrace, 5, 3000L);
//            ClockUtil.CurrentTime = nowPlus(4000L);
//            tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.That(tasks.Count, Is.EqualTo(1));

//            // verify that exception is accessible properly
//            task = tasks[0];
//            Assert.That(task.ErrorMessage, Is.EqualTo(errorMessage.Substring(0, 666)));
//            Assert.That(task.Retries, Is.EqualTo(5));
//            Assert.That(externalTaskService.GetExternalTaskErrorDetails(task.Id), Is.EqualTo(exceptionStackTrace));
//            Assert.That(task.ErrorDetails, Is.EqualTo(exceptionStackTrace));
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureZeroRetries()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            // when reporting a failure and setting retries to 0
//            ClockUtil.CurrentTime = nowPlus(3000L);

//            string errorMessage = "errorMessage";
//            externalTaskService.HandleFailure(task.Id, WORKER_ID, errorMessage, 0, 3000L);

//            // then the task cannot be fetched anymore even when the lock expires
//            ClockUtil.CurrentTime = nowPlus(4000L);

//            tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, tasks.Count);

//            // and an incident has been created
//            IIncident incident = runtimeService.CreateIncidentQuery().First();

//            Assert.NotNull(incident);
//            Assert.NotNull(incident.Id);
//            Assert.AreEqual(errorMessage, incident.IncidentMessage);
//            Assert.AreEqual(task.ExecutionId, incident.ExecutionId);
//            Assert.AreEqual("externalTask", incident.ActivityId);
//            Assert.AreEqual(incident.Id, incident.CauseIncidentId);
//            Assert.AreEqual("failedExternalTask", incident.IncidentType);
//            Assert.AreEqual(task.ProcessDefinitionId, incident.ProcessDefinitionId);
//            Assert.AreEqual(task.ProcessInstanceId, incident.ProcessInstanceId);
//            Assert.AreEqual(incident.Id, incident.RootCauseIncidentId);
//            AssertUtil.AssertEqualsSecondPrecision(nowMinus(4000L), incident.IncidentTimestamp);
//            Assert.AreEqual(task.Id, incident.Configuration);
//            Assert.IsNull(incident.JobDefinitionId);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureAndDeleteProcessInstance()
//        {
//            // given a failed external task with incident
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            externalTaskService.HandleFailure(task.Id, WORKER_ID, "someError", 0, LOCK_TIME);

//            // when
//            runtimeService.DeleteProcessInstance(processInstance.Id, null);

//            // then
//            AssertProcessEnded(processInstance.Id);
//        }


//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureThenComplete()
//        {
//            // given a failed external task with incident
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            externalTaskService.HandleFailure(task.Id, WORKER_ID, "someError", 0, LOCK_TIME);

//            // when
//            externalTaskService.Complete(task.Id, WORKER_ID);

//            // then the task has been completed nonetheless
//            ITask followingTask = taskService.CreateTaskQuery().First();
//            Assert.NotNull(followingTask);
//            Assert.AreEqual("afterExternalTask", followingTask.TaskDefinitionKey);

//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureWithWrongWorkerId()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then it is not possible to complete the task with a different worker id
//            try
//            {
//                externalTaskService.HandleFailure(externalTasks[0].Id, "someCrazyWorkerId", "error", 5, LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent("Failure of External ITask " + externalTasks[0].Id + " cannot be reported by worker 'someCrazyWorkerId'. It is locked by worker '" + WORKER_ID + "'.", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.HandleFailure("nonExistingTaskId", WORKER_ID, "error", 5, LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNullTaskId()
//        {
//            try
//            {
//                externalTaskService.HandleFailure(null, WORKER_ID, "error", 5, LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("Cannot find external task with id " + null, e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNullWorkerId()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            try
//            {
//                externalTaskService.HandleFailure(externalTasks[0].Id, null, "error", 5, LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (NullValueException e)
//            {
//                AssertTextPresent("workerId is null", e.Message);
//            }

//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNegativeLockDuration()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            try
//            {
//                externalTaskService.HandleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, -LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("retryDuration is not greater than or equal to 0", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNegativeRetries()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then
//            try
//            {
//                externalTaskService.HandleFailure(externalTasks[0].Id, WORKER_ID, "error", -5, LOCK_TIME);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("retries is not greater than or equal to 0", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureNullErrorMessage()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // when
//            externalTaskService.HandleFailure(externalTasks[0].Id, WORKER_ID, null, 5, LOCK_TIME);

//            // then the failure was reported successfully and the error message is null
//            Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery().First();

//            Assert.AreEqual(5, (int)task.Retries);
//            Assert.IsNull(task.ErrorMessage);
//            Assert.IsNull(externalTaskService.GetExternalTaskErrorDetails(task.Id));
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleFailureSuspendedTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = externalTasks[0];

//            // when suspending the process instance
//            runtimeService.SuspendProcessInstanceById(processInstance.Id);

//            // then a failure cannot be reported
//            try
//            {
//                externalTaskService.HandleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, LOCK_TIME);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("IExternalTask with id '" + task.Id + "' is suspended", e.Message);
//            }

//            AssertProcessNotEnded(processInstance.Id);

//            // when activating the process instance again
//            runtimeService.ActivateProcessInstanceById(processInstance.Id);

//            // then the failure can be reported successfully
//            externalTaskService.HandleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, LOCK_TIME);

//            Externaltask.IExternalTask updatedTask = externalTaskService.CreateExternalTaskQuery().First();
//            Assert.AreEqual(5, (int)updatedTask.Retries);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetRetries()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // when
//            externalTaskService.SetRetries(externalTasks[0].Id, 5);

//            // then
//            Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery().First();

//            Assert.AreEqual(5, (int)task.Retries);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetRetriesResolvesFailureIncident()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask lockedTask = externalTasks[0];
//            externalTaskService.HandleFailure(lockedTask.Id, WORKER_ID, "error", 0, LOCK_TIME);

//            IIncident incident = runtimeService.CreateIncidentQuery().First();

//            // when
//            externalTaskService.SetRetries(lockedTask.Id, 5);

//            // then the incident is resolved
//            Assert.AreEqual(0, runtimeService.CreateIncidentQuery().Count());

//            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelFull.Id)
//            {

//                IHistoricIncident historicIncident = historyService.CreateHistoricIncidentQuery().First();
//                Assert.NotNull(historicIncident);
//                Assert.AreEqual(incident.Id, historicIncident.Id);
//                Assert.True(historicIncident.Resolved);
//            }

//            // and the task can be fetched again
//            ClockUtil.CurrentTime = nowPlus(LOCK_TIME + 3000L);

//            externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(1, externalTasks.Count);
//            Assert.AreEqual(lockedTask.Id, externalTasks[0].Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetRetriesToZero()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask lockedTask = externalTasks[0];

//            // when
//            externalTaskService.SetRetries(lockedTask.Id, 0);

//            // then
//            IIncident incident = runtimeService.CreateIncidentQuery().First();
//            Assert.NotNull(incident);
//            Assert.AreEqual(lockedTask.Id, incident.Configuration);

//            // and resetting the retries removes the incident again
//            externalTaskService.SetRetries(lockedTask.Id, 5);

//            Assert.AreEqual(0, runtimeService.CreateIncidentQuery().Count());

//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetRetriesNegative()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            try
//            {
//                // when
//                externalTaskService.SetRetries(externalTasks[0].Id, -5);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("retries is not greater than or equal to 0", e.Message);
//            }
//        }

//        public virtual void testSetRetriesNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.SetRetries("someExternalTaskId", 5);
//                Assert.Fail("expected exception");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("externalTask is null", e.Message);
//            }
//        }

//        public virtual void testSetRetriesNullTaskId()
//        {
//            try
//            {
//                externalTaskService.SetRetries((string)null, 5);
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("externalTaskId is null"));
//            }
//        }


//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetPriority()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // when
//            externalTaskService.SetPriority(externalTasks[0].Id, 5);

//            // then
//            Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery().First();

//            Assert.AreEqual(5, (int)task.Priority);
//        }


//        public virtual void testSetPriorityNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.SetPriority("someExternalTaskId", 5);
//                Assert.Fail("expected exception");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("externalTask is null", e.Message);
//            }
//        }

//        public virtual void testSetPriorityNullTaskId()
//        {
//            try
//            {
//                externalTaskService.SetPriority(null, 5);
//                Assert.Fail("expected exception");
//            }
//            catch (NullValueException e)
//            {
//                Assert.That(e.Message, Does.Contain("externalTaskId is null"));
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml" })]
//        public virtual void testAfterSetPriorityFetchHigherTask()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(2, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(2, externalTasks.Count);
//            ILockedExternalTask task = externalTasks[1];
//            Assert.AreEqual(0, task.Priority);
//            externalTaskService.SetPriority(task.Id, 9);
//            // and the lock expires without the task being reclaimed
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);
//            // then
//            externalTasks = externalTaskService.FetchAndLock(1, "anotherWorkerId", true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(externalTasks[0].Priority, 9);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testSetPriorityLockExpiredTask()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // and the lock expires without the task being reclaimed
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);
//            // then the priority can be set
//            externalTaskService.SetPriority(externalTasks[0].Id, 9);

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID, true)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(1, externalTasks.Count);
//            Assert.AreEqual(externalTasks[0].Priority, 9);
//        }

//        [Deployment]
//        public virtual void testCancelExternalTaskWithBoundaryEvent()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("boundaryExternalTaskProcess");
//            Assert.AreEqual(1, externalTaskService.CreateExternalTaskQuery().Count());

//            // when the external task is cancelled by a boundary event
//            runtimeService.CorrelateMessage("Message");

//            // then the external task instance has been removed
//            Assert.AreEqual(0, externalTaskService.CreateExternalTaskQuery().Count());

//            ITask afterBoundaryTask = taskService.CreateTaskQuery().First();
//            Assert.NotNull(afterBoundaryTask);
//            Assert.AreEqual("afterBoundaryTask", afterBoundaryTask.TaskDefinitionKey);

//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnError()
//        {
//            //given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");
//            // when
//            IList<ILockedExternalTask> externalTasks = helperHandleBpmnError(1, WORKER_ID, TOPIC_NAME, LOCK_TIME, "ERROR-OCCURED");
//            //then
//            Assert.AreEqual(0, externalTasks.Count);
//            Assert.AreEqual(0, externalTaskService.CreateExternalTaskQuery().Count());
//            ITask afterBpmnError = taskService.CreateTaskQuery().First();
//            Assert.NotNull(afterBpmnError);
//            Assert.AreEqual(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorWithoutDefinedBoundary()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            //when
//            IList<ILockedExternalTask> externalTasks = helperHandleBpmnError(1, WORKER_ID, TOPIC_NAME, LOCK_TIME, "ERROR-OCCURED");

//            //then
//            Assert.AreEqual(0, externalTasks.Count);
//            Assert.AreEqual(0, externalTaskService.CreateExternalTaskQuery().Count());
//            ITask afterBpmnError = taskService.CreateTaskQuery().First();
//            Assert.IsNull(afterBpmnError);
//            AssertProcessEnded(processInstance.Id);
//        }

//        /// <summary>
//        /// Helper method to handle a bmpn error on an external task, which is fetched with the given parameters.
//        /// </summary>
//        /// <param name="taskCount"> the Count of task to fetch </param>
//        /// <param name="workerID"> the worker id </param>
//        /// <param name="topicName"> the topic name of the external task </param>
//        /// <param name="lockTime"> the lock time for the fetch </param>
//        /// <param name="errorCode"> the error code of the bpmn error </param>
//        /// <returns> returns the locked external tasks after the bpmn error was handled </returns>
//        public virtual IList<ILockedExternalTask> helperHandleBpmnError(int taskCount, string workerID, string topicName, long lockTime, string errorCode)
//        {
//            // when
//            //IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(taskCount, workerID).Topic(topicName, lockTime).Execute();

//            //externalTaskService.HandleBpmnError(externalTasks[0].Id, workerID, errorCode);

//            //externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            //return externalTasks;
//            return null;
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorLockExpiredTask()
//        {
//            //given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");
//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // and the lock expires without the task being reclaimed
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);
//            externalTaskService.HandleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-OCCURED");

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            Assert.AreEqual(0, externalTasks.Count);
//            Assert.AreEqual(0, externalTaskService.CreateExternalTaskQuery().Count());
//            ITask afterBpmnError = taskService.CreateTaskQuery().First();
//            Assert.NotNull(afterBpmnError);
//            Assert.AreEqual(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorReclaimedLockExpiredTaskWithoutDefinedBoundary()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            handleBpmnErrorReclaimedLockExpiredTask();
//            AssertProcessEnded(processInstance.Id);
//        }

//        [Deployment(new string[] { "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorReclaimedLockExpiredTaskWithBoundary()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");
//            //then
//            handleBpmnErrorReclaimedLockExpiredTask();
//        }

//        /// <summary>
//        /// Helpher method which reclaims an external task after the lock is expired.
//        /// </summary>
//        public virtual void handleBpmnErrorReclaimedLockExpiredTask()
//        {
//            // when
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // and the lock expires
//            //ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).ToDate();
//            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMilliseconds(LOCK_TIME * 2);

//            // and it is reclaimed by another worker
//            IList<ILockedExternalTask> reclaimedTasks = externalTaskService.FetchAndLock(1, "anotherWorkerId")/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            // then the first worker cannot complete the task
//            try
//            {
//                externalTaskService.HandleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-OCCURED");
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("Bpmn error of External ITask " + externalTasks[0].Id + " cannot be reported by worker '" + WORKER_ID + "'. It is locked by worker 'anotherWorkerId'.", e.Message);
//            }

//            // and the second worker can
//            externalTaskService.Complete(reclaimedTasks[0].Id, "anotherWorkerId");

//            externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
//            Assert.AreEqual(0, externalTasks.Count);
//        }

//        public virtual void testHandleBpmnErrorNonExistingTask()
//        {
//            try
//            {
//                externalTaskService.HandleBpmnError("nonExistingTaskId", WORKER_ID, "ERROR-OCCURED");
//                Assert.Fail("exception expected");
//            }
//            catch (NotFoundException e)
//            {
//                // not found exception lets client distinguish this from other failures
//                AssertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
//            }
//        }

//        public virtual void testHandleBpmnNullTaskId()
//        {
//            try
//            {
//                externalTaskService.HandleBpmnError(null, WORKER_ID, "ERROR-OCCURED");
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("Cannot find external task with id " + null, e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnNullErrorCode()
//        {
//            //given
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            //when
//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            //then
//            ILockedExternalTask task = tasks[0];
//            try
//            {
//                externalTaskService.HandleBpmnError(task.Id, WORKER_ID, null);
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("errorCode is null", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorNullWorkerId()
//        {
//            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

//            IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = tasks[0];

//            try
//            {
//                externalTaskService.HandleBpmnError(task.Id, null, "ERROR-OCCURED");
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("workerId is null", e.Message);
//            }
//        }

//        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
//        public virtual void testHandleBpmnErrorSuspendedTask()
//        {
//            // given
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
//            IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();

//            ILockedExternalTask task = externalTasks[0];

//            // when suspending the process instance
//            runtimeService.SuspendProcessInstanceById(processInstance.Id);

//            // then the external task cannot be completed
//            try
//            {
//                externalTaskService.HandleBpmnError(task.Id, WORKER_ID, "ERROR-OCCURED");
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("IExternalTask with id '" + task.Id + "' is suspended", e.Message);
//            }
//        }

//        protected internal virtual DateTime nowPlus(long millis)
//        {
//            //return new DateTime(ClockUtil.CurrentTime.Time + millis);
//            return new DateTime(ClockUtil.CurrentTime.Ticks + millis);
//        }

//        protected internal virtual DateTime nowMinus(long millis)
//        {
//            //return new DateTime(ClockUtil.CurrentTime.Time - millis);
//            return new DateTime(ClockUtil.CurrentTime.Ticks - millis);
//        }
//    }
//}