using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     Walks the hierarchy of <seealso cref="MigratingProcessElementInstance" />s in a top-down-fashion.
    ///     Maintains a context of the current instance and the <seealso cref="MigratingScopeInstanceBranch" />
    ///     that it is in.
    ///     
    /// </summary>
    public class MigratingProcessElementInstanceTopDownWalker :
        ReferenceWalker<MigratingProcessElementInstanceTopDownWalker.MigrationContext>
    {
        public MigratingProcessElementInstanceTopDownWalker(MigratingActivityInstance activityInstance)
            : base(new MigrationContext(activityInstance, new MigratingScopeInstanceBranch()))
        {
        }

        protected internal override ICollection<MigrationContext> NextElements()
        {
            ICollection<MigrationContext> nextElements = new LinkedList<MigrationContext>();

            var currentElement = CurrentElement;

            // continue migration for non-leaf instances (i.e. scopes)
            if (currentElement.processElementInstance is MigratingScopeInstance)
            {
                // Child instances share the same scope instance branch;
                // This ensures "once-per-parent" instantiation semantics,
                // i.e. if a new parent scope is added to more than one child, all those children
                // will share the same new parent instance.
                // By changing the way how the branches are created here, it should be possible
                // to implement other strategies, e.g. "once-per-child" semantics
                var childrenScopeBranch = currentElement.scopeInstanceBranch.Copy();
                var childrenCompensationScopeBranch = currentElement.scopeInstanceBranch.Copy();

                var scopeInstance = (MigratingScopeInstance) currentElement.processElementInstance;

                childrenScopeBranch.Visited(scopeInstance);
                childrenCompensationScopeBranch.Visited(scopeInstance);

                foreach (var child in scopeInstance.Children)
                {
                    MigratingScopeInstanceBranch instanceBranch = null;

                    // compensation and non-compensation scopes cannot share the same scope instance branch
                    // e.g. when adding a sub process, we want to create a new activity instance as well
                    // as a new event scope instance for that sub process
                    if (child is MigratingEventScopeInstance || child is MigratingCompensationEventSubscriptionInstance)
                        instanceBranch = childrenCompensationScopeBranch;
                    else
                        instanceBranch = childrenScopeBranch;
                    nextElements.Add(new MigrationContext(child, instanceBranch));
                }
            }

            return nextElements;
        }

        public class MigrationContext
        {
            protected internal MigratingProcessElementInstance processElementInstance;
            protected internal MigratingScopeInstanceBranch scopeInstanceBranch;

            public MigrationContext(MigratingProcessElementInstance processElementInstance,
                MigratingScopeInstanceBranch scopeInstanceBranch)
            {
                this.processElementInstance = processElementInstance;
                this.scopeInstanceBranch = scopeInstanceBranch;
            }

            public virtual MigratingProcessElementInstance ProcessElementInstance
            {
                get { return processElementInstance; }
            }

            public virtual MigratingScopeInstanceBranch ScopeInstanceBranch
            {
                get { return scopeInstanceBranch; }
            }
        }
    }
}