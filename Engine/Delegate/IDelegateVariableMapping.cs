



using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     Represents a delegated mapping of input and output variables.
    ///      
    /// </summary>
    public interface IDelegateVariableMapping
    {
        /// <summary>
        ///     Maps the input variables into the given variables map.
        ///     The variables map will be used by the sub process.
        /// </summary>
        /// <param name="superExecution"> the execution object of the super (outer) process </param>
        /// <param name="subVariables"> the variables map of the sub (inner) process </param>
        void MapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables);

        /// <summary>
        ///     Maps the output variables into the outer process. This means the variables of
        ///     the sub process, which can be accessed via the subInstance, will be
        ///     set as variables into the super process, for example via ${superExecution.setVariables}.
        /// </summary>
        /// <param name="superExecution"> the execution object of the super (outer) process, which gets the output variables </param>
        /// <param name="subInstance"> the instance of the sub process, which contains the variables </param>
        void MapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance);
    }
}

