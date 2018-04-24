using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using List = ESS.FW.Bpm.Model.Dmn.instance.List;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class ListImpl : ExpressionImpl, List
    {

        protected internal static IChildElementCollection/*<IExpression>*/ ExpressionCollection;

        public ListImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IExpression> Expressions => ExpressionCollection.Get<IExpression>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<List>(/*typeof(List), */DmnModelConstants.DmnElementList)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IExpression))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ExpressionCollection = sequenceBuilder.ElementCollection<IExpression>(/*typeof(IExpression)*/).Build/*<IExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<List>
        {

            public virtual List NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ListImpl(instanceContext);
            }
        }

    }

}