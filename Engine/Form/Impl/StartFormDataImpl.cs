using System;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class StartFormDataImpl : FormDataImpl, IStartFormData
    {
        private const long SerialVersionUid = 1L;

        protected internal IProcessDefinition processDefinition;

        // getters and setters //////////////////////////////////////////////////////

        public virtual IProcessDefinition ProcessDefinition
        {
            get { return processDefinition; }
            set { processDefinition = value; }
        }
    }
}