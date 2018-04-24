using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Bpmn.ServiceTask
{
    /// <summary>
    /// </summary>
    public class ModelExecutionContextServiceTask : IJavaDelegate
    {
        public static IBpmnModelInstance ModelInstance;
        public static IServiceTask ServiceTask;

        /// <summary>
        ///     有可能会报错转换失败
        /// </summary>
        /// <param name="execution"></param>
        public void Execute(IBaseDelegateExecution execution)
        {
            var delexecution = (IDelegateExecution) execution;
            ModelInstance = delexecution.BpmnModelInstance; // IBpmnModelInstance;//  IExecution.IBpmnModelInstance;
            ServiceTask = (IServiceTask) delexecution.BpmnModelElementInstance; //BpmnModelElementInstance;
        }


        public static void Clear()
        {
            ModelInstance = null;
            ServiceTask = null;
        }
    }
}