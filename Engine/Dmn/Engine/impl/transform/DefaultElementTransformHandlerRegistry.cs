using System.Collections.Generic;
using System.Data;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;
using Type = System.Type;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DefaultElementTransformHandlerRegistry<TSource, TArget> : IDmnElementTransformHandlerRegistry
        where TSource : IDmnModelElementInstance
    {
        //protected internal static readonly IDictionary<Type, IDmnElementTransformHandler<Source, Target>> handlers =DefaultElementTransformHandlers;
        protected internal static readonly IDictionary<Type, object> Handlers = DefaultElementTransformHandlers;

        protected internal static IDictionary<Type, object> DefaultElementTransformHandlers
        {
            get
            {
                IDictionary<Type, object> handlers =
                    new Dictionary<Type, object>();
                handlers[typeof(IDefinitions)] = new DmnDecisionRequirementsGraphTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IDecision)] = new DmnDecisionTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)

                handlers[typeof(IDecisionTable)] = new DmnDecisionTableTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IInput)] = new DmnDecisionTableInputTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IInputExpression)] = new DmnDecisionTableInputExpressionTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IOutput)] = new DmnDecisionTableOutputTransformHandler();
                // (IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IRule)] = new DmnDecisionTableRuleTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IInputEntry)] = new DmnDecisionTableConditionTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IOutputEntry)] = new DmnLiternalExpressionTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(ILiteralExpression)] = new DmnLiternalExpressionTransformHandler();
                //(IDmnElementTransformHandler<Source, Target>)
                handlers[typeof(IVariable)] = new DmnVariableTransformHandler();
                // (IDmnElementTransformHandler<Source, Target>)

                return handlers;
            }
        }

        //protected internal static IDictionary<Type, IDmnElementTransformHandler<Source, Target>> DefaultElementTransformHandlers
        //{
        //    get
        //    {
        //        IDictionary<Type, IDmnElementTransformHandler<Source, Target>> handlers =
        //            new Dictionary<Type, IDmnElementTransformHandler<Source, Target>>();
        //        handlers[typeof (Definitions)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionRequirementsGraphTransformHandler();
        //        handlers[typeof (Decision)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTransformHandler();

        //        handlers[typeof (DecisionTable)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableTransformHandler();
        //        handlers[typeof (Input)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableInputTransformHandler();
        //        handlers[typeof (InputExpression)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableInputExpressionTransformHandler();
        //        handlers[typeof (Output)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableOutputTransformHandler();
        //        handlers[typeof (Rule)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableRuleTransformHandler();
        //        handlers[typeof (InputEntry)] = (IDmnElementTransformHandler<Source, Target>)new DmnDecisionTableConditionTransformHandler();
        //        handlers[typeof (OutputEntry)] = (IDmnElementTransformHandler<Source, Target>)new DmnLiternalExpressionTransformHandler();

        //        handlers[typeof (LiteralExpression)] = (IDmnElementTransformHandler<Source, Target>)new DmnLiternalExpressionTransformHandler();
        //        handlers[typeof (Variable)] = (IDmnElementTransformHandler<Source, Target>)new DmnVariableTransformHandler();

        //        return handlers;
        //    }
        //}
        /// <summary>
        ///     model.dmn.instance.Type已转换位System.Type
        /// </summary>
        /// <typeparam name="TSource1"></typeparam>
        /// <param name="sourceClass"></param>
        /// <param name="handler"></param>
        /// <summary>
        ///     model.dmn.instance.Type已转换位System.Type
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="sourceClass"></param>
        /// <param name="handler"></param>
        //public void addHandler<Source>(System.Type sourceClass, IDmnElementTransformHandler<Source, Target> handler) where Source : IDmnModelElementInstance
        //{
        //    throw new NotImplementedException();
        //}
        void IDmnElementTransformHandlerRegistry.addHandler<TSource1, TArget1>(Type sourceClass,
            IDmnElementTransformHandler<TSource1, TArget1> handler)
        {
            Handlers[sourceClass] = /*(IDmnElementTransformHandler<TSource, TArget>)*/ handler;
        }

        IDmnElementTransformHandler<TSource1, TArget1> IDmnElementTransformHandlerRegistry.getHandler<TSource1, TArget1>(
            Type sourceClass)
        {
            return (IDmnElementTransformHandler<TSource1, TArget1>) Handlers[sourceClass];
        }


        //IDmnElementTransformHandler<Source1, model.bpmn.impl.instance.Target> IDmnElementTransformHandlerRegistry.getHandler<Source1>(System.Type sourceClass)
        //{
        //    return handlers[sourceClass];

        //}
    }
}