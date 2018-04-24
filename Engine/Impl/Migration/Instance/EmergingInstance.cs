using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     Represents a instance which will be created by a migration, i.e.
    ///     new event subscriptions or jobs.
    /// </summary>
    public interface IEmergingInstance
    {
        /// <summary>
        ///     Creates this instance and attachs it to the given execution.
        /// </summary>
        /// <param name="scopeExecution"> the execution to attach the new instance </param>
        void Create(ExecutionEntity scopeExecution);
    }
}