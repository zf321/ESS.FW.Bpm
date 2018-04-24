using System;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.dc
{
    public class BoundsImpl : BpmnModelElementInstanceImpl, IBounds
    {

        protected internal static IAttribute/*<double?>*/ XAttribute;
        protected internal static IAttribute/*<double?>*/ YAttribute;
        protected internal static IAttribute/*<double?>*/ WidthAttribute;
        protected internal static IAttribute/*<double?>*/ HeightAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBounds>(/*typeof(IBounds),*/ BpmnModelConstants.DcElementBounds)
                .NamespaceUri(BpmnModelConstants.DcNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            XAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeX).Required().Build();

            YAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeY).Required().Build();

            WidthAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeWidth).Required().Build();

            HeightAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeHeight).Required().Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBounds>
        {
            public virtual IBounds NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BoundsImpl(instanceContext);
            }
        }

        public BoundsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual double? GetX()
        {
            return XAttribute.GetValue<Double>(this);
        }

        public virtual void SetX(double x)
        {
            XAttribute.SetValue(this, x);
        }

        public virtual double? GetY()
        {
            return YAttribute.GetValue<Double>(this);
        }

        public virtual void SetY(double y)
        {
            YAttribute.SetValue(this, y);
        }

        public virtual double? GetWidth()
        {
            return WidthAttribute.GetValue<Double>(this);
        }

        public virtual void SetWidth(double width)
        {
            WidthAttribute.SetValue(this, width);
        }

        public virtual double? GetHeight()
        {
            return HeightAttribute.GetValue<Double>(this);
        }

        public virtual void SetHeight(double height)
        {
            HeightAttribute.SetValue(this, height);
        }
    }

}