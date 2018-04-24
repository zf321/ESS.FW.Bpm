using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.validation;

namespace Engine.Tests.Api.Runtime.Migration
{
    public class ModifiableBpmnModelInstance : IBpmnModelInstance
    {
        protected internal IBpmnModelInstance modelInstance;

        public ModifiableBpmnModelInstance(IBpmnModelInstance modelInstance)
        {
            this.modelInstance = modelInstance;
        }

        public virtual IDefinitions Definitions
        {
            get { return modelInstance.Definitions; }
            set { modelInstance.Definitions = value; }
        }


        public IBpmnModelInstance Clone()
        {
            return modelInstance.Clone();
        }

        public virtual IDomDocument Document
        {
            get { return modelInstance.Document; }
        }

        public virtual IModelElementInstance DocumentElement
        {
            get { return modelInstance.DocumentElement; }
            set { modelInstance.DocumentElement = value; }
        }


        public virtual T NewInstance<T>(Type type) where T : IModelElementInstance
        {
            return modelInstance.NewInstance<T>(type);
        }

        public virtual T NewInstance<T>(IModelElementType type) where T : IModelElementInstance
        {
            return modelInstance.NewInstance<T>(type);
        }

        public virtual IModel Model
        {
            get { return modelInstance.Model; }
        }

        //public virtual T GetModelElementById<T>(string id) where T : IModelElementInstance
        //{
        //    return modelInstance.GetModelElementById<T>(id);
        //}

        public virtual IModelElementInstance GetModelElementById(string id)
        {
            return modelInstance.GetModelElementById(id);
        }

        public virtual IEnumerable<T> GetModelElementsByType<T>(Type referencingClass) where T : IModelElementInstance
        {
            return modelInstance.GetModelElementsByType<T>(referencingClass);
        }

        IModelInstance IModelInstance.Clone()
        {
            return Clone();
        }

        //public virtual IValidationResults Validate<T1>(ICollection<T1> validators)
        //{
        //    return null;
        //}

        public virtual IValidationResults Validate(ICollection<IModelElementValidator<IModelElementInstance>> validators)
        {
            return null;
        }

        public IModelElementType RegisterGenericType(string namespaceUri, string localName)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Copies the argument; following modifications are not applied to the original model instance
        /// </summary>
        public static ModifiableBpmnModelInstance Modify(IBpmnModelInstance modelInstance)
        {
            return new ModifiableBpmnModelInstance(modelInstance.Clone());
        }

        /// <summary>
        ///     wraps the argument; following modifications are applied to the original model instance
        /// </summary>
        public static ModifiableBpmnModelInstance Wrap(IBpmnModelInstance modelInstance)
        {
            return new ModifiableBpmnModelInstance(modelInstance);
        }

        public virtual IEnumerable<IModelElementInstance> GetModelElementsByType(IModelElementType referencingType)
        {
            return modelInstance.GetModelElementsByType(referencingType);
        }
        
        public virtual T GetBuilderForElementById<T>(string id, Type builderClass)
            //where T : AbstractBaseElementBuilder<,>
        {
            throw new NotImplementedException();
            //IBaseElement modelElementById = modelInstance.GetModelElementById(id);
            //return (T)modelElementById.Builder();
        }

        public virtual AbstractActivityBuilder<TE> ActivityBuilder<TB, TE>(string activityId)
            where TB : AbstractActivityBuilder<TE> where TE : IActivity
        {
            return GetBuilderForElementById<AbstractActivityBuilder<TE>>(activityId,
                typeof(AbstractActivityBuilder<TE>));
        }

        public virtual AbstractFlowNodeBuilder<TE> FlowNodeBuilder<TB, TE>(string flowNodeId)
            where TB : AbstractFlowNodeBuilder<TE> where TE : IFlowNode
        {
            return GetBuilderForElementById<AbstractFlowNodeBuilder<TE>>(flowNodeId,
                typeof(AbstractFlowNodeBuilder<TE>));
        }

        public virtual UserTaskBuilder UserTaskBuilder(string userTaskId)
        {
            return GetBuilderForElementById<UserTaskBuilder>(userTaskId, typeof(UserTaskBuilder));
        }

        public virtual ServiceTaskBuilder ServiceTaskBuilder(string serviceTaskId)
        {
            return GetBuilderForElementById< ServiceTaskBuilder>(serviceTaskId, typeof(ServiceTaskBuilder));
        }

        public virtual CallActivityBuilder CallActivityBuilder(string callActivityId)
        {
            return GetBuilderForElementById<CallActivityBuilder>(callActivityId, typeof(CallActivityBuilder));
        }

        public virtual IntermediateCatchEventBuilder IntermediateCatchEventBuilder(string eventId)
        {
            return GetBuilderForElementById< IntermediateCatchEventBuilder>(eventId, typeof(IntermediateCatchEventBuilder));
        }

        public virtual StartEventBuilder StartEventBuilder(string eventId)
        {
            return GetBuilderForElementById< StartEventBuilder>(eventId, typeof(StartEventBuilder));
        }

        public virtual EndEventBuilder EndEventBuilder(string eventId)
        {
            return GetBuilderForElementById< EndEventBuilder>(eventId, typeof(EndEventBuilder));
        }

        public virtual ModifiableBpmnModelInstance ChangeElementId(string oldId, string newId)
        {
            var element = GetModelElementById(oldId);
            element.Id = newId;
            return this;
        }

