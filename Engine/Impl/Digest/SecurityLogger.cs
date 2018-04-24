using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    public class SecurityLogger : ProcessEngineLogger
    {
        public virtual ProcessEngineException CannotResolveAlgorithmPrefixFromGivenPasswordException(
            string resolvedHashAlgorithmName, ICollection<string> availableHashAlgorithmNames)
        {
            return
                new PasswordEncryptionException(ExceptionMessage("001",
                    "Could not resolve hash algorithm name of a hashed password. Resolved hash algorithm name {}. Available hash algorithms {}",
                    resolvedHashAlgorithmName, availableHashAlgorithmNames));
        }

        public virtual ProcessEngineException HashAlgorithmForPasswordEncryptionAlreadyAvailableException(
            string hashAlgorithmName)
        {
            return
                new PasswordEncryptionException(ExceptionMessage("002",
                    "Hash algorithm with the name '{}' was already added. The algorithm cannot be added twice!",
                    hashAlgorithmName));
        }
    }
}