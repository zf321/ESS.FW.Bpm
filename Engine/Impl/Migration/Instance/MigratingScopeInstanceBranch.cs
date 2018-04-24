using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     Keeps track of scope instances (activity instances; event scope instances) created in a branch
    ///     of the activity/event scope tree from the process instance downwards
    ///     
    /// </summary>
    public class MigratingScopeInstanceBranch
    {
        protected internal IDictionary<ScopeImpl, MigratingScopeInstance> ScopeInstances;

        public MigratingScopeInstanceBranch() : this(new Dictionary<ScopeImpl, MigratingScopeInstance>())
        {
        }

        protected internal MigratingScopeInstanceBranch(IDictionary<ScopeImpl, MigratingScopeInstance> scopeInstances)
        {
            this.ScopeInstances = scopeInstances;
        }

        public virtual MigratingScopeInstanceBranch Copy()
        {
            return new MigratingScopeInstanceBranch(new Dictionary<ScopeImpl, MigratingScopeInstance>(ScopeInstances));
        }

        public virtual MigratingScopeInstance GetInstance(ScopeImpl scope)
        {
            return ScopeInstances[scope];
        }

        public virtual bool HasInstance(ScopeImpl scope)
        {
            return ScopeInstances.ContainsKey(scope);
        }

        public virtual void Visited(MigratingScopeInstance scopeInstance)
        {
            var targetScope = scopeInstance.TargetScope;
            if (targetScope.IsScope)
                ScopeInstances[targetScope] = scopeInstance;
        }
    }
}