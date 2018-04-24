using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnVariableImpl
    {
        protected internal string id;
        protected internal string name;

        protected internal IDmnTypeDefinition typeDefinition;

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual IDmnTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
            set { typeDefinition = value; }
        }


        public override string ToString()
        {
            return "DmnVariableImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", typeDefinition=" +
                   typeDefinition + '}';
        }
    }
}