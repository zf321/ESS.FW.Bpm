using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    [Serializable]
    public class DeleteTenantGroupMembershipCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal readonly string GroupId;

        protected internal readonly string TenantId;

        public DeleteTenantGroupMembershipCmd(string tenantId, string groupId)
        {
            this.TenantId = tenantId;
            this.GroupId = groupId;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("tenantId", TenantId);
            EnsureUtil.EnsureNotNull("groupId", GroupId);

            commandContext.WritableIdentityProvider.DeleteTenantGroupMembership(TenantId, GroupId);

            return null;
        }
    }
}