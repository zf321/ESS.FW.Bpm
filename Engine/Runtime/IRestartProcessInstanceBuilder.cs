using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.History;

namespace ESS.FW.Bpm.Engine.Runtime
{
    

    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>

    public interface IRestartProcessInstanceBuilder : IInstantiationBuilder<IRestartProcessInstanceBuilder>
    {

        /// <param name="query"> a query which selects the historic process instances to restart.
        ///   Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ_HISTORY"/> permission. </param>
        IRestartProcessInstanceBuilder SetHistoricProcessInstanceQuery(IQueryable<IHistoricProcessInstance> query);

        /// <param name="processInstanceIds"> the process instance ids to restart. </param>
        IRestartProcessInstanceBuilder SetProcessInstanceIds(params string[] processInstanceIds);

        /// <param name="processInstanceIds"> the process instance ids to restart. </param>
        IRestartProcessInstanceBuilder SetProcessInstanceIds(IList<string> processInstanceIds);

        /// <summary>
        /// Sets the initial set of variables during restart. By default, the last set of variables is used
        /// </summary>
        IRestartProcessInstanceBuilder InitialSetOfVariables();

        /// <summary>
        /// Does not take over the business key of the historic process instance
        /// </summary>
        IRestartProcessInstanceBuilder SetWithoutBusinessKey();

        /// <summary>
        /// Skips custom execution listeners when creating activity instances during restart
        /// </summary>
        IRestartProcessInstanceBuilder SetSkipCustomListeners();

        /// <summary>
        /// Skips io mappings when creating activity instances during restart
        /// </summary>
        IRestartProcessInstanceBuilder SetSkipIoMappings();

        /// <summary>
        /// Executes the restart synchronously.
        /// </summary>
        void Execute();

        /// <summary>
        /// Executes the restart asynchronously as batch. The returned batch
        /// can be used to track the progress of the restart.
        /// </summary>
        /// <returns> the batch which executes the restart asynchronously.
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///   if the user has not all of the following permissions
        ///   <ul>
        ///     <li><seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#BATCH"/></li>
        ///   </ul> </exception>
        IBatch ExecuteAsync();

    }
}
