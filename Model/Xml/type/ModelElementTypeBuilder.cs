using System;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.type
{
    public interface IModelElementTypeBuilder
    {

        IModelElementTypeBuilder NamespaceUri(string namespaceUri);

        IModelElementTypeBuilder ExtendsType(Type extendedType);

        IModelElementTypeBuilder ExtendsType<T>(T extendedType) where T:IModelElementInstance;
        
        IModelElementTypeBuilder InstanceProvider<T>(IModelTypeInstanceProvider<T> instanceProvider) where T : IModelElementInstance;

        IModelElementTypeBuilder AbstractType();

        IAttributeBuilder<Boolean> BooleanAttribute(string attributeName);

        IStringAttributeBuilder StringAttribute(string attributeName);

        IAttributeBuilder<Int32?> IntegerAttribute(string attributeName);

        IAttributeBuilder<Double?> DoubleAttribute(string attributeName);
        
        IAttributeBuilder<T> EnumAttribute<T>(string attributeName) where T:struct;
        
        IAttributeBuilder<T> NamedEnumAttribute<T>(string attributeName) where T:struct;

        ISequenceBuilder Sequence();

        IModelElementType Build();


    }
    

    public interface IModelTypeInstanceProvider<out T> where T : IModelElementInstance
    {
        T NewInstance(ModelTypeInstanceContext instanceContext);
    }
}