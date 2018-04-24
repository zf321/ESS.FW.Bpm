using System;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///     
    ///      
    /// </summary>
    [Serializable]
    public class GetUserPictureCmd : ICommand<Picture>
    {
        private const long SerialVersionUid = 1L;
        protected internal string UserId;

        public GetUserPictureCmd(string userId)
        {
            this.UserId = userId;
        }

        public virtual Picture Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("userId", UserId);

            IdentityInfoEntity pictureInfo = commandContext.IdentityInfoManager.FindUserInfoByUserIdAndKey(UserId, "picture");

            if (pictureInfo != null)
            {
                string pictureByteArrayId = pictureInfo.Value;
                if (!string.ReferenceEquals(pictureByteArrayId, null))
                {
                    ResourceEntity byteArray = commandContext.ByteArrayManager.Get(pictureByteArrayId);
                    return  new Picture(byteArray.Bytes,byteArray.Name);
                }
            }
            return null;
        }
    }
}