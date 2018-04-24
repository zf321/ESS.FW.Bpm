using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class AuthorizationServiceImpl : ServiceImpl, IAuthorizationService
    {
        public virtual IQueryable<IAuthorization> CreateAuthorizationQuery(Expression<Func<AuthorizationEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<AuthorizationEntity>(expression));
        }

        public virtual IAuthorization CreateNewAuthorization(int type)
        {
            return CommandExecutor.Execute(new CreateAuthorizationCommand(type));
        }

        public virtual IAuthorization SaveAuthorization(IAuthorization authorization)
        {
            return CommandExecutor.Execute(new SaveAuthorizationCmd(authorization));
        }

        public virtual void DeleteAuthorization(string authorizationId)
        {
            CommandExecutor.Execute(new DeleteAuthorizationCmd(authorizationId));
        }

        public virtual bool IsUserAuthorized(string userId, IList<string> groupIds, Permissions permission,
            Resources resource)
        {
            return CommandExecutor.Execute(new AuthorizationCheckCmd(userId, groupIds, permission, resource, null));
        }

        public virtual bool IsUserAuthorized(string userId, IList<string> groupIds, Permissions permission,
            Resources resource, string resourceId)
        {
            return CommandExecutor.Execute(new AuthorizationCheckCmd(userId, groupIds, permission, resource, resourceId));
        }
    }
}