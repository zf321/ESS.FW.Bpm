using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class InvocationImpl : ExpressionImpl, INvocation
    {

        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;
        protected internal static IChildElementCollection/*<IBinding>*/ BindingCollection;

        public InvocationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }


        public virtual ICollection<IBinding> Bindings => BindingCollection.Get<IBinding>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INvocation>(/*typeof(INvocation),*/ DmnModelConstants.DmnElementInvocation)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Build/*<IExpression>*/();

            BindingCollection = sequenceBuilder.ElementCollection<IBinding>(/*typeof(IBinding)*/).Build/*<IBinding>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INvocation>
        {
            public virtual INvocation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InvocationImpl(instanceContext);
            }
        }
    }
}