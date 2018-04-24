using System;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Denotes an 'activity' in the sense of BPMN 2.0:
    ///     a parent class for all tasks, subprocess and callActivity.
    ///     
    /// </summary>
    public class AbstractBpmnActivityBehavior : FlowNodeActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BpmnBehaviorLogger;

        protected internal virtual bool TransactionNotActive
        {
            get { return !context.Impl.Context.CommandContext.TransactionContext.IsTransactionActive; }
        }

        /// <summary>
        ///     Subclasses that call leave() will first pass through this method, before
        ///     the regular <seealso cref="FlowNodeActivityBehavior#leave(ActivityExecution)" /> is
        ///     called.
        /// </summary>
        public override void DoLeave(IActivityExecution execution)
        {
            var currentActivity = execution.Activity;
            var compensationHandler = ((ActivityImpl)currentActivity).findCompensationHandler();

            // subscription for compensation event subprocess is already created
            if ((compensationHandler != null) && !IsCompensationEventSubprocess(compensationHandler))
                CreateCompensateEventSubscription(execution, compensationHandler);
            base.DoLeave(execution);
        }

        protected internal virtual bool IsCompensationEventSubprocess(ActivityImpl activity)
        {
            return activity.CompensationHandler && activity.SubProcessScope && activity.TriggeredByEvent;
        }

        protected internal virtual void CreateCompensateEventSubscription(IActivityExecution execution,
            ActivityImpl compensationHandler)
        {
            // the compensate event subscription is created at subprocess or miBody of the the current activity
            var currentActivity = execution.Activity;
            var scopeExecution = execution.FindExecutionForFlowScope(currentActivity.FlowScope);

            EventSubscriptionEntity.CreateAndInsert((ExecutionEntity)scopeExecution, EventType.Compensate, compensationHandler);
        }
        
        protected internal virtual void PropagateExceptionAsError(System.Exception exception, IActivityExecution execution)
        {
            if (IsProcessEngineExceptionWithoutCause(exception) || TransactionNotActive)
                throw exception;
            PropagateError(null, exception.Message, exception, execution);
        }

        /// <summary>
        ///     Takes an <seealso cref="IActivityExecution" />  and wraps
        ///     the call to the Callable with the proper error propagation. This method
        ///     also makes sure that exceptions not caught by following activities in the
        ///     process will be thrown and not propagated.
        /// </summary>
        /// <param name="execution"> </param>
        /// <param name="toExecute"> </param>
        /// <exception cref="exception"> </exception>
        protected internal virtual void ExecuteWithErrorPropagation(IActivityExecution execution,
            Action toExecute)
        {
            var activityInstanceId = execution.ActivityInstanceId;
            try
            {
                toExecute.Invoke();
            }
            catch (System.Exception ex)
            {
                execution.CreateIncident(IncidentFields.FailedTask,"",ex.ToString().Length>1500? ex.ToString().Substring(0,1500): ex.ToString());
                if (activityInstanceId==execution.ActivityInstanceId)
                {
                    try
                    {
                        PropagateException(execution, ex);
                    }
                    catch (ErrorPropagationException e)
                    {
                        LOG.ErrorPropagationException(activityInstanceId, e.InnerException);
                        // re-throw the original exception so that it is logged
                        // and set as cause of the failure
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        ///     Decides how to propagate the exception properly, e.g. as bpmn error or "normal" error.
        /// </summary>
        /// <param name="execution"> the current execution </param>
        /// <param name="ex"> the exception to propagate </param>
        /// <exception cref="Exception"> if no error handler could be found </exception>
        protected internal virtual void PropagateException(IActivityExecution execution, System.Exception ex)
        {
            var bpmnError = CheckIfCauseOfExceptionIsBpmnError(ex);
            if (bpmnError != null)
                PropagateBpmnError(bpmnError, execution);
            else
                PropagateExceptionAsError(ex, execution);
        }

        /// <summary>
        ///     Searches recursively through the exception to see if the exception itself
        ///     or one of its causes is a <seealso cref="BpmnError" />.
        /// </summary>
        /// <param name="e">
        ///     the exception to check
        /// </param>
        /// <returns>
        ///     the BpmnError that was the cause of this exception or null if no
        ///     BpmnError was found
        /// </returns>
        protected internal virtual BpmnError CheckIfCauseOfExceptionIsBpmnError(System.Exception e)
        {
            if (e is BpmnError)
                return (BpmnError)e;
            if (e.InnerException == null)
                return null;
            return CheckIfCauseOfExceptionIsBpmnError(e.InnerException);
        }

        protected internal virtual bool IsProcessEngineExceptionWithoutCause(System.Exception exception)
        {
            return exception is ProcessEngineException && (exception.InnerException == null);
        }
        protected internal virtual void PropagateBpmnError(BpmnError error, IActivityExecution execution)
        {
            PropagateError(error.ErrorCode, error.Message, null, execution);
        }
        
        protected internal virtual void PropagateError(string errorCode, string errorMessage, System.Exception origException,
            IActivityExecution execution)
        {
            var walker = new ActivityExecutionHierarchyWalker(execution);
            
            var errorDeclarationFinder = new ErrorDeclarationForProcessInstanceFinder(this, origException, errorCode,
                execution.Activity);
            var activityExecutionMappingCollector = new ActivityExecutionMappingCollector(execution);

            walker.AddScopePreVisitor(errorDeclarationFinder);
            walker.AddExecutionPreVisitor(activityExecutionMappingCollector);
            // map variables to super executions in the hierarchy of called process instances
            walker.AddExecutionPreVisitor(new OutputVariablesPropagator());

            try
            {
                walker.WalkUntil((element) => errorDeclarationFinder.ErrorEventDefinition != null || element == null);
            }
            catch (System.Exception e)
            {
                //separate the exception handling to support a fail-safe error propagation
                throw new ErrorPropagationException(this, e);
            }

            var errorHandlingActivity = errorDeclarationFinder.ErrorHandlerActivity;

            // process the error
            if (errorHandlingActivity == null)
            {
                if (origException == null)
                {
                    Log.MissingBoundaryCatchEvent(execution.Activity.Id, errorCode);
                    execution.End(true);
                }
                else
                {
                    // throw original exception
                    throw origException;
                }
            }
            else
            {
                var errorDefinition = errorDeclarationFinder.ErrorEventDefinition;
                var errorHandlingExecution =
                    activityExecutionMappingCollector.GetExecutionForScope(errorHandlingActivity.EventScope);

                if (!ReferenceEquals(errorDefinition.ErrorCodeVariable, null))
                    errorHandlingExecution.SetVariable(errorDefinition.ErrorCodeVariable, errorCode);
                if (!ReferenceEquals(errorDefinition.ErrorMessageVariable, null))
                    errorHandlingExecution.SetVariable(errorDefinition.ErrorMessageVariable, errorMessage);
                errorHandlingExecution.ExecuteActivity(errorHandlingActivity);
            }
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            if (CompensationUtil.SignalCompensationDone.Equals(signalName))
                SignalCompensationDone(execution);
            else
                base.Signal(execution, signalName, signalData);
        }

        protected internal virtual void SignalCompensationDone(IActivityExecution execution)
        {
            // default behavior is to join compensating executions and propagate the signal if all executions have compensated

            // only wait for non-event-scope executions cause a compensation event subprocess consume the compensation event and
            // do not have to compensate embedded subprocesses (which are still non-event-scope executions)

            if (((PvmExecutionImpl)execution).NonEventScopeExecutions.Count == 0)
                if (execution.Parent != null)
                {
                    var parent = execution.Parent;
                    execution.Remove();
                    parent.Signal(CompensationUtil.SignalCompensationDone, null);
                }
        }


        public class ErrorDeclarationForProcessInstanceFinder : ITreeVisitor<IPvmScope>
        {
            private readonly AbstractBpmnActivityBehavior _outerInstance;
            protected internal IPvmActivity CurrentActivity;
            protected internal string ErrorCode;
            protected internal ErrorEventDefinition errorEventDefinition;
            protected internal IPvmActivity errorHandlerActivity;


            protected internal System.Exception Exception;

            public ErrorDeclarationForProcessInstanceFinder(AbstractBpmnActivityBehavior outerInstance,
                System.Exception exception, string errorCode, IPvmActivity currentActivity)
            {
                this._outerInstance = outerInstance;
                this.Exception = exception;
                this.ErrorCode = errorCode;
                this.CurrentActivity = currentActivity;
            }

            public virtual IPvmActivity ErrorHandlerActivity
            {
                get { return errorHandlerActivity; }
            }

            public virtual ErrorEventDefinition ErrorEventDefinition
            {
                get { return errorEventDefinition; }
            }

            public virtual void Visit(IPvmScope scope)
            {
                var errorEventDefinitions = scope.Properties.Get(BpmnProperties.ErrorEventDefinitions);
                foreach (var errorEventDefinition in errorEventDefinitions)
                {
                    var activityHandler = scope.ProcessDefinition.FindActivity(errorEventDefinition.HandlerActivityId);
                    if (!IsReThrowingErrorEventSubprocess(activityHandler) &&
                        (((Exception != null) && errorEventDefinition.CatchesException(Exception)) ||
                         ((Exception == null) && errorEventDefinition.CatchesError(ErrorCode))))
                    {
                        errorHandlerActivity = activityHandler;
                        this.errorEventDefinition = errorEventDefinition;
                        break;
                    }
                }
            }

            protected internal virtual bool IsReThrowingErrorEventSubprocess(IPvmActivity activityHandler)
            {
                var activityHandlerScope = (ScopeImpl)activityHandler;
                return activityHandlerScope.IsAncestorFlowScopeOf((ScopeImpl)CurrentActivity);
            }
        }

        protected internal class ErrorPropagationException : System.Exception
        {
            internal const long SerialVersionUid = 1L;
            private readonly AbstractBpmnActivityBehavior _outerInstance;

            public ErrorPropagationException(AbstractBpmnActivityBehavior outerInstance, System.Exception cause)
                : base(cause.Message)
            {
                this._outerInstance = outerInstance;
            }
        }
    }
}