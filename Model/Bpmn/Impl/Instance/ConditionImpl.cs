

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConditionImpl : ExpressionImpl, ICondition
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICondition>(/*typeof(ICondition),*/ BpmnModelConstants.BpmnElementCondition)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IExpression))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICondition>
        {
            public virtual ICondition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConditionImpl(instanceContext);
            }
        }

        public ConditionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}