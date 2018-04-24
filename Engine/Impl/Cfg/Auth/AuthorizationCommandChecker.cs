using System;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;
using DecisionDefinitionEntity = ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionDefinitionEntity;
using DecisionRequirementsDefinitionEntity =
    ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository.DecisionRequirementsDefinitionEntity;
using HistoricExternalTaskLogEntity = ESS.FW.Bpm.Engine.History.Impl.Event.HistoricExternalTaskLogEntity;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Auth
{
    /// <summary>
    ///     <seealso cref="ICommandChecker" /> that uses the <seealso cref="AuthorizationManager" /> to perform
    ///     authorization checks.
    /// </summary>
    public class AuthorizationCommandChecker : ICommandChecker
    {


        // helper ////////////////////////////////////////

        protected internal virtual IAuthorizationManager AuthorizationManager
        {
            get { return Context.CommandContext.AuthorizationManager; }
        }

        public virtual void CheckEvaluateDecision(IDecisionDefinition decisionDefinition)
        {
            AuthorizationManager.CheckAuthorization(Permissions.CreateInstance, Resources.DecisionDefinition, decisionDefinition.Key);
        }

        public virtual void CheckCreateProcessInstance(IProcessDefinition processDefinition)
        {
            // necessary permissions:
            // - CREATE on PROCESS_INSTANCE
            // AND
            // - CREATE_INSTANCE on PROCESS_DEFINITION
            AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.ProcessInstance);
            AuthorizationManager.CheckAuthorization(Permissions.CreateInstance, Resources.ProcessDefinition, processDefinition.Key);
        }

        public virtual void CheckReadProcessDefinition(IProcessDefinition processDefinition)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.ProcessDefinition, processDefinition.Key);
        }

        public virtual void CheckCreateCaseInstance(ICaseDefinition caseDefinition)
        {
            // no authorization check for CMMN
        }


        public virtual void CheckUpdateProcessDefinitionById(string processDefinitionId)
        {
            if (AuthorizationManager.AuthorizationEnabled)
            {
                var processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null)
                {
                    CheckUpdateProcessDefinitionByKey(processDefinition.Key);
                }
            }
        }

        public virtual void CheckUpdateProcessDefinitionByKey(string processDefinitionKey)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Update, Resources.ProcessDefinition, processDefinitionKey);
        }

        public virtual void CheckDeleteProcessDefinitionById(string processDefinitionId)
        {
            if (AuthorizationManager.AuthorizationEnabled)
            {
                var processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null)
                {
                    CheckDeleteProcessDefinitionByKey(processDefinition.Key);
                }
            }
        }

        public virtual void CheckDeleteProcessDefinitionByKey(string processDefinitionKey)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Delete, Resources.ProcessDefinition, processDefinitionKey);
        }

        public virtual void CheckUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId)
        {
            if (AuthorizationManager.AuthorizationEnabled)
            {
                var processDefinition = FindLatestProcessDefinitionById(processDefinitionId);
                if (processDefinition != null)
                {
                    CheckUpdateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
                }
            }
        }

        public virtual void CheckUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
        {
            // necessary permissions:
            // - UPDATE on ANY PROCESS_INSTANCE

            var firstCheck = new PermissionCheck();
            firstCheck.Permission = Permissions.Update;
            firstCheck.Resource = Resources.ProcessInstance; ;

            // ... OR ...

            // - UPDATE_INSTANCE on PROCESS_DEFINITION

            var secondCheck = new PermissionCheck();
            secondCheck.Permission = Permissions.UpdateInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = processDefinitionKey;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckReadProcessInstance(string processInstanceId)
        {
            var execution = FindExecutionById(processInstanceId);
            if (execution != null)
            {
                CheckReadProcessInstance(execution);
            }
        }

        public virtual void CheckDeleteProcessInstance(ExecutionEntity execution)
        {
            var processDefinition = (ProcessDefinitionEntity)execution.GetProcessDefinition();

            // necessary permissions:
            // - DELETE on PROCESS_INSTANCE

            var firstCheck = new PermissionCheck();
            firstCheck.Permission = Permissions.Delete;
            firstCheck.Resource = Resources.ProcessInstance;
            firstCheck.ResourceId = execution.ProcessInstanceId;

            // ... OR ...

            // - DELETE_INSTANCE on PROCESS_DEFINITION

            var secondCheck = new PermissionCheck();
            secondCheck.Permission = Permissions.DeleteInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = processDefinition.Key;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckUpdateProcessInstanceById(string processInstanceId)
        {
            var execution = FindExecutionById(processInstanceId);
            if (execution != null)
            {
                CheckUpdateProcessInstance(execution);
            }
        }

        public virtual void CheckUpdateProcessInstance(ExecutionEntity execution)
        {
            var processDefinition = (ProcessDefinitionEntity)execution.GetProcessDefinition();

            // necessary permissions:
            // - UPDATE on PROCESS_INSTANCE

            var firstCheck = new PermissionCheck();
            firstCheck.Permission = Permissions.Update;
            firstCheck.Resource = Resources.ProcessInstance;
            firstCheck.ResourceId = execution.ProcessInstanceId;

            // ... OR ...

            // - UPDATE_INSTANCE on PROCESS_DEFINITION

            var secondCheck = new PermissionCheck();
            secondCheck.Permission = Permissions.UpdateInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = processDefinition.Key;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckUpdateJob(JobEntity job)
        {
            if (ReferenceEquals(job.ProcessDefinitionKey, null))
            {
                // "standalone" job: nothing to do!
                return;
            }

            // necessary permissions:
            // - READ on PROCESS_INSTANCE

            PermissionCheck firstCheck = AuthorizationManager.NewPermissionCheck();
            firstCheck.Permission = Permissions.Update;
            firstCheck.Resource = Resources.ProcessInstance;
            firstCheck.ResourceId = job.ProcessInstanceId;

            // ... OR ...

            // - UPDATE_INSTANCE on PROCESS_DEFINITION
            PermissionCheck secondCheck = AuthorizationManager.NewPermissionCheck();
            secondCheck.Permission = Permissions.UpdateInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = job.ProcessDefinitionKey;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckCreateMigrationPlan(IProcessDefinition sourceProcessDefinition,
            IProcessDefinition targetProcessDefinition)
        {
            CheckReadProcessDefinition(sourceProcessDefinition);
            CheckReadProcessDefinition(targetProcessDefinition);
        }

        public virtual void CheckMigrateProcessInstance(ExecutionEntity processInstance,
            IProcessDefinition targetProcessDefinition)
        {
        }

        public virtual void CheckReadProcessInstance(ExecutionEntity execution)
        {
            var processDefinition = (ProcessDefinitionEntity)execution.GetProcessDefinition();

            // necessary permissions:
            // - READ on PROCESS_INSTANCE

            var firstCheck = new PermissionCheck();
            firstCheck.Permission = Permissions.Read;
            firstCheck.Resource = Resources.ProcessInstance;
            firstCheck.ResourceId = execution.ProcessInstanceId;

            // ... OR ...

            // - READ_INSTANCE on PROCESS_DEFINITION
            var secondCheck = new PermissionCheck();
            secondCheck.Permission = Permissions.ReadInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = processDefinition.Key;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckReadJob(JobEntity job)
        {
            if (ReferenceEquals(job.ProcessDefinitionKey, null))
            {
                // "standalone" job: nothing to do!
                return;
            }

            // necessary permissions:
            // - READ on PROCESS_INSTANCE

            PermissionCheck firstCheck = AuthorizationManager.NewPermissionCheck();
            firstCheck.Permission = Permissions.Read;
            firstCheck.Resource = Resources.ProcessInstance;
            firstCheck.ResourceId = job.ProcessInstanceId;

            // ... OR ...

            // - READ_INSTANCE on PROCESS_DEFINITION
            PermissionCheck secondCheck = AuthorizationManager.NewPermissionCheck();
            secondCheck.Permission = Permissions.ReadInstance;
            secondCheck.Resource = Resources.ProcessDefinition;
            secondCheck.ResourceId = job.ProcessDefinitionKey;
            secondCheck.AuthorizationNotFoundReturnValue = 0L;

            AuthorizationManager.CheckAuthorization(firstCheck, secondCheck);
        }

        public virtual void CheckReadTask(TaskEntity task)
        {
            var taskId = task.Id;

            var executionId = task.ExecutionId;
            if (!ReferenceEquals(executionId, null))
            {
                // if ITask exists in context of a process instance
                // then check the following permissions:
                // - READ on ITask
                // - READ_TASK on PROCESS_DEFINITION

                ExecutionEntity execution = task.GetExecution();
                ProcessDefinitionEntity processDefinition = execution.GetProcessDefinition();


                PermissionCheck readPermissionCheck = AuthorizationManager.NewPermissionCheck();
                readPermissionCheck.Permission = Permissions.Read;
                readPermissionCheck.Resource = Resources.Task;
                readPermissionCheck.ResourceId = taskId;

                PermissionCheck readTaskPermissionCheck = AuthorizationManager.NewPermissionCheck();
                readTaskPermissionCheck.Permission = Permissions.ReadTask;
                readTaskPermissionCheck.Resource = Resources.ProcessDefinition;
                readTaskPermissionCheck.ResourceId = processDefinition.Key;
                readTaskPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

                AuthorizationManager.CheckAuthorization(readPermissionCheck, readTaskPermissionCheck);
            }
            else
            {
                // if ITask does not exist in context of process
                // instance, then it is either a (a) standalone ITask
                // or (b) it exists in context of a case instance.

                // (a) standalone ITask: check following permission
                // - READ on ITask
                // (b) ITask in context of a case instance, in this
                // case it is not necessary to check any permission,
                // because such tasks can always be read

                var caseExecutionId = task.CaseExecutionId;
                if (ReferenceEquals(caseExecutionId, null))
                {
                    AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.Task, taskId);
                }
            }
        }

        public virtual void CheckUpdateTask(TaskEntity task)
        {
            var taskId = task.Id;

            var executionId = task.ExecutionId;
            if (!ReferenceEquals(executionId, null))
            {
                // if ITask exists in context of a process instance
                // then check the following permissions:
                // - UPDATE on ITask
                // - UPDATE_TASK on PROCESS_DEFINITION

                ExecutionEntity execution = task.GetExecution();
                var processDefinition = (ProcessDefinitionEntity)execution.GetProcessDefinition();

                var updatePermissionCheck = new PermissionCheck();
                updatePermissionCheck.Permission = Permissions.Update;
                updatePermissionCheck.Resource = Resources.Task;
                updatePermissionCheck.ResourceId = taskId;

                var updateTaskPermissionCheck = new PermissionCheck();
                updateTaskPermissionCheck.Permission = Permissions.UpdateTask;
                updateTaskPermissionCheck.Resource = Resources.ProcessDefinition;
                updateTaskPermissionCheck.ResourceId = processDefinition.Key;
                updateTaskPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

                AuthorizationManager.CheckAuthorization(updatePermissionCheck, updateTaskPermissionCheck);
            }
            else
            {
                // if ITask does not exist in context of process
                // instance, then it is either a (a) standalone ITask
                // or (b) it exists in context of a case instance.

                // (a) standalone ITask: check following permission
                // - READ on ITask
                // (b) ITask in context of a case instance, in this
                // case it is not necessary to check any permission,
                // because such tasks can always be updated

                var caseExecutionId = task.CaseExecutionId;
                if (ReferenceEquals(caseExecutionId, null))
                {
                    // standalone ITask
                    AuthorizationManager.CheckAuthorization(Permissions.Update, Resources.Task, taskId);
                }
            }
        }

        public virtual void CheckDeleteBatch(BatchEntity batch)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Delete, Resources.Batch, batch.Id);
        }

        public virtual void CheckDeleteHistoricBatch(HistoricBatchEntity batch)
        {
            AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.Batch, batch.Id);
        }

        public virtual void CheckSuspendBatch(BatchEntity batch)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Update, Resources.Batch, batch.Id);
        }

        public virtual void CheckActivateBatch(BatchEntity batch)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Update, Resources.Batch, batch.Id);
        }

        /// DEPLOYMENT 

        // create permission ////////////////////////////////////////////////

        public virtual void CheckCreateDeployment()
        {
            AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Deployment);
        }

        // read permission //////////////////////////////////////////////////

        public virtual void CheckReadDeployment(string deploymentId)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.Deployment, deploymentId);
        }

        //delete permission //////////////////////////////////////////////////

        public virtual void CheckDeleteDeployment(string deploymentId)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Delete, Resources.Deployment, deploymentId);
        }

        public virtual void CheckReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.DecisionDefinition, decisionDefinition.Key);
        }

        public virtual void CheckReadDecisionRequirementsDefinition(
            DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.DecisionRequirementsDefinition,
                decisionRequirementsDefinition.Key);
        }

        public virtual void CheckReadCaseDefinition(ICaseDefinition caseDefinition)
        {
        }

        public void CheckCreateProcessInstance(ProcessDefinitionEntity processDefinition)
        {

        }

        //delete permission ////////////////////////////////////////

        public virtual void CheckDeleteHistoricTaskInstance(HistoricTaskInstanceEventEntity task)
        {
            //deleting unexisting historic ITask instance should be silently ignored
            //see javaDoc HistoryService.deleteHistoricTaskInstance
            if (task != null)
            {
                if (!ReferenceEquals(task.ProcessDefinitionKey, null))
                {
                    AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.ProcessDefinition,
                        task.ProcessDefinitionKey);
                }
            }
        }

        //delete permission /////////////////////////////////////////////////

        public virtual void CheckDeleteHistoricProcessInstance(IHistoricProcessInstance instance)
        {
            AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.ProcessDefinition, instance.ProcessDefinitionKey);
        }

        public virtual void CheckDeleteHistoricCaseInstance(IHistoricCaseInstance instance)
        {
        }

        public virtual void CheckDeleteHistoricDecisionInstance(string decisionDefinitionKey)
        {
            AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.DecisionDefinition, decisionDefinitionKey);
        }

        public virtual void CheckDeleteHistoricDecisionInstance(IHistoricDecisionInstance decisionInstance)
        {
            AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.DecisionDefinition,
                decisionInstance.DecisionDefinitionKey);
        }

        public virtual void CheckReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog)
        {
            if (!ReferenceEquals(historicJobLog.ProcessDefinitionKey, null))
            {
                AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.ProcessDefinition,
                    historicJobLog.ProcessDefinitionKey);
            }
        }

        public virtual void CheckReadHistoryAnyProcessDefinition()
        {
            AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.ProcessDefinition, AuthorizationFields.Any);
        }

        public virtual void CheckReadHistoryAnyTaskInstance()
        {
            AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.Task, AuthorizationFields.Any);
        }

        public virtual void CheckUpdateCaseInstance(ICaseExecution caseExecution)
        {
        }

        public virtual void CheckReadCaseInstance(ICaseExecution caseExecution)
        {
        }

        public virtual void CheckTaskAssign(TaskEntity task)
        {
            var taskId = task.Id;

            var executionId = task.ExecutionId;
            if (!ReferenceEquals(executionId, null))
            {
                //Permissions to ITask actions is based on the order in which PermissioncheckBuilder is built
                var taskWorkPermission =
                    (new PermissionCheckBuilder()).Disjunctive()
                        .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.TaskAssign)
                        .AtomicCheckForResourceId(Resources.ProcessDefinition, task.ProcessDefinition.Key, Permissions.TaskAssign)
                        .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.Update)
                        .AtomicCheckForResourceId(Resources.ProcessDefinition, task.ProcessDefinition.Key, Permissions.UpdateTask)
                        .Build();

                AuthorizationManager.CheckAuthorization(taskWorkPermission);
            }
            else
            {
                //if ITask does not exist in context of process
                //instance, then it is either a (a) standalone ITask
                //or (b) it exists in context of a case instance.

                //(a) standalone ITask: check following permission
                //- TASK_ASSIGN or UPDATE
                //(b) ITask in context of a case instance, in this
                //case it is not necessary to check any permission,
                //because such tasks can always be updated

                var caseExecutionId = task.CaseExecutionId;
                if (ReferenceEquals(caseExecutionId, null))
                {
                    //standalone ITask
                    var taskWorkPermission =
                        (new PermissionCheckBuilder()).Disjunctive()
                            .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.TaskAssign)
                            .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.Update)
                            .Build();

                    AuthorizationManager.CheckAuthorization(taskWorkPermission);
                }
            }
        }

        //create permission /////////////////////////////////////////////

        public virtual void CheckCreateTask(TaskEntity entity)
        {
            AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Task);
        }

        public virtual void CheckCreateTask()
        {
            AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Task);
        }

        public virtual void CheckTaskWork(TaskEntity task)
        {
            var taskId = task.Id;

            var executionId = task.ExecutionId;
            if (executionId != null)
            {
                //Permissions to Task actions is based on the order in which PermissioncheckBuilder is built
                PermissionCheckBuilder taskWorkPermission = new PermissionCheckBuilder();
                taskWorkPermission.Disjunctive();
                taskWorkPermission.AtomicCheckForResourceId(Resources.Task, taskId, Permissions.TaskWork);
                UserOperationLogManager.Log.LogDebug("task.ProcessDefinition is null?", $"{task.ProcessDefinition==null},task.ProcessDefinition.Id:{task.ProcessDefinitionId}");
                taskWorkPermission.AtomicCheckForResourceId(Resources.ProcessDefinition, task.ProcessDefinition.Key, Permissions.TaskWork);
                taskWorkPermission.AtomicCheckForResourceId(Resources.Task, taskId, Permissions.Update);
                taskWorkPermission.AtomicCheckForResourceId(Resources.ProcessDefinition, task.ProcessDefinition.Key, Permissions.UpdateTask);
                var ckeck= taskWorkPermission.Build();

                AuthorizationManager.CheckAuthorization(ckeck);
            }
            else
            {
                //if ITask does not exist in context of process
                //instance, then it is either a (a) standalone ITask
                //or (b) it exists in context of a case instance.

                //(a) standalone ITask: check following permission
                //- TASK_WORK or UPDATE
                //(b) ITask in context of a case instance, in this
                //case it is not necessary to check any permission,
                //because such tasks can always be updated

                var caseExecutionId = task.CaseExecutionId;
                if (ReferenceEquals(caseExecutionId, null))
                {
                    //standalone task
                    var taskWorkPermission =
                        (new PermissionCheckBuilder()).Disjunctive()
                            .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.TaskWork)
                            .AtomicCheckForResourceId(Resources.Task, taskId, Permissions.Update)
                            .Build();

                    AuthorizationManager.CheckAuthorization(taskWorkPermission);
                }
            }
        }

        public virtual void CheckDeleteTask(TaskEntity task)
        {
            var taskId = task.Id;

            //Note: Calling TaskService#deleteTask() to
            //delete a ITask which exists in context of
            //a process instance or case instance cannot
            //be deleted. In such a case TaskService#deleteTask()
            //throws an exception before invoking the
            //authorization check.

            var executionId = task.ExecutionId;
            var caseExecutionId = task.CaseExecutionId;

            if (ReferenceEquals(executionId, null) && ReferenceEquals(caseExecutionId, null))
            {
                AuthorizationManager.CheckAuthorization(Permissions.Delete, Resources.Task, taskId);
            }
        }

        public virtual void CheckDeleteUserOperationLog(IUserOperationLogEntry entry)
        {
            if (entry != null)
            {
                var processDefinitionKey = entry.ProcessDefinitionKey;
                if (!ReferenceEquals(processDefinitionKey, null))
                {
                    AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.ProcessDefinition, processDefinitionKey);
                }
            }
        }

        public virtual void CheckReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog)
        {
            if (!ReferenceEquals(historicExternalTaskLog.ProcessDefinitionKey, null))
            {
                AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.ProcessDefinition,
                    historicExternalTaskLog.ProcessDefinitionKey);
            }
        }


        protected internal virtual ProcessDefinitionEntity FindLatestProcessDefinitionById(string processDefinitionId)
        {
            return Context.CommandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
        }

        protected internal virtual ExecutionEntity FindExecutionById(string processInstanceId)
        {
            return Context.CommandContext.ExecutionManager.FindExecutionById(processInstanceId);
        }

    }
}

