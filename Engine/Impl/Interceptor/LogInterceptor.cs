using ESS.FW.Bpm.Engine.Impl.Cmd;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///      
    /// </summary>
    public class LogInterceptor : CommandInterceptor
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        public override T Execute<T>(ICommand<T> command)
        {
            Log.DebugStartingCommand(command);
            try
            {
                return Next.Execute(command);
            }
            finally
            {
                Log.DebugFinishingCommand(command);
            }
        }
    }
}