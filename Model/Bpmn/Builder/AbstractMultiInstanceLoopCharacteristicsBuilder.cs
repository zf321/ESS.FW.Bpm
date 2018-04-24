using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class AbstractMultiInstanceLoopCharacteristicsBuilder :AbstractBaseElementBuilder<IMultiInstanceLoopCharacteristics>, IMultiInstanceLoopCharacteristicsBuilder
    {

	  protected internal AbstractMultiInstanceLoopCharacteristicsBuilder(IBpmnModelInstance modelInstance, IMultiInstanceLoopCharacteristics element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the multi instance loop characteristics to be sequential.
	  /// </summary>
	  /// <returns>  the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder Sequential()
	  {
		element.Sequential = true;
		return this;
	  }

	  /// <summary>
	  /// Sets the multi instance loop characteristics to be parallel.
	  /// </summary>
	  /// <returns>  the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder Parallel()
	  {
		element.Sequential = false;
		return this;
	  }

	  /// <summary>
	  /// Sets the cardinality expression.
	  /// </summary>
	  /// <param name="expression"> the cardinality expression </param>
	  /// <returns> the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder Cardinality(string expression)
	  {
		ILoopCardinality cardinality = GetCreateSingleChild< ILoopCardinality>(typeof(ILoopCardinality));
		cardinality.TextContent = expression;
		return this;
	  }

	  /// <summary>
	  /// Sets the completion condition expression.
	  /// </summary>
	  /// <param name="expression"> the completion condition expression </param>
	  /// <returns> the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder CompletionCondition(string expression)
	  {
		ICompletionCondition condition = GetCreateSingleChild< ICompletionCondition>(typeof(ICompletionCondition));
		condition.TextContent = expression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda collection expression.
	  /// </summary>
	  /// <param name="expression"> the collection expression </param>
	  /// <returns> the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder CamundaCollection(string expression)
	  {
		element.CamundaCollection = expression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda element variable name.
	  /// </summary>
	  /// <param name="variableName"> the name of the element variable </param>
	  /// <returns> the builder object </returns>
	  public virtual IMultiInstanceLoopCharacteristicsBuilder CamundaElementVariable(string variableName)
	  {
		element.CamundaElementVariable = variableName;
		return this;
	  }

        /// <summary>
        /// Finishes the building of a multi instance loop characteristics.
        /// </summary>
        /// <returns> the parent activity builder </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractActivityBuilder> T multiInstanceDone()
        public virtual IActivityBuilder<IActivity> MultiInstanceDone() /*where T : AbstractActivityBuilder*/
        {
            return ((IActivity)element.ParentElement).Builder<IActivityBuilder<IActivity>,IActivity>();
        }

    }

}