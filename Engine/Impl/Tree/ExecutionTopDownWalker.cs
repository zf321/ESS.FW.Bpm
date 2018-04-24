using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///      
    /// </summary>
    public class ExecutionTopDownWalker : ReferenceWalker<ExecutionEntity>
    {
        public ExecutionTopDownWalker(ExecutionEntity initialElement) : base(initialElement)
        {
        }

        public ExecutionTopDownWalker(IList<ExecutionEntity> initialElements) : base(initialElements)
        {
        }

        protected internal override ICollection<ExecutionEntity> NextElements()
        {
            ICollection<ExecutionEntity> result = new List<ExecutionEntity>();
            var executions = CurrentElement.Executions;
            if (executions == null)
                return result;

            foreach(var item in executions)
            {
                result.Add(item as ExecutionEntity);
            }
            return result;
        }
    }
}