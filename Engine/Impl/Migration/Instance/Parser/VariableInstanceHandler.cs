using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class VariableInstanceHandler :
        IMigratingDependentInstanceParseHandler<MigratingProcessElementInstance, IList<VariableInstanceEntity>>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext,
            MigratingProcessElementInstance owningInstance, IList<VariableInstanceEntity> variables)
        {
            var representativeExecution = owningInstance.ResolveRepresentativeExecution();

            foreach (var variable in variables)
                parseContext.Consume(variable);
        }
    }
}