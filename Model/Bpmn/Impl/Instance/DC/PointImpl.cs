

using System;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.dc
{
    public class PointImpl : BpmnModelElementInstanceImpl, IPoint
    {

        protected internal static IAttribute/*<double?>*/ XAttribute;
        protected internal static IAttribute/*<double?>*/ YAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IPoint>(/*typeof(IPoint),*/ BpmnModelConstants.DcElementPoint)
                .NamespaceUri(BpmnModelConstants.DcNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            XAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeX).Required().Build();

            YAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeY).Required().Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IPoint>
        {
            public virtual IPoint NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new PointImpl(instanceContext);
            }
        }

        public PointImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public double X
        {
            get { return XAttribute.GetValue<Double?>(this).GetValueOrDefault(); }
            set { XAttribute.SetValue(this, value); }
        }
        public double Y
        {
            get { return YAttribute.GetValue<Double?>(this).GetValueOrDefault(); }
            set { YAttribute.SetValue(this, value); }
        }
    }
}