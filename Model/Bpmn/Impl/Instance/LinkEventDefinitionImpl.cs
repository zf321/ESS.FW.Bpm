using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class LinkEventDefinitionImpl : EventDefinitionImpl, ILinkEventDefinition
    {
        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection SourceCollection;//IElementReferenceCollection<ILinkEventDefinition, Source>
        protected internal static IElementReference TargetChild;//IElementReference<ILinkEventDefinition, Target>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILinkEventDefinition>(/*typeof(ILinkEventDefinition),*/ BpmnModelConstants.BpmnElementLinkEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Required().Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            SourceCollection = sequenceBuilder
                .ElementCollection<Source>(/*typeof(Source)*/)
                .QNameElementReferenceCollection<ILinkEventDefinition>(/*typeof(ILinkEventDefinition)*/)
                .Build();

            TargetChild = sequenceBuilder.Element<Target>(/*typeof(Target)*/)
                .QNameElementReference<ILinkEventDefinition>(/*typeof(ILinkEventDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ILinkEventDefinition>
        {

            public virtual ILinkEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LinkEventDefinitionImpl(instanceContext);
            }
        }

        public LinkEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get { return NameAttribute.GetValue<String>(this); }
            set { NameAttribute.SetValue(this, value); }
        }


        public virtual ICollection<ILinkEventDefinition> Sources => SourceCollection.GetReferenceTargetElements<ILinkEventDefinition>(this);

        public virtual ILinkEventDefinition Target
        {
            get => (ILinkEventDefinition)TargetChild.GetReferenceTargetElement(this);
            set => TargetChild.SetReferenceTargetElement(this, value);
        }
    }
}