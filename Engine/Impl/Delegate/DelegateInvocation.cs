using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     Provides context about the invocation of usercode and handles the actual
    ///     invocation
    ///     
    /// </summary>
    /// <seealso cref= "DelegateInterceptor"></seealso>
    public abstract class DelegateInvocation
    {
        /// <summary>
        ///     Provide a context execution or resource definition in which context the invocation
        ///     should be performed. If both parameters are null, the invocation is performed in the
        ///     current context.
        /// </summary>
        /// <param name="contextExecution"> set to an execution </param>
        public DelegateInvocation(IBaseDelegateExecution contextExecution, IResourceDefinitionEntity contextResource)
        {
            // This constructor forces sub classes to call it, thereby making it more visible
            // whether a context switch is going to be performed for them.
            this.ContextExecution = contextExecution;
            this.ContextResource = contextResource;
        }

        /// <returns>
        ///     the result of the invocation (can be null if the invocation does
        ///     not return a result)
        /// </returns>
        public virtual object InvocationResult
        {
            get;
            set;
        }

        /// <summary>
        ///     returns the execution in which context this delegate is invoked. may be null
        /// </summary>
        public virtual IBaseDelegateExecution ContextExecution { get; }

        public virtual IResourceDefinitionEntity ContextResource { get; }

        /// <summary>
        ///     make the invocation proceed, performing the actual invocation of the user
        ///     code.
        /// </summary>
        /// <exception cref="exception">
        ///     the exception thrown by the user code
        /// </exception>
        public virtual void Proceed()
        {
            Invoke();
        }
        
        protected internal abstract void Invoke();
    }
}