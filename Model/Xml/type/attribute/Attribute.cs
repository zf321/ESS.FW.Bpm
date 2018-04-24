using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.type.attribute
{
    public interface IAttribute
    {
        T GetValue<T>(IModelElementInstance modelElement);

        void SetValue<T>(IModelElementInstance modelElement, T value);

        T GetDefaultValue<T>();

        bool Required { get; }

        /// <returns> the namespaceUri </returns>
        string NamespaceUri { get; }

        /// <returns> the attributeName </returns>
        string AttributeName { get; }

        bool IdAttribute { get; }

        IModelElementType OwningElementType { get; }

        IList<IReference> IncomingReferences { get; }

        IList<IReference> OutgoingReferences { get; }
    }
}