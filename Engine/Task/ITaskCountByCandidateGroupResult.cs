namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public interface ITaskCountByCandidateGroupResult
    {
        /// <summary>
        ///     The number of tasks for a specific group
        /// </summary>
        int TaskCount { get; }

        /// <summary>
        ///     The group which as the number of tasks
        /// </summary>
        string GroupName { get; }
    }
}