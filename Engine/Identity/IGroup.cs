namespace ESS.FW.Bpm.Engine.Identity
{
    /// <summary>
    ///     Represents a group, used in <seealso cref="IdentityService" />.
    ///      
    /// </summary>
    public interface IGroup
    {
        string Id { get; set; }

        string Name { get; set; }

        string Type { get; set; }
    }
}