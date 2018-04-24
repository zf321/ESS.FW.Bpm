using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{



    /// <summary>
    ///     
    /// </summary>
    public class SaveAuthorizationCmd : ICommand<IAuthorization>
    {
        protected internal AuthorizationEntity Authorization;

        public SaveAuthorizationCmd(IAuthorization authorization)
        {
            this.Authorization = (AuthorizationEntity) authorization;
            Validate();
        }

        public virtual IAuthorization Execute(CommandContext commandContext)
        {
            IAuthorizationManager authorizationManager = commandContext.AuthorizationManager;

            if (ReferenceEquals(Authorization.Id, null))
            {
                authorizationManager.Add(Authorization);
            }
            else
            {
                authorizationManager.Update(Authorization);
            }

            return Authorization;
        }

        protected internal virtual void Validate()
        {
            EnsureUtil.EnsureOnlyOneNotNull("Authorization must either have a 'userId' or a 'groupId'.", Authorization.UserId,
                Authorization.GroupId);
            EnsureUtil.EnsureNotNull("Authorization 'resourceType' cannot be null.", "authorization.getResource()",
                Authorization.GetResource());
        }
    }
}