

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.instance;
using ModelInstanceImpl = ESS.FW.Bpm.Model.Xml.impl.ModelInstanceImpl;

namespace ESS.FW.Bpm.Model.Dmn.impl
{
    using ModelBuilder = ModelBuilder;
    using ModelImpl = ModelImpl;
    using ModelInstanceImpl = ModelInstanceImpl;

    public class DmnModelInstanceImpl : ModelInstanceImpl, IDmnModelInstance
    {
        //TODO Attribute Id，Name等解析
        public DmnModelInstanceImpl(ModelImpl model, ModelBuilder modelBuilder, IDomDocument document) : base(model, modelBuilder, document)
        {
            //var test = Definitions;
        }

        public IDefinitions Definitions
        {
            get
            {
                return this.DocumentElement as IDefinitions;// getDocumentElement();
            }
            set
            {
                this.DocumentElement = value;
            }
        }

        //IDefinitions DmnModelInstance.Definitions
        //{
        //    get
        //    {
        //        throw (IDefinitions)DocumentElement;
        //    }

        //    set
        //    {
        //        DocumentElement = value;
        //    }
        //}

        public virtual IDmnModelInstance Clone()
        {
            return new DmnModelInstanceImpl(model, ModelBuilder, document.Clone());
        }
    }
}