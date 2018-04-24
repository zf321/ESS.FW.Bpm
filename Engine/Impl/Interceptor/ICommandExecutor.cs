namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     The command executor for internal usage/
    ///      
    /// </summary>
    public interface ICommandExecutor
    {
        T Execute<T>(ICommand<T> command);
    }
}