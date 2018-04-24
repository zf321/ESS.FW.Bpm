using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ModelExecutionContextTaskListener : ITaskListener
    {

        public static IBpmnModelInstance ModelInstance;
        // public static IUserTask userTask;
        public static IFlowElement UserTask;

        public virtual void Notify(IDelegateTask delegateTask)
        {
            ModelInstance = delegateTask.BpmnModelInstance;
            UserTask = delegateTask.BpmnModelElementInstance;
        }

        public static void Clear()
        {
            UserTask = null;
            ModelInstance = null;
        }

    }

}