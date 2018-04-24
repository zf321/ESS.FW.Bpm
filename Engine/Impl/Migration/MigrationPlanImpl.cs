using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public class MigrationPlanImpl : IMigrationPlan
    {
        protected internal IList<IMigrationInstruction> instructions;

        protected internal string sourceProcessDefinitionId;
        protected internal string targetProcessDefinitionId;

        public MigrationPlanImpl(string sourceProcessDefinitionId, string targetProcessDefinitionId)
        {
            this.sourceProcessDefinitionId = sourceProcessDefinitionId;
            this.targetProcessDefinitionId = targetProcessDefinitionId;
            instructions = new List<IMigrationInstruction>();
        }

        public virtual string SourceProcessDefinitionId
        {
            get { return sourceProcessDefinitionId; }
            set { sourceProcessDefinitionId = value; }
        }


        public virtual string TargetProcessDefinitionId
        {
            get { return targetProcessDefinitionId; }
            set { targetProcessDefinitionId = value; }
        }


        public virtual IList<IMigrationInstruction> Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }


        public override string ToString()
        {
            return "MigrationPlan[" + "sourceProcessDefinitionId='" + sourceProcessDefinitionId + '\'' +
                   ", targetProcessDefinitionId='" + targetProcessDefinitionId + '\'' + ", instructions=" + instructions +
                   ']';
        }
    }
}