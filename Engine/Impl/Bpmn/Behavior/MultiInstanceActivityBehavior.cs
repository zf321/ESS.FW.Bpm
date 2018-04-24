using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Abstract Multi Instance Behavior: used for both parallel and sequential
    ///     multi instance implementation.
    ///     
    /// </summary>
    public abstract class MultiInstanceActivityBehavior : AbstractBpmnActivityBehavior, ICompositeActivityBehavior,
        IModificationObserverBehavior
    {
        // Variable names for mi-body scoped variables (as described in spec)
        public const string NumberOfInstances = "nrOfInstances";
        public const string NumberOfActiveInstances = "nrOfActiveInstances";
        public const string NumberOfCompletedInstances = "nrOfCompletedInstances";

        // Variable names for mi-instance scoped variables (as described in the spec)
        public const string LoopCounter = "loopCounter";

        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        protected internal string collectionElementVariable;
        protected internal IExpression collectionExpression;
        protected internal string collectionVariable;
        protected internal IExpression completionConditionExpression;

        protected internal IExpression loopCardinalityExpression;

        // Getters and Setters ///////////////////////////////////////////////////////////

        public virtual IExpression LoopCardinalityExpression
        {
            get { return loopCardinalityExpression; }
            set { loopCardinalityExpression = value; }
        }


        public virtual IExpression CompletionConditionExpression
        {
            get { return completionConditionExpression; }
            set { completionConditionExpression = value; }
        }


        public virtual IExpression CollectionExpression
        {
            get { return collectionExpression; }
            set { collectionExpression = value; }
        }


        public virtual string CollectionVariable
        {
            get { return collectionVariable; }
            set { collectionVariable = value; }
        }


        public virtual string CollectionElementVariable
        {
            get { return collectionElementVariable; }
            set { collectionElementVariable = value; }
        }

        public abstract void Complete(IActivityExecution scopeExecution);

        public abstract void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution);
        
        public override void Execute(IActivityExecution execution)
        {
            var nrOfInstances = ResolveNrOfInstances(execution);
            if (nrOfInstances == 0)
                Leave(execution);
            else if (nrOfInstances < 0)
                throw Log.InvalidAmountException("instances", nrOfInstances);
            else
                CreateInstances(execution, nrOfInstances);
        }

        public abstract void DestroyInnerInstance(IActivityExecution concurrentExecution);
        public abstract IActivityExecution CreateInnerInstance(IActivityExecution scopeExecution);

        public abstract IList<IActivityExecution> InitializeScope(IActivityExecution scopeExecution,
            int nrOfInnerInstances);

        protected internal virtual void PerformInstance(IActivityExecution execution, IPvmActivity activity,
            int loopCounter)
        {
            SetLoopVariable(execution, LoopCounter, loopCounter);
            EvaluateCollectionVariable(execution, loopCounter);
            execution.IsEnded = false;
            execution.IsActive = true;
            execution.ExecuteActivity(activity);
        }

        protected internal virtual void EvaluateCollectionVariable(IActivityExecution execution, int loopCounter)
        {
            if (UsesCollection() && !ReferenceEquals(collectionElementVariable, null))
            {
                IList collection = null;
                if (collectionExpression != null)
                    collection = (IList) collectionExpression.GetValue(execution);
                else if (!ReferenceEquals(collectionVariable, null))
                    collection = (IList) execution.GetVariable(collectionVariable);

                var value = GetElementAtIndex(loopCounter, collection);
                SetLoopVariable(execution, collectionElementVariable, value);
            }
        }
        
        protected internal abstract void CreateInstances(IActivityExecution execution, int nrOfInstances);

        // Helpers //////////////////////////////////////////////////////////////////////

        protected internal virtual int ResolveNrOfInstances(IActivityExecution execution)
        {
            var nrOfInstances = -1;
            if (loopCardinalityExpression != null)
            {
                nrOfInstances = ResolveLoopCardinality(execution);
            }
            else if (collectionExpression != null)
            {
                var obj = collectionExpression.GetValue(execution);
                if (!(obj.GetType().IsAssignableFrom(typeof(ICollection))))
                    throw Log.UnresolvableExpressionException(collectionExpression.ExpressionText, "Collection");
                nrOfInstances = ((IList) obj)?.Count??0;
            }
            else if (!ReferenceEquals(collectionVariable, null))
            {
                var obj = execution.GetVariable(collectionVariable);
                if (!(obj.GetType().IsAssignableFrom(typeof(ICollection))))
                    throw Log.InvalidVariableTypeException(collectionVariable, "Collection");
                nrOfInstances = ((IList) obj)?.Count??0;
            }
            else
            {
                throw Log.ResolveCollectionExpressionOrVariableReferenceException();
            }
            return nrOfInstances;
        }

        protected internal virtual object GetElementAtIndex(int i, IList collection)
        {
            var value = collection[i];
            return value;
        }

        protected internal virtual bool UsesCollection()
        {
            return (collectionExpression != null) || !ReferenceEquals(collectionVariable, null);
        }

        protected internal virtual int ResolveLoopCardinality(IActivityExecution execution)
        {
            // Using decimal since expr can evaluate to eg. Long (which is also the default for Juel)
            var value = loopCardinalityExpression.GetValue(execution);
            if (value is int)
                return (int) value;
            if (value is decimal)
                return Convert.ToInt32(value);
            if (value is string)
                return Convert.ToInt32((string)value);
            if (value is ITypedValue)
                return (int) ((ITypedValue) value).Value;
            throw Log.ExpressionNotANumberException("loopCardinality", loopCardinalityExpression.ExpressionText);
        }

        protected internal virtual bool CompletionConditionSatisfied(IActivityExecution execution)
        {
            if (completionConditionExpression != null)
            {
                var value = completionConditionExpression.GetValue(execution);
                if (!(value is bool?))
                    throw Log.ExpressionNotBooleanException("completionCondition",
                        completionConditionExpression.ExpressionText);
                var booleanValue = (bool?) value;

                Log.MultiInstanceCompletionConditionState(booleanValue);
                return booleanValue.Value;
            }
            return false;
        }

        public override void DoLeave(IActivityExecution execution)
        {
            CompensationUtil.CreateEventScopeExecution((ExecutionEntity) execution);

            base.DoLeave(execution);
        }

        /// <summary>
        ///     Get the inner activity of the multi instance execution.
        /// </summary>
        /// <param name="execution">
        ///     of multi instance activity
        /// </param>
        /// <returns> inner activity </returns>
        public virtual ActivityImpl GetInnerActivity(IPvmActivity miBodyActivity)
        {
            foreach (var activity in miBodyActivity.Activities)
            {
                var innerActivity = (ActivityImpl) activity;
                // note that miBody can contains also a compensation handler
                if (!innerActivity.CompensationHandler)
                    return innerActivity;
            }
            throw new ProcessEngineException("inner activity of multi instance body activity '" + miBodyActivity.Id +
                                             "' not found");
        }

        protected internal virtual void SetLoopVariable(IActivityExecution execution, string variableName, object value)
        {
            execution.SetVariableLocal(variableName, value);
        }

        protected internal virtual int GetLoopVariable(IActivityExecution execution, string variableName)
        {
            IIntegerValue value = execution.GetVariableLocalTyped<IIntegerValue>(variableName);
            EnsureUtil.EnsureNotNull(
                "The variable \"" + variableName + "\" could not be found in execution with id " + execution.Id, "value",
                value);
            return (int) value.Value;
        }


        protected internal virtual int? GetLocalLoopVariable(IActivityExecution execution, string variableName)
        {
            return (int?) execution.GetVariableLocal(variableName);
        }

        public virtual bool HasLoopVariable(IActivityExecution execution, string variableName)
        {
            return execution.HasVariableLocal(variableName);
        }

        public virtual void RemoveLoopVariable(IActivityExecution execution, string variableName)
        {
            execution.RemoveVariableLocal(variableName);
        }
    }
}