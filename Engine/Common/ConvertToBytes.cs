

namespace System
{
    public static class ConvertToBytes
    {
        /// <summary>
        /// Bytes[]转换成byte[]
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this byte[] source)
        {
            byte[] bts=new byte[source.Length];
            Buffer.BlockCopy(source, 0, bts, 0, source.Length);
            return bts;
        }

        public static byte[] ToSBytes(this byte[] source)
        {
            byte[] bts=new byte[source.Length];
            Buffer.BlockCopy(source,0,bts,0,source.Length);
            return bts;
        }
    }
}
