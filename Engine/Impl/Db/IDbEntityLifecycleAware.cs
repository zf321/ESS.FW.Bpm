namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     Interface which can be implemented by <seealso cref="IDbEntity" /> which
    ///     want to be aware of their lifecycle.
    ///     
    /// </summary>
    public interface IDbEntityLifecycleAware
    {
        void PostLoad();
    }
}