using System;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    /// <summary>
    ///     <para><seealso cref="IPasswordEncryptor" /> implementation using base64 encoded SHA password hashes</para>
    ///     
    /// </summary>
    public class ShaHashDigest : Base64EncodedHashDigest, IPasswordEncryptor
    {
        public bool Check(string password, string encrypted)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string password)
        {
            throw new NotImplementedException();
        }

        string IPasswordEncryptor.HashAlgorithmName()
        {
            return "SHA";
            
        }

        protected internal override string HashAlgorithmName()
        {
            return "SHA";
        }
    }
}