using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Impl.Plugin
{

    /// <summary>
    ///     
    /// </summary>
    public class AdministratorAuthorizationPlugin : AbstractProcessEnginePlugin
    {
        private static readonly AdministratorAuthorizationPluginLogger Log = ProcessEngineLogger.AdminPluginLogger;

        /// <summary>
        ///     The name of the administrator group.
        ///     If this name is set to a non-null and non-empty value,
        ///     the plugin will create group-level Administrator authorizations
        ///     on all built-in resources.
        /// </summary>
        protected internal string administratorGroupName;

        /// <summary>
        ///     The name of the administrator group.
        ///     If this name is set to a non-null and non-empty value,
        ///     the plugin will create group-level Administrator authorizations
        ///     on all built-in resources.
        /// </summary>
        protected internal string administratorUserName;

        protected internal bool AuthorizationEnabled;


        // getter / setters ////////////////////////////////////

        public virtual string AdministratorGroupName
        {
            get { return administratorGroupName; }
            set { administratorGroupName = value; }
        }


        public virtual string AdministratorUserName
        {
            get { return administratorUserName; }
            set { administratorUserName = value; }
        }

        public override void PostInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            AuthorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
        }

        public override void PostProcessEngineBuild(IProcessEngine processEngine)
        {
            if (!AuthorizationEnabled)
                return;
            
            var authorizationService = processEngine.AuthorizationService;

            if (!ReferenceEquals(administratorGroupName, null) && (administratorGroupName.Length > 0))
            {
                // create ADMIN authorizations on all built-in resources for configured group
                //foreach (Resource resource in Resources.values())
                //{
                //    if (
                //        authorizationService.createAuthorizationQuery()
                //            .groupIdIn(administratorGroupName)
                //            .resourceType(resource)
                //            .resourceId(ANY)
                //            .count() == 0)
                //    {
                //        AuthorizationEntity adminGroupAuth = new AuthorizationEntity(AUTH_TYPE_GRANT);
                //        adminGroupAuth.GroupId = administratorGroupName;
                //        adminGroupAuth.setResource(resource);
                //        adminGroupAuth.ResourceId = ANY;
                //        adminGroupAuth.addPermission(ALL);
                //        authorizationService.saveAuthorization(adminGroupAuth);
                //        LOG.grantGroupPermissions(administratorGroupName, resource.resourceName());
                //    }
                //}
            }

            if (!ReferenceEquals(administratorUserName, null) && (administratorUserName.Length > 0))
            {
                // create ADMIN authorizations on all built-in resources for configured user
                //foreach (Resource resource in Resources.values())
                //{
                //    if (
                //        authorizationService.createAuthorizationQuery()
                //            .userIdIn(administratorUserName)
                //            .resourceType(resource)
                //            .resourceId(ANY)
                //            .count() == 0)
                //    {
                //        AuthorizationEntity adminUserAuth = new AuthorizationEntity(AUTH_TYPE_GRANT);
                //        adminUserAuth.UserId = administratorUserName;
                //        adminUserAuth.setResource(resource);
                //        adminUserAuth.ResourceId = ANY;
                //        adminUserAuth.addPermission(ALL);
                //        authorizationService.saveAuthorization(adminUserAuth);
                //        LOG.grantUserPermissions(administratorUserName, resource.resourceName());
                //    }
                //}
            }
        }
    }
}