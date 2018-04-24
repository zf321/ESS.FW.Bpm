
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.type.attribute
{
    public interface IAttributeBuilder<T>
    {
        IAttributeBuilder<T> Namespace(string namespaceUri);

        IAttributeBuilder<T> DefaultValue(T defaultValue);

        IAttributeBuilder<T> Required();

        IAttributeBuilder<T> IdAttribute();

        IAttribute Build();
    }
}