using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class CreateTaskCmd : ICommand<ITask>
    {
        protected internal string TaskId;

        public CreateTaskCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual ITask Execute(CommandContext commandContext)
        {
            CheckCreateTask(commandContext);

            var task = TaskEntity.Create();
            task.Id = TaskId;
            return (ITask) task;
        }

        protected internal virtual void CheckCreateTask(CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckCreateTask();
        }
    }
}