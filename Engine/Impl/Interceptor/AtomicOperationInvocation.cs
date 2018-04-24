using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Exceptions;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     An invocation of an atomic operation
    ///     一个原子操作调用
    /// </summary>
    public class AtomicOperationInvocation
    {
        private static readonly ContextLogger Log = ProcessEngineLogger.ContextLogger;
        protected internal string activityId;

        // for logging
        protected internal string applicationContextName;

        protected internal ExecutionEntity execution;

        protected internal IAtomicOperation operation;

        protected internal bool performAsync;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation">执行器</param>
        /// <param name="execution">ExecutionEntity</param>
        /// <param name="performAsync">是否异步</param>
        public AtomicOperationInvocation(IAtomicOperation operation, ExecutionEntity execution, bool performAsync)
        {
            Init(operation, execution, performAsync);
        }

        // getters / setters ////////////////////////////////////

        public virtual IAtomicOperation Operation
        {
            get { return operation; }
        }

        public virtual ExecutionEntity Execution
        {
            get { return execution; }
        }

        public virtual bool PerformAsync
        {
            get { return performAsync; }
        }
        /// <summary>
        /// Application上下文名称 供Logging用
        /// </summary>
        public virtual string ApplicationContextName
        {
            get { return applicationContextName; }
        }

        public virtual string ActivityId
        {
            get { return activityId; }
        }

        protected internal virtual void Init(IAtomicOperation operation, ExecutionEntity execution, bool performAsync)
        {
            this.operation = operation;
            this.execution = execution;
            this.performAsync = performAsync;
        }

        public virtual void Execute(BpmnStackTrace stackTrace)
        {
            if ((operation != PvmAtomicOperationFields.ActivityStartCancelScope) &&
                (operation != PvmAtomicOperationFields.ActivityStartInterruptScope) &&
                (operation != PvmAtomicOperationFields.ActivityStartConcurrent))
            {
                // execution might be replaced in the meantime:
                ExecutionEntity replacedBy = (ExecutionEntity) execution.ReplacedBy;
                if (replacedBy != null)
                {
                    execution = replacedBy;
                }
            }

            //execution was canceled for example via terminate end event
            if (execution.Canceled &&
                (operation == PvmAtomicOperationFields.TransitionNotifyListenerEnd ||
                 operation == PvmAtomicOperationFields.ActivityNotifyListenerEnd))
            {
                return;
            }

            //// execution might have ended in the meanwhile
            if (execution.IsEnded &&
                (operation == PvmAtomicOperationFields.TransitionNotifyListenerTake ||
                 operation == PvmAtomicOperationFields.ActivityStartCreateScope))
            {
                return;
            }
            //TODO Context.CurrentProcessApplication
            var currentPa = context.Impl.Context.CurrentProcessApplication;
            if (currentPa != null)
                applicationContextName = currentPa.Name;
            activityId = execution.ActivityId;
            stackTrace.Add(this);

            try
            {
                Context.SetExecutionContext(execution);
                if (!performAsync)//同步
                {
                    Log.DebugExecutingAtomicOperation(operation, execution);
                    operation.Execute(execution);
                }
                else//异步
                {
                    execution.ScheduleAtomicOperationAsync(this);
                }
            }
            catch(ClassMethodDelegateException ex)
            {
                if (execution.IsInTransaction)
                {
                    System.Transactions.Transaction tr = Context.CommandContext.CurrentTransaction;
                    Log.LogDebug("外部代码异常，事务Id:", tr.TransactionInformation.LocalIdentifier);
                    if (context.Impl.Context.CommandContext.CurrentTransaction != null)
                    {
                        Log.LogDebug("移除当前外部事务，添加到待回滚中。", context.Impl.Context.CommandContext.CurrentTransaction.TransactionInformation.LocalIdentifier);
                        context.Impl.Context.CommandContext.MoveToRollBack();
                    }

                }
                else
                {
                    Log.LogDebug("外部代码不在事务范围内,ActivityId:" + execution.ActivityId, ex.Message);
                }
            }
            //debug抛异常
            catch(System.Exception e)
            {
                throw e;
            }
            finally
            {
                context.Impl.Context.RemoveExecutionContext();
            }
        }
    }
}