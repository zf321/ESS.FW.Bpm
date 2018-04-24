using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    public abstract class AbstractModificationCmd<T> : ICommand<T>
    {

        protected internal ModificationBuilderImpl builder;

        public AbstractModificationCmd(ModificationBuilderImpl modificationBuilderImpl)
        {
            this.builder = modificationBuilderImpl;
        }

        protected internal virtual ICollection<string> CollectProcessInstanceIds(CommandContext commandContext)
        {

            ISet<string> collectedProcessInstanceIds = new HashSet<string>();

            IList<string> processInstanceIds = builder.ProcessInstanceIds;
            if (processInstanceIds != null)
            {
                collectedProcessInstanceIds.AddAll(processInstanceIds);
            }
            
            var processInstanceQuery = builder.ProcessInstanceQuery;
            if (processInstanceQuery != null)
            {
                collectedProcessInstanceIds.AddAll(processInstanceQuery.Select(c=>c.Id).ToList());
            }

            return collectedProcessInstanceIds;
        }

        protected internal virtual void WriteUserOperationLog(CommandContext commandContext, IProcessDefinition processDefinition, int numInstances, bool async)
        {

            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            propertyChanges.Add(new PropertyChange("async", null, async));

            commandContext.OperationLogManager.LogProcessInstanceOperation(UserOperationLogEntryFields.OperationTypeModifyProcessInstance, null, processDefinition.Id, processDefinition.Key, propertyChanges);
        }

        protected internal virtual ProcessDefinitionEntity GetProcessDefinition(CommandContext commandContext, string processDefinitionId)
        {

            return commandContext.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
        }

        public abstract T Execute(CommandContext commandContext);
    }
}
