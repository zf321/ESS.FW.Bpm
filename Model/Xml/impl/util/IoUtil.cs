//                            _ooOoo_  
//                           o8888888o  
//                           88" . "88  
//                           (| -_- |)  
//                            O\ = /O  
//                        ____/`---'\____  
//                      .   ' \\| |// `.  
//                       / \\||| : |||// \  
//                     / _||||| -:- |||||- \  
//                       | | \\\ - /// | |  
//                     | \_| ''\---/'' | |  
//                      \ .-\__ `-` ___/-. /  
//                   ___`. .' /--.--\ `. . __  
//                ."" '< `.___\_<|>_/___.' >'"".  
//               | | : `- \`.;`\ _ /`;.`/ - ` : | |  
//                 \ \ `-. \_ __\ /__ _/ .-` / /  
//         ======`-.____`-.___\_____/___.-`____.-'======  
//                            `=---='  
//  
//         .............................................  
//                  佛祖镇楼                  BUG辟易  
//          佛曰:  
//                  写字楼里写字间，写字间里程序员；  
//                  程序人员写程序，又拿程序换酒钱。  
//                  酒醒只在网上坐，酒醉还来网下眠；  
//                  酒醉酒醒日复日，网上网下年复年。  
//                  但愿老死电脑间，不愿鞠躬老板前；  
//                  奔驰宝马贵者趣，公交自行程序员。  
//                  别人笑我忒疯癫，我笑自己命太贱；  
//                  不见满街漂亮妹，哪个归得程序员？

using System;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.impl.util
{
    public sealed class IoUtil
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


        public static string GetStringFromInputStream(Stream inputStream)
        {
            return GetStringFromInputStream(inputStream, true);
        }

        private static string GetStringFromInputStream(Stream inputStream, bool trim)
        {
            using (StreamReader bufferedReader = new StreamReader(inputStream))
            {
                StringBuilder stringBuilder = new StringBuilder();
                string line;
                while (!string.IsNullOrEmpty((line = bufferedReader.ReadLine())))
                {
                    if (trim)
                    {
                        stringBuilder.Append(line.Trim());
                    }
                    else
                    {
                        stringBuilder.Append(line).Append("\n");
                    }
                }
                return stringBuilder.ToString();
            }
        }

        public static Stream ConvertOutputStreamToInputStream(Stream outputStream)
        {
            byte[] data = ((MemoryStream)outputStream).GetBuffer();
            return new MemoryStream(data);
        }

        /// <summary>
        /// Converts a <seealso cref="DomDocument"/> to its String representation
        /// </summary>
        /// <param name="document">  the XML document to convert </param>
        public static string ConvertXmlDocumentToString(IDomDocument document)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {

                TransformDocumentToXml(document, writer);
                return sb.ToString();
            }
        }
        
        public static void WriteDocumentToOutputStream(IDomDocument document, Stream outputStream)
        {
            using (TextWriter writer = new StreamWriter(outputStream))
            {
                TransformDocumentToXml(document, writer);
            }
        }

        public static void TransformDocumentToXml(IDomDocument document, TextWriter writer)
        {
            try
            {
                //document.DomSource.Declaration.Encoding = Encoding.UTF8.EncodingName;
                lock (document)
                {
                    document.DomSource.Save(writer);
                }
            }
            catch (Exception ex)
            {
                throw new ModelIoException("Unable to transform model to xml", ex);
            }

        }

    }

}