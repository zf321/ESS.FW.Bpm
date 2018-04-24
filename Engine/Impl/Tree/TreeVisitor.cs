namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     A visitor for <seealso cref="SingleReferenceWalker" />.
    ///     
    /// </summary>
    public interface ITreeVisitor<T>
    {
        /// <summary>
        ///     Invoked for a node in tree.
        /// </summary>
        /// <param name="obj"> a reference to the node </param>
        void Visit(T obj);
    }
}