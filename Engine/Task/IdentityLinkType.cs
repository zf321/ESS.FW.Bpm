namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     Contains constants for all types of identity links that can be used to involve a
    ///     user or group with a certain task.
    /// </summary>
    /// <seealso cref= TaskService# addUserIdentityLink( String, String, String
    /// )
    /// </seealso>
    /// <seealso cref= TaskService# addGroupIdentityLink( String, String, String)
    /// 
    ///     @  Joram Barrez
    /// </seealso>
    public class IdentityLinkType
    {
        // Activiti native roles 

        public const string Assignee = "assignee";

        public const string Candidate = "candidate";

        public const string Owner = "owner";
    }
}