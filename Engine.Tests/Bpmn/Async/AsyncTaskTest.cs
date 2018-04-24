using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{
    [TestFixture]
    public class AsyncTaskTest : PluggableProcessEngineTestCase
    {
        public static bool INVOCATION;
        public static int NUM_INVOCATIONS = 0;

        [Test]
        [Deployment]
        public virtual void testAsyncServiceNoListeners()
        {
            INVOCATION = false;
            // start process
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("asyncService");

            // now we have one transition instance below the process instance:
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(1, activityInstance.ChildTransitionInstances.Length);
            Assert.AreEqual(0, activityInstance.ChildActivityInstances.Length);

            Assert.NotNull(activityInstance.ChildTransitionInstances[0]);

            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the service was not invoked:
            Assert.IsFalse(INVOCATION);

            ExecuteAvailableJobs();

            // the service was invoked
            Assert.True(INVOCATION);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceListeners()
        {
            string pid = runtimeService.StartProcessInstanceByKey("asyncService").ProcessInstanceId;
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not yet invoked:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));

            ExecuteAvailableJobs();

            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceConcurrent()
        {
            INVOCATION = false;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");
            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the service was not invoked:
            Assert.IsFalse(INVOCATION);

            ExecuteAvailableJobs();

            // the service was invoked
            Assert.True(INVOCATION);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncSequentialMultiInstanceWithServiceTask()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for the multi-instance body to execute:
            ExecuteAvailableJobs(1);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncParallelMultiInstanceWithServiceTask()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for the multi-instance body to execute:
            ExecuteAvailableJobs(1);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceWrappedInSequentialMultiInstance()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for the first service task wrapped in the multi-instance body:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // execute all jobs - one for each service task:
            ExecuteAvailableJobs(5);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceWrappedInParallelMultiInstance()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for each service task wrapped in the multi-instance body:
            Assert.AreEqual(5, managementService.CreateJobQuery().Count());
            // execute all jobs:
            ExecuteAvailableJobs(5);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncBeforeAndAfterOfServiceWrappedInParallelMultiInstance()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for each service task wrapped in the multi-instance body:
            Assert.AreEqual(5, managementService.CreateJobQuery().Count());
            // execute all jobs - one for asyncBefore and another for asyncAfter:
            ExecuteAvailableJobs(5 + 5);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncBeforeSequentialMultiInstanceWithAsyncAfterServiceWrappedInMultiInstance()
        {
            NUM_INVOCATIONS = 0;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for the multi-instance body:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // execute all jobs - one for multi-instance body and one for each service task wrapped in the multi-instance body:
            ExecuteAvailableJobs(1 + 5);

            // the service was invoked
            Assert.AreEqual(5, NUM_INVOCATIONS);
            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        protected internal virtual void AssertTransitionInstances(string ProcessInstanceId, string activityId, int numInstances)
        {
            IActivityInstance tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            Assert.AreEqual(numInstances, tree.GetTransitionInstances(activityId).Length);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncBeforeAndAfterParallelMultiInstanceWithAsyncBeforeAndAfterServiceWrappedInMultiInstance()
        {
            NUM_INVOCATIONS = 0;
            // start process
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("asyncService");

            // the service was not invoked:
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // now there should be one job for the multi-instance body:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            AssertTransitionInstances(processInstance.Id, "service" + BpmnParse.MultiInstanceBodyIdSuffix, 1);

            // when the mi body before job is executed
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            // then there are five inner async before jobs
            IList<IJob> innerBeforeJobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(5, innerBeforeJobs.Count);
            AssertTransitionInstances(processInstance.Id, "service", 5);
            Assert.AreEqual(0, NUM_INVOCATIONS);

            // when executing all inner jobs
            foreach (IJob innerBeforeJob in innerBeforeJobs)
            {
                managementService.ExecuteJob(innerBeforeJob.Id);
            }
            Assert.AreEqual(5, NUM_INVOCATIONS);

            // then there are five async after jobs
            IList<IJob> innerAfterJobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(5, innerAfterJobs.Count);
            AssertTransitionInstances(processInstance.Id, "service", 5);

            // when executing all inner jobs
            foreach (IJob innerAfterJob in innerAfterJobs)
            {
                managementService.ExecuteJob(innerAfterJob.Id);
            }

            // then there is one mi body after job
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            AssertTransitionInstances(processInstance.Id, "service" + BpmnParse.MultiInstanceBodyIdSuffix, 1);

            // when executing this job, the process ends
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncTaskTest.TestAsyncServiceWrappedInParallelMultiInstance.bpmn20.xml" })]
        public virtual void testAsyncServiceWrappedInParallelMultiInstanceActivityInstance()
        {
            // given a process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("asyncService");

            // when there are five jobs for the inner activity
            Assert.AreEqual(5, managementService.CreateJobQuery().Count());

            // then they are represented in the activity instance tree by transition instances
            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.Assert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("service#multiInstanceBody").Transition("service").Transition("service").Transition("service").Transition("service").Transition("service").Done());
            //Assert.That(tree).HasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("service#multiInstanceBody").Transition("service").Transition("service").Transition("service").Transition("service").Transition("service").Done());
        }

        [Test]
        [Deployment]
        public virtual void testFailingAsyncServiceTimer()
        {
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");
            // now there should be one job in the database, and it is a message
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            IJob job = managementService.CreateJobQuery().First();
            if (!(job is MessageEntity))
            {
                Assert.Fail("the job must be a message");
            }

            ExecuteAvailableJobs();

            // the service failed: the execution is still sitting in the service task:
            IExecution execution = runtimeService.CreateExecutionQuery().First();
            Assert.NotNull(execution);
            Assert.AreEqual("service", runtimeService.GetActiveActivityIds(execution.Id).ElementAt(0));

            // there is still a single job because the timer was created in the same transaction as the
            // service was executed (which rolled back)
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            runtimeService.DeleteProcessInstance(execution.Id, "dead");
        }

        [Test]
        [Deployment]
        public virtual void FAILING_testFailingAsyncServiceTimer()
        {
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");
            // now there are two jobs the message and a timer:
            Assert.AreEqual(2, managementService.CreateJobQuery().Count());

            // let 'max-retires' on the message be reached
            ExecuteAvailableJobs();

            // the service failed: the execution is still sitting in the service task:
            IExecution execution = runtimeService.CreateExecutionQuery().First();
            Assert.NotNull(execution);
            Assert.AreEqual("service", runtimeService.GetActiveActivityIds(execution.Id).ElementAt(0));

            // there are two jobs, the message and the timer (the message will not be retried anymore, max retires is reached.)
            Assert.AreEqual(2, managementService.CreateJobQuery().Count());

            // now the timer triggers:
            ClockUtil.CurrentTime = new DateTime(DateTime.Now.Ticks + 10000);
            ClockUtil.CurrentTime = DateTime.Now.AddMilliseconds(10000);
            ExecuteAvailableJobs();

            // and we are done:
            Assert.IsNull(runtimeService.CreateExecutionQuery().First());
            // and there are no more jobs left:
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceSubProcessTimer()
        {
            INVOCATION = false;
            // start process
            runtimeService.StartProcessInstanceByKey("asyncService");
            // now there should be two jobs in the database:
            Assert.AreEqual(2, managementService.CreateJobQuery().Count());
            // the service was not invoked:
            Assert.IsFalse(INVOCATION);

            //IJob job = managementService.CreateJobQuery(c=>c.Type == "message").First();
            //managementService.ExecuteJob(job.Id);

            // the service was invoked
            Assert.True(INVOCATION);
            // both the timer and the message are cancelled
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

        }

        [Test]
        [Deployment]
        public virtual void testAsyncServiceSubProcess()
        {
            // start process
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("asyncService");

            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            //ActivityInstanceVerification.IActivityInstance activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);

            //Assert.That(activityInstance.HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Transition("subProcess").Done());
            //Assert.That(activityInstance).HasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).Transition("subProcess").Done());

            ExecuteAvailableJobs();

            // both the timer and the message are cancelled
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

        }

        [Test]
        [Deployment]
        public virtual void testAsyncTask()
        {
            // start process
            runtimeService.StartProcessInstanceByKey("asyncTask");
            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            ExecuteAvailableJobs();

            // the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testAsyncScript()
        {
            // start process
            var processInstanceId = runtimeService.StartProcessInstanceByKey("asyncScript").ProcessInstanceId;
            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the script was not invoked:
            string eid = runtimeService.CreateExecutionQuery().First().Id;
            Assert.IsNull(runtimeService.GetVariable(eid, "invoked"));

            ExecuteAvailableJobs();

            // and the job is done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

            // the script was invoked
            Assert.AreEqual("true", runtimeService.GetVariable(eid, "invoked"));

            runtimeService.Signal(eid);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncTaskTest.TestAsyncCallActivity.bpmn20.xml", "resources/bpmn/async/AsyncTaskTest.TestAsyncServiceNoListeners.bpmn20.xml" })]
        public virtual void testAsyncCallActivity()
        {
            // start process
            runtimeService.StartProcessInstanceByKey("asyncCallactivity");
            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            ExecuteAvailableJobs();

            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

        }

        [Test]
        [Deployment]
        public virtual void testAsyncUserTask()
        {
            // start process
            string pid = runtimeService.StartProcessInstanceByKey("asyncUserTask").ProcessInstanceId;
            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not yet invoked:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no usertask
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();
            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));

            // there is a usertask
            Assert.NotNull(taskService.CreateTaskQuery().First());
            // and no more job
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

        }

        [Test]
        [Deployment]
        public virtual void testAsyncManualTask()
        {
            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncManualTask").ProcessInstanceId;

            // now there should be one job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not yet invoked:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no manual Task
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();

            // the listener was invoked now:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore:
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            // now there is a userTask
            Assert.NotNull(taskService.CreateTaskQuery().First());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncIntermediateCatchEvent()
        {
            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncIntermediateCatchEvent").ProcessInstanceId;

            // now there is 1 job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not invoked now:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no intermediate catch event:
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();
            runtimeService.CorrelateMessage("testMessage1");

            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            // now there is a userTask
            Assert.NotNull(taskService.CreateTaskQuery().First());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

        }

        [Test]
        [Deployment]
        public virtual void testAsyncIntermediateThrowEvent()
        {
            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncIntermediateThrowEvent").ProcessInstanceId;

            // now there is 1 job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not invoked now:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no intermediate throw event:
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();

            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            // now there is a userTask
            Assert.NotNull(taskService.CreateTaskQuery().First());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncExclusiveGateway()
        {
            // The test needs variables to work properly
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["flow"] = new BooleanValueImpl(false);

            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncExclusiveGateway", variables).ProcessInstanceId;

            // now there is 1 job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not invoked now:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no gateway:
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();

            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            // now there is a userTask
            Assert.NotNull(taskService.CreateTaskQuery().First());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncInclusiveGateway()
        {
            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncInclusiveGateway").ProcessInstanceId;

            // now there is 1 job in the database:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not invoked now:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no gateway:
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();

            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            // now there are 2 IUser tasks
            IList<ITask> list = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(2, list.Count);

            // complete these tasks and finish the process instance
            foreach (ITask task in list)
            {
                taskService.Complete(task.Id);
            }
        }

        [Test]
        [Deployment]
        public virtual void testAsyncEventGateway()
        {
            // start PI
            string pid = runtimeService.StartProcessInstanceByKey("asyncEventGateway").ProcessInstanceId;

            // now there is a job in the database
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            // the listener was not invoked now:
            Assert.IsNull(runtimeService.GetVariable(pid, "listener"));
            // there is no task:
            Assert.IsNull(taskService.CreateTaskQuery().First());

            ExecuteAvailableJobs();

            // the listener was now invoked:
            Assert.NotNull(runtimeService.GetVariable(pid, "listener"));
            // there isn't a job anymore
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

            // correlate Message
            runtimeService.CorrelateMessage("testMessageDef1");

            // now there is a userTask
            Assert.NotNull(taskService.CreateTaskQuery().First());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);
        }

        [Test]
        [Deployment]
        public virtual void testDeleteShouldNotInvokeListeners()
        {
            RecorderExecutionListener.Clear();

            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("asyncListener", Variables.CreateVariables().PutValue("listener", new RecorderExecutionListener()) as
                IDictionary<string, ITypedValue>);
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            // when deleting the process instance
            runtimeService.DeleteProcessInstance(instance.Id, "");

            // then no listeners for the async activity should have been invoked because
            // it was not active yet
            Assert.AreEqual(0, RecorderExecutionListener.RecordedEvents.Count);

            RecorderExecutionListener.Clear();
        }

        [Test]
        [Deployment]
        public virtual void testDeleteInScopeShouldNotInvokeListeners()
        {
            RecorderExecutionListener.Clear();

            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("asyncListenerSubProcess", Variables.CreateVariables().PutValue("listener", new RecorderExecutionListener()) as IDictionary<string, ITypedValue>);
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            // when deleting the process instance
            runtimeService.DeleteProcessInstance(instance.Id, "");

            // then the async task end listener has not been executed but the listeners of the sub
            // process and the process

            IList<RecorderExecutionListener.RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(2, recordedEvents.Count);
            Assert.AreEqual("subProcess", recordedEvents[0].ActivityId);
            Assert.IsNull(recordedEvents[1].ActivityId); // process instance end event has no activity id

            RecorderExecutionListener.Clear();
        }

        [Test]
        [Deployment]
        public virtual void testDeleteShouldNotInvokeOutputMapping()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("asyncOutputMapping");
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            // when
            runtimeService.DeleteProcessInstance(instance.Id, "");

            // then the output mapping has not been executed because the
            // activity was not active yet
            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)
            {
                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery().Count());
            }

        }

        [Test]
        [Deployment]
        public virtual void testDeleteInScopeShouldNotInvokeOutputMapping()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("asyncOutputMappingSubProcess");
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            // when
            runtimeService.DeleteProcessInstance(instance.Id, "");

            // then
            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)
            {
                // the output mapping of the task has not been executed because the
                // activity was not active yet
                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("taskOutputMappingExecuted")*/.Count());

                // but the containing sub process output mapping was executed
                Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("subProcessOutputMappingExecuted")*/.Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testDeployAndRemoveAsyncActivity()
        {
            ISet<string> deployments = new HashSet<string>();

            try
            {
                // given a deployment that contains a process called "process" with an async task "task"
                IDeployment deployment1 = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/async/AsyncTaskTest.TestDeployAndRemoveAsyncActivity.v1.bpmn20.xml").Deploy();
                deployments.Add(deployment1.Id);

                // when redeploying the process where that task is not contained anymore
                IDeployment deployment2 = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/async/AsyncTaskTest.TestDeployAndRemoveAsyncActivity.v2.bpmn20.xml").Deploy();
                deployments.Add(deployment2.Id);

                // and clearing the deployment cache (note that the equivalent of this in a real-world
                // scenario would be making the deployment with a different engine
                processEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();

                // then it should be possible to load the latest process definition
                IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
                Assert.NotNull(processInstance);

            }
            finally
            {
                foreach (string deploymentId in deployments)
                {
                    repositoryService.DeleteDeployment(deploymentId, true);
                }
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/processWithGatewayAndTwoEndEvents.bpmn20.xml" })]
        public virtual void testGatewayWithTwoEndEventsLastJobReAssignedToParentExe()
        {
            string processKey = repositoryService.CreateProcessDefinitionQuery().First().Key;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey(processKey).Id;

            IList<IJob> jobList = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).ToList();

            // There should be two jobs
            Assert.NotNull(jobList);
            Assert.AreEqual(2, jobList.Count);

            managementService.ExecuteJob(jobList[0].Id);

            // There should be only one job left
            jobList = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(1, jobList.Count);

            // There should only be 1 execution left - the root execution
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            // root execution should be attached to the last job
            Assert.AreEqual(ProcessInstanceId, jobList[0].ExecutionId);

            managementService.ExecuteJob(jobList[0].Id);

            // There should be no more jobs
            jobList = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(0, jobList.Count);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/processGatewayAndTwoEndEventsPlusTimer.bpmn20.xml" })]
        public virtual void testGatewayWithTwoEndEventsLastTimerReAssignedToParentExe()
        {
            string processKey = repositoryService.CreateProcessDefinitionQuery().First().Key;
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey(processKey).Id;

            IList<IJob> jobList = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).ToList();

            // There should be two jobs
            Assert.NotNull(jobList);
            Assert.AreEqual(2, jobList.Count);

            // execute timer first
            string timerId = managementService.CreateJobQuery()/*/*.Timers()*/.First().Id;
            managementService.ExecuteJob(timerId);

            // There should be only one job left
            jobList = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(1, jobList.Count);

            // There should only be 1 execution left - the root execution
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery().Count());

            // root execution should be attached to the last job
            Assert.AreEqual(ProcessInstanceId, jobList[0].ExecutionId);

            // execute service task
            managementService.ExecuteJob(jobList[0].Id);

            // There should be no more jobs
            jobList = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(0, jobList.Count);
        }

        [Test]
        [Deployment]
        public virtual void FAILING_testLongProcessDefinitionKey()
        {
            string key = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey(key).Id;

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();

            Assert.AreEqual(key, job.ProcessDefinitionKey);
        }
    }

}