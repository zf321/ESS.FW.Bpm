using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    ///      
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        ///     Evaluates the condition and returns the result.
        ///     The scope will be the same as the execution.
        /// </summary>
        /// <param name="execution"> the execution which is used to evaluate the condition </param>
        /// <returns> the result </returns>
        bool Evaluate(IDelegateExecution execution);

        /// <summary>
        ///     Evaluates the condition and returns the result.
        /// </summary>
        /// <param name="scope"> the variable scope which can differ of the execution </param>
        /// <param name="execution"> the execution which is used to evaluate the condition </param>
        /// <returns> the result </returns>
        bool Evaluate(IVariableScope scope, IDelegateExecution execution);

        /// <summary>
        ///     Tries to evaluate the condition. If the property which is used in the condition does not exist
        ///     false will be returned.
        /// </summary>
        /// <param name="scope"> the variable scope which can differ of the execution </param>
        /// <param name="execution"> the execution which is used to evaluate the condition </param>
        /// <returns> the result </returns>
        bool TryEvaluate(IVariableScope scope, IDelegateExecution execution);
    }
}