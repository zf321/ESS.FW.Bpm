using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN resourceParameterBinding element
    /// 
    /// 
    /// </summary>
    public class ResourceParameterBindingImpl : BaseElementImpl, IResourceParameterBinding
    {

        protected internal static IAttributeReference ParameterRefAttribute;//IAttributeReference<IResourceParameter>
        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IResourceParameterBinding>(/*typeof(IResourceParameterBinding),*/ BpmnModelConstants.BpmnElementResourceParameterBinding)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new MyModelTypeInstanceProvider());

            ParameterRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeParameterRef)
                .Required()
                .QNameAttributeReference<IResourceParameter>(/*typeof(IResourceParameter)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Required().Build/*<IExpression>*/();

            typeBuilder.Build();
        }
        public class MyModelTypeInstanceProvider : IModelTypeInstanceProvider<IResourceParameterBinding>
        {
            public IResourceParameterBinding NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceParameterBindingImpl(instanceContext);
            }
        }



        public ResourceParameterBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IResourceParameter Parameter
        {
            get => ParameterRefAttribute.GetReferenceTargetElement<IResourceParameter>(this);
            set => ParameterRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }
    }

}