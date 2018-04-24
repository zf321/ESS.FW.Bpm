

using ESS.FW.Bpm.Model.Xml.instance;
using System.Xml;

namespace ESS.FW.Bpm.Model.Xml.impl.util
{
    /// <summary>
    /// <para>Thrown when a Model XmlElement is added to the wrong document</para>
    /// 
    /// 
    /// 
    /// </summary>
    public class WrongDocumentException : ModelException
    {

        private const long SerialVersionUid = 1L;

        public WrongDocumentException(XmlNode nodeToAdd, IDomDocument targetDocument) : base("Cannot add attribute '" + nodeToAdd + "' to document '" + targetDocument + "' not created by document.")
        {
        }
    }
}