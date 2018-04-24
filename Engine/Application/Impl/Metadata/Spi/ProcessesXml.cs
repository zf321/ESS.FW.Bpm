using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application.Impl.Metadata.Spi
{
    /// <summary>
    ///     <para>Java API representation of the <seealso cref="ProcessesXml" /> Metadata.</para>
    ///     
    /// </summary>
    public abstract class ProcessesXml
    {
        /// <para>Constant representing the empty processes.xml</para>
        /// <summary>
        /// </summary>
        public static ProcessesXml EmptyProcessesXml;

        /// <returns>
        ///     A <seealso cref="List" /> of <seealso cref="ProcessEngineXml" /> Metadata Items representing process engine
        ///     configurations.
        /// </returns>
        //public IList<ProcessEngineXml> ProcessEngines { get; }

        /// <returns>
        ///     A <seealso cref="List" /> of <seealso cref="IProcessArchiveXml" /> Metadata Items representing process
        ///     archive deployments.
        /// </returns>
        public virtual IList<IProcessArchiveXml> ProcessArchives { get; }

        //public List<ProcessEngineXml> GetProcessEngines()
        //{
        //    return null;
        //}

        public List<IProcessArchiveXml> GetProcessArchives()
        {
            throw new NotImplementedException();
            
            //var processArchives = new List<IProcessArchiveXml>();

            //// add single PA
            //var pa = new ProcessArchiveXmlImpl();
            //processArchives.Add(pa);

            //pa.ProcessResourceNames = null;

            //// with default properties
            //var properties = new Dictionary<string, string>();
            //pa.Properties = properties;
            //properties.Add(ProcessArchiveXmlFields.PropIsDeleteUponUndeploy, false.ToString());
            //properties.Add(ProcessArchiveXmlFields.PropIsScanForProcessDefinitions, true.ToString());
            //properties.Add(ProcessArchiveXmlFields.PropIsDeployChangedOnly, false.ToString());
            //properties.Add(ProcessArchiveXmlFields.PropResumePreviousBy,
            //    ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY);

            //return processArchives;
        }
    }
}