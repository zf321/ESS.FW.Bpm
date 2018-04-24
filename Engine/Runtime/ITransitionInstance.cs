using System;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>
    ///         A transition instance represents an execution token that
    ///         has just completed a transition (sequence flow in BPMN) or is about
    ///         to take an outgoing transition. This happens before starting or after
    ///         leaving an activity. The execution token
    ///         is not actually executing the activity that this instance points to
    ///         which is why the corresponding activity instance does not exist.
    ///     </para>
    ///     <para>
    ///         Transition instances are the result of
    ///         asynchronous continuations, asyncBefore or asyncAfter.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public interface ITransitionInstance : IProcessElementInstance
    {
        /// <summary>
        ///     returns the id of the target activity
        /// </summary>
        /// @deprecated a transition instances represents a transition
        /// <b>to</b>
        /// or
        /// <b>from</b>
        /// an activity; use
        /// <seealso cref="#getActivityId()" />
        /// instead.
        [Obsolete("a transition instances represents a transition <b>to</b> or <b>from</b>")]
        string TargetActivityId { get; }

        /// <summary>
        ///     returns the id of the activity a transition is made from/to
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     returns the id of of the execution that is
        ///     executing this transition instance
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     returns the type of the activity a transition is made from/to.
        ///     Corresponds to BPMN element name in XML (e.g. 'userTask').
        ///     The type of the root activity instance (the one corresponding to the process instance)
        ///     is 'processDefinition'.
        /// </summary>
        string ActivityType { get; }

        /// <summary>
        ///     returns the name of the activity a transition is made from/to
        /// </summary>
        string ActivityName { get; }
    }
}