namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     Represents a instance which will be removed by a migration, i.e.
    ///     removed event subscriptions or jobs.
    /// </summary>
    public interface IRemovingInstance
    {
        /// <summary>
        ///     Remove this instance from the process instance.
        /// </summary>
        void Remove();
    }
}