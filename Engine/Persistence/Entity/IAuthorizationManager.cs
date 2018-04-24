using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IAuthorizationManager: IRepository<AuthorizationEntity,string>
    {
        bool AuthorizationEnabled { get; }
        void CheckAuthorization(CompositePermissionCheck compositePermissionCheck);
        void CheckAuthorization(IList<PermissionCheck> permissionChecks);
        void CheckAuthorization(params PermissionCheck[] permissionChecks);
        void CheckAuthorization(Permissions permission, Resources resource);
        void CheckAuthorization(Permissions permission, Resources resource,string resourceId);
        void CheckCamundaAdmin();
        //void ConfigureExecutionQuery(ListQueryParameterObject query);
        //void ConfigureQuery(ListQueryParameterObject query);
        //void ConfigureQuery(ListQueryParameterObject query, Resources resource, string queryParam);
        //void ConfigureQuery(ListQueryParameterObject query, Resources resource, string queryParam, Permissions permission);
        IAuthorization CreateNewAuthorization(int type);
        void DeleteAuthorizationsByResourceId(Resources resource, string resourceId);
        void DeleteAuthorizationsByResourceIdAndGroupId(Resources resource, string resourceId, string groupId);
        void DeleteAuthorizationsByResourceIdAndUserId(Resources resource, string resourceId, string userId);
        void EnableQueryAuthCheck(AuthorizationCheck authCheck);
        IList<string> FilterAuthenticatedGroupIds(IList<string> authenticatedGroupIds);
        AuthorizationEntity FindAuthorization(int type, string userId, string groupId, Resources resource, string resourceId);
        AuthorizationEntity FindAuthorizationByGroupIdAndResourceId(int type, string groupId, Resources resource, string resourceId);
        AuthorizationEntity FindAuthorizationByUserIdAndResourceId(int type, string userId, Resources resource, string resourceId);
        bool IsAuthorized(CompositePermissionCheck compositePermissionCheck);
        bool IsAuthorized(Permissions permission, Resources resource, string resourceId);
        bool IsAuthorized(string userId, IList<string> groupIds, CompositePermissionCheck compositePermissionCheck);
        bool IsAuthorized(string userId, IList<string> groupIds, IList<PermissionCheck> permissionChecks);
        bool IsAuthorized(string userId, IList<string> groupIds, Permissions permission, Resources resource, string resourceId);
        bool IsCamundaAdmin(Authentication authentication);
        PermissionCheck NewPermissionCheck();
        PermissionCheckBuilder NewPermissionCheckBuilder();
    }
}