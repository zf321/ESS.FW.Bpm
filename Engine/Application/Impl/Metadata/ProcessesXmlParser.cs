using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Application.Impl.Metadata
{
    /// <summary>
    ///     <para>A SAX Parser for the processes.xml file</para>
    ///     
    /// </summary>
    public class ProcessesXmlParser : Parser
    {
        /// <summary>
        ///     The process application namespace
        /// </summary>
        public const string ProcessAppNs = "http://www.camunda.org/schema/1.0/ProcessApplication";

        /// <summary>
        ///     The location of the XSD file in the classpath.
        /// </summary>
        public const string ProcessApplicationXsd = "ProcessApplication.xsd";

        /// <summary>
        ///     create an configure the <seealso cref="ProcessesXmlParse" /> object.
        /// </summary>
        //public override Parse createParse()
        //{
        //    var processesXmlParse = new ProcessesXmlParse(this);
        //    processesXmlParse.SchemaResource = ReflectUtil.getResourceUrlAsString(ProcessApplicationXsd);
        //    return processesXmlParse;
        //}
    }
}