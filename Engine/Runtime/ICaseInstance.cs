namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    public interface ICaseInstance : ICaseExecution
    {
        /// <summary>
        ///     The business key of this process instance.
        /// </summary>
        string BusinessKey { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case instance is completed.</para>
        ///     <para>
        ///         <strong>Note:</strong> If this case execution is not the case instance,
        ///         it will always return <code>false</code>.
        ///     </para>
        /// </summary>
        bool Completed { get; }
    }
}