

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TimeCycleImpl : ExpressionImpl, ITimeCycle
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITimeCycle>(/*typeof(ITimeCycle), */BpmnModelConstants.BpmnElementTimeCycle)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITimeCycle>
        {
            public virtual ITimeCycle NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TimeCycleImpl(instanceContext);
            }
        }

        public TimeCycleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}