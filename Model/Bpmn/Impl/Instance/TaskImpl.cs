using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN task element
    /// 
    /// 
    /// </summary>
    public class TaskImpl : ActivityImpl, ITask
    {

        /// <summary>
        /// camunda extensions </summary>

        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITask>(/*typeof(ITask),*/ BpmnModelConstants.BpmnElementTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IActivity))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());
            
            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITask>
        {
            public virtual ITask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TaskImpl(instanceContext);
            }
        }

        public TaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public override TB Builder<TB, TE>()
        {
            throw new ModelTypeException("No builder implemented.");
        }

        public new virtual AbstractTaskBuilder<ITask> Builder()
        {
            throw new ModelTypeException("No builder implemented.");
        }


        /// <summary>
        /// camunda extensions </summary>

        /// @deprecated use isCamundaAsyncBefore() instead. 
        [Obsolete("use isCamundaAsyncBefore() instead.")]
        public virtual bool CamundaAsync
        {
            get { return CamundaAsyncAttribute.GetValue<Boolean>(this); }
            set { CamundaAsyncAttribute.SetValue(this, value); }
        }

        //IBpmnShape ITask.DiagramElement { get; }


        public new IBpmnShape DiagramElement
        {
            get { return (IBpmnShape) base.DiagramElement; }
        }

    }

}