using System;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    [Serializable]
    public class CreateTenantQueryCmd : ICommand<IQueryable<ITenant>>
    {
        private const long SerialVersionUid = 1L;

        public virtual IQueryable<ITenant> Execute(CommandContext commandContext)
        {
            return commandContext.ReadOnlyIdentityProvider.CreateTenantQuery();
        }
    }
}