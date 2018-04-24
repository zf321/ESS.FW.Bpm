

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class UnaryTestsImpl : DmnElementImpl, IUnaryTests
    {

        protected internal static IAttribute/*<string>*/ ExpressionLanguageAttribute;

        protected internal static IChildElement/*<IText>*/ TextChild;

        public UnaryTestsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IUnaryTests>(/*typeof(IUnaryTests),*/ DmnModelConstants.DmnElementUnaryTests)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns)
                    .ExtendsType(typeof(IDmnElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ExpressionLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExpressionLanguage).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TextChild = sequenceBuilder.Element<IText>(/*typeof(IText)*/).Build/*<IText>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IUnaryTests>
        {
            public virtual IUnaryTests NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new UnaryTestsImpl(instanceContext);
            }
        }

    }

}