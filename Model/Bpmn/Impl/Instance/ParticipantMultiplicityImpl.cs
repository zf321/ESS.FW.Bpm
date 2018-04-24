

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ParticipantMultiplicityImpl : BaseElementImpl, IParticipantMultiplicity
    {

        protected internal static IAttribute/*<int?>*/ MinimumAttribute;
        protected internal static IAttribute/*<int?>*/ MaximumAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IParticipantMultiplicity>(/*typeof(IParticipantMultiplicity),*/ BpmnModelConstants.BpmnElementParticipantMultiplicity)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            MinimumAttribute = typeBuilder.IntegerAttribute(BpmnModelConstants.BpmnAttributeMinimum).DefaultValue(0).Build();

            MaximumAttribute = typeBuilder.IntegerAttribute(BpmnModelConstants.BpmnAttributeMaximum).DefaultValue(1).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IParticipantMultiplicity>
        {
            public virtual IParticipantMultiplicity NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ParticipantMultiplicityImpl(instanceContext);
            }
        }

        public ParticipantMultiplicityImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual int Minimum
        {
            get { return MinimumAttribute.GetValue<Int32?>(this).GetValueOrDefault(); }
            set { MinimumAttribute.SetValue(this, value); }
        }


        public virtual int Maximum
        {
            get { return MaximumAttribute.GetValue<Int32?>(this).GetValueOrDefault(); }
            set { MaximumAttribute.SetValue(this, value); }
        }
    }
}