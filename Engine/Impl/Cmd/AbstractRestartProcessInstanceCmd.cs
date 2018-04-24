using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    public abstract class AbstractRestartProcessInstanceCmd<T> : ICommand<T>
    {

        protected internal ICommandExecutor commandExecutor;
        protected internal RestartProcessInstanceBuilderImpl builder;

        public AbstractRestartProcessInstanceCmd(ICommandExecutor commandExecutor, RestartProcessInstanceBuilderImpl builder)
        {
            this.commandExecutor = commandExecutor;
            this.builder = builder;
        }

        protected internal virtual ICollection<string> CollectProcessInstanceIds()
        {

            List<string> collectedProcessInstanceIds = new List<string>();

            IList<string> processInstanceIds = builder.ProcessInstanceIds;
            if (processInstanceIds != null)
            {
                collectedProcessInstanceIds.AddRange(processInstanceIds);
            }
            var historicProcessInstanceQuery = builder.HistoricProcessInstanceQuery;
            if (historicProcessInstanceQuery != null)
            {
                collectedProcessInstanceIds.AddRange(historicProcessInstanceQuery.Select(c=>c.Id).ToList());
            }

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "processInstanceIds", collectedProcessInstanceIds);
            return collectedProcessInstanceIds;
        }

        protected internal virtual void WriteUserOperationLog(CommandContext commandContext, IProcessDefinition processDefinition, int numInstances, bool async)
        {

            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            propertyChanges.Add(new PropertyChange("async", null, async));

            commandContext.OperationLogManager.LogProcessInstanceOperation(UserOperationLogEntryFields.OperationTypeRestartProcessInstance, null, processDefinition.Id, processDefinition.Key, propertyChanges);
        }

        protected internal virtual ProcessDefinitionEntity GetProcessDefinition(CommandContext commandContext, string processDefinitionId)
        {

            return commandContext.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
        }

        public abstract T Execute(CommandContext commandContext);
    }
}
