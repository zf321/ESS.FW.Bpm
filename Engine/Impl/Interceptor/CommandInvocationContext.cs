using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using ESS.FW.Bpm.Engine.Common;
using System.Linq;
using ESS.FW.Bpm.Engine.Exception;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    #region 源码
    /// <summary>
    ///     In contrast to <seealso cref="CommandContext" />, this context holds resources that are only valid
    ///     during execution of a single command (i.e. the current command or an exception that was thrown
    ///     during its execution).
    ///     
    /// </summary>
    //public class CommandInvocationContext
    //{
    //    private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;
    //    protected internal BpmnStackTrace BpmnStackTrace = new BpmnStackTrace();
    //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
    //    //ORIGINAL LINE: protected Command< ? > command;
    //    protected internal ICommand<object> command;
    //    protected internal bool IsExecuting = false;
    //    protected internal IList<AtomicOperationInvocation> QueuedInvocations = new List<AtomicOperationInvocation>();

    //    protected internal System.Exception throwable;

    //    public CommandInvocationContext(ICommand<object> command)
    //    {
    //        this.command = command;
    //    }

    //    public virtual System.Exception Throwable
    //    {
    //        get { return throwable; }
    //    }

    //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
    //    //ORIGINAL LINE: public Command<object> getCommand()
    //    //public virtual ICommand<object> Command
    //    //{
    //    //    get { return command; }
    //    //}

    //    public virtual void TrySetThrowable(System.Exception t)
    //    {
    //        if (throwable == null)
    //            throwable = t;
    //        else
    //            Log.MaskedExceptionInCommandContext(throwable);
    //    }

    //    public virtual void PerformOperation(IAtomicOperation executionOperation, ExecutionEntity execution)
    //    {
    //        PerformOperation(executionOperation, execution, false);
    //    }

    //    public virtual void PerformOperationAsync(IAtomicOperation executionOperation, ExecutionEntity execution)
    //    {
    //        PerformOperation(executionOperation, execution, true);
    //    }

    //    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //    //ORIGINAL LINE: public void performOperation(final org.camunda.bpm.engine.impl.Pvm.Runtime.AtomicOperation executionOperation, final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution, final boolean performAsync)
    //    public virtual void PerformOperation(IAtomicOperation executionOperation, ExecutionEntity execution,
    //        bool performAsync)
    //    {
    //        //AtomicOperationInvocation invocation = new AtomicOperationInvocation(executionOperation, execution, performAsync);
    //        //queuedInvocations.Insert(0, invocation);
    //        PerformNext();
    //    }

    //    protected internal virtual void PerformNext()
    //    {
    //        var nextInvocation = QueuedInvocations[0];

    //        if (nextInvocation.Operation.AsyncCapable && IsExecuting)
    //        {
    //            // will be picked up by while loop below
    //        }

    //        //          ProcessApplicationReference targetProcessApplication = getTargetProcessApplication(nextInvocation.execution);
    //        //          if (requiresContextSwitch(targetProcessApplication))
    //        //{

    //        //  //Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this), targetProcessApplication, new InvocationContext(nextInvocation.execution));
    //        //}
    //        //else
    //        //{
    //        //  if (!nextInvocation.Operation.AsyncCapable)
    //        //  {
    //        //	// if operation is not async capable, perform right away.
    //        //	invokeNext();
    //        //  }
    //        //  else
    //        //  {
    //        //	try
    //        //	{
    //        //	  isExecuting = true;
    //        //	  while (queuedInvocations.Count > 0)
    //        //	  {
    //        //		// assumption: all operations are executed within the same process application...
    //        //		nextInvocation = queuedInvocations[0];
    //        //		invokeNext();
    //        //	  }
    //        //	}
    //        //	finally
    //        //	{
    //        //	  isExecuting = false;
    //        //	}
    //        //  }
    //        //}
    //    }

    //    protected internal virtual void InvokeNext()
    //    {
    //        //AtomicOperationInvocation invocation = queuedInvocations.RemoveAt(0);
    //        //try
    //        //{
    //        //  invocation.execute(bpmnStackTrace);
    //        //}
    //        //catch (Exception e)
    //        //{
    //        //  // log bpmn stacktrace
    //        //  bpmnStackTrace.printStackTrace(Context.ProcessEngineConfiguration.BpmnStacktraceVerbose);
    //        //  // rethrow
    //        //  throw e;
    //        //}
    //    }

    //    protected internal virtual bool RequiresContextSwitch(IProcessApplicationReference processApplicationReference)
    //    {
    //        return ProcessApplicationContextUtil.RequiresContextSwitch(processApplicationReference);
    //    }

    //    protected internal virtual IProcessApplicationReference GetTargetProcessApplication(ExecutionEntity execution)
    //    {
    //        //return ProcessApplicationContextUtil.getTargetProcessApplication(execution);
    //        return null;
    //    }

    //    public virtual void Rethrow()
    //    {
    //        if (throwable != null)
    //        {
    //            // if (throwable is Exception)
    //            // {
    //            //throw (Exception) throwable;
    //            // }
    //            // else if (throwable is PersistenceException)
    //            // {
    //            //throw new ProcessEngineException("Process engine persistence exception", throwable);
    //            // }
    //            // else if (throwable is Exception)
    //            // {
    //            //throw (Exception) throwable;
    //            // }
    //            // else
    //            // {
    //            //throw new ProcessEngineException("exception while executing command " + command, throwable);
    //            // }
    //        }
    //    }

    //    private class CallableAnonymousInnerClass /*: Callable<object>*/
    //    {
    //        private readonly CommandInvocationContext _outerInstance;

    //        public CallableAnonymousInnerClass(CommandInvocationContext outerInstance)
    //        {
    //            this._outerInstance = outerInstance;
    //        }

    //        //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
    //        //ORIGINAL LINE: public void call() throws Exception
    //        public virtual void Call()
    //        {
    //            _outerInstance.PerformNext();
    //            //return null;
    //        }
    //    }
    //}
    #endregion

    public class CommandInvocationContext
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;
        protected internal BpmnStackTrace BpmnStackTrace = new BpmnStackTrace();
        protected internal ICommand<object> command;
        /// <summary>
        /// 执行器允许异步执行时 手动设置成了true
        /// </summary>
        protected internal bool isExecuting = false;
        /// <summary>
        /// 待执行的队列
        /// </summary>
        protected internal Queue<AtomicOperationInvocation> queuedInvocations = new Queue<AtomicOperationInvocation>();
        protected internal System.Exception throwable;

        public CommandInvocationContext(ICommand<object> command)
        {
            this.command = command;
        }

        public virtual System.Exception Throwable
        {
            get { return throwable; }
        }

        public virtual ICommand<object> Command
        {
            get { return command; }
        }

        public virtual void TrySetThrowable(System.Exception t)
        {
            if (throwable == null)
                throwable = t;
            else
                Log.MaskedExceptionInCommandContext(throwable);
        }
        /// <summary>
        /// 添加一个同步执行的执行器到队列queuedInvocations并执行
        /// </summary>
        /// <param name="executionOperation"></param>
        /// <param name="execution"></param>
        public virtual void PerformOperation(IAtomicOperation executionOperation, ExecutionEntity execution)
        {
            PerformOperation(executionOperation, execution, false);
        }
        /// <summary>
        /// 添加一个异步执行的执行器到队列queuedInvocations 并执行
        /// </summary>
        /// <param name="executionOperation">执行器</param>
        /// <param name="execution"></param>
        public virtual void PerformOperationAsync(IAtomicOperation executionOperation, ExecutionEntity execution)
        {
            PerformOperation(executionOperation, execution, true);
        }
        /// <summary>
        /// 添加一个执行器到队列queuedInvocations并执行
        /// </summary>
        /// <param name="executionOperation">执行器</param>
        /// <param name="execution"></param>
        /// <param name="performAsync">是否异步</param>
        public virtual void PerformOperation(IAtomicOperation executionOperation, ExecutionEntity execution,
            bool performAsync)
        {
            AtomicOperationInvocation invocation = new AtomicOperationInvocation(executionOperation, execution, performAsync);
            queuedInvocations.Enqueue(invocation);//.Insert(0, invocation);
            PerformNext();
        }
        /// <summary>
        ///用queuedInvocations里面的executionOperation执行execution
        /// </summary>
        protected internal virtual void PerformNext()
        {
            var nextInvocation = queuedInvocations.Peek();//[0];
            //如果执行器允许异步执行，且已经进入异步操作 直接返回
            if (nextInvocation.Operation.AsyncCapable && isExecuting)
            {
                // will be picked up by while loop below
                return;
            }

            IProcessApplicationReference targetProcessApplication = GetTargetProcessApplication(nextInvocation.execution);
            if (RequiresContextSwitch(targetProcessApplication))
            {
                Context.ExecuteWithinProcessApplication<object>(() =>
                {
                    PerformNext();
                    return null;
                }, targetProcessApplication, new InvocationContext(nextInvocation.execution));
            }
            else
            {
                if (!nextInvocation.Operation.AsyncCapable)//如果执行器不支持异步操作 直接执行
                {
                    // if operation is not async capable, perform right away.
                    InvokeNext();
                }
                else//异步操作
                {
                    try
                    {
                        isExecuting = true;
                        while (queuedInvocations.Count > 0)
                        {
                            // assumption: all operations are executed within the same process application...
                            nextInvocation = queuedInvocations.Peek();//[0];
                            InvokeNext();
                        }
                    }
                    finally
                    {
                        isExecuting = false;
                    }
                }
            }
        }

        protected internal virtual void InvokeNext()
        {
            //AtomicOperationInvocation invocation = queuedInvocations.RemoveAt(0);
            AtomicOperationInvocation invocation = queuedInvocations.Dequeue(); ;//.FirstOrDefault();
            //TODO 暂时取消异常拦截
            try
            {
                invocation.Execute(BpmnStackTrace);
            }
            catch (System.Exception e)
            {
                // log bpmn stacktrace
                BpmnStackTrace.PrintStackTrace(Context.ProcessEngineConfiguration.BpmnStacktraceVerbose);
                TrySetThrowable(e);
                Rethrow();
            }
        }

        protected internal virtual bool RequiresContextSwitch(IProcessApplicationReference processApplicationReference)
        {
            return ProcessApplicationContextUtil.RequiresContextSwitch(processApplicationReference);
        }

        protected internal virtual IProcessApplicationReference GetTargetProcessApplication(ExecutionEntity execution)
        {
            return ProcessApplicationContextUtil.GetTargetProcessApplication(execution);
        }

        public virtual void Rethrow()
        {
            //TODO 自定义异常（可能丢失了层级细节）
            if (throwable != null)
            {
                //if (throwable is System.Exception)
                //{
                //    throw (System.Exception)throwable;
                //}
                //else
                if (throwable is PersistenceException)
                {
                    throw new ProcessEngineException("Process engine persistence exception :"+throwable.Message, throwable);
                }
                else if (throwable is RuntimeException)
                {
                    throw throwable;
                }
                else if (throwable is OptimisticLockingException)
                    throw throwable;
                else
                {
                    throw new ProcessEngineException(string.Format("exception while executing command[{0}]:{1}] ",command,throwable.Message), throwable);
                }
            }
        }

    }
}