using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    [Serializable]
    public class CreateTenantUserMembershipCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly string TenantId;
        protected internal readonly string UserId;

        public CreateTenantUserMembershipCmd(string tenantId, string userId)
        {
            this.TenantId = tenantId;
            this.UserId = userId;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("tenantId", TenantId);
            EnsureUtil.EnsureNotNull("userId", UserId);

            commandContext.WritableIdentityProvider.CreateTenantUserMembership(TenantId, UserId);

            return null;
        }
    }
}