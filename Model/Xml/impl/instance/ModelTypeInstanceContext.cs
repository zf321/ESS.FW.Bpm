

using ESS.FW.Bpm.Model.Xml.impl.type;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.impl.instance
{
    

    /// <summary>
	/// 
	/// 
	/// 
	/// </summary>
	public sealed class ModelTypeInstanceContext
	{

	  private readonly IModelInstance _model;
	  private readonly IDomElement _domElement;
	  private readonly ModelElementTypeImpl _modelType;

	  public ModelTypeInstanceContext(IDomElement domElement, IModelInstance model, ModelElementTypeImpl modelType)
	  {
		this._domElement = domElement;
		this._model = model;
		this._modelType = modelType;
	  }

	  /// <returns> the dom element </returns>
	  public IDomElement DomElement
	  {
		  get
		  {
			return _domElement;
		  }
	  }

	  /// <returns> the model </returns>
	  public IModelInstance Model
	  {
		  get
		  {
			return _model;
		  }
	  }

	  /// <returns> the modelType </returns>
	  public ModelElementTypeImpl ModelType
	  {
		  get
		  {
			return _modelType;
		  }
	  }

	}

}