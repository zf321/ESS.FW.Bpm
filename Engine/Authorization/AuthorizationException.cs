using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Authorization
{
    /// <summary>
    ///     <para>
    ///         Exception thrown by the process engine in case a user tries to
    ///         interact with a resource in an unauthorized way.
    ///     </para>
    ///     <para>
    ///         The exception contains a list of Missing authorizations. The List is a
    ///         disjunction i.e. a user should have any of the authorization for the engine
    ///         to continue the execution beyond the point where it failed.
    ///     </para>
    ///     
    /// </summary>
    public class AuthorizationException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;
        public readonly IList<MissingAuthorization> MissingAuthorizations;

        protected internal readonly string userId;

        [Obsolete] protected internal string PermissionName;

        [Obsolete] protected internal string resourceId;

        // these properties have been replaced by the list of missingAuthorizations
        // and are only left because this is a public API package and users might
        // have subclasses relying on these fields
        [Obsolete] protected internal string resourceType;

        public AuthorizationException(string message) : base(message)
        {
            userId = null;
            MissingAuthorizations = new List<MissingAuthorization>();
        }

        public AuthorizationException(string userId, string permissionName, string resourceType, string resourceId)
            : this(userId, new MissingAuthorization(permissionName, resourceType, resourceId))
        {
        }

        public AuthorizationException(string userId, MissingAuthorization exceptionInfo)
            : base(
                "The user with id '" + userId + "' does not have " + GenerateMissingAuthorizationMessage(exceptionInfo) +
                ".")
        {
            this.userId = userId;
            MissingAuthorizations = new List<MissingAuthorization>();
            MissingAuthorizations.Add(exceptionInfo);

            resourceType = exceptionInfo.ResourceType;
            PermissionName = exceptionInfo.ViolatedPermissionName;
            resourceId = exceptionInfo.ResourceId;
        }

        public AuthorizationException(string userId, IList<MissingAuthorization> info)
            : base(GenerateExceptionMessage(userId, info))
        {
            this.userId = userId;
            MissingAuthorizations = info;
        }

        /// <returns>
        ///     the type of the resource if there
        ///     is only one <seealso cref="MissingAuthorization" />, {@code null} otherwise
        /// </returns>
        /// @deprecated Use
        /// <seealso cref="#getMissingAuthorizations()" />
        /// to get the type of the resource
        /// of the
        /// <seealso cref="MissingAuthorization" />
        /// (s). This method may be removed in future versions.
        [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the type of the resource")]
        public virtual string ResourceType
        {
            get
            {
                string resourceType = null;
                if (MissingAuthorizations.Count == 1)
                    resourceType = MissingAuthorizations[0].ResourceType;
                return resourceType;
            }
        }

        /// <returns>
        ///     the type of the violated permission name if there
        ///     is only one <seealso cref="MissingAuthorization" />, {@code null} otherwise
        /// </returns>
        /// @deprecated Use
        /// <seealso cref="#getMissingAuthorizations()" />
        /// to get the violated permission name
        /// of the
        /// <seealso cref="MissingAuthorization" />
        /// (s). This method may be removed in future versions.
        [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the violated permission name")]
        public virtual string ViolatedPermissionName
        {
            get
            {
                if (MissingAuthorizations.Count == 1)
                    return MissingAuthorizations[0].ViolatedPermissionName;
                return null;
            }
        }

        /// <returns>
        ///     id of the user in which context the request was made and who misses authorizations
        ///     to perform it successfully
        /// </returns>
        public virtual string UserId
        {
            get { return userId; }
        }

        /// <returns>
        ///     the id of the resource if there
        ///     is only one <seealso cref="MissingAuthorization" />, {@code null} otherwise
        /// </returns>
        /// @deprecated Use
        /// <seealso cref="#getMissingAuthorizations()" />
        /// to get the id of the resource
        /// of the
        /// <seealso cref="MissingAuthorization" />
        /// (s). This method may be removed in future versions.
        [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the id of the resource")]
        public virtual string ResourceId
        {
            get
            {
                if (MissingAuthorizations.Count == 1)
                    return MissingAuthorizations[0].ResourceId;
                return null;
            }
        }

        /// <returns>
        ///     Disjunctive list of <seealso cref="MissingAuthorization" /> from
        ///     which a user needs to have at least one for the authorization to pass
        /// </returns>
        //public virtual IList<MissingAuthorization> MissingAuthorizations
        //{
        //    get
        //    {
        //        return (missingAuthorizations);
        //    }
        //}
        /// <summary>
        ///     Generate exception message from the missing authorizations.
        /// </summary>
        /// <param name="userId"> to use </param>
        /// <param name="missingAuthorizations"> to use </param>
        /// <returns> The prepared exception message </returns>
        private static string GenerateExceptionMessage(string userId, IList<MissingAuthorization> missingAuthorizations)
        {
            var sBuilder = new StringBuilder();
            sBuilder.Append("The user with id '");
            sBuilder.Append(userId);
            sBuilder.Append("' does not have one of the following permissions: ");
            var first = true;
            foreach (var missingAuthorization in missingAuthorizations)
            {
                if (!first)
                    sBuilder.Append(" or ");
                else
                    first = false;
                sBuilder.Append(GenerateMissingAuthorizationMessage(missingAuthorization));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        ///     Generated exception message for the missing authorization.
        /// </summary>
        /// <param name="exceptionInfo"> to use </param>
        private static string GenerateMissingAuthorizationMessage(MissingAuthorization exceptionInfo)
        {
            var builder = new StringBuilder();
            var permissionName = exceptionInfo.ViolatedPermissionName;
            var resourceType = exceptionInfo.ResourceType;
            var resourceId = exceptionInfo.ResourceId;
            builder.Append("'");
            builder.Append(permissionName);
            builder.Append("' permission on resource '");
            builder.Append(!ReferenceEquals(resourceId, null) ? resourceId + "' of type '" : "");
            builder.Append(resourceType);
            builder.Append("'");

            return builder.ToString();
        }
    }
}