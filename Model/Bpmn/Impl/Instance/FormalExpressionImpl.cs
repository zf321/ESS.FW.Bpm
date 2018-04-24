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
    public class FormalExpressionImpl : ExpressionImpl, IFormalExpression
    {

        protected internal static IAttribute/*<string>*/ LanguageAttribute;
        protected internal static IAttributeReference EvaluatesToTypeRefAttribute;//IAttributeReference<IItemDefinition>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFormalExpression>(/*typeof(IFormalExpression),*/ BpmnModelConstants.BpmnElementFormalExpression)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            LanguageAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeLanguage)
                .Build();

            EvaluatesToTypeRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeEvaluatesToTypeRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IFormalExpression>
        {
            public virtual IFormalExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new FormalExpressionImpl(instanceContext);
            }
        }

        public FormalExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Language
        {
            get => LanguageAttribute.GetValue<String>(this);
            set => LanguageAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition EvaluatesToType
        {
            get => EvaluatesToTypeRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => EvaluatesToTypeRefAttribute.SetReferenceTargetElement(this, value);
        }

    }

}