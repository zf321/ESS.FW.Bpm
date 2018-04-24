using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Xml.type.child
{
    public interface IChildElementCollection
    {

        bool Immutable { get; }

        int MinOccurs { get; }

        int MaxOccurs { get; }

        IModelElementType GetChildElementType(IModel model);

        T GetChildElementTypeClass<T>(T typeClass) where T : IModelElementInstance;

        IModelElementType ParentElementType { get; }

        ICollection<T> Get<T>(IModelElementInstance element) where T : IModelElementInstance;

    }

}