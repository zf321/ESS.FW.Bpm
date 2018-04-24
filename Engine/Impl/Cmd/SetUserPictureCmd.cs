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
    public class SetUserPictureCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal Picture Picture;
        protected internal string UserId;


        public SetUserPictureCmd(string userId, Picture picture)
        {
            this.UserId = userId;
            this.Picture = picture;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("userId", UserId);

            IdentityInfoEntity pictureInfo = commandContext.IdentityInfoManager.FindUserInfoByUserIdAndKey(UserId,
            "picture");

            if (pictureInfo != null)
            {
                string byteArrayId = pictureInfo.Value;
                if (!ReferenceEquals(byteArrayId, null))
                {
                    commandContext.ByteArrayManager.DeleteByteArrayById(byteArrayId);
                }
            }
            else
            {
                pictureInfo = new IdentityInfoEntity();
                pictureInfo.UserId = UserId;
                pictureInfo.Key = "picture";
                commandContext.IdentityInfoManager.Add(pictureInfo);
            }

            var byteArrayEntity = new ResourceEntity(Picture.MimeType, Picture.Bytes);

            commandContext.ByteArrayManager.Add(byteArrayEntity);

            pictureInfo.Value = byteArrayEntity.Id;
            return null;
        }
    }
}