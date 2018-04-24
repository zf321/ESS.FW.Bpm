using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class GatewayImpl : FlowNodeImpl, IGateway
    {

        protected internal static IAttribute/*<GatewayDirection>*/ GatewayDirectionAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IGateway>(/*typeof(IGateway),*/ BpmnModelConstants.BpmnElementGateway)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IFlowNode))
                    .AbstractType();

            GatewayDirectionAttribute = typeBuilder.EnumAttribute<GatewayDirection>(BpmnModelConstants.BpmnAttributeGatewayDirection/*, typeof(GatewayDirection)*/)
                    .DefaultValue(GatewayDirection.Unspecified)
                    .Build();

            typeBuilder.Build();
        }

        public GatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new virtual AbstractGatewayBuilder<IGateway> Builder()
        {
            throw new BpmnModelException("[" + this.GetType().FullName + "]No builder implemented for type " + ElementType.TypeNamespace + ":" + ElementType.TypeName);
        }

        public virtual GatewayDirection GatewayDirection
        {
            get
            {
                return GatewayDirectionAttribute.GetValue<GatewayDirection>(this);
            }
            set
            {
                GatewayDirectionAttribute.SetValue(this, value);
            }
        }

        public new IBpmnShape DiagramElement
        {
            get
            {
                return (IBpmnShape)base.DiagramElement;
            }
        }

    }

}