using System;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.End
{


    /// <summary>
    /// 
    /// </summary>
    public class EndEventTest : PluggableProcessEngineTestCase
    {

        // Test case for ACT-1259
        [Deployment]
        [Test]
        public virtual void testConcurrentEndOfSameProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskWithDelay");
            ITask task = taskService.CreateTaskQuery().FirstOrDefault();
            Assert.NotNull(task);

            // We will now start two threads that both complete the task.
            // In the process, the task is followed by a delay of three seconds
            // This will cause both threads to call the taskService.Complete method with enough time,
            // before ending the process. Both threads will now try to end the process
            // and only one should succeed (due to optimistic locking).
            TaskCompleter taskCompleter1 = new TaskCompleter(taskService,this, task.Id);
            TaskCompleter taskCompleter2 = new TaskCompleter(taskService, this, task.Id);

            Assert.IsFalse(taskCompleter1.Succeeded);
            Assert.IsFalse(taskCompleter2.Succeeded);

            taskCompleter1.Start();
            taskCompleter2.Start();
            taskCompleter1.Join();
            taskCompleter2.Join();

            int successCount = 0;
            if (taskCompleter1.Succeeded)
            {
                successCount++;
            }
            if (taskCompleter2.Succeeded)
            {
                successCount++;
            }

            Assert.AreEqual(/*"(Only) one thread should have been able to successfully end the process",*/ 1, successCount);
            AssertProcessEnded(processInstance.Id);
        }

        /// <summary>
        /// Helper class for concurrent testing </summary>
        internal class TaskCompleter /*: Thread*/
        {
            private readonly EndEventTest outerInstance;


            protected internal string taskId;
            protected internal bool succeeded;
            private ITaskService taskService;
            public TaskCompleter(ITaskService taskService, EndEventTest outerInstance, string taskId)
            {
                this.outerInstance = outerInstance;
                this.taskId = taskId;
                this.taskService = taskService;
            }

            public virtual bool Succeeded
            {
                get
                {
                    return succeeded;
                }
            }
           
            public virtual void Run()
            {
                try
                {
                    taskService.Complete(taskId);
                    succeeded = true;
                }
                catch (OptimisticLockingException e)
                {
                    throw e;
                    // Exception is expected for one of the threads
                }
                catch(System.Exception e)
                {
                    throw e;
                }
            }

            internal void Start()
            {
                if (taskService != null)
                {
                    Run();
                }
                else
                {
                    throw new NotImplementedException("taskService is Null");
                }
            }

            internal void Join()
            {
                //Thread t = new Thread();
                //t.Join()
                throw new NotImplementedException("线程管理 Thread.Join");
            }
        }

    }

}