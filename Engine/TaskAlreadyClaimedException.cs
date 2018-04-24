namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     This exception is thrown when you try to claim a task that is already claimed
    ///     by someone else.
    ///      Jorg Heymans
    ///     
    /// </summary>
    public class TaskAlreadyClaimedException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        /// <summary>
        ///     the assignee of the task that is already claimed
        /// </summary>
        private readonly string _taskAssignee;

        /// <summary>
        ///     the id of the task that is already claimed
        /// </summary>
        private readonly string _taskId;

        public TaskAlreadyClaimedException(string taskId, string taskAssignee)
            : base("Task '" + taskId + "' is already claimed by someone else.")
        {
            this._taskId = taskId;
            this._taskAssignee = taskAssignee;
        }

        public virtual string TaskId
        {
            get { return _taskId; }
        }

        public virtual string TaskAssignee
        {
            get { return _taskAssignee; }
        }
    }
}