using System;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    [Serializable]
    public class CreateTenantCmd : AbstractWritableIdentityServiceCmd<ITenant>, ICommand<ITenant>
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly string TenantId;

        public CreateTenantCmd(string tenantId)
        {
            EnsureUtil.EnsureNotNull("tenantId", tenantId);
            this.TenantId = tenantId;
        }

        protected internal override ITenant ExecuteCmd(CommandContext commandContext)
        {
            return commandContext.WritableIdentityProvider.CreateNewTenant(TenantId);
        }
    }
}