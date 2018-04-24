using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class RelationImpl : ExpressionImpl, IRelation
    {

        protected internal static IChildElementCollection/*<IColumn>*/ ColumnCollection;
        protected internal static IChildElementCollection/*<IRow>*/ RowCollection;

        public RelationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IColumn> Columns => ColumnCollection.Get<IColumn>(this);

        public virtual ICollection<IRow> Rows => RowCollection.Get<IRow>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IRelation>(/*typeof(IRelation),*/ DmnModelConstants.DmnElementRelation)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ColumnCollection = sequenceBuilder.ElementCollection<IColumn>(/*typeof(IColumn)*/).Build/*<IColumn>*/();

            RowCollection = sequenceBuilder.ElementCollection<IRow>(/*typeof(IRow)*/).Build/*<IRow>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IRelation>
        {
            public virtual IRelation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new RelationImpl(instanceContext);
            }
        }
    }
}