using System;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetTaskFormCmd : ICommand<ITaskFormData>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public GetTaskFormCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual ITaskFormData Execute(CommandContext commandContext)
        {
            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("No ITask found for taskId '" + TaskId + "'", "ITask", task);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadTask(task);
            }

            if (task.TaskDefinition != null)
            {
                ITaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
                EnsureUtil.EnsureNotNull("No taskFormHandler specified for ITask '" + TaskId + "'", "taskFormHandler", taskFormHandler);

                return taskFormHandler.CreateTaskForm(task);
            }
            else
            {
                // Standalone ITask, no TaskFormData available
                return null;
            }
        }
    }
}