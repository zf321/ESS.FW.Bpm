namespace ESS.FW.Bpm.Engine.Identity
{
    /// <summary>
    ///     Represents a tenant, used in <seealso cref="IdentityService" />.
    /// </summary>
    public interface ITenant
    {
        /// <returns> the id of the tenant </returns>
        string Id { get; set; }


        /// <returns> the name of the tenant </returns>
        string Name { get; set; }
    }
}