using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Dmn.instance
{
    /// <summary>
    /// The DMN extensionElements element
    /// </summary>
    public interface IExtensionElements : IDmnModelElementInstance
    {

        ICollection<IModelElementInstance> Elements { get; }

        IQuery<IModelElementInstance> ElementsQuery { get; }

        IModelElementInstance AddExtensionElement(string namespaceUri, string localName);

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T AddExtensionElement<T>(IType extensionElementClass) where T : IModelElementInstance;
    }
}