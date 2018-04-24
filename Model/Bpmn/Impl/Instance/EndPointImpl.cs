using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class EndPointImpl : RootElementImpl, IEndPoint
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEndPoint>(/*typeof(IEndPoint),*/ BpmnModelConstants.BpmnElementEndPoint)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEndPoint>
        {
            public virtual IEndPoint NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EndPointImpl(instanceContext);
            }
        }

        public EndPointImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
    }

}