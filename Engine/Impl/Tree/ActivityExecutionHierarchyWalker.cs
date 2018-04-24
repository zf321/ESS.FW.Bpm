using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Combination of flow scope and execution walker. Walks the flow scope
    ///     hierarchy upwards from the given execution to the top level process instance.
    ///     
    /// </summary>
    public class ActivityExecutionHierarchyWalker : SingleReferenceWalker<ActivityExecutionTuple>
    {
        private IDictionary<ScopeImpl, PvmExecutionImpl> _activityExecutionMapping;

        public ActivityExecutionHierarchyWalker(IActivityExecution execution) : base(CreateTupel(execution))
        {
            _activityExecutionMapping = execution.CreateActivityExecutionMapping();
        }

        protected internal override ActivityExecutionTuple NextElement()
        {
            var currentScope = CurrentElement.Scope;
            IPvmScope flowScope = currentScope.FlowScope;

            if (flowScope != null)
            {
                // walk to parent scope
                var execution = _activityExecutionMapping[(ScopeImpl) flowScope];
                return new ActivityExecutionTuple(flowScope, execution);
            }
            // this is the process instance, look for parent
            var currentExecution = _activityExecutionMapping[(ScopeImpl) currentScope];
            var superExecution = (PvmExecutionImpl) currentExecution.SuperExecution;

            if (superExecution != null)
            {
                // walk to parent process instance
                _activityExecutionMapping = superExecution.CreateActivityExecutionMapping();
                return CreateTupel(superExecution);
            }
            // this is the top level process instance
            return null;
        }

        protected internal static ActivityExecutionTuple CreateTupel(IActivityExecution execution)
        {
            var flowScope = GetCurrentFlowScope(execution);
            return new ActivityExecutionTuple(flowScope, execution);
        }

        protected internal static IPvmScope GetCurrentFlowScope(IActivityExecution execution)
        {
            ScopeImpl scope = null;
            if (execution.Transition != null)
                scope = execution.Transition.Destination.FlowScope;
            else
                scope = (ScopeImpl) execution.Activity;

            if (scope.IsScope)
                return scope;
            return scope.FlowScope;
        }

        public virtual ReferenceWalker<ActivityExecutionTuple> AddScopePreVisitor(ITreeVisitor<IPvmScope> visitor)
        {
            return AddPreVisitor(new ScopeVisitorWrapper(this, visitor));
        }

        public virtual ReferenceWalker<ActivityExecutionTuple> AddScopePostVisitor(ITreeVisitor<IPvmScope> visitor)
        {
            return AddPostVisitor(new ScopeVisitorWrapper(this, visitor));
        }

        public virtual ReferenceWalker<ActivityExecutionTuple> AddExecutionPreVisitor(
            ITreeVisitor<IActivityExecution> visitor)
        {
            return AddPreVisitor(new ExecutionVisitorWrapper(this, visitor));
        }

        public virtual ReferenceWalker<ActivityExecutionTuple> AddExecutionPostVisitor(
            ITreeVisitor<IActivityExecution> visitor)
        {
            return AddPostVisitor(new ExecutionVisitorWrapper(this, visitor));
        }

        private class ExecutionVisitorWrapper : ITreeVisitor<ActivityExecutionTuple>
        {
            internal readonly ITreeVisitor<IActivityExecution> Collector;
            private readonly ActivityExecutionHierarchyWalker _outerInstance;

            public ExecutionVisitorWrapper(ActivityExecutionHierarchyWalker outerInstance,
                ITreeVisitor<IActivityExecution> collector)
            {
                this._outerInstance = outerInstance;
                this.Collector = collector;
            }

            public virtual void Visit(ActivityExecutionTuple tupel)
            {
                Collector.Visit(tupel.Execution);
            }
        }

        private class ScopeVisitorWrapper : ITreeVisitor<ActivityExecutionTuple>
        {
            internal readonly ITreeVisitor<IPvmScope> Collector;
            private readonly ActivityExecutionHierarchyWalker _outerInstance;

            public ScopeVisitorWrapper(ActivityExecutionHierarchyWalker outerInstance, ITreeVisitor<IPvmScope> collector)
            {
                this._outerInstance = outerInstance;
                this.Collector = collector;
            }

            public virtual void Visit(ActivityExecutionTuple tupel)
            {
                Collector.Visit(tupel.Scope);
            }
        }
    }
}