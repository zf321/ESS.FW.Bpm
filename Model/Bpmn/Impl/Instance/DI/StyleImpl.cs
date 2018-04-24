using System;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class StyleImpl : BpmnModelElementInstanceImpl, IStyle
    {

        protected internal static IAttribute/*<string>*/ IdAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IStyle>(/*typeof(IStyle),*/ BpmnModelConstants.DiElementStyle)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .AbstractType();

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DiAttributeId).IdAttribute().Build();

            typeBuilder.Build();
        }

        public StyleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public override string Id
        {
            get { return IdAttribute.GetValue<String>(this); }
            set { IdAttribute.SetValue(this, value); }
        }

    }
}