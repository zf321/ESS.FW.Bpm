using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application.Impl.Metadata.Spi;

namespace ESS.FW.Bpm.Engine.Application.Impl.Metadata
{
    public class ProcessArchiveXmlImpl : IProcessArchiveXml
    {
        public virtual string Name { get; set; }


        public virtual string TenantId { get; set; }


        public virtual string ProcessEngineName { get; set; }


        public virtual IList<string> ProcessResourceNames { get; set; }


        public virtual IDictionary<string, string> Properties { get; set; }
    }
}