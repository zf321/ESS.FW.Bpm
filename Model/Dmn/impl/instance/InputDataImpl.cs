using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class InputDataImpl : DrgElementImpl, INputData
    {

        protected internal static IChildElement/*<INformationItem>*/ InformationItemChild;

        public InputDataImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual INformationItem InformationItem
        {
            get => (INformationItem)InformationItemChild.GetChild(this);
            set => InformationItemChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INputData>(/*typeof(INputData),*/ DmnModelConstants.DmnElementInputData)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns)
                    .ExtendsType(typeof(IDrgElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InformationItemChild = sequenceBuilder.Element<INformationItem>(/*typeof(INformationItem)*/).Build/*<INformationItem>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INputData>
        {
            public virtual INputData NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InputDataImpl(instanceContext);
            }
        }

    }

}