namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     Listener interface implemented by user code which wants to be notified when a property of a ITask changes.
    ///     <para>
    ///         The following ITask Events are supported:
    ///         <ul>
    ///             <li>
    ///                 <seealso cref="#EVENTNAME_CREATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#EVENTNAME_ASSIGNMENT" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#EVENTNAME_COMPLETE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#EVENTNAME_DELETE" />
    ///             </li>
    ///         </ul>
    ///     </para>
    ///      
    /// </summary>
    public interface ITaskListener
    {
        void Notify(IDelegateTask delegateTask);
    }

    public static class TaskListenerFields
    {
        public const string EventnameCreate = "create";
        public const string EventnameAssignment = "assignment";
        public const string EventnameComplete = "complete";
        public const string EventnameDelete = "delete";
    }
}