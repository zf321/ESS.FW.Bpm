using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///      
    /// </summary>
    public interface ITaskFormHandler : IFormHandler
    {
        ITaskFormData CreateTaskForm(TaskEntity task);
    }
}