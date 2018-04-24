namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     <para>
    ///         Interface providing access to the <seealso cref="ProcessEngineServices" /> from Java
    ///         delegation code.
    ///     </para>
    ///     
    /// </summary>
    public interface IProcessEngineServicesAware
    {
        /// <summary>
        ///     Returns the <seealso cref="ProcessEngineServices" /> providing access to the
        ///     public API of the process engine.
        /// </summary>
        /// <returns> the <seealso cref="ProcessEngineServices" />. </returns>
        IProcessEngineServices ProcessEngineServices { get; }
    }
}