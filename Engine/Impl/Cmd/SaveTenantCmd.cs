using System;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    [Serializable]
    public class SaveTenantCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal ITenant Tenant;

        public SaveTenantCmd(ITenant tenant)
        {
            this.Tenant = tenant;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("tenant", Tenant);

            commandContext.WritableIdentityProvider.SaveTenant(Tenant);

            return null;
        }
    }
}