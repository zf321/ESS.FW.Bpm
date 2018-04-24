namespace ESS.FW.Bpm.Engine.Authorization
{
    /// <summary>
    ///     Holds the set of built-in user identities for camunda BPM.
    ///     
    /// </summary>
    public interface IGroups
    {
    }

    public static class GroupsFields
    {
        public const string CamundaAdmin = "camunda-admin";
        public const string GroupTypeSystem = "SYSTEM";
        public const string GroupTypeWorkflow = "WORKFLOW";
    }
}