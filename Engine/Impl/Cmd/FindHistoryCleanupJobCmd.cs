using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class FindHistoryCleanupJobCmd : ICommand<IJob>
    {

        public IJob Execute(CommandContext commandContext)
        {
            return commandContext.JobManager.FindJobByHandlerType(HistoryCleanupJobHandler.TYPE);
        }

    }

}
