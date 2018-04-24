using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.engine.spi;

//www.apache.org/licenses/LICENSE-2.0

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     The configuration of a <seealso cref="IDmnEngine" />. It can be used
    ///     to build a new engine using <seealso cref="#buildEngine()" />.
    ///     <para>
    ///         To create a new default DMN engine configuration the
    ///         method <seealso cref="#createDefaultDmnEngineConfiguration()" />
    ///         can be used.
    ///     </para>
    ///     <para>
    ///         Please be aware that changes to the configuration can also
    ///         influence the behavior of engines which were already created
    ///         by this configuration instance.
    ///     </para>
    /// </summary>
    public abstract class DmnEngineConfiguration
    {
        /// <returns> the configured engine metric collector </returns>
        public abstract IDmnEngineMetricCollector EngineMetricCollector { get; set; }

        /// <returns> the list of custom pre decision table evaluation listeners </returns>
        public abstract IList<IDmnDecisionTableEvaluationListener> CustomPreDecisionTableEvaluationListeners { get; set;
        }

        /// <returns> the list of custom post decision table evaluation listeners </returns>
        public abstract IList<IDmnDecisionTableEvaluationListener> CustomPostDecisionTableEvaluationListeners { get; set;
        }

        /// <returns> the list of custom pre decision evaluation listeners </returns>
        public abstract IList<IDmnDecisionEvaluationListener> CustomPreDecisionEvaluationListeners { get; set; }

        /// <returns> the list of custom post decision evaluation listeners </returns>
        public abstract IList<IDmnDecisionEvaluationListener> CustomPostDecisionEvaluationListeners { get; set; }

        /// <returns> a new default dmn engine configuration </returns>
        public static DmnEngineConfiguration createDefaultDmnEngineConfiguration()
        {
            return new DefaultDmnEngineConfiguration();
        }


        /// <summary>
        ///     Set the engine metric collector
        /// </summary>
        /// <param name="engineMetricCollector"> the engine metric collector to use </param>
        /// <returns> this configuration </returns>
        public abstract DmnEngineConfiguration engineMetricCollector(IDmnEngineMetricCollector engineMetricCollector);


        /// <summary>
        ///     Set the list of pre decision table evaluation listeners. They will be notified before
        ///     the default decision table evaluation listeners.
        /// </summary>
        /// <param name="decisionTableEvaluationListeners"> the list of pre decision table evaluation listeners </param>
        /// <returns> this configuration </returns>
        public abstract DmnEngineConfiguration customPreDecisionTableEvaluationListeners(
            IList<IDmnDecisionTableEvaluationListener> decisionTableEvaluationListeners);


        /// <summary>
        ///     Set the list of post decision table evaluation listeners. They will be notified after
        ///     the default decision table evaluation listeners.
        /// </summary>
        /// <param name="decisionTableEvaluationListeners"> the list of post decision table evaluation listeners </param>
        /// <returns> this configuration </returns>
        public abstract DmnEngineConfiguration customPostDecisionTableEvaluationListeners(
            IList<IDmnDecisionTableEvaluationListener> decisionTableEvaluationListeners);


        /// <summary>
        ///     Set the list of pre decision evaluation listeners. They will be notified before
        ///     the default decision evaluation listeners.
        /// </summary>
        /// <param name="decisionEvaluationListeners"> the list of pre decision evaluation listeners </param>
        /// <returns> this configuration </returns>
        public abstract DmnEngineConfiguration customPreDecisionEvaluationListeners(
            IList<IDmnDecisionEvaluationListener> decisionEvaluationListeners);


        /// <summary>
        ///     Set the list of post decision evaluation listeners. They will be notified after
        ///     the default decision evaluation listeners.
        /// </summary>
        /// <param name="decisionTableEvaluationListeners"> the list of post decision evaluation listeners </param>
        /// <returns> this configuration </returns>
        public abstract DmnEngineConfiguration customPostDecisionEvaluationListeners(
            IList<IDmnDecisionEvaluationListener> decisionEvaluationListeners);

        /// <summary>
        ///     Create a <seealso cref="IDmnEngine" /> with this configuration
        /// </summary>
        /// <returns> the created <seealso cref="IDmnEngine" /> </returns>
        public abstract IDmnEngine buildEngine();
    }
}