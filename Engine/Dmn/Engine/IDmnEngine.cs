using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     A DMN engine which can parse DMN decision models
    ///     and evaluate decisions.
    ///     <para>
    ///         A new DMN engine can be build with a DMN engine configuration
    ///         (see <seealso cref="DmnEngineConfiguration#buildEngine()" />).
    ///     </para>
    /// </summary>
    public interface IDmnEngine
    {
        /// <summary>
        ///     The configuration of this engine.
        /// </summary>
        /// <returns> the DMN engine configuration </returns>
        DmnEngineConfiguration Configuration { get; }

        /// <summary>
        ///     Parse all decisions in a DMN decision model.
        /// </summary>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <returns>
        ///     a list of the <seealso cref="IDmnDecision" />s of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IList<IDmnDecision> ParseDecisions(Stream inputStream);

        /// <summary>
        ///     Parse all decisions in a DMN decision model.
        /// </summary>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <returns>
        ///     a list of the <seealso cref="IDmnDecision" />s of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IList<IDmnDecision> ParseDecisions(IDmnModelInstance dmnModelInstance);

        /// <summary>
        ///     Parse the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision
        ///     in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to parse </param>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <returns>
        ///     the first <seealso cref="IDmnDecision" /> of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IDmnDecision ParseDecision(string decisionKey, Stream inputStream);

        /// <summary>
        ///     Parse the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision
        ///     in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to parse </param>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <returns>
        ///     the first <seealso cref="IDmnDecision" /> of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IDmnDecision ParseDecision(string decisionKey, IDmnModelInstance dmnModelInstance);

        /// <summary>
        ///     Parse the decision requirements graph in a DMN decision model.
        /// </summary>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <returns>
        ///     a list of the <seealso cref="IDmnDecision" />s of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IDmnDecisionRequirementsGraph ParseDecisionRequirementsGraph(Stream inputStream);

        /// <summary>
        ///     Parse the decision requirements graph in a DMN decision model.
        /// </summary>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <returns>
        ///     a list of the <seealso cref="IDmnDecision" />s of the DMN file
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        IDmnDecisionRequirementsGraph ParseDecisionRequirementsGraph(IDmnModelInstance dmnModelInstance);

        /// <summary>
        ///     Evaluates a decision which is implemented as decision table.
        /// </summary>
        /// <param name="decision"> the <seealso cref="IDmnDecision" /> to evaluate </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table (see <seealso cref="IDmnDecision#isDecisionTable()" />
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= DmnEngine# evaluateDecision( DmnDecision, Map
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(IDmnDecision decision, IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates a decision which is implemented as decision table.
        /// </summary>
        /// <param name="decision"> the <seealso cref="IDmnDecision" /> to evaluate </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table (see <seealso cref="IDmnDecision#isDecisionTable()" />
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= # evaluateDecision( DmnDecision, VariableContext
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(IDmnDecision decision, IVariableContext variableContext);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= # evaluateDecision( String, InputStream, Map
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, Stream inputStream,
            IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= # evaluateDecision( String, InputStream, VariableContext
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, Stream inputStream,
            IVariableContext variableContext);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= # evaluateDecision( String, DmnModelInstance, Map
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, IDmnModelInstance dmnModelInstance,
            IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the decision table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionTableResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision is not implemented as decision table
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        /// <seealso cref= # evaluateDecision( String, DmnModelInstance, VariableContext
        /// )
        /// </seealso>
        IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, IDmnModelInstance dmnModelInstance,
            IVariableContext variableContext);

        /// <summary>
        ///     Evaluates a decision. The decision can be implemented as any kind of supported decision logic (e.g., decision
        ///     table, literal expression).
        /// </summary>
        /// <param name="decision"> the <seealso cref="IDmnDecision" /> to evaluate </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(IDmnDecision decision, IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates a decision. The decision can be implemented as any kind of supported decision logic (e.g., decision
        ///     table, literal expression).
        /// </summary>
        /// <param name="decision"> the <seealso cref="IDmnDecision" /> to evaluate </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the table
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(IDmnDecision decision, IVariableContext variableContext);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        ///     The decision can be implemented as any kind of supported decision logic (e.g., decision table, literal expression).
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(string decisionKey, Stream inputStream, IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        ///     The decision can be implemented as any kind of supported decision logic (e.g., decision table, literal expression).
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="inputStream"> the <seealso cref="InputStream" /> of the DMN file </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the decision
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(string decisionKey, Stream inputStream, IVariableContext variableContext);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        ///     The decision can be implemented as any kind of supported decision logic (e.g., decision table, literal expression).
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <param name="variables">
        ///     the variables which are available during the evaluation
        ///     of expressions in the decision
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(string decisionKey, IDmnModelInstance dmnModelInstance,
            IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in a DMN decision model.
        ///     The key is the {@code id} attribute of the decision in the DMN XML file.
        ///     The decision can be implemented as any kind of supported decision logic (e.g., decision table, literal expression).
        /// </summary>
        /// <param name="decisionKey"> the key of the decision to evaluated </param>
        /// <param name="dmnModelInstance"> the <seealso cref="DmnModelInstance" /> of the DMN decision model </param>
        /// <param name="variableContext">
        ///     the variables context which is available during the evaluation
        ///     of expressions in the decision
        /// </param>
        /// <returns>
        ///     the <seealso cref="IDmnDecisionResult" /> of this evaluation
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the parsing of the decision model
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if no decision is found with the given key
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if the decision logic is not supported
        /// </exception>
        /// <exception cref="DmnEngineException">
        ///     if an error occurs during the evaluation
        /// </exception>
        IDmnDecisionResult EvaluateDecision(string decisionKey, IDmnModelInstance dmnModelInstance,
            IVariableContext variableContext);
    }
}