using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using System;
using System.Transactions;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Parent class for all BPMN 2.0 ITask types such as ServiceTask, ScriptTask, UserTask, etc.
    ///     When used on its own, it behaves just as a pass-through activity.
    ///     
    /// </summary>
    public class TaskActivityBehavior : AbstractBpmnActivityBehavior
    {
        /// <summary>
        ///     Activity instance id before execution.
        /// </summary>
        protected internal string ActivityInstanceId;

        /// <summary>
        ///     The method which will be called before the execution is performed.
        /// </summary>
        /// <param name="execution"> the execution which is used during execution </param>
        /// <exception cref="exception"> </exception>
        protected internal virtual void PreExecution(IActivityExecution execution)
        {
            ActivityInstanceId = execution.ActivityInstanceId;
        }

        /// <summary>
        ///     The method which should be overridden by the sub classes to perform an execution.
        /// </summary>
        /// <param name="execution"> the execution which is used during performing the execution </param>
        /// <exception cref="exception"> </exception>
        protected virtual void PerformExecution(IActivityExecution execution)
        {
            Leave(execution);
        }

        /// <summary>
        ///     The method which will be called after performing the execution.
        /// </summary>
        /// <param name="execution"> the execution </param>
        /// <exception cref="exception"> </exception>
        protected internal virtual void PostExecution(IActivityExecution execution)
        {
        }

        public override void Execute(IActivityExecution execution)
        {
            var transOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = new TimeSpan(0, 10, 0)
            };
            using (var ts = new TransactionScope(TransactionScopeOption.Required,
                transOptions,
                EnterpriseServicesInteropOption.Automatic))
            {
                PerformExecution(execution);
                ts.Complete();
            }
        }
    }
}