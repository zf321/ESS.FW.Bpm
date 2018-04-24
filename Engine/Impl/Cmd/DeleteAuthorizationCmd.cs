using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class DeleteAuthorizationCmd : ICommand<object>
    {
        protected internal string AuthorizationId;

        public DeleteAuthorizationCmd(string authorizationId)
        {
            this.AuthorizationId = authorizationId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            IAuthorizationManager authorizationManager = commandContext.AuthorizationManager;

            var authorization =authorizationManager.First(c=>c.Id == AuthorizationId);

            EnsureUtil.EnsureNotNull("Authorization for Id '" + AuthorizationId + "' does not exist", "authorization",
                authorization);

            authorizationManager.Delete(authorization);
            return null;
        }
    }
}