using ESS.FW.Bpm.Engine.EntityFramework;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class GetDbContextCmd : ICommand<DbContext>
    {
        public DbContext Execute(CommandContext commandContext)
        {
            return commandContext.DbContext;
        }
    }
}
