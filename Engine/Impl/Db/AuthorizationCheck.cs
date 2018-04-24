using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     <para>Input for the authorization check algorithm</para>
    ///     
    /// </summary>
    [Serializable]
    public class AuthorizationCheck
    {
        private const long SerialVersionUid = 1L;

        /// <summary>
        ///     the default permissions to use if no matching authorization
        ///     can be found.
        /// </summary>
        protected internal int authDefaultPerm = (int)Permissions.All;

        /// <summary>
        ///     the ids of the groups to check permissions for
        /// </summary>
        protected internal IList<string> authGroupIds = new List<string>();

        /// <summary>
        ///     the id of the user to check permissions for
        /// </summary>
        protected internal string authUserId;

        /// <summary>
        ///     If true authorization check is performed. This switch is
        ///     useful when implementing a query which may perform an authorization check
        ///     only under certain circumstances.
        /// </summary>
        protected internal bool isAuthorizationCheckEnabled;

        /// <summary>
        ///     Indicates if the revoke authorization checks are enabled or not.
        ///     The authorization checks without checking revoke permissions are much more faster.
        /// </summary>
        protected internal bool IsRevokeAuthorizationCheckEnabled;

        protected internal CompositePermissionCheck permissionChecks = new CompositePermissionCheck();

        public AuthorizationCheck()
        {
        }

        public AuthorizationCheck(string authUserId, IList<string> authGroupIds, IList<PermissionCheck> permissionChecks,
            bool isRevokeAuthorizationCheckEnabled)
        {
            this.authUserId = authUserId;
            this.authGroupIds = authGroupIds;
            this.permissionChecks.AtomicChecks = permissionChecks;
            this.IsRevokeAuthorizationCheckEnabled = isRevokeAuthorizationCheckEnabled;
        }

        public AuthorizationCheck(string authUserId, IList<string> authGroupIds,
            CompositePermissionCheck permissionCheck, bool isRevokeAuthorizationCheckEnabled)
        {
            this.authUserId = authUserId;
            this.authGroupIds = authGroupIds;
            permissionChecks = permissionCheck;
            this.IsRevokeAuthorizationCheckEnabled = isRevokeAuthorizationCheckEnabled;
        }

        // getters / setters /////////////////////////////////////////

        public virtual bool AuthorizationCheckEnabled
        {
            get { return isAuthorizationCheckEnabled; }
            set { isAuthorizationCheckEnabled = value; }
        }

        /// <summary>
        ///     is used by myBatis
        /// </summary>
        public virtual bool IsAuthorizationCheckEnabled
        {
            get { return isAuthorizationCheckEnabled; }
        }


        public virtual string AuthUserId
        {
            get { return authUserId; }
            set { authUserId = value; }
        }


        public virtual IList<string> AuthGroupIds
        {
            get { return authGroupIds; }
            set { authGroupIds = value; }
        }


        public virtual int AuthDefaultPerm
        {
            get { return authDefaultPerm; }
            set { authDefaultPerm = value; }
        }


        // authorization check parameters

        public virtual CompositePermissionCheck PermissionChecks
        {
            get { return permissionChecks; }
            set { permissionChecks = value; }
        }

        public virtual IList<PermissionCheck> AtomicPermissionChecks
        {
            set { permissionChecks.AtomicChecks = value; }
        }


        public virtual bool RevokeAuthorizationCheckEnabled
        {
            get { return IsRevokeAuthorizationCheckEnabled; }
            set { IsRevokeAuthorizationCheckEnabled = value; }
        }

        public virtual void AddAtomicPermissionCheck(PermissionCheck permissionCheck)
        {
            permissionChecks.AddAtomicCheck(permissionCheck);
        }
    }
}