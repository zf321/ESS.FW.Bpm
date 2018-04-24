using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    [Serializable]
    public class DeleteTenantCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly string TenantId;

        public DeleteTenantCmd(string tenantId)
        {
            EnsureUtil.EnsureNotNull("groupId", tenantId);

            this.TenantId = tenantId;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            commandContext.WritableIdentityProvider.DeleteTenant(TenantId);

            return null;
        }
    }
}