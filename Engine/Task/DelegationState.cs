namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     Defines the different states of delegation that a task can be in.
    ///      
    /// </summary>
    public enum DelegationState
    {
        None=-1,
        /// <summary>
        ///     The owner delegated the task and wants to review the result
        ///     after the assignee has resolved the task.  When the assignee
        ///     completes the task, the task is marked as <seealso cref="#RESOLVED" /> and
        ///     sent back to the owner. When that happens, the owner is set as
        ///     the assignee so that the owner gets this task back in the ToDo.
        /// </summary>
        Pending,

        /// <summary>
        ///     The assignee has resolved the task, the assignee was set to the owner
        ///     again and the owner now finds this task back in the ToDo list for review.
        ///     The owner now is able to complete the task.
        /// </summary>
        Resolved
    }
}