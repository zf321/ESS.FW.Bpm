

namespace ESS.FW.Bpm.Model.Bpmn.instance.dc
{

	/// <summary>
	/// The DC bounds element
	/// 
	/// 
	/// </summary>
	public interface IBounds : IBpmnModelElementInstance
	{

	  double? GetX();

	  void SetX(double x);

	  double? GetY();

	  void SetY(double y);

	  double? GetWidth();

	  void SetWidth(double width);

	  double? GetHeight();

	  void SetHeight(double height);

	}

}