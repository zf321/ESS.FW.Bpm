using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationProcessEnd : PvmAtomicOperationActivityInstanceEnd
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameEnd; }
        }

        public override string CanonicalName
        {
            get { return "process-end"; }
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            execution.LeaveActivityInstance();

            PvmExecutionImpl superExecution = (PvmExecutionImpl) execution.SuperExecution;

            ISubProcessActivityBehavior subProcessActivityBehavior = null;

            // copy variables before destroying the ended sub process instance
            if (superExecution != null)
            {
                IPvmActivity activity = superExecution.Activity;
                Log.LogDebug("-----------------ActivityBehavior类型异常", activity.GetType().Name);
                subProcessActivityBehavior = (ISubProcessActivityBehavior)activity.ActivityBehavior;

                try
                {
                    subProcessActivityBehavior.PassOutputVariables(superExecution, execution);
                }
                catch (System.Exception e)
                {
                    Log.ExceptionWhileCompletingSupProcess(execution, e);
                    throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
                }
            }
            //else if (superCaseExecution != null)
            //{
            //    CmmnActivity activity = superCaseExecution.Activity;
            //    transferVariablesBehavior = (TransferVariablesActivityBehavior)activity.ActivityBehavior;
            //    try
            //    {
            //        transferVariablesBehavior.transferVariables(execution, superCaseExecution);
            //    }
            //    catch (Exception e)
            //    {
            //        LOG.exceptionWhileCompletingSupProcess(execution, e);
            //        throw e;
            //    }
            //    catch (Exception e)
            //    {
            //        LOG.exceptionWhileCompletingSupProcess(execution, e);
            //        throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
            //    }
            //}

            execution.Destroy();
            execution.Remove();

            // and trigger execution afterwards
            if (superExecution != null)
            {
                superExecution.SubProcessInstance = null;
                try
                {
                    subProcessActivityBehavior.Completed(superExecution);
                }
                catch (System.Exception e)
                {
                    Log.ExceptionWhileCompletingSupProcess(execution, e);
                    throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
                }
            }
            //else if (superCaseExecution != null)
            //{
            //    superCaseExecution.complete();
            //}
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.ProcessDefinition;
        }

    }
}