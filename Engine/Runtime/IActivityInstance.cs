namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>An activity instance represents an instance of an activity.</para>
    ///     <para>For documentation, see <seealso cref="IRuntimeService#getActivityInstance(String)" /></para>
    ///     
    /// </summary>
    public interface IActivityInstance : IProcessElementInstance
    {
        /// <summary>
        ///     the id of the activity
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     the name of the activity
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        ///     Type of the activity, corresponds to BPMN element name in XML (e.g. 'userTask').
        ///     The type of the Root activity instance (the one corresponding to the process instance will be 'processDefinition'.
        /// </summary>
        string ActivityType { get; }

        /// <summary>
        ///     Returns the child activity instances.
        ///     Returns an empty list if there are no child instances
        /// </summary>
        IActivityInstance[] ChildActivityInstances { get; }

        /// <summary>
        ///     Returns the child transition instances.
        ///     Returns an empty list if there are no child transition instances
        /// </summary>
        ITransitionInstance[] ChildTransitionInstances { get; }

        /// <summary>
        ///     the list of executions that are currently waiting in this activity instance
        /// </summary>
        string[] ExecutionIds { get; }

        /// <summary>
        ///     all descendant (children, grandchildren, etc.) activity instances that are instances of the supplied activity
        /// </summary>
        IActivityInstance[] GetActivityInstances(string activityId);

        /// <summary>
        ///     all descendant (children, grandchildren, etc.) transition instances that are leaving or entering the supplied
        ///     activity
        /// </summary>
        ITransitionInstance[] GetTransitionInstances(string activityId);
    }
}