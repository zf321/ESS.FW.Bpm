using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.type
{
    public interface IModelElementType
    {

        string TypeName { get; }

        string TypeNamespace { get; }

        Type InstanceType { get; }

        T GetInstanceType<T>() where T : IModelElementInstance;
        
        IList<IAttribute> Attributes { get; }

        IModelElementInstance NewInstance(IModelInstance modelInstance);

        IModelElementType BaseType { get; }

        bool Abstract { get; }

        ICollection<IModelElementType> ExtendingTypes { get; }

        ICollection<IModelElementType> AllExtendingTypes { get; }

        IAttribute GetAttribute(string attributeName);

        IModel Model { get; }

        ICollection<IModelElementInstance> GetInstances(IModelInstance modelInstanceImpl);

        IList<IModelElementType> ChildElementTypes { get; }

        IList<IModelElementType> AllChildElementTypes { get; }
    }
}