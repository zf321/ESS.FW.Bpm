using System;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class TaskFormDataImpl : FormDataImpl, ITaskFormData
    {
        private const long SerialVersionUid = 1L;


        // getters and setters //////////////////////////////////////////////////////

        public virtual ITask Task { get; set; }
    }
}