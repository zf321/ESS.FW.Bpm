using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class Base64EncodedHashDigest
    {
        //public virtual string encrypt(string password)
        //{
        //    // create hash as byte array
        //    var hash = createByteHash(password);

        //    // stringify hash (default implementation use BASE64 encoding)
        //    return encodeHash(hash);
        //}

        //public virtual bool check(string password, string encrypted)
        //{
        //    return encrypt(password).Equals(encrypted);
        //}

        //protected internal virtual byte[] createByteHash(string password)
        //{
        //    MessageDigest digest = createDigestInstance();
        //    try
        //    {
        //        digest.update(password.GetBytes(Encoding.UTF8));
        //        return digest.digest();
        //    }
        //    catch (UnsupportedEncodingException)
        //    {
        //        throw new ProcessEngineException("UnsupportedEncodingException while calculating password digest");
        //    }
        //}

        //protected internal virtual MessageDigest createDigestInstance()
        //{
        //    try
        //    {
        //        return MessageDigest.getInstance(hashAlgorithmName());
        //    }
        //    catch (NoSuchAlgorithmException)
        //    {
        //        throw new ProcessEngineException("Cannot lookup " + hashAlgorithmName() + " algorithm");
        //    }
        //}

        protected internal virtual string EncodeHash(byte[] hash)
        {
            return System.Text.Encoding.UTF8.GetString(hash);
            //return StringHelperClass.NewString(Base64.EncodeBase64(hash));
        }

        /// <summary>
        ///     allows subclasses to select the hash algorithm
        /// </summary>
        protected internal abstract string HashAlgorithmName();
    }
}