namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     A reference to a process application.
    ///     
    /// </summary>
    public interface IProcessApplicationReference
    {
        /// <returns> the name of the process application </returns>
        string Name { get; }

        /// <summary>
        ///     Get the process application.
        /// </summary>
        /// <returns> the <seealso cref="AbstractProcessApplication" /> </returns>
        /// <exception cref="ProcessApplicationUnavailableException">
        ///     if the process application is unavailable
        /// </exception>
        IProcessApplicationInterface ProcessApplication { get; }
    }
}