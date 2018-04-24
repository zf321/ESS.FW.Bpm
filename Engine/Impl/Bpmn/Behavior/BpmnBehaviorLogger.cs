using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     .
    /// </summary>
    public class BpmnBehaviorLogger : ProcessEngineLogger
    {
        public virtual void MissingBoundaryCatchEvent(string executionId, string errorCode)
        {
            LogInfo("001",
                "Execution with id '{0}' throws an error event with errorCode '{1}', but no catching boundary event was defined. " +
                "Execution is ended (none end event semantics).", executionId, errorCode);
        }

        public virtual void LeavingActivity(string activityId)
        {
            LogDebug("002", "Leaving activity '{0}'.", activityId);
        }

        public virtual void MissingOutgoingSequenceFlow(string activityId)
        {
            LogDebug("003", "No outgoing sequence flow found for activity '{0}'. Ending execution.", activityId);
        }

        public virtual ProcessEngineException StuckExecutionException(string activityId)
        {
            return
                new ProcessEngineException(ExceptionMessage("004",
                    "No outgoing sequence flow for the element with id '{0}' could be selected for continuing the process.",
                    activityId));
        }

        public virtual ProcessEngineException MissingDefaultFlowException(string activityId, string defaultSequenceFlow)
        {
            return
                new ProcessEngineException(ExceptionMessage("005",
                    "Default sequence flow '{0}' for element with id '{1}' could not be not found.", defaultSequenceFlow,
                    activityId));
        }

        public virtual ProcessEngineException MissingConditionalFlowException(string activityId)
        {
            return
                new ProcessEngineException(ExceptionMessage("006",
                    "No conditional sequence flow leaving the Flow Node '{0}' could be selected for continuing the process.",
                    activityId));
        }

        public virtual ProcessEngineException IncorrectlyUsedSignalException(string className)
        {
            return
                new ProcessEngineException(ExceptionMessage("007", "signal() can only be called on a '{0}' instance.",
                    className));
        }

        public virtual ProcessEngineException MissingDelegateParentClassException(string className, string javaDelegate,
            string activityBehavior)
        {
            return
                new ProcessEngineException(ExceptionMessage("008", "Class '{0}' doesn't implement '{1}' nor '{2}'.",
                    className, javaDelegate, activityBehavior));
        }

        public virtual ProcessEngineException MissingClassException(string className )
        {
            return
                new ProcessEngineException(ExceptionMessage("008", "Class or Method '{0}' doesn't  find",className ));
        }

        public virtual void OutgoingSequenceFlowSelected(string sequenceFlowId)
        {
            LogDebug("009", "Sequence flow with id '{0}' was selected as outgoing sequence flow.", sequenceFlowId);
        }

        public virtual ProcessEngineException UnsupportedSignalException(string activityId)
        {
            return
                new ProcessEngineException(ExceptionMessage("010", "The activity with id '{0}' doesn't accept signals.",
                    activityId));
        }

        public virtual void ActivityActivation(string activityId)
        {
            LogDebug("011", "Element with id '{0}' activates.", activityId);
        }

        public virtual void NoActivityActivation(string activityId)
        {
            LogDebug("012", "Element with id '{0}' does not activate.", activityId);
        }

        public virtual void IgnoringEventSubscription(EventSubscriptionEntity eventSubscription,
            string processDefinitionId)
        {
            LogDebug("014", "Found event subscription '{0}' but process definition with id '{1}' could not be found.",
                eventSubscription.ToString(), processDefinitionId);
        }

        public virtual ProcessEngineException SendingEmailException(string recipient, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("015", "Unable to send email to recipient '{0}'.", recipient), cause);
        }

        public virtual ProcessEngineException EmailFormatException()
        {
            return
                new ProcessEngineException(ExceptionMessage("016",
                    "'html' or 'text' is required to be defined as mail format when using the mail activity."));
        }

        public virtual ProcessEngineException EmailCreationException(string format, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("017", "Unable to create a mail with format '{0}'.", format), cause);
        }

        public virtual ProcessEngineException AddRecipientException(string recipient, System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("018", "Unable to add '{0}' as recipient.", recipient),
                cause);
        }

        public virtual ProcessEngineException MissingRecipientsException()
        {
            return new ProcessEngineException(ExceptionMessage("019", "No recipient could be found for sending email."));
        }

        public virtual ProcessEngineException AddSenderException(string sender, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("020", "Could not set '{0}' as from address in email.", sender), cause);
        }

        public virtual ProcessEngineException AddCcException(string cc, System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("021", "Could not add '{0}' as cc recipient.", cc), cause);
        }

        public virtual ProcessEngineException AddBccException(string bcc, System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("022", "Could not add '{0}' as bcc recipient.", bcc),
                cause);
        }

        public virtual ProcessEngineException InvalidAmountException(string type, int amount)
        {
            return
                new ProcessEngineException(ExceptionMessage("023",
                    "Invalid number of '{0}': must be positive integer value or zero, but was '{1}'.", type, amount));
        }

        public virtual ProcessEngineException UnresolvableExpressionException(string expression, string type)
        {
            return
                new ProcessEngineException(ExceptionMessage("024", "Expression '{0}' didn't resolve to type '{1}'.",
                    expression, type));
        }

        public virtual ProcessEngineException InvalidVariableTypeException(string variable, string type)
        {
            return
                new ProcessEngineException(ExceptionMessage("025", "Variable '{0}' is not from type '{1}'.", variable,
                    type));
        }

        public virtual ProcessEngineException ResolveCollectionExpressionOrVariableReferenceException()
        {
            return
                new ProcessEngineException(ExceptionMessage("026",
                    "Couldn't resolve collection expression nor variable reference"));
        }

        public virtual ProcessEngineException ExpressionNotANumberException(string type, string expression)
        {
            return
                new ProcessEngineException(ExceptionMessage("027",
                    "Could not resolve expression from type '{0}'. Expression '{1}' needs to be a number or number String.",
                    type, expression));
        }

        public virtual ProcessEngineException ExpressionNotBooleanException(string type, string expression)
        {
            return
                new ProcessEngineException(ExceptionMessage("028",
                    "Could not resolve expression from type '{0}'. Expression '{1}' needs to evaluate to a boolean value.",
                    type, expression));
        }

        public virtual void MultiInstanceCompletionConditionState(bool? state)
        {
            LogDebug("029", "Completion condition of multi-instance satisfied: '{0}'", state);
        }

        public virtual void ActivityActivation(string activityId, int joinedExecutions, int availableExecution)
        {
            LogDebug("030", "Element with id '{0}' activates. Joined '{1}' of '{2}' available executions.", activityId,
                joinedExecutions, availableExecution);
        }

        public virtual void NoActivityActivation(string activityId, int joinedExecutions, int availableExecution)
        {
            LogDebug("031", "Element with id '{0}' does not activate. Joined '{1}' of '{2}' available executions.",
                activityId, joinedExecutions, availableExecution);
        }

        public virtual ProcessEngineException UnsupportedConcurrencyException(string scopeExecutionId, string className)
        {
            return
                new ProcessEngineException(ExceptionMessage("032",
                    "Execution '{0}' with execution behavior of class '{1}' cannot have concurrency.", scopeExecutionId,
                    className));
        }

        public virtual ProcessEngineException ResolveDelegateExpressionException(IExpression expression,
            Type parentClass,
            Type javaDelegateClass)
        {
            return
                new ProcessEngineException(ExceptionMessage("033",
                    "Delegate Expression '{0}' did neither resolve to an implementation of '{1}' nor '{2}'.", expression,
                    parentClass, javaDelegateClass));
        }

        public virtual ProcessEngineException ShellExecutionException(System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("034", "Could not execute shell command."), cause);
        }

        public virtual void ErrorPropagationException(string activityId, System.Exception cause)
        {
            LogError("035", "caught an exception while propagate error in activity with id '{0}' : {1}", activityId, cause);
        }

        public virtual void DebugConcurrentScopeIsPruned(IActivityExecution execution)
        {
            LogDebug("036", "Concurrent scope is pruned {0}", execution);
        }

        public virtual void DebugCancelConcurrentScopeExecution(IActivityExecution execution)
        {
            LogDebug("037", "Cancel concurrent scope execution {0}", execution);
        }

        public virtual void DestroyConcurrentScopeExecution(IActivityExecution execution)
        {
            LogDebug("038", "Destroy concurrent scope execution : '{0}'", execution);
        }

        public virtual void CompleteNonScopeEventSubprocess()
        {
            LogDebug("039", "Destroy non-socpe event subprocess");
        }

        public virtual void EndConcurrentExecutionInEventSubprocess()
        {
            LogDebug("040", "End concurrent execution in event subprocess");
        }

        public virtual ProcessEngineException MissingDelegateVariableMappingParentClassException(string className,
            string delegateVarMapping)
        {
            return
                new ProcessEngineException(ExceptionMessage("041", "Class '{0}' doesn't implement '{1}'.", className,
                    delegateVarMapping));
        }
    }
}