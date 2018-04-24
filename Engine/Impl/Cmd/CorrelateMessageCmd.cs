using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class CorrelateMessageCmd : AbstractCorrelateMessageCmd, ICommand<IMessageCorrelationResult>
    {
        /// <summary>
        ///     Initialize the command with a builder
        /// </summary>
        /// <param name="messageCorrelationBuilderImpl"> </param>
        public CorrelateMessageCmd(MessageCorrelationBuilderImpl messageCorrelationBuilderImpl)
            : base(messageCorrelationBuilderImpl)
        {
        }
        
        public virtual IMessageCorrelationResult Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureAtLeastOneNotNull("At least one of the following correlation criteria has to be present: " + "messageName, businessKey, correlationKeys, processInstanceId", MessageName, Builder.BusinessKey, Builder.CorrelationProcessInstanceVariables, Builder.processInstanceId);

            var correlationHandler = context.Impl.Context.ProcessEngineConfiguration.CorrelationHandler;
            var correlationSet = new CorrelationSet(Builder);
            CorrelationHandlerResult correlationResult =
                commandContext.RunWithoutAuthorization(()=> correlationHandler.CorrelateMessage(commandContext,MessageName, correlationSet));

            if (correlationResult == null)
            {
                throw new MismatchingMessageCorrelationException(MessageName,
                    "No process definition or execution matches the parameters");
            }

            // check authorization
            CheckAuthorization(correlationResult);

            return CreateMessageCorrelationResult(commandContext, correlationResult);
        }
        
    }
}