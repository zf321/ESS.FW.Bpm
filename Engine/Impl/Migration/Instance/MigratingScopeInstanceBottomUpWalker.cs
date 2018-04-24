using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingScopeInstanceBottomUpWalker : SingleReferenceWalker<MigratingScopeInstance>
    {
        protected internal MigratingScopeInstance Parent;

        public MigratingScopeInstanceBottomUpWalker(MigratingScopeInstance initialElement) : base(initialElement)
        {
            // determine parent beforehand since it may be removed while walking
            Parent = initialElement.Parent;
        }

        protected internal override MigratingScopeInstance NextElement()
        {
            var nextElement = Parent;
            if (Parent != null)
                Parent = Parent.Parent;
            return nextElement;
        }
    }
}