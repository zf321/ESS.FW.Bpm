using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.SubProcess.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SubProcess
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class SubProcessTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void TestSimpleSubProcess()
        {
            // After staring the process, the task in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess");
            ITask subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // we have 3 levels in the activityInstance:
            // pd
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("subProcess", subProcessInstance.ActivityId);
            // usertask
            Assert.AreEqual(1, subProcessInstance.ChildActivityInstances.Length);
            var userTaskInstance = subProcessInstance.ChildActivityInstances[0];
            Assert.AreEqual("subProcessTask", userTaskInstance.ActivityId);// .ActivityId);

            // After completing the task in the subprocess,
            // the subprocess scope is destroyed and the complete process ends
            taskService.Complete(subProcessTask.Id);
            Assert.IsNull(runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == pi.Id).FirstOrDefault());
        }

        /// <summary>
        /// Same test case as before, but now with all automatic steps
        /// </summary>

        [Test]
        [Deployment]
        public virtual void TestSimpleAutomaticSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("simpleSubProcessAutomatic");
            Assert.True(pi.IsEnded);
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void TestSimpleSubProcessWithTimer()
        {

            DateTime startTime = DateTime.Now;

            // After staring the process, the task in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess");
            ITask subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // we have 3 levels in the activityInstance:
            // pd
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("subProcess", subProcessInstance.ActivityId);
            // usertask
            Assert.AreEqual(1, subProcessInstance.ChildActivityInstances.Length);
            var userTaskInstance = subProcessInstance.ChildActivityInstances[0];
            Assert.AreEqual("subProcessTask", userTaskInstance.ActivityId);

            // Setting the clock forward 2 hours 1 second (timer fires in 2 hours) and fire up the job executor
            ClockUtil.CurrentTime = startTime.AddHours(2).AddSeconds(1);// new DateTime(startTime.Ticks + (2 * 60 * 60 * 1000) + 1000);
            WaitForJobExecutorToProcessAllJobs(5000L);

            // The subprocess should be left, and the escalated task should be active
            ITask escalationTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Fix escalated problem", escalationTask.Name);
        }

        /// <summary>
        /// A test case that has a timer attached to the subprocess,
        /// where 2 concurrent paths are defined when the timer fires.
        /// </summary>

        //[Test]
        [Deployment]
        public virtual void IGNORE_testSimpleSubProcessWithConcurrentTimer()
        {

            // After staring the process, the task in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("simpleSubProcessWithConcurrentTimer");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)/*.OrderByTaskName()*//*.Asc()*/;

            ITask subProcessTask = taskQuery.First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // When the timer is fired (after 2 hours), two concurrent paths should be created
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            IList<ITask> tasksAfterTimer = taskQuery.ToList().OrderBy(m=>m.Name).ToList();
            Assert.AreEqual(2, tasksAfterTimer.Count);
            ITask taskAfterTimer1 = tasksAfterTimer[0];
            ITask taskAfterTimer2 = tasksAfterTimer[1];
            Assert.AreEqual("Task after timer 1", taskAfterTimer1.Name);
            Assert.AreEqual("Task after timer 2", taskAfterTimer2.Name);

            // Completing the two tasks should end the process instance
            taskService.Complete(taskAfterTimer1.Id);
            taskService.Complete(taskAfterTimer2.Id);
            AssertProcessEnded(pi.Id);
        }

        /// <summary>
        /// Test case where the simple sub process of previous test cases
        /// is nested within another subprocess.
        /// </summary>

        [Test]
        [Deployment]
        public virtual void TestNestedSimpleSubProcess()
        {
            // Start and Delete a process with a nested subprocess when it is not yet ended
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nestedSimpleSubProcess", CollectionUtil.SingletonMap("someVar", "abc"));
            runtimeService.DeleteProcessInstance(pi.Id, "deleted");

            // After staring the process, the task in the inner subprocess must be active
            pi = runtimeService.StartProcessInstanceByKey("nestedSimpleSubProcess");
            ITask subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // now we have 4 levels in the activityInstance:
            // pd
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess1
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("outerSubProcess", subProcessInstance1.ActivityId);
            //subprocess2
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance2 = subProcessInstance1.ChildActivityInstances[0];
            Assert.AreEqual("innerSubProcess", subProcessInstance2.ActivityId);
            // usertask
            Assert.AreEqual(1, subProcessInstance2.ChildActivityInstances.Length);
            var userTaskInstance = subProcessInstance2.ChildActivityInstances[0];
            Assert.AreEqual("innerSubProcessTask", userTaskInstance.ActivityId);

            // After completing the task in the subprocess,
            // both subprocesses are destroyed and the task after the subprocess should be active
            taskService.Complete(subProcessTask.Id);
            ITask taskAfterSubProcesses = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(taskAfterSubProcesses);
            Assert.AreEqual("Task after subprocesses", taskAfterSubProcesses.Name);
            taskService.Complete(taskAfterSubProcesses.Id);
            AssertProcessEnded(pi.Id);
        }



        [Test]
        [Deployment]
        public virtual void TestNestedSimpleSubprocessWithTimerOnInnerSubProcess()
        {
            DateTime startTime = DateTime.Now;

            // After staring the process, the task in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nestedSubProcessWithTimer");
            ITask subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // now we have 4 levels in the activityInstance:
            // pd
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess1
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("outerSubProcess", subProcessInstance1.ActivityId);
            //subprocess2
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance2 = subProcessInstance1.ChildActivityInstances[0];
            Assert.AreEqual("innerSubProcess", subProcessInstance2.ActivityId);
            // usertask
            Assert.AreEqual(1, subProcessInstance2.ChildActivityInstances.Length);
            var userTaskInstance = subProcessInstance2.ChildActivityInstances[0];
            Assert.AreEqual("innerSubProcessTask", userTaskInstance.ActivityId);

            // Setting the clock forward 1 hour 1 second (timer fires in 1 hour) and fire up the job executor
            ClockUtil.CurrentTime = startTime.AddHours(1).AddSeconds(1);// new DateTime(startTime.Ticks + (60 * 60 * 1000) + 1000);
            WaitForJobExecutorToProcessAllJobs(5000L);

            // The inner subprocess should be destoyed, and the escalated task should be active
            ITask escalationTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Escalated task", escalationTask.Name);

            // now we have 3 levels in the activityInstance:
            // pd
            rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess1
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("outerSubProcess", subProcessInstance1.ActivityId);
            //subprocess2
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var escalationTaskInst = subProcessInstance1.ChildActivityInstances[0];
            Assert.AreEqual("escalationTask", escalationTaskInst.ActivityId);

            // Completing the escalated task, destroys the outer scope and activates the task after the subprocess
            taskService.Complete(escalationTask.Id);
            ITask taskAfterSubProcess = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task after subprocesses", taskAfterSubProcess.Name);
        }

        /// <summary>
        /// Test case where the simple sub process of previous test cases
        /// is nested within two other sub processes
        /// </summary>


        [Test]
        [Deployment]
        public virtual void TestDoubleNestedSimpleSubProcess()
        {
            // After staring the process, the task in the inner subprocess must be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nestedSimpleSubProcess");
            ITask subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            // After completing the task in the subprocess,
            // both subprocesses are destroyed and the task after the subprocess should be active
            taskService.Complete(subProcessTask.Id);
            ITask taskAfterSubProcesses = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task after subprocesses", taskAfterSubProcesses.Name);
        }



        [Test]
        [Deployment]
        public virtual void TestSimpleParallelSubProcess()
        {

            // After starting the process, the two task in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("simpleParallelSubProcess");
            IList<ITask> subProcessTasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)/*.OrderByTaskName()*//*.Asc()*/.ToList().OrderBy(m=>m.Name).ToList();

            // Tasks are ordered by name (see query)
            ITask taskA = subProcessTasks[0];
            ITask taskB = subProcessTasks[1];
            Assert.AreEqual("Task A", taskA.Name);
            Assert.AreEqual("Task B", taskB.Name);

            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            //subprocess1
            Assert.AreEqual(1, rootActivityInstance.ChildActivityInstances.Length);
            var subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
            Assert.AreEqual("subProcess", subProcessInstance.ActivityId);
            // 2 tasks are present
            Assert.AreEqual(2, subProcessInstance.ChildActivityInstances.Length);

            // Completing both tasks, should destroy the subprocess and activate the task after the subprocess
            taskService.Complete(taskA.Id);

            rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
            // 1 task + 1 join
            Assert.AreEqual(2, subProcessInstance.ChildActivityInstances.Length);

            taskService.Complete(taskB.Id);
            ITask taskAfterSubProcess = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Task after sub process", taskAfterSubProcess.Name);
        }



        [Test]
        [Deployment]
        public virtual void TestSimpleParallelSubProcessWithTimer()
        {

            // After staring the process, the tasks in the subprocess should be active
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("simpleParallelSubProcessWithTimer");
            IList<ITask> subProcessTasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)/*.OrderByTaskName()*//*.Asc()*/.ToList().OrderBy(m => m.Name).ToList();

            // Tasks are ordered by name (see query)
            ITask taskA = subProcessTasks[0];
            ITask taskB = subProcessTasks[1];
            Assert.AreEqual("Task A", taskA.Name);
            Assert.AreEqual("Task B", taskB.Name);

            IJob job = managementService.CreateJobQuery(c => c.ProcessInstanceId == processInstance.Id).First();

            managementService.ExecuteJob(job.Id);

            // The inner subprocess should be destoyed, and the tsk after the timer should be active
            ITask taskAfterTimer = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            Assert.AreEqual("Task after timer", taskAfterTimer.Name);

            // Completing the task after the timer ends the process instance
            taskService.Complete(taskAfterTimer.Id);
            AssertProcessEnded(processInstance.Id);
        }



        [Test]
        [Deployment]
        public virtual void TestTwoSubProcessInParallel()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("twoSubProcessInParallel");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)/*.OrderByTaskName()*//*.Asc()*/;
            IList<ITask> tasks = taskQuery.ToList().OrderBy(m=>m.Name).ToList();

            // After process start, both tasks in the subprocesses should be active
            Assert.AreEqual("Task in subprocess A", tasks[0].Name);
            Assert.AreEqual("Task in subprocess B", tasks[1].Name);

            // validate activity instance tree
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            Assert.AreEqual(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
            Assert.AreEqual(2, rootActivityInstance.ChildActivityInstances.Length);
            var childActivityInstances = rootActivityInstance.ChildActivityInstances;
            foreach (var activityInstance in childActivityInstances)
            {
                Assert.True((new string[] { "subProcessA", "subProcessB" }).Contains(activityInstance.ActivityId));
                var subProcessChildren = activityInstance.ChildActivityInstances;
                Assert.AreEqual(1, subProcessChildren.Length);
                Assert.True((new string[] { "subProcessATask", "subProcessBTask" }).Contains(subProcessChildren[0].ActivityId));
            }

            // Completing both tasks should active the tasks outside the subprocesses
            taskService.Complete(tasks[0].Id);

            tasks = taskQuery.ToList().OrderBy(m=>m.Name).ToList();
            Assert.AreEqual("Task after subprocess A", tasks[0].Name);
            Assert.AreEqual("Task in subprocess B", tasks[1].Name);

            taskService.Complete(tasks[1].Id);

            tasks = taskQuery.ToList();

            Assert.AreEqual("Task after subprocess A", tasks[0].Name);
            Assert.AreEqual("Task after subprocess B", tasks[1].Name);

            // Completing these tasks should end the process
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            AssertProcessEnded(pi.Id);
        }



        [Test]
        [Deployment]
        public virtual void TestTwoSubProcessInParallelWithinSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("twoSubProcessInParallelWithinSubProcess");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id);
            IList<ITask> tasks = taskQuery.ToList().OrderBy(m=>m.Name).ToList();

            // After process start, both tasks in the subprocesses should be active
            ITask taskA = tasks[0];
            ITask taskB = tasks[1];
            Assert.AreEqual("Task in subprocess A", taskA.Name);
            Assert.AreEqual("Task in subprocess B", taskB.Name);

            // validate activity instance tree
            var rootActivityInstance = runtimeService.GetActivityInstance(pi.ProcessInstanceId);
            ActivityInstanceAssert.That(rootActivityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(pi.ProcessDefinitionId).BeginScope("outerSubProcess").BeginScope("subProcessA").Activity("subProcessATask").EndScope().BeginScope("subProcessB").Activity("subProcessBTask").Done());
            ActivityInstanceAssert.That(rootActivityInstance)
                .Equals(ActivityInstanceAssert.DescribeActivityInstanceTree(pi.ProcessDefinitionId)
                .BeginScope("outerSubProcess").BeginScope("subProcessA").Activity("subProcessATask").EndScope().
                BeginScope("subProcessB").Activity("subProcessBTask").Done());

            // Completing both tasks should active the tasks outside the subprocesses
            taskService.Complete(taskA.Id);
            taskService.Complete(taskB.Id);

            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task should end the process
            taskService.Complete(taskAfterSubProcess.Id);
            AssertProcessEnded(pi.Id);
        }



        [Test]
        [Deployment]
        public virtual void TestTwoNestedSubProcessesInParallelWithTimer()
        {

            //    Date startTime = new Date();

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nestedParallelSubProcessesWithTimer");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)/*.OrderByTaskName()*//*.Asc()*/;
            IList<ITask> tasks = taskQuery.ToList().OrderBy(m=>m.Name).ToList();

            // After process start, both tasks in the subprocesses should be active
            ITask taskA = tasks[0];
            ITask taskB = tasks[1];
            Assert.AreEqual("Task in subprocess A", taskA.Name);
            Assert.AreEqual("Task in subprocess B", taskB.Name);

            // Firing the timer should destroy all three subprocesses and activate the task after the timer
            //    ClockUtil.SetCurrentTime(new Date(startTime.GetTime() + (2 * 60 * 60 * 1000 ) + 1000));
            //    waitForJobExecutorToProcessAllJobs(5000L, 50L);
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            ITask taskAfterTimer = taskQuery.First();
            Assert.AreEqual("Task after timer", taskAfterTimer.Name);

            // Completing the task should end the process instance
            taskService.Complete(taskAfterTimer.Id);
            AssertProcessEnded(pi.Id);
        }

        /// <seealso cref= http://jira.codehaus.org/browse/ACT-1072 </seealso>


        [Test]
        [Deployment]
        public virtual void TestNestedSimpleSubProcessWithoutEndEvent()
        {
            TestNestedSimpleSubProcess();
        }

        /// <seealso cref= http://jira.codehaus.org/browse/ACT-1072 </seealso>


        [Test]
        [Deployment]
        public virtual void TestSimpleSubProcessWithoutEndEvent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testSimpleSubProcessWithoutEndEvent");
            AssertProcessEnded(pi.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestNestedSubProcessesWithoutEndEvents()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testNestedSubProcessesWithoutEndEvents");
            AssertProcessEnded(pi.Id);
        }


        [Test]
        [Deployment]
        public virtual void TestActivityInstanceTreeNestedCmd()
        {
            // SEE https://app.Camunda.com/jira/browse/CAM-2169
            //
            //activityInstance = null;
            runtimeService.StartProcessInstanceByKey("process");

            var activityInstance = GetActInstanceDelegate.ActivityInstance;

            Assert.NotNull(activityInstance);
            var subProcessInstance = activityInstance.ChildActivityInstances[0];
            Assert.NotNull(subProcessInstance);
            Assert.AreEqual("SubProcess_1", subProcessInstance.ActivityId);

            var serviceTaskInstance = subProcessInstance.ChildActivityInstances[0];
            Assert.NotNull(serviceTaskInstance);
            Assert.AreEqual("ServiceTask_1", serviceTaskInstance.ActivityId);
        }


        [Test]
        [Deployment]
        public virtual void TestActivityInstanceTreeNestedCmdAfterTx()
        {
            // SEE https://app.Camunda.com/jira/browse/CAM-2169
            //
            GetActInstanceDelegate.ActivityInstance = null;
            runtimeService.StartProcessInstanceByKey("process");

            // send message
            runtimeService.CorrelateMessage("message");

            var activityInstance = GetActInstanceDelegate.ActivityInstance;

            Assert.NotNull(activityInstance);
            var subProcessInstance = activityInstance.ChildActivityInstances[0];
            Assert.NotNull(subProcessInstance);
            Assert.AreEqual("SubProcess_1", subProcessInstance.ActivityId);

            var serviceTaskInstance = subProcessInstance.ChildActivityInstances[0];
            Assert.NotNull(serviceTaskInstance);
            Assert.AreEqual("ServiceTask_1", serviceTaskInstance.ActivityId);
        }


        [Test]
        //[Deployment]
        public virtual void TestConcurrencyInSubProcess()
        {

            IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/subprocess/SubProcessTest.fixSystemFailureProcess.bpmn20.xml").Deploy();

            // After staring the process, both tasks in the subprocess should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("fixSystemFailure");
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)/*.OrderByTaskName()*//*.Asc()*/.ToList().OrderBy(m=>m.Name).ToList();

            // Tasks are ordered by name (see query)
            Assert.AreEqual(2, tasks.Count);
            ITask investigateHardwareTask = tasks[0];
            ITask investigateSoftwareTask = tasks[1];
            Assert.AreEqual("Investigate hardware", investigateHardwareTask.Name);
            Assert.AreEqual("Investigate software", investigateSoftwareTask.Name);

            // Completing both the tasks finishes the subprocess and enables the task after the subprocess
            taskService.Complete(investigateHardwareTask.Id);
            taskService.Complete(investigateSoftwareTask.Id);

            ITask writeReportTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.AreEqual("Write report", writeReportTask.Name);

            // Clean up
            repositoryService.DeleteDeployment(deployment.Id, true);
        }
    }

}