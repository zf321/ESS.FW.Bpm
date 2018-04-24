using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Application.Impl.Event
{
    /// <summary>
    ///     <para>
    ///         <seealso cref="IExecutionListener" /> and <seealso cref="ITaskListener" /> implementation delegating to
    ///         the <seealso cref="IExecutionListener" /> and <seealso cref="ITaskListener" /> provided by a
    ///         <seealso cref="IProcessApplicationInterface" />.
    ///     </para>
    ///     <para>
    ///         If the process application does not provide an execution listener (ie.
    ///         <seealso cref="IProcessApplicationInterface#getExecutionListener()" /> returns null), the
    ///         request is silently ignored.
    ///     </para>
    ///     <para>
    ///         If the process application does not provide a ITask listener (ie.
    ///         <seealso cref="IProcessApplicationInterface#getTaskListener()" /> returns null), the
    ///         request is silently ignored.
    ///     </para>
    ///     
    /// </summary>
    /// <seealso cref= IProcessApplicationInterface# getExecutionListener
    /// (
    /// )
    /// </seealso>
    /// <seealso cref= ProcessApplicationInterface# getTaskListener
    /// (
    /// )
    /// </seealso>
    public class ProcessApplicationEventListenerDelegate : IDelegateListener<IBaseDelegateExecution>, ITaskListener
    {
        private static readonly ProcessApplicationLogger LOG = ProcessEngineLogger.ProcessApplicationLogger;
        
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            //ICallable<object> notification = new CallableAnonymousInnerClass(this, (IDelegateExecution) execution).Call();
            //PerformNotification((IDelegateExecution) execution, notification);
        }
        
        public virtual void Notify(IDelegateTask delegateTask)
        {
            if (delegateTask.Execution == null)
            {
                LOG.TaskNotRelatedToExecution(delegateTask);
            }
            else
            {
                var execution = delegateTask.Execution;
                try
                {
                    PerformNotification(execution, ()=> NotifyTaskListener(delegateTask));
                }
                catch (System.Exception e)
                {
                    throw LOG.ExceptionWhileNotifyingPaTaskListener(e);
                }
            }
        }
        
        protected internal virtual void PerformNotification(IDelegateExecution execution, Action notification)
        {
            //IProcessApplicationReference processApp = ProcessApplicationContextUtil.GetTargetProcessApplication(execution);
            //if (processApp == null)
            //{
            //    // ignore silently
            //    LOG.NoTargetProcessApplicationForExecution(execution);

            //}
            //else
            //{
            //    if (ProcessApplicationContextUtil.RequiresContextSwitch(processApp))
            //    {
            //        // this should not be necessary since context switch is already performed by OperationContext and / or DelegateInterceptor
            //        //Context.ExecuteWithinProcessApplication(notification, processApp, new InvocationContext(execution));

            //    }
            //    else
            //    {
            //        // context switch already performed
            //        notification();

            //    }
            //}
        }
        
        protected internal virtual void NotifyExecutionListener(IDelegateExecution execution)
        {
            var processApp = Context.CurrentProcessApplication;
            try
            {
                var processApplication = processApp.ProcessApplication;
                var executionListener = processApplication.ExecutionListener;
                if (executionListener != null)
                    executionListener.Notify(execution);
                else
                    LOG.PaDoesNotProvideExecutionListener(processApp.Name);
            }
            catch (ProcessApplicationUnavailableException e)
            {
                // Process Application unavailable => ignore silently
                LOG.CannotInvokeListenerPaUnavailable(processApp.Name, e);
            }
        }
       
        protected internal virtual void NotifyTaskListener(IDelegateTask ITask)
        {
            var processApp = Context.CurrentProcessApplication;
            try
            {
                var processApplication = processApp.ProcessApplication;
                var taskListener = processApplication.TaskListener;
                if (taskListener != null)
                    taskListener.Notify(ITask);
                else
                    LOG.PaDoesNotProvideTaskListener(processApp.Name);
            }
            catch (ProcessApplicationUnavailableException e)
            {
                // Process Application unavailable => ignore silently
                LOG.CannotInvokeListenerPaUnavailable(processApp.Name, e);
            }
        }
        
        
    }
}