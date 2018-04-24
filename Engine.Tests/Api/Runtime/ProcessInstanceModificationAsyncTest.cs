using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationAsyncTest : PluggableProcessEngineTestCase
    {
        protected internal const string EXCLUSIVE_GATEWAY_ASYNC_BEFORE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGatewayAsyncTask.bpmn20.xml";

        protected internal const string ASYNC_BEFORE_ONE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.asyncBeforeOneTaskProcess.bpmn20.xml";

        protected internal const string ASYNC_BEFORE_ONE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.asyncBeforeOneScopeTaskProcess.bpmn20.xml";

        protected internal const string NESTED_ASYNC_BEFORE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncBeforeOneTaskProcess.bpmn20.xml";

        protected internal const string NESTED_ASYNC_BEFORE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncBeforeOneScopeTaskProcess.bpmn20.xml";

        protected internal const string NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncBeforeConcurrentScopeTaskProcess.bpmn20.xml";

        protected internal const string NESTED_ASYNC_BEFORE_IO_LISTENER_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncBeforeOneTaskProcessIoAndListeners.bpmn20.xml";

        protected internal const string ASYNC_AFTER_ONE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.asyncAfterOneTaskProcess.bpmn20.xml";

        protected internal const string NESTED_ASYNC_AFTER_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncAfterOneTaskProcess.bpmn20.xml";

        protected internal const string NESTED_ASYNC_AFTER_END_EVENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelAsyncAfterEndEventProcess.bpmn20.xml";

        protected internal const string ASYNC_AFTER_FAILING_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.asyncAfterFailingTaskProcess.bpmn20.xml";

        protected internal const string ASYNC_BEFORE_FAILING_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.asyncBeforeFailingTaskProcess.bpmn20.xml";
        [Test][Deployment(EXCLUSIVE_GATEWAY_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testStartBeforeAsync()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .Execute();

            // the task does not yet exist because it is started asynchronously
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.IsNull(task);

            // and there is no activity instance for task2 yet
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Transition("task2")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // when the async job is executed
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            ExecuteAvailableJobs();

            // then there is the task
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.NotNull(task);

            // and there is an activity instance for task2
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());

            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        /// <summary>
        ///     starting after a task should not respect that tasks asyncAfter setting
        /// </summary>
        [Test]
        [Deployment]
        public virtual void testStartAfterAsync()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("task2")
                .Execute();

            // there is now a job for the end event after task2
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            var jobExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "end2" && c.Id == job.ExecutionId)
                .First();
            Assert.NotNull(jobExecution);

            // end process
            completeTasksInOrder("task1");
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(ProcessInstanceId);
        }
        [Test][Deployment( NESTED_ASYNC_BEFORE_TASK_PROCESS) ]
        public virtual void testCancelParentScopeOfAsyncBeforeActivity()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // when I cancel the subprocess
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            // then the process instance is in a valid state
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_SCOPE_TASK_PROCESS) ]
        public virtual void testCancelParentScopeOfAsyncBeforeScopeActivity()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // when I cancel the subprocess
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            // then the process instance is in a valid state
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testCancelParentScopeOfParallelAsyncBeforeScopeActivity()
        {
            // given a process instance with two concurrent async scope tasks in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedConcurrentTasksProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // when I cancel the subprocess
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            // then the process instance is in a valid state
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelAsyncActivityInstanceFails()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // the the async task is not an activity instance so it cannot be cancelled as follows
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .CancelActivityInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask")
                        .Id)
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("activityInstance is null", e.Message);
            }
        }

        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelAsyncBeforeTransitionInstance()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // when the async task is cancelled via cancelTransitionInstance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask")
                    .Id)
                .Execute();

            // then the job has been removed
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());

            // and the activity instance and execution trees match
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // and the process can be completed successfully
            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(ASYNC_BEFORE_ONE_TASK_PROCESS)]
        public virtual void testCancelAsyncBeforeTransitionInstanceEndsProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask")
                    .Id)
                .Execute();

            // then the process instance has ended
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( ASYNC_BEFORE_ONE_SCOPE_TASK_PROCESS) ]
        public virtual void testCancelAsyncBeforeScopeTransitionInstanceEndsProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask")
                    .Id)
                .Execute();

            // then the process instance has ended
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ASYNC_BEFORE_ONE_TASK_PROCESS) ]
        public virtual void testCancelAndStartAsyncBeforeTransitionInstance()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            var asyncJob = managementService.CreateJobQuery()
                .First();

            // when cancelling the only transition instance in the process and immediately starting it again
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask")
                    .Id)
                .StartBeforeActivity("theTask")
                .Execute();

            // then the activity instance tree should be as before
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("theTask")
                    .Done());

            // and the async job should be a new one
            var newAsyncJob = managementService.CreateJobQuery()
                .First();
            Assert.IsFalse(asyncJob.Id.Equals(newAsyncJob.Id));

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("theTask")
                    .Scope()
                    .Done());

            // and the process can be completed successfully
            ExecuteAvailableJobs();
            completeTasksInOrder("theTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testCancelNestedConcurrentTransitionInstance()
        {
            // given a process instance with an instance of outerTask and two asynchronous tasks nested
            // in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedConcurrentTasksProcess");
            var ProcessInstanceId = processInstance.Id;

            // when one of the inner transition instances is cancelled
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask1")
                    .Id)
                .Execute();

            // then the activity instance and execution trees should match
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Transition("innerTask2")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // and the job for innerTask2 should still be there and assigned to the correct execution
            var innerTask2Job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(innerTask2Job);

            var innerTask2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask2")
                .First();
            Assert.NotNull(innerTask2Execution);

            Assert.AreEqual(innerTask2Job.ExecutionId, innerTask2Execution.Id);

            // and completing the process should succeed
            completeTasksInOrder("outerTask");
            managementService.ExecuteJob(innerTask2Job.Id);
            completeTasksInOrder("innerTask2");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testCancelNestedConcurrentTransitionInstanceWithConcurrentScopeTask()
        {
            // given a process instance where the job for innerTask2 is already executed
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedConcurrentTasksProcess");
            var ProcessInstanceId = processInstance.Id;

            var innerTask2Job = managementService.CreateJobQuery(c=> c.ActivityId =="innerTask2")
                .First();
            Assert.NotNull(innerTask2Job);
            managementService.ExecuteJob(innerTask2Job.Id);

            // when the transition instance to innerTask1 is cancelled
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask1")
                    .Id)
                .Execute();

            // then the activity instance and execution tree should match
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // and there should be no job for innerTask1 anymore
            Assert.AreEqual(0, managementService.CreateJobQuery(c=> c.ActivityId =="innerTask1")
                .Count());

            // and completing the process should succeed
            completeTasksInOrder("innerTask2", "outerTask");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test][Deployment( NESTED_ASYNC_BEFORE_IO_LISTENER_PROCESS) ]
        public virtual void testCancelTransitionInstanceShouldNotInvokeIoMappingAndListenersOfTargetActivity()
        {
            RecorderExecutionListener.Clear();

            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("listener", new RecorderExecutionListener()));

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // when the async task is cancelled via cancelTransitionInstance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask")
                    .Id)
                .Execute();

            // then no io mapping is executed and no end listener is executed
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(c=>c.Name == "outputMappingExecuted")
                .Count());

            // and the process can be completed successfully
            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_ASYNC_AFTER_TASK_PROCESS) ]
        public virtual void testCancelAsyncAfterTransitionInstance()
        {
            // given a process instance with an asyncAfter task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            var innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerTask")
                .First();
            Assert.NotNull(innerTask);
            taskService.Complete(innerTask.Id);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // when the async task is cancelled via cancelTransitionInstance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask")
                    .Id)
                .Execute();

            // then the job has been removed
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());

            // and the activity instance and execution trees match
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // and the process can be completed successfully
            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment( NESTED_ASYNC_AFTER_END_EVENT_PROCESS)]
        public virtual void testCancelAsyncAfterEndEventTransitionInstance()
        {
            // given a process instance with an asyncAfter end event in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedAsyncEndEventProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // when the async task is cancelled via cancelTransitionInstance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "subProcessEnd")
                    .Id)
                .Execute();

            // then the job has been removed
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());

            // and the activity instance and execution trees match
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // and the process can be completed successfully
            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(ASYNC_AFTER_ONE_TASK_PROCESS)]
        public virtual void testCancelAsyncAfterTransitionInstanceEndsProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask")
                    .Id)
                .Execute();

            // then the process instance has ended
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCancelAsyncAfterTransitionInstanceInvokesParentListeners()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("listener", new RecorderExecutionListener()));
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "subProcessEnd")
                    .Id)
                .Execute();

            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);
            var @event = RecorderExecutionListener.RecordedEvents[0];
            Assert.AreEqual("subProcess", @event.ActivityId);

            RecorderExecutionListener.Clear();
        }

        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelAllCancelsTransitionInstances()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");

            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // when the async task is cancelled via cancelAll
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("innerTask")
                .Execute();

            // then the job has been removed
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());

            // and the activity instance and execution trees match
            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // and the process can be completed successfully
            completeTasksInOrder("outerTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(ASYNC_AFTER_FAILING_TASK_PROCESS)]
        public virtual void testStartBeforeAsyncAfterTask()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("failingAfterAsyncTask");

            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task1")
                .Execute();

            // then there are two transition instances of task1
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("task1")
                    .Transition("task1")
                    .Done());

            // when all jobs are executed
            ExecuteAvailableJobs();

            // then the tree is still the same, since the jobs failed
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("task1")
                    .Transition("task1")
                    .Done());
        }

        [Test]
        [Deployment(ASYNC_AFTER_FAILING_TASK_PROCESS)]
        public virtual void testStartBeforeAsyncAfterTaskActivityStatistics()
        {
            // given a process instance with an async task in a subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("failingAfterAsyncTask");

            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            // there is one statistics instance
            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);
            Assert.AreEqual("task1", statistics[0].Id);
            Assert.AreEqual(0, statistics[0].FailedJobs);
            Assert.AreEqual(0, statistics[0].IncidentStatistics.Count);
            Assert.AreEqual(1, statistics[0].Instances);

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task1")
                .Execute();

            // then there are statistics instances of task1
            statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                /*.IncludeFailedJobs()*/
                /*.IncludeIncidents()*/
                
                .ToList();

            Assert.AreEqual(1, statistics.Count);
            Assert.AreEqual("task1", statistics[0].Id);
            Assert.AreEqual(0, statistics[0].FailedJobs);
            Assert.AreEqual(0, statistics[0].IncidentStatistics.Count);
            Assert.AreEqual(2, statistics[0].Instances);


            // when all jobs are executed
            ExecuteAvailableJobs();
        }


        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelAllTransitionInstanceInScope()
        {
            // given there are two transition instances in an inner scope
            // and an active activity instance in an outer scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedOneTaskProcess")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("outerTask")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);

            // when i cancel both transition instances
            var transitionInstances = tree.GetTransitionInstances("innerTask");

            runtimeService.CreateProcessInstanceModification(instance.Id)
                .CancelTransitionInstance(transitionInstances[0].Id)
                .CancelTransitionInstance(transitionInstances[1].Id)
                .Execute();

            // then the outer activity instance is the only one remaining
            tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelTransitionInstanceTwiceFails()
        {
            // given there are two transition instances in an inner scope
            // and an active activity instance in an outer scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedOneTaskProcess")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("innerTask")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);

            // when i cancel both transition instances
            var transitionInstances = tree.GetTransitionInstances("innerTask");

            // this test ensures that the replacedBy link of executions is not followed
            // in case the original execution was actually removed/cancelled
            var transitionInstanceId = transitionInstances[0].Id;
            try
            {
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .CancelTransitionInstance(transitionInstanceId)
                    .CancelTransitionInstance(transitionInstanceId)
                    .Execute();
                Assert.Fail("should not be possible to cancel the first instance twice");
            }
            catch (NotValidException e)
            {
                AssertTextPresentIgnoreCase(
                    "Cannot perform instruction: Cancel transition instance '" + transitionInstanceId +
                    "'; Transition instance '" + transitionInstanceId + "' does not exist: transitionInstance is null",
                    e.Message);
            }
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_ASYNC_BEFORE_TASK_PROCESS)]
        public virtual void testCancelTransitionInstanceTwiceFailsCase2()
        {
            // given there are two transition instances in an inner scope
            // and an active activity instance in an outer scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedOneTaskProcess")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("innerTask")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);

            // when i cancel both transition instances
            var transitionInstances = tree.GetTransitionInstances("innerTask");

            // this test ensures that the replacedBy link of executions is not followed
            // in case the original execution was actually removed/cancelled

            try
            {
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .CancelTransitionInstance(transitionInstances[0].Id)
                    .StartBeforeActivity("innerTask")
                    .StartBeforeActivity("innerTask")
                    .CancelTransitionInstance(transitionInstances[1].Id)
                    .CancelTransitionInstance(transitionInstances[1].Id)
                    .Execute();
                // should Assert.Fail -  does not trigger compaction -  expand tree again -  compacts the tree;
                // => execution for transitionInstances[1] is replaced by scope execution
                // => scope execution is replaced by a new concurrent execution
                // => execution for transitionInstances[1] should no longer have a replacedBy link
                Assert.Fail("should not be possible to cancel the first instance twice");
            }
            catch (NotValidException e)
            {
                var transitionInstanceId = transitionInstances[1].Id;
                AssertTextPresentIgnoreCase(
                    "Cannot perform instruction: Cancel transition instance '" + transitionInstanceId +
                    "'; Transition instance '" + transitionInstanceId + "' does not exist: transitionInstance is null",
                    e.Message);
            }
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testCancelStartCancelInScope()
        {
            // given there are two transition instances in an inner scope
            // and an active activity instance in an outer scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("innerTask1")
                .StartBeforeActivity("innerTask1")
                .StartBeforeActivity("outerTask")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);

            // when i cancel both transition instances
            var transitionInstances = tree.GetTransitionInstances("innerTask1");

            runtimeService.CreateProcessInstanceModification(instance.Id)
                .CancelTransitionInstance(transitionInstances[0].Id)
                .StartBeforeActivity("innerTask2")
                .CancelTransitionInstance(transitionInstances[1].Id)
                .Execute(); // triggers tree expansion -  triggers tree compaction

            // then the outer activity instance is the only one remaining
            tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Transition("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testStartAndCancelAllForTransitionInstance()
        {
            // given there is one transition instance in a scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("innerTask1")
                .StartBeforeActivity("innerTask1")
                .StartBeforeActivity("innerTask1")
                .Execute();

            // when I start an activity in the same scope
            // and cancel the first transition instance
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask1")
                .Execute();

            // then the activity was successfully instantiated
            var tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Transition("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testRepeatedStartAndCancellationForTransitionInstance()
        {
            // given there is one transition instance in a scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("innerTask1")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);
            var transitionInstance = tree.GetTransitionInstances("innerTask1")[0];

            // when I start an activity in the same scope
            // and cancel the first transition instance
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask2")
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask2")
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask2")
                .CancelTransitionInstance(transitionInstance.Id)
                .Execute(); // compact tree -  expand tree -  compact tree -  expand tree -  compact tree -  expand tree

            // then the process has ended
            AssertProcessEnded(instance.Id);
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testRepeatedCancellationAndStartForTransitionInstance()
        {
            // given there is one transition instance in a scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("innerTask1")
                .StartBeforeActivity("innerTask1")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);
            var transitionInstances = tree.GetTransitionInstances("innerTask1");

            // when I start an activity in the same scope
            // and cancel the first transition instance
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .CancelTransitionInstance(transitionInstances[0].Id)
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask2")
                .StartBeforeActivity("innerTask2")
                .CancelAllForActivity("innerTask2")
                .StartBeforeActivity("innerTask2")
                .CancelTransitionInstance(transitionInstances[1].Id)
                .Execute(); // expand tree -  compact tree -  expand tree -  compact tree -  expand tree -  compact tree

            // then there is only an activity instance for innerTask2
            tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Transition("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testStartBeforeAndCancelSingleTransitionInstance()
        {
            // given there is one transition instance in a scope
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("innerTask1")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);
            var transitionInstance = tree.GetTransitionInstances("innerTask1")[0];

            // when I start an activity in the same scope
            // and cancel the first transition instance
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("innerTask2")
                .CancelTransitionInstance(transitionInstance.Id)
                .Execute();

            // then the activity was successfully instantiated
            tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Transition("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());
        }

        /// <summary>
        ///     CAM-4090
        /// </summary>
        [Test]
        [Deployment(NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
        public virtual void testStartBeforeSyncEndAndCancelSingleTransitionInstance()
        {
            // given there is one transition instance in a scope and an outer activity instance
            var instance = runtimeService.CreateProcessInstanceByKey("nestedConcurrentTasksProcess")
                .StartBeforeActivity("outerTask")
                .StartBeforeActivity("innerTask1")
                .Execute();

            var tree = runtimeService.GetActivityInstance(instance.Id);
            var transitionInstance = tree.GetTransitionInstances("innerTask1")[0];

            // when I start an activity in the same scope that ends immediately
            // and cancel the first transition instance
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("subProcessEnd2")
                .CancelTransitionInstance(transitionInstance.Id)
                .Execute();

            // then only the outer activity instance is left
            tree = runtimeService.GetActivityInstance(instance.Id);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());
        }

        [Test]
        [Deployment(ASYNC_BEFORE_FAILING_TASK_PROCESS)]
        public virtual void testRestartAFailedServiceTask()
        {
            // given a failed job
            var instance = runtimeService.CreateProcessInstanceByKey("failingAfterBeforeTask")
                .StartBeforeActivity("task2")
                .Execute();

            ExecuteAvailableJobs();
            var incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);

            // when the service task is restarted
            var tree = runtimeService.GetActivityInstance(instance.Id);
            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartBeforeActivity("task2")
                .CancelTransitionInstance(tree.GetTransitionInstances("task2")[0].Id)
                .Execute();

            ExecuteAvailableJobs();

            // then executing the task has failed again and there is a new incident
            var newIncident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(newIncident);

            Assert.AreNotSame(incident.Id, newIncident.Id);
        }

        protected internal virtual string getInstanceIdForActivity(IActivityInstance activityInstance, string activityId)
        {
            var instance = getChildInstanceForActivity(activityInstance, activityId);
            if (instance != null)
                return instance.Id;
            return null;
        }

        protected internal virtual IActivityInstance getChildInstanceForActivity(IActivityInstance activityInstance,
            string activityId)
        {
            if (activityId.Equals(activityInstance.ActivityId))
                return activityInstance;

            foreach (var childInstance in activityInstance.ChildActivityInstances)
            {
                var instance = getChildInstanceForActivity(childInstance, activityId);
                if (instance != null)
                    return instance;
            }

            return null;
        }

        protected internal virtual ITransitionInstance getChildTransitionInstanceForTargetActivity(
            IActivityInstance activityInstance, string targetActivityId)
        {
            foreach (var childTransitionInstance in activityInstance.ChildTransitionInstances)
                if (targetActivityId.Equals(childTransitionInstance.ActivityId))
                    return childTransitionInstance;

            foreach (var childInstance in activityInstance.ChildActivityInstances)
            {
                var instance = getChildTransitionInstanceForTargetActivity(childInstance, targetActivityId);
                if (instance != null)
                    return instance;
            }

            return null;
        }

        protected internal virtual void completeTasksInOrder(params string[] taskNames)
        {
            foreach (var taskName in taskNames)
            {
                // complete any task with that name
                var tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey == taskName)
                    /*.ListPage(0, 1)*/
                    .ToList();
                Assert.True(tasks.Count > 0, "task for activity " + taskName + " does not exist");
                taskService.Complete(tasks[0].Id);
            }
        }
    }
}