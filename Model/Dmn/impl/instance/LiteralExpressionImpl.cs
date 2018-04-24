

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class LiteralExpressionImpl : ExpressionImpl, ILiteralExpression
    {

        protected internal static IAttribute/*<string>*/ ExpressionLanguageAttribute;

        protected internal static IChildElement/*<IText>*/ TextChild;
        protected internal static IChildElement/*<IMportedValues>*/ ImportedValuesChild;

        public LiteralExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string ExpressionLanguage
        {
            get => ExpressionLanguageAttribute.GetValue<String>(this);
            set => ExpressionLanguageAttribute.SetValue(this, value);
        }


        public virtual IText Text
        {
            get => (IText)TextChild.GetChild(this);
            set => TextChild.SetChild(this, value);
        }


        public virtual IMportedValues ImportValues
        {
            get => (IMportedValues)ImportedValuesChild.GetChild(this);
            set => ImportedValuesChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILiteralExpression>(/*typeof(ILiteralExpression),*/ DmnModelConstants.DmnElementLiteralExpression).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IExpression)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ExpressionLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExpressionLanguage).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TextChild = sequenceBuilder.Element<IText>(/*typeof(IText)*/).Build/*<IText>*/();

            ImportedValuesChild = sequenceBuilder.Element<IMportedValues>(/*typeof(IMportedValues)*/).Build/*<IMportedValues>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ILiteralExpression>
        {
            public virtual ILiteralExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LiteralExpressionImpl(instanceContext);
            }
        }

    }

}