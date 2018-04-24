using ESS.FW.Bpm.Model.Bpmn.instance;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// 基础接口
    /// </summary>
    /// <typeparam name="TB"></typeparam>
    /// <typeparam name="TE"></typeparam>
    public interface IBpmnModelElementBuilder<in TE> where TE: IBpmnModelElementInstance
    {
        IBpmnModelInstance Done();
        SubProcessBuilder SubProcessDone();
        TransactionBuilder TransactionDone();
    }
}