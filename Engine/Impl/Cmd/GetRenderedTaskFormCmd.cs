using System;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetRenderedTaskFormCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string FormEngineName;
        protected internal string TaskId;

        public GetRenderedTaskFormCmd(string taskId, string formEngineName)
        {
            this.TaskId = taskId;
            this.FormEngineName = formEngineName;
        }


        public virtual object Execute(CommandContext commandContext)
        {
            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("ITask '" + TaskId + "' not found", "ITask", task);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadTask(task);
            }
            EnsureUtil.EnsureNotNull("ITask form definition for '" + TaskId + "' not found", "ITask.getTaskDefinition()",
                task.TaskDefinition);

            ITaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
            if (taskFormHandler == null)
            {
                return null;
            }

            var formEngine = context.Impl.Context.ProcessEngineConfiguration.FormEngines[FormEngineName];

            EnsureUtil.EnsureNotNull("No formEngine '" + FormEngineName + "' defined process engine configuration",
                "formEngine", formEngine);

            var taskForm = taskFormHandler.CreateTaskForm(task);

            return formEngine.RenderTaskForm(taskForm);
        }
    }
}