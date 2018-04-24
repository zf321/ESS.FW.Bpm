namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    /// <summary>
    ///     <para>Exception thrown during the encryption process.</para>
    ///     <para>
    ///         <strong>Possible reasons:</strong>
    ///         <ul>
    ///             <li>several hashing algorithms with the same prefix are added</li>
    ///             <li>cannot resolve the hash algorithm prefix from a given encrypted password</li>
    ///         </ul>
    ///     </para>
    /// </summary>
    public class PasswordEncryptionException : ProcessEngineException
    {
        

        public PasswordEncryptionException(string message) : base(message)
        {
        }
    }
}