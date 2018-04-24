

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class OutputClauseImpl : DmnElementImpl, IOutputClause
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ TypeRefAttribute;

        protected internal static IChildElement/*<IOutputValues>*/ OutputValuesChild;
        protected internal static IChildElement/*<IDefaultOutputEntry>*/ DefaultOutputEntryChild;

        public OutputClauseImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual string TypeRef
        {
            get => TypeRefAttribute.GetValue<String>(this);
            set => TypeRefAttribute.SetValue(this, value);
        }


        public virtual IOutputValues OutputValues
        {
            get => (IOutputValues)OutputValuesChild.GetChild(this);
            set => OutputValuesChild.SetChild(this, value);
        }


        public virtual IDefaultOutputEntry DefaultOutputEntry
        {
            get => (IDefaultOutputEntry)DefaultOutputEntryChild.GetChild(this);
            set => DefaultOutputEntryChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOutputClause>(/*typeof(IOutputClause),*/ DmnModelConstants.DmnElementOutputClause)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IDmnElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeName).Build();

            TypeRefAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTypeRef).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            OutputValuesChild = sequenceBuilder.Element<IOutputValues>(/*typeof(IOutputValues)*/).Build/*<IOutputValues>*/();

            DefaultOutputEntryChild = sequenceBuilder.Element<IDefaultOutputEntry>(/*typeof(IDefaultOutputEntry)*/).Build/*<IDefaultOutputEntry>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOutputClause>
        {
            public virtual IOutputClause NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutputClauseImpl(instanceContext);
            }
        }

    }

}