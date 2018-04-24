using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     Specific <seealso cref="IFormData" /> for completing a task.
    ///      
    /// </summary>
    public interface ITaskFormData : IFormData
    {
        /// <summary>
        ///     The task for which this form is used to complete it.
        /// </summary>
        ITask Task { get; }
    }
}