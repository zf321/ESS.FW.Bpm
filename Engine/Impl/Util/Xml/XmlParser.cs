using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///     一个xmlparser只处理一个xml文档
    /// </summary>
    public class XmlParser : IDisposable
    {
        private Stream _stream;
        private XmlDocument _xmlDocument;

        public void Dispose()
        {
            if (_stream != null) _stream = null;
            if (_xmlDocument != null) _xmlDocument = null;
        }

        /// <summary>
        ///     输入xml文件流
        /// </summary>
        /// <param name="inputStream"></param>
        public void SourceInputStream(Stream inputStream)
        {
            _stream = inputStream;
            ExcuteXmlDocument();
        }

        /// <summary>
        ///     输入xml字符串,utf8编码
        /// </summary>
        /// <param name="xmlSource"></param>
        public void SourceXmlString(string xmlSource)
        {
            SourceXmlString(xmlSource, Encoding.UTF8);
        }

        /// <summary>
        ///     输入xml字符串
        /// </summary>
        /// <param name="xmlSource">xml字符串</param>
        /// <param name="encoding">默认utf8</param>
        public void SourceXmlString(string xmlSource, Encoding encoding)
        {
            using (var ms = new MemoryStream(encoding.GetBytes(xmlSource)))
            {
                _stream = ms;
            }
            ExcuteXmlDocument();
        }

        /// <summary>
        ///     输入文件路径,解析文件流
        /// </summary>
        /// <param name="filePath">文件物理路径</param>
        public void SourceFilePath(string filePath)
        {
            using (var sr = File.OpenRead(filePath)) // StreamReader(filePath))
            {
                var bts = new byte[sr.Length];
                sr.Read(bts, 0, bts.Length);
                sr.Flush();
                _stream = new MemoryStream(bts);
            }
            ExcuteXmlDocument();
        }

        public void SourceFileName(string fileName)
        {
            SourceFilePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
        }

        /// <summary>
        ///     下载远程url的xml资源
        /// </summary>
        /// <param name="url"></param>
        public void SourceUrl(string url)
        {
            using (var client = new WebClient())
            {
                _stream = client.OpenRead(url);
            }
            ExcuteXmlDocument();
        }

        /// <summary>
        ///     下载远程uri的xml资源
        /// </summary>
        /// <param name="uri"></param>
        public void SourceUri(Uri uri)
        {
            using (var client = new WebClient())
            {
                _stream = client.OpenRead(uri);
            }
            ExcuteXmlDocument();
        }

        private XmlDocument ExcuteXmlDocument()
        {
            if (_stream == null)
                throw new NullReferenceException("xml数据源为空");
            if (_xmlDocument != null)
                throw new System.Exception(typeof(XmlParser).FullName + ":不要重复输入数据源");
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(_stream);
            return _xmlDocument;
        }

        /// <summary>
        ///     获取解析完成的xmldocument文档(前提:输入源)
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {
            return _xmlDocument;
        }

        public static Stream GetStreamFromFilePath(string filePath)
        {
            using (var sr = File.OpenRead(filePath))
            {
                var bts = new byte[sr.Length];
                sr.Read(bts, 0, bts.Length);
                sr.Flush();
                return new MemoryStream(bts);
            }
        }

        public static XmlNode GetXmlFromFilePath(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            var node = doc.DocumentElement;
            doc = null;
            return node;
        }
    }
}