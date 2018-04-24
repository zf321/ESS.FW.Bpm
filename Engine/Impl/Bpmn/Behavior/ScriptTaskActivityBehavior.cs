using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{

    /// <summary>
    ///     <para>
    ///         <seealso cref="IActivityBehavior" /> implementation of the BPMN 2.0 script ITask.
    ///     </para>
    ///     
    /// </summary>
    public class ScriptTaskActivityBehavior : TaskActivityBehavior
    {
        protected internal string ResultVariable;

        protected internal ExecutableScript script;

        public ScriptTaskActivityBehavior(ExecutableScript script, string resultVariable)
        {
            this.script = script;
            this.ResultVariable = resultVariable;
        }

        public virtual ExecutableScript Script
        {
            get { return script; }
        }
        
        protected override void PerformExecution(IActivityExecution execution)
        {
            ExecuteWithErrorPropagation(execution, () =>
            {
                    var invocation = new ScriptInvocation(script, execution);
                    Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
                    var result = invocation.InvocationResult;
                    if (result != null && !ReferenceEquals(ResultVariable, null))
                    {
                        execution.SetVariable(ResultVariable, result);
                    }
                    Leave(execution);
            });
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
        protected internal override BpmnError CheckIfCauseOfExceptionIsBpmnError(System.Exception e)
        {
            if (e is BpmnError)
                return (BpmnError) e;
            if (e.InnerException == null)
                return null;
            return CheckIfCauseOfExceptionIsBpmnError(e.InnerException);
        }
        
    }
}