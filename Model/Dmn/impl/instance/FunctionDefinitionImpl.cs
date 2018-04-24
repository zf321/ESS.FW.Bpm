using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class FunctionDefinitionImpl : ExpressionImpl, IFunctionDefinition
    {

        protected internal static IChildElementCollection/*<IFormalParameter>*/ FormalParameterCollection;
        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

        public FunctionDefinitionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IFormalParameter> FormalParameters => FormalParameterCollection.Get<IFormalParameter>(this);

        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFunctionDefinition>(/*typeof(IFunctionDefinition),*/ DmnModelConstants.DmnElementFunctionDefinition)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            FormalParameterCollection = sequenceBuilder.ElementCollection<IFormalParameter>(/*typeof(IFormalParameter)*/).Build/*<IFormalParameter>*/();

            ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Build/*<IExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IFunctionDefinition>
        {
            public virtual IFunctionDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new FunctionDefinitionImpl(instanceContext);
            }
        }

    }

}