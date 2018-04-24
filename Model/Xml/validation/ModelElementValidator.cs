using System;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Xml.validation
{
    /// <summary>
	/// A validator for model element instances.
	/// </summary>
	/// <seealso cref=" ModelInstance#validate(java.util.Collection)"> </seealso>
	public interface IModelElementValidator<T> where T : IModelElementInstance
	{

        /// <summary>
        /// <para>The type of the element this validator is applied to. The validator is applied to all
        /// instances implementing this type.</para>
        /// 
        /// <para>Example from BPMN: Assume the type returned is 'Task'. Then the validator is invoked for
        /// all instances of task, including instances of 'ServiceTask', 'UserTask', ...</para>
        /// </summary>
        /// <returns> the type of the element this validator is applied to. </returns>
        IModelElementType ElementType {get;}

	  /// <summary>
	  /// Validate an element.
	  /// </summary>
	  /// <param name="element"> the element to validate </param>
	  /// <param name="validationResultCollector"> object used to collect validation results for this element. </param>
	  void Validate(T element, IValidationResultCollector validationResultCollector);
	}

}