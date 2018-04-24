using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///      
    /// </summary>
    public class DefaultTaskFormHandler : DefaultFormHandler, ITaskFormHandler
    {
        public virtual ITaskFormData CreateTaskForm(TaskEntity task)
        {
            var taskFormData = new TaskFormDataImpl();

            //Expression formKey = task.TaskDefinition.FormKey;

            //if (formKey != null)
            //{
            //    var formValue = formKey.getValue(task);
            //    if (formValue != null)
            //    {
            //        taskFormData.FormKey = formValue.ToString();
            //    }
            //}

            taskFormData.DeploymentId = DeploymentId;
            //taskFormData.Task = task;
            //initializeFormProperties(taskFormData, task.getExecution());
            //initializeFormFields(taskFormData, task.getExecution());
            return taskFormData;
        }
    }
}