using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class GetManagerCmd<TManager>: ICommand<TManager> where TManager:class
    {
        public TManager Execute(CommandContext commandContext)
        {
            return commandContext.Scope.Resolve<TManager>();
        }
    }
}
