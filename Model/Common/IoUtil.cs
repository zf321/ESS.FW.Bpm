
//using System.IO;

using System.IO;

namespace ESS.FW.Bpm.Model.Common
{
    public class IoUtil
    {
        public static Stream FileAsStream(string filePath)
        {
            using (FileStream fStream = File.OpenRead(filePath))
            {
                byte[] bts = new byte[fStream.Length];
                fStream.Read(bts, 0, bts.Length);
                fStream.Flush();
                return new MemoryStream(bts);
            }
        }
    }
}
