using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
    /// The BPMN extensionElements element
    /// 
    /// 
    /// </summary>
    public interface IExtensionElements : IBpmnModelElementInstance
    {

        ICollection<IModelElementInstance> Elements { get; }

        IQuery<IModelElementInstance> ElementsQuery { get; }

        IModelElementInstance AddExtensionElement(string namespaceUri, string localName);

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T AddExtensionElement<T>(System.Type extensionElementClass) where T : IModelElementInstance;
    }

}