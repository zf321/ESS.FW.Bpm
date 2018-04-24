using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     Defines the start behavior for <seealso cref="ActivityImpl activities" />.
    ///     
    /// </summary>
    public enum ActivityStartBehavior
    {
        /// <summary>
        ///     Default start behavior for an activity is to "do nothing special". Meaning:
        ///     the activity is executed by the execution which enters it.
        ///     NOTE: Only activities contained in normal flow can have DEFALUT start behavior.
        /// </summary>
        Default,

        /// <summary>
        ///     Used for activities which <seealso cref="PvmExecutionImpl#interrupt(String) interrupt" />
        ///     their <seealso cref="IPvmActivity#getFlowScope() flow scope" />. Examples:
        ///     - Terminate end event
        ///     - Cancel end event
        ///     NOTE: can only be used for activities contained in normal flow
        /// </summary>
        InterruptFlowScope,

        /// <summary>
        ///     Used for activities which are executed concurrently to activities
        ///     within the same <seealso cref="ActivityImpl#getFlowScope() flowScope" />.
        /// </summary>
        ConcurrentInFlowScope,

        /// <summary>
        ///     Used for activities which <seealso cref="PvmExecutionImpl#interrupt(String) interrupt" />
        ///     their <seealso cref="IPvmActivity#getEventScope() event scope" />
        ///     NOTE: cannot only be used for activities contained in normal flow
        /// </summary>
        InterruptEventScope,

        /// <summary>
        ///     Used for activities which cancel their <seealso cref="IPvmActivity#getEventScope() event scope" />.
        ///     - Boundary events with cancelActivity=true
        ///     NOTE: cannot only be used for activities contained in normal flow
        /// </summary>
        CancelEventScope
    }
}