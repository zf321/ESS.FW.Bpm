namespace ESS.FW.Bpm.Engine.Impl.Util
{
    public sealed class EncryptionUtil
    {
        public static string SaltPassword(string password, string salt)
        {
            // Before version 7.7 no salt was used. Thus, if no salt
            // is available an empty salt should be used.
            if (ReferenceEquals(salt, null))
                salt = "";
            return password + salt;
        }
    }
}