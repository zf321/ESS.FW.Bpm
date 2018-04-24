using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{



    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SaveGroupCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal GroupEntity Group;

        public SaveGroupCmd(GroupEntity group)
        {
            this.Group = group;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("group", Group);

            commandContext.WritableIdentityProvider.SaveGroup(Group);
            return null;
        }
    }
}