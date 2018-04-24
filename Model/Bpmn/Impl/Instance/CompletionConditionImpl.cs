using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class CompletionConditionImpl : ExpressionImpl, ICompletionCondition
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICompletionCondition>(/*typeof(ICompletionCondition),*/ BpmnModelConstants.BpmnElementCompletionCondition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICompletionCondition>
        {
            public virtual ICompletionCondition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CompletionConditionImpl(instanceContext);
            }
        }

        public CompletionConditionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

    }

}