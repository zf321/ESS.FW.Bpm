using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.oplog;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{
    /// <summary>
    ///     <para>
    ///         The producer for history events. The history event producer is
    ///         responsible for extracting data from the runtime structures
    ///         (Executions, Tasks, ...) and adding the data to a <seealso cref="HistoryEvent" />.
    ///         
    ///         
    ///         
    ///     </para>
    /// </summary>
    public interface IHistoryEventProducer
    {
        // Process instances //////////////////////////////////////

        /// <summary>
        ///     Creates the history event fired when a process instances is <strong>created</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateProcessInstanceStartEvt(IDelegateExecution execution);

        /// <summary>
        ///     Creates the history event fired when a process instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="execution"> the process instance </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateProcessInstanceUpdateEvt(IDelegateExecution execution);

        /// <summary>
        ///     Creates the history event fired when a process instance is <strong>migrated</strong>.
        /// </summary>
        /// <param name="execution"> the process instance </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateProcessInstanceMigrateEvt(IDelegateExecution execution);

        /// <summary>
        ///     Creates the history event fired when a process instance is <strong>ended</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateProcessInstanceEndEvt(IDelegateExecution execution);

        // Activity instances /////////////////////////////////////

        /// <summary>
        ///     Creates the history event fired when an activity instance is <strong>started</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateActivityInstanceStartEvt(IDelegateExecution execution);

        /// <summary>
        ///     Creates the history event fired when an activity instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateActivityInstanceUpdateEvt(IDelegateExecution execution);

        /// <summary>
        ///     Creates the history event fired when an activity instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <param name="task"> the ITask association that is currently updated. (May be null in case there is not ITask associated.) </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateActivityInstanceUpdateEvt(IDelegateExecution execution, IDelegateTask task);

        /// <summary>
        ///     Creates the history event which is fired when an activity instance is migrated.
        /// </summary>
        /// <param name="actInstance"> the migrated activity instance which contains the new id's </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateActivityInstanceMigrateEvt(MigratingActivityInstance actInstance);

        /// <summary>
        ///     Creates the history event fired when an activity instance is <strong>ended</strong>.
        /// </summary>
        /// <param name="execution"> the current execution. </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateActivityInstanceEndEvt(IDelegateExecution execution);


        // ITask Instances /////////////////////////////////////////

        /// <summary>
        ///     Creates the history event fired when a ITask instance is <strong>created</strong>.
        /// </summary>
        /// <param name="task"> the ITask </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateTaskInstanceCreateEvt(IDelegateTask task);

        /// <summary>
        ///     Creates the history event fired when a ITask instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="task"> the ITask </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateTaskInstanceUpdateEvt(IDelegateTask task);

        /// <summary>
        ///     Creates the history event fired when a ITask instance is <strong>migrated</strong>.
        /// </summary>
        /// <param name="task"> the ITask </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateTaskInstanceMigrateEvt(IDelegateTask task);

        /// <summary>
        ///     Creates the history event fired when a ITask instances is <strong>completed</strong>.
        /// </summary>
        /// <param name="task"> the ITask </param>
        /// <param name="deleteReason"> </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateTaskInstanceCompleteEvt(IDelegateTask task, string deleteReason);

        // User Operation Logs ///////////////////////////////

        /// <summary>
        ///     Creates the history event fired whenever an operation has been performed by a user. This is
        ///     used for logging actions such as creating a new ITask, completing a ITask, canceling a
        ///     a process instance, ...
        /// </summary>
        /// <param name="context"> the <seealso cref="UserOperationLogContext" /> providing the needed informations </param>
        /// <returns> a <seealso cref="List" /> of <seealso cref="HistoryEvent" />s </returns>
        IList<HistoryEvent> CreateUserOperationLogEvents(UserOperationLogContext context);

        // HistoricVariableUpdateEventEntity //////////////////////

        /// <summary>
        ///     Creates the history event fired when a variable is <strong>created</strong>.
        /// </summary>
        /// <param name="variableInstance"> the runtime variable instance </param>
        /// <param name="the"> scope to which the variable is linked </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateHistoricVariableCreateEvt(VariableInstanceEntity variableInstance,
            IVariableScope sourceVariableScope);

        /// <summary>
        ///     Creates the history event fired when a variable is <strong>updated</strong>.
        /// </summary>
        /// <param name="variableInstance"> the runtime variable instance </param>
        /// <param name="the"> scope to which the variable is linked </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateHistoricVariableUpdateEvt(VariableInstanceEntity variableInstance,
            IVariableScope sourceVariableScope);

        /// <summary>
        ///     Creates the history event fired when a variable is <strong>migrated</strong>.
        /// </summary>
        /// <param name="variableInstance"> the runtime variable instance </param>
        /// <param name="the"> scope to which the variable is linked </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateHistoricVariableMigrateEvt(VariableInstanceEntity variableInstance);

        /// <summary>
        ///     Creates the history event fired when a variable is <strong>deleted</strong>.
        /// </summary>
        /// <param name="variableInstance"> </param>
        /// <param name="variableScopeImpl"> </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateHistoricVariableDeleteEvt(VariableInstanceEntity variableInstance,
            IVariableScope sourceVariableScope);

        // Form properties //////////////////////////////////////////

        /// <summary>
        ///     Creates the history event fired when a form property is <strong>updated</strong>.
        /// </summary>
        /// <param name="processInstance"> the id for the process instance </param>
        /// <param name="propertyId"> the id of the form property </param>
        /// <param name="propertyValue"> the value of the form property </param>
        /// <param name="taskId"> </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateFormPropertyUpdateEvt(ExecutionEntity execution, string propertyId, string propertyValue,
            string taskId);

        // Incidents //////////////////////////////////////////

        HistoryEvent CreateHistoricIncidentCreateEvt(IIncident incident);

        HistoryEvent CreateHistoricIncidentResolveEvt(IIncident incident);

        HistoryEvent CreateHistoricIncidentDeleteEvt(IIncident incident);

        HistoryEvent CreateHistoricIncidentMigrateEvt(IIncident incident);

        // Job Log ///////////////////////////////////////////

        /// <summary>
        ///     Creates the history event fired when a job has been <strong>created</strong>.
        ///     
        /// </summary>
        HistoryEvent CreateHistoricJobLogCreateEvt(IJob job);

        /// <summary>
        ///     Creates the history event fired when the execution of a job <strong>failed</strong>.
        ///     
        /// </summary>
        HistoryEvent CreateHistoricJobLogFailedEvt(IJob job, System.Exception exception);

        /// <summary>
        ///     Creates the history event fired when the execution of a job was <strong>successful</strong>.
        ///     
        /// </summary>
        HistoryEvent CreateHistoricJobLogSuccessfulEvt(IJob job);

        /// <summary>
        ///     Creates the history event fired when the a job has been <strong>deleted</strong>.
        ///     
        /// </summary>
        HistoryEvent CreateHistoricJobLogDeleteEvt(IJob job);

        /// <summary>
        ///     Creates the history event fired when the a batch has been <strong>started</strong>.
        /// </summary>
        HistoryEvent CreateBatchStartEvent(IBatch batch);


        /// <summary>
        ///     Creates the history event fired when the a batch has been <strong>completed</strong>.
        /// </summary>
        HistoryEvent CreateBatchEndEvent(IBatch batch);

        /// <summary>
        ///     Fired when an identity link is added
        /// </summary>
        /// <param name="identitylink">
        ///     @return
        /// </param>
        HistoryEvent CreateHistoricIdentityLinkAddEvent(IIdentityLink identitylink);

        /// <summary>
        ///     Fired when an identity links is deleted
        /// </summary>
        /// <param name="identityLink">
        ///     @return
        /// </param>
        HistoryEvent CreateHistoricIdentityLinkDeleteEvent(IIdentityLink identityLink);

        /// <summary>
        ///     Creates the history event when an external ITask has been <strong>created</strong>.
        /// </summary>
        HistoryEvent CreateHistoricExternalTaskLogCreatedEvt(IExternalTask ITask);

        /// <summary>
        ///     Creates the history event when the execution of an external ITask has <strong>failed</strong>.
        /// </summary>
        HistoryEvent CreateHistoricExternalTaskLogFailedEvt(IExternalTask ITask);

        /// <summary>
        ///     Creates the history event when the execution of an external ITask was <strong>successful</strong>.
        /// </summary>
        HistoryEvent CreateHistoricExternalTaskLogSuccessfulEvt(IExternalTask ITask);

        /// <summary>
        ///     Creates the history event when an external ITask has been <strong>deleted</strong>.
        /// </summary>
        HistoryEvent CreateHistoricExternalTaskLogDeletedEvt(IExternalTask ITask);
    }
}