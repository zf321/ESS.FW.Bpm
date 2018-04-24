using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CorrelationPropertyImpl : RootElementImpl, ICorrelationProperty
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference TypeAttribute; //IAttributeReference<IItemDefinition>
        protected internal static IChildElementCollection/*<ICorrelationPropertyRetrievalExpression>*/ CorrelationPropertyRetrievalExpressionCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICorrelationProperty>(/*typeof(ICorrelationProperty),*/ BpmnModelConstants.BpmnElementCorrelationProperty)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            TypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeType)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CorrelationPropertyRetrievalExpressionCollection = sequenceBuilder.ElementCollection<ICorrelationPropertyRetrievalExpression>(/*typeof(ICorrelationPropertyRetrievalExpression)*/)
                .Required()
                .Build/*<ICorrelationPropertyRetrievalExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICorrelationProperty>
        {
            public virtual ICorrelationProperty NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationPropertyImpl(instanceContext);
            }
        }

        public CorrelationPropertyImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Type
        {
            get => TypeAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => TypeAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual ICollection<ICorrelationPropertyRetrievalExpression> CorrelationPropertyRetrievalExpressions => CorrelationPropertyRetrievalExpressionCollection.Get<ICorrelationPropertyRetrievalExpression>(this);
    }

}