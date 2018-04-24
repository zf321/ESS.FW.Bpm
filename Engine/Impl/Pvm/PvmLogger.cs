using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmLogger : ProcessEngineLogger
    {
        public virtual void NotTakingTranistion(IPvmTransition outgoingTransition)
        {
            LogDebug("001", "Not taking transition '{0}', outgoing execution has ended.", outgoingTransition);
        }

        public virtual void DebugExecutesActivity(PvmExecutionImpl execution, ActivityImpl activity, string name)
        {
            LogDebug("002", "{0} executed activity {1}: {2}", execution, activity, name);
        }

        public virtual void DebugLeavesActivityInstance(PvmExecutionImpl execution, string activityInstanceId)
        {
            LogDebug("003", "Execution {0} leaves activity instance {1}", execution, activityInstanceId);
        }

        public virtual void DebugDestroyScope(IActivityExecution execution, IActivityExecution propagatingExecution)
        {
            LogDebug("004", "Execution {0} leaves parent scope {1}", execution, propagatingExecution);
        }

        public virtual void Destroying(PvmExecutionImpl pvmExecutionImpl)
        {
            LogDebug("005", "Detroying scope {0}", pvmExecutionImpl);
        }

        public virtual void RemovingEventScope(IActivityExecution childExecution)
        {
            LogDebug("006", "Removeing event scope {0}", childExecution);
        }

        public virtual void InterruptingExecution(string reason, bool skipCustomListeners)
        {
            LogDebug("007", "Interrupting execution execution {0}, {1}", reason, skipCustomListeners);
        }

        public virtual void DebugEnterActivityInstance(PvmExecutionImpl pvmExecutionImpl,
            string parentActivityInstanceId)
        {
            LogDebug("008", "Enter activity instance {0} parent: {1}", pvmExecutionImpl, parentActivityInstanceId);
        }

        public virtual void ExceptionWhileCompletingSupProcess(PvmExecutionImpl execution, System.Exception e)
        {
            LogError("009", "Exception while completing subprocess of execution {0}", execution, e);
        }

        public virtual void CreateScope(PvmExecutionImpl execution, PvmExecutionImpl propagatingExecution)
        {
            LogDebug("010", "Create scope: parent exection {0} continues as  {1}", execution, propagatingExecution);
        }

        public virtual ProcessEngineException ScopeNotFoundException(string activityId, string executionId)
        {
            return
                new ProcessEngineException(ExceptionMessage("011",
                    "Scope with specified activity Id {0} and execution {1} not found", activityId, executionId));
        }
    }
}