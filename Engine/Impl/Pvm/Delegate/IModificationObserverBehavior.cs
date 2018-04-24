using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{
    /// <summary>
    ///     When a (scope) activity behavior implements this behavior,
    ///     its scope execution is notified in case of an external modification about the following:
    ///     <ul>
    ///         <li>
    ///             the scope execution is newly created
    ///             <li>
    ///                 a new concurrent execution is created in that scope
    ///                 <li> a concurrent execution is removed in that scope
    ///     </ul>
    ///     
    /// </summary>
    public interface IModificationObserverBehavior : IActivityBehavior
    {
        /// <summary>
        ///     Implement to customize initialization of the scope. Called with the
        ///     scope execution already created. Implementations may set variables, etc.
        ///     Implementations should provide return as many executions as there are requested by the argument.
        ///     Valid number of instances are >= 0.
        /// </summary>
        IList<IActivityExecution> InitializeScope(IActivityExecution scopeExecution, int nrOfInnerInstances);

        /// <summary>
        ///     Returns an execution that can be used to execute an activity within that scope.
        ///     May reorganize other executions in that scope (e.g. implement to override the default pruning behavior).
        /// </summary>
        IActivityExecution CreateInnerInstance(IActivityExecution scopeExecution);

        /// <summary>
        ///     implement to destroy an execution in this scope and handle the scope's reorganization
        ///     (e.g. implement to override the default pruning behavior). The argument execution is not yet removed.
        /// </summary>
        void DestroyInnerInstance(IActivityExecution concurrentExecution);
    }
}