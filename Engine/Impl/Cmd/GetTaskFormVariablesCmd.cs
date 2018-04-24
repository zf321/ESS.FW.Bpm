using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;


using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTaskFormVariablesCmd : AbstractGetFormVariablesCmd
    {
        private const long SerialVersionUid = 1L;

        public GetTaskFormVariablesCmd(string taskId, ICollection<string> variableNames, bool deserializeObjectValues)
            : base(taskId, variableNames, deserializeObjectValues)
        {
        }

        public override IVariableMap Execute(CommandContext commandContext)
        {
            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(ResourceId);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Cannot find ITask with id '" + ResourceId + "'.", "ITask", task);

            CheckGetTaskFormVariables(task, commandContext);

            VariableMapImpl result = new VariableMapImpl();

            // first, evaluate form fields
            TaskDefinition taskDefinition = task.TaskDefinition;
            if (taskDefinition != null)
            {
                ITaskFormData taskFormData = taskDefinition.TaskFormHandler.CreateTaskForm(task);
                foreach (IFormField formField in taskFormData.FormFields)
                {
                    if (FormVariableNames == null || FormVariableNames.Contains(formField.Id))
                    {
                        result.PutValue(formField.Id, CreateVariable(formField, task));
                    }
                }
            }

            // collect remaining variables from ITask scope and parent scopes
            task.CollectVariables(result, FormVariableNames, false, DeserializeObjectValues);

            return result;
        }

        protected internal virtual void CheckGetTaskFormVariables(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadTask(task);
        }
    }
}