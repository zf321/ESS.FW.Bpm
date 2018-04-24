using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    public class CorrelateStartMessageCmd : AbstractCorrelateMessageCmd, ICommand<IProcessInstance>
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        public CorrelateStartMessageCmd(MessageCorrelationBuilderImpl messageCorrelationBuilderImpl)
            : base(messageCorrelationBuilderImpl)
        {
        }
        
        public virtual IProcessInstance Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("messageName", MessageName);
            
            var correlationHandler = context.Impl.Context.ProcessEngineConfiguration.CorrelationHandler;
            var correlationSet = new CorrelationSet(Builder);

            IList<CorrelationHandlerResult> correlationResults =
                commandContext.RunWithoutAuthorization(()=> correlationHandler.CorrelateStartMessages(commandContext, MessageName,correlationSet));

            if (correlationResults.Count == 0)
            {
                throw new MismatchingMessageCorrelationException(MessageName,
                    "No process definition matches the parameters");
            }
            if (correlationResults.Count > 1)
            {
                throw Log.ExceptionCorrelateMessageToSingleProcessDefinition(MessageName, correlationResults.Count,
                    correlationSet);
            }
            var correlationResult = correlationResults[0];

            CheckAuthorization(correlationResult);

            var processInstance = InstantiateProcess(commandContext, correlationResult);
            return processInstance;
        }
        
    }
}