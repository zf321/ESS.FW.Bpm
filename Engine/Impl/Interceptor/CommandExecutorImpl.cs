using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///      
    /// </summary>
    public class CommandExecutorImpl : CommandInterceptor
    {
        public override T Execute<T>(ICommand<T> command)
        {
            return command.Execute(Context.CommandContext);
        }
    }
}