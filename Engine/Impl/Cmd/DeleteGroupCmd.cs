using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    /// </summary>
    [Serializable]
    public class DeleteGroupCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        internal string GroupId;

        public DeleteGroupCmd(string groupId)
        {
            this.GroupId = groupId;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("groupId", GroupId);
            commandContext.WritableIdentityProvider.DeleteGroup(GroupId);

            return null;
        }
    }
}