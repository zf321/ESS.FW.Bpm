
using System;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableInstanceEntityFactory : IVariableInstanceFactory<VariableInstanceEntity>
	{

	  public static readonly VariableInstanceEntityFactory Instance = new VariableInstanceEntityFactory();

        public  VariableInstanceEntity Build(string name, ITypedValue value, bool isTransient)
	  {
		return VariableInstanceEntity.Create(name, value, isTransient);
	  }

	}

}