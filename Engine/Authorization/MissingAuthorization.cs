namespace ESS.FW.Bpm.Engine.Authorization
{
    /// <summary>
    ///     Wrapper containing the missing authorization information. It contains the name of the violated permission,
    ///     the type of the resouce and the Id of the resource.
    ///      
    /// </summary>
    public class MissingAuthorization
    {
        public MissingAuthorization(string permissionName, string resourceType, string resourceId)
        {
            this.ViolatedPermissionName = permissionName;
            this.ResourceType = resourceType;
            this.ResourceId = resourceId;
        }

        public virtual string ViolatedPermissionName { get; }

        public virtual string ResourceType { get; }

        public virtual string ResourceId { get; }

        public override string ToString()
        {
            return GetType().Name + "[permissionName=" + ViolatedPermissionName + ", resourceType=" + ResourceType +
                   ", resourceId=" + ResourceId + "]";
        }
    }
}