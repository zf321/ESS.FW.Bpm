using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConditionalEventDefinitionImpl : EventDefinitionImpl, IConditionalEventDefinition
    {

        protected internal static IChildElement/*<ICondition>*/ ConditionChild;
        protected internal static IAttribute/*<string>*/ camundaVariableName;
        protected internal static IAttribute/*<string>*/ camundaVariableEvents;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConditionalEventDefinition>(/*typeof(IConditionalEventDefinition),*/ BpmnModelConstants.BpmnElementConditionalEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ConditionChild = sequenceBuilder.Element<ICondition>(/*typeof(ICondition)*/).Required().Build/*<ICondition>*/();

            camundaVariableName = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariableName).Namespace(BpmnModelConstants.CamundaNs).Build();

            camundaVariableEvents = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariableEvents).Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IConditionalEventDefinition>
        {
            public IConditionalEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConditionalEventDefinitionImpl(instanceContext);
            }
        }

        public ConditionalEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ICondition Condition
        {
            get => (ICondition)ConditionChild.GetChild(this);
            set => ConditionChild.SetChild(this, value);
        }


        public virtual string CamundaVariableName
        {
            get => camundaVariableName.GetValue<String>(this);
            set => camundaVariableName.SetValue(this, value);
        }


        public virtual string CamundaVariableEvents
        {
            get => camundaVariableEvents.GetValue<String>(this);
            set => camundaVariableEvents.SetValue(this, value);
        }


        public virtual IList<string> CamundaVariableEventsList
        {
            get
            {
                string variableEvents = camundaVariableEvents.GetValue<String>(this);
                return StringUtil.SplitCommaSeparatedList(variableEvents);
            }
            set
            {
                string variableEvents = StringUtil.JoinCommaSeparatedList(value);
                camundaVariableEvents.SetValue(this, variableEvents);
            }
        }

    }

}