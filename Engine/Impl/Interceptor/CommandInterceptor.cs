namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///      
    /// </summary>
    public abstract class CommandInterceptor : ICommandExecutor
    {
        public virtual ICommandExecutor Next { get; set; }

        public abstract T Execute<T>(ICommand<T> command);
    }
}