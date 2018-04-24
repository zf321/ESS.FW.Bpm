using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ExtensionElementsImpl : BpmnModelElementInstanceImpl, IExtensionElements
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExtensionElements>(/*typeof(IExtensionElements),*/ BpmnModelConstants.BpmnElementExtensionElements)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExtensionElements>
        {
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

        public virtual T AddExtensionElement<T>(Type extensionElementClass) where T : IModelElementInstance
        {
            IModelElementInstance extensionElement = modelInstance.NewInstance<T>(extensionElementClass);
            AddChildElement(extensionElement);
            return (T)extensionElement;
        }

        public override void AddChildElement(IModelElementInstance extensionElement)
        {
            DomElement.AppendChild(extensionElement.DomElement);
        }

    }

}