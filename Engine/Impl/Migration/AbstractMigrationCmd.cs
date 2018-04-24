using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractMigrationCmd<T> : ICommand<T>
    {
        protected internal MigrationPlanExecutionBuilderImpl ExecutionBuilder;

        public AbstractMigrationCmd(MigrationPlanExecutionBuilderImpl executionBuilder)
        {
            this.ExecutionBuilder = executionBuilder;
        }

        public abstract T Execute(CommandContext commandContext);

        protected internal virtual void CheckAuthorizations(CommandContext commandContext,
            ProcessDefinitionEntity sourceDefinition, ProcessDefinitionEntity targetDefinition,
            ICollection<string> processInstanceIds)
        {
            //CompositePermissionCheck migrateInstanceCheck = (new PermissionCheckBuilder()).conjunctive().atomicCheckForResourceId(Resources.PROCESS_DEFINITION, sourceDefinition.Key, Permissions.MIGRATE_INSTANCE).atomicCheckForResourceId(Resources.PROCESS_DEFINITION, targetDefinition.Key, Permissions.MIGRATE_INSTANCE).build();

            //commandContext.AuthorizationManager.checkAuthorization(migrateInstanceCheck);
        }

        protected internal virtual ICollection<string> CollectProcessInstanceIds(CommandContext commandContext)
        {
            ISet<string> collectedProcessInstanceIds = new HashSet<string>();

            var processInstanceIds = ExecutionBuilder.ProcessInstanceIdsRenamed;
            if (processInstanceIds != null)
            {
                //collectedProcessInstanceIds.addAll(processInstanceIds);
            }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery = (org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl) executionBuilder.getProcessInstanceQuery();
            var processInstanceQuery =  ExecutionBuilder.ProcessInstanceQueryRenamed;
            if (processInstanceQuery != null)
            {
                //collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
            }

            return collectedProcessInstanceIds;
        }

        protected internal virtual void WriteUserOperationLog(CommandContext commandContext,
            ProcessDefinitionEntity sourceProcessDefinition, ProcessDefinitionEntity targetProcessDefinition,
            int numInstances, bool async)
        {
            //IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            //propertyChanges.Add(new PropertyChange("processDefinitionId", sourceProcessDefinition.Id, targetProcessDefinition.Id));
            //propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            //propertyChanges.Add(new PropertyChange("async", null, async));

            //commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MIGRATE, null, sourceProcessDefinition.Id, sourceProcessDefinition.Key, propertyChanges);
        }

        protected internal virtual ProcessDefinitionEntity ResolveSourceProcessDefinition(CommandContext commandContext)
        {
            var sourceProcessDefinitionId = ExecutionBuilder.MigrationPlan.SourceProcessDefinitionId;

            var sourceProcessDefinition = GetProcessDefinition(commandContext, sourceProcessDefinitionId);
            EnsureUtil.EnsureNotNull("sourceProcessDefinition", sourceProcessDefinition);

            return sourceProcessDefinition;
        }

        protected internal virtual ProcessDefinitionEntity ResolveTargetProcessDefinition(CommandContext commandContext)
        {
            var targetProcessDefinitionId = ExecutionBuilder.MigrationPlan.TargetProcessDefinitionId;

            var sourceProcessDefinition = GetProcessDefinition(commandContext, targetProcessDefinitionId);
            EnsureUtil.EnsureNotNull("sourceProcessDefinition", sourceProcessDefinition);

            return sourceProcessDefinition;
        }

        protected internal virtual ProcessDefinitionEntity GetProcessDefinition(CommandContext commandContext,
            string processDefinitionId)
        {
            //return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
            return null;
        }
    }
}