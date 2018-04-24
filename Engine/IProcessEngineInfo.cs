namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Represents information about the initialization of the process engine.
    /// </summary>
    /// <seealso cref= "ProcessEngines" />
    public interface IProcessEngineInfo
    {
        /// <summary>
        ///     Returns the name of the process engine.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Returns the resources the engine was configured from.
        /// </summary>
        string ResourceUrl { get; }

        /// <summary>
        ///     Returns the exception stacktrace in case an exception occurred while initializing
        ///     the engine. When no exception occured, null is returned.
        /// </summary>
        string Exception { get; }
    }
}