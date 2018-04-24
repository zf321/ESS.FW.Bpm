using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class GetHistoryLevelCmd : ICommand<int>
    {
        public virtual int Execute(CommandContext commandContext)
        {
            commandContext.AuthorizationManager.CheckCamundaAdmin();
            return context.Impl.Context.ProcessEngineConfiguration.HistoryLevel.Id;
        }
    }
}