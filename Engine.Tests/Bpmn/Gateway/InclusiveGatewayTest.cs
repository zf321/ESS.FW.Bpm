using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Gateway
{
    [TestFixture]
    public class InclusiveGatewayTest : PluggableProcessEngineTestCase
    {

        private const string TASK1_NAME = "Task 1";
        private const string TASK2_NAME = "Task 2";
        private const string TASK3_NAME = "Task 3";

        private const string BEAN_TASK1_NAME = "Basic service";
        private const string BEAN_TASK2_NAME = "Standard service";
        private const string BEAN_TASK3_NAME = "Gold Member service";

        [Test]
        [Deployment]
        public virtual void testDivergingInclusiveGateway()
        {
            for (int i = 1; i <= 3; i++)
            {
                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveGwDiverging", CollectionUtil.SingletonMap("input", i));
                IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
                IList<string> expectedNames = new List<string>();
                if (i == 1)
                {
                    expectedNames.Add(TASK1_NAME);
                }
                if (i <= 2)
                {
                    expectedNames.Add(TASK2_NAME);
                }
                expectedNames.Add(TASK3_NAME);
                foreach (ITask task in tasks)
                {
                    Console.WriteLine("task " + task.Name);
                }
                Assert.AreEqual(4 - i, tasks.Count);
                foreach (ITask task in tasks)
                {
                    expectedNames.Remove(task.Name);
                }
                Assert.AreEqual(0, expectedNames.Count);
                runtimeService.DeleteProcessInstance(pi.Id, "testing deletion");
            }
        }

        [Test]
        [Deployment]
        public virtual void testMergingInclusiveGateway()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveGwMerging", CollectionUtil.SingletonMap("input", 2));
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            runtimeService.DeleteProcessInstance(pi.Id, "testing deletion");
        }

        [Test]
        [Deployment]
        public virtual void testMergingInclusiveGatewayAsync()
        {
            // Todo: 在shutdown清理数据稳定之前，如果测试不通过需手动清理TB_GOS_BPM_RU_JOB表数据
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveGwMerging", CollectionUtil.SingletonMap("input", 2));
            IList<IJob> list = managementService.CreateJobQuery().ToList();
            foreach (IJob job in list)
            {
                managementService.ExecuteJob(job.Id);
            }
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            runtimeService.DeleteProcessInstance(pi.Id, "testing deletion");
        }

        [Test]
        [Deployment]
        public virtual void testPartialMergingInclusiveGateway()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("partialInclusiveGwMerging", CollectionUtil.SingletonMap("input", 2));
            ITask partialTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual("partialTask", partialTask.TaskDefinitionKey);

            taskService.Complete(partialTask.Id);

            ITask fullTask = taskService.CreateTaskQuery().First();
            Assert.AreEqual("theTask", fullTask.TaskDefinitionKey);

            runtimeService.DeleteProcessInstance(pi.Id, "testing deletion");
        }

        [Test]
        [Deployment]
        public virtual void testNoSequenceFlowSelected()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("inclusiveGwNoSeqFlowSelected", CollectionUtil.SingletonMap("input", 4));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-02004 No outgoing sequence flow for the element with id 'inclusiveGw' could be selected for continuing the process.", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testParentActivationOnNonJoiningEnd()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("parentActivationOnNonJoiningEnd");

            // Todo: 在shutdown清理数据稳定前，如果Test不通过需手动清理Ru_Execution表
            IList<IExecution> executionsBefore = runtimeService.CreateExecutionQuery().ToList();
            Assert.AreEqual(3, executionsBefore.Count);

            // start first round of tasks
            IList<ITask> firstTasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();

            Assert.AreEqual(2, firstTasks.Count);

            foreach (ITask t in firstTasks)
            {
                taskService.Complete(t.Id);
            }

            // start first round of tasks
            IList<ITask> secondTasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();

            Assert.AreEqual(2, secondTasks.Count);

            // complete one task
            ITask task = secondTasks[0];
            taskService.Complete(task.Id);

            // should have merged last child execution into parent
            IList<IExecution> executionsAfter = runtimeService.CreateExecutionQuery().ToList();
            Assert.AreEqual(1, executionsAfter.Count);

            IExecution execution = executionsAfter[0];

            // and should have one active activity
            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(execution.Id);
            Assert.AreEqual(1, activeActivityIds.Count);

            // Completing last task should finish the process instance

            ITask lastTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            taskService.Complete(lastTask.Id);

            Assert.AreEqual(0L, runtimeService.CreateProcessInstanceQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
        }

        [Test]
        [Deployment]
        public virtual void testWhitespaceInExpression()
        {
            // Starting a process instance will lead to an exception if whitespace are
            // incorrectly handled
            runtimeService.StartProcessInstanceByKey("inclusiveWhiteSpaceInExpression", CollectionUtil.SingletonMap("input", 1));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/InclusiveGatewayTest.TestDivergingInclusiveGateway.bpmn20.xml" })]
        public virtual void testUnknownVariableInExpression()
        {
            // Instead of 'input' we're starting a process instance with the name
            // 'iinput' (ie. a typo)
            try
            {
                runtimeService.StartProcessInstanceByKey("inclusiveGwDiverging", CollectionUtil.SingletonMap("iinput", 1));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Unknown property used in expression", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnBeanProperty()
        {
            runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnBeanProperty", CollectionUtil.SingletonMap("order", new InclusiveGatewayTestOrder(150)));
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(2, tasks.Count);
            IDictionary<string, string> expectedNames = new Dictionary<string, string>();
            expectedNames[BEAN_TASK2_NAME] = BEAN_TASK2_NAME;
            expectedNames[BEAN_TASK3_NAME] = BEAN_TASK3_NAME;
            foreach (ITask task in tasks)
            {
                expectedNames.Remove(task.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnListOrArrayOfBeans()
        {
            IList<InclusiveGatewayTestOrder> orders = new List<InclusiveGatewayTestOrder>();
            orders.Add(new InclusiveGatewayTestOrder(50));
            orders.Add(new InclusiveGatewayTestOrder(300));
            orders.Add(new InclusiveGatewayTestOrder(175));

            IProcessInstance pi = null;
            try
            {
                pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans",
                    CollectionUtil.SingletonMap("orders", orders));
                Assert.Fail();
            }
            catch (ProcessEngineException ex)
            {
                // expect an exception to be thrown here
            }

            orders[1] = new InclusiveGatewayTestOrder(175);
            pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.SingletonMap("orders", orders));
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual(BEAN_TASK3_NAME, task.Name);

            orders[1] = new InclusiveGatewayTestOrder(125);
            pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.SingletonMap("orders", orders));
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.NotNull(tasks);
            Assert.AreEqual(2, tasks.Count);
            IList<string> expectedNames = new List<string>();
            expectedNames.Add(BEAN_TASK2_NAME);
            expectedNames.Add(BEAN_TASK3_NAME);
            foreach (ITask t in tasks)
            {
                expectedNames.Remove(t.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);

            // Arrays are usable in exactly the same way
            InclusiveGatewayTestOrder[] orderArray = orders.ToArray();
            orderArray[1].Price = 10;
            pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.SingletonMap("orders", orderArray));
            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.NotNull(tasks);
            Assert.AreEqual(3, tasks.Count);
            expectedNames.Clear();
            expectedNames.Add(BEAN_TASK1_NAME);
            expectedNames.Add(BEAN_TASK2_NAME);
            expectedNames.Add(BEAN_TASK3_NAME);
            foreach (ITask t in tasks)
            {
                expectedNames.Remove(t.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnBeanMethod()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.SingletonMap("order", new InclusiveGatewayTestOrder(200)));
                
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual(BEAN_TASK3_NAME, task.Name);

            pi = runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.SingletonMap("order", new InclusiveGatewayTestOrder(125)));
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(2, tasks.Count);
            IList<string> expectedNames = new List<string>();
            expectedNames.Add(BEAN_TASK2_NAME);
            expectedNames.Add(BEAN_TASK3_NAME);
            foreach (ITask t in tasks)
            {
                expectedNames.Remove(t.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);

            try
            {
                runtimeService.StartProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.SingletonMap("order", new InclusiveGatewayTestOrder(300)));
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
                // Should get an exception indicating that no path could be taken
            }

        }

        [Test]
        [Deployment]
        public virtual void testInvalidMethodExpression()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("inclusiveInvalidMethodExpression", CollectionUtil.SingletonMap("order", new InclusiveGatewayTestOrder(50)));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Unknown method used in expression", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDefaultSequenceFlow()
        {
            // Input == 1 -> default is not selected, other 2 tasks are selected
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 1));
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(2, tasks.Count);
            IDictionary<string, string> expectedNames = new Dictionary<string, string>();
            expectedNames["Input is one"] = "Input is one";
            expectedNames["Input is three or one"] = "Input is three or one";
            foreach (ITask t in tasks)
            {
                expectedNames.Remove(t.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);
            runtimeService.DeleteProcessInstance(pi.Id, null);

            // Input == 3 -> default is not selected, "one or three" is selected
            pi = runtimeService.StartProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 3));
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Input is three or one", task.Name);

            // Default input
            pi = runtimeService.StartProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 5));
            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Default input", task.Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/InclusiveGatewayTest.TestDefaultSequenceFlow.bpmn20.xml" })]
        public virtual void testDefaultSequenceFlowExecutionIsActive()
        {
            // Todo:在ShutDown清理数据没稳定之前，如果用例不通过，需手动清理tb_gos_bpm_ru_execution表数据
            // given a triggered inclusive gateway default flow
            runtimeService.StartProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 5));

            // then the process instance execution is not deactivated
            ExecutionEntity execution = (ExecutionEntity)runtimeService.CreateExecutionQuery().First();
            Assert.AreEqual("theTask2", execution.ActivityId);
            Assert.True(execution.IsActive);
        }

        [Test]
        [Deployment]
        public virtual void testSplitMergeSplit()
        {
            // given a process instance with two concurrent tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("inclusiveGwSplitAndMerge", CollectionUtil.SingletonMap("input", 1));

            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(2, tasks.Count);

            // when the executions are joined at an inclusive gateway and the gateway itself has an outgoing default flow
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // then the task after the inclusive gateway is reached by the process instance execution (i.E. concurrent root)
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            Assert.AreEqual(processInstance.Id, task.ExecutionId);
        }


        [Test]
        [Deployment]
        public virtual void testNoIdOnSequenceFlow()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveNoIdOnSequenceFlow", CollectionUtil.SingletonMap("input", 3));
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Input is more than one", task.Name);

            // Both should be enabled on 1
            pi = runtimeService.StartProcessInstanceByKey("inclusiveNoIdOnSequenceFlow", CollectionUtil.SingletonMap("input", 1));
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(2, tasks.Count);
            IDictionary<string, string> expectedNames = new Dictionary<string, string>();
            expectedNames["Input is one"] = "Input is one";
            expectedNames["Input is more than one"] = "Input is more than one";
            foreach (ITask t in tasks)
            {
                expectedNames.Remove(t.Name);
            }
            Assert.AreEqual(0, expectedNames.Count);
        }

        
        [Test]
        [Deployment]
        public virtual void testLoop()
        {
            // Todo: groovy脚本不支持

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveTestLoop", CollectionUtil.SingletonMap("counter", 1));

            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task C", task.Name);

            taskService.Complete(task.Id);
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());


            foreach (IExecution execution in runtimeService.CreateExecutionQuery().ToList())
            {
                Console.WriteLine(((ExecutionEntity)execution).ActivityId);
            }
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count(), "Found executions: " + runtimeService.CreateExecutionQuery());

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testJoinAfterSubprocesses()
        {
            // Test case to test act-1204
            IDictionary<string, object> variableMap = new Dictionary<string, object>();
            variableMap["a"] = 1;
            variableMap["b"] = 1;
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("InclusiveGateway", variableMap);
            Assert.NotNull(processInstance.Id);

            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            taskService.Complete(tasks[0].Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            taskService.Complete(tasks[1].Id);

            ITask task = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == "c").FirstOrDefault();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            processInstance = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).FirstOrDefault();
            Assert.IsNull(processInstance);

            variableMap = new Dictionary<string, object>();
            variableMap["a"] = 1;
            variableMap["b"] = 2;
            processInstance = runtimeService.StartProcessInstanceByKey("InclusiveGateway", variableMap);
            Assert.NotNull(processInstance.Id);

            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            task = tasks[0];
            Assert.AreEqual("a", task.Assignee);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == "c").FirstOrDefault();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            processInstance = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).FirstOrDefault();
            Assert.IsNull(processInstance);

            variableMap = new Dictionary<string, object>();
            variableMap["a"] = 2;
            variableMap["b"] = 2;
            try
            {
                processInstance = runtimeService.StartProcessInstanceByKey("InclusiveGateway", variableMap);
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.Contains("No outgoing sequence flow"));
            }
        }

        
        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/InclusiveGatewayTest.TestJoinAfterCall.bpmn20.xml",
            "resources/bpmn/gateway/InclusiveGatewayTest.TestJoinAfterCallSubProcess.bpmn20.xml" })]
        public virtual void testJoinAfterCall()
        {
            // Test case to test act-1026
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("InclusiveGatewayAfterCall");
            Assert.NotNull(processInstance.Id);
            Assert.AreEqual(3, taskService.CreateTaskQuery().Count());

            // now complete task A and check number of remaining tasks.
            // inclusive gateway should wait for the "ITask B" and "ITask C"
            ITask taskA = taskService.CreateTaskQuery(c => c.NameWithoutCascade == "Task A").First();
            Assert.NotNull(taskA);
            taskService.Complete(taskA.Id);
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            // now complete task B and check number of remaining tasks
            // inclusive gateway should wait for "ITask C"
            ITask taskB = taskService.CreateTaskQuery(c => c.NameWithoutCascade == "Task B").First();
            Assert.NotNull(taskB);
            taskService.Complete(taskB.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            // now complete task C. Gateway activates and "ITask C" remains
            ITask taskC = taskService.CreateTaskQuery(c => c.NameWithoutCascade == "Task C").First();
            Assert.NotNull(taskC);
            taskService.Complete(taskC.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            // check that remaining task is in fact task D
            ITask taskD = taskService.CreateTaskQuery(c => c.NameWithoutCascade == "Task D").First();
            Assert.NotNull(taskD);
            Assert.AreEqual("Task D", taskD.Name);
            taskService.Complete(taskD.Id);

            processInstance = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).FirstOrDefault();
            Assert.IsNull(processInstance);
        }

        [Test]
        [Deployment]
        public virtual void testDecisionFunctionality()
        {

            IDictionary<string, object> variables = new Dictionary<string, object>();

            // Test with input == 1
            variables["input"] = 1;
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("inclusiveGateway", variables);
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(3, tasks.Count);
            IDictionary<string, string> expectedMessages = new Dictionary<string, string>();
            expectedMessages[TASK1_NAME] = TASK1_NAME;
            expectedMessages[TASK2_NAME] = TASK2_NAME;
            expectedMessages[TASK3_NAME] = TASK3_NAME;
            foreach (ITask task in tasks)
            {
                expectedMessages.Remove(task.Name);
            }
            Assert.AreEqual(0, expectedMessages.Count);

            // Test with input == 2
            variables["input"] = 2;
            pi = runtimeService.StartProcessInstanceByKey("inclusiveGateway", variables);
            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(2, tasks.Count);
            expectedMessages = new Dictionary<string, string>();
            expectedMessages[TASK2_NAME] = TASK2_NAME;
            expectedMessages[TASK3_NAME] = TASK3_NAME;
            foreach (ITask task in tasks)
            {
                expectedMessages.Remove(task.Name);
            }
            Assert.AreEqual(0, expectedMessages.Count);

            // Test with input == 3
            variables["input"] = 3;
            pi = runtimeService.StartProcessInstanceByKey("inclusiveGateway", variables);
            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).ToList();
            Assert.AreEqual(1, tasks.Count);
            expectedMessages = new Dictionary<string, string>();
            expectedMessages[TASK3_NAME] = TASK3_NAME;
            foreach (ITask task in tasks)
            {
                expectedMessages.Remove(task.Name);
            }
            Assert.AreEqual(0, expectedMessages.Count);

            // Test with input == 4
            variables["input"] = 4;
            try
            {
                runtimeService.StartProcessInstanceByKey("inclusiveGateway", variables);
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
                // Exception is expected since no outgoing sequence flow matches
            }

        }

        [Test]
        [Deployment]
        public virtual void testJoinAfterSequentialMultiInstanceSubProcess()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // when
            ITask task = query.Where(c => c.TaskDefinitionKey == "task").First();
            taskService.Complete(task.Id);

            // then
            Assert.IsNull(query.Where(c => c.TaskDefinitionKey == "taskAfterJoin").FirstOrDefault());
        }

        [Test]
        [Deployment]
        public virtual void testJoinAfterParallelMultiInstanceSubProcess()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // when
            ITask task = query.Where(c => c.TaskDefinitionKey == "task").First();
            taskService.Complete(task.Id);

            // then
            Assert.IsNull(query.Where(c => c.TaskDefinitionKey == "taskAfterJoin").FirstOrDefault());
        }

        [Test]
        [Deployment]
        public virtual void testJoinAfterNestedScopes()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<ITask> query = taskService.CreateTaskQuery();

            // when
            ITask task = query.Where(c => c.TaskDefinitionKey == "task").First();
            taskService.Complete(task.Id);

            // then
            Assert.IsNull(query.Where(c => c.TaskDefinitionKey == "taskAfterJoin").FirstOrDefault());
        }

        public virtual void testTriggerGatewayWithEnoughArrivedTokens()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask("beforeTask").InclusiveGateway("gw").UserTask("afterTask").EndEvent().Done());

            // given
            IProcessInstance processInstance = runtimeService.CreateProcessInstanceByKey("process").StartBeforeActivity("beforeTask").StartBeforeActivity("beforeTask").Execute();

            ITask task = taskService.CreateTaskQuery().First();

            // when
            taskService.Complete(task.Id);

            // then
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .Activity("beforeTask").Activity("afterTask").Done());

        }

        [Test]
        [Deployment]
        public virtual void testLoopingInclusiveGateways()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // when
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);

            // then
            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .Activity("task1").Activity("task2").Activity("inclusiveGw3").Done());
        }

        // Todo: EndEventBuilder.MoveToNode()
        //public virtual void testRemoveConcurrentExecutionLocalVariablesOnJoin()
        //{
        //    Deployment(Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().InclusiveGateway("fork").UserTask("task1").InclusiveGateway("join").UserTask("afterTask")
        //        .EndEvent().MoveToNode("fork").UserTask("task2").ConnectTo("join").Done());

        //    // given
        //    runtimeService.StartProcessInstanceByKey("process");

        //    IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
        //    foreach (ITask task in tasks)
        //    {
        //        runtimeService.SetVariableLocal(task.ExecutionId, "var", "value");
        //    }

        //    // when
        //    taskService.Complete(tasks[0].Id);
        //    taskService.Complete(tasks[1].Id);

        //    // then
        //    Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery().Count());
        //}

    }

}