using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Common
{
    public class ResourceHelper
    {
        public static readonly string BaseResourcePath =AppDomain.CurrentDomain.BaseDirectory;
        public static Stream GetStreamFromResource(string resourceName)
        {
            string path = Path.Combine(BaseResourcePath, resourceName);
            using(FileStream fileStream = File.OpenRead(path))
            {
                byte[] bts = new byte[fileStream.Length];
                fileStream.Read(bts, 0, bts.Length);
                fileStream.Flush();
                fileStream.Dispose();
                return new MemoryStream(bts);
            }
        }
        public static string GetStringFromResource(string resourceName)
        {
            string path = Path.Combine(BaseResourcePath, resourceName);
            return File.ReadAllText(path);
        }
    }
}
