using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class FoxFailedJobCommandFactory : IFailedJobCommandFactory
    {
        public virtual ICommand<object> GetCommand(string jobId, System.Exception exception)
        {
            return new FoxJobRetryCmd(jobId, exception);
        }
    }
}