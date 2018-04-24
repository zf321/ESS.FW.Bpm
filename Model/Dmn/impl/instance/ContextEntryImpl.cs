using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class ContextEntryImpl : DmnModelElementInstanceImpl, IContextEntry
    {

        protected internal static IChildElement/*<IVariable>*/ VariableChild;
        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

        public ContextEntryImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IContextEntry>(/*typeof(IContextEntry),*/ DmnModelConstants.DmnElementContextEntry).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            VariableChild = sequenceBuilder.Element<IVariable>(/*typeof(IVariable)*/).Build/*<IVariable>*/();

            ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Required().Build/*<IExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IContextEntry>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IContextEntry NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ContextEntryImpl(instanceContext);
            }
        }


        public virtual IVariable Variable
        {
            get => (IVariable)VariableChild.GetChild(this);
            set => VariableChild.SetChild(this, value);
        }


        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }

    }
}