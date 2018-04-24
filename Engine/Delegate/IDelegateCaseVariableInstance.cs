namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    /// </summary>
    public interface IDelegateCaseVariableInstance : IDelegateVariableInstance<IDelegateCaseExecution>
    {
        /// <summary>
        ///     The event name, which caused the listener to be notified.
        ///     Valid values are the constants defined in <seealso cref="ICaseVariableListener" />.
        /// </summary>
        //string EventName { get; }

        /// <summary>
        ///     The case execution in which context the variable was created/updated/deleted.
        /// </summary>
        //IDelegateCaseExecution SourceExecution { get; }
    }
}