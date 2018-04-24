using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigrationCompensationInstanceVisitor : MigratingProcessElementInstanceVisitor
    {
        public override void Visit<T>(T obj)
        {
            throw new NotImplementedException();
        }

        protected internal override bool CanMigrate(MigratingProcessElementInstance instance)
        {
            return instance is MigratingEventScopeInstance || instance is MigratingCompensationEventSubscriptionInstance;
        }

        protected internal override void InstantiateScopes(MigratingScopeInstance ancestorScopeInstance,
            MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate)
        {
            if (scopesToInstantiate.Count == 0)
                return;

            var ancestorScopeExecution = ancestorScopeInstance.ResolveRepresentativeExecution();

            var parentExecution = ancestorScopeExecution;

            foreach (var scope in scopesToInstantiate)
            {
                var compensationScopeExecution = parentExecution.CreateExecution();
                compensationScopeExecution.IsScope = true;
                compensationScopeExecution.IsEventScope = true;

                //compensationScopeExecution.setActivity((IPvmActivity) scope);
                compensationScopeExecution.IsActive= false;
                //compensationScopeExecution.activityInstanceStarting();
                //compensationScopeExecution.enterActivityInstance();

                //EventSubscriptionEntity eventSubscription = EventSubscriptionEntity.createAndInsert(parentExecution,
                //    EventType.COMPENSATE, (ActivityImpl) scope);
                //eventSubscription.Configuration = compensationScopeExecution.Id;

                //executionBranch.visited(new MigratingEventScopeInstance(eventSubscription, compensationScopeExecution,
                //    scope));

                parentExecution = compensationScopeExecution;
            }
        }
    }
}