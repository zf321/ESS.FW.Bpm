using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class MigratingProcessElementInstanceVisitor :
        ITreeVisitor<MigratingProcessElementInstanceTopDownWalker.MigrationContext>
    {
        public virtual void Visit(MigratingProcessElementInstanceTopDownWalker.MigrationContext obj)
        {
            if (CanMigrate(obj.processElementInstance))
                MigrateProcessElementInstance(obj.processElementInstance, obj.scopeInstanceBranch);
        }

        public abstract void Visit<T>(T obj);

        protected internal abstract bool CanMigrate(MigratingProcessElementInstance instance);

        protected internal abstract void InstantiateScopes(MigratingScopeInstance ancestorScopeInstance,
            MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate);

        protected internal virtual void MigrateProcessElementInstance(MigratingProcessElementInstance migratingInstance,
            MigratingScopeInstanceBranch migratingInstanceBranch)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final MigratingScopeInstance parentMigratingInstance = migratingInstance.Parent;
            var parentMigratingInstance = migratingInstance.Parent;

            var sourceScope = migratingInstance.SourceScope;
            var targetScope = migratingInstance.TargetScope;
            var targetFlowScope = targetScope.FlowScope;
            var parentActivityInstanceTargetScope = parentMigratingInstance != null
                ? parentMigratingInstance.TargetScope
                : null;

            if ((sourceScope != sourceScope.ProcessDefinition) && (targetFlowScope != parentActivityInstanceTargetScope))
            {
                // create intermediate scopes

                // 1. manipulate execution tree

                // determine the list of ancestor scopes (parent, grandparent, etc.) for which
                //     no executions exist yet
                var nonExistingScopes = CollectNonExistingFlowScopes(targetFlowScope, migratingInstanceBranch);

                // get the closest ancestor scope that is instantiated already
                var existingScope = nonExistingScopes.Count == 0 ? targetFlowScope : nonExistingScopes[0].FlowScope;

                // and its scope instance
                var ancestorScopeInstance = migratingInstanceBranch.GetInstance(existingScope);

                // Instantiate the scopes as children of the scope execution
                InstantiateScopes(ancestorScopeInstance, migratingInstanceBranch, nonExistingScopes);

                var targetFlowScopeInstance = migratingInstanceBranch.GetInstance(targetFlowScope);

                // 2. detach instance
                // The order of steps 1 and 2 avoids intermediate execution tree compaction
                // which in turn could overwrite some dependent instances (e.g. variables)
                migratingInstance.DetachState();

                // 3. attach to newly created activity instance
                migratingInstance.AttachState(targetFlowScopeInstance);
            }

            // 4. update state (e.g. activity id)
            migratingInstance.MigrateState();

            // 5. migrate instance state other than execution-tree structure
            migratingInstance.MigrateDependentEntities();
        }

        /// <summary>
        ///     Returns a list of flow scopes from the given scope until a scope is reached that is already present in the given
        ///     <seealso cref="MigratingScopeInstanceBranch" /> (exclusive). The order of the returned list is top-down, i.e. the
        ///     highest scope
        ///     is the first element of the list.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected java.Util.List<org.camunda.bpm.engine.impl.Pvm.Process.ScopeImpl> collectNonExistingFlowScopes(org.camunda.bpm.engine.impl.Pvm.Process.ScopeImpl scope, final MigratingScopeInstanceBranch migratingExecutionBranch)
        protected internal virtual IList<ScopeImpl> CollectNonExistingFlowScopes(ScopeImpl scope,
            MigratingScopeInstanceBranch migratingExecutionBranch)
        {
            var walker = new FlowScopeWalker(scope);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.Util.List<org.camunda.bpm.engine.impl.Pvm.Process.ScopeImpl> result = new java.Util.LinkedList<org.camunda.bpm.engine.impl.Pvm.Process.ScopeImpl>();
            IList<ScopeImpl> result = new List<ScopeImpl>();
            walker.AddPreVisitor(new TreeVisitorAnonymousInnerClass(this, result));

            //walker.walkWhile(new WalkConditionAnonymousInnerClass(this, migratingExecutionBranch));

            return result;
        }

        private class TreeVisitorAnonymousInnerClass : ITreeVisitor<ScopeImpl>
        {
            private readonly MigratingProcessElementInstanceVisitor _outerInstance;

            private readonly IList<ScopeImpl> _result;

            public TreeVisitorAnonymousInnerClass(MigratingProcessElementInstanceVisitor outerInstance,
                IList<ScopeImpl> result)
            {
                this._outerInstance = outerInstance;
                this._result = result;
            }


            public virtual void Visit(ScopeImpl obj)
            {
                _result.Insert(0, obj);
            }
        }

        //private class WalkConditionAnonymousInnerClass : org.camunda.bpm.engine.impl.tree.ReferenceWalker<ScopeImpl>
        //{
        //    private readonly MigratingProcessElementInstanceVisitor outerInstance;

        //    private org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstanceBranch migratingExecutionBranch;

        //    public WalkConditionAnonymousInnerClass(MigratingProcessElementInstanceVisitor outerInstance, org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstanceBranch migratingExecutionBranch)
        //    {

        //        this.outerInstance = outerInstance;
        //        this.migratingExecutionBranch = migratingExecutionBranch;
        //    }


        //    public virtual bool isFulfilled(ScopeImpl element)
        //    {
        //        return migratingExecutionBranch.hasInstance(element);
        //    }
        //}
    }
}