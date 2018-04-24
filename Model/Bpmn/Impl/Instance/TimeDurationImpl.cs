using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TimeDurationImpl : ExpressionImpl, ITimeDuration
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITimeDuration>(/*typeof(ITimeDuration), */BpmnModelConstants.BpmnElementTimeDuration)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITimeDuration>
        {
            public virtual ITimeDuration NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TimeDurationImpl(instanceContext);
            }
        }

        public TimeDurationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

    }

}