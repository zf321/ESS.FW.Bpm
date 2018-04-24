namespace ESS.FW.Bpm.Engine.Identity
{
    /// <summary>
    ///     Represents a user, used in <seealso cref="IdentityService" />.
    ///      
    /// </summary>
    public interface IUser
    {
        string Id { get; set; }

        string FirstName { get; set; }

        string LastName { set; get; }

        string Email { set; get; }

        string Password { get; set; }
    }
}