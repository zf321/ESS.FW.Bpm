using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application.Impl.Metadata.Spi;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Application.Impl.Metadata
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //import  org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS_ARCHIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS_ENGINE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROPERTIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.RESOURCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.TENANT_ID;

    /// <summary>
    ///     <para><seealso cref="Parse" /> object for the <code>processes.xml</code> file.</para>
    ///     <para>This class is NOT Threadsafe</para>
    ///     
    /// </summary>
    public class ProcessesXmlParse //: DeploymentMetadataParse
    {
        /// <summary>
        ///     the constructed ProcessXml
        /// </summary>
        protected internal ProcessesXml processesXml;

        public ProcessesXmlParse(Parser parser)// : base(parser)
        {
        }

        public virtual ProcessesXml ProcessesXml
        {
            get { return processesXml; }
        }

        public  Parse Execute()
        {
            throw new NotImplementedException();
        //    base.Execute();
        //    return this;
        }

        /// <summary>
        ///     we know this is a <code>&lt;process-application ... /&gt;</code> structure.
        /// </summary>
        protected internal  void ParseRootElement()
        {
            throw new NotImplementedException();
            //IList<ProcessEngineXml> processEngines = new List<ProcessEngineXml>();
            //IList<IProcessArchiveXml> processArchives = new List<IProcessArchiveXml>();

            //foreach (var element in rootElement.GetAllElement())
            //    if (DeploymentMetadataConstants.PROCESS_ENGINE.Equals(element.TagName))
            //        parseProcessEngine(element, processEngines);
            //    else if (DeploymentMetadataConstants.PROCESS_ARCHIVE.Equals(element.TagName))
            //        ParseProcessArchive(element, processArchives);

            //processesXml = new ProcessesXmlImpl(processEngines, processArchives);
        }

        /// <summary>
        ///     parse a <code>&lt;process-archive .../&gt;</code> element and add it to the list of parsed elements
        /// </summary>
        //protected internal virtual void ParseProcessArchive(Element element,
        //    IList<IProcessArchiveXml> parsedProcessArchives)
        //{
        //    var processArchive = new ProcessArchiveXmlImpl();

        //    processArchive.Name = element.GetAttributeValue(DeploymentMetadataConstants.NAME);
        //    processArchive.TenantId = element.GetAttributeValue(DeploymentMetadataConstants.TENANT_ID);

        //    IList<string> processResourceNames = new List<string>();

        //    IDictionary<string, string> properties = new Dictionary<string, string>();
        //    foreach (var childElement in element.GetAllElement())
        //        if (DeploymentMetadataConstants.PROCESS_ENGINE.Equals(childElement.TagName))
        //            processArchive.ProcessEngineName = childElement.Text;
        //        else if (DeploymentMetadataConstants.PROCESS.Equals(childElement.TagName) ||
        //                 DeploymentMetadataConstants.RESOURCE.Equals(childElement.TagName))
        //            processResourceNames.Add(childElement.Text);
        //        else if (DeploymentMetadataConstants.PROPERTIES.Equals(childElement.TagName))
        //            parseProperties(childElement, properties);

        //    // set properties
        //    processArchive.Properties = properties;

        //    // add collected resource names.
        //    processArchive.ProcessResourceNames = processResourceNames;

        //    // add process archive to list of parsed archives.
        //    parsedProcessArchives.Add(processArchive);
        //}

        //public override Parse sourceUrl(Uri url)
        //{
        //    base.sourceUrl(url);
        //    return this;
        //}
    }
}