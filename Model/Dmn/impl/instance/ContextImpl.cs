using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class ContextImpl : ExpressionImpl, IContext
    {

        protected internal static IChildElementCollection/*<IContextEntry>*/ ContextEntryCollection;

        public ContextImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IContextEntry> ContextEntries => ContextEntryCollection.Get<IContextEntry>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IContext>(/*typeof(IContext),*/ DmnModelConstants.DmnElementContext).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IExpression)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ContextEntryCollection = sequenceBuilder.ElementCollection<IContextEntry>(/*typeof(IContextEntry)*/).Build/*<IContextEntry>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IContext>
        {
            public virtual IContext NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ContextImpl(instanceContext);
            }
        }
    }
}