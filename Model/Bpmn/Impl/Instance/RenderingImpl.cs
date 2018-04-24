using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class RenderingImpl : BaseElementImpl, IRendering
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IRendering>(/*typeof(IRendering),*/ BpmnModelConstants.BpmnElementRendering)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IRendering>
        {
            public virtual IRendering NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new RenderingImpl(instanceContext);
            }
        }

        public RenderingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}