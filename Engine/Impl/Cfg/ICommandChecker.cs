using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;
using DecisionDefinitionEntity = ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionDefinitionEntity;
using DecisionRequirementsDefinitionEntity =
    ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionRequirementsDefinitionEntity;
using HistoricExternalTaskLogEntity = ESS.FW.Bpm.Engine.History.Impl.Event.HistoricExternalTaskLogEntity;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{

    /// <summary>
    ///     Is invoked while executing a command to check if the current operation is
    ///     allowed on the entity. If it is not allowed, the checker throws a
    ///     <seealso cref="ProcessEngineException" />.
    /// </summary>
    public interface ICommandChecker
    {
        /// <summary>
        ///     Checks if it is allowed to evaluate the given decision.
        /// </summary>
        void CheckEvaluateDecision(IDecisionDefinition decisionDefinition);

        /// <summary>
        ///     Checks if it is allowed to create an instance of the given process definition.
        /// </summary>
        void CheckCreateProcessInstance(IProcessDefinition processDefinition);

        /// <summary>
        ///     Checks if it is allowed to read the given process definition.
        /// </summary>
        void CheckReadProcessDefinition(IProcessDefinition processDefinition);

        /// <summary>
        ///     Checks if it is allowed to create an instance of the given case definition.
        /// </summary>
        void CheckCreateCaseInstance(ICaseDefinition caseDefinition);

        /// <summary>
        ///     Checks if it is allowed to update a process definition of the given process definition id.
        /// </summary>
        void CheckUpdateProcessDefinitionById(string processDefinitionId);

        /// <summary>
        ///     Checks if it is allowed to update a process instance of the given process definition id.
        /// </summary>
        void CheckUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId);

        /// <summary>
        ///     Checks if it is allowed to update a process definition of the given process definition key.
        /// </summary>
        void CheckUpdateProcessDefinitionByKey(string processDefinitionKey);

        /// <summary>
        ///     Checks if it is allowed to delete a process definition, which corresponds to the given id.
        /// </summary>
        /// <param name="processDefinitionId"> the id which corresponds to the process definition </param>
        void CheckDeleteProcessDefinitionById(string processDefinitionId);

        /// <summary>
        ///     Checks if it is allowed to delete a process definition, which corresponds to the given key.
        /// </summary>
        /// <param name="processDefinitionKey"> the key which corresponds to the process definition </param>
        void CheckDeleteProcessDefinitionByKey(string processDefinitionKey);

        /// <summary>
        ///     Checks if it is allowed to update a process instance of the given process definition key.
        /// </summary>
        void CheckUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey);

        /// <summary>
        ///     Checks if it is allowed to update a process instance of the given process instance id.
        /// </summary>
        void CheckUpdateProcessInstanceById(string processInstanceId);

        /// <summary>
        ///     Checks if it is allowed to update a process instance of the given execution.
        /// </summary>
        void CheckUpdateProcessInstance(ExecutionEntity execution);

        void CheckCreateMigrationPlan(IProcessDefinition sourceProcessDefinition,
            IProcessDefinition targetProcessDefinition);

        void CheckMigrateProcessInstance(ExecutionEntity processInstance, IProcessDefinition targetProcessDefinition);

        void CheckReadProcessInstance(string processInstanceId);

        /// <summary>
        ///     Checks if it is allowed to read the given job.
        /// </summary>
        void CheckReadJob(JobEntity job);

        /// <summary>
        ///     Checks if it is allowed to update the given job.
        /// </summary>
        void CheckUpdateJob(JobEntity job);

        /// <summary>
        ///     Checks if it is allowed to read a process instance of the given execution.
        /// </summary>
        void CheckReadProcessInstance(ExecutionEntity execution);

        /// <summary>
        ///     Check if it is allowed to delete a process instance of the given execution.
        /// </summary>
        void CheckDeleteProcessInstance(ExecutionEntity execution);

        /// <summary>
        ///     Check if it is allowed to read a task.
        /// </summary>
        void CheckReadTask(TaskEntity task);

        /// <summary>
        ///     Check if it is allowed to update a task
        /// </summary>
        void CheckUpdateTask(TaskEntity task);

        /// <summary>
        ///     Check if it is allowed to delete a batch
        /// </summary>
        void CheckDeleteBatch(BatchEntity batch);

        /// <summary>
        ///     Check if it is allowed to delete a historic batch
        /// </summary>
        void CheckDeleteHistoricBatch(HistoricBatchEntity batch);

        /// <summary>
        ///     Check if it is allowed to suspend a batch
        /// </summary>
        void CheckSuspendBatch(BatchEntity batch);

        /// <summary>
        ///     Check if it is allowed to activate a batch
        /// </summary>
        void CheckActivateBatch(BatchEntity batch);

        /// <summary>
        ///     Checks if it is allowed to create a deployment.
        /// </summary>
        void CheckCreateDeployment();

        /// <summary>
        ///     Checks if it is allowed to read a deployment of the given deployment id.
        /// </summary>
        void CheckReadDeployment(string deploymentId);

        /// <summary>
        ///     Checks if it is allowed to delete a deployment of the given deployment id.
        /// </summary>
        void CheckDeleteDeployment(string deploymentId);

        /// <summary>
        ///     Check if it is allowed to assign a task
        /// </summary>
        void CheckTaskAssign(TaskEntity task);

        /// <summary>
        ///     Check if it is allowed to create a task
        /// </summary>
        void CheckCreateTask(TaskEntity task);

        /// <summary>
        ///     Check if it is allowed to create a task
        /// </summary>
        void CheckCreateTask();

        /// <summary>
        ///     Check if it is allowed to work on a task
        /// </summary>
        void CheckTaskWork(TaskEntity task);

        /// <summary>
        ///     Check if it is allowed to delete a task
        /// </summary>
        void CheckDeleteTask(TaskEntity task);

        /// <summary>
        ///     Checks if it is allowed to read the given decision definition.
        /// </summary>
        void CheckReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition);

        /// <summary>
        ///     Checks if it is allowed to read the given decision requirements definition.
        /// </summary>
        void CheckReadDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition);

        /// <summary>
        ///     Checks if it is allowed to read the given case definition.
        /// </summary>
        void CheckReadCaseDefinition(ICaseDefinition caseDefinition);

        void CheckCreateProcessInstance(ProcessDefinitionEntity processDefinition);

        /// <summary>
        ///     Checks if it is allowed to delete the given historic task instance.
        /// </summary>
        void CheckDeleteHistoricTaskInstance(HistoricTaskInstanceEventEntity task);

        /// <summary>
        ///     Checks if it is allowed to delete the given historic process instance.
        /// </summary>
        void CheckDeleteHistoricProcessInstance(IHistoricProcessInstance instance);

        /// <summary>
        ///     Checks if it is allowed to delete the given historic case instance.
        /// </summary>
        void CheckDeleteHistoricCaseInstance(IHistoricCaseInstance instance);

        /// <summary>
        ///     Checks if it is allowed to delete the historic decision instance of the given
        ///     decision definition key.
        /// </summary>
        void CheckDeleteHistoricDecisionInstance(string decisionDefinitionKey);

        /// <summary>
        ///     Checks if it is allowed to delete the given historic decision instance.
        /// </summary>
        void CheckDeleteHistoricDecisionInstance(IHistoricDecisionInstance instance);

        /// <summary>
        ///     Checks if it is allowed to read the given historic job log.
        /// </summary>
        void CheckReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog);

        /// <summary>
        ///     Check if it is allowed to read the history for any process definition.
        /// </summary>
        void CheckReadHistoryAnyProcessDefinition();

        /// <summary>
        ///     Check if it is allowed to read the history for any task instance
        /// </summary>
        void CheckReadHistoryAnyTaskInstance();

        /// <summary>
        ///     Check if it is allowed to update a case instance of the given case execution.
        /// </summary>
        void CheckUpdateCaseInstance(ICaseExecution caseExecution);

        /// <summary>
        ///     Check if it is allowed to delete the user operation log of the given user operation log entry.
        /// </summary>
        void CheckDeleteUserOperationLog(IUserOperationLogEntry entry);

        /// <summary>
        ///     Check if it is allowed to read a case instance of the given case execution.
        /// </summary>
        void CheckReadCaseInstance(ICaseExecution caseExecution);

        /// <summary>
        ///     Checks if it is allowed to read the given historic external task log.
        /// </summary>
        void CheckReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog);
    }
}