using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingActivityInstanceVisitor : MigratingProcessElementInstanceVisitor
    {
        protected internal bool SkipCustomListeners;
        protected internal bool SkipIoMappings;

        public MigratingActivityInstanceVisitor(bool skipCustomListeners, bool skipIoMappings)
        {
            this.SkipCustomListeners = skipCustomListeners;
            this.SkipIoMappings = skipIoMappings;
        }

        public override void Visit<T>(T obj)
        {
            throw new NotImplementedException();
        }

        protected internal override bool CanMigrate(MigratingProcessElementInstance instance)
        {
            return instance is MigratingActivityInstance || instance is MigratingTransitionInstance;
        }

        protected internal override void InstantiateScopes(MigratingScopeInstance ancestorScopeInstance,
            MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate)
        {
            if (scopesToInstantiate.Count == 0)
                return;

            // must always be an activity instance
            var ancestorActivityInstance = (MigratingActivityInstance) ancestorScopeInstance;

            var newParentExecution = ancestorActivityInstance.CreateAttachableExecution();

            //IDictionary<IPvmActivity, PvmExecutionImpl> createdExecutions =
            //    newParentExecution.instantiateScopes((IList) scopesToInstantiate, skipCustomListeners, skipIoMappings);

            //foreach (var scope in scopesToInstantiate)
            //{
            //    var createdExecution = (ExecutionEntity) createdExecutions[scope];
            //    createdExecution.setActivity(null);
            //    createdExecution.Active = false;
            //    executionBranch.visited(new MigratingActivityInstance(scope, createdExecution));
            //}
        }
    }
}