using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class RestartProcessInstancesCmd : AbstractRestartProcessInstanceCmd<object>
    {
        private static readonly CommandLogger LOG = ProcessEngineLogger.CmdLogger;

        protected internal bool writeUserOperationLog;

        public RestartProcessInstancesCmd(ICommandExecutor commandExecutor, RestartProcessInstanceBuilderImpl builder,
            bool writeUserOperationLog) : base(commandExecutor, builder)
        {
            this.writeUserOperationLog = writeUserOperationLog;
        }

        public override object Execute(CommandContext commandContext)
        {
            var processInstanceIds = CollectProcessInstanceIds();
            var instructions = builder.Instructions;

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Restart instructions cannot be empty",
                "instructions", instructions);
            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty",
                "Process instance ids", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null",
                "Process instance ids", processInstanceIds);

            var processDefinition = GetProcessDefinition(commandContext, builder.ProcessDefinitionId);
            EnsureUtil.EnsureNotNull("Process definition cannot be found", "processDefinition", processDefinition);

            CheckAuthorization(commandContext, processDefinition);

            if (writeUserOperationLog)
                WriteUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, false);

            var processDefinitionId = builder.ProcessDefinitionId;

            foreach (var processInstanceId in processInstanceIds)
            {
                var historicProcessInstance = GetHistoricProcessInstance(commandContext, processInstanceId);

                EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Historic process instance cannot be found",
                    "historicProcessInstanceId", historicProcessInstance);
                EnsureHistoricProcessInstanceNotActive(historicProcessInstance);
                EnsureSameProcessDefinition(historicProcessInstance, processDefinitionId);

                var instantiationBuilder = GetProcessInstantiationBuilder(commandExecutor, processDefinitionId);
                ApplyProperties(instantiationBuilder, processDefinition, historicProcessInstance);

                var modificationBuilder = instantiationBuilder.ModificationBuilder;
                modificationBuilder.ModificationOperations = instructions;

                var variables = CollectVariables(commandContext, historicProcessInstance);
                //instantiationBuilder.SetVariables(variables);

                instantiationBuilder.Execute(builder.SkipCustomListeners, builder.SkipIoMappings);
            }
            return null;
        }

        protected internal virtual void CheckAuthorization(CommandContext commandContext,
            IProcessDefinition processDefinition)
        {
            commandContext.AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.ProcessDefinition,
                processDefinition.Key);
        }

        protected internal virtual IHistoricProcessInstance GetHistoricProcessInstance(CommandContext commandContext,
            string processInstanceId)
        {
            var historyService = commandContext.ProcessEngineConfiguration.HistoryService;
            return historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessInstanceId == processInstanceId)
                .FirstOrDefault();
        }

        protected internal virtual void EnsureSameProcessDefinition(IHistoricProcessInstance instance,
            string processDefinitionId)
        {
            if (!processDefinitionId.Equals(instance.ProcessDefinitionId))
                throw LOG.ProcessDefinitionOfHistoricInstanceDoesNotMatchTheGivenOne(instance, processDefinitionId);
        }

        protected internal virtual void EnsureHistoricProcessInstanceNotActive(IHistoricProcessInstance instance)
        {
            if (instance.EndTime == null)
                throw LOG.HistoricProcessInstanceActive(instance);
        }

        protected internal virtual ProcessInstantiationBuilderImpl GetProcessInstantiationBuilder(
            ICommandExecutor commandExecutor, string processDefinitionId)
        {
            return (ProcessInstantiationBuilderImpl) ProcessInstantiationBuilderImpl.CreateProcessInstanceById(
                commandExecutor, processDefinitionId);
        }

        protected internal virtual void ApplyProperties(ProcessInstantiationBuilderImpl instantiationBuilder,
            IProcessDefinition processDefinition, IHistoricProcessInstance processInstance)
        {
            var tenantId = processInstance.TenantId;
            //if (processDefinition.TenantId == null && !ReferenceEquals(tenantId, null))
                //instantiationBuilder.TenantId(tenantId);

            //if (!builder.WithoutBusinessKey)
                //instantiationBuilder.BusinessKey(processInstance.BusinessKey);
        }

        protected internal virtual IVariableMap CollectVariables(CommandContext commandContext,
            IHistoricProcessInstance processInstance)
        {
            IVariableMap variables = null;

            if (builder.InitialVariables)
                variables = CollectInitialVariables(commandContext, processInstance);
            else
                variables = CollectLastVariables(commandContext, processInstance);

            return variables;
        }

        protected internal virtual IVariableMap CollectInitialVariables(CommandContext commandContext,
            IHistoricProcessInstance processInstance)
        {
            var historyService = commandContext.ProcessEngineConfiguration.HistoryService;

            var startActivityInstance = ResolveStartActivityInstance(processInstance);

            var query =  historyService.CreateHistoricDetailQuery(c=>c.EventType == "VariableUpdate" && c.ExecutionId == processInstance.Id && c.ActivityInstanceId == startActivityInstance.Id);


            //IList<IHistoricDetail> historicDetails = query.Where(c => c.SequenceCounter == 1)
            //    .ToList();

            IVariableMap variables = new VariableMapImpl();
            foreach (var detail in query /*historicDetails*/)
            {
                var variableUpdate = (IHistoricVariableUpdate) detail;
                variables.PutValueTyped(variableUpdate.VariableName, variableUpdate.TypedValue);
            }

            return variables;
        }

        protected internal virtual IVariableMap CollectLastVariables(CommandContext commandContext,
            IHistoricProcessInstance processInstance)
        {
            var historyService = commandContext.ProcessEngineConfiguration.HistoryService;

            IList<IHistoricVariableInstance> historicVariables = historyService
                .CreateHistoricVariableInstanceQuery(c => c.ExecutionId == processInstance.Id)
                .ToList();
               

            IVariableMap variables = new VariableMapImpl();
            foreach (var variable in historicVariables)
                variables.PutValueTyped(variable.Name, variable.TypedValue);

            return variables;
        }

        protected internal virtual IHistoricActivityInstance ResolveStartActivityInstance(
            IHistoricProcessInstance processInstance)
        {
            var historyService = Context.ProcessEngineConfiguration.HistoryService;

            var processInstanceId = processInstance.Id;
            var startActivityId = processInstance.StartActivityId;

            EnsureUtil.EnsureNotNull("startActivityId", startActivityId);

            IList<IHistoricActivityInstance> historicActivityInstances = historyService
                .CreateHistoricActivityInstanceQuery(c => c.ProcessInstanceId == processInstanceId &&
                                                          c.ActivityId ==
                                                          startActivityId) /*.OrderPartiallyByOccurrence().Asc()*/
                .ToList();

            EnsureUtil.EnsureNotEmpty("historicActivityInstances", historicActivityInstances);

            var startActivityInstance = historicActivityInstances[0];
            return startActivityInstance;
        }
    }
}