using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;



namespace ESS.FW.Bpm.Model.Dmn
{
    public interface IQuery<T> where T : IModelElementInstance
    {
        IList<T> List();

        int Count();

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        IQuery<TV> FilterByType<TV>(IModelElementType elementType) where TV : IModelElementInstance;

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        IQuery<TV> FilterByType<TV>(System.Type elementClass) where TV : IModelElementInstance;

        T SingleResult();
    }
}