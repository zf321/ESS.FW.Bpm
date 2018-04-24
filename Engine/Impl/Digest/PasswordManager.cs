using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{


    /// <summary>
    ///     Different Camunda versions use different hashing algorithms. In addition, it is possible
    ///     to add a custom hashing algorithm. The <seealso cref="PasswordManager" /> ensures that the right
    ///     algorithm is used for the encryption.
    ///     Default algorithms:
    ///     Version:           |    Algorithm
    ///     <= Camunda 7.6     | SHA1
    ///     >= Camunda 7.7     | SHA512
    /// </summary>
    public class PasswordManager
    {
        public static readonly SecurityLogger Log = ProcessEngineLogger.SecurityLogger;
        protected internal IPasswordEncryptor DefaultPasswordEncryptor;

        protected internal IDictionary<string, IPasswordEncryptor> PasswordChecker =
            new Dictionary<string, IPasswordEncryptor>();

        protected internal DatabasePrefixHandler PrefixHandler = new DatabasePrefixHandler();

        public PasswordManager(IPasswordEncryptor defaultPasswordEncryptor,
            IList<IPasswordEncryptor> customPasswordChecker)
        {
            // add default password encryptors for password checking
            // for Camunda 7.6 and earlier
            AddPasswordCheckerAndThrowErrorIfAlreadyAvailable(new ShaHashDigest());
            // from Camunda 7.7
            AddPasswordCheckerAndThrowErrorIfAlreadyAvailable(new Sha512HashDigest());

            // add custom encryptors
            AddAllPasswordChecker(customPasswordChecker);

            AddDefaultEncryptor(defaultPasswordEncryptor);
        }

        protected internal virtual void AddAllPasswordChecker(IList<IPasswordEncryptor> list)
        {
            foreach (var encryptor in list)
                AddPasswordCheckerAndThrowErrorIfAlreadyAvailable(encryptor);
        }

        protected internal virtual void AddPasswordCheckerAndThrowErrorIfAlreadyAvailable(IPasswordEncryptor encryptor)
        {
            if (PasswordChecker.ContainsKey(encryptor.HashAlgorithmName()))
                throw Log.HashAlgorithmForPasswordEncryptionAlreadyAvailableException(encryptor.HashAlgorithmName());
            PasswordChecker[encryptor.HashAlgorithmName()] = encryptor;
        }

        protected internal virtual void AddDefaultEncryptor(IPasswordEncryptor defaultPasswordEncryptor)
        {
            this.DefaultPasswordEncryptor = defaultPasswordEncryptor;
            PasswordChecker[defaultPasswordEncryptor.HashAlgorithmName()] = defaultPasswordEncryptor;
        }

        public virtual string Encrypt(string password)
        {
            var prefix = PrefixHandler.GeneratePrefix(DefaultPasswordEncryptor.HashAlgorithmName());
            return prefix + DefaultPasswordEncryptor.Encrypt(password);
        }

        public virtual bool Check(string password, string encrypted)
        {
            var encryptor = GetCorrectEncryptorForPassword(encrypted);
            var encryptedPasswordWithoutPrefix = PrefixHandler.RemovePrefix(encrypted);
            EnsureUtil.EnsureNotNull("encryptedPasswordWithoutPrefix", encryptedPasswordWithoutPrefix);
            return encryptor.Check(password, encryptedPasswordWithoutPrefix);
        }

        protected internal virtual IPasswordEncryptor GetCorrectEncryptorForPassword(string encryptedPassword)
        {
            var hashAlgorithmName = PrefixHandler.RetrieveAlgorithmName(encryptedPassword);
            if (ReferenceEquals(hashAlgorithmName, null) || !PasswordChecker.ContainsKey(hashAlgorithmName))
                throw Log.CannotResolveAlgorithmPrefixFromGivenPasswordException(hashAlgorithmName, PasswordChecker.Keys);
            return PasswordChecker[hashAlgorithmName];
        }
    }
}