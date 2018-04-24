using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.type.child
{
    
	public interface IChildElement : IChildElementCollection
	{

	    /// <summary>
	    /// Sets the child element, potentially replacing an existing child element.
	    /// </summary>
	    /// <param name="element"> the parent element of the child element </param>
	    /// <param name="newChildElement"> the new child element to set </param>
	    void SetChild(IModelElementInstance element, IModelElementInstance newChildElement);

	    /// <summary>
	    /// Returns the child element.
	    /// </summary>
	    /// <param name="element"> the parent element of the child element </param>
	    /// <returns> the child element of the parent, or null if not exist </returns>
	    IModelElementInstance GetChild(IModelElementInstance element);

        /// <summary>
        /// Removes the child element.
        /// </summary>
        /// <param name="element">  the parent element of the child element </param>
        /// <returns> true if the child was remove otherwise false </returns>
        bool RemoveChild(IModelElementInstance element);
    }

}