using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class DecisionRuleImpl : DmnElementImpl, IDecisionRule
    {

        protected internal static IChildElementCollection/*<IInputEntry>*/ InputEntryCollection;
        protected internal static IChildElementCollection/*<IOutputEntry>*/ OutputEntryCollection;

        public DecisionRuleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IInputEntry> InputEntries => InputEntryCollection.Get<IInputEntry>(this);

        public virtual ICollection<IOutputEntry> OutputEntries => OutputEntryCollection.Get<IOutputEntry>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDecisionRule>(/*typeof(IDecisionRule),*/ DmnModelConstants.DmnElementDecisionRule).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDmnElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InputEntryCollection = sequenceBuilder.ElementCollection<IInputEntry>(/*typeof(IInputEntry)*/).Build/*<IInputEntry>*/();

            OutputEntryCollection = sequenceBuilder.ElementCollection<IOutputEntry>(/*typeof(IOutputEntry)*/).Required().Build/*<IOutputEntry>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDecisionRule>
        {
            public virtual IDecisionRule NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DecisionRuleImpl(instanceContext);
            }
        }
    }
}