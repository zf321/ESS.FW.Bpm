

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TimeDateImpl : ExpressionImpl, ITimeDate
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITimeDate>(/*typeof(ITimeDate),*/ BpmnModelConstants.BpmnElementTimeDate)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITimeDate>
        {
            public virtual ITimeDate NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TimeDateImpl(instanceContext);
            }
        }

        public TimeDateImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}