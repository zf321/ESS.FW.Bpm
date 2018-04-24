using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using StyleImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.di.StyleImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{
    public class BpmnLabelStyleImpl : StyleImpl, IBpmnLabelStyle
    {

        protected internal static IChildElement/*<IFont>*/ FontChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnLabelStyle>(/*typeof(IBpmnLabelStyle),*/ BpmnModelConstants.BpmndiElementBpmnLabelStyle)
                    .NamespaceUri(BpmnModelConstants.BpmndiNs)
                    .ExtendsType(typeof(IStyle))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            FontChild = sequenceBuilder.Element<IFont>(/*typeof(IFont)*/).Required().Build/*<IFont>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnLabelStyle>
        {
            public virtual IBpmnLabelStyle NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BpmnLabelStyleImpl(instanceContext);
            }
        }

        public BpmnLabelStyleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IFont Font
        {
            get => (IFont)FontChild.GetChild(this);
            set => FontChild.SetChild(this, value);
        }

    }

}