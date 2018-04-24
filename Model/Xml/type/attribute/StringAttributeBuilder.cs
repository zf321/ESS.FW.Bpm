

using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;
using System;

namespace ESS.FW.Bpm.Model.Xml.type.attribute
{
    public interface IStringAttributeBuilder : IAttributeBuilder<String>
    {

        new IStringAttributeBuilder Namespace(string namespaceUri);

        new IStringAttributeBuilder DefaultValue(string defaultValue);

        new IStringAttributeBuilder Required();

        new IStringAttributeBuilder IdAttribute();

        IAttributeReferenceBuilder QNameAttributeReference<T>() where T : IModelElementInstance;

        IAttributeReferenceBuilder IdAttributeReference<T>() where T : IModelElementInstance;

        IAttributeReferenceCollectionBuilder<TTarget> IdAttributeReferenceCollection<TTarget, TReference>(AttributeReferenceCollection<TReference> attributeReferenceCollection) where TTarget:IModelElementInstance where TReference : IModelElementInstance;
    }
}