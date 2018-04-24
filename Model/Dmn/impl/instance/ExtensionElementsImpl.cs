using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_EXTENSION_ELEMENTS;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;
    using ModelUtil = ModelUtil;

    /// <summary>
    /// The DMN extensionElements element
    /// </summary>
    public class ExtensionElementsImpl : DmnModelElementInstanceImpl, IExtensionElements
    {

        public static void RegisterType(ModelBuilder modelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExtensionElements>(/*typeof(IExtensionElements),*/ DmnModelConstants.DmnElementExtensionElements).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExtensionElements>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IExtensionElements NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ExtensionElementsImpl(instanceContext);
            }
        }

        public ExtensionElementsImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ICollection<IModelElementInstance> Elements
        {
            get
            {
                return ModelUtil.GetModelElementCollection<IModelElementInstance>(DomElement.ChildElements, modelInstance);
            }
        }

        public virtual IQuery<IModelElementInstance> ElementsQuery
        {
            get
            {
                return new QueryImpl<IModelElementInstance>(Elements);
            }
        }

        public virtual IModelElementInstance AddExtensionElement(string namespaceUri, string localName)
        {
            IModelElementType extensionElementType = ((ModelInstanceImpl)modelInstance).RegisterGenericType(namespaceUri, localName);
            IModelElementInstance extensionElement = extensionElementType.NewInstance(modelInstance);
            AddChildElement(extensionElement);
            return extensionElement;
        }


        public override void AddChildElement(IModelElementInstance extensionElement)
        {
            DomElement.AppendChild(extensionElement.DomElement);
        }

        public T AddExtensionElement<T>(IType extensionElementClass) where T : IModelElementInstance
        {
            IModelElementInstance extensionElement = modelInstance.NewInstance<IModelElementInstance>((IModelElementType)extensionElementClass);
            AddChildElement(extensionElement);
            return (T)extensionElement;
        }
    }
}