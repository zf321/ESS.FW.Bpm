using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ExtensionImpl : BpmnModelElementInstanceImpl, IExtension
    {

        protected internal static IAttribute/*<string>*/ DefinitionAttribute;
        protected internal static IAttribute/*<bool>*/ MustUnderstandAttribute;
        protected internal static IChildElementCollection/*<IDocumentation>*/ DocumentationCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExtension>(/*typeof(IExtension),*/ BpmnModelConstants.BpmnElementExtension)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());
            
            DefinitionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeDefinition).Build();

            MustUnderstandAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeMustUnderstand).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DocumentationCollection = sequenceBuilder.ElementCollection<IDocumentation>(/*typeof(IDocumentation)*/).Build/*<IDocumentation>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExtension>
        {
            public virtual IExtension NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ExtensionImpl(instanceContext);
            }
        }

        public ExtensionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Definition
        {
            get => DefinitionAttribute.GetValue<String>(this);
            set => DefinitionAttribute.SetValue(this, value);
        }

        public virtual bool MustUnderstand
        {
            get => MustUnderstandAttribute.GetValue<Boolean>(this);
            set => MustUnderstandAttribute.SetValue(this, value);
        }

        public virtual ICollection<IDocumentation> Documentations => DocumentationCollection.Get<IDocumentation>(this);
    }

}