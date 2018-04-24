using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;

namespace ESS.FW.Bpm.Engine.Runtime
{
    public interface IModificationBuilder : IInstantiationBuilder<IModificationBuilder>
    {

        /// <summary>
        /// <para><i>Submits the instruction:</i></para>
        /// 
        /// <para>Cancel all instances of the given activity in an arbitrary order, which are:
        /// <ul>
        ///   <li>activity instances of that activity
        ///   <li>transition instances entering or leaving that activity
        /// </ul></para>
        /// 
        /// <para>The cancellation order of the instances is arbitrary</para>
        /// </summary>
        /// <param name="activityId"> the activity for which all instances should be cancelled </param>
        IModificationBuilder CancelAllForActivity(string activityId);

        /// <param name="processInstanceIds"> the process instance ids to modify. </param>
        IModificationBuilder SetProcessInstanceIds(IList<string> processInstanceIds);

        /// <param name="processInstanceIds"> the process instance ids to modify. </param>
        IModificationBuilder SetProcessInstanceIds(params string[] processInstanceIds);

        /// <param name="processInstanceQuery"> a query which selects the process instances to modify.
        ///   Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ"/> permission. </param>
        IModificationBuilder SetProcessInstanceQuery(IQueryable<IProcessInstance> processInstanceQuery);

        /// <summary>
        /// Skips custom execution listeners when creating/removing activity instances during modification
        /// </summary>
        IModificationBuilder SetSkipCustomListeners();

        /// <summary>
        /// Skips io mappings when creating/removing activity instances during modification
        /// </summary>
        IModificationBuilder SetSkipIoMappings();

        /// <summary>
        /// Execute the modification synchronously.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///   if the user has not all of the following permissions
        ///   <ul>
        ///      <li>if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
        ///   </ul> </exception>
        void Execute();

        /// <summary>
        /// Execute the modification asynchronously as batch. The returned batch
        /// can be used to track the progress of the modification.
        /// </summary>
        /// <returns> the batch which executes the modification asynchronously.
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///   if the user has not all of the following permissions
        ///   <ul>
        ///     <li><seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#BATCH"/></li>
        ///   </ul> </exception>
        IBatch ExecuteAsync();
    }

}
