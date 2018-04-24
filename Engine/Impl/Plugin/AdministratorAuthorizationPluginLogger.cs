namespace ESS.FW.Bpm.Engine.Impl.Plugin
{
    /// <summary>
    ///     
    /// </summary>
    public class AdministratorAuthorizationPluginLogger : ProcessEngineLogger
    {
        public virtual void GrantGroupPermissions(string administratorGroupName, string resourceName)
        {
            LogInfo("001", "GRANT group {} ALL permissions on resource {}.", administratorGroupName, resourceName);
        }

        public virtual void GrantUserPermissions(string administratorUserName, string resourceName)
        {
            LogInfo("002", "GRANT user {} ALL permissions on resource {}.", administratorUserName, resourceName);
        }
    }
}