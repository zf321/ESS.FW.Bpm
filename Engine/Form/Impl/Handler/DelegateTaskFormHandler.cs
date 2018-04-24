using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class DelegateTaskFormHandler : DelegateFormHandler, ITaskFormHandler
    {
        public DelegateTaskFormHandler(ITaskFormHandler formHandler, DeploymentEntity deployment)
            : base(formHandler, deployment.Id)
        {
        }

        public override IFormHandler FormHandler
        {
            get { return (ITaskFormHandler) formHandler; }
        }

        public ITaskFormData CreateTaskForm(TaskEntity ITask)
        {
            throw new NotImplementedException();
        }
        

        public void SubmitFormVariables(IVariableMap properties, IVariableScope variableScope)
        {
            throw new NotImplementedException();
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public org.camunda.bpm.engine.form.TaskFormData createTaskForm(final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity ITask)
        //public virtual TaskFormData createTaskForm(TaskEntity ITask)
        //{

        //    return performContextSwitch(new CallableAnonymousInnerClass(this, ITask));
        //}
    }
}