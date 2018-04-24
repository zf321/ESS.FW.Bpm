

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class ImportedValuesImpl : ImportImpl, IMportedValues
    {

        protected internal static IAttribute/*<string>*/ ExpressionLanguageAttribute;

        protected internal static IChildElement/*<IMportedElement>*/ ImportedElementChild;

        public ImportedValuesImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string ExpressionLanguage
        {
            get => ExpressionLanguageAttribute.GetValue<String>(this);
            set => ExpressionLanguageAttribute.SetValue(this, value);
        }


        public virtual IMportedElement ImportedElement
        {
            get => (IMportedElement)ImportedElementChild.GetChild(this);
            set => ImportedElementChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMportedValues>(/*typeof(IMportedValues),*/ DmnModelConstants.DmnElementImportedValues)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns)
                    .ExtendsType(typeof(IMport))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ExpressionLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExpressionLanguage).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ImportedElementChild = sequenceBuilder.Element<IMportedElement>(/*typeof(IMportedElement)*/).Required().Build/*<IMportedElement>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMportedValues>
        {
            public virtual IMportedValues NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ImportedValuesImpl(instanceContext);
            }
        }

    }

}