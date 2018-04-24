using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static EnsureUtil.ensureNotNull;


    //using DmnElementTransformHandler = org.camunda.bpm.dmn.engine.impl.spi.transform.DmnElementTransformHandler;

    public class DefaultDmnTransform : IDmnTransform, IDmnElementTransformContext
    {
        private static readonly DmnTransformLogger LOG = DmnLogger.TRANSFORM_LOGGER;
        protected internal IDmnDataTypeTransformerRegistry dataTypeTransformerRegistry;
        protected internal DmnDecisionImpl decision;
        protected internal DmnDecisionTableImpl decisionTable;
        protected internal IDmnElementTransformHandlerRegistry handlerRegistry;
        protected internal IDmnHitPolicyHandlerRegistry hitPolicyHandlerRegistry;

        // context
        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnModelInstance modelInstance_Renamed;
        protected internal object parent;

        protected internal IDmnTransformer transformer;

        protected internal IList<IDmnTransformListener> transformListeners;

        public DefaultDmnTransform(IDmnTransformer transformer)
        {
            this.transformer = transformer;
            transformListeners = transformer.TransformListeners;
            handlerRegistry = transformer.ElementTransformHandlerRegistry;
            dataTypeTransformerRegistry = transformer.DataTypeTransformerRegistry;
            hitPolicyHandlerRegistry = transformer.HitPolicyHandlerRegistry;
        }

        public virtual object Parent
        {
            get { return parent; }
        }

        public virtual IDmnDecision Decision
        {
            get { return decision; }
        }

        public virtual IDmnDataTypeTransformerRegistry DataTypeTransformerRegistry
        {
            get { return dataTypeTransformerRegistry; }
        }

        public virtual IDmnHitPolicyHandlerRegistry HitPolicyHandlerRegistry
        {
            get { return hitPolicyHandlerRegistry; }
        }

        IDmnModelInstance IDmnElementTransformContext.ModelInstance
        {
            get
            {
                return modelInstance_Renamed;

            }
        }

        public Stream ModelInstance
        {
            set { throw new NotImplementedException(); }
        }

        public virtual IDmnTransform modelInstance(Stream inputStream)
        {
            SetModelInstance(inputStream);
            return this;
        }

        public virtual IDmnTransform modelInstance(IDmnModelInstance modelInstance)
        {
            SetModelInstance(modelInstance);
            return this;
        }

        // transform ////////////////////////////////////////////////////////////////

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.dmn.engine.DmnDecisionRequirementsGraph> T transformDecisionRequirementsGraph()
        public virtual T transformDecisionRequirementsGraph<T>() where T : IDmnDecisionRequirementsGraph
        {
            //TODO 暂时取消异常拦截
            //try
            //{
                var definitions = modelInstance_Renamed.Definitions;
                return (T)TransformDefinitions(definitions);
            //}
            //catch (System.Exception e)
            //{
                //throw LOG.ErrorWhileTransformingDefinitions(e);
            //}
        }

        //TODO Input,out，Rule解析入口
        List<T> IDmnTransform.transformDecisions<T>()
        {
            //TODO decisions input,output,rule属性没有值
            var definitions = GetModelInstance().Definitions;
            ICollection<IDecision> decisions = definitions.GetChildElementsByType<IDecision>(typeof(IDecision));
            //var t= transformDecisions(decisions) as List<T>;
            return TransformDecisions(decisions) as List<T>;
            //        try
            //        {

            //            Definitions definitions = modelInstance.getDefinitions();
            //            Collection<Decision> decisions = definitions.getChildElementsByType(Decision.class);
            //  return (List<T>) transformDecisions(decisions);
            //}
            //catch (Exception e) {
            //throw LOG.errorWhileTransformingDecisions(e);
            //}
        }

        public virtual void SetModelInstance(string file)
        {
            EnsureUtil.EnsureNotNull("file", file);
            try
            {
                modelInstance_Renamed = Model.Dmn.Dmn.ReadModelFromFile(file);
            }
            catch (DmnModelException e)
            {
                throw LOG.UnableToTransformDecisionsFromFile(file, e);
            }
        }

        public virtual IDmnTransform modelInstance(string file)
        {
            SetModelInstance(file);
            return this;
        }

        public virtual void SetModelInstance(Stream inputStream)
        {
            EnsureUtil.EnsureNotNull("inputStream", inputStream);
            try
            {
                modelInstance_Renamed = Model.Dmn.Dmn.ReadModelFromStream(inputStream);
            }
            catch (DmnModelException e)
            {
                throw LOG.UnableToTransformDecisionsFromInputStream(e);
            }
        }

        public virtual void SetModelInstance(IDmnModelInstance modelInstance)
        {
            EnsureUtil.EnsureNotNull("dmnModelInstance", modelInstance);
            modelInstance_Renamed = modelInstance;
        }

        protected internal virtual IDmnDecisionRequirementsGraph TransformDefinitions(IDefinitions definitions)
        {
            var handler =
                handlerRegistry.getHandler<IDefinitions, DmnDecisionRequirementsGraphImpl>(typeof(IDefinitions));
            var dmnDrg = handler.HandleElement(this, definitions);

            //validate id of drd
            if (ReferenceEquals(dmnDrg.Key, null))
                throw LOG.DrdIdIsMissing(dmnDrg);

            ICollection<IDecision> decisions = new List<IDecision>();
            //definitions.getChildElementsByType(typeof (Decision)) as ICollection<Decision>;
            foreach (var item in definitions.GetChildElementsByType(typeof(IDecision)))
                decisions.Add(item as IDecision);
            var dmnDecisions = TransformDecisions(decisions);
            foreach (var dmnDecision in dmnDecisions)
                dmnDrg.AddDecision(dmnDecision);

            NotifyTransformListeners(definitions, dmnDrg);
            return dmnDrg;
            //return null;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.dmn.engine.DmnDecision> java.Util.List<T> transformDecisions()
        public virtual IList<T> TransformDecisions<T>() where T : IDmnDecision
        {
            try
            {
                var definitions = modelInstance_Renamed.Definitions;

                IList<T> decisions = new List<T>();
                foreach (var item in definitions.GetChildElementsByType(typeof(IDecision)))
                    decisions.Add((T)item);
                return decisions;
            }
            catch (System.Exception e)
            {
                throw LOG.ErrorWhileTransformingDecisions(e);
            }
        }

        protected internal virtual IList<IDmnDecision> TransformDecisions(ICollection<IDecision> decisions)
        {
            //TODO dmnDecisions转换错误 找不到Id(其他属性input,output,Rule为null)
            var dmnDecisions = TransformIndividualDecisions(decisions);
            BuildDecisionRequirements(decisions, dmnDecisions);
            IList<IDmnDecision> dmnDecisionList = new List<IDmnDecision>(dmnDecisions.Values);

            foreach (var decision in decisions)
                if (decision.Id != null)
                {
                    //decision.Id 又可能不在集合中 不加入结果
                    IDmnDecision dmnDecision;
                    if (dmnDecisions.ContainsKey(decision.Id))
                    {
                        dmnDecision = dmnDecisions[decision.Id];
                        NotifyTransformListeners(decision, dmnDecision);
                    }
                }
            EnsureNoLoopInDecisions(dmnDecisionList);

            return dmnDecisionList;
        }

        protected internal virtual IDictionary<string, DmnDecisionImpl> TransformIndividualDecisions(
            ICollection<IDecision> decisions)
        {
            IDictionary<string, DmnDecisionImpl> dmnDecisions = new Dictionary<string, DmnDecisionImpl>();

            foreach (var decision in decisions)
            {
                var dmnDecision = TransformDecision(decision);
                if (dmnDecision != null)
                    dmnDecisions[dmnDecision.Key] = dmnDecision;
            }
            return dmnDecisions;
        }

        //TODO 基于Attribute中的id转换 dmnDecisions[decision.Id] 
        protected internal virtual void BuildDecisionRequirements(ICollection<IDecision> decisions,
            IDictionary<string, DmnDecisionImpl> dmnDecisions)
        {
            foreach (var decision in decisions)
            {
                var requiredDmnDecisions = GetRequiredDmnDecisions(decision, dmnDecisions);
                //var dmnDecision = dmnDecisions[decision.Id];
                DmnDecisionImpl dmnDecision = null;
                if ((decision.Id != null) && dmnDecisions.ContainsKey(decision.Id) && (decision != null))
                    dmnDecision = dmnDecisions[decision.Id];
                if (requiredDmnDecisions.Count > 0)
                    dmnDecision.RequiredDecisions = requiredDmnDecisions;
            }
        }

        protected internal virtual void EnsureNoLoopInDecisions(IList<IDmnDecision> dmnDecisionList)
        {
            IList<string> visitedDecisions = new List<string>();

            foreach (var decision in dmnDecisionList)
                EnsureNoLoopInDecision(decision, new List<string>(), visitedDecisions);
        }

        protected internal virtual void EnsureNoLoopInDecision(IDmnDecision decision, IList<string> parentDecisionList,
            IList<string> visitedDecisions)
        {
            if (visitedDecisions.Contains(decision.Key))
                return;

            parentDecisionList.Add(decision.Key);

            foreach (var requiredDecision in decision.RequiredDecisions)
            {
                if (parentDecisionList.Contains(requiredDecision.Key))
                    throw LOG.RequiredDecisionLoopDetected(requiredDecision.Key);

                EnsureNoLoopInDecision(requiredDecision, new List<string>(parentDecisionList), visitedDecisions);
            }
            visitedDecisions.Add(decision.Key);
        }

        protected internal virtual IList<IDmnDecision> GetRequiredDmnDecisions(IDecision decision,
            IDictionary<string, DmnDecisionImpl> dmnDecisions)
        {
            IList<IDmnDecision> requiredDecisionList = new List<IDmnDecision>();
            foreach (var informationRequirement in decision.InformationRequirements)
            {
                var requiredDecision = informationRequirement.RequiredDecision;
                if (requiredDecision != null)
                {
                    IDmnDecision requiredDmnDecision = dmnDecisions[requiredDecision.Id];
                    requiredDecisionList.Add(requiredDmnDecision);
                }
            }
            return requiredDecisionList;
        }

        //TODO Dmn获取Decision
        protected internal virtual DmnDecisionImpl TransformDecision(IDecision decision)
        {
            //TODO Hander处理 
            var handler = handlerRegistry.getHandler<IDecision, DmnDecisionImpl>(typeof(IDecision));
            var dmnDecision = handler.HandleElement(this, decision);
            this.decision = dmnDecision;
            // validate decision id
            if (ReferenceEquals(dmnDecision.Key, null))
                throw LOG.DecisionIdIsMissing(dmnDecision);

            var expression = decision.Expression;
            if (expression == null)
            {
                LOG.DecisionWithoutExpression(decision);
                return null;
            }
            //TODO expression异常
            if (expression is IDecisionTable)
            {
                var dmnDecisionTable = TransformDecisionTable((IDecisionTable)expression);
                dmnDecision.DecisionLogic = dmnDecisionTable;
            }
            else if (expression is ILiteralExpression)
            {
                var dmnDecisionLiteralExpression = TransformDecisionLiteralExpression(decision,
                    (ILiteralExpression)expression);
                dmnDecision.DecisionLogic = dmnDecisionLiteralExpression;
            }
            else
            {
                LOG.DecisionTypeNotSupported(expression, decision);
                return null;
            }

            //return null;
            return dmnDecision;
        }

        protected internal virtual DmnDecisionTableImpl TransformDecisionTable(IDecisionTable decisionTable)
        {
            var handler =
                handlerRegistry.getHandler<IDecisionTable, DmnDecisionTableImpl>(typeof(IDecisionTable));
            var dmnDecisionTable = handler.HandleElement(this, decisionTable);
            //TODO Input异常
            foreach (var input in decisionTable.Inputs)
            {
                parent = dmnDecisionTable;
                this.decisionTable = dmnDecisionTable;
                //TODO actualNsToAlternative[actualNs]异常 
                var dmnInput = TransformDecisionTableInput(input);
                if (dmnInput != null)
                {
                    dmnDecisionTable.Inputs.Add(dmnInput);
                    NotifyTransformListeners(input, dmnInput);
                }
            }
            //TODO OutPut异常
            var needsName = decisionTable.Outputs.Count > 1;
            ISet<string> usedNames = new HashSet<string>();
            foreach (var output in decisionTable.Outputs)
            {
                parent = dmnDecisionTable;
                this.decisionTable = dmnDecisionTable;
                var dmnOutput = TransformDecisionTableOutput(output);
                if (dmnOutput != null)
                {
                    // validate output name
                    var outputName = dmnOutput.OutputName;
                    if (needsName && ReferenceEquals(outputName, null))
                        throw LOG.CompoundOutputsShouldHaveAnOutputName(dmnDecisionTable, dmnOutput);
                    if (usedNames.Contains(outputName))
                        throw LOG.CompoundOutputWithDuplicateName(dmnDecisionTable, dmnOutput);
                    usedNames.Add(outputName);

                    dmnDecisionTable.Outputs.Add(dmnOutput);
                    NotifyTransformListeners(output, dmnOutput);
                }
            }

            foreach (var rule in decisionTable.Rules)
            {
                parent = dmnDecisionTable;
                this.decisionTable = dmnDecisionTable;
                var dmnRule = TransformDecisionTableRule(rule);
                if (dmnRule != null)
                {
                    dmnDecisionTable.Rules.Add(dmnRule);
                    NotifyTransformListeners(rule, dmnRule);
                }
            }
            return dmnDecisionTable;
        }

        protected internal virtual DmnDecisionTableInputImpl TransformDecisionTableInput(IInput input)
        {
            var handler =
                handlerRegistry.getHandler<IInput, DmnDecisionTableInputImpl>(typeof(IInput));
            //TODO 421异常
            var dmnInput = handler.HandleElement(this, input);

            //validate input id
            if (ReferenceEquals(dmnInput.Id, null))
                throw LOG.DecisionTableInputIdIsMissing(decision, dmnInput);

            var inputExpression = input.InputExpression;
            if (inputExpression != null)
            {
                parent = dmnInput;
                var dmnExpression = TransformInputExpression(inputExpression);
                if (dmnExpression != null)
                    dmnInput.Expression = dmnExpression;
            }

            return dmnInput;
            //return null;
        }

        //TODO DmnDecisionTableOutputImpl解析
        protected internal virtual DmnDecisionTableOutputImpl TransformDecisionTableOutput(IOutput output)
        {
            var handler =
                handlerRegistry.getHandler<IOutput, DmnDecisionTableOutputImpl>(typeof(IOutput));
            var dmnOutput = handler.HandleElement(this, output);

            // validate output id
            if (ReferenceEquals(dmnOutput.Id, null))
                throw LOG.DecisionTableOutputIdIsMissing(decision, dmnOutput);

            return dmnOutput;
        }

        protected internal virtual DmnDecisionTableRuleImpl TransformDecisionTableRule(IRule rule)
        {
            var handler =
                handlerRegistry.getHandler<IRule, DmnDecisionTableRuleImpl>(typeof(IRule));
            var dmnRule = handler.HandleElement(this, rule);

            //validate rule id
            if (ReferenceEquals(dmnRule.Id, null))
                throw LOG.DecisionTableRuleIdIsMissing(decision, dmnRule);

            var inputs = decisionTable.Inputs;
            ICollection<IInputEntry> inputEntries = rule.InputEntries;
            if (inputs.Count != inputEntries.Count)
                throw LOG.DifferentNumberOfInputsAndInputEntries(inputs.Count, inputEntries.Count, dmnRule);

            foreach (var inputEntry in inputEntries)
            {
                parent = dmnRule;

                var condition = TransformInputEntry(inputEntry);
                dmnRule.Conditions.Add(condition);
            }

            var outputs = decisionTable.Outputs;
            ICollection<IOutputEntry> outputEntries = rule.OutputEntries;
            if (outputs.Count != outputEntries.Count)
                throw LOG.DifferentNumberOfOutputsAndOutputEntries(outputs.Count, outputEntries.Count, dmnRule);

            foreach (var outputEntry in outputEntries)
            {
                parent = dmnRule;
                var conclusion = TransformOutputEntry(outputEntry);
                dmnRule.Conclusions.Add(conclusion);
            }

            return dmnRule;
            //return null;
        }

        protected internal virtual DmnExpressionImpl TransformInputExpression(IInputExpression inputExpression)
        {
            var handler =
                handlerRegistry.getHandler<IInputExpression, DmnExpressionImpl>(typeof(IInputExpression));
            return handler.HandleElement(this, inputExpression);
            //return null;
        }

        protected internal virtual DmnExpressionImpl TransformInputEntry(IInputEntry inputEntry)
        {
            var handler =
                handlerRegistry.getHandler<IInputEntry, DmnExpressionImpl>(typeof(IInputEntry));
            return handler.HandleElement(this, inputEntry);
            //return null;
        }

        protected internal virtual DmnExpressionImpl TransformOutputEntry(IOutputEntry outputEntry)
        {
            var handler =
                handlerRegistry.getHandler<IOutputEntry, DmnExpressionImpl>(typeof(IOutputEntry));
            return handler.HandleElement(this, outputEntry);
            //return null;
        }

        protected internal virtual DmnDecisionLiteralExpressionImpl TransformDecisionLiteralExpression(
            IDecision decision, ILiteralExpression literalExpression)
        {
            var dmnDecisionLiteralExpression = new DmnDecisionLiteralExpressionImpl();

            var variable = decision.Variable;
            if (variable == null)
                throw LOG.DecisionVariableIsMissing(decision.Id);
            //TODO dmnVariable解析
            var dmnVariable = TransformVariable(variable);
            dmnDecisionLiteralExpression.Variable = dmnVariable;

            var dmnLiteralExpression = TransformLiteralExpression(literalExpression);
            dmnDecisionLiteralExpression.Expression = dmnLiteralExpression;

            return dmnDecisionLiteralExpression;
        }

        protected internal virtual DmnExpressionImpl TransformLiteralExpression(ILiteralExpression literalExpression)
        {
            var handler =
                handlerRegistry.getHandler<ILiteralExpression, DmnExpressionImpl>(typeof(ILiteralExpression));
            return handler.HandleElement(this, literalExpression);
            //return null;
        }

        protected internal virtual DmnVariableImpl TransformVariable(IVariable variable)
        {
            var handler =
                handlerRegistry.getHandler<IVariable, DmnVariableImpl>(typeof(IVariable));
            return handler.HandleElement(this, variable);
            //return null;
        }

        // listeners ////////////////////////////////////////////////////////////////

        protected internal virtual void NotifyTransformListeners(IDecision decision, IDmnDecision dmnDecision)
        {
            foreach (var transformListener in transformListeners)
                transformListener.transformDecision(decision, dmnDecision);
        }

        protected internal virtual void NotifyTransformListeners(IInput input, DmnDecisionTableInputImpl dmnInput)
        {
            foreach (var transformListener in transformListeners)
                transformListener.transformDecisionTableInput(input, dmnInput);
        }

        protected internal virtual void NotifyTransformListeners(IDefinitions definitions,
            DmnDecisionRequirementsGraphImpl dmnDecisionRequirementsGraph)
        {
            foreach (var transformListener in transformListeners)
                transformListener.transformDecisionRequirementsGraph(definitions, dmnDecisionRequirementsGraph);
        }

        protected internal virtual void NotifyTransformListeners(IOutput output, DmnDecisionTableOutputImpl dmnOutput)
        {
            foreach (var transformListener in transformListeners)
                transformListener.transformDecisionTableOutput(output, dmnOutput);
        }

        protected internal virtual void NotifyTransformListeners(IRule rule, DmnDecisionTableRuleImpl dmnRule)
        {
            foreach (var transformListener in transformListeners)
                transformListener.transformDecisionTableRule(rule, dmnRule);
        }

        // context //////////////////////////////////////////////////////////////////

        public virtual IDmnModelInstance GetModelInstance()
        {
            return modelInstance_Renamed;
        }
    }
}