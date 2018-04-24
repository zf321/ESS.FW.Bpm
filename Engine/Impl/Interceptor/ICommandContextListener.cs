namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     Command context lifecycle events
    ///     
    /// </summary>
    public interface ICommandContextListener
    {
        void OnCommandContextClose(CommandContext commandContext);

        void OnCommandFailed(CommandContext commandContext, System.Exception t);
    }
}