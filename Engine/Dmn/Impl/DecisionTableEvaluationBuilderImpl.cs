using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.Impl.Cmd;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.exception.dmn;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.Impl
{

    /// <summary>
    ///     
    /// </summary>
    public class DecisionTableEvaluationBuilderImpl : IDecisionEvaluationBuilder
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


        public DecisionTableEvaluationBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        // getters ////////////////////////////////////

        public virtual string DecisionDefinitionKey
        {
            get { return decisionDefinitionKey; }
        }

        public virtual string DecisionDefinitionId
        {
            get { return decisionDefinitionId; }
        }

        public virtual int? Version
        {
            get { return VersionRenamed; }
        }

        public virtual IDictionary<string, ITypedValue> Variables
        {
            get
            {
                return null;// variables_Renamed;
            }
        }

        public virtual string DecisionDefinitionTenantId
        {
            get { return DecisionDefinitionTenantIdRenamed; }
        }

        public virtual bool TenantIdSet
        {
            get { return IsTenantIdSet; }
        }

        public virtual IDecisionEvaluationBuilder SetVariables(IDictionary<string, object> variables)
        {
            VariablesRenamed = variables;
            return this;
        }

        public virtual IDecisionEvaluationBuilder SetVersion(int? version)
        {
            VersionRenamed = version;
            return this;
        }

        public virtual IDecisionEvaluationBuilder SetDecisionDefinitionTenantId(string tenantId)
        {
            DecisionDefinitionTenantIdRenamed = tenantId;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IDecisionEvaluationBuilder DecisionDefinitionWithoutTenantId()
        {
            DecisionDefinitionTenantIdRenamed = null;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IDmnDecisionTableResult Evaluate()
        {
            //ensureOnlyOneNotNull(typeof (NotValidException), "either decision definition id or key must be set",
            //decisionDefinitionId, decisionDefinitionKey);

            if (IsTenantIdSet && !ReferenceEquals(decisionDefinitionId, null))
                throw Log.ExceptionEvaluateDecisionDefinitionByIdAndTenantId();

            try
            {
                return CommandExecutor.Execute(new EvaluateDecisionTableCmd(this));
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

        public static IDecisionEvaluationBuilder EvaluateDecisionTableByKey(ICommandExecutor commandExecutor,
            string decisionDefinitionKey)
        {
            var builder = new DecisionTableEvaluationBuilderImpl(commandExecutor);
            builder.decisionDefinitionKey = decisionDefinitionKey;
            return builder;
        }

        public static IDecisionEvaluationBuilder EvaluateDecisionTableById(ICommandExecutor commandExecutor,
            string decisionDefinitionId)
        {
            var builder = new DecisionTableEvaluationBuilderImpl(commandExecutor);
            builder.decisionDefinitionId = decisionDefinitionId;
            return builder;
        }
    }
}