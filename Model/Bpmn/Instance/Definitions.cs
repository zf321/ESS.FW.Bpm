using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
    /// The BPMN definitions element
    /// 
    /// 
    /// </summary>
    public interface IDefinitions : IBpmnModelElementInstance
    {

        string Id { get; set; }


        string Name { get; set; }


        string TargetNamespace { get; set; }


        string ExpressionLanguage { get; set; }


        string TypeLanguage { get; set; }


        string Exporter { get; set; }


        string ExporterVersion { get; set; }


        ICollection<IMport> Imports { get; }

        ICollection<IExtension> Extensions { get; }

        ICollection<IRootElement> RootElements { get; }

        ICollection<IBpmnDiagram> BpmDiagrams { get; }

        ICollection<IRelationship> Relationships { get; }
    }
}