using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Identity
{
    /// <summary>
    ///     <para>
    ///         Allows to expose the id of the currently authenticated user,
    ///         his groups and his tenants to the process engine.
    ///     </para>
    ///     <para>
    ///         The current authentication is managed using a Thread Local. The value can
    ///         be set using <seealso cref="#setCurrentAuthentication(String, List)" />,
    ///         retrieved using <seealso cref="#getCurrentAuthentication()" /> and cleared
    ///         using <seealso cref="#clearCurrentAuthentication()" />.
    ///     </para>
    ///     <para>
    ///         Users typically do not use this class directly but rather use
    ///         the corresponding Service API methods:
    ///         <ul>
    ///             <li></li>
    ///         </ul>
    ///     </para>
    ///      
    ///     
    /// </summary>
    public class Authentication
    {
        protected internal IList<string> AuthenticatedGroupIds;
        protected internal IList<string> AuthenticatedTenantIds;

        protected internal string AuthenticatedUserId;

        public Authentication()
        {
        }

        public Authentication(string authenticatedUserId, IList<string> groupIds)
            : this(authenticatedUserId, groupIds, null)
        {
        }

        public Authentication(string authenticatedUserId, IList<string> authenticatedGroupIds,
            IList<string> authenticatedTenantIds)
        {
            this.AuthenticatedUserId = authenticatedUserId;

            if (authenticatedGroupIds != null)
                this.AuthenticatedGroupIds = new List<string>(authenticatedGroupIds);

            if (authenticatedTenantIds != null)
                this.AuthenticatedTenantIds = new List<string>(authenticatedTenantIds);
        }

        public virtual IList<string> GroupIds
        {
            get { return AuthenticatedGroupIds; }
        }

        public virtual string UserId
        {
            get { return AuthenticatedUserId; }
        }

        public virtual IList<string> TenantIds
        {
            get { return AuthenticatedTenantIds; }
        }
    }
}