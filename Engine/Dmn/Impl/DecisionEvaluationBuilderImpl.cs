using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.Impl.Cmd;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.exception.dmn;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Dmn.Impl
{

    public class DecisionEvaluationBuilderImpl : IDecisionsEvaluationBuilder
    {
        private static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        protected internal ICommandExecutor CommandExecutor;
        protected internal string decisionDefinitionId;

        protected internal string decisionDefinitionKey;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string DecisionDefinitionTenantIdRenamed;
        protected internal bool IsTenantIdSet;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDictionary<string, object> VariablesRenamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal int? VersionRenamed;


        public DecisionEvaluationBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        // getters ////////////////////////////////////

        public virtual string DecisionDefinitionKey
        {
            get { return DecisionDefinitionKey; }
        }

        public virtual string DecisionDefinitionId
        {
            get { return DecisionDefinitionId; }
        }

        public virtual int? _Version
        {
            get { return VersionRenamed; }
        }

        public virtual IDictionary<string, object> _Variables
        {
            get
            {
                return VariablesRenamed; 
            }
        }

        public virtual string _DecisionDefinitionTenantId
        {
            get { return DecisionDefinitionTenantIdRenamed; }
        }

        public virtual bool TenantIdSet
        {
            get { return IsTenantIdSet; }
        }

        public virtual IDecisionsEvaluationBuilder Variables(IDictionary<string, object> variables)
        {
            VariablesRenamed = variables;
            return this;
        }

        public virtual IDecisionsEvaluationBuilder Version(int? version)
        {
            VersionRenamed = version;
            return this;
        }

        public virtual IDecisionsEvaluationBuilder DecisionDefinitionTenantId(string tenantId)
        {
            DecisionDefinitionTenantIdRenamed = tenantId;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IDecisionsEvaluationBuilder DecisionDefinitionWithoutTenantId()
        {
            DecisionDefinitionTenantIdRenamed = null;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IDmnDecisionResult Evaluate()
        {
            EnsureUtil.EnsureOnlyOneNotNull(typeof (NotValidException), "either decision definition id or key must be set",
            decisionDefinitionId, decisionDefinitionKey);

            if (IsTenantIdSet && !ReferenceEquals(DecisionDefinitionId, null))
                throw Log.ExceptionEvaluateDecisionDefinitionByIdAndTenantId();

            try
            {
                return CommandExecutor.Execute(new EvaluateDecisionCmd(this));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DecisionDefinitionNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public static IDecisionsEvaluationBuilder EvaluateDecisionByKey(ICommandExecutor commandExecutor,
            string decisionDefinitionKey)
        {
            var builder = new DecisionEvaluationBuilderImpl(commandExecutor);
            builder.decisionDefinitionKey = decisionDefinitionKey;
            return builder;
        }

        public static IDecisionsEvaluationBuilder EvaluateDecisionById(ICommandExecutor commandExecutor,
            string decisionDefinitionId)
        {
            var builder = new DecisionEvaluationBuilderImpl(commandExecutor);
            builder.decisionDefinitionId = decisionDefinitionId;
            return builder;
        }
    }
}