using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class CommandLogger : ProcessEngineLogger
    {
        public virtual void DebugCreatingNewDeployment()
        {
            LogDebug("001", "Creating new deployment");
        }

        public virtual void UsingExistingDeployment()
        {
            LogDebug("002", "Using existing deployment");
        }

        public virtual void DebugModificationInstruction(string processInstanceId, int i, string describe)
        {
            LogDebug("003", "Modifying process instance '{0}': Instruction {1}: {2}", processInstanceId, i, describe);
        }

        public virtual void DebugStartingInstruction(string processInstanceId, int i, string describe)
        {
            LogDebug("004", "Starting process instance '{0}': Instruction {1}: {2}", processInstanceId, i, describe);
        }

        public virtual void DebugStartingCommand<T1>(ICommand<T1> cmd)
        {
            LogDebug("005", "Starting command -------------------- {0} ----------------------",
                ClassNameUtil.GetClassNameWithoutPackage(cmd));
        }

        public virtual void DebugFinishingCommand<T1>(ICommand<T1> cmd)
        {
            LogDebug("006", "Finishing command -------------------- {0} ----------------------",
                ClassNameUtil.GetClassNameWithoutPackage(cmd));
        }

        public virtual void DebugWaitingFor(long waitTime)
        {
            LogDebug("007", "Waiting for {0} before retrying command", waitTime);
        }

        public virtual void DebugCaughtOptimisticLockingException(OptimisticLockingException e)
        {
            LogDebug("008", "caught optimistic locking excpetion", e);
        }

        public virtual void DebugOpeningNewCommandContext()
        {
            LogDebug("009", "opening new command context");
        }

        public virtual void DebugReusingExistingCommandContext()
        {
            LogDebug("010", "reusing existing command context");
        }

        public virtual void ClosingCommandContext()
        {
            LogDebug("011", "closing existing command context");
        }

        public virtual void CalledInsideTransaction()
        {
            LogDebug("012", "called inside transaction skipping");
        }

        public virtual void MaskedExceptionInCommandContext(System.Exception throwable)
        {
            LogDebug("013",
                "masked exception in command context. for root cause, see below as it will be rethrown later. {0}",
                throwable);
        }

        public virtual void ExceptionWhileRollingBackTransaction(System.Exception e)
        {
            LogError("014", "exception while rolling back transaction {0}", e);
        }

        public virtual void ExceptionWhileGettingValueForVariable(System.Exception t)
        {
            LogDebug("015", "exception while getting value for variable {0}", t.Message, t);
        }

        public virtual void CouldNotFindProcessDefinitionForEventSubscription(
            EventSubscriptionEntity messageEventSubscription, string processDefinitionId)
        {
            LogDebug("016", "Found event subscription with {0} but process definition {1} could not be found.",
                messageEventSubscription, processDefinitionId);
        }

        public virtual void DebugIgnoringEventSubscription(EventSubscriptionEntity eventSubscription,
            string processDefinitionId)
        {
            LogDebug("017", "Found event subscription with {0} but process definition {1} could not be found.",
                eventSubscription, processDefinitionId);
        }

        public virtual void DebugProcessingDeployment(string name)
        {
            LogDebug("018", "Processing deployment {0}", name);
        }

        public virtual void DebugProcessingResource(string name)
        {
            LogDebug("019", "Processing resource {0}", name);
        }

        public virtual ProcessEngineException PaWithNameNotRegistered(string name)
        {
            return
                new ProcessEngineException(ExceptionMessage("020",
                    "A process application with name '{0}' is not registered", name));
        }

        public virtual ProcessEngineException CannotReolvePa(ProcessApplicationIdentifier processApplicationIdentifier)
        {
            return
                new ProcessEngineException(ExceptionMessage("021", "Cannot resolve process application based on {0}",
                    processApplicationIdentifier));
        }

        public virtual void WarnDisabledDeploymentLock()
        {
            LogWarn("022",
                "No exclusive lock is aquired while deploying because it is disabled. " +
                "This can lead to problems when multiple process engines use the same data source (i.e. in cluster mode).");
        }

        public virtual BadUserRequestException ExceptionStartProcessInstanceByIdAndTenantId()
        {
            return
                new BadUserRequestException(ExceptionMessage("023",
                    "Cannot specify a tenant-id when start a process instance by process definition id."));
        }

        public virtual BadUserRequestException ExceptionStartProcessInstanceAtStartActivityAndSkipListenersOrMapping()
        {
            return
                new BadUserRequestException(ExceptionMessage("024",
                    "Cannot skip custom listeners or input/output mappings when start a process instance at default start activity."));
        }

        public virtual BadUserRequestException ExceptionCorrelateMessageWithProcessDefinitionId()
        {
            return
                new BadUserRequestException(ExceptionMessage("025",
                    "Cannot specify a process definition id when correlate a message, except for explicit correlation of a start message."));
        }

        public virtual BadUserRequestException ExceptionCorrelateStartMessageWithCorrelationVariables()
        {
            return
                new BadUserRequestException(ExceptionMessage("026",
                    "Cannot specify correlation variables of a process instance when correlate a start message."));
        }

        public virtual BadUserRequestException ExceptionDeliverSignalToSingleExecutionWithTenantId()
        {
            return
                new BadUserRequestException(ExceptionMessage("027",
                    "Cannot specify a tenant-id when deliver a signal to a single execution."));
        }

        public virtual BadUserRequestException ExceptionCorrelateMessageWithProcessInstanceAndTenantId()
        {
            return
                new BadUserRequestException(ExceptionMessage("028",
                    "Cannot specify a tenant-id when correlate a message to a single process instance."));
        }

        public virtual BadUserRequestException ExceptionCorrelateMessageWithProcessDefinitionAndTenantId()
        {
            return
                new BadUserRequestException(ExceptionMessage("029",
                    "Cannot specify a tenant-id when correlate a start message to a specific version of a process definition."));
        }

        public virtual MismatchingMessageCorrelationException ExceptionCorrelateMessageToSingleProcessDefinition(
            string messageName, long processDefinitionCound, CorrelationSet correlationSet)
        {
            return
                new MismatchingMessageCorrelationException(ExceptionMessage("030",
                    "Cannot correlate a message with name '{0}' to a single process definition. {1} process definitions match the correlations keys: {2}",
                    messageName, processDefinitionCound, correlationSet));
        }

        public virtual MismatchingMessageCorrelationException ExceptionCorrelateMessageToSingleExecution(
            string messageName, long executionCound, CorrelationSet correlationSet)
        {
            return
                new MismatchingMessageCorrelationException(ExceptionMessage("031",
                    "Cannot correlate a message with name '{0}' to a single execution. {1} executions match the correlation keys: {2}",
                    messageName, executionCound, correlationSet));
        }

        public virtual BadUserRequestException ExceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey()
        {
            return
                new BadUserRequestException(ExceptionMessage("032",
                    "Can only specify a tenant-id when update the suspension state which is referenced by process definition key."));
        }

        public virtual ProcessEngineException ExceptionBpmnErrorPropagationFailed(string errorCode, System.Exception cause)
        {
            return
                new ProcessEngineException(ExceptionMessage("033", "Propagation of bpmn error {0} failed. ", errorCode),
                    cause);
        }

        public virtual ProcessEngineException ExceptionCommandWithUnauthorizedTenant(string command)
        {
            return
                new ProcessEngineException(ExceptionMessage("034",
                    "Cannot {0} because it belongs to no authenticated tenant.", command));
        }

        public virtual void WarnDeploymentResourceWasIgnored(string resourceName)
        {
            LogWarn("035",
                string.Format(
                    "Deployment resource '{0}' was not processed during deployment. Hint: check file extention.",
                    resourceName));
        }

        public ProcessEngineException ProcessInstanceDoesNotExist(string processInstanceId)
        {
            return new ProcessEngineException(ExceptionMessage(
                "036",
                "Process instance '{}' cannot be modified. The process instance does not exist",
                processInstanceId));
        }

        public ProcessEngineException ProcessDefinitionOfInstanceDoesNotMatchModification(ExecutionEntity processInstance, string processDefinitionId)
        {
            return new ProcessEngineException(ExceptionMessage(
                "037",
                "Process instance '{}' cannot be modified. Its process definition '{}' does not match given process definition '{}'",
                processInstance.Id,
                processInstance.ProcessDefinitionId,
                processDefinitionId
            ));
        }

        public void DebugHistoryCleanupWrongConfiguration()
        {
            LogDebug("038", "History cleanup won't be scheduled. Either configure batch window or call it with immediatelyDue = true.");
        }

        public ProcessEngineException ProcessDefinitionOfHistoricInstanceDoesNotMatchTheGivenOne(IHistoricProcessInstance historicProcessInstance, string processDefinitionId)
        {
            return new ProcessEngineException(ExceptionMessage(
                "039",
                "Historic process instance '{}' cannot be restarted. Its process definition '{}' does not match given process definition '{}'",
                historicProcessInstance.Id,
                historicProcessInstance.ProcessDefinitionId,
                processDefinitionId
            ));
        }

        public ProcessEngineException HistoricProcessInstanceActive(IHistoricProcessInstance historicProcessInstance)
        {
            return new ProcessEngineException(ExceptionMessage(
                "040",
                "Historic process instance '{}' cannot be restarted. It is not completed or terminated.",
                historicProcessInstance.Id,
                historicProcessInstance.ProcessDefinitionId
            ));
        }
    }
}