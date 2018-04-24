using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public interface IPvmScope : IPvmProcessElement
    {
        /// <summary>
        ///     Indicates whether this is a local scope for variables and events
        ///     if true, there will _always_ be a scope execution created for it.
        ///     <para>
        ///         Note: the fact that this is a scope does not mean that it is also a
        ///         <seealso cref="#isSubProcessScope() sub process scope." />
        ///         @returns true if this activity is a scope
        ///     </para>
        /// </summary>
        bool IsScope { get; set; }

        /// <summary>
        ///     Indicates whether this scope is a sub process scope.
        ///     A sub process scope is a scope which contains "normal flow".Scopes which are flow scopes but not sub process
        ///     scopes:
        ///     <ul>
        ///         <li>a multi instance body scope</li>
        ///         <li>leaf scope activities which are pure event scopes (Example: User task with attached boundary event)</li>
        ///     </ul>
        /// </summary>
        /// <returns> true if this is a sub process scope </returns>
        bool SubProcessScope { get; }

        /// <summary>
        ///     The event scope for an activity is the scope in which the activity listens for events.
        ///     This may or may not be the <seealso cref="#getFlowScope() flow scope." />.
        ///     Consider: boundary events have a different event scope than flow scope.
        ///     <para>
        ///         The event scope is always a <seealso cref="#isScope() scope" />.
        ///     </para>
        /// </summary>
        /// <returns> the event scope of the activity </returns>
        IPvmScope EventScope { get; set; }

        /// <summary>
        ///     The flow scope of the activity. The scope in which the activity itself is executed.
        ///     <para>
        ///         Note: in order to ensure backwards compatible behavior,  a flow scope is not necessarily
        ///         a <seealso cref="#isScope() a scope" />. Example: event sub processes.
        ///     </para>
        /// </summary>
        ScopeImpl FlowScope { get; }

        /// <summary>
        ///     The "level of subprocess scope" as defined in bpmn: this is the subprocess
        ///     containing the activity. Usually this is the same as the flow scope, instead if
        ///     the activity is multi instance: in that case the activity is nested inside a
        ///     mutli instance body but "at the same level of subprocess" as other activities which
        ///     are siblings to the mi-body.
        /// </summary>
        /// <returns> the level of subprocess scope as defined in bpmn </returns>
        IPvmScope LevelOfSubprocessScope { get; }

        /// <summary>
        ///     Returns the flow activities of this scope. This is the list of activities for which this scope is
        ///     the <seealso cref="IPvmActivity#getFlowScope() flow scope" />.
        /// </summary>
        /// <returns> the list of flow activities for this scope. </returns>
        IList<IPvmActivity> Activities { get; }

        /// <summary>
        ///     Recursively finds a flow activity. This is an activitiy which is in the hierarchy of flow activities.
        /// </summary>
        /// <param name="activityId"> the id of the activity to find. </param>
        /// <returns> the activity or null </returns>
        IPvmActivity FindActivity(string activityId);

        /// <summary>
        ///     Finds an activity at the same level of subprocess.
        /// </summary>
        /// <param name="activityId"> the id of the activity to find. </param>
        /// <returns> the activity or null </returns>
        IPvmActivity FindActivityAtLevelOfSubprocess(string activityId);

        /// <summary>
        ///     Recursively finds a transition.
        /// </summary>
        /// <param name="transitionId"> the transiton to find </param>
        /// <returns> the transition or null </returns>
        TransitionImpl FindTransition(string transitionId);
    }
}