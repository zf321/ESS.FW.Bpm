using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Service exposing information about ongoing and past process instances.  This is different
    ///     from the runtime information in the sense that this runtime information only contains
    ///     the actual runtime state at any given moment and it is optimized for runtime
    ///     process execution performance.  The history information is optimized for easy
    ///     querying and remains permanent in the persistent storage.
    ///     
    ///      
    ///     
    /// </summary>
    public interface IHistoryService
    {
        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricProcessInstance" />s.
        /// </summary>
        IQueryable<IHistoricProcessInstance> CreateHistoricProcessInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression =null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricActivityInstance" />s.
        /// </summary>
        IQueryable<IHistoricActivityInstance> CreateHistoricActivityInstanceQuery(
            Expression<Func<HistoricActivityInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Query for the number of historic activity instances aggregated by activities of a single process definition.
        /// </summary>
        IQueryable<IHistoricActivityStatistics> CreateHistoricActivityStatisticsQuery(string processDefinitionId);
        IQueryable<IHistoricActivityStatistics> CreateHistoricActivityStatisticsQuery(Expression<Func<HistoricActivityStatisticsImpl,bool>> expression = null);

        /// <summary>
        ///     Query for the number of historic case activity instances aggregated by case activities of a single case definition.
        /// </summary>
      // IQueryable<IHistoricCaseActivityStatistics> CreateHistoricCaseActivityStatisticsQuery(string caseDefinitionId);
        IQueryable<IHistoricCaseActivityStatistics> CreateHistoricCaseActivityStatisticsQuery(Expression<Func<HistoricCaseActivityStatisticsImpl, bool>> expression = null);
        
        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricTaskInstance" />s.
        /// </summary>
        IQueryable<IHistoricTaskInstance> CreateHistoricTaskInstanceQuery(Expression<Func<History.Impl.Event.HistoricTaskInstanceEventEntity, bool>> expression = null);

        /// <summary>
        /// 需要子类查询使用泛型
        ///     Creates a new programmatic query to search for <seealso cref="HistoricDetail" />s.
        /// </summary>
        IQueryable<IHistoricDetail> CreateHistoricDetailQuery(Expression<Func<HistoricDetailEventEntity, bool>> expression = null);
        IQueryable<IHistoricDetail> CreateHistoricDetailQuery<T>(Expression<Func<T, bool>> expression = null) where T: HistoricDetailEventEntity,new();
        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricVariableInstance" />s.
        /// </summary>
        IQueryable<IHistoricVariableInstance> CreateHistoricVariableInstanceQuery(Expression<Func<HistoricVariableInstanceEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="UserOperationLogEntry" /> instances.
        /// </summary>
        IQueryable<IUserOperationLogEntry> CreateUserOperationLogQuery(Expression<Func<UserOperationLogEntryEventEntity, bool>> expression =null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricIncident historic incidents" />.
        /// </summary>
        IQueryable<IHistoricIncident> CreateHistoricIncidentQuery(Expression<Func<HistoricIncidentEntity,bool>>  expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricIdentityLinkLog historic identity links" />.
        /// </summary>
        IQueryable<IHistoricIdentityLinkLog> CreateHistoricIdentityLinkLogQuery(Expression<Func<HistoricIdentityLinkLogEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricCaseInstance" />s.
        /// </summary>
        IQueryable<IHistoricCaseInstance> CreateHistoricCaseInstanceQuery(Expression<Func<HistoricCaseInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricCaseActivityInstance" />s.
        /// </summary>
        IQueryable<IHistoricCaseActivityInstance> CreateHistoricCaseActivityInstanceQuery(Expression<Func<HistoricCaseActivityInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricDecisionInstance" />s.
        ///     If the user has no <seealso cref="Permissions#READ_HISTORY" /> permission on
        ///     <seealso cref="Resources#DECISION_DEFINITION" />
        ///     then the result of the query is empty.
        /// </summary>
        IQueryable<IHistoricDecisionInstance> CreateHistoricDecisionInstanceQuery(Expression<Func<HistoricDecisionInstanceEntity, bool>> expression = null);
        IJob FindHistoryCleanupJob();

        /// <summary>
        ///     Deletes historic task instance.  This might be useful for tasks that are
        ///     <seealso cref="ITaskService#newTask() dynamically created" /> and then
        ///     <seealso cref="ITaskService#complete(String) completed" />.
        ///     If the historic task instance doesn't exist, no exception is thrown and the
        ///     method returns normal.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void DeleteHistoricTaskInstance(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null);//string taskId
        void DeleteHistoricTaskInstance(string taskId);

        /// <summary>
        ///     Deletes historic process instance. All historic activities, historic task and
        ///     historic details (variable updates, form properties) are deleted as well.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void DeleteHistoricProcessInstance(string processInstanceId);

        /// <summary>
        ///     Deletes historic process instances. All historic activities, historic task and
        ///     historic details (variable updates, form properties) are deleted as well.
        /// </summary>
        /// <exception cref="BadUserRequestException">
        ///     when no process instances is found with the given ids or ids are null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void DeleteHistoricProcessInstances(IList<string> processInstanceIds);

        /// <summary>
        ///     Deletes historic process instances asynchronously. All historic activities, historic task and
        ///     historic details (variable updates, form properties) are deleted as well.
        /// </summary>
        /// <exception cref="BadUserRequestException">
        ///     when no process instances is found with the given ids or ids are null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     or no <seealso cref="Permissions#CREATE" /> permission on <seealso cref="Resources#BATCH" />.
        /// </exception>
        IBatch DeleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason);

        /// <summary>
        ///     Deletes historic process instances asynchronously based on query. All historic activities, historic task and
        ///     historic details (variable updates, form properties) are deleted as well.
        /// </summary>
        /// <exception cref="BadUserRequestException">
        ///     when no process instances is found with the given ids or ids are null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     or no <seealso cref="Permissions#CREATE" /> permission on <seealso cref="Resources#BATCH" />.
        /// </exception>
        IBatch DeleteHistoricProcessInstancesAsync(IQueryable<IHistoricProcessInstance> query, string deleteReason);

        /// <summary>
        ///     Deletes historic process instances asynchronously based on query and a list of process instances. Query result and
        ///     list of ids will be merged.
        ///     All historic activities, historic task and historic details (variable updates, form properties) are deleted as
        ///     well.
        /// </summary>
        /// <exception cref="BadUserRequestException">
        ///     when no process instances is found with the given ids or ids are null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     or no <seealso cref="Permissions#CREATE" /> permission on <seealso cref="Resources#BATCH" />.
        /// </exception>
        IBatch DeleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, IQueryable<IHistoricProcessInstance> query,
            string deleteReason);

        /// <summary>
        ///     Deletes a user operation log entry. Does not cascade to any related entities.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void DeleteUserOperationLogEntry(string entryId);

        /// <summary>
        ///     Deletes historic case instance. All historic case activities, historic task and
        ///     historic details are deleted as well.
        /// </summary>
        void DeleteHistoricCaseInstance(string caseInstanceId);

        /// <summary>
        ///     Deletes historic decision instances of a decision definition. All historic
        ///     decision inputs and outputs are deleted as well.
        /// </summary>
        /// @deprecated Note that this method name is not expressive enough, because it is also possible to delete the historic
        /// decision instance by the instance id. Therefore use
        /// <seealso cref="#deleteHistoricDecisionInstanceByDefinitionId" />
        /// instead
        /// to delete the historic decision instance by the definition id.
        /// <param name="decisionDefinitionId">
        ///     the id of the decision definition
        /// </param>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        [Obsolete(
             "Note that this method name is not expressive enough, because it is also possible to delete the historic")]
        void DeleteHistoricDecisionInstance(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null);//string decisionDefinitionId);

        /// <summary>
        ///     Deletes historic decision instances of a decision definition. All historic
        ///     decision inputs and outputs are deleted as well.
        /// </summary>
        /// <param name="decisionDefinitionId">
        ///     the id of the decision definition
        /// </param>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        void DeleteHistoricDecisionInstanceByDefinitionId(string decisionDefinitionId);


        /// <summary>
        ///     Deletes historic decision instances by its id. All historic
        ///     decision inputs and outputs are deleted as well.
        /// </summary>
        /// <param name="historicDecisionInstanceId">
        ///     the id of the historic decision instance
        /// </param>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE_HISTORY" /> permission on
        ///     <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        void DeleteHistoricDecisionInstanceByInstanceId(string historicDecisionInstanceId);
        void DeleteHistoricDecisionInstanceByInstanceId(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null);//string historicDecisionInstanceId);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricProcessInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricProcessInstance> CreateNativeHistoricProcessInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricTaskInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricTaskInstance> CreateNativeHistoricTaskInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricActivityInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricActivityInstance> CreateNativeHistoricActivityInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricCaseInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricCaseInstance> CreateNativeHistoricCaseInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricCaseActivityInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricCaseActivityInstance> CreateNativeHistoricCaseActivityInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     creates a native query to search for <seealso cref="HistoricDecisionInstance" />s via SQL
        /// </summary>
        IQueryable<IHistoricDecisionInstance> CreateNativeHistoricDecisionInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to search for <seealso cref="HistoricJobLog historic job logs" />.
        ///     
        /// </summary>
        IQueryable<IHistoricJobLog> CreateHistoricJobLogQuery(Expression<Func<HistoricJobLogEventEntity,bool>> expression = null);

        /// <summary>
        ///     Returns the full stacktrace of the exception that occurs when the
        ///     historic job log with the given id was last executed. Returns null
        ///     when the historic job log has no exception stacktrace.
        /// </summary>
        /// <param name="historicJobLogId"> id of the historic job log, cannot be null. </param>
        /// <exception cref="ProcessEngineException">
        ///     when no historic job log exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#READ_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     
        /// </exception>
        string GetHistoricJobLogExceptionStacktrace(string historicJobLogId);

        /// <summary>
        ///     Creates a new programmatic query to create a historic process instance report.
        ///     
        /// </summary>
        IHistoricProcessInstanceReport CreateHistoricProcessInstanceReport(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a new programmatic query to create a historic task instance report.
        ///     
        /// </summary>
        IHistoricTaskInstanceReport CreateHistoricTaskInstanceReport(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Creates a query to search for <seealso cref="IHistoricBatch" /> instances.
        ///     
        /// </summary>
        IQueryable<IHistoricBatch> CreateHistoricBatchQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Deletes a historic batch instance. All corresponding historic job logs are deleted as well;
        ///     
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#DELETE" /> permission on <seealso cref="Resources#BATCH" />
        /// </exception>
        void DeleteHistoricBatch(string id);


        /// <summary>
        ///     Query for the statistics of DRD evaluation.
        /// </summary>
        /// <param name="decisionRequirementsDefinitionId">
        ///     - id of decision requirement definition
        ///     
        /// </param>
        IQueryable<IHistoricDecisionInstanceStatistics> CreateHistoricDecisionInstanceStatisticsQuery(string decisionRequirementsDefinitionId);
        //IQueryable<IHistoricDecisionInstanceStatistics> CreateHistoricDecisionInstanceStatisticsQuery(
        //    Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression);
        //string decisionRequirementsDefinitionId);

        /// <summary>
        ///     Creates a new programmatic query to search for
        ///     <seealso cref="HistoricExternalTaskLog historic external task logs" />.
        ///     
        /// </summary>
        IQueryable<IHistoricExternalTaskLog> CreateHistoricExternalTaskLogQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null);

        /// <summary>
        ///     Returns the full error details that occurs when the
        ///     historic external task log with the given id was last executed. Returns null
        ///     when the historic external task log contains no error details.
        /// </summary>
        /// <param name="historicExternalTaskLogId"> id of the historic external task log, cannot be null. </param>
        /// <exception cref="ProcessEngineException">
        ///     when no historic external task log exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#READ_HISTORY" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     
        /// </exception>
        string GetHistoricExternalTaskLogErrorDetails(string historicExternalTaskLogId);

        /// <summary>
        /// Schedules history cleanup job at batch window start time. The job will delete historic data for finished processes
        /// taking into account <seealso cref="ProcessDefinition#getHistoryTimeToLive()"/> value.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> </exception>
        /// <returns> history cleanup job. Job id can be used to check job logs, incident etc. </returns>
        IJob CleanUpHistoryAsync();

        /// <summary>
        /// Schedules history cleanup job. The job will delete historic data for finished processes
        /// taking into account <seealso cref="ProcessDefinition#getHistoryTimeToLive()"/> value.
        /// </summary>
        /// <param name="immediatelyDue"> must be true if cleanup must be scheduled at once, otherwise is will be scheduled according to configured batch window </param>
        /// <exception cref="AuthorizationException">
        ///      If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> </exception>
        /// <returns> history cleanup job. Job id can be used to check job logs, incident etc.
        ///  </returns>
        IJob CleanUpHistoryAsync(bool immediatelyDue);


        /// <summary>
        /// Deletes decision instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
        /// of given decision instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
        /// </summary>
        /// <param name="decisionInstanceIds"> list of decision instance ids for removal.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        void DeleteHistoricDecisionInstancesBulk(IList<string> decisionInstanceIds);

        /// <summary>
        /// Deletes historic process instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
        /// of given process instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
        /// </summary>
        /// <param name="processInstanceIds"> list of process instance ids for removal
        /// </param>
        /// <exception cref="BadUserRequestException">
        ///          when no process instances are found with the given ids or ids are null or when some of the process instances are not finished yet </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void DeleteHistoricProcessInstancesBulk(IList<string> processInstanceIds);

        /// <summary>
        /// Deletes historic case instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
        /// of given case instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
        /// </summary>
        /// <param name="caseInstanceIds"> list of case instance ids for removal </param>
        void DeleteHistoricCaseInstancesBulk(IList<string> caseInstanceIds);

        IQueryable<IHistoricProcessInstance> CreateHistoricProcessInstanceQuerySuperProcessInstanceId(string processInstanceId, Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression=null);

    }
}