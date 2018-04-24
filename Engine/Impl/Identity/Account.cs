using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Identity
{
    /// <summary>
    ///      
    /// </summary>
    public interface IAccount
    {
        string Name { get; }
        string Username { get; }
        string Password { get; }
        IDictionary<string, string> Details { get; }
    }

    public static class AccountFields
    {
        public const string NameAlfresco = "Alfresco";
        public const string NameGoogle = "Google";
        public const string NameSkype = "Skype";
        public const string NameMail = "Mail";
    }
}