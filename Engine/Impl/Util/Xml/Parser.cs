namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///     解析器
    /// </summary>
    public class Parser
    {
        //protected internal static SAXParserFactory defaultSaxParserFactory = SAXParserFactory.newInstance();

        public static readonly Parser Instance = new Parser();

        //ORIGINAL LINE: protected javax.xml.parsers.SAXParser getSaxParser() throws Exception

        //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        //protected internal virtual SAXParser SaxParser
        //{
        // get
        // {
        //return SaxParserFactory.newSAXParser();
        // }
        //}
        public XmlParser XmlParser = XmlParserFactory.Create();

        public virtual Parse CreateParse()
        {
            return new Parse(this);
        }

        //protected internal virtual SAXParserFactory SaxParserFactory
        //{

        // get
        // {
        //return defaultSaxParserFactory;
        // }
        //}
    }
}