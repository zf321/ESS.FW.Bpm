using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///      
    /// </summary>
    public interface IVariableScope
    {
        string VariableScopeKey { get; }

        IDictionary<string, object> Variables { get; set; }

        IVariableMap VariablesTyped { get; }

        IDictionary<string, object> VariablesLocal { get; set; }

        IVariableMap VariablesLocalTyped { get; }

        ISet<string> VariableNames { get; }

        ISet<string> VariableNamesLocal { get; }

        IVariableMap GetVariablesTyped(bool deserializeValues);

        IVariableMap GetVariablesLocalTyped(bool deserializeValues);

        object GetVariable(string variableName);

        object GetVariableLocal(string variableName);
        
        T GetVariableTyped<T>(string variableName);
        
        T GetVariableTyped<T>(string variableName, bool deserializeValue);
        
        T GetVariableLocalTyped<T>(string variableName);
        
        T GetVariableLocalTyped<T>(string variableName, bool deserializeValue);

        void SetVariable(string variableName, object value);

        void SetVariableLocal(string variableName, object value);


        bool HasVariables();

        bool HasVariablesLocal();

        bool HasVariable(string variableName);

        bool HasVariableLocal(string variableName);

        /// <summary>
        ///     Removes the variable and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" />.
        /// </summary>
        void RemoveVariable(string variableName);

        /// <summary>
        ///     Removes the local variable and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" />.
        /// </summary>
        void RemoveVariableLocal(string variableName);

        /// <summary>
        ///     Removes the variables and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" /> for each of them.
        /// </summary>
        void RemoveVariables(ICollection<string> variableNames);

        /// <summary>
        ///     Removes the local variables and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" /> for each of them.
        /// </summary>
        void RemoveVariablesLocal(ICollection<string> variableNames);

        /// <summary>
        ///     Removes the (local) variables and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" /> for each of them.
        /// </summary>
        void RemoveVariables();

        /// <summary>
        ///     Removes the (local) variables and creates a new
        ///     <seealso cref="HistoricVariableUpdateEntity" /> for each of them.
        /// </summary>
        void RemoveVariablesLocal();
    }
}