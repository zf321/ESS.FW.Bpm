using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class DeleteUserPictureCmd : ICommand<object>
    {
        protected internal string UserId;

        public DeleteUserPictureCmd(string userId)
        {
            this.UserId = userId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("UserId", UserId);

            IdentityInfoEntity infoEntity = commandContext.IdentityInfoManager.FindUserInfoByUserIdAndKey(UserId, "picture");

            if (infoEntity != null)
            {
                string byteArrayId = infoEntity.Value;
                if (!string.ReferenceEquals(byteArrayId, null))
                {
                    commandContext.ByteArrayManager.DeleteByteArrayById(byteArrayId);
                }
                commandContext.IdentityInfoManager.Delete(infoEntity);
            }


            return null;
        }
    }
}