using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Xml.type.reference
{
    public interface IAttributeReference : IReference
    {
        IAttribute ReferenceSourceAttribute { get; }
    }
}