using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{
    [TestFixture]
    public class AsyncAfterTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testTransitionIdRequired()
        {
            // if an outgoing sequence flow has no id, we cannot use it in asyncAfter
            try
            {
                
                //Assert.Fail("Exception expected");
                var val = Assert.Throws<ProcessEngineException>(() =>
                {
                    repositoryService.CreateDeployment()
                        .AddClasspathResource("resources/bpmn/async/AsyncAfterTest.TestTransitionIdRequired.bpmn20.xml")
                        .DeployAndReturnDefinitions();
                });
            }
            catch (ProcessEngineException e)
            {
                var val =
                    e.Message.Contains(
                        "Sequence flow with sourceRef='service' must have an id, activity with id 'service' uses 'asyncAfter'.");
                Assert.IsTrue(val);
                //AssertTextPresent("Sequence flow with sourceRef='service' must have an id, activity with id 'service' uses 'asyncAfter'.", e.Message);
            }

        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterServiceTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // listeners should be fired by now
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the catch event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the Waiting job is executed, the process instance should end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterMultiInstanceUserTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            IList<ITask> list = taskService.CreateTaskQuery().ToList();
            
            // multiinstance says three in the bpmn                    
            //Assert.That(list, hasSize(3));          
            Assert.AreEqual(3, list.Count);

            foreach (ITask task in list)
            {
                taskService.Complete(task.Id);
            }
            WaitForJobExecutorToProcessAllJobs(5);
            //WaitForJobExecutorToProcessAllJobs(TimeUnit.MILLISECONDS.convert(5L, TimeUnit.SECONDS));

            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeServiceTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // the service task is not yet invoked
            AssertNotListenerStartInvoked(pi);
            AssertNotBehaviorInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the job is executed
            managementService.ExecuteJob(job.Id);

            // the manual task is invoked
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and now the process is Waiting *after* the manual task
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the Waiting job, the process instance will end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterServiceTaskMultipleTransitions()
        {
            IDictionary<string, ITypedValue> varMap = new Dictionary<string, ITypedValue>();
            varMap["flowToTake"] = new StringValueImpl("flowToTake");

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", varMap);

            // the service task is completely invoked
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and the execution is Waiting *after* the service task
            IJob continuationJob = managementService.CreateJobQuery().First();
            Assert.NotNull(continuationJob);

            // if we execute the job, the process instance continues along the selected path
            managementService.ExecuteJob(continuationJob.Id);

            Assert.NotNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow2").First());
            Assert.IsNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow3").First());

            // end the process
            runtimeService.Signal(pi.Id);

            //////////////////////////////////////////////////////////////

            // start process instance
            varMap = new Dictionary<string, ITypedValue>();
            varMap["flowToTake"] = new StringValueImpl("flow3");
            pi = runtimeService.StartProcessInstanceByKey("testProcess", varMap);

            // the service task is completely invoked
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and the execution is Waiting *after* the service task
            continuationJob = managementService.CreateJobQuery().First();
            Assert.NotNull(continuationJob);

            // if we execute the job, the process instance continues along the selected path
            managementService.ExecuteJob(continuationJob.Id);

            Assert.IsNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow2").First());
            Assert.NotNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow3").First());

        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterServiceTaskMultipleTransitionsConcurrent()
        {

            // start process instance
            IDictionary<string, ITypedValue> varMap = new Dictionary<string, ITypedValue>();
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", varMap);

            // the service task is completely invoked
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi);
            AssertListenerEndInvoked(pi);

            // there are two async jobs
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, jobs.Count);
            managementService.ExecuteJob(jobs[0].Id);
            managementService.ExecuteJob(jobs[1].Id);

            // both subsequent tasks are activated
            Assert.NotNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow2").First());
            Assert.NotNull(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "taskAfterFlow3").First());

        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterWithoutTransition()
        {

            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // the service task is completely invoked
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and the execution is Waiting *after* the service task
            IJob continuationJob = managementService.CreateJobQuery().First();
            Assert.NotNull(continuationJob);

            // but the process end listeners have not been invoked yet
            Assert.IsNull(runtimeService.GetVariable(pi.Id, "process-listenerEndInvoked"));

            // if we execute the job, the process instance ends.
            managementService.ExecuteJob(continuationJob.Id);
            AssertProcessEnded(pi.Id);

        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterInNestedWithoutTransition()
        {

            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // the service task is completely invoked
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and the execution is Waiting *after* the service task
            IJob continuationJob = managementService.CreateJobQuery().First();
            Assert.NotNull(continuationJob);

            // but the subprocess end listeners have not been invoked yet
            Assert.IsNull(runtimeService.GetVariable(pi.Id, "subprocess-listenerEndInvoked"));

            // if we execute the job, the listeners are invoked;
            managementService.ExecuteJob(continuationJob.Id);
            Assert.True((bool)runtimeService.GetVariable(pi.Id, "subprocess-listenerEndInvoked"));

        }
       
        [Test]
        [Deployment]
        public virtual void testAsyncAfterManualTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testManualTask");

            // listeners should be fired by now
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the catch event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the Waiting job is executed, the process instance should end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeManualTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testManualTask");

            // the service task is not yet invoked
            AssertNotListenerStartInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the job is executed
            managementService.ExecuteJob(job.Id);

            // the manual task is invoked
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and now the process is Waiting *after* the manual task
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the Waiting job, the process instance will end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterIntermediateCatchEvent()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testIntermediateCatchEvent");

            // the intermediate catch event is Waiting for its message
            runtimeService.CorrelateMessage("testMessage1");

            // listeners should be fired by now
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the catch event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the Waiting job is executed, the process instance should end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeIntermediateCatchEvent()
        {

            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testIntermediateCatchEvent");

            // check that no listener is invoked by now
            AssertNotListenerStartInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            // the process is Waiting before the message event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // execute job to get to the message event
            ExecuteAvailableJobs();

            // now we need to trigger the message to proceed
            runtimeService.CorrelateMessage("testMessage1");

            // now the listener should be invoked
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and now the process is Waiting *after* the intermediate catch event
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the Waiting job, the process instance will end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterIntermediateThrowEvent()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testIntermediateThrowEvent");

            // listeners should be fired by now
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the throw event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the Waiting job is executed, the process instance should end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeIntermediateThrowEvent()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testIntermediateThrowEvent");

            // the throw event is not yet invoked
            AssertNotListenerStartInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // if the job is executed
            managementService.ExecuteJob(job.Id);

            // the listeners are invoked
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and now the process is Waiting *after* the throw event
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the Waiting job, the process instance will end
            managementService.ExecuteJob(job.Id);
            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterInclusiveGateway()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testInclusiveGateway");

            // listeners should be fired
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the gateway
            //Assert.AreEqual(2, managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

            ExecuteAvailableJobs();

            // if the Waiting job is executed there should be 2 IUser tasks
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            //Assert.AreEqual(2, taskQuery.Active().Count());

            // finish tasks
            //IList<ITask> tasks = taskQuery.Active().ToList();
            //foreach (ITask task in tasks)
            //{
            //    taskService.Complete(task.Id);
            //}

            AssertProcessEnded(pi.ProcessInstanceId);

        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeInclusiveGateway()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testInclusiveGateway");

            // no listeners are fired:
            AssertNotListenerStartInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            // we should Wait *before* the gateway:
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the gateway:
            managementService.ExecuteJob(job.Id);

            // the listeners are fired:
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and we will Wait *after* the gateway:
            IList<IJob> jobs = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).ToList();
            Assert.AreEqual(2, jobs.Count);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterExclusiveGateway()
        {
            // start process instance with variables
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["flow"] = new BooleanValueImpl(false);

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testExclusiveGateway", variables);

            // listeners should be fired
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* the gateway
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

            ExecuteAvailableJobs();

            // if the Waiting job is executed there should be 2 IUser tasks
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            //Assert.AreEqual(1, taskQuery.Active().Count());

            // finish tasks
            //IList<ITask> tasks = taskQuery.Active().ToList();
            //foreach (ITask task in tasks)
            //{
            //    taskService.Complete(task.Id);
            //}

            AssertProcessEnded(pi.ProcessInstanceId);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterAndBeforeExclusiveGateway()
        {
            // start process instance with variables
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["flow"] = new BooleanValueImpl(false);

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testExclusiveGateway", variables);

            // no listeners are fired:
            AssertNotListenerStartInvoked(pi);
            AssertNotListenerEndInvoked(pi);

            // we should Wait *before* the gateway:
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // after executing the gateway:
            managementService.ExecuteJob(job.Id);

            // the listeners are fired:
            AssertListenerStartInvoked(pi);
            AssertListenerEndInvoked(pi);

            // and we will Wait *after* the gateway:
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterWithExecutionListener()
        {
            // given an async after job and an execution listener on that task
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            AssertNotListenerTakeInvoked(processInstance);

            // when the job is executed
            managementService.ExecuteJob(job.Id);

            // then the process should advance and not recreate the job
            job = managementService.CreateJobQuery().First();
            Assert.IsNull(job);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            AssertListenerTakeInvoked(processInstance);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterOnParallelGatewayFork()
        {
            //string configuration = PvmAtomicOperation.TRANSITION_NOTIFY_LISTENER_TAKE.CanonicalName;
            string configuration = PvmAtomicOperationFields.TransitionNotifyListenerTake.CanonicalName;
            string config1 = configuration + "$afterForkFlow1";
            string config2 = configuration + "$afterForkFlow2";

            runtimeService.StartProcessInstanceByKey("process");

            // there are two jobs
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, jobs.Count);
            IJob jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, config1);
            Assert.NotNull(jobToExecute);
            managementService.ExecuteJob(jobToExecute.Id);

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="theTask1").First();
            Assert.NotNull(task1);

            // there is one left
            jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(1, jobs.Count);
            jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, config2);
            managementService.ExecuteJob(jobToExecute.Id);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="theTask2").First();
            Assert.NotNull(task2);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterParallelMultiInstanceWithServiceTask()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // listeners and behavior should be invoked by now
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi, 5);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* execute all service tasks
            ExecuteAvailableJobs(1);

            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterServiceWrappedInParallelMultiInstance()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // listeners and behavior should be invoked by now
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi, 5);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* execute each service task wrapped in the multi-instance body
            Assert.AreEqual(5L, managementService.CreateJobQuery().Count());
            // execute all jobs - one for each service task
            ExecuteAvailableJobs(5);

            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void testAsyncAfterServiceWrappedInSequentialMultiInstance()
        {
            // start process instance
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            // listeners and behavior should be invoked by now
            AssertListenerStartInvoked(pi);
            AssertBehaviorInvoked(pi, 1);
            AssertListenerEndInvoked(pi);

            // the process should Wait *after* execute each service task step-by-step 
            Assert.AreEqual(1L, managementService.CreateJobQuery().Count());
            // execute all jobs - one for each service task wrapped in the multi-instance body
            ExecuteAvailableJobs(5);

            // behavior should be invoked for each service task
            AssertBehaviorInvoked(pi, 5);

            // the process should Wait on IUser task after execute all service tasks
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }
        
        [Test]
        [Deployment]
        public virtual void FAILING_testAsyncAfterOnParallelGatewayJoin()
        {
            string configuration = PvmAtomicOperationFields.ActivityEnd.CanonicalName;

            runtimeService.StartProcessInstanceByKey("process");

            // there are three jobs
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(3, jobs.Count);
            IJob jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, configuration);
            Assert.NotNull(jobToExecute);
            managementService.ExecuteJob(jobToExecute.Id);

            // there are two jobs left
            jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, jobs.Count);
            jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, configuration);
            managementService.ExecuteJob(jobToExecute.Id);

            // there is one job left
            jobToExecute = managementService.CreateJobQuery().First();
            Assert.NotNull(jobToExecute);
            managementService.ExecuteJob(jobToExecute.Id);

            // the process should stay in the IUser task
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
        }

        
        protected internal virtual IJob fetchFirstJobByHandlerConfiguration(IList<IJob> jobs, string configuration)
        {
            foreach (IJob job in jobs)
            {
                JobEntity jobEntity = (JobEntity)job;
                string jobConfig = jobEntity.JobHandlerConfigurationRaw;
                if (configuration.Equals(jobConfig))
                {
                    return job;
                }
            }

            return null;
        }

        protected internal virtual void AssertListenerStartInvoked(IExecution e)
        {
            Assert.True((bool)runtimeService.GetVariable(e.Id, "listenerStartInvoked"));
        }
        
        protected internal virtual void AssertListenerTakeInvoked(IExecution e)
        {
            Assert.True((bool)runtimeService.GetVariable(e.Id, "listenerTakeInvoked"));
        }
        
        protected internal virtual void AssertListenerEndInvoked(IExecution e)
        {
            Assert.True((bool)runtimeService.GetVariable(e.Id, "listenerEndInvoked"));
        }
        
        protected internal virtual void AssertBehaviorInvoked(IExecution e)
        {
            Assert.True((bool)runtimeService.GetVariable(e.Id, "behaviorInvoked"));
        }
        
        private void AssertBehaviorInvoked(IProcessInstance pi, int times)
        {
            long? behaviorInvoked = (long?)runtimeService.GetVariable(pi.Id, "behaviorInvoked");
            Assert.NotNull(behaviorInvoked, "behavior was not invoked");
            //Assert.NotNull("behavior was not invoked", behaviorInvoked);
            Assert.AreEqual(times, behaviorInvoked.Value);

        }
        
        protected internal virtual void AssertNotListenerStartInvoked(IExecution e)
        {
            Assert.IsNull(runtimeService.GetVariable(e.Id, "listenerStartInvoked"));
        }
        
        protected internal virtual void AssertNotListenerTakeInvoked(IExecution e)
        {
            Assert.IsNull(runtimeService.GetVariable(e.Id, "listenerTakeInvoked"));
        }
        
        protected internal virtual void AssertNotListenerEndInvoked(IExecution e)
        {
            Assert.IsNull(runtimeService.GetVariable(e.Id, "listenerEndInvoked"));
        }
        
        protected internal virtual void AssertNotBehaviorInvoked(IExecution e)
        {
            Assert.IsNull(runtimeService.GetVariable(e.Id, "behaviorInvoked"));
        }

    }

}