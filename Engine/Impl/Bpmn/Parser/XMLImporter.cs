using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     A XML importer
    ///     
    /// </summary>
    public interface IXmlImporter
    {
        /// <summary>
        ///     Imports the definitions in the XML declared in element
        /// </summary>
        /// <param name="element"> the declarations to be imported </param>
        /// <param name="parse"> the parse who called this importer </param>
        void ImportFrom(Element element, BpmnParse parse);
    }
}