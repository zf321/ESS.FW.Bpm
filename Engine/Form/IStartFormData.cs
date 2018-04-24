using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     Specific <seealso cref="IFormData" /> for starting a new process instance.
    ///      
    /// </summary>
    public interface IStartFormData : IFormData
    {
        /// <summary>
        ///     The process definition for which this form is starting a new process instance
        /// </summary>
        IProcessDefinition ProcessDefinition { get; }
    }
}