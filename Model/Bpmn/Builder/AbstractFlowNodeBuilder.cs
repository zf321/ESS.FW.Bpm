using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml.instance;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractFlowNodeBuilder<TE> : AbstractFlowElementBuilder<TE>, IFlowNodeBuilder<TE> where TE : IFlowNode
    {

        private SequenceFlowBuilder _currentSequenceFlowBuilder;

        protected internal AbstractFlowNodeBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
        {
        }

        private SequenceFlowBuilder CurrentSequenceFlowBuilder
        {
            get
            {
                if (_currentSequenceFlowBuilder == null)
                {
                    ISequenceFlow sequenceFlow = CreateSibling<ISequenceFlow>(typeof(ISequenceFlow));
                    _currentSequenceFlowBuilder = sequenceFlow.Builder();
                }
                return _currentSequenceFlowBuilder;
            }
        }

        public virtual IFlowNodeBuilder<TE> Condition(string name, string condition)
        {
            if (!string.ReferenceEquals(name, null))
            {
                CurrentSequenceFlowBuilder
                          .Name<SequenceFlowBuilder>(name);
            }
            IConditionExpression conditionExpression = CreateInstance<IConditionExpression>(typeof(IConditionExpression));
            conditionExpression.TextContent = condition;
            CurrentSequenceFlowBuilder.Condition(conditionExpression);
            return this;
        }

        protected internal virtual void ConnectTarget(IFlowNode target)
        {
            CurrentSequenceFlowBuilder.From(element).To(target);

            ISequenceFlow sequenceFlow = CurrentSequenceFlowBuilder.element;
            CreateBpmnEdge(sequenceFlow);
            _currentSequenceFlowBuilder = null;
        }

        public virtual IFlowNodeBuilder<TE> SequenceFlowId(string sequenceFlowId)
        {
            CurrentSequenceFlowBuilder.Id<SequenceFlowBuilder>(sequenceFlowId);
            return this;
        }

        private T CreateTarget<T>(Type typeClass) where T : IFlowNode
        {
            return CreateTarget<T>(typeClass, null);
        }

        protected internal virtual T CreateTarget<T>(Type typeClass, string identifier) where T : IFlowNode
        {
            T target = CreateSibling<T>(typeClass, identifier);

            IBpmnShape targetBpmnShape = CreateBpmnShape(target);
            Coordinates = targetBpmnShape;
            ConnectTarget(target);
            ResizeSubProcess(targetBpmnShape);
            return target;
        }

        public virtual ServiceTaskBuilder ServiceTask()
        {
            return CreateTarget<IServiceTask>(typeof(IServiceTask)).Builder();
        }

        public virtual ServiceTaskBuilder ServiceTask(string id)
        {
            return CreateTarget<IServiceTask>(typeof(IServiceTask), id).Builder();
        }

        public virtual SendTaskBuilder SendTask()
        {
            return CreateTarget<ISendTask>(typeof(ISendTask)).Builder();
        }

        public virtual SendTaskBuilder SendTask(string id)
        {
            return CreateTarget<ISendTask>(typeof(ISendTask), id).Builder();
        }

        public virtual UserTaskBuilder UserTask()
        {
            return CreateTarget<IUserTask>(typeof(IUserTask)).Builder();
        }

        public virtual UserTaskBuilder UserTask(string id)
        {
            return CreateTarget<IUserTask>(typeof(IUserTask)).Builder();
            //return CreateTarget<IUserTask>(typeof(IUserTask), id).Builder<UserTaskBuilder, IUserTask>();
        }

        public virtual BusinessRuleTaskBuilder BusinessRuleTask()
        {
            return CreateTarget<IBusinessRuleTask>(typeof(IBusinessRuleTask)).Builder();
        }

        public virtual BusinessRuleTaskBuilder BusinessRuleTask(string id)
        {
            return CreateTarget<IBusinessRuleTask>(typeof(IBusinessRuleTask), id).Builder();
        }

        public virtual ScriptTaskBuilder ScriptTask()
        {
            return CreateTarget<IScriptTask>(typeof(IScriptTask)).Builder();
        }

        public virtual ScriptTaskBuilder ScriptTask(string id)
        {
            return CreateTarget<IScriptTask>(typeof(IScriptTask), id).Builder();
        }

        public virtual ReceiveTaskBuilder ReceiveTask()
        {
            return CreateTarget<IReceiveTask>(typeof(IReceiveTask)).Builder();
        }

        public virtual ReceiveTaskBuilder ReceiveTask(string id)
        {
            return CreateTarget<IReceiveTask>(typeof(IReceiveTask), id).Builder();
        }

        public virtual ManualTaskBuilder ManualTask()
        {
            return CreateTarget<IManualTask>(typeof(IManualTask)).Builder();
        }

        public virtual ManualTaskBuilder ManualTask(string id)
        {
            return CreateTarget<IManualTask>(typeof(IManualTask), id).Builder();
        }

        public virtual EndEventBuilder EndEvent()
        {
            return CreateTarget<IEndEvent>(typeof(IEndEvent)).Builder();
        }

        public virtual EndEventBuilder EndEvent(string id)
        {
            return CreateTarget<IEndEvent>(typeof(IEndEvent), id).Builder();
        }

        public virtual ParallelGatewayBuilder ParallelGateway()
        {
            return CreateTarget<IParallelGateway>(typeof(IParallelGateway)).Builder();
        }

        public virtual ParallelGatewayBuilder ParallelGateway(string id)
        {
            return CreateTarget<IParallelGateway>(typeof(IParallelGateway), id).Builder();
        }

        public virtual ExclusiveGatewayBuilder ExclusiveGateway()
        {
            return CreateTarget<IExclusiveGateway>(typeof(IExclusiveGateway)).Builder();
        }

        public virtual InclusiveGatewayBuilder InclusiveGateway()
        {
            return CreateTarget<INclusiveGateway>(typeof(INclusiveGateway)).Builder();
        }

        public virtual EventBasedGatewayBuilder EventBasedGateway()
        {
            return CreateTarget<IEventBasedGateway>(typeof(IEventBasedGateway)).Builder();
        }

        public virtual ExclusiveGatewayBuilder ExclusiveGateway(string id)
        {
            return CreateTarget<IExclusiveGateway>(typeof(IExclusiveGateway), id).Builder();
        }

        public virtual InclusiveGatewayBuilder InclusiveGateway(string id)
        {
            return CreateTarget<INclusiveGateway>(typeof(INclusiveGateway), id).Builder();
        }

        public virtual IntermediateCatchEventBuilder IntermediateCatchEvent()
        {
            return CreateTarget<INtermediateCatchEvent>(typeof(INtermediateCatchEvent)).Builder();
        }

        public virtual IntermediateCatchEventBuilder IntermediateCatchEvent(string id)
        {
            return CreateTarget<INtermediateCatchEvent>(typeof(INtermediateCatchEvent), id).Builder();
        }

        public virtual IntermediateThrowEventBuilder IntermediateThrowEvent()
        {
            return CreateTarget<INtermediateThrowEvent>(typeof(INtermediateThrowEvent)).Builder();
        }

        public virtual IntermediateThrowEventBuilder IntermediateThrowEvent(string id)
        {
            return CreateTarget<INtermediateThrowEvent>(typeof(INtermediateThrowEvent), id).Builder();
        }

        public virtual CallActivityBuilder CallActivity()
        {
            return CreateTarget<ICallActivity>(typeof(ICallActivity)).Builder();
        }

        public virtual CallActivityBuilder CallActivity(string id)
        {
            return CreateTarget<ICallActivity>(typeof(ICallActivity), id).Builder();
        }

        public virtual SubProcessBuilder SubProcess()
        {
            return CreateTarget<ISubProcess>(typeof(ISubProcess)).Builder();
        }

        public virtual SubProcessBuilder SubProcess(string id)
        {
            return CreateTarget<ISubProcess>(typeof(ISubProcess), id).Builder();
        }

        public virtual TransactionBuilder Transaction()
        {
            ITransaction transaction = CreateTarget<ITransaction>(typeof(ITransaction));
            return new TransactionBuilder(modelInstance, transaction);
        }

        public virtual TransactionBuilder Transaction(string id)
        {
            ITransaction transaction = CreateTarget<ITransaction>(typeof(ITransaction), id);
            return new TransactionBuilder(modelInstance, transaction);
        }

        public virtual IGateway FindLastGateway()
        {
            IFlowNode lastGateway = element;
            while (true)
            {
                try
                {
                    lastGateway = lastGateway.PreviousNodes.SingleResult();
                    if (lastGateway is IGateway)
                    {
                        return (IGateway)lastGateway;
                    }
                }
                catch (BpmnModelException e)
                {
                    throw new BpmnModelException("Unable to determine an unique previous gateway of " + lastGateway.Id, e);
                }
            }
        }
        
        public virtual IGatewayBuilder<TE> /*IBaseElementBuilder*/ MoveToLastGateway()
        {
            return FindLastGateway().Builder<IGatewayBuilder<TE>, TE>();
        }

        public virtual IFlowNodeBuilder<TE> MoveToNode(string identifier)
        {
            IModelElementInstance instance = modelInstance.GetModelElementById(identifier);
            if (instance != null && instance is IFlowNode)
            {
                return ((IFlowNode)instance).Builder<IFlowNodeBuilder<TE>, TE>();
            }
            else
            {
                throw new BpmnModelException("Flow node not found for id " + identifier);
            }
        }

        public virtual IActivityBuilder<IActivity> MoveToActivity(string identifier) /*where T : IActivityBuilder*/
        {
            IModelElementInstance instance = modelInstance.GetModelElementById(identifier);
            if (instance != null && instance is IActivity)
            {
                return ((IActivity)instance).Builder<IActivityBuilder<IActivity>, IActivity>();
            }
            else
            {
                throw new BpmnModelException("Activity not found for id " + identifier);
            }
        }
        
        public virtual IFlowNodeBuilder<TE> ConnectTo(string identifier)
        {
            IModelElementInstance target = (TE)modelInstance.GetModelElementById(identifier);
            if (target == null)
            {
                throw new BpmnModelException("Unable to connect " + element.Id + " to element " + identifier + " cause it not exists.");
            }
            else if (!(target is IFlowNode))
            {
                throw new BpmnModelException("Unable to connect " + element.Id + " to element " + identifier + " cause its not a flow node.");
            }
            else
            {
                IFlowNode targetNode = (IFlowNode)target;
                ConnectTarget(targetNode);
                return targetNode.Builder<IFlowNodeBuilder<TE>, TE>();
            }
        }

        /// <summary>
        /// Sets the Camunda AsyncBefore attribute for the build flow node.
        /// </summary>
        /// <param name="asyncBefore">
        ///          boolean value to set </param>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaAsyncBefore(bool asyncBefore)
        {
            element.CamundaAsyncBefore = asyncBefore;
            return this;
        }

        /// <summary>
        /// Sets the Camunda asyncBefore attribute to true.
        /// </summary>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaAsyncBefore()
        {
            element.CamundaAsyncBefore = true;
            return this;
        }

        /// <summary>
        /// Sets the Camunda asyncAfter attribute for the build flow node.
        /// </summary>
        /// <param name="asyncAfter">
        ///          boolean value to set </param>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaAsyncAfter(bool asyncAfter)
        {
            element.CamundaAsyncAfter = asyncAfter;
            return this;
        }

        /// <summary>
        /// Sets the Camunda asyncAfter attribute to true.
        /// </summary>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaAsyncAfter()
        {
            element.CamundaAsyncAfter = true;
            return this;
        }

        /// <summary>
        /// Sets the Camunda exclusive attribute to true.
        /// </summary>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> NotCamundaExclusive()
        {
            element.CamundaExclusive = false;
            return this;
        }

        /// <summary>
        /// Sets the camunda exclusive attribute for the build flow node.
        /// </summary>
        /// <param name="exclusive">
        ///          boolean value to set </param>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaExclusive(bool exclusive)
        {
            element.CamundaExclusive = exclusive;
            return this;
        }

        public virtual IFlowNodeBuilder<TE> CamundaJobPriority(string jobPriority)
        {
            element.CamundaJobPriority = jobPriority;
            return this;
        }

        /// <summary>
        /// Sets the camunda failedJobRetryTimeCycle attribute for the build flow node.
        /// </summary>
        /// <param name="retryTimeCycle">
        ///          the retry time cycle value to set </param>
        /// <returns> the builder object </returns>
        public virtual IFlowNodeBuilder<TE> CamundaFailedJobRetryTimeCycle(string retryTimeCycle)
        {
            ICamundaFailedJobRetryTimeCycle failedJobRetryTimeCycle = CreateInstance<ICamundaFailedJobRetryTimeCycle>(typeof(ICamundaFailedJobRetryTimeCycle));
            failedJobRetryTimeCycle.TextContent = retryTimeCycle;

            AddExtensionElement(failedJobRetryTimeCycle);

            return this;
        }

        public virtual IFlowNodeBuilder<TE> CamundaExecutionListenerClass(string eventName, string fullQualifiedClassName)
        {
            ICamundaExecutionListener executionListener = CreateInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
            executionListener.CamundaEvent = eventName;
            executionListener.CamundaClass = fullQualifiedClassName;

            AddExtensionElement(executionListener);

            return this;
        }

        public virtual IFlowNodeBuilder<TE> CamundaExecutionListenerExpression(string eventName, string expression)
        {
            ICamundaExecutionListener executionListener = CreateInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
            executionListener.CamundaEvent = eventName;
            executionListener.CamundaExpression = expression;

            AddExtensionElement(executionListener);

            return this;
        }

        public virtual IFlowNodeBuilder<TE> CamundaExecutionListenerDelegateExpression(string eventName, string delegateExpression)
        {
            ICamundaExecutionListener executionListener = CreateInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
            executionListener.CamundaEvent = eventName;
            executionListener.CamundaDelegateExpression = delegateExpression;

            AddExtensionElement(executionListener);

            return this;
        }

        public IFlowNodeBuilder<TE> Func(Action<TE> action)
        {
            action.Invoke(element);
            return this;
        }
    }

}