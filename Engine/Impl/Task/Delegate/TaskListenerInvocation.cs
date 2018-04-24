using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.task.@delegate
{
    /// <summary>
    ///     Class handling invocations of <seealso cref="ITaskListener" />
    ///     
    /// </summary>
    public class TaskListenerInvocation : DelegateInvocation
    {
        protected internal readonly IDelegateTask DelegateTask;

        protected internal readonly ITaskListener TaskListenerInstance;

        public TaskListenerInvocation(ITaskListener executionListenerInstance, IDelegateTask delegateTask)
            : this(executionListenerInstance, delegateTask, null)
        {
        }

        public TaskListenerInvocation(ITaskListener taskListenerInstance, IDelegateTask delegateTask,
            IBaseDelegateExecution contextExecution) : base(contextExecution, null)
        {
            this.TaskListenerInstance = taskListenerInstance;
            this.DelegateTask = delegateTask;
        }
        
        protected internal override void Invoke()
        {
            TaskListenerInstance.Notify(DelegateTask);
        }
    }
}