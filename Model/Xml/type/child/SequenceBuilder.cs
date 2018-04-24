using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.type.child
{
    public interface ISequenceBuilder
    {
        IChildElementBuilder Element<T>() where T : IModelElementInstance;

        IChildElementCollectionBuilder ElementCollection<T>() where T : IModelElementInstance;
    }
}