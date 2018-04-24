using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Gateway
{
    [TestFixture]
	public class ParallelGatewayTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testSplitMergeNoWaitstates()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("forkJoinNoWaitStates");
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testUnstructuredConcurrencyTwoForks()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("unstructuredConcurrencyTwoForks");
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testUnstructuredConcurrencyTwoJoins()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("unstructuredConcurrencyTwoJoins");
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testForkFollowedByOnlyEndEvents()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("forkFollowedByEndEvents");
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testNestedForksFollowedByEndEvents()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nestedForksFollowedByEndEvents");
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testNestedForkJoin()
        {
            string pid = runtimeService.StartProcessInstanceByKey("nestedForkJoin").Id;

            // After process startm, only task 0 should be active
            IQueryable<ITask> query = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/;
            IList<ITask> tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Task 0", tasks[0].Name);
            Assert.AreEqual(1, runtimeService.GetActivityInstance(pid).ChildActivityInstances.Length);

            // Completing task 0 will create ITask A and B
            taskService.Complete(tasks[0].Id);
            tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Task A", tasks[0].Name);
            Assert.AreEqual("Task B", tasks[1].Name);
            Assert.AreEqual(2, runtimeService.GetActivityInstance(pid).ChildActivityInstances.Length);

            // Completing task A should not trigger any new tasks
            taskService.Complete(tasks[0].Id);
            tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Task B", tasks[0].Name);
            Assert.AreEqual(2, runtimeService.GetActivityInstance(pid).ChildActivityInstances.Length);

            // Completing task B creates tasks B1 and B2
            taskService.Complete(tasks[0].Id);
            tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Task B1", tasks[0].Name);
            Assert.AreEqual("Task B2", tasks[1].Name);
            Assert.AreEqual(3, runtimeService.GetActivityInstance(pid).ChildActivityInstances.Length);

            // Completing B1 and B2 will activate both joins, and process reaches task C
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Task C", tasks[0].Name);
            Assert.AreEqual(1, runtimeService.GetActivityInstance(pid).ChildActivityInstances.Length);
        }

        [Test]
        [Deployment]
        public virtual void testReceyclingExecutionWithCallActivity()
        {
            var id = runtimeService.StartProcessInstanceByKey("parent-process").Id;

            // After process start we have two tasks, one from the parent and one from
            // the sub process
            IQueryable<ITask> query = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/;
            IList<ITask> tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Another task", tasks[0].Name);
            Assert.AreEqual("Some Task", tasks[1].Name);

            // we complete the task from the parent process, the root execution is
            // receycled, the task in the sub process is still there
            taskService.Complete(tasks[1].Id);
            tasks = query.ToList().OrderBy(c => c.Name).ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Another task", tasks[0].Name);

            // we end the task in the sub process and the sub process instance end is
            // propagated to the parent process
            taskService.Complete(tasks[0].Id);
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

            // There is a QA config without history, so we cannot work with this:
            // Assert.AreEqual(1,
            // historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId && c.EndTime !=null).Count());
        }

        [Test]
        [Deployment]
        public virtual void testCompletingJoin()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncParallelGateway()
        {
            // Todo: 在shutdown回收数据库功能没有稳定之前，需要手动删除TB_GOS_BPM_RU_JOBDEF表数据
            IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();
            Assert.NotNull(jobDefinition);
            Assert.AreEqual("parallelJoinEnd", jobDefinition.ActivityId);

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.IsFalse(processInstance.IsEnded);

            // there are two jobs to continue the gateway:
            // Todo: 在shutdown回收数据库功能没有稳定之前，需要手动删除TB_GOS_BPM_RU_JOB表数据
            IList<IJob> list = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, list.Count);

            managementService.ExecuteJob(list[0].Id);
            managementService.ExecuteJob(list[1].Id);

            // Todo: 在shutdown回收数据库功能没有稳定之前，需要手动删除TB_GOS_BPM_RU_EXECUTION表数据
            Assert.IsNull(runtimeService.CreateProcessInstanceQuery().FirstOrDefault());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncParallelGatewayAfterScopeTask()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.IsFalse(processInstance.IsEnded);

            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // there are two jobs to continue the gateway:
            IList<IJob> list = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, list.Count);

            managementService.ExecuteJob(list[0].Id);
            managementService.ExecuteJob(list[1].Id);

            Assert.IsNull(runtimeService.CreateProcessInstanceQuery().FirstOrDefault());
        }

        [Test]
        [Deployment]
        public virtual void testCompletingJoinInSubProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testParallelGatewayBeforeAndInSubProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            //Assert.That(tasks, hasSize(3));
            Assert.AreEqual(3, tasks.Count);

            IActivityInstance instance = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.That(instance.ActivityName, Is.EqualTo("Process1"));
            IActivityInstance[] childActivityInstances = instance.ChildActivityInstances;
            foreach (IActivityInstance activityInstance in childActivityInstances)
            {
                if (activityInstance.ActivityId.Equals("SubProcess_1"))
                {
                    IActivityInstance[] instances = activityInstance.ChildActivityInstances;
                    foreach (IActivityInstance activityInstance2 in instances)
                    {
                        //Assert.That(activityInstance2.ActivityName, Is.EqualTo(Either(equalTo("Inner IUser ITask 1")).or(CoreMatchers.EqualTo<object>("Inner IUser ITask 2"))));
                        Assert.That(activityInstance2.ActivityName, Is.EqualTo("Inner User Task 1").Or.EqualTo("Inner User Task 2"));
                    }
                }
                else
                {
                    Assert.That(activityInstance.ActivityName, Is.EqualTo("Outer User Task"));
                }
            }
        }

        [Test]
        [Deployment]
        public virtual void testForkJoin()
        {

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("forkJoin");
            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id)/*.OrderByTaskName()*//*.Asc()*/;

            IList<ITask> tasks = query.ToList().OrderBy(m => m.Name).ToList();
            Assert.AreEqual(2, tasks.Count);
            // the tasks are ordered by name (see above)
            ITask task1 = tasks[0];
            Assert.AreEqual("Receive Payment", task1.Name);
            ITask task2 = tasks[1];
            Assert.AreEqual("Ship Order", task2.Name);

            // Completing both tasks will join the concurrent executions
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            tasks = query.ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Archive Order", tasks[0].Name);
        }

        [Test]
        [Deployment]
        public virtual void testUnbalancedForkJoin()
        {

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("UnbalancedForkJoin");
            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id)/*.OrderByTaskName()*//*.Asc()*/;

            IList<ITask> tasks = query.ToList().OrderBy(c=>c.Name).ToList();
            Assert.AreEqual(3, tasks.Count);
            // the tasks are ordered by name (see above)
            ITask task1 = tasks[0];
            Assert.AreEqual("Task 1", task1.Name);
            ITask task2 = tasks[1];
            Assert.AreEqual("Task 2", task2.Name);

            // Completing the first task should *not* trigger the join
            taskService.Complete(task1.Id);

            // Completing the second task should trigger the first join
            taskService.Complete(task2.Id);

            tasks = query.ToList().OrderBy(c => c.Name).ToList();
            ITask task3 = tasks[0];
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("Task 3", task3.Name);
            ITask task4 = tasks[1];
            Assert.AreEqual("Task 4", task4.Name);

            // Completing the remaing tasks should trigger the second join and end the process
            taskService.Complete(task3.Id);
            taskService.Complete(task4.Id);

            AssertProcessEnded(pi.Id);
        }

        // Todo: EndEventBuilder.MoveToNode()
        [Test]
        [Deployment]
        public virtual void testRemoveConcurrentExecutionLocalVariablesOnJoin()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ParallelGateway("fork")
                .UserTask("task1").ParallelGateway("join").UserTask("afterTask").EndEvent().MoveToNode("fork").UserTask("task2").ConnectTo("join").Done());

            // given
            runtimeService.StartProcessInstanceByKey("process");

            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask task in tasks)
            {
                runtimeService.SetVariableLocal(task.ExecutionId, "var", "value");
            }

            // when
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery().Count());
        }
    }

}