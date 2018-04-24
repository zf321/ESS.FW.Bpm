namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///    
    /// </summary>
    public interface IBaseDelegateExecution : IVariableScope
    {
        /// <summary>
        ///     Unique id of this path of execution that can be used as a handle to provide external signals back into the engine
        ///     after wait states.
        /// </summary>
        string Id { get;  set; }

        /// <summary>
        ///     The <seealso cref="IExecutionListener#EVENTNAME_START event name" /> in case this execution is passed in for an
        ///     <seealso cref="IExecutionListener" />
        /// </summary>
        string EventName { get; }

        /// <summary>
        ///     The business key for this execution. Only returns a value if the delegate execution
        ///     is a a root execution (such as a process instance).
        /// </summary>
        string BusinessKey { get; set; }
    }
}