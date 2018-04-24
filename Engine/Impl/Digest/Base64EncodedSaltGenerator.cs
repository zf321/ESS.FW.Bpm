using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;
using System;

namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    public abstract class Base64EncodedSaltGenerator : ISaltGenerator
    {
        //protected internal Random secureRandom = new SecureRandom();

        protected internal abstract int? SaltLengthInByte { get; }

        public virtual string GenerateSalt()
        {
            var byteSalt = GenerateByteSalt();
            return EncodeSalt(byteSalt);
        }

        protected internal virtual byte[] GenerateByteSalt()
        {
            throw new NotImplementedException();
            //var salt = new byte[(int)SaltLengthInByte];
            //SecureRandom.NextBytes(salt);
            //return salt;
        }

        protected internal virtual string EncodeSalt(byte[] salt)
        {
            return System.Text.Encoding.UTF8.GetString(salt);
            //return StringHelperClass.NewString(Base64.EncodeBase64(salt));
        }
    }
}