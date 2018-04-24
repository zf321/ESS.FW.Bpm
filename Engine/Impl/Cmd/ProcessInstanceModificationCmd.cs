using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    public class ProcessInstanceModificationCmd : AbstractModificationCmd<object>
    {

        private static readonly CommandLogger LOG = ProcessEngineLogger.CmdLogger;
        protected internal bool writeUserOperationLog;

        public ProcessInstanceModificationCmd(ModificationBuilderImpl modificationBuilderImpl, bool writeUserOperationLog) : base(modificationBuilderImpl)
        {
            this.writeUserOperationLog = writeUserOperationLog;
        }
        
        public override object Execute(CommandContext commandContext)
        {
            IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
            ICollection<string> processInstanceIds = CollectProcessInstanceIds(commandContext);

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Modification instructions cannot be empty", instructions);
            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "Process instance ids", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "Process instance ids", processInstanceIds);

            ProcessDefinitionEntity processDefinition = GetProcessDefinition(commandContext, builder.ProcessDefinitionId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Process definition id cannot be null", processDefinition);

            if (writeUserOperationLog)
            {
                WriteUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, false);
            }

            bool skipCustomListeners = builder.SkipCustomListeners;
            bool skipIoMappings = builder.SkipIoMappings;

            foreach (string processInstanceId in processInstanceIds)
            {
                ExecutionEntity processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);

                EnsureProcessInstanceExist(processInstanceId, processInstance);
                EnsureSameProcessDefinition(processInstance, processDefinition.Id);

                ProcessInstanceModificationBuilderImpl builder = new ProcessInstanceModificationBuilderImpl(commandContext, processInstanceId);
                SetProcessInstanceId(instructions, processInstanceId);
                builder.ModificationOperations = instructions;

                builder.Execute(false, skipCustomListeners/*, skipIoMappings*/);
            }

            return null;
        }

        protected internal virtual void SetProcessInstanceId(IList<AbstractProcessInstanceModificationCommand> instructions, string processInstanceId)
        {
            foreach (AbstractProcessInstanceModificationCommand operationCmd in instructions)
            {
                operationCmd.ProcessInstanceId = processInstanceId;
            }
        }

        protected internal virtual void EnsureSameProcessDefinition(ExecutionEntity processInstance, string processDefinitionId)
        {
            if (!processDefinitionId.Equals(processInstance.ProcessDefinitionId))
            {
                throw LOG.ProcessDefinitionOfInstanceDoesNotMatchModification(processInstance, processDefinitionId);
            }
        }

        protected internal virtual void EnsureProcessInstanceExist(string processInstanceId, ExecutionEntity processInstance)
        {
            if (processInstance == null)
            {
                throw LOG.ProcessInstanceDoesNotExist(processInstanceId);
            }
        }

    }
}
