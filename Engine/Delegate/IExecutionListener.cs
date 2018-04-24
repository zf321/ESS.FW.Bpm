namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     Callback interface to be notified of execution events like starting a process instance,
    ///     ending an activity instance or taking a transition.
    ///      
    /// </summary>
    [System.Obsolete("空接口弃用，直接用IDelegateListener<IBaseDelegateExecution>",true)]
    public interface IExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
    }
    //public interface IExecutionListener : IDelegateListener<IDelegateExecution>
    //{
    //}
    public static class ExecutionListenerFields
    {
        public const string EventNameStart = "start";
        public const string EventNameEnd = "end";
        public const string EventNameTake = "take";
    }
}