using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public interface IFailedJobCommandFactory
    {
        ICommand<object> GetCommand(string jobId, System.Exception exception);
    }
}