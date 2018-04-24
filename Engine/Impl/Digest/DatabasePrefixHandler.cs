using System;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    /// <summary>
    ///     In order to distinguish between the used hashed algorithm
    ///     for the password encryption, as prefix is persisted with the
    ///     encrypted to the database.
    ///     The <seealso cref="DatabasePrefixHandler" /> is used to handle the prefix, especially for building
    ///     the prefix, retrieving the algorithm name from the prefix and
    ///     removing the prefix name from the hashed password.
    /// </summary>
    public class DatabasePrefixHandler
    {
        protected internal string Pattern = "^\\{(.*?)\\}";

        public virtual string GeneratePrefix(string algorithmName)
        {
            return "{" + algorithmName + "}";
        }

        public virtual string RetrieveAlgorithmName(string encryptedPasswordWithPrefix)
        {
            var reg = new Regex(Pattern);
            var matcher = reg.Match(encryptedPasswordWithPrefix);
            if (matcher.Success)
                return matcher.Groups[0].Value;
            return null;
        }

        public virtual string RemovePrefix(string encryptedPasswordWithPrefix)
        {
            var index = encryptedPasswordWithPrefix.IndexOf("}", StringComparison.Ordinal);
            if (!encryptedPasswordWithPrefix.StartsWith("{", StringComparison.Ordinal) || (index < 0))
                return null;
            return encryptedPasswordWithPrefix.Substring(index + 1);
        }
    }
}