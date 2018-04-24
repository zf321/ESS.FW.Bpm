

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class ImportImpl : DmnModelElementInstanceImpl, IMport
    {

        protected internal static IAttribute/*<string>*/ NamespaceAttribute;
        protected internal static IAttribute/*<string>*/ LocationUriAttribute;
        protected internal static IAttribute/*<string>*/ ImportTypeAttribute;

        public ImportImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Namespace
        {
            get
            {
                return NamespaceAttribute.GetValue<String>(this);
            }
            set
            {
                NamespaceAttribute.SetValue(this, value);
            }
        }


        public virtual string LocationUri
        {
            get
            {
                return LocationUriAttribute.GetValue<String>(this);
            }
            set
            {
                LocationUriAttribute.SetValue(this, value);
            }
        }


        public virtual string ImportType
        {
            get
            {
                return ImportTypeAttribute.GetValue<String>(this);
            }
            set
            {
                ImportTypeAttribute.SetValue(this, value);
            }
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMport>(/*typeof(IMport), */DmnModelConstants.DmnElementImport).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NamespaceAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeNamespace).Required().Build();

            LocationUriAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeLocationUri).Build();

            ImportTypeAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeImportType).Required().Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMport>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IMport NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ImportImpl(instanceContext);
            }
        }

    }

}