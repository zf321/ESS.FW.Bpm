using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ScriptTaskImpl : TaskImpl, IScriptTask
    {

        protected internal static IAttribute/*<string>*/ ScriptFormatAttribute;
        protected internal static IChildElement/*<IScript>*/ ScriptChild;        

        protected internal static IAttribute/*<string>*/ CamundaResultVariableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaResourceAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IScriptTask>(/*typeof(IScriptTask),*/ BpmnModelConstants.BpmnElementScriptTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ITask))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ScriptFormatAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeScriptFormat).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ScriptChild = sequenceBuilder.Element<IScript>(/*typeof(IScript)*/).Build/*<IScript>*/();

            CamundaResultVariableAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeResultVariable).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaResourceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeResource).Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IScriptTask>
        {
            public virtual IScriptTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ScriptTaskImpl(instanceContext);
            }
        }

        public ScriptTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ScriptTaskBuilder Builder()
        {
            return new ScriptTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual string ScriptFormat
        {
            get => ScriptFormatAttribute.GetValue<String>(this);
            set => ScriptFormatAttribute.SetValue(this, value);
        }


        public virtual IScript Script
        {
            get => (IScript)ScriptChild.GetChild(this);
            set => ScriptChild.SetChild(this, value);
        }


        /// <summary>
        /// camunda extensions </summary>

        public virtual string CamundaResultVariable
        {
            get => CamundaResultVariableAttribute.GetValue<String>(this);
            set => CamundaResultVariableAttribute.SetValue(this, value);
        }


        public virtual string CamundaResource
        {
            get => CamundaResourceAttribute.GetValue<String>(this);
            set => CamundaResourceAttribute.SetValue(this, value);
        }
    }
}