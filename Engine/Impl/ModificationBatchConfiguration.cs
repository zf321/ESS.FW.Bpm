using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;

namespace ESS.FW.Bpm.Engine.Impl
{
    
    public class ModificationBatchConfiguration : BatchConfiguration
    {

        protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
        protected internal bool skipCustomListeners;
        protected internal bool skipIoMappings;
        protected internal string processDefinitionId;

        public ModificationBatchConfiguration(IList<string> ids, string processDefinitionId, IList<AbstractProcessInstanceModificationCommand> instructions, bool skipCustomListeners, bool skipIoMappings) : base(ids)
        {
            this.instructions = instructions;
            this.processDefinitionId = processDefinitionId;
            this.skipCustomListeners = skipCustomListeners;
            this.skipIoMappings = skipIoMappings;
        }

        public virtual IList<AbstractProcessInstanceModificationCommand> Instructions
        {
            get
            {
                return instructions;
            }
        }

        public virtual string ProcessDefinitionId
        {
            get
            {
                return processDefinitionId;
            }
        }

        public virtual bool SkipCustomListeners
        {
            get
            {
                return skipCustomListeners;
            }
        }

        public virtual bool SkipIoMappings
        {
            get
            {
                return skipIoMappings;
            }
        }

    }
}
