using System.Collections.Generic;
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
    public class CorrelateAllMessageCmd : AbstractCorrelateMessageCmd, ICommand<IList<IMessageCorrelationResult>>
    {
        /// <summary>
        ///     Initialize the command with a builder
        /// </summary>
        /// <param name="messageCorrelationBuilderImpl"> </param>
        public CorrelateAllMessageCmd(MessageCorrelationBuilderImpl messageCorrelationBuilderImpl)
            : base(messageCorrelationBuilderImpl)
        {
        }
        
        public virtual IList<IMessageCorrelationResult> Execute(CommandContext commandContext)
        {
            //EnsureUtil.EnsureAtLeastOneNotNull("At least one of the following correlation criteria has to be present: " 
            //    + "messageName, businessKey, correlationKeys, processInstanceId", 
            //    MessageName, Builder.BusinessKey, Builder.CorrelationProcessInstanceVariables, Builder.ProcessInstanceId);
            
            var correlationHandler = context.Impl.Context.ProcessEngineConfiguration.CorrelationHandler;
            var correlationSet = new CorrelationSet(Builder);
            IList<CorrelationHandlerResult> correlationResults =
                commandContext.RunWithoutAuthorization(()=> correlationHandler.CorrelateMessages(commandContext, MessageName, correlationSet));

            // check authorization
            foreach (var correlationResult in correlationResults)
            {
                CheckAuthorization(correlationResult);
            }

            IList<IMessageCorrelationResult> results = new List<IMessageCorrelationResult>();
            foreach (var correlationResult in correlationResults)
            {
                results.Add(CreateMessageCorrelationResult(commandContext, correlationResult));
            }

            return null;
        }
        
    }
}