using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Builder to execute a migration.
    /// </summary>
    public interface IMigrationPlanExecutionBuilder
    {
        /// <param name="processInstanceIds"> the process instance ids to migrate. </param>
        IMigrationPlanExecutionBuilder ProcessInstanceIds(IList<string> processInstanceIds);

        /// <param name="processInstanceIds"> the process instance ids to migrate. </param>
        IMigrationPlanExecutionBuilder ProcessInstanceIds(params string[] processInstanceIds);

        /// <param name="processInstanceQuery">
        ///     a query which selects the process instances to migrate.
        ///     Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ" />
        ///     permission.
        /// </param>
        IMigrationPlanExecutionBuilder ProcessInstanceQuery(IQueryable<IProcessInstance> processInstanceQuery);

        /// <summary>
        ///     Skips custom execution listeners when creating/removing activity instances during migration
        /// </summary>
        IMigrationPlanExecutionBuilder SkipCustomListeners();

        /// <summary>
        ///     Skips io mappings when creating/removing activity instances during migration
        /// </summary>
        IMigrationPlanExecutionBuilder SkipIoMappings();

        /// <summary>
        ///     Execute the migration synchronously.
        /// </summary>
        /// <exception cref="MigratingProcessInstanceValidationException">
        ///     if the migration plan contains
        ///     instructions that are not applicable to any of the process instances
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has not all of the following permissions
        ///     <ul>
        ///         <li>
        ///             <seealso cref="Permissions#MIGRATE_INSTANCE" /> permission on
        ///             <seealso cref="Resources#PROCESS_DEFINITION" /> for source and target
        ///         </li>
        ///     </ul>
        /// </exception>
        void Execute();

        /// <summary>
        ///     Execute the migration asynchronously as batch. The returned batch
        ///     can be used to track the progress of the migration.
        /// </summary>
        /// <returns>
        ///     the batch which executes the migration asynchronously.
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has not all of the following permissions
        ///     <ul>
        ///         <li>
        ///             <seealso cref="Permissions#MIGRATE_INSTANCE" /> permission on
        ///             <seealso cref="Resources#PROCESS_DEFINITION" /> for source and target
        ///         </li>
        ///         <li><seealso cref="Permissions#CREATE" /> permission on <seealso cref="Resources#BATCH" /></li>
        ///     </ul>
        /// </exception>
        IBatch ExecuteAsync();
    }
}