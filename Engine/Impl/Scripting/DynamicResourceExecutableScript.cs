using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class DynamicResourceExecutableScript : DynamicExecutableScript
    {
        public DynamicResourceExecutableScript(String language, IExpression scriptResourceExpression):base(scriptResourceExpression,language)
        {
        }

        public override String GetScriptSource(IVariableScope variableScope)
        {
            String scriptPath = EvaluateExpression(variableScope);
            return ResourceUtil.LoadResourceContent(scriptPath, getDeployment());
        }

        protected DeploymentEntity getDeployment()
        {
            return Context.BpmnExecutionContext.Deployment;
        }
    }
}
