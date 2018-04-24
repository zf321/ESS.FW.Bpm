

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TransactionImpl : SubProcessImpl, ITransaction
    {

        protected internal static IAttribute/*<TransactionMethod>*/ MethodAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITransaction>(/*typeof(ITransaction), */BpmnModelConstants.BpmnElementTransaction)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ISubProcess))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            MethodAttribute = typeBuilder.NamedEnumAttribute<TransactionMethod>(BpmnModelConstants.BpmnAttributeMethod/*, typeof(TransactionMethod)*/)
                .DefaultValue(TransactionMethod.Compensate).Build();
            
            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITransaction>
        {
            public virtual ITransaction NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TransactionImpl(instanceContext);
            }
        }

        public TransactionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual TransactionMethod Method
        {
            get
            {
                return MethodAttribute.GetValue<TransactionMethod>(this);
            }
            set
            {
                MethodAttribute.SetValue(this, value);
            }
        }
    }
}