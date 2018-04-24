namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    /// <summary>
    ///     The <seealso cref="IPasswordEncryptor" /> provides the api to customize
    ///     the encryption of passwords.
    ///     
    ///     
    /// </summary>
    public interface IPasswordEncryptor
    {
        /// <summary>
        ///     Encrypt the given password
        /// </summary>
        /// <param name="password">
        ///     @return
        /// </param>
        string Encrypt(string password);

        /// <summary>
        ///     Returns true if the given plain text equals to the encrypted password.
        /// </summary>
        /// <param name="password"> </param>
        /// <param name="encrypted">
        ///     @return
        /// </param>
        bool Check(string password, string encrypted);

        /// <summary>
        ///     In order to distinguish which algorithm was used to hash the
        ///     password, it needs a unique id. In particular, this is needed
        ///     for <seealso cref="#check" />.
        /// </summary>
        /// <returns> the name of the algorithm </returns>
        string HashAlgorithmName();
    }
}