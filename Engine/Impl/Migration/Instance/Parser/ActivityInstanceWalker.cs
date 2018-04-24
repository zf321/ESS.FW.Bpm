using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceWalker : ReferenceWalker<IActivityInstance>
    {
        public ActivityInstanceWalker(IActivityInstance initialElement) : base(initialElement)
        {
        }

        protected internal override ICollection<IActivityInstance> NextElements()
        {
            var children = CurrentElement.ChildActivityInstances;
            return children;
        }
    }
}