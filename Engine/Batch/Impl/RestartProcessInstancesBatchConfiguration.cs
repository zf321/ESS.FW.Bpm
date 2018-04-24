using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cmd;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    
    /// 
    /// <summary>
    /// 
    /// </summary>
    public class RestartProcessInstancesBatchConfiguration : BatchConfiguration
    {
        public RestartProcessInstancesBatchConfiguration(IList<string> processInstanceIds, IList<AbstractProcessInstanceModificationCommand> instructions, string processDefinitionId, bool initialVariables, bool skipCustomListeners, bool skipIoMappings, bool withoutBusinessKey) : base(processInstanceIds)
        {
            this.Instructions = instructions;
            this.ProcessDefinitionId = processDefinitionId;
            this.InitialVariables = initialVariables;
            this.SkipCustomListeners = skipCustomListeners;
            this.SkipIoMappings = skipIoMappings;
            this.WithoutBusinessKey = withoutBusinessKey;
        }

        public virtual IList<AbstractProcessInstanceModificationCommand> Instructions { get; set; }


        public virtual string ProcessDefinitionId { get; set; }


        public virtual bool InitialVariables { get; set; }


        public virtual bool SkipCustomListeners { get; set; }


        public virtual bool SkipIoMappings { get; set; }


        public virtual bool WithoutBusinessKey { get; set; }
    }
}
