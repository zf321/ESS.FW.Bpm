using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface ICorrelationHandler
    {
        /// <summary>
        ///     Correlate the given message to either a waiting execution or a process
        ///     definition with a message start event.
        /// </summary>
        /// <param name="correlationSet">
        ///     any of its members may be <code>null</code>
        /// </param>
        /// <returns>
        ///     the matched correlation target or <code>null</code> if the message
        ///     could not be correlated.
        /// </returns>
        CorrelationHandlerResult CorrelateMessage(CommandContext commandContext, string messageName,
            CorrelationSet correlationSet);

        /// <summary>
        ///     Correlate the given message to all waiting executions and all process
        ///     definitions which have a message start event.
        /// </summary>
        /// <param name="correlationSet">
        ///     any of its members may be <code>null</code>
        /// </param>
        /// <returns>
        ///     all matched correlation targets or an empty List if the message
        ///     could not be correlated.
        /// </returns>
        IList<CorrelationHandlerResult> CorrelateMessages(CommandContext commandContext, string messageName,
            CorrelationSet correlationSet);

        /// <summary>
        ///     Correlate the given message to process definitions with a message start
        ///     event.
        /// </summary>
        /// <param name="correlationSet">
        ///     any of its members may be <code>null</code>
        /// </param>
        /// <returns>
        ///     the matched correlation targets or an empty list if the message
        ///     could not be correlated.
        /// </returns>
        IList<CorrelationHandlerResult> CorrelateStartMessages(CommandContext commandContext, string messageName,
            CorrelationSet correlationSet);
    }
}