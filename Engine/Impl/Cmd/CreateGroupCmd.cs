using System;
using System.Text.RegularExpressions;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    /// </summary>
    [Serializable]
    public class CreateGroupCmd : AbstractWritableIdentityServiceCmd<IGroup>, ICommand<IGroup>
    {
        private const long SerialVersionUid = 1L;

        protected internal string GroupId;

        public CreateGroupCmd(string groupId)
        {
            EnsureUtil.EnsureNotNull("groupId", groupId);
            this.GroupId = groupId;
        }

        protected internal override IGroup ExecuteCmd(CommandContext commandContext)
        {
            return commandContext.WritableIdentityProvider.CreateNewGroup(GroupId);
        }
    }
}