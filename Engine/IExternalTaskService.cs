using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Service that provides access to <seealso cref="IExternalTask" /> instances. External tasks
    ///     represent work items that are processed externally and independently of the process
    ///     engine.
    ///     
    ///     
    /// </summary>
    public interface IExternalTaskService
    {
        /// <summary>
        ///     Calls method fetchAndLock(maxTasks, workerId, usePriority), where usePriority is false.
        /// </summary>
        /// <param name="maxTasks"> the maximum number of tasks to return </param>
        /// <param name="workerId"> the id of the worker to lock the tasks for </param>
        /// <returns> a builder to define and execute an external ITask fetching operation </returns>
        /// <seealso cref=
        /// <seealso cref="IExternalTaskService#fetchAndLock(int, java.lang.String, boolean)" />
        /// .
        /// </seealso>
        IQueryable<IExternalTask> FetchAndLock(int maxTasks, string workerId);


        /// <summary>
        ///     <para>
        ///         Defines fetching of external tasks by using a fluent builder.
        ///         The following parameters must be specified:
        ///         A worker id, a maximum number of tasks to fetch and a flag that indicates
        ///         whether priority should be regarded or not.
        ///         The builder allows to specify multiple topics to fetch tasks for and
        ///         individual lock durations. For every topic, variables can be fetched
        ///         in addition.Is the priority enabled the tasks with the highest priority are fetched.
        ///     </para>
        ///     <para>
        ///         Returned tasks are locked for the given worker until
        ///         <code>now + lockDuration</code> expires.
        ///         Locked tasks cannot be fetched or completed by other workers. When the lock time has expired,
        ///         a ITask may be fetched and locked by other workers.
        ///     </para>
        ///     <para>
        ///         Returns at most <code>maxTasks</code> tasks. The tasks are arbitrarily
        ///         distributed among the specified topics. Example: Fetching 10 tasks of topics
        ///         "a"/"b"/"c" may return 3/3/4 tasks, or 10/0/0 tasks, etc.
        ///     </para>
        ///     <para>
        ///         May return less than <code>maxTasks</code> tasks, if there exist not enough
        ///         unlocked tasks matching the provided topics or if parallel fetching by other workers
        ///         results in locking failures.
        ///     </para>
        ///     <para>
        ///         Returns only tasks that the currently authenticated user has at least one
        ///         permission out of all of the following groups for:
        ///         <ul>
        ///             <li><seealso cref="Permissions#READ" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///             <li><seealso cref="Permissions#READ_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///         </ul>
        ///         <ul>
        ///             <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///             <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="maxTasks"> the maximum number of tasks to return </param>
        /// <param name="workerId"> the id of the worker to lock the tasks for </param>
        /// <param name="usePriority"> the flag to enable the priority fetching mechanism </param>
        /// <returns> a builder to define and execute an external ITask fetching operation </returns>
        IQueryable<IExternalTask> FetchAndLock(int maxTasks, string workerId, bool usePriority=false);

        /// <summary>
        ///     <para>
        ///         Completes an external ITask on behalf of a worker. The given ITask must be
        ///         assigned to the worker.
        ///     </para>
        /// </summary>
        /// <param name="externalTaskId"> the id of the external to complete </param>
        /// <param name="workerId"> the id of the worker that completes the ITask </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ITask is assigned to a different worker </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void Complete(string externalTaskId, string workerId);

        /// <summary>
        ///     <para>
        ///         Completes an external ITask on behalf of a worker and submits variables
        ///         to the process instance before continuing execution. The given ITask must be
        ///         assigned to the worker.
        ///     </para>
        /// </summary>
        /// <param name="externalTaskId"> the id of the external to complete </param>
        /// <param name="workerId"> the id of the worker that completes the ITask </param>
        /// <param name="variables">
        ///     a map of variables to set on the execution (non-local)
        ///     the external ITask is assigned to
        /// </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ITask is assigned to a different worker </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void Complete(string externalTaskId, string workerId, IDictionary<string, object> variables);


        /// <summary>
        ///     <para>
        ///         Signals that an external ITask could not be successfully executed.
        ///         The ITask must be assigned to the given worker. The number of retries left can be specified. In addition, a
        ///         timeout can be
        ///         provided, such that the ITask cannot be fetched before <code>now + retryTimeout</code> again.
        ///     </para>
        ///     <para>
        ///         If <code>retries</code> is 0, an incident with the given error message is created. The incident gets resolved,
        ///         once the number of retries is increased again.
        ///     </para>
        /// </summary>
        /// <param name="externalTaskId"> the id of the external ITask to report a failure for </param>
        /// <param name="workerId"> the id of the worker that reports the failure </param>
        /// <param name="errorMessage">
        ///     short error message related to this failure. This message can be retrieved via
        ///     <seealso cref="IExternalTask#getErrorMessage()" /> and is used as the incident message in case <code>retries</code>
        ///     is <code>null</code>.
        ///     May be <code>null</code>.
        /// </param>
        /// <param name="retries">
        ///     the number of retries left. External tasks with 0 retries cannot be fetched anymore unless
        ///     the number of retries is increased via API. Must be >= 0.
        /// </param>
        /// <param name="retryTimeout">
        ///     the timeout before the ITask can be fetched again. Must be >= 0.
        /// </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ITask is assigned to a different worker </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void HandleFailure(string externalTaskId, string workerId, string errorMessage, int retries, long retryTimeout);

        /// <summary>
        ///     <para>
        ///         Signals that an external ITask could not be successfully executed.
        ///         The ITask must be assigned to the given worker. The number of retries left can be specified. In addition, a
        ///         timeout can be
        ///         provided, such that the ITask cannot be fetched before <code>now + retryTimeout</code> again.
        ///     </para>
        ///     <para>
        ///         If <code>retries</code> is 0, an incident with the given error message is created. The incident gets resolved,
        ///         once the number of retries is increased again.
        ///     </para>
        /// </summary>
        /// <param name="externalTaskId"> the id of the external ITask to report a failure for </param>
        /// <param name="workerId"> the id of the worker that reports the failure </param>
        /// <param name="errorMessage">
        ///     short error message related to this failure. This message can be retrieved via
        ///     <seealso cref="IExternalTask#getErrorMessage()" /> and is used as the incident message in case <code>retries</code>
        ///     is <code>null</code>.
        ///     May be <code>null</code>.
        /// </param>
        /// <param name="errorDetails">
        ///     full error message related to this failure. This message can be retrieved via
        ///     <seealso cref="IExternalTaskService#getExternalTaskErrorDetails(String)" /> ()}
        /// </param>
        /// <param name="retries">
        ///     the number of retries left. External tasks with 0 retries cannot be fetched anymore unless
        ///     the number of retries is increased via API. Must be >= 0.
        /// </param>
        /// <param name="retryTimeout">
        ///     the timeout before the ITask can be fetched again. Must be >= 0.
        /// </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ITask is assigned to a different worker </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void HandleFailure(string externalTaskId, string workerId, string errorMessage, string errorDetails, int retries,
            long retryTimeout);

        /// <summary>
        ///     <para>
        ///         Signals that an business error appears, which should be handled by the process engine.
        ///         The ITask must be assigned to the given worker. The error will be propagated to the next error handler.
        ///         Is no existing error handler for the given bpmn error the activity instance of the external ITask
        ///         ends.
        ///     </para>
        /// </summary>
        /// <param name="externalTaskId"> the id of the external ITask to report a bpmn error </param>
        /// <param name="workerId"> the id of the worker that reports the bpmn error </param>
        /// <param name="errorCode">
        ///     the error code of the corresponding bmpn error
        ///     
        /// </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ITask is assigned to a different worker </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void HandleBpmnError(string externalTaskId, string workerId, string errorCode);

        /// <summary>
        ///     Unlocks an external ITask instance.
        /// </summary>
        /// <param name="externalTaskId"> the id of the ITask to unlock </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void Unlock(string externalTaskId);

        /// <summary>
        ///     Sets the retries for an external ITask. If the new value is 0, a new incident with a <code>null</code>
        ///     message is created. If the old value is 0 and the new value is greater than 0, an existing incident
        ///     is resolved.
        /// </summary>
        /// <param name="externalTaskId"> the id of the ITask to set the </param>
        /// <param name="retries"> </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void SetRetries(string externalTaskId, int retries);


        /// <summary>
        /// Sets the retries for external tasks. If the new value is 0, a new incident with a <code>null</code>
        /// message is created. If the old value is 0 and the new value is greater than 0, an existing incident
        /// is resolved.
        /// </summary>
        /// <param name="externalTaskIds"> the ids of the tasks to set the </param>
        /// <param name="retries"> </param>
        /// <exception cref="NotFoundException"> if no external task with one of the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ids are null or the number of retries is negative </exception>
        /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
        ///   <ul>
        ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
        ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
        ///   </ul> </exception>
         void SetRetries(IList<string> externalTaskIds, int retries);


        /// <summary>
        ///     Sets the priority for an external ITask.
        /// </summary>
        /// <param name="externalTaskId"> the id of the ITask to set the </param>
        /// <param name="priority"> the new priority of the ITask </param>
        /// <exception cref="NotFoundException"> if no external ITask with the given id exists </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess any of the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions#UPDATE" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///         <li><seealso cref="Permissions#UPDATE_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///     </ul>
        /// </exception>
        void SetPriority(string externalTaskId, long priority);

        /// <summary>
        ///     <para>
        ///         Queries for tasks that the currently authenticated user has at least one
        ///         of the following permissions for:
        ///         <ul>
        ///             <li><seealso cref="Permissions#READ" /> on <seealso cref="Resources#PROCESS_INSTANCE" /></li>
        ///             <li><seealso cref="Permissions#READ_INSTANCE" /> on <seealso cref="Resources#PROCESS_DEFINITION" /></li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <returns>
        ///     a new <seealso cref="IQueryable<IExternalTask>" /> that can be used to dynamically
        ///     query for external tasks.
        /// </returns>
        IQueryable<IExternalTask> CreateExternalTaskQuery(Expression<Func<ExternalTaskEntity, bool>> expression =null);

        /// <summary>
        ///     Returns the full error details that occurred while running external ITask
        ///     with the given id. Returns null when the external ITask has no error details.
        /// </summary>
        /// <param name="externalTaskId">
        ///     id of the external ITask, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     When no external ITask exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#READ" /> permission on <seealso cref="Resources#PROCESS_INSTANCE" />
        ///     or no <seealso cref="Permissions#READ_INSTANCE" /> permission on <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     
        /// </exception>
        string GetExternalTaskErrorDetails(string externalTaskId);


        /// <summary>
        /// Sets the retries for external tasks asynchronously as batch. The returned batch
        /// can be used to track the progress. If the new value is 0, a new incident with a <code>null</code>
        /// message is created. If the old value is 0 and the new value is greater than 0, an existing incident
        /// is resolved.
        /// 
        /// </summary>
        /// <returns> the batch
        /// </returns>
        /// <param name="externalTaskIds"> the ids of the tasks to set the </param>
        /// <param name="retries"> </param>
        /// <param name="externalTaskQuery"> a query which selects the external tasks to set the retries for. </param>
        /// <exception cref="NotFoundException"> if no external task with one of the given id exists </exception>
        /// <exception cref="BadUserRequestException"> if the ids are null or the number of retries is negative </exception>
        /// <exception cref="AuthorizationException"> thrown if the current user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#BATCH"/>
        ///    or does not possess any of the following permissions:
        ///   <ul>
        ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
        ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
        ///   </ul> </exception>
         IBatch SetRetriesAsync(IList<string> externalTaskIds, IQueryable<IExternalTask> externalTaskQuery, int retries);

    }
}