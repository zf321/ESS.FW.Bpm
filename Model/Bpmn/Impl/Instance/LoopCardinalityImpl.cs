

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class LoopCardinalityImpl : ExpressionImpl, ILoopCardinality
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILoopCardinality>(/*typeof(ILoopCardinality),*/ BpmnModelConstants.BpmnElementLoopCardinality)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ILoopCardinality>
        {
            public virtual ILoopCardinality NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LoopCardinalityImpl(instanceContext);
            }
        }

        public LoopCardinalityImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}