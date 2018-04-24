using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.type.reference
{
    public interface IAttributeReferenceCollectionBuilder<T>: IAttributeReferenceBuilder where T:IModelElementInstance
    {
        new AttributeReferenceCollection<T> Build();
    }
}