using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.FW.Bpm.Engine.Impl.Core.Instance
{
    /// <summary>
    ///     Defines the base API for the execution of an activity.
    /// </summary>
    [Serializable]
    public abstract class CoreExecution : AbstractVariableScope, IBaseDelegateExecution
    {
        

        private readonly CoreLogger _logger = ProcessEngineLogger.CoreLogger;

        //internal CaseExecutionEntity GetCaseInstance()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        ///     the business key for this execution
        /// </summary>

        // events ///////////////////////////////////////////////////////////////////
        
        

        [NotMapped]
        public virtual CoreModelElement EventSource
        {get; set; }

        [NotMapped]
        public virtual int ListenerIndex { get; set; }


        public virtual string TenantId { get; set; }

        [NotMapped]
        public virtual bool SkipCustomListeners { get; set; }

        [NotMapped]
        public virtual bool SkipIoMapping { get; set; }
        public virtual string BusinessKey { get; set; }
        // event handling ////////////////////////////////////////////////////////
        [NotMapped]
        public virtual string EventName { get; set; }

        // getters / setters /////////////////////////////////////////////////

        public virtual string Id { get; set; }




        // atomic operations ////////////////////////////////////////////////////////

        public virtual void PerformOperation<T>(ICoreAtomicOperation<T> operation) where T : CoreExecution
        {
            _logger.DebugPerformingAtomicOperation(operation, this);
            operation.Execute((T) this);
        }
        
        public virtual void PerformOperationSync<T>(ICoreAtomicOperation<T> operation) where T : CoreExecution
        {
            _logger.DebugPerformingAtomicOperation(operation, this);
            operation.Execute((T) this);
        }


        public void InvokeListener(IDelegateListener<IBaseDelegateExecution> listener)
        {
            listener.Notify(this);
        }

        
    }
}