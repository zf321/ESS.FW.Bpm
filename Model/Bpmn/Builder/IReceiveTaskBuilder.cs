using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IReceiveTaskBuilder: ITaskBuilder<IReceiveTask>
    {
        IReceiveTaskBuilder Implementation(string implementation);
        IReceiveTaskBuilder Instantiate();
        IReceiveTaskBuilder Message(IMessage message);
        IReceiveTaskBuilder Message(string messageName);
        IReceiveTaskBuilder Operation(IOperation operation);
    }
}