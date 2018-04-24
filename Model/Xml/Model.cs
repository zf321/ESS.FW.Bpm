using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Xml
{
    public interface IModel
    {
        ICollection<IModelElementType> Types { get; }

        IModelElementType GetType(Type instanceClass);

        IModelElementType GetTypeForName(string typeName);

        IModelElementType GetTypeForName(string namespaceUri, string typeName);

        string ModelName { get; }

        string GetActualNamespace(string alternativeNs);

        string GetAlternativeNamespace(string actualNs);

    }

}