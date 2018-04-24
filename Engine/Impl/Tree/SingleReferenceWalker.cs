using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class SingleReferenceWalker<T> : ReferenceWalker<T>
    {
        public SingleReferenceWalker(T initialElement) : base(initialElement)
        {
        }

        protected internal override ICollection<T> NextElements()
        {
            var nextElement = NextElement();

            if (nextElement != null)
                return new List<T> {nextElement};
            return new List<T>();
        }

        protected internal abstract T NextElement();
    }
}