using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class DeleteUnmappedInstanceVisitor : ITreeVisitor<MigratingScopeInstance>
    {
        protected internal bool SkipCustomListeners;
        protected internal bool SkipIoMappings;

        protected internal IList<MigratingScopeInstance> VisitedInstances = new List<MigratingScopeInstance>();

        public DeleteUnmappedInstanceVisitor(bool skipCustomListeners, bool skipIoMappings)
        {
            this.SkipCustomListeners = skipCustomListeners;
            this.SkipIoMappings = skipIoMappings;
        }

        public virtual void Visit(MigratingScopeInstance currentInstance)
        {
            VisitedInstances.Add(currentInstance);
            if (!currentInstance.Migrates())
            {
                ISet<MigratingProcessElementInstance> children =
                    new HashSet<MigratingProcessElementInstance>(currentInstance.Children);
                var parent = currentInstance.Parent;

                // 1. detach children
                currentInstance.DetachChildren();

                // 2. manipulate execution tree (i.e. remove this instance)
                currentInstance.Remove(SkipCustomListeners, SkipIoMappings);

                // 3. reconnect parent and children
                foreach (var child in children)
                    child.AttachState(parent);
            }
            else
            {
                currentInstance.RemoveUnmappedDependentInstances();
            }
        }

        public virtual bool HasVisitedAll(ICollection<MigratingScopeInstance> activityInstances)
        {
            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.Util.Collection 'containsAll' method:
            //return visitedInstances.containsAll(activityInstances);
            return VisitedInstances.All(r => activityInstances.Contains(r));
        }
    }
}