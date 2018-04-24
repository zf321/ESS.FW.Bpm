using System;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     The default implementation of the DelegateInterceptor.
    ///     <p />
    ///     This implementation has the following features:
    ///     <ul>
    ///         <li>it performs context switch into the target process application (if applicable)</li>
    ///         <li>
    ///             it checks autorizations if
    ///             <seealso cref="ProcessEngineConfigurationImpl#isAuthorizationEnabledForCustomCode()" /> is true
    ///         </li>
    ///     </ul>
    ///     
    ///     
    /// </summary>
    public class DefaultDelegateInterceptor : IDelegateInterceptor
    {
        public virtual void HandleInvocation(DelegateInvocation invocation)
        {
            var processApplication = GetProcessApplicationForInvocation(invocation);

            if ((processApplication != null) && ProcessApplicationContextUtil.RequiresContextSwitch(processApplication))
            {
                Context.ExecuteWithinProcessApplication<object>(() =>
                {
                    HandleInvocation(invocation);
                    return null;
                },
                    processApplication, new InvocationContext(invocation.ContextExecution));
            }
            else
            {
                HandleInvocationInContext(invocation);
            }
        }
        
        protected internal virtual void HandleInvocationInContext(DelegateInvocation invocation)
        {
            var commandContext = Context.CommandContext;
            var oldValue = commandContext.AuthorizationCheckEnabled;
            var contextExecution = invocation.ContextExecution;

            ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

            var popExecutionContext = false;

            try
            {
                if (!configuration.AuthorizationEnabledForCustomCode)
                {
                    // the custom code should be executed without authorization
                    commandContext.DisableAuthorizationCheck();
                }

                try
                {
                    commandContext.DisableUserOperationLog();

                    try
                    {
                        if ((contextExecution != null) && !IsCurrentContextExecution(contextExecution))
                            popExecutionContext = SetExecutionContext(contextExecution);

                        invocation.Proceed();
                    }
                    //TODO Debug
                    catch(System.Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (popExecutionContext)
                            Context.RemoveExecutionContext();
                    }
                }
                finally
                {
                    commandContext.EnableUserOperationLog();
                }
            }
            finally
            {
                if (oldValue)
                    commandContext.EnableAuthorizationCheck();
            }
        }

        /// <returns> true if the execution context is modified by this invocation </returns>
        protected internal virtual bool SetExecutionContext(IBaseDelegateExecution execution)
        {
            if (execution is ExecutionEntity)
                return true;
            //if (execution is CaseExecutionEntity)
            //    return true;
            return false;
        }

        protected internal virtual bool IsCurrentContextExecution(IBaseDelegateExecution execution)
        {
            var coreExecutionContext = Context.CoreExecutionContext;
            //return coreExecutionContext != null && coreExecutionContext.Execution == execution;
            return coreExecutionContext != null && coreExecutionContext.GetExecution<CoreExecution>() == execution;
        }
        
        protected internal virtual IProcessApplicationReference GetProcessApplicationForInvocation(
            DelegateInvocation invocation)
        {
            var contextExecution = invocation.ContextExecution;
            var contextResource = invocation.ContextResource;

            if (contextExecution != null)
                return ProcessApplicationContextUtil.GetTargetProcessApplication(contextExecution as CoreExecution);
            if (contextResource != null)
                return ProcessApplicationContextUtil.GetTargetProcessApplication(contextResource);
            return null;
        }
    }
}