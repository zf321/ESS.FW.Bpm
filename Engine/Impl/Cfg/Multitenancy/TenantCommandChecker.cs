using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;
using DecisionDefinitionEntity = ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionDefinitionEntity;
using DecisionRequirementsDefinitionEntity =
    ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionRequirementsDefinitionEntity;
using HistoricExternalTaskLogEntity = ESS.FW.Bpm.Engine.History.Impl.Event.HistoricExternalTaskLogEntity;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy
{
    /// <summary>
    ///     <seealso cref="ICommandChecker" /> to ensure that commands are only executed for
    ///     entities which belongs to one of the authenticated tenants.
    /// </summary>
    public class TenantCommandChecker : ICommandChecker
    {
        protected internal static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected ITenantManager TenantManager
        {
            get
            {
                return Context.CommandContext.TenantManager;
            }
        }
        public virtual void CheckEvaluateDecision(IDecisionDefinition decisionDefinition)
        {
            if (!TenantManager.IsAuthenticatedTenant(decisionDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("evaluate the decision '" + decisionDefinition.Id + "'");
            }
        }

        public virtual void CheckCreateProcessInstance(IProcessDefinition processDefinition)
        {
            if (!TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("create an instance of the process definition '" + processDefinition.Id + "'");
            }
        }

        public virtual void CheckReadProcessDefinition(IProcessDefinition processDefinition)
        {
            if (!TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the process definition '" + processDefinition.Id + "'");
            }
        }

        public virtual void CheckCreateCaseInstance(ICaseDefinition caseDefinition)
        {
            if (!TenantManager.IsAuthenticatedTenant(caseDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("create an instance of the case definition '" + caseDefinition.Id + "'");
            }
        }

        public virtual void CheckUpdateProcessDefinitionById(string processDefinitionId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                ProcessDefinitionEntity processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null && !TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("update the process definition '" + processDefinitionId + "'");
                }
            }
        }

        public virtual void CheckUpdateProcessDefinitionByKey(string processDefinitionKey)
        {
        }

        public virtual void CheckDeleteProcessDefinitionById(string processDefinitionId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                ProcessDefinitionEntity processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null && !TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("delete the process definition '" + processDefinitionId + "'");
                }
            }
        }

        public virtual void CheckDeleteProcessDefinitionByKey(string processDefinitionKey)
        {
        }


        public virtual void CheckUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                ProcessDefinitionEntity processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null && !TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("update the process definition '" + processDefinitionId + "'");
                }
            }
        }

        public virtual void CheckUpdateProcessInstance(ExecutionEntity execution)
        {
            if (execution != null && !TenantManager.IsAuthenticatedTenant(execution.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("update the process instance '" + execution.Id + "'");
            }
        }

        public virtual void CheckUpdateJob(JobEntity job)
        {
            if (job != null && !TenantManager.IsAuthenticatedTenant(job.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("update the job '" + job.Id + "'");
            }
        }

        public virtual void CheckUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
        {
        }

        public virtual void CheckUpdateProcessInstanceById(string processInstanceId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                ExecutionEntity execution = FindExecutionById(processInstanceId);
                if (execution != null && !TenantManager.IsAuthenticatedTenant(execution.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("update the process instance '" + processInstanceId + "'");
                }
            }
        }
        

        public virtual void CheckCreateMigrationPlan(IProcessDefinition sourceProcessDefinition, IProcessDefinition targetProcessDefinition)
        {
            string sourceTenant = sourceProcessDefinition.TenantId;
            string targetTenant = targetProcessDefinition.TenantId;

            if (!TenantManager.IsAuthenticatedTenant(sourceTenant))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get process definition '" + sourceProcessDefinition.Id + "'");
            }
            if (!TenantManager.IsAuthenticatedTenant(targetTenant))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get process definition '" + targetProcessDefinition.Id + "'");
            }

            if (!string.ReferenceEquals(sourceTenant, null) && !string.ReferenceEquals(targetTenant, null) && !sourceTenant.Equals(targetTenant))
            {
                throw ProcessEngineLogger.MigrationLogger.CannotMigrateBetweenTenants(sourceTenant, targetTenant);
            }
        }

        public virtual void CheckReadProcessInstance(string processInstanceId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                ExecutionEntity execution = FindExecutionById(processInstanceId);
                if (execution != null && !TenantManager.IsAuthenticatedTenant(execution.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("read the process instance '" + processInstanceId + "'");
                }
            }
        }

        public virtual void CheckReadJob(JobEntity job)
        {
            if (job != null && !TenantManager.IsAuthenticatedTenant(job.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("read the job '" + job.Id + "'");
            }
        }

        public virtual void CheckReadProcessInstance(ExecutionEntity execution)
        {
            if (execution != null && !TenantManager.IsAuthenticatedTenant(execution.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("read the process instance '" + execution.Id + "'");
            }
        }

        public virtual void CheckDeleteProcessInstance(ExecutionEntity execution)
        {
            if (execution != null && !TenantManager.IsAuthenticatedTenant(execution.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the process instance '" + execution.Id + "'");
            }
        }

        public virtual void CheckMigrateProcessInstance(ExecutionEntity processInstance, IProcessDefinition targetProcessDefinition)
        {
            string sourceTenant = processInstance.TenantId;
            string targetTenant = targetProcessDefinition.TenantId;

            if (TenantManager.TenantCheckEnabled)
            {
                if (processInstance != null && !TenantManager.IsAuthenticatedTenant(processInstance.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("migrate process instance '" + processInstance.Id + "'");
                }
            }

            if (!string.ReferenceEquals(targetTenant, null) && (string.ReferenceEquals(sourceTenant, null) || !sourceTenant.Equals(targetTenant)))
            {
                throw ProcessEngineLogger.MigrationLogger.CannotMigrateInstanceBetweenTenants(processInstance.Id, sourceTenant, targetTenant);
            }
        }

        public virtual void CheckReadTask(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("read the ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckUpdateTask(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("update the ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckDeleteBatch(BatchEntity batch)
        {
            if (batch != null && !TenantManager.IsAuthenticatedTenant(batch.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete batch '" + batch.Id + "'");
            }
        }

        public virtual void CheckDeleteHistoricBatch(HistoricBatchEntity batch)
        {
            if (batch != null && !TenantManager.IsAuthenticatedTenant(batch.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete historic batch '" + batch.Id + "'");
            }
        }

        public virtual void CheckSuspendBatch(BatchEntity batch)
        {
            if (batch != null && !TenantManager.IsAuthenticatedTenant(batch.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("suspend batch '" + batch.Id + "'");
            }
        }

        public virtual void CheckActivateBatch(BatchEntity batch)
        {
            if (batch != null && !TenantManager.IsAuthenticatedTenant(batch.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("activate batch '" + batch.Id + "'");
            }
        }

        public virtual void CheckCreateDeployment()
        {
        }

        public virtual void CheckReadDeployment(string deploymentId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                DeploymentEntity deployment = FindDeploymentById(deploymentId);
                if (deployment != null && !TenantManager.IsAuthenticatedTenant(deployment.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("get the deployment '" + deploymentId + "'");
                }
            }
        }

        public virtual void CheckDeleteDeployment(string deploymentId)
        {
            if (TenantManager.TenantCheckEnabled)
            {
                DeploymentEntity deployment = FindDeploymentById(deploymentId);
                if (deployment != null && !TenantManager.IsAuthenticatedTenant(deployment.TenantId))
                {
                    throw Log.ExceptionCommandWithUnauthorizedTenant("delete the deployment '" + deploymentId + "'");
                }
            }
        }

        public virtual void CheckDeleteTask(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckTaskAssign(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("assign the ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckCreateTask(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("create the ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckCreateTask()
        {
        }

        public virtual void CheckTaskWork(TaskEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("work on ITask '" + task.Id + "'");
            }
        }

        public virtual void CheckReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
        {
            if (decisionDefinition != null && !TenantManager.IsAuthenticatedTenant(decisionDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the decision definition '" + decisionDefinition.Id + "'");
            }
        }

        public virtual void CheckReadDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
        {
            if (decisionRequirementsDefinition != null && !TenantManager.IsAuthenticatedTenant(decisionRequirementsDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the decision requirements definition '" + decisionRequirementsDefinition.Id + "'");
            }
        }

        public virtual void CheckReadCaseDefinition(ICaseDefinition caseDefinition)
        {
            if (caseDefinition != null && !TenantManager.IsAuthenticatedTenant(caseDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the case definition '" + caseDefinition.Id + "'");
            }
        }

        public void CheckCreateProcessInstance(ProcessDefinitionEntity processDefinition)
        {
            if (!TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("create an instance of the process definition '" + processDefinition.Id + "'");
            }
        }

        public virtual void CheckDeleteHistoricTaskInstance(HistoricTaskInstanceEventEntity task)
        {
            if (task != null && !TenantManager.IsAuthenticatedTenant(task.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the historic ITask instance '" + task.Id + "'");
            }
        }

        public virtual void CheckDeleteHistoricProcessInstance(IHistoricProcessInstance instance)
        {
            if (instance != null && !TenantManager.IsAuthenticatedTenant(instance.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the historic process instance '" + instance.Id + "'");
            }
        }

        public virtual void CheckDeleteHistoricCaseInstance(IHistoricCaseInstance instance)
        {
            if (instance != null && !TenantManager.IsAuthenticatedTenant(instance.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the historic case instance '" + instance.Id + "'");
            }
        }

        public virtual void CheckDeleteHistoricDecisionInstance(string decisionDefinitionKey)
        {
            // No tenant check here because it is called in the SQL query:
            // HistoricDecisionInstance.selectHistoricDecisionInstancesByDecisionDefinitionId
            // It is necessary to make the check there because of performance issues. If the check
            // is done here then the we get all history decision instances (also from possibly
            // other tenants) and then filter them. If there are a lot instances this can cause
            // latency. Therefore does the SQL query only return the
            // historic decision instances which belong to the authenticated tenant.
        }
        
        public virtual void CheckDeleteHistoricDecisionInstance(IHistoricDecisionInstance decisionInstance)
        {
            if (decisionInstance != null && !TenantManager.IsAuthenticatedTenant(decisionInstance.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("delete the historic decision instance '" + decisionInstance.Id + "'");
            }
        }

        public virtual void CheckReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog)
        {
            if (historicJobLog != null && !TenantManager.IsAuthenticatedTenant(historicJobLog.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the historic job log '" + historicJobLog.Id + "'");
            }
        }

        public virtual void CheckReadHistoryAnyProcessDefinition()
        {
            // No tenant check here because it is called in the SQL query:
            // Report.selectHistoricProcessInstanceDurationReport
            // It is necessary to make the check there because the query may be return only the
            // historic process instances which belong to the authenticated tenant.
        }

        public virtual void CheckReadHistoryAnyTaskInstance()
        {
            // No tenant check here because it is called in the SQL query:
            // Report.selectHistoricProcessInstanceDurationReport
            // It is necessary to make the check there because the query may be return only the
            // historic process instances which belong to the authenticated tenant.
        }

        public virtual void CheckUpdateCaseInstance(ICaseExecution caseExecution)
        {
            if (caseExecution != null && !TenantManager.IsAuthenticatedTenant(caseExecution.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("update the case execution '" + caseExecution.Id + "'");
            }
        }

        public virtual void CheckReadCaseInstance(ICaseExecution caseExecution)
        {
            if (caseExecution != null && !TenantManager.IsAuthenticatedTenant(caseExecution.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the case execution '" + caseExecution.Id + "'");
            }
        }

        //helper //////////////////////////////////////////////////


        protected internal virtual ProcessDefinitionEntity FindLatestProcessDefinitionById(string processDefinitionId)
        {
            return Context.CommandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
        }

        protected internal virtual ExecutionEntity FindExecutionById(string processInstanceId)
        {
            return Context.CommandContext.ExecutionManager.FindExecutionById(processInstanceId);
        }

        protected internal virtual DeploymentEntity FindDeploymentById(string deploymentId)
        {
            return Context.CommandContext.DeploymentManager.FindDeploymentById(deploymentId);
        }

        public virtual void CheckDeleteUserOperationLog(IUserOperationLogEntry entry)
        {
            // tenant check is not available for user operation log
        }

        public virtual void CheckReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog)
        {
            if (historicExternalTaskLog != null && !TenantManager.IsAuthenticatedTenant(historicExternalTaskLog.TenantId))
            {
                throw Log.ExceptionCommandWithUnauthorizedTenant("get the historic external ITask log '" + historicExternalTaskLog.Id + "'");
            }
        }
    }
}