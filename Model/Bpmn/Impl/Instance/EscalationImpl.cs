

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN escalation element
    /// 
    /// 
    /// </summary>
    public class EscalationImpl : RootElementImpl, IEscalation
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ EscalationCodeAttribute;
        protected internal static IAttributeReference StructureRefAttribute;//IAttributeReference<IItemDefinition>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEscalation>(/*typeof(IEscalation),*/ BpmnModelConstants.BpmnElementEscalation)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IRootElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeName)
                .Build();

            EscalationCodeAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeEscalationCode)
                .Build();

            StructureRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeStructureRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEscalation>
        {
            public virtual IEscalation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EscalationImpl(instanceContext);
            }
        }

        public EscalationImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual string EscalationCode
        {
            get => EscalationCodeAttribute.GetValue<String>(this);
            set => EscalationCodeAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Structure
        {
            get => /*(IItemDefinition)*/StructureRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => StructureRefAttribute.SetReferenceTargetElement(this, value);
        }


    }

}