        public virtual ModifiableBpmnModelInstance ChangeElementName(string elementId, string newName)
        {
            var flowElement = (IFlowElement)GetModelElementById(elementId);
            flowElement.Name = newName;
            return this;
        }

        public virtual ModifiableBpmnModelInstance RemoveChildren(string elementId)
        {
            var element = GetModelElementById/*<IBaseElement>*/(elementId);

            var children = (ICollection<IBaseElement>) element.GetChildElementsByType(typeof(IBaseElement));
            foreach (var child in children)
                element.RemoveChildElement(child);

            return this;
        }

        public virtual ModifiableBpmnModelInstance RenameMessage(string oldMessageName, string newMessageName)
        {
            var messages = modelInstance.GetModelElementsByType<IMessage>(typeof(IMessage));

            foreach (var message in messages)
                if (message.Name.Equals(oldMessageName))
                    message.Name = newMessageName;

            return this;
        }

        public virtual ModifiableBpmnModelInstance AddDocumentation(string content)
        {
            IEnumerable<IProcess> processes = modelInstance.GetModelElementsByType<IProcess>(typeof(IProcess));
            var documentation = modelInstance.NewInstance<IDocumentation>(typeof(IDocumentation));
            documentation.TextContent = content;
            foreach (IProcess process in processes)
            {
                process.AddChildElement(documentation);
            }
            return this;
        }

        public virtual ModifiableBpmnModelInstance RenameSignal(string oldSignalName, string newSignalName)
        {
            var signals = (ICollection<ISignal>) modelInstance.GetModelElementsByType<ISignal>(typeof(ISignal));

            foreach (var signal in signals)
                if (signal.Name.Equals(oldSignalName))
                    signal.Name = newSignalName;

            return this;
        }

        public virtual ModifiableBpmnModelInstance SwapElementIds(string firstElementId, string secondElementId)
        {
            var firstElement = GetModelElementById/*<IBaseElement>*/(firstElementId);
            var secondElement = GetModelElementById/*<IBaseElement>*/(secondElementId);

            secondElement.Id = "___TEMP___ID___";
            firstElement.Id = secondElementId;
            secondElement.Id = firstElementId;

            return this;
        }

        public virtual SubProcessBuilder AddSubProcessTo(string parentId)
        {
            var eventSubProcess = modelInstance.NewInstance<ISubProcess>(typeof(ISubProcess));

            var parent = (IBpmnModelElementInstance)GetModelElementById/*<IBpmnModelElementInstance>*/(parentId);
            parent.AddChildElement(eventSubProcess);

            return eventSubProcess.Builder();
        }

        public virtual ModifiableBpmnModelInstance RemoveFlowNode(string flowNodeId)
        {
            var flowNode = (IFlowNode)GetModelElementById/*<IFlowNode>*/(flowNodeId);
            var scope = flowNode.ParentElement;

            foreach (var outgoingFlow in flowNode.Outgoing)
            {
                RemoveBpmnEdge(outgoingFlow);
                scope.RemoveChildElement(outgoingFlow);
            }
            foreach (var incomingFlow in flowNode.Incoming)
            {
                RemoveBpmnEdge(incomingFlow);
                scope.RemoveChildElement(incomingFlow);
            }
            var associations = (ICollection<IAssociation>) scope.GetChildElementsByType(typeof(IAssociation));
            foreach (var association in associations)
                if (flowNode.Equals(association.Source) || flowNode.Equals(association.Target))
                {
                    RemoveBpmnEdge(association);
                    scope.RemoveChildElement(association);
                }

            RemoveBpmnShape(flowNode);
            scope.RemoveChildElement(flowNode);

            return this;
        }

        protected internal virtual void RemoveBpmnEdge(IBaseElement element)
        {
            var edges = (ICollection<IBpmnEdge>) modelInstance.GetModelElementsByType< IBpmnEdge>(typeof(IBpmnEdge));
            foreach (var edge in edges)
                if (edge.BpmnElement.Equals(element))
                {
                    var bpmnPlane = edge.ParentElement;
                    bpmnPlane.RemoveChildElement(edge);
                    break;
                }
        }

        protected internal virtual void RemoveBpmnShape(IFlowNode flowNode)
        {
            var bpmnShapes = (ICollection<IBpmnShape>) modelInstance.GetModelElementsByType< IBpmnShape>(typeof(IBpmnShape));
            foreach (var shape in bpmnShapes)
                if (shape.BpmnElement.Equals(flowNode))
                {
                    var bpmnPlane = shape.ParentElement;
                    bpmnPlane.RemoveChildElement(shape);
                    break;
                }
        }

        public virtual ModifiableBpmnModelInstance AsyncBeforeInnerMiActivity(string activityId)
        {
            var activity = modelInstance.GetModelElementById/*<IActivity>*/(activityId);

            var miCharacteristics =
                (IMultiInstanceLoopCharacteristics)
                activity.GetUniqueChildElementByType(typeof(IMultiInstanceLoopCharacteristics));
            miCharacteristics.CamundaAsyncBefore = true;

            return this;
        }

        public virtual ModifiableBpmnModelInstance AsyncAfterInnerMiActivity(string activityId)
        {
            var activity = modelInstance.GetModelElementById/*<IActivity>*/(activityId);

            var miCharacteristics =
                (IMultiInstanceLoopCharacteristics)
                activity.GetUniqueChildElementByType(typeof(IMultiInstanceLoopCharacteristics));
            miCharacteristics.CamundaAsyncAfter = true;

            return this;
        }
        
    }
}