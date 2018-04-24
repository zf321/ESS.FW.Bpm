using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{

    public class DefaultDmnEngine : IDmnEngine
    {
        protected internal static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal DefaultDmnEngineConfiguration dmnEngineConfiguration;
        protected internal IDmnTransformer transformer;

        public DefaultDmnEngine(DefaultDmnEngineConfiguration dmnEngineConfiguration)
        {
            this.dmnEngineConfiguration = dmnEngineConfiguration;
            transformer = dmnEngineConfiguration.Transformer;
        }

        public virtual DmnEngineConfiguration Configuration
        {
            get { return dmnEngineConfiguration; }
        }

        public virtual IList<IDmnDecision> ParseDecisions(Stream inputStream)
        {
            EnsureUtil.EnsureNotNull("inputStream", inputStream);
            return transformer.createTransform().modelInstance(inputStream).transformDecisions<IDmnDecision>();
        }

        public virtual IList<IDmnDecision> ParseDecisions(IDmnModelInstance dmnModelInstance)
        {
            EnsureUtil.EnsureNotNull("dmnModelInstance", dmnModelInstance);
            return transformer.createTransform().modelInstance(dmnModelInstance).transformDecisions<IDmnDecision>();
        }

        public virtual IDmnDecision ParseDecision(string decisionKey, Stream inputStream)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(inputStream);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return decision;
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

        public virtual IDmnDecision ParseDecision(string decisionKey, IDmnModelInstance dmnModelInstance)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(dmnModelInstance);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return decision;
            //return null;
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

        public virtual IDmnDecisionRequirementsGraph ParseDecisionRequirementsGraph(Stream inputStream)
        {
            EnsureUtil.EnsureNotNull("inputStream", inputStream);
            return
                transformer.createTransform()
                    .modelInstance(inputStream)
                    .transformDecisionRequirementsGraph<IDmnDecisionRequirementsGraph>();
        }

        public virtual IDmnDecisionRequirementsGraph ParseDecisionRequirementsGraph(IDmnModelInstance dmnModelInstance)
        {
            EnsureUtil.EnsureNotNull("dmnModelInstance", dmnModelInstance);
            return
                transformer.createTransform()
                    .modelInstance(dmnModelInstance)
                    .transformDecisionRequirementsGraph<IDmnDecisionRequirementsGraph>();
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(IDmnDecision decision,
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("decision", decision);
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecisionTable(decision, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(IDmnDecision decision,
            IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decision", decision);
            EnsureUtil.EnsureNotNull("variableContext", variableContext);

            if (decision is DmnDecisionImpl && decision.DecisionTable)
            {
                var decisionContext = new DefaultDmnDecisionContext(dmnEngineConfiguration);

                var decisionResult = decisionContext.evaluateDecision(decision, variableContext);
                return DmnDecisionTableResultImpl.Wrap(decisionResult);
            }
            throw LOG.decisionIsNotADecisionTable(decision);
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, Stream inputStream,
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecisionTable(decisionKey, inputStream, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey, Stream inputStream,
            IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(inputStream);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return EvaluateDecisionTable(decision, variableContext);
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey,
            IDmnModelInstance dmnModelInstance, IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecisionTable(decisionKey, dmnModelInstance, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTable(string decisionKey,
            IDmnModelInstance dmnModelInstance, IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(dmnModelInstance);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return EvaluateDecisionTable(decision, variableContext);
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

        public virtual IDmnDecisionResult EvaluateDecision(IDmnDecision decision, IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("decision", decision);
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecision(decision, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionResult EvaluateDecision(IDmnDecision decision, IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decision", decision);
            EnsureUtil.EnsureNotNull("variableContext", variableContext);

            if (decision is DmnDecisionImpl)
            {
                var decisionContext = new DefaultDmnDecisionContext(dmnEngineConfiguration);
                return decisionContext.evaluateDecision(decision, variableContext);
            }
            throw LOG.decisionTypeNotSupported(decision);
        }

        public virtual IDmnDecisionResult EvaluateDecision(string decisionKey, Stream inputStream,
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecision(decisionKey, inputStream, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionResult EvaluateDecision(string decisionKey, Stream inputStream,
            IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(inputStream);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return EvaluateDecision(decision, variableContext);
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

        public virtual IDmnDecisionResult EvaluateDecision(string decisionKey, IDmnModelInstance dmnModelInstance,
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            return EvaluateDecision(decisionKey, dmnModelInstance, Variables.FromMap(variables).AsVariableContext());
        }

        public virtual IDmnDecisionResult EvaluateDecision(string decisionKey, IDmnModelInstance dmnModelInstance,
            IVariableContext variableContext)
        {
            EnsureUtil.EnsureNotNull("decisionKey", decisionKey);
            var decisions = ParseDecisions(dmnModelInstance);
            foreach (var decision in decisions)
                if (decisionKey.Equals(decision.Key))
                    return EvaluateDecision(decision, variableContext);
            throw LOG.unableToFindDecisionWithKey(decisionKey);
        }

       
    }
}