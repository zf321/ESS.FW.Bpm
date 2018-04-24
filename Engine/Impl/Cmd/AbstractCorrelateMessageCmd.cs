using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    ///     
    ///     
    /// </summary>
    public abstract class AbstractCorrelateMessageCmd
    {
        protected internal readonly MessageCorrelationBuilderImpl Builder;

        protected internal readonly string MessageName;

        /// <summary>
        ///     Initialize the command with a builder
        /// </summary>
        /// <param name="builder"> </param>
        protected internal AbstractCorrelateMessageCmd(MessageCorrelationBuilderImpl builder)
        {
            this.Builder = builder;
            MessageName = builder.MessageName;
        }

        protected internal virtual void TriggerExecution(CommandContext commandContext,
            CorrelationHandlerResult correlationResult)
        {
            var executionId = correlationResult.ExecutionEntity.Id;

            var command = new MessageEventReceivedCmd(MessageName, executionId, Builder.PayloadProcessInstanceVariables,
                Builder.ExclusiveCorrelation);
            command.Execute(commandContext);
        }

        protected internal virtual IProcessInstance InstantiateProcess(CommandContext commandContext,
            CorrelationHandlerResult correlationResult)
        {
            var processDefinitionEntity = correlationResult.ProcessDefinitionEntity;
            ActivityImpl messageStartEvent =(ActivityImpl) processDefinitionEntity.FindActivity(correlationResult.StartEventActivityId);
            ExecutionEntity processInstance = (ExecutionEntity)processDefinitionEntity.CreateProcessInstance(Builder.BusinessKey, messageStartEvent);
            processInstance.Start(Builder.PayloadProcessInstanceVariables);

            return processInstance;
        }

        protected internal virtual void CheckAuthorization(CorrelationHandlerResult correlation)
        {
            var commandContext = context.Impl.Context.CommandContext;

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                if (MessageCorrelationResultType.Execution.Equals(correlation.ResultType))
                {
                    var execution = correlation.ExecutionEntity;
                    checker.CheckUpdateProcessInstanceById(execution.ProcessInstanceId);
                }
                else
                {
                    var definition = correlation.ProcessDefinitionEntity;

                    checker.CheckCreateProcessInstance(definition);
                }
        }

        protected internal virtual IMessageCorrelationResult CreateMessageCorrelationResult(
            CommandContext commandContext, CorrelationHandlerResult handlerResult)
        {
            var result = new MessageCorrelationResultImpl(handlerResult);
            if (MessageCorrelationResultType.Execution.Equals(handlerResult.ResultType))
            {
                TriggerExecution(commandContext, handlerResult);
            }
            else
            {
                var instance = InstantiateProcess(commandContext, handlerResult);
                result.ProcessInstance = instance;
            }
            return result;
        }
    }
